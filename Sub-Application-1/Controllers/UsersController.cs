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

		// Constructor initializes repository for user management, and iwebhost fir file management
		public UsersController(IUserRepository userRepository, IWebHostEnvironment webHostEnvironment)
		{
			_userRepository = userRepository;
			_webHostEnvironment = webHostEnvironment;
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

			if (registerDto.Password != registerDto.confirmPassword)
			{
				ModelState.AddModelError("Password", "Passwords do not match.");
			}
			var existingUserByUsername = await _userRepository.GetUserByUsernameAsync(registerDto.Username);
			if (existingUserByUsername != null)
			{
				ModelState.AddModelError("Username", "Username already exists.");
			}
			var existingUserByEmail = await _userRepository.GetUserByEmailAsync(registerDto.Email);
			if (existingUserByEmail != null)
			{
				ModelState.AddModelError("Email", "Email already registered.");
			}
			if (!ModelState.IsValid)
			{
				return View(registerDto);
			}


			var result = await _userRepository.RegisterUserAsync(registerDto);
			Console.WriteLine("Result: " + result);
			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Home"); // Successful registration.
			}

			foreach (var error in result.Errors)
			{
				Console.WriteLine("Error: " + error.Description);
				ModelState.AddModelError(string.Empty, error.Description);
			}


			return View(registerDto); // Unuccessful registration.
		}

		// Post login
		[HttpPost]
		public async Task<IActionResult> Login(LoginDto loginDto)
		{

			if (!ModelState.IsValid)
			{
				return View();
			}

			var result = await _userRepository.LoginAsync(loginDto);

			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Home");
			}
			else if (result.IsLockedOut)
			{
				// this shouldnt be able to be triggered, but it is created to be used in the eventual future
				// just to prove we know that we can lock out users if we want to :hrek_smile:
				ModelState.AddModelError("Login", "This account is locked out. Please try again later.");
			}
			else
			{
				// error message purposely vague to prevent brute force attacks
				ModelState.AddModelError("Login", "Invalid username or password.");
			}
			return View();
		}
		[HttpGet]
		public async Task<IActionResult> Logout()
		{
			await _userRepository.LogoutAsync();
			return RedirectToAction("Index", "Home");
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> Settings()
		{
			var user = await _userRepository.GetUserAsync(User);
			if (user == null)
			{
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

			return View(userDto);
		}

		// POST update profile
		[Authorize]
		[HttpPost]
		public async Task<IActionResult> UpdateProfile(UserProfileDto userProfileDto)
		{
			var user = await _userRepository.GetUserAsync(User);
			if (user == null)
			{
				ModelState.AddModelError("User", "User not found");
				return View("Settings", userProfileDto);
			}
			if (!ModelState.IsValid)
			{
				return View("Settings", userProfileDto);
			}
			if (!string.IsNullOrEmpty(userProfileDto.Username))
			{

				user.UserName = userProfileDto.Username;
				var resultuname = await _userRepository.UpdateUserAsync(user);
				// Sign out and sign in again to update the username in the cookie
				if (resultuname.Succeeded)
				{
					await _userRepository.SignOutAsync();
					await _userRepository.SignInAsync(user, isPersistent: false);
				}

			}
			else
			{
				ViewData["ProfileError"] = "Username cannot be empty.";
				return View("Settings", userProfileDto);
			}
			if (!string.IsNullOrEmpty(userProfileDto.Email))
				user.Email = userProfileDto.Email;
			else
			{
				ViewData["ProfileError"] = "Email cannot be empty.";
				return View("Settings", userProfileDto);
			}
			if (!string.IsNullOrEmpty(userProfileDto.Bio))
				user.Bio = userProfileDto.Bio;
			else
			{
				ViewData["ProfileError"] = "Bio cannot be empty.";
				return View("Settings", userProfileDto);
			}

			// Handle profile picture upload
			if (userProfileDto.ProfilePicture != null && userProfileDto.ProfilePicture.Length > 0)
			{
				if (!FileSmallerThan10MB(userProfileDto.ProfilePicture))
				{
					ViewData["ProfileError"] = "Profile picture must be smaller than 10MB.";
					return View("Settings", userProfileDto);
				}
				// checks if we have a folder for uploads, if not, create it
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
			var updateResult = await _userRepository.UpdateUserAsync(user);
			if (updateResult.Succeeded)
			{
				ViewData["ProfileSuccess"] = "Profile updated successfully.";
			}
			else
			{
				var errorMessages = string.Join("<br/>", updateResult.Errors.Select(e => e.Description));
				ViewData["ProfileError"] = errorMessages;
			}

			return View("Settings", userProfileDto);
		}

		// PUT: api/Users/change-password
		[Authorize]
		[HttpPost]
		public async Task<IActionResult> ChangePassword(UserProfileDto userProfileDto)
		{
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
				ViewData["PasswordError"] = "User not found";
				return View("Settings", updatedUserDto);
			}
			if (string.IsNullOrEmpty(userProfileDto.CurrentPassword) || string.IsNullOrEmpty(userProfileDto.NewPassword) || string.IsNullOrEmpty(userProfileDto.ConfirmPassword))
			{
				ViewData["PasswordError"] = "Please fill in all fields";
				return View("Settings", updatedUserDto);
			}
			if (userProfileDto.NewPassword != userProfileDto.ConfirmPassword)
			{
				ViewData["PasswordError"] = "The passwords do not match";
				return View("Settings", updatedUserDto);
			}
			var results = await _userRepository.ChangePasswordAsync(user, userProfileDto.CurrentPassword, userProfileDto.NewPassword);
			if (results.Succeeded)
			{
				ViewData["PasswordSuccess"] = "Your password is updated";
			}
			else
			{
				ViewData["PasswordError"] = "Current password is incorrect";
			}
			return View("Settings", updatedUserDto);
		}
		// DELETE: api/Users/delete-account
		[Authorize]
		[HttpPost]
		public async Task<IActionResult> DeleteAccount()
		{
			var user = await _userRepository.GetUserAsync(User);
			if (user == null)
			{
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
				}
			}
			var result = await _userRepository.DeleteUserAsync(user);
			if (result.Succeeded)
			{
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
