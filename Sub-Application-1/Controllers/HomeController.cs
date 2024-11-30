using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Sub_Application_1.Models;
using Sub_Application_1.Data;
using Sub_Application_1.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IO;

namespace Sub_Application_1.Controllers 
{
	public class HomeController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public HomeController(UserManager<User> userManager, AppDbContext context, IWebHostEnvironment webHostEnvironment)
		{
			_userManager = userManager;
			_context = context;
			_webHostEnvironment = webHostEnvironment;
		}

		// GET: /Home/Index or /
		[HttpGet]
		public async Task<IActionResult> Index()
		{
				if (User.Identity != null && User.Identity.IsAuthenticated)
				{
						var user = await _userManager.GetUserAsync(User);
						ViewData["Username"] = user?.UserName;
				}
				else
				{
						ViewData["Username"] = null;
				}

				var posts = await _context.Posts
						.OrderByDescending(p => p.DateUploaded)
						.Include(p => p.User) // Include post owner details
						.Include(p => p.Likes) // Include likes for each post
						.ThenInclude(l => l.User) // Include details of users who liked the post
						.Include(p => p.Comments)
						.ThenInclude(c => c.User) // Include user details for comments
						.Select(p => new PostDto
						{
								PostId = p.PostId,
								ImagePath = p.ImagePath,
								TextContent = p.TextContent,
								DateUploaded = p.DateUploaded,
								FontSize = p.FontSize,
								TextColor = p.TextColor,
								BackgroundColor = p.BackgroundColor,
								Author = new UserDto
								{
										UserId = p.User.Id,
										Username = p.User.UserName,
										ProfilePictureUrl = p.User.ProfilePictureUrl
								},
								LikesCount = p.Likes.Count, // Count of likes for the post
								Likes = p.Likes.Select(l => new UserDto
								{
										UserId = l.User.Id,
										Username = l.User.UserName,
										ProfilePictureUrl = l.User.ProfilePictureUrl
								}).ToList() // List of users who liked the post
						})
						.ToListAsync();

				return View(posts);
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
			var user = await _userManager.GetUserAsync(User);
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

			_context.Posts.Add(post);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		[Authorize]
		[HttpGet("EditPost/{postId}")]
		public async Task<IActionResult> EditPost(int postId)
		{
			// Fetch the post by ID
			var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == postId);
			
			// Check the user 
			var user = await _userManager.GetUserAsync(User);
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
				ViewData["ImagePath"] = (await _context.Posts.FindAsync(postId))?.ImagePath; // Maintain image preview on error
				return View(updatePostDto);
			}

			// Fetch the post
			var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == postId);

			// Check the user 
			var user = await _userManager.GetUserAsync(User);
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



			_context.Posts.Update(post);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}


		// DELETE: /Home/DeletePost/5
		[Authorize]
		[HttpPost("DeletePost")]
		public async Task<IActionResult> DeletePost(int id)
		{
			

			// Check the user 
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Unauthorized();
			}
    	string userId = user.Id;

			// Find the post by ID
			var post = await _context.Posts.FindAsync(id);

			if (post == null)
				return NotFound("Post not found.");

			// Check if the user is authorized to delete the post
			if (post.UserId != userId)
				return Forbid("You are not authorized to delete this post.");

			// Delete the post
			_context.Posts.Remove(post);
			await _context.SaveChangesAsync();

			// Redirect back to the main posts page or index
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
			var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);

			if (user == null)
			{
				return NotFound($"User with username '{username}' not found.");
			}

			// Fetch posts created by the user
			var posts = await _context.Posts
				.Where(p => p.UserId == user.Id)
				.OrderByDescending(p => p.DateUploaded)
				.Include(p => p.User)
				.Include(p => p.Likes)
				.Include(p => p.Comments)
				.ThenInclude(c => c.User) // Include user details for comments
				.Select(p => new PostDto
				{
					PostId = p.PostId,
					ImagePath = p.ImagePath,
					TextContent = p.TextContent,
					DateUploaded = p.DateUploaded,
					FontSize = p.FontSize,
					TextColor = p.TextColor,
					BackgroundColor = p.BackgroundColor,
					Author = new UserDto
					{
						UserId = p.User.Id,
						Username = p.User != null && p.User.UserName != null ? p.User.UserName : "[DELETED]", // Default value for deleted users, incase they are displayed, they should not be though
						ProfilePictureUrl = p.User != null && p.User.ProfilePictureUrl != null ? p.User.ProfilePictureUrl : "/images/default-profile.png" // sets default profile picture if there is a null value
					},
					LikesCount = p.Likes.Count,
				})
				.ToListAsync();

			// Set ViewData for profile details
			ViewData["Username"] = user.UserName;
			ViewData["ProfilePictureUrl"] = user.ProfilePictureUrl;
			ViewData["Bio"] = user.Bio;

			// Return the view with posts
			return View(posts);
		}



		public static bool FileSmallerThan10MB(IFormFile file)
		{
				long MaxFileSizeInBytes = 10 * 1024 * 1024; // 10MB in bytes
				return file.Length <= MaxFileSizeInBytes;
		}
	}
}