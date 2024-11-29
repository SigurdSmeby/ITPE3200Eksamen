using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sub_Application_1.Data;
using Sub_Application_1.DTOs;
using Sub_Application_1.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Sub_Application_1.Controllers
{
    [Route("Posts")]
    public class PostController : Controller
    {
        private readonly UserManager<User> _userManager;
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public PostController(UserManager<User> userManager, AppDbContext context, IWebHostEnvironment webHostEnvironment)
		{
			_userManager = userManager;
			_context = context;
			_webHostEnvironment = webHostEnvironment;
		}

        // GET: Posts/Index
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .OrderByDescending(p => p.DateUploaded)
                .Include(p => p.User) // Include post owner details
                .Include(p => p.Likes)
                .ThenInclude(l => l.User)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
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
                        AuthorUsername = c.User.UserName
                    }).ToList(),
                    Likes = p.Likes.Select(l => new UserDto
                    {
                        UserId = l.User.Id,
                        Username = l.User.UserName,
                        ProfilePictureUrl = l.User.ProfilePictureUrl
                    }).ToList()
                })
                .ToListAsync();

            return View(posts);
        }

        // POST: Posts/CreatePost
        [Authorize]
        [HttpPost("CreatePost")]
        public async Task<IActionResult> CreatePost(CreatePostDto postDto)
        {
            string userId = GetCurrentUserId();

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
                ImagePath = imagePath,
                TextContent = postDto.TextContent,
                FontSize = postDto.FontSize ?? 16,
                TextColor = postDto.TextColor ?? "#000000",
                BackgroundColor = postDto.BackgroundColor ?? "#FFFFFF"
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        // POST: Posts/DeletePost
        [Authorize]
        [HttpPost("DeletePost")]
        public async Task<IActionResult> Delete(int id)
        {
            string userId = GetCurrentUserId();

            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound("Post not found.");
            }

            if (post.UserId != userId)
            {
                return Forbid("You are not authorized to delete this post.");
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Posts/Edit/{id}
        [Authorize]
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
            {
                return NotFound("Post not found.");
            }

            if (post.UserId != GetCurrentUserId())
            {
                return Forbid();
            }

            var updatePostDto = new UpdatePostDto
            {
                TextContent = post.TextContent,
                FontSize = post.FontSize,
                TextColor = post.TextColor,
                BackgroundColor = post.BackgroundColor
            };

            ViewData["ImagePath"] = post.ImagePath;

            return View(updatePostDto);
        }

        // POST: Posts/EditPost/{id}
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

			if (post == null)
			{
				return NotFound($"Post with ID '{postId}' not found.");
			}

			// Check if the logged-in user is the owner of the post
			if (post.UserId != GetCurrentUserId())
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

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
		public static bool FileSmallerThan10MB(IFormFile file)
        {
            long MaxFileSizeInBytes = 10 * 1024 * 1024; // 10MB in bytes
            return file.Length <= MaxFileSizeInBytes;
        }
    }
}
