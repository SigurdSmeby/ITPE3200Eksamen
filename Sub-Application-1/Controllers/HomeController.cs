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

		public HomeController(
			IPostRepository postRepository,
			IUserRepository userRepository,
			IWebHostEnvironment webHostEnvironment)
		{
			_postRepository = postRepository;
			_userRepository = userRepository;
			_webHostEnvironment = webHostEnvironment;
		}

		// GET: /Home/Index or /
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			if (User.Identity != null && User.Identity.IsAuthenticated)
			{
				var user = await _userRepository.GetUserAsync(User);
				ViewData["Username"] = user?.UserName;
			}
			else
			{
				ViewData["Username"] = null;
			}

			var posts = await _postRepository.GetAllPostsWithDetailsAsync();

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


			return View(postDtos);
		}





		// Optional: About page
		[HttpGet]
		public IActionResult About()
		{
			return View();
		}

		/* --------------------------------------------------------------------------------------------
            POSTS
            The following methods are used to create, read, update, and delete posts.
        ----------------------------------------------------------------------------------------------*/

		[HttpGet("CreatePost")]
		public IActionResult CreatePost()
		{
			return View();
		}


		// POST: api/Posts
		[Authorize]
		[HttpPost("CreatePost")]
		public async Task<IActionResult> CreatePost(CreatePostDto postDto)
		{
			var user = await _userRepository.GetUserAsync(User);
			if (user == null)
			{
				return Unauthorized();
			}
			string userId = user.Id;

			if (!ModelState.IsValid)
			{
				return View(postDto);
			}
			if (postDto.Image == null && string.IsNullOrEmpty(postDto.TextContent))
			{
				ViewData["ErrorMessage"] = "Please provide an image or text content.";
				return View(postDto);
			}

			string imagePath = string.Empty;

			if (postDto.Image != null && postDto.Image.Length > 0)
			{
				// Check if the file size is less than 10MB
				if (!FileSmallerThan10MB(postDto.Image))
				{
					ViewData["ErrorMessage"] = "File size must be less than 10MB.";
					return View(postDto);
				}
				// Define the uploads folder path
				var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
				}

				// Generate a unique filename for the uploaded file
				var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(postDto.Image.FileName);
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				// Save the file
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await postDto.Image.CopyToAsync(fileStream);
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

			return RedirectToAction("Index");
		}

		[Authorize]
		[HttpGet("EditPost/{postId}")]
		public async Task<IActionResult> EditPost(int postId)
		{
			// Fetch the post by ID
			var post = await _postRepository.GetPostByIdAsync(postId);
			// Check the user 
			var user = await _userRepository.GetUserAsync(User);
			if (user == null)
			{
				return Unauthorized();
			}
			string userId = user.Id;

			if (post == null)
			{
				return NotFound($"Post with ID '{postId}' not found.");
			}

			// Check if the logged-in user is the owner of the post
			if (post.UserId != userId)
			{
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
			return View(updatePostDto);
		}

		[Authorize]
		[HttpPost("EditPost/{postId}")]
		public async Task<IActionResult> EditPost(int postId, UpdatePostDto updatePostDto, IFormFile? ImageFile)
		{
			if (!ModelState.IsValid)
			{
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
				return Unauthorized();
			}
			string userId = user.Id;

			if (post == null)
			{
				return NotFound($"Post with ID '{postId}' not found.");
			}

			// Check if the logged-in user is the owner of the post
			if (post.UserId != user.Id)
			{
				return Forbid();
			}

			// Update image if a new one is uploaded
			if (ImageFile != null && ImageFile.Length > 0)
			{
				if (!FileSmallerThan10MB(ImageFile))
				{
					ModelState.AddModelError("ImageFile", "File size must be less than 10MB.");
					ViewData["ImagePath"] = post.ImagePath; // Maintain image preview on error
					return View(updatePostDto);
				}
				var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
				if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

				var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await ImageFile.CopyToAsync(fileStream);
				}

				// Remove old image file if it exists
				if (!string.IsNullOrEmpty(post.ImagePath))
				{
					var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, post.ImagePath.TrimStart('/'));
					if (System.IO.File.Exists(oldFilePath))
					{
						System.IO.File.Delete(oldFilePath);
					}
				}

				post.ImagePath = "/uploads/" + uniqueFileName; // Set new image path
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

			return RedirectToAction("Index");
		}


		// DELETE: /Home/DeletePost/5
		[Authorize]
		[HttpPost("DeletePost")]
		public async Task<IActionResult> DeletePost(int id)
		{


			// Check the user 
			var user = await _userRepository.GetUserAsync(User);
			if (user == null)
			{
				return Unauthorized();
			}
			string userId = user.Id;

			// Find the post by ID
			var post = await _postRepository.GetPostByIdAsync(id);

			if (post == null)
				return NotFound("Post not found.");

			// Check if the user is authorized to delete the post
			if (post.UserId != userId)
				return Forbid("You are not authorized to delete this post.");

			// Remove old image file if it exists
			if (!string.IsNullOrEmpty(post.ImagePath))
			{
				var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, post.ImagePath.TrimStart('/'));
				if (System.IO.File.Exists(oldFilePath))
				{
					System.IO.File.Delete(oldFilePath);
				}
			}

			// Delete the post
			_postRepository.Delete(post);
			await _postRepository.SaveAsync();

			return RedirectToAction("Index");
		}

		[HttpGet("Profile/{username?}")] // Make 'username' optional 
		public async Task<IActionResult> Profile(string? username)
		{
			// Use the logged-in user's username if 'username' is null or empty
			if (string.IsNullOrEmpty(username))
			{
				username = User.Identity?.Name; // Get the logged-in user's username

				if (string.IsNullOrEmpty(username))
				{
					// If no logged-in user, return an error or redirect to login
					return RedirectToAction("Login", "Users");
				}
			}

			// Fetch the user by username
			var user = await _userRepository.GetUserByUsernameAsync(username);

			if (user == null)
			{
				return NotFound($"User with username '{username}' not found.");
			}

			// fetch posts by user
			var posts = await _postRepository.GetPostsByUserIdAsync(user.Id);
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