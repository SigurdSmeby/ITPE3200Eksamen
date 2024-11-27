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
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(postDto.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await postDto.ImageFile.CopyToAsync(fileStream);
                }

                imagePath = "/uploads/posts/" + uniqueFileName; // Set relative path
            }

            // Create and save the new post
            var post = new Post
            {
                UserId = userId,
                ImagePath = imagePath,
                TextContent = postDto.TextContent,
                FontSize = postDto.FontSize ?? 16,
                TextColor = postDto.TextColor ?? "#000000",
                BackgroundColor = postDto.BackgroundColor ?? "#FFFFFF"
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return Ok("Post created successfully");
        }

        // GET: api/Posts
        // Retrieves all posts, sorted by upload date, along with user and interaction data.
        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _context.Posts
                .OrderByDescending(p => p.DateUploaded)
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
        // Retrieves a specific post by its ID. Includes user and interaction data.
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
                return NotFound();

            // Update the image if provided
            if (updatePostDto.ImageFile != null && updatePostDto.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(post.ImagePath))
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, post.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

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

            if (post == null)
                return NotFound();

            if (post.UserId != userId)
                return Forbid("You are not authorized to delete this post.");

            if (!string.IsNullOrEmpty(post.ImagePath))
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, post.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Ok("Post deleted successfully.");
        }

        // GET: api/Posts/user/{username}
        // Retrieves all posts created by a specific user, with pagination support.
        [HttpGet("user/{username}")]
        public async Task<IActionResult> GetPostsByUsername(string username, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var user = await _context.Users
                .Where(u => u.Username == username)
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User not found");

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
