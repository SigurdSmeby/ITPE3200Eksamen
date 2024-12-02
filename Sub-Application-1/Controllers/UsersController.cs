using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sub_Application_1.DTOs;
using Sub_Application_1.Models;
using Sub_Application_1.Repositories.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Sub_Application_1.Controllers
{
	public class UsersController : Controller
	{
		private readonly IUserRepository _userRepository;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly ILogger<UsersController> _logger;
		// Constructor initializes repository for user management, and iwebhost fir file management
		public UsersController(IUserRepository userRepository, IWebHostEnvironment webHostEnvironment, ILogger<UsersController> logger)
		{
			_userRepository = userRepository;
			_webHostEnvironment = webHostEnvironment;
			_logger = logger;
		}
		public IActionResult Register()
		{
			_logger.LogInformation("Register GET action called.");
			return View();
		}
		[HttpGet]
		public IActionResult Login()
		{
			_logger.LogInformation("Login GET action called.");
			return View();
		}
		[HttpGet]
		public IActionResult Profile()
		{
			_logger.LogInformation("Profile GET action called.");
			return View();
		}

		// Post register
		[HttpPost]
		public async Task<IActionResult> Register(RegisterDto registerDto)
		{
			_logger.LogInformation("Register POST action called.");

			if (registerDto.Password != registerDto.confirmPassword)
			{
				_logger.LogWarning("Passwords do not match for registration.");
				ModelState.AddModelError("Password", "Passwords do not match.");
			}
			var existingUserByUsername = await _userRepository.GetUserByUsernameAsync(registerDto.Username);
			if (existingUserByUsername != null)
			{
				_logger.LogWarning("Username {Username} already exists.", registerDto.Username);
				ModelState.AddModelError("Username", "Username already exists.");
			}
			var existingUserByEmail = await _userRepository.GetUserByEmailAsync(registerDto.Email);
			if (existingUserByEmail != null)
			{
				_logger.LogWarning("Email {Email} already registered.", registerDto.Email);
				ModelState.AddModelError("Email", "Email already registered.");
			}
			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Model state invalid for registration.");
				return View(registerDto);
			}


			var result = await _userRepository.RegisterUserAsync(registerDto);
			Console.WriteLine("Result: " + result);
			if (result.Succeeded)
			{
				_logger.LogInformation("User {Username} registered successfully.", registerDto.Username);
				return RedirectToAction("Index", "Home"); // Successful registration.
			}

			foreach (var error in result.Errors)
			{
				_logger.LogWarning("Registration error: {Error}", error.Description);
				Console.WriteLine("Error: " + error.Description);
				ModelState.AddModelError(string.Empty, error.Description);
			}


			return View(registerDto); // Unuccessful registration.
		}

		// Post login
		[HttpPost]
		public async Task<IActionResult> Login(LoginDto loginDto)
		{
			_logger.LogInformation("Login POST action called.");

			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Model state invalid for login.");
				return View();
			}

			var result = await _userRepository.LoginAsync(loginDto);

			if (result.Succeeded)
			{
				_logger.LogInformation("User {Username} logged in successfully.", loginDto.Username);
				return RedirectToAction("Index", "Home");
			}
			else if (result.IsLockedOut)
			{
				_logger.LogWarning("User {Username} is locked out.", loginDto.Username);
				ModelState.AddModelError("Login", "This account is locked out. Please try again later.");
			}
			else
			{
				// error message purposely vague to prevent brute force attacks
				_logger.LogWarning("Invalid login attempt for username {Username}.", loginDto.Username);
				ModelState.AddModelError("Login", "Invalid username or password.");
			}
			return View();
		}
		[HttpGet]
		public async Task<IActionResult> Logout()
		{
			_logger.LogInformation("Logout action called.");
			await _userRepository.LogoutAsync();
			_logger.LogInformation("User logged out successfully.");
			return RedirectToAction("Index", "Home");
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> Settings()
		{
			_logger.LogInformation("Settings GET action called.");
			var user = await _userRepository.GetUserAsync(User);
			if (user == null)
			{
				_logger.LogWarning("User not found for settings.");
				ModelState.AddModelError("User", "User not found");
				Console.WriteLine("User not found");
				return View();
			}

			var userDto = new UserProfileDto
			{
				Username = user.UserName,
				Email = user.Email,
				ProfilePictureUrl = user.ProfilePictureUrl,
				Bio = user.Bio
			};
			_logger.LogInformation("User {Username} loaded settings successfully.", user.UserName);

			return View(userDto);
		}

		// POST update profile
		[Authorize]
		[HttpPost]
		public async Task<IActionResult> UpdateProfile(UserProfileDto userProfileDto)
		{
			_logger.LogInformation("UpdateProfile action invoked.");
			var user = await _userRepository.GetUserAsync(User);
			if (user == null)
			{
				_logger.LogWarning("User not found while updating profile.");
				ModelState.AddModelError("User", "User not found");
				return View("Settings", userProfileDto);
			}
			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Model state invalid while updating profile for user {UserId}.", user.Id);
				return View("Settings", userProfileDto);
			}
			if (!string.IsNullOrEmpty(userProfileDto.Username))
			{

				user.UserName = userProfileDto.Username;
				var resultuname = await _userRepository.UpdateUserAsync(user);
				// Sign out and sign in again to update the username in the cookie
				if (resultuname.Succeeded)
				{
					_logger.LogInformation("Username updated successfully for user {UserId}.", user.Id);
					await _userRepository.SignOutAsync();
					await _userRepository.SignInAsync(user, isPersistent: false);
				}

			}
			else
			{
				_logger.LogWarning("Empty username provided for user {UserId}.", user.Id);
				ViewData["ProfileError"] = "Username cannot be empty.";
				return View("Settings", userProfileDto);
			}
			if (!string.IsNullOrEmpty(userProfileDto.Email))
				user.Email = userProfileDto.Email;
			else
			{
				_logger.LogWarning("Empty email provided for user {UserId}.", user.Id);
				ViewData["ProfileError"] = "Email cannot be empty.";
				return View("Settings", userProfileDto);
			}
			if (!string.IsNullOrEmpty(userProfileDto.Bio))
				user.Bio = userProfileDto.Bio;
			else
			{
				_logger.LogWarning("Empty bio provided for user {UserId}.", user.Id);
				ViewData["ProfileError"] = "Bio cannot be empty.";
				return View("Settings", userProfileDto);
			}

			// Handle profile picture upload
			if (userProfileDto.ProfilePicture != null && userProfileDto.ProfilePicture.Length > 0)
			{
				if (!FileSmallerThan10MB(userProfileDto.ProfilePicture))
				{
					_logger.LogWarning("Profile picture exceeds size limit for user {UserId}.", user.Id);
					ViewData["ProfileError"] = "Profile picture must be smaller than 10MB.";
					return View("Settings", userProfileDto);
				}
				// checks if we have a folder for uploads, if not, create it
				var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/profile_pictures");
				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
					_logger.LogInformation("Created profile pictures folder at {UploadsFolder}.", uploadsFolder);
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
						_logger.LogInformation("Deleted old profile picture for user {UserId}.", user.Id);
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
			var updateResult = await _userRepository.UpdateUserAsync(user);
			if (updateResult.Succeeded)
			{
				_logger.LogInformation("Profile updated successfully for user {UserId}.", user.Id);
				ViewData["ProfileSuccess"] = "Profile updated successfully.";
			}
			else
			{
				var errorMessages = string.Join("<br/>", updateResult.Errors.Select(e => e.Description));
				_logger.LogWarning("Profile update failed for user {UserId}: {Errors}", user.Id, errorMessages);
				ViewData["ProfileError"] = errorMessages;
			}

			return View("Settings", userProfileDto);
		}

		// PUT: api/Users/change-password
		[Authorize]
		[HttpPost]
		public async Task<IActionResult> ChangePassword(UserProfileDto userProfileDto)
		{
			_logger.LogInformation("ChangePassword action invoked.");
			var user = await _userRepository.GetUserAsync(User);
			// Reload user data to ensure the Settings view is populated with the latest data on each form submission
			var updatedUserDto = new UserProfileDto
			{
				Username = user?.UserName,
				Email = user?.Email,
				Bio = user?.Bio,
				ProfilePictureUrl = user?.ProfilePictureUrl
			};
			if (user == null)
			{
				_logger.LogWarning("User not found while changing password.");
				ViewData["PasswordError"] = "User not found";
				return View("Settings", updatedUserDto);
			}
			if (string.IsNullOrEmpty(userProfileDto.CurrentPassword) || string.IsNullOrEmpty(userProfileDto.NewPassword) || string.IsNullOrEmpty(userProfileDto.ConfirmPassword))
			{
				_logger.LogWarning("Incomplete password fields for user {UserId}.", user.Id);
				ViewData["PasswordError"] = "Please fill in all fields";
				return View("Settings", updatedUserDto);
			}
			if (userProfileDto.NewPassword != userProfileDto.ConfirmPassword)
			{
				_logger.LogWarning("Password confirmation does not match for user {UserId}.", user.Id);
				ViewData["PasswordError"] = "The passwords do not match";
				return View("Settings", updatedUserDto);
			}
			var results = await _userRepository.ChangePasswordAsync(user, userProfileDto.CurrentPassword, userProfileDto.NewPassword);
			if (results.Succeeded)
			{
				_logger.LogInformation("Password changed successfully for user {UserId}.", user.Id);
				ViewData["PasswordSuccess"] = "Your password is updated";
			}
			else
			{
				var errorMessages = string.Join("<br/>", results.Errors.Select(e => e.Description));
				_logger.LogWarning("Password change failed for user {UserId}.", user.Id);
				ViewData["PasswordError"] = errorMessages;
			}
			return View("Settings", updatedUserDto);
		}
		// DELETE: api/Users/delete-account
		[Authorize]
		[HttpPost]
		public async Task<IActionResult> DeleteAccount()
		{
			_logger.LogInformation("DeleteAccount action invoked.");
			var user = await _userRepository.GetUserAsync(User);
			if (user == null)
			{
				_logger.LogWarning("User not found while deleting account.");
				ModelState.AddModelError("User", "User not found");
				return RedirectToAction("Settings");
			}
			if (user.ProfilePictureUrl != "/images/default_profile.jpg")
			{
				// Delete the old profile picture
				var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfilePictureUrl.TrimStart('/'));
				if (System.IO.File.Exists(oldFilePath))
				{
					System.IO.File.Delete(oldFilePath);
					_logger.LogInformation("Deleted profile picture for user {UserId}.", user.Id);
				}
			}
			var result = await _userRepository.DeleteUserAsync(user);
			if (result.Succeeded)
			{
				_logger.LogInformation("Account deleted successfully for user {UserId}.", user.Id);
				return RedirectToAction("Login", "Users");
			}

			ViewData["ProfileError"] = "Error deleting account.";
			return RedirectToAction("Settings");

		}

		public static bool FileSmallerThan10MB(IFormFile file)
		{
			long MaxFileSizeInBytes = 10 * 1024 * 1024; // 10MB in bytes
			return file.Length <= MaxFileSizeInBytes;
		}

	}
}
