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
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        /* --------------------------------------------------------------------------------------------
            POSTS
            The following methods are used to create, read, update, and delete posts.
        ----------------------------------------------------------------------------------------------*/

        [HttpGet]
        public IActionResult CreatePost()
        {
            return View();
        }

        // POST: api/Posts
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostDto postDto)
        {
            String userId = GetCurrentUserId();

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

            return RedirectToAction("Index", "Home");
        }
//  ---------------------------
//      Fortsett HER
//-------------------------


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
                        UserId = p.User.Id,
                        Username = p.User.UserName,
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
                    UserId = post.User.Id,
                    Username = post.User.UserName,
                    ProfilePictureUrl = post.User.ProfilePictureUrl
                },
                LikesCount = post.Likes.Count,
                Comments = post.Comments.Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    Content = c.Content,
                    DateCommented = c.DateCommented,
                    AuthorUsername = c.User.UserName
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
            String userId = GetCurrentUserId();

            var post = await _context.Posts.FindAsync(id);

            if (post == null)
                return NotFound();

            if (!post.UserId.Equals(userId))
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
                .Where(u => u.UserName == username)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found");
            }

            var posts = await _context.Posts
                .Where(p => p.UserId.Equals(user.Id))
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
                        UserId = p.User.Id,
                        Username = p.User.UserName,
                        ProfilePictureUrl = p.User.ProfilePictureUrl
                    },
                    LikesCount = p.Likes.Count,
                    CommentsCount = p.Comments.Count
                })
                .ToListAsync();

            var response = new
            {
                Username = user.UserName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Bio = user.Bio,
                Posts = posts
            };

            return Ok(response);
        }

        /* --------------------------------------------------------------------------------------------
            COMMENTS
            The following methods are used to create, read and delete comments.
        ----------------------------------------------------------------------------------------------*/

		// POST: api/Comments
		[Authorize]
		[HttpPost]
		public async Task<IActionResult> AddComment([FromBody] AddCommentDto commentDto)
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

			return Ok("Comment added successfully.");
		}


		// GET: api/Comments/post/5
		[HttpGet("post/{postId}")]
		public async Task<IActionResult> GetCommentsForPost(int postId)
		{
			var comments = await _context.Comments
				.Where(c => c.PostId == postId)
				.Include(c => c.User)
				.OrderBy(c => c.DateCommented)
				.Select(c => new CommentDto
				{
					CommentId = c.CommentId,
					Content = c.Content,
					DateCommented = c.DateCommented,
					AuthorUsername = c.User.UserName
				})
				.ToListAsync();

			return Ok(comments);
		}

		// DELETE: api/Comments/5
		[Authorize]
		[HttpDelete("{id}")]
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

			return Ok("Comment deleted successfully.");
		}


        /* --------------------------------------------------------------------------------------------
            LIKES
            The following methods are used to create, read and delete likes.
        ----------------------------------------------------------------------------------------------*/
                // POST: api/Likes/like/5
        [Authorize]
        [HttpPost("like/{postId}")]
        public async Task<IActionResult> LikePost(int postId)
        {
            String userId = GetCurrentUserId();

            if (await _context.Likes.AnyAsync(l => l.PostId == postId && l.UserId == userId))
            {
                return BadRequest("You have already liked this post.");
            }

            var like = new Like
            {
                UserId = userId,
                PostId = postId
            };

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            return Ok("Post liked successfully.");
        }

        // DELETE: api/Likes/unlike/5
        [Authorize]
        [HttpDelete("unlike/{postId}")]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            String userId = GetCurrentUserId();

            var like = await _context.Likes.FindAsync(userId, postId);

            if (like == null)
                return NotFound("You have not liked this post.");

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return Ok("Post unliked successfully.");
        }

        // GET: api/Likes/post/5
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetLikesForPost(int postId)
        {
            var likes = await _context.Likes
                .Where(l => l.PostId == postId)
                .Include(l => l.User)
                .Select(l => new UserDto
                {
                    UserId = l.User.Id,
                    Username = l.User.UserName,
                    ProfilePictureUrl = l.User.ProfilePictureUrl
                })
                .ToListAsync();

            return Ok(likes);
        }

        

        // Helper method
		private String GetCurrentUserId()
		{
			return User.FindFirstValue(ClaimTypes.NameIdentifier);
		}
    }
}
