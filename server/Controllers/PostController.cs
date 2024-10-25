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
				Title = postDto.Title
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
				.Select(p => new PostDto
				{
					PostId = p.PostId,
					ImageUrl = p.ImageUrl,
					Title = p.Title,
					DateUploaded = p.DateUploaded,
					Author = new UserDto
					{
						UserId = p.User.UserId,
						Username = p.User.Username,
						ProfilePictureUrl = p.User.ProfilePictureUrl
					},
					LikesCount = p.Likes.Count
				})
				.ToListAsync();

			return Ok(posts);
		}

		// GET: api/Posts/5
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

			var postDto = new PostDto
			{
				PostId = post.PostId,
				ImageUrl = post.ImageUrl,
				Title = post.Title,
				DateUploaded = post.DateUploaded,
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

			post.Title = postDto.Title ?? post.Title;
			post.ImageUrl = postDto.ImageUrl ?? post.ImageUrl;

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
			// Find the user by username
			var user = await _context.Users
				.Where(u => u.Username == username)
				.FirstOrDefaultAsync();

			if (user == null)
			{
				return NotFound("User not found");
			}

			// Fetch the posts for the user
			var posts = await _context.Posts
				.Where(p => p.UserId == user.UserId) // Filter by the UserId of the user with the provided username
				.OrderByDescending(p => p.DateUploaded)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.Include(p => p.User)
				.Include(p => p.Likes)
				.Select(p => new PostDto
				{
					PostId = p.PostId,
					ImageUrl = p.ImageUrl,
					Title = p.Title,
					DateUploaded = p.DateUploaded,
					Author = new UserDto
					{
						UserId = p.User.UserId,
						Username = p.User.Username,
						ProfilePictureUrl = p.User.ProfilePictureUrl
					},
					LikesCount = p.Likes.Count
				})
				.ToListAsync();

			// Return user information along with the posts
			var response = new
			{
				Username = user.Username,
				ProfilePictureUrl = user.ProfilePictureUrl,
				Bio = user.Bio,
				Posts = posts // List of posts for the user
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
