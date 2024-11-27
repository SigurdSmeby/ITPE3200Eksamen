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

        public UsersController(AppDbContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        // POST: api/Users/register
        // Register a new user with a username, email, and password
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Ensure username and email are unique
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                return BadRequest("Username is already taken.");

            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                return BadRequest("Email is already registered.");

            // Hash the password and set the default profile picture
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

            return Ok("Registration successful.");
        }

        // POST: api/Users/login
        // Authenticate a user and return a JWT token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Verify user credentials
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return Unauthorized("Invalid username or password.");

            // Generate and return a JWT token
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        // GET: api/Users/profile
        // Retrieve the profile of the currently logged-in user
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            int userId = GetCurrentUserId();

            // Fetch user details and their posts
            var user = await _context.Users
                .Include(u => u.Posts)
                .SingleOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound();

            // Map user data to a DTO
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

            return Ok(userDto);
        }

        // PUT: api/Users/profile
        // Update the profile of the currently logged-in user
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto updateProfileDto)
        {
            int userId = GetCurrentUserId();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            // Update username if provided and unique
            if (!string.IsNullOrEmpty(updateProfileDto.Username))
            {
                if (await _context.Users.AnyAsync(u => u.Username == updateProfileDto.Username && u.UserId != userId))
                    return BadRequest("Username is already taken.");
                user.Username = updateProfileDto.Username;
            }

            // Update email if provided and unique
            if (!string.IsNullOrEmpty(updateProfileDto.Email))
            {
                if (await _context.Users.AnyAsync(u => u.Email == updateProfileDto.Email && u.UserId != userId))
                    return BadRequest("Email is already registered.");
                user.Email = updateProfileDto.Email;
            }

            // Handle profile picture upload
            if (updateProfileDto.ProfilePicture != null && updateProfileDto.ProfilePicture.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/profile_pictures");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(updateProfileDto.ProfilePicture.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Delete old profile picture if not default
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl) && !user.ProfilePictureUrl.Contains("default_profile.jpg"))
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfilePictureUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                // Save new profile picture
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await updateProfileDto.ProfilePicture.CopyToAsync(fileStream);
                }

                user.ProfilePictureUrl = "/uploads/profile_pictures/" + uniqueFileName;
            }

            // Update bio if provided
            if (!string.IsNullOrEmpty(updateProfileDto.Bio))
                user.Bio = updateProfileDto.Bio;

            // Save changes to the database
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Profile updated successfully." });
        }

        // PUT: api/Users/change-password
        // Change the password of the currently logged-in user
        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto passwordDto)
        {
            int userId = GetCurrentUserId();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            // Verify the current password
            if (!BCrypt.Net.BCrypt.Verify(passwordDto.CurrentPassword, user.PasswordHash))
                return BadRequest("Current password is incorrect.");

            // Update password hash
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordDto.NewPassword);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Password changed successfully.");
        }

        // DELETE: api/Users/delete-account
        // Delete the currently logged-in user's account and associated data
        [Authorize]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            int userId = GetCurrentUserId();

            // Fetch user and their posts
            var user = await _context.Users
                .Include(u => u.Posts)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound();

            // Delete profile picture if not default
            if (!string.IsNullOrEmpty(user.ProfilePictureUrl) && !user.ProfilePictureUrl.Contains("default_profile.jpg"))
            {
                var profilePicturePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfilePictureUrl.TrimStart('/'));
                if (System.IO.File.Exists(profilePicturePath))
                    System.IO.File.Delete(profilePicturePath);
            }

            // Delete all post images
            foreach (var post in user.Posts)
            {
                if (!string.IsNullOrEmpty(post.ImagePath))
                {
                    var postImagePath = Path.Combine(_webHostEnvironment.WebRootPath, post.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(postImagePath))
                        System.IO.File.Delete(postImagePath);
                }
            }

            // Remove user and save changes
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("Account and associated data deleted successfully.");
        }

        // Retrieve the ID of the currently logged-in user
        private int GetCurrentUserId()
        {
            var userIdClaim = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User ID claim is missing."));
            return userIdClaim;
        }

        // Generate a JWT token for a user
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
