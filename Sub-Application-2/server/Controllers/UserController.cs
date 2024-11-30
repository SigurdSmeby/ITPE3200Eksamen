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

        // Constructor initializes dependencies for database, configuration, and web host environment
        public UsersController(AppDbContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        // POST: api/Users/register
        // Registers a new user by validating the username and email, hashing the password, and saving the user in the database.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Check for duplicate username or email
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                return BadRequest("Username is already taken.");

            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                return BadRequest("Email is already registered.");

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

            return Ok("Registration successful.");
        }

        // POST: api/Users/login
        // Authenticates a user by verifying credentials and generates a JWT token upon success.
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Retrieve the user by username
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == loginDto.Username);

            // Validate the user's password
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return Unauthorized("Invalid username or password.");

            // Generate a JWT token for the user
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        // GET: api/Users/all
        // Retrieves all users, returning their usernames and profile pictures.
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            // Query the database for all users
            var users = await _context.Users
                .Select(u => new
                {
                    u.Username,
                    u.ProfilePictureUrl
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/Users/profile
        // Retrieves the profile of the currently logged-in user, including user details and post count.
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            int userId = GetCurrentUserId();

            // Fetch user details and associated posts
            var user = await _context.Users
                .Include(u => u.Posts)
                .SingleOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound();

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

            return Ok(userDto);
        }

        // PUT: api/Users/profile
        // Updates the currently logged-in user's profile, including username, email, bio, and profile picture.
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

                // Delete the old profile picture if it is not the default
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl) && !user.ProfilePictureUrl.Contains("default_profile.jpg"))
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfilePictureUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                // Save the new profile picture
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
