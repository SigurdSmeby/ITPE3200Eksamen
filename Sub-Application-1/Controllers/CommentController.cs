using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Sub_Application_1.Data;
using Sub_Application_1.DTOs;
using Sub_Application_1.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Sub_Application_1.Controllers
{
    [Route("Comments")]
    public class CommentController : Controller
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
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
                    AuthorUsername = c.User.UserName
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

            string userId = GetCurrentUserId();

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
			// Get the current user ID
			string userId = GetCurrentUserId();

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
					AuthorUsername = c.User.UserName
				})
				.ToListAsync();

			// Return the updated partial view
			return PartialView("CommentsListPartial", comments);
		}


        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
