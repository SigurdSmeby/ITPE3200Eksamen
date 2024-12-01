using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.Models;
using server.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.IO;

namespace server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly IConfiguration _configuration;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly ILogger<UsersController> _logger;

		// Constructor initializes dependencies for database, configuration, and web host environment        

		public UsersController(AppDbContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, ILogger<UsersController> logger)
		{
			_context = context;
			_configuration = configuration;
			_webHostEnvironment = webHostEnvironment;
			_logger = logger;
		}

		// POST: api/Users/register
		// Registers a new user by validating the username and email, hashing the password, and saving the user in the database.
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
		{
			_logger.LogInformation("Attempting to register user with username: {Username}", registerDto.Username);
			// Check for duplicate username or email
			if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
			{
				_logger.LogWarning("Registration failed: Username {Username} is already taken.", registerDto.Username);
				return BadRequest("Username is already taken.");
			}

			if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
			{
				_logger.LogWarning("Registration failed: Email {Email} is already registered.", registerDto.Email);
				return BadRequest("Email is already registered.");
			}

			// Create the new user object
			var user = new User
			{
				Username = registerDto.Username,
				Email = registerDto.Email,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
				ProfilePictureUrl = "/uploads/profile_pictures/default_profile.jpg",
			};

			// Save the user to the database
			_context.Users.Add(user);
			await _context.SaveChangesAsync();
			_logger.LogInformation("User {Username} registered successfully.", registerDto.Username);

			return Ok("Registration successful.");
		}

		// POST: api/Users/login
		// Authenticates a user by verifying credentials and generates a JWT token upon success.
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
		{
			_logger.LogInformation("User login attempt for username: {Username}", loginDto.Username);
			// Retrieve the user by username
			var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == loginDto.Username);

			// Validate the user's password
			if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
			{
				_logger.LogWarning("Login failed for username: {Username}. Invalid credentials.", loginDto.Username);
				return Unauthorized("Invalid username or password.");
			}

			// Generate a JWT token for the user
			var token = GenerateJwtToken(user);
			_logger.LogInformation("User {Username} logged in successfully.", loginDto.Username);
			return Ok(new { Token = token });
		}

		// GET: api/Users/all
		// Retrieves all users, returning their usernames and profile pictures.
		[HttpGet("all")]
		public async Task<IActionResult> GetAllUsers()
		{
			_logger.LogInformation("Fetching all users.");
			// Query the database for all users
			var users = await _context.Users
				.Select(u => new
				{
					u.Username,
					u.ProfilePictureUrl
				})
				.ToListAsync();
			_logger.LogInformation("Fetched {UserCount} users.", users.Count);
			return Ok(users);
		}

		// GET: api/Users/profile
		// Retrieves the profile of the currently logged-in user, including user details and post count.
		[Authorize]
		[HttpGet("profile")]
		public async Task<IActionResult> GetProfile()
		{
			int userId = GetCurrentUserId();
			_logger.LogInformation("Fetching profile for UserId: {UserId}", userId);

			// Fetch user details and associated posts
			var user = await _context.Users
				.Include(u => u.Posts)
				.SingleOrDefaultAsync(u => u.UserId == userId);

			if (user == null)
			{
				_logger.LogWarning("Profile fetch failed: UserId {UserId} not found.", userId);
				return NotFound();
			}

			// Map user details to a DTO for response
			var userDto = new UserProfileDto
			{
				UserId = user.UserId,
				Username = user.Username,
				Email = user.Email,
				ProfilePictureUrl = user.ProfilePictureUrl,
				DateJoined = user.DateJoined,
				PostCount = user.Posts.Count,
				Bio = user.Bio
			};
			_logger.LogInformation("Profile for UserId {UserId} fetched successfully.", userId);

			return Ok(userDto);
		}

		// PUT: api/Users/profile
		// Updates the currently logged-in user's profile, including username, email, bio, and profile picture.
		[Authorize]
		[HttpPut("profile")]
		public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto updateProfileDto)
		{
			int userId = GetCurrentUserId();
			_logger.LogInformation("UserId {UserId} attempting to update profile.", userId);

			var user = await _context.Users.FindAsync(userId);
			if (user == null)
			{
				_logger.LogWarning("Profile update failed: UserId {UserId} not found.", userId);
				return NotFound();
			}

			// Update username if provided and unique
			if (!string.IsNullOrEmpty(updateProfileDto.Username))
			{
				if (await _context.Users.AnyAsync(u => u.Username == updateProfileDto.Username && u.UserId != userId))
				{
					_logger.LogWarning("Profile update failed: Username {Username} is already taken.", updateProfileDto.Username);
					return BadRequest("Username is already taken.");
				}
				user.Username = updateProfileDto.Username;
				_logger.LogInformation("Username updated for UserId {UserId}.", userId);
			}

			// Update email if provided and unique
			if (!string.IsNullOrEmpty(updateProfileDto.Email))
			{
				if (await _context.Users.AnyAsync(u => u.Email == updateProfileDto.Email && u.UserId != userId))
				{
					_logger.LogWarning("Profile update failed: Email {Email} is already registered.", updateProfileDto.Email);
					return BadRequest("Email is already registered.");
				}
				_logger.LogInformation("Email updated for UserId {UserId}.", userId);
				user.Email = updateProfileDto.Email;
			}

			// Handle profile picture upload
			if (updateProfileDto.ProfilePicture != null && updateProfileDto.ProfilePicture.Length > 0)
			{
				_logger.LogInformation("Processing profile picture upload for UserId: {UserId}.", userId);

				var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/profile_pictures");
				if (!Directory.Exists(uploadsFolder))
					Directory.CreateDirectory(uploadsFolder);

				var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(updateProfileDto.ProfilePicture.FileName);
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				// Delete the old profile picture if it is not the default
				if (!string.IsNullOrEmpty(user.ProfilePictureUrl) && !user.ProfilePictureUrl.Contains("default_profile.jpg"))
				{
					var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfilePictureUrl.TrimStart('/'));
					if (System.IO.File.Exists(oldFilePath))
					{
						_logger.LogInformation("Deleted old profile picture for UserId: {UserId}.", userId);
						System.IO.File.Delete(oldFilePath);
					}
				}

				// Save the new profile picture
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await updateProfileDto.ProfilePicture.CopyToAsync(fileStream);
				}

				user.ProfilePictureUrl = "/uploads/profile_pictures/" + uniqueFileName;
				_logger.LogInformation("Profile picture updated for UserId: {UserId}.", userId);
			}

			// Update bio if provided
			if (!string.IsNullOrEmpty(updateProfileDto.Bio))
			{
				user.Bio = updateProfileDto.Bio;
				_logger.LogInformation("Bio updated for UserId: {UserId}.", userId);
			}

			// Save changes to the database
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
			_logger.LogInformation("Profile updated successfully for UserId: {UserId}.", userId);

			return Ok(new { success = true, message = "Profile updated successfully." });
		}

		// PUT: api/Users/change-password
		// Change the password of the currently logged-in user
		[Authorize]
		[HttpPut("change-password")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto passwordDto)
		{
			_logger.LogInformation("User is attempting to change password.");
			int userId = GetCurrentUserId();
			_logger.LogInformation("Retrieved UserId: {UserId} for password change.", userId);

			var user = await _context.Users.FindAsync(userId);
			if (user == null)
			{
				_logger.LogWarning("ChangePassword failed: UserId {UserId} not found.", userId);
				return NotFound();
			}
			// Verify the current password
			if (!BCrypt.Net.BCrypt.Verify(passwordDto.CurrentPassword, user.PasswordHash))
			{
				_logger.LogWarning("ChangePassword failed: Incorrect current password for UserId {UserId}.", userId);
				return BadRequest("Current password is incorrect.");
			}
			// Update password hash
			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordDto.NewPassword);
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
			_logger.LogInformation("Password successfully changed for UserId {UserId}.", userId);

			return Ok("Password changed successfully.");
		}
		// DELETE: api/Users/delete-account
		// Delete the currently logged-in user's account and associated data
		[Authorize]
		[HttpDelete("delete-account")]
		public async Task<IActionResult> DeleteAccount()
		{
			_logger.LogInformation("User is attempting to delete their account.");
			int userId = GetCurrentUserId();
			_logger.LogInformation("Retrieved UserId: {UserId} for account deletion.", userId);
			// Fetch user and their posts
			var user = await _context.Users
				.Include(u => u.Posts)
				.FirstOrDefaultAsync(u => u.UserId == userId);
			if (user == null)
			{
				_logger.LogWarning("DeleteAccount failed: UserId {UserId} not found.", userId);
				return NotFound();
			}
			// Delete profile picture if not default
			if (!string.IsNullOrEmpty(user.ProfilePictureUrl) && !user.ProfilePictureUrl.Contains("default_profile.jpg"))
			{
				var profilePicturePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfilePictureUrl.TrimStart('/'));
				if (System.IO.File.Exists(profilePicturePath))

				{
					System.IO.File.Delete(profilePicturePath);
					_logger.LogInformation("Deleted profile picture for UserId: {UserId}.", userId);
				}
			}
			// Delete all post images
			foreach (var post in user.Posts)
			{
				if (!string.IsNullOrEmpty(post.ImagePath))
				{
					var postImagePath = Path.Combine(_webHostEnvironment.WebRootPath, post.ImagePath.TrimStart('/'));
					if (System.IO.File.Exists(postImagePath))
					{
						System.IO.File.Delete(postImagePath);
						_logger.LogInformation("Deleted image for PostId: {PostId} of UserId: {UserId}.", post.PostId, userId);
					}
				}
			}
			// Remove user and save changes
			_context.Users.Remove(user);
			await _context.SaveChangesAsync();
			_logger.LogInformation("Account and associated data successfully deleted for UserId {UserId}.", userId);

			return Ok("Account and associated data deleted successfully.");
		}

		// Helper method: Retrieves the ID of the currently logged-in user from JWT claims.
		private int GetCurrentUserId()
		{
			var userIdClaim = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User ID claim is missing."));
			return userIdClaim;
		}

		// Helper method: Generates a JWT token for user authentication.
		private string GenerateJwtToken(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? "DefaultSecretKey");

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
					new Claim(ClaimTypes.Name, user.Username)
				}),
				Expires = DateTime.Now.AddHours(2),
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
