using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Sub_Application_1.Data;
using Sub_Application_1.DTOs;
using Sub_Application_1.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;


namespace Sub_Application_1.Controllers
{
    [Route("Comments")]
    public class CommentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public CommentController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Comments/CommentsListPartial?postId=5
        [HttpGet("CommentsListPartial")]
        public async Task<IActionResult> GetCommentsListPartial(int postId)
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
                    AuthorUsername = c.User != null && c.User.UserName != null ? c.User.UserName : "[Deleted]"
                })
                .ToListAsync();

            return PartialView("CommentsListPartial", comments);
        }

        // POST: Comments/AddComment
        [Authorize]
        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment(AddCommentDto commentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid comment data.");
            }

            var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Unauthorized();
			}
    	    string userId = user.Id;

            var comment = new Comment
            {
                UserId = userId,
                PostId = commentDto.PostId,
                Content = commentDto.Content,
                DateCommented = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("GetCommentsListPartial", new { postId = commentDto.PostId });
        }

        // POST: Comments/DeleteComment
		[Authorize]
		[HttpPost("DeleteComment")]
		public async Task<IActionResult> DeleteComment(int id)
		{
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            string userId = user.Id;

			// Find the comment by ID
			var comment = await _context.Comments.Include(c => c.Post).FirstOrDefaultAsync(c => c.CommentId == id);

			if (comment == null)
			{
				return NotFound("Comment not found.");
			}

			// Check if the user is authorized to delete the comment
			if (comment.UserId != userId)
			{
				return Forbid("You are not authorized to delete this comment.");
			}

			// Retrieve the Post ID from the comment
			var postId = comment.PostId;

			// Delete the comment
			_context.Comments.Remove(comment);
			await _context.SaveChangesAsync();

			// Fetch the updated comments list for the post
			var comments = await _context.Comments
				.Where(c => c.PostId == postId)
				.Include(c => c.User)
				.OrderBy(c => c.DateCommented)
				.Select(c => new CommentDto
				{
					CommentId = c.CommentId,
					Content = c.Content,
					DateCommented = c.DateCommented,
					AuthorUsername = c.User != null && c.User.UserName != null ? c.User.UserName : "[Deleted]"// Default value for deleted users incase they are displayed, they should not be though
				})
				.ToListAsync();

			// Return the updated partial view
			return PartialView("CommentsListPartial", comments);
		}

    }
}
