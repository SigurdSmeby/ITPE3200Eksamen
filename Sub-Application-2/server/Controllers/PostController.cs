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

namespace server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PostsController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public PostsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
		}

		// POST: api/Posts
		// Creates a new post with optional text and an image. The post is associated with the currently logged-in user.
		[Authorize]
		[HttpPost]
		public async Task<IActionResult> CreatePost([FromForm] CreatePostDto postDto)
		{
			int userId = GetCurrentUserId();

			string? imagePath = null;

			// Handle the image file if provided in the DTO
			if (postDto.ImageFile != null && postDto.ImageFile.Length > 0)
			{
				var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/posts");
				if (!Directory.Exists(uploadsFolder))
				{
					// Create the uploads directory if it does not exist
					Directory.CreateDirectory(uploadsFolder);
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

			return Ok("Post created successfully");
		}

		// GET: api/Posts
		// Retrieves all posts, sorted by upload date, along with user and interaction data.
		[HttpGet]
		public async Task<IActionResult> GetPosts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
		{
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

			return Ok(response);
		}

		// GET: api/Posts/5
		// Retrieves a specific post by its ID. Includes user and interaction data.
		[Authorize]
		[HttpGet("{id}")]
		public async Task<IActionResult> GetPost(int id)
		{
			// Find the post by ID and include user and interaction data
			var post = await _context.Posts
				.Include(p => p.User)
				.Include(p => p.Likes)
				.Include(p => p.Comments)
				.ThenInclude(c => c.User) 
				.SingleOrDefaultAsync(p => p.PostId == id);

			if (post == null)
				return NotFound(); // Return 404 if the post is not found

			// Map the post to a DTO
			var postDto = new PostDto
			{
				ImagePath = post.ImagePath,
				TextContent = post.TextContent,
				FontSize = post.FontSize,
				TextColor = post.TextColor,
				BackgroundColor = post.BackgroundColor,
			};

			return Ok(postDto);
		}

		// PUT: api/Posts/5
		// Updates an existing post by its ID. Allows updating the image, text, and styling.
		[Authorize]
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdatePost(int id, [FromForm] UpdatePostDto updatePostDto)
		{
			var post = await _context.Posts.FindAsync(id);

			if (post == null)
				return NotFound(); // Return 404 if the post does not exist

			// Update the image if a new file is provided
			if (updatePostDto.ImageFile != null && updatePostDto.ImageFile.Length > 0)
			{
				// Remove the old image file if it exists
				if (!string.IsNullOrEmpty(post.ImagePath))
				{
					var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, post.ImagePath.TrimStart('/'));
					if (System.IO.File.Exists(oldFilePath))
					{
						System.IO.File.Delete(oldFilePath);
					}
				}

				// Save the new image file
				var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/posts");
				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
				}

				var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(updatePostDto.ImageFile.FileName);
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await updatePostDto.ImageFile.CopyToAsync(fileStream);
				}

				post.ImagePath = "/uploads/posts/" + uniqueFileName;
			}

			// Update text and styling fields if provided
			if (!string.IsNullOrEmpty(updatePostDto.TextContent))
				post.TextContent = updatePostDto.TextContent;

			if (updatePostDto.FontSize.HasValue)
				post.FontSize = updatePostDto.FontSize.Value;

			if (!string.IsNullOrEmpty(updatePostDto.TextColor))
				post.TextColor = updatePostDto.TextColor;

			if (!string.IsNullOrEmpty(updatePostDto.BackgroundColor))
				post.BackgroundColor = updatePostDto.BackgroundColor;

			// Save changes to the database
			_context.Posts.Update(post);
			await _context.SaveChangesAsync();

			return Ok(new { success = true, message = "Post updated successfully!" });
		}

		// DELETE: api/Posts/5
		// Deletes a specific post by its ID, including the associated image file.
		[Authorize]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePost(int id)
		{
			int userId = GetCurrentUserId();

			var post = await _context.Posts.FindAsync(id);

			// Return 404 if the post does not exist
			if (post == null)
				return NotFound(); 

			// Ensure the current user owns the post
			if (post.UserId != userId)
				return Forbid("You are not authorized to delete this post.");

			// Delete the associated image file if it exists
			if (!string.IsNullOrEmpty(post.ImagePath))
			{
				var filePath = Path.Combine(_webHostEnvironment.WebRootPath, post.ImagePath.TrimStart('/'));
				if (System.IO.File.Exists(filePath))
				{
					System.IO.File.Delete(filePath);
				}
			}

			// Remove the post from the database
			_context.Posts.Remove(post);
			await _context.SaveChangesAsync();

			return Ok("Post deleted successfully.");
		}

		// GET: api/Posts/user/{username}
		// Retrieves all posts for a specific user, with pagination support.
		[HttpGet("user/{username}")]
		public async Task<IActionResult> GetPostsByUsername(string username, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
		{
			// Find the user by username
			var user = await _context.Users
				.Where(u => u.Username == username)
				.FirstOrDefaultAsync();

			// Return 404 if the user does not exist
			if (user == null)
				return NotFound("User not found"); 

			// Get the total count of posts for the user
			var totalPosts = await _context.Posts
				.Where(p => p.UserId == user.UserId)
				.CountAsync();

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
