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

namespace Sub_Application_1.Controllers // Proper namespace declaration
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
			if (User.Identity.IsAuthenticated)
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
						Username = p.User.UserName,
						ProfilePictureUrl = p.User.ProfilePictureUrl
					},
					LikesCount = p.Likes.Count,
					CommentsCount = p.Comments.Count,
					Comments = p.Comments.Select(c => new CommentDto
					{
						CommentId = c.CommentId,
						Content = c.Content,
						DateCommented = c.DateCommented,
						AuthorUsername = c.User.UserName // Assuming User navigation property
					}).ToList()
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
			string userId = GetCurrentUserId();

			if (!ModelState.IsValid)
			{
				return View(postDto);
			}

			string imagePath = string.Empty;

			if (postDto.Image != null && postDto.Image.Length > 0)
			{
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

		// GET: api/Posts
		[HttpGet]
		public async Task<IActionResult> GetPosts()
		{
			var posts = await _context.Posts
				.OrderByDescending(p => p.DateUploaded)
				.Include(p => p.User)
				.Include(p => p.Likes)
				.Include(p => p.Comments)
				.ToListAsync();

			return PartialView("_PostsPartial", posts);
		}

		// DELETE: /Home/DeletePost/5
		[Authorize]
		[HttpPost("DeletePost")]
		public async Task<IActionResult> DeletePost(int id)
		{
			// Get the current user's ID
			string userId = GetCurrentUserId();

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
						Username = p.User.UserName,
						ProfilePictureUrl = p.User.ProfilePictureUrl
					},
					LikesCount = p.Likes.Count,
					CommentsCount = p.Comments.Count,
					Comments = p.Comments.Select(c => new CommentDto
					{
						CommentId = c.CommentId,
						Content = c.Content,
						DateCommented = c.DateCommented,
						AuthorUsername = c.User.UserName // Assuming User navigation property
					}).ToList()
				})
				.ToListAsync();

			// Set ViewData for profile details
			ViewData["Username"] = user.UserName;
			ViewData["ProfilePictureUrl"] = user.ProfilePictureUrl;
			ViewData["Bio"] = user.Bio;

			// Return the view with posts
			return View(posts);
		}


		/* --------------------------------------------------------------------------------------------
		COMMENTS

		The following methods are used to create, read, and delete comments.
		--------------------------------------------------------------------------------------------*/

		// POST: api/Comments
		[Authorize]
		[HttpPost("CreateComment")]
		public async Task<IActionResult> AddComment(AddCommentDto commentDto)
		{
			String userId = GetCurrentUserId();

			var comment = new Comment
			{
				UserId = userId,
				PostId = commentDto.PostId,
				Content = commentDto.Content
			};

			_context.Comments.Add(comment);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		// DELETE: api/Comments/5
		[Authorize]
		[HttpPost("DeleteComment")]
		public async Task<IActionResult> DeleteComment(int id)
		{
			String userId = GetCurrentUserId();

			var comment = await _context.Comments.FindAsync(id);

			if (comment == null)
				return NotFound();

			if (comment.UserId != userId)
				return Forbid("You are not authorized to delete this comment.");

			_context.Comments.Remove(comment);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}


		// Helper method
		private string GetCurrentUserId()
		{
			return User.FindFirstValue(ClaimTypes.NameIdentifier);
		}
	}
}
