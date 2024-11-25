using Microsoft.AspNetCore.Mvc;
using Sub_Application_1.Data;
using Sub_Application_1.Models;
using Sub_Application_1.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

namespace Sub_Application_1.Controllers
{
    
    public class UsersController : Controller
    { 
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        //Constructor
        public UsersController(
            AppDbContext context, 
            IConfiguration configuration, 
            IWebHostEnvironment webHostEnvironment, 
            SignInManager<User> signInManager, 
            UserManager<User> userManager)
        {
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _signInManager = signInManager;
            _userManager = userManager;
        }   
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    




        // Post register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }
            if (registerDto.Password != registerDto.confirmPassword)
            {
                ModelState.AddModelError("Password", "Passwords do not match.");
                return View(registerDto);
            }
            var user = new User
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                };
            
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            Console.WriteLine("Result: " + result);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {   
                Console.WriteLine("Error: " + error.Description);
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(registerDto);
        }

        // 
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == loginDto.Username);

            if (!ModelState.IsValid)
            {
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(loginDto.Username, loginDto.Password, isPersistent: true, lockoutOnFailure: false);
            
            
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }    else if (result.IsLockedOut)
            {
                ModelState.AddModelError("Login", "This account is locked out. Please try again later.");
            }
            else
            {
                ModelState.AddModelError("Login", "Invalid username or password.");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            String userId = GetCurrentUserId();

            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null){
                ModelState.AddModelError("User", "User not found");
                return View();
            }
            var userDto = new UserProfileDto
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl,
                DateJoined = user.DateJoined,
                PostCount = user.Posts.Count,
                Bio = user.Bio
            };

            return View(userDto);
        }

        // 
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Settings(IFormFile? profilePicture,  string? username,  string? email, string? bio)
        {
            String userId = GetCurrentUserId();

            var user = await _context.Users.FindAsync(userId);

            if (user == null){
                ModelState.AddModelError("User", "User not found");
                return View();
                }

            // Update username if provided
            if (!string.IsNullOrEmpty(username))
            {
                // Ensure the new username isn't taken by someone else
                if (await _context.Users.AnyAsync(u => u.UserName == username && u.Id != userId))
                {
                    ModelState.AddModelError("User", "Username is already taken");
                    return View();
                }
                user.UserName = username;
            }

            // Update email if provided
            if (!string.IsNullOrEmpty(email))
            {
                // Ensure the new email isn't already registered to another user
                if (await _context.Users.AnyAsync(u => u.Email == email && u.Id != userId))
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View();
                }
                user.Email = email;
            }

            // Handle profile picture upload
            if (profilePicture != null && profilePicture.Length > 0)
            {
                // TODO: maybe unnececary consider removal
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(profilePicture.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(fileStream);
                }

                // Update the user's profile picture URL to the relative path
                user.ProfilePictureUrl = "/uploads/" + uniqueFileName;
            }

            // Update bio if provided
            if (!string.IsNullOrEmpty(bio))
            {
                user.Bio = bio;
            }

            // Save changes to the database
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            ViewBag.SuccessMessage = "Profile updated successfully.";
            return View();
        }

        // PUT: api/Users/change-password
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto passwordDto)
        {
            String userId = GetCurrentUserId();

            var user = await _context.Users.FindAsync(userId);

            if (user == null){
                ModelState.AddModelError("User", "User not found");
                return RedirectToAction("Settings");
        }
            if (!BCrypt.Net.BCrypt.Verify(passwordDto.CurrentPassword, user.PasswordHash))
            {
                ModelState.AddModelError("password", "Current password is incorrect");
                return RedirectToAction("Settings");

            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordDto.NewPassword);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Password updated successfully.";
            return RedirectToAction("Settings");
        }

        // DELETE: api/Users/delete-account
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            String userId = GetCurrentUserId();

            var user = await _context.Users.FindAsync(userId);

            if (user == null){
                ModelState.AddModelError("User", "User not found");
                return RedirectToAction("Settings");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Account deleted successfully.";
            return RedirectToAction("Login");

        }

        // Helper methods
		private String GetCurrentUserId()
		{
			return User.FindFirstValue(ClaimTypes.NameIdentifier);
		}

        private string GenerateJwtToken(User user)
        {
            return "hei";
        }
    }
}
