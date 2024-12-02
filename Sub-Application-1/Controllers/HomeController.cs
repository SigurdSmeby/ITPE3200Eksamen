using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sub_Application_1.DTOs;
using Sub_Application_1.Models;
using Sub_Application_1.Repositories.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sub_Application_1.Controllers
{
	public class HomeController : Controller
	{
		// Repository for post management
		private readonly IPostRepository _postRepository;
		private readonly IUserRepository _userRepository;
		// Webhost for file management
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly ILogger<HomeController> _logger;

		public HomeController(
			IPostRepository postRepository,
			IUserRepository userRepository,
			IWebHostEnvironment webHostEnvironment,
			ILogger<HomeController> logger)
		{
			_postRepository = postRepository;
			_userRepository = userRepository;
			_webHostEnvironment = webHostEnvironment;
			_logger = logger;
		}

		// GET: /Home/Index or /
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			_logger.LogInformation("Index action invoked.");
			if (User.Identity != null && User.Identity.IsAuthenticated)
			{
				var user = await _userRepository.GetUserAsync(User);
				ViewData["Username"] = user?.UserName;
				_logger.LogInformation("Logged-in user: {Username}", user?.UserName);
			}
			else
			{
				ViewData["Username"] = null;
				_logger.LogInformation("No user logged in.");
			}

			var posts = await _postRepository.GetAllPostsWithDetailsAsync();
			_logger.LogInformation("Fetched {PostCount} posts.", posts);

			// Map posts to PostDto
			var postDtos = posts.Select(p => new PostDto
			{
				PostId = p.PostId,
				ImagePath = p.ImagePath,
				TextContent = p.TextContent,
				DateUploaded = p.DateUploaded,
				FontSize = p.FontSize,
				TextColor = p.TextColor,
				BackgroundColor = p.BackgroundColor,
				// Create a userDTO for the author
				Author = new UserDto
				{
					UserId = p.User.Id,
					Username = p.User.UserName,
					ProfilePictureUrl = p.User.ProfilePictureUrl
				},
				LikesCount = p.Likes.Count,
				// Create a list of userDTOs for the likes, so we kwow who liked the post
				Likes = p.Likes.Select(l => new UserDto
				{
					UserId = l.User.Id,
					Username = l.User.UserName,
					ProfilePictureUrl = l.User.ProfilePictureUrl
				}).ToList()
			}).ToList();
			_logger.LogInformation("Mapped posts to PostDto.");

			return View(postDtos);
		}


		// About page
		[HttpGet]
		public IActionResult About()
		{
			_logger.LogInformation("About action invoked.");
			return View();
		}

		/* --------------------------------------------------------------------------------------------
            POSTS
            The following methods are used to create, read, update, and delete posts.
        ----------------------------------------------------------------------------------------------*/

		[HttpGet("CreatePost")]
		public IActionResult CreatePost()
		{
			_logger.LogInformation("CreatePost GET action invoked.");
			return View();
		}


		// POST: api/Posts
		[Authorize]
		[HttpPost("CreatePost")]
		public async Task<IActionResult> CreatePost(CreatePostDto postDto)
		{
			_logger.LogInformation("CreatePost POST action invoked.");

			var user = await _userRepository.GetUserAsync(User);
			if (user == null)
			{
				_logger.LogWarning("Unauthorized user attempted to create a post.");
				return Unauthorized();
			}
			string userId = user.Id;

			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Invalid post data submitted.");
				return View(postDto);
			}
			if (postDto.Image == null && string.IsNullOrEmpty(postDto.TextContent))
			{
				_logger.LogWarning("Post creation failed due to missing image or text content.");
				ViewData["ErrorMessage"] = "Please provide an image or text content.";
				return View(postDto);
			}

			string imagePath = string.Empty;

			if (postDto.Image != null && postDto.Image.Length > 0)
			{
				// Check if the file size is less than 10MB
				if (!FileSmallerThan10MB(postDto.Image))
				{
					_logger.LogWarning("Post creation failed due to file size exceeding limit.");
					ViewData["ErrorMessage"] = "File size must be less than 10MB.";
					return View(postDto);
				}
				// Define the uploads folder path
				var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
					_logger.LogInformation("Uploads folder created at {UploadsFolder}.", uploadsFolder);
				}

				// Generate a unique filename for the uploaded file
				var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(postDto.Image.FileName);
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				// Save the file
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await postDto.Image.CopyToAsync(fileStream);
					_logger.LogInformation("Image saved at {FilePath}.", filePath);
				}

				// Set the relative path
				imagePath = "/uploads/" + uniqueFileName;
			}

			var post = new Post
			{
				UserId = userId,
				ImagePath = imagePath, // Save the relative path
				TextContent = postDto.TextContent,
				FontSize = postDto.FontSize ?? 16, // Default font size
				TextColor = postDto.TextColor ?? "#000000", // Default text color (black)
				BackgroundColor = postDto.BackgroundColor ?? "#FFFFFF" // Default background color (white)
			};

			await _postRepository.AddAsync(post);
			await _postRepository.SaveAsync();
			_logger.LogInformation("Post created successfully by user {UserId}.", user.Id);

			return RedirectToAction("Index");
		}

		[Authorize]
		[HttpGet("EditPost/{postId}")]
		public async Task<IActionResult> EditPost(int postId)
		{
			_logger.LogInformation("EditPost GET action invoked for postId {PostId}.", postId);
			// Fetch the post by ID
			var post = await _postRepository.GetPostByIdAsync(postId);
			// Check the user 
			var user = await _userRepository.GetUserAsync(User);
			if (user == null)
			{
				_logger.LogWarning("Unauthorized access attempt to edit post {PostId}.", postId);
				return Unauthorized();
			}
			string userId = user.Id;

			if (post == null)
			{
				_logger.LogWarning("Post with ID {PostId} not found.", postId);
				return NotFound($"Post with ID '{postId}' not found.");
			}

			// Check if the logged-in user is the owner of the post
			if (post.UserId != userId)
			{
				_logger.LogWarning("User {UserId} is not authorized to edit post {PostId}.", user.Id, postId);
				return Forbid();
			}

			// Map the existing post data to UpdatePostDto
			var updatePostDto = new UpdatePostDto
			{
				TextContent = post.TextContent,
				FontSize = post.FontSize,
				TextColor = post.TextColor,
				BackgroundColor = post.BackgroundColor
			};
			ViewData["PostId"] = post.PostId;
			ViewData["ImagePath"] = post.ImagePath; // Pass the existing image path to the view
			_logger.LogInformation("EditPost GET action completed for postId {PostId}.", postId);
			return View(updatePostDto);
		}

		[Authorize]
		[HttpPost("EditPost/{postId}")]
		public async Task<IActionResult> EditPost(int postId, UpdatePostDto updatePostDto, IFormFile? ImageFile)
		{
			_logger.LogInformation("EditPost POST action invoked for postId {PostId}.", postId);
			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Invalid model state for postId {PostId}.", postId);
				var existingPost = await _postRepository.GetPostByIdAsync(postId);
				ViewData["ImagePath"] = existingPost?.ImagePath; // Maintain image preview on error
				return View(updatePostDto);
			}

			// Fetch the post
			var post = await _postRepository.GetPostByIdAsync(postId);

			// Check the user 
			var user = await _userRepository.GetUserAsync(User);
			if (user == null)
			{
				_logger.LogWarning("Unauthorized attempt to edit post {PostId}.", postId);
				return Unauthorized();
			}
			string userId = user.Id;

			if (post == null)
			{
				_logger.LogWarning("Post with ID {PostId} not found during edit.", postId);
				return NotFound($"Post with ID '{postId}' not found.");
			}

			// Check if the logged-in user is the owner of the post
			if (post.UserId != user.Id)
			{
				_logger.LogWarning("User {UserId} is not authorized to edit post {PostId}.", user.Id, postId);
				return Forbid();
			}

			// Update image if a new one is uploaded
			if (ImageFile != null && ImageFile.Length > 0)
			{
				if (!FileSmallerThan10MB(ImageFile))
				{
					_logger.LogWarning("File size exceeds 10MB for postId {PostId}.", postId);
					ModelState.AddModelError("ImageFile", "File size must be less than 10MB.");
					ViewData["ImagePath"] = post.ImagePath; // Maintain image preview on error
					return View(updatePostDto);
				}
				var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
					_logger.LogInformation("Created uploads folder at {UploadsFolder}.", uploadsFolder);
				}

				var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await ImageFile.CopyToAsync(fileStream);
					_logger.LogInformation("Saved image file for postId {PostId} at {FilePath}.", postId, filePath);
				}

				// Remove old image file if it exists
				if (!string.IsNullOrEmpty(post.ImagePath))
				{
					var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, post.ImagePath.TrimStart('/'));
					if (System.IO.File.Exists(oldFilePath))
					{
						System.IO.File.Delete(oldFilePath);
						_logger.LogInformation("Deleted old image file for postId {PostId} at {OldFilePath}.", postId, oldFilePath);
					}
				}

				post.ImagePath = "/uploads/" + uniqueFileName; // Set new image path
				_logger.LogInformation("Updated image path for postId {PostId} to {ImagePath}.", postId, post.ImagePath);
			}
			else
			{
				// Update the post
				post.TextContent = updatePostDto.TextContent;
				post.FontSize = updatePostDto.FontSize ?? post.FontSize;
				post.TextColor = updatePostDto.TextColor;
				post.BackgroundColor = updatePostDto.BackgroundColor;
			}



			_postRepository.Update(post);
			await _postRepository.SaveAsync();
			_logger.LogInformation("Post with ID {PostId} updated successfully by user {UserId}.", postId, user.Id);

			return RedirectToAction("Index");
		}


		// DELETE: /Home/DeletePost/5
		[Authorize]
		[HttpPost("DeletePost")]
		public async Task<IActionResult> DeletePost(int id)
		{
			_logger.LogInformation("DeletePost action invoked for postId {PostId}.", id);

			// Check the user 
			var user = await _userRepository.GetUserAsync(User);
			if (user == null)
			{
				_logger.LogWarning("Unauthorized attempt to delete post {PostId}.", id);
				return Unauthorized();
			}
			string userId = user.Id;

			// Find the post by ID
			var post = await _postRepository.GetPostByIdAsync(id);

			if (post == null)
			{
				_logger.LogWarning("Post with ID {PostId} not found.", id);
				return NotFound("Post not found.");
			}

			// Check if the user is authorized to delete the post
			if (post.UserId != userId)
			{
				_logger.LogWarning("User {UserId} is not authorized to delete post {PostId}.", user.Id, id);
				return Forbid("You are not authorized to delete this post.");
			}
			// Remove old image file if it exists
			if (!string.IsNullOrEmpty(post.ImagePath))
			{
				var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, post.ImagePath.TrimStart('/'));
				if (System.IO.File.Exists(oldFilePath))
				{
					System.IO.File.Delete(oldFilePath);
					_logger.LogInformation("Deleted image file for postId {PostId} at {FilePath}.", id, oldFilePath);
				}
			}

			// Delete the post
			_postRepository.Delete(post);
			await _postRepository.SaveAsync();
			_logger.LogInformation("Post with ID {PostId} deleted successfully by user {UserId}.", id, user.Id);

			return RedirectToAction("Index");
		}

		[HttpGet("Profile/{username?}")]
		public async Task<IActionResult> Profile(string? username)
		{
			_logger.LogInformation("Profile action invoked for username {Username}.", username);
			// Use the logged-in user's username if 'username' is null or empty
			if (string.IsNullOrEmpty(username))
			{
				username = User.Identity?.Name;

				if (string.IsNullOrEmpty(username))
				{
					_logger.LogWarning("No username provided and user is not logged in.");
					return RedirectToAction("Login", "Users");
				}
			}

			// Fetch the user by username
			var user = await _userRepository.GetUserByUsernameAsync(username);

			if (user == null)
			{
				_logger.LogWarning("User with username {Username} not found.", username);
				return NotFound($"User with username '{username}' not found.");
			}

			// fetch posts by user
			var posts = await _postRepository.GetPostsByUserIdAsync(user.Id);
			_logger.LogInformation("Fetched {PostCount} posts for user {Username}.", posts, username);
			// Map posts to PostDto
			var postDtos = posts.Select(p => new PostDto
			{
				PostId = p.PostId,
				ImagePath = p.ImagePath,
				TextContent = p.TextContent,
				DateUploaded = p.DateUploaded,
				FontSize = p.FontSize,
				TextColor = p.TextColor,
				BackgroundColor = p.BackgroundColor,
				// Create a userDTO for the author and add to the postDto
				Author = new UserDto
				{
					UserId = p.User.Id,
					Username = p.User.UserName ?? "[DELETED]",
					ProfilePictureUrl = p.User.ProfilePictureUrl ?? "/images/default-profile.png"
				},
				LikesCount = p.Likes.Count,
			}).ToList();

			// Set ViewData for profile details
			ViewData["Username"] = user.UserName;
			ViewData["ProfilePictureUrl"] = user.ProfilePictureUrl;
			ViewData["Bio"] = user.Bio;
			_logger.LogInformation("Profile action completed for username {Username}.", username);
			// Return the view with posts
			return View(postDtos);
		}



		public static bool FileSmallerThan10MB(IFormFile file)
		{
			long MaxFileSizeInBytes = 10 * 1024 * 1024; // 10MB in bytes
			return file.Length <= MaxFileSizeInBytes;
		}
	}
}