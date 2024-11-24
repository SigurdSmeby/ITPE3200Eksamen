using Microsoft.AspNetCore.Mvc;
using Sub_Application_1.Data;
using Sub_Application_1.Models;
using Sub_Application_1.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IO;

namespace Sub_Application_1.Controllers
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
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDto postDto, [FromForm] IFormFile? imageFile)
        {
            int userId = GetCurrentUserId();

            string? imagePath = null;

            if (imageFile != null && imageFile.Length > 0)
            {
                // Define the uploads folder path
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate a unique filename for the uploaded file
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
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

            var postDto = new PostDto
            {
                PostId = post.PostId,
                ImagePath = post.ImagePath,
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
		public async Task<IActionResult> UpdatePost(int id, [FromForm] IFormFile? imageFile, [FromForm] string? textContent, [FromForm] int? fontSize, [FromForm] string? textColor, [FromForm] string? backgroundColor)
		{
			var post = await _context.Posts.FindAsync(id);

			if (post == null)
				return NotFound();

			// If an image is uploaded, process it
			if (imageFile != null && imageFile.Length > 0)
			{
				var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
				}

				var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await imageFile.CopyToAsync(fileStream);
				}

				post.ImagePath = "/uploads/" + uniqueFileName; // Update the image path
			}

			// Update other fields for text posts
			if (textContent != null)
				post.TextContent = textContent;

			if (fontSize.HasValue)
				post.FontSize = fontSize.Value;

			if (!string.IsNullOrEmpty(textColor))
				post.TextColor = textColor;

			if (!string.IsNullOrEmpty(backgroundColor))
				post.BackgroundColor = backgroundColor;

			_context.Posts.Update(post);
			await _context.SaveChangesAsync();

			return Ok(new { success = true, message = "Post updated successfully!" });
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
