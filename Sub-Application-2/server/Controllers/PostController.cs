using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.Models;
using server.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IO;
using System;

namespace server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PostsController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly ILogger<PostsController> _logger;

		public PostsController(AppDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<PostsController> logger)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
			_logger = logger;
		}

		// POST: api/Posts
		// Creates a new post with optional text and an image. The post is associated with the currently logged-in user.
		[Authorize]
		[HttpPost]
		public async Task<IActionResult> CreatePost([FromForm] CreatePostDto postDto)
		{
			_logger.LogInformation("User is attempting to create a post.");
			int userId = GetCurrentUserId();

			string? imagePath = null;

			// Handle the image file if provided in the DTO
			if (postDto.ImageFile != null && postDto.ImageFile.Length > 0)
			{
				_logger.LogInformation("Processing image upload for new post.");
				var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/posts");

				if (!Directory.Exists(uploadsFolder))
				{
					// Create the uploads directory if it does not exist
					Directory.CreateDirectory(uploadsFolder);
					_logger.LogInformation("Created uploads directory at: {UploadsFolder}", uploadsFolder);
				}

				// Generate a unique file name for the image
				var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(postDto.ImageFile.FileName);
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				// Save the image to the server
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await postDto.ImageFile.CopyToAsync(fileStream);
				}

				// Store the relative image path for database storage
				imagePath = "/uploads/posts/" + uniqueFileName;
				_logger.LogInformation("Image uploaded and saved to: {ImagePath}", imagePath);
			}

			// Create and save the new post
			var post = new Post
			{
				UserId = userId,
				ImagePath = imagePath,
				TextContent = postDto.TextContent,
				FontSize = postDto.FontSize ?? 16,
				TextColor = postDto.TextColor ?? "#000000",
				BackgroundColor = postDto.BackgroundColor ?? "#FFFFFF" // Default background color (white)
			};

			_context.Posts.Add(post);
			await _context.SaveChangesAsync();
			_logger.LogInformation("Post created successfully for UserId: {UserId}.", userId);

			return Ok("Post created successfully");
		}

		// GET: api/Posts
		// Retrieves all posts, sorted by upload date, along with user and interaction data.
		[HttpGet]
		public async Task<IActionResult> GetPosts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
		{
			_logger.LogInformation("Fetching posts. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
			// Get the total count of posts in the database
			var totalPosts = await _context.Posts.CountAsync();

			// Retrieve paginated posts with user and interaction details
			var posts = await _context.Posts
				.OrderByDescending(p => p.DateUploaded)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.Include(p => p.User)
				.Include(p => p.Likes)
				.Include(p => p.Comments)
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
						UserId = p.User.UserId,
						Username = p.User.Username,
						ProfilePictureUrl = p.User.ProfilePictureUrl
					},
					LikesCount = p.Likes.Count,
					CommentsCount = p.Comments.Count
				})
				.ToListAsync();

			// Return posts along with pagination metadata
			var response = new
			{
				TotalPosts = totalPosts,
				PageNumber = pageNumber,
				PageSize = pageSize,
				Posts = posts
			};
			_logger.LogInformation("Fetched {PostCount} posts.", posts.Count);

			return Ok(response);
		}

		// GET: api/Posts/5
		// Retrieves a specific post by its ID. Includes user and interaction data.
		[Authorize]
		[HttpGet("{id}")]
		public async Task<IActionResult> GetPost(int id)
		{
			_logger.LogInformation("User is attempting to retrieve PostId: {PostId}.", id);
			// Find the post by ID and include user and interaction data
			var post = await _context.Posts
				.Include(p => p.User)
				.Include(p => p.Likes)
				.Include(p => p.Comments)
				.ThenInclude(c => c.User)
				.SingleOrDefaultAsync(p => p.PostId == id);

			if (post == null)
			{
				_logger.LogWarning("GetPost failed: PostId {PostId} not found.", id);
				return NotFound(); // Return 404 if the post is not found
			}
			_logger.LogInformation("PostId {PostId} retrieved successfully.", id);

			// Map the post to a DTO
			var postDto = new PostDto
			{
				ImagePath = post.ImagePath,
				TextContent = post.TextContent,
				FontSize = post.FontSize,
				TextColor = post.TextColor,
				BackgroundColor = post.BackgroundColor,
			};
			_logger.LogInformation("PostId {PostId} mapped to DTO and returned.", id);

			return Ok(postDto);
		}

		// PUT: api/Posts/5
		// Updates an existing post by its ID. Allows updating the image, text, and styling.
		[Authorize]
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdatePost(int id, [FromForm] UpdatePostDto updatePostDto)
		{
			_logger.LogInformation("User is attempting to update PostId: {PostId}.", id);
			var post = await _context.Posts.FindAsync(id);

			if (post == null)
			{
				_logger.LogWarning("UpdatePost failed: PostId {PostId} not found.", id);
				return NotFound(); // Return 404 if the post does not exist
			}
			_logger.LogInformation("PostId {PostId} found. Processing update.", id);
			// Update the image if a new file is provided
			if (updatePostDto.ImageFile != null && updatePostDto.ImageFile.Length > 0)
			{
				_logger.LogInformation("Processing image update for PostId: {PostId}.", id);
				// Remove the old image file if it exists
				if (!string.IsNullOrEmpty(post.ImagePath))
				{
					var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, post.ImagePath.TrimStart('/'));
					if (System.IO.File.Exists(oldFilePath))
					{
						System.IO.File.Delete(oldFilePath);
						_logger.LogInformation("Old image file deleted for PostId: {PostId}.", id);
					}
				}

				// Save the new image file
				var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/posts");
				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
					_logger.LogInformation("Uploads folder created at: {UploadsFolder}.", uploadsFolder);
				}

				var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(updatePostDto.ImageFile.FileName);
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await updatePostDto.ImageFile.CopyToAsync(fileStream);
				}

				post.ImagePath = "/uploads/posts/" + uniqueFileName;
				_logger.LogInformation("New image uploaded and set for PostId: {PostId}.", id);
			}

			// Update text and styling fields if provided
			if (!string.IsNullOrEmpty(updatePostDto.TextContent))
			{
				post.TextContent = updatePostDto.TextContent;
				_logger.LogInformation("TextContent updated for PostId: {PostId}.", id);
			}

			if (updatePostDto.FontSize.HasValue)
			{
				post.FontSize = updatePostDto.FontSize.Value;
				_logger.LogInformation("FontSize updated for PostId: {PostId}.", id);
			}

			if (!string.IsNullOrEmpty(updatePostDto.TextColor))
			{
				post.TextColor = updatePostDto.TextColor;
				_logger.LogInformation("TextColor updated for PostId: {PostId}.", id);
			}

			if (!string.IsNullOrEmpty(updatePostDto.BackgroundColor))
			{
				post.BackgroundColor = updatePostDto.BackgroundColor;
				_logger.LogInformation("BackgroundColor updated for PostId: {PostId}.", id);
			}

			// Save changes to the database
			_context.Posts.Update(post);
			await _context.SaveChangesAsync();
			_logger.LogInformation("PostId {PostId} updated successfully.", id);

			return Ok(new { success = true, message = "Post updated successfully!" });
		}

		// DELETE: api/Posts/5
		// Deletes a specific post by its ID, including the associated image file.
		[Authorize]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePost(int id)
		{
			_logger.LogInformation("User is attempting to delete PostId: {PostId}", id);
			int userId = GetCurrentUserId();

			var post = await _context.Posts.FindAsync(id);

			// Return 404 if the post does not exist
			if (post == null)
			{
				_logger.LogWarning("DeletePost failed: PostId {PostId} not found.", id);
				return NotFound();
			}

			// Ensure the current user owns the post
			if (post.UserId != userId)
			{
				_logger.LogWarning("UserId {UserId} is not authorized to delete PostId {PostId}.", userId, id);
				return Forbid("You are not authorized to delete this post.");
			}

			// Delete the associated image file if it exists
			if (!string.IsNullOrEmpty(post.ImagePath))
			{
				var filePath = Path.Combine(_webHostEnvironment.WebRootPath, post.ImagePath.TrimStart('/'));
				if (System.IO.File.Exists(filePath))
				{
					System.IO.File.Delete(filePath);
					_logger.LogInformation("Deleted image associated with PostId {PostId} at: {FilePath}", id, filePath);
				}
			}

			// Remove the post from the database
			_context.Posts.Remove(post);
			await _context.SaveChangesAsync();
			_logger.LogInformation("PostId {PostId} deleted successfully by UserId {UserId}.", id, userId);

			return Ok("Post deleted successfully.");
		}

		// GET: api/Posts/user/{username}
		// Retrieves all posts for a specific user, with pagination support.
		[HttpGet("user/{username}")]
		public async Task<IActionResult> GetPostsByUsername(string username, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
		{
			_logger.LogInformation("Fetching posts for username: {Username}, PageNumber: {PageNumber}, PageSize: {PageSize}", username, pageNumber, pageSize);
			// Find the user by username
			var user = await _context.Users
				.Where(u => u.Username == username)
				.FirstOrDefaultAsync();

			// Return 404 if the user does not exist
			if (user == null)
			{
				_logger.LogWarning("GetPostsByUsername failed: User '{Username}' not found.", username);
				return NotFound("User not found");
			}

			_logger.LogInformation("User '{Username}' found. Fetching posts.", username);
			// Get the total count of posts for the user
			var totalPosts = await _context.Posts
				.Where(p => p.UserId == user.UserId)
				.CountAsync();
			_logger.LogInformation("User '{Username}' has {TotalPosts} total posts.", username, totalPosts);
			// Retrieve paginated posts for the user
			var posts = await _context.Posts
				.Where(p => p.UserId == user.UserId)
				.OrderByDescending(p => p.DateUploaded)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.Include(p => p.User)
				.Include(p => p.Likes)
				.Include(p => p.Comments)
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
						UserId = p.User.UserId,
						Username = p.User.Username,
						ProfilePictureUrl = p.User.ProfilePictureUrl
					},
					LikesCount = p.Likes.Count,
					CommentsCount = p.Comments.Count
				})
				.ToListAsync();

			_logger.LogInformation("Fetched {PostCount} posts for User '{Username}'.", posts.Count, username);
			// Return user and their posts
			var response = new
			{
				Username = user.Username,
				ProfilePictureUrl = user.ProfilePictureUrl,
				Bio = user.Bio,
				DateJoined = user.DateJoined,
				TotalPosts = totalPosts,
				Posts = posts
			};
			_logger.LogInformation("Returning posts for User '{Username}' with {PostCount} posts.", username, posts.Count);

			return Ok(response);
		}

		// Helper method
		// Retrieves the ID of the currently logged-in user
		private int GetCurrentUserId()
		{
			var userIdClaim = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User ID claim is missing."));
			return userIdClaim;
		}
	}
}
