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
        [HttpGet]
        public IActionResult Profile()
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

            if (!ModelState.IsValid)
            {
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(loginDto.Username, loginDto.Password, isPersistent: false, lockoutOnFailure: false);
            
            
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
            var user = await _userManager.GetUserAsync(User);

            if (user == null){
                ModelState.AddModelError("User", "User not found");
                Console.WriteLine("User not found");
                return View();
            }
            //TODO: Se ann om vi trenger å sende me all infoen
            var userDto = new UserProfileDto
            {
                Username = user.UserName,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Bio = user.Bio
            };

            return View(userDto);
        }

        // 
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserProfileDto userProfileDto)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null){
                ModelState.AddModelError("User", "User not found");
                return View("Settings", userProfileDto);
            }
            if (!ModelState.IsValid)
            {
                return View("Settings", userProfileDto);
            }
                user.UserName = userProfileDto.Username;
                user.Email = userProfileDto.Email;
                user.Bio = userProfileDto.Bio; 

            // Handle profile picture upload
            if (userProfileDto.ProfilePicture != null && userProfileDto.ProfilePicture.Length > 0)
            {
                // TODO: maybe unnececary consider removal
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/profile_pictures");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }


                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(userProfileDto.ProfilePicture.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                if (user.ProfilePictureUrl != "/images/default_profile.jpg")
                {
                    // Delete the old profile picture
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfilePictureUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await userProfileDto.ProfilePicture.CopyToAsync(fileStream);
                }

                // Update the user's profile picture URL to the relative path
                user.ProfilePictureUrl = "/uploads/profile_pictures/" + uniqueFileName;
                userProfileDto.ProfilePictureUrl = user.ProfilePictureUrl;
            }
            var result = await _userManager.UpdateAsync(user);
            
            //TODO: Add error handling

            ViewBag.SuccessMessage = "Profile updated successfully.";
            return View("Settings", userProfileDto);
        }

        // PUT: api/Users/change-password
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserProfileDto userProfileDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null){
                ModelState.AddModelError("User", "User not found");
                return View("Settings", userProfileDto);
            }
            if (userProfileDto.NewPassword != userProfileDto.ConfirmPassword) {
                ViewData["PasswordError"] = "The passwords do not match";
                return View("Settings", userProfileDto);
            }
            var results = await _userManager.ChangePasswordAsync(user, userProfileDto.CurrentPassword, userProfileDto.NewPassword);
            if (results.Succeeded){
                ViewData["PasswordSuccess"] = "Your password is updated";
            }
            else {
                var errorMessages = string.Join("<br/>", results.Errors.Select(e => e.Description));
                ViewData["PasswordError"] = errorMessages;
            }
            return View("Settings", userProfileDto);
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
