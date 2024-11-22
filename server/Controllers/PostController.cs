using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.Models;
using server.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PostsController : ControllerBase
	{
		private readonly AppDbContext _context;

		public PostsController(AppDbContext context)
		{
			_context = context;
		}

		// POST: api/Posts
		[Authorize]
		[HttpPost]
		public async Task<IActionResult> CreatePost([FromBody] CreatePostDto postDto)
		{
			int userId = GetCurrentUserId();

			var post = new Post
			{
				UserId = userId,
				ImageUrl = postDto.ImageUrl,
				TextContent = postDto.TextContent,
				FontSize = postDto.FontSize ?? 16, // Default font size
				TextColor = postDto.TextColor ?? "#000000", // Default text color (black)
				BackgroundColor = postDto.BackgroundColor ?? "#FFFFFF" // Default background color (white)
			};

			_context.Posts.Add(post);
			await _context.SaveChangesAsync();

			return Ok("Post created successfully.");
		}

		// GET: api/Posts
		[HttpGet]
		public async Task<IActionResult> GetPosts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
		{
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
					ImageUrl = p.ImageUrl,
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
					/*Comments = p.Comments.Select(c => new CommentDto
					{
						CommentId = c.CommentId,
						Content = c.Content,
						DateCommented = c.DateCommented,
						AuthorUsername = c.User.Username
					}).ToList(),*/
					CommentsCount = p.Comments.Count // Calculate comment count
				})
				.ToListAsync();

			return Ok(posts);
		}


		// GET: api/Posts/5
		[Authorize]
		[HttpGet("{id}")]
		public async Task<IActionResult> GetPost(int id)
		{
			var post = await _context.Posts
				.Include(p => p.User)
				.Include(p => p.Likes)
				.Include(p => p.Comments)
				.ThenInclude(c => c.User)
				.SingleOrDefaultAsync(p => p.PostId == id);

			if (post == null)
				return NotFound();

			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
			if (userIdClaim == null)
				return Unauthorized("User is not authenticated.");

			int userId = int.Parse(userIdClaim.Value);

			if (post.UserId != userId)
				return Forbid("You are not authorized to view this post.");

			var postDto = new PostDto
			{
				PostId = post.PostId,
				ImageUrl = post.ImageUrl,
				TextContent = post.TextContent,
				DateUploaded = post.DateUploaded,
				FontSize = post.FontSize,
				TextColor = post.TextColor,
				BackgroundColor = post.BackgroundColor,
				Author = new UserDto
				{
					UserId = post.User.UserId,
					Username = post.User.Username,
					ProfilePictureUrl = post.User.ProfilePictureUrl
				},
				LikesCount = post.Likes.Count,
				Comments = post.Comments.Select(c => new CommentDto
				{
					CommentId = c.CommentId,
					Content = c.Content,
					DateCommented = c.DateCommented,
					AuthorUsername = c.User.Username
				}).ToList()
			};

			return Ok(postDto);
		}

		// PUT: api/Posts/5
		[Authorize]
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostDto postDto)
		{
			int userId = GetCurrentUserId();

			var post = await _context.Posts.FindAsync(id);

			if (post == null)
				return NotFound();

			if (post.UserId != userId)
				return Forbid("You are not authorized to update this post.");

			post.ImageUrl = postDto.ImageUrl ?? post.ImageUrl;
			post.TextContent = postDto.TextContent ?? post.TextContent;
			post.FontSize = postDto.FontSize ?? post.FontSize;
			post.TextColor = postDto.TextColor ?? post.TextColor;
			post.BackgroundColor = postDto.BackgroundColor ?? post.BackgroundColor;

			_context.Posts.Update(post);
			await _context.SaveChangesAsync();

			return Ok("Post updated successfully.");
		}

		// DELETE: api/Posts/5
		[Authorize]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePost(int id)
		{
			int userId = GetCurrentUserId();

			var post = await _context.Posts.FindAsync(id);

			if (post == null)
				return NotFound();

			if (post.UserId != userId)
				return Forbid("You are not authorized to delete this post.");

			_context.Posts.Remove(post);
			await _context.SaveChangesAsync();

			return Ok("Post deleted successfully.");
		}

		// GET: api/Posts/user/{username}
		[HttpGet("user/{username}")]
		public async Task<IActionResult> GetPostsByUsername(string username, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
		{
			var user = await _context.Users
				.Where(u => u.Username == username)
				.FirstOrDefaultAsync();

			if (user == null)
			{
				return NotFound("User not found");
			}

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
					ImageUrl = p.ImageUrl,
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

			var response = new
			{
				Username = user.Username,
				ProfilePictureUrl = user.ProfilePictureUrl,
				Bio = user.Bio,
				Posts = posts
			};

			return Ok(response);
		}

		// Helper method
		private int GetCurrentUserId()
		{
			return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
		}
	}
}
