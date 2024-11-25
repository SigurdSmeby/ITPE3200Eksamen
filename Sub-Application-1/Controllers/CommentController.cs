using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Sub_Application_1.Data;
using Sub_Application_1.Models;
using Sub_Application_1.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Sub_Application_1.Controllers
{
	[Route("Comments")]
	public class CommentsController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly AppDbContext _context;

		public CommentsController(UserManager<User> userManager, AppDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		// POST: api/Comments
		[Authorize]
		[HttpPost("create")]
		public async Task<IActionResult> AddComment(AddCommentDto commentDto)
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

			return Json(new
			{
				success = true,
				commentId = comment.CommentId,
				content = comment.Content,
				author = User.Identity.Name,
				dateCommented = comment.DateCommented.ToString("MMM dd, yyyy")
			});
		}


		// GET: api/Comments/post/5
		[HttpGet("GetCommentsForPost")]
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

			return PartialView("_CommentsPartial", comments);
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

		// Helper method
		private String GetCurrentUserId()
		{
			return User.FindFirstValue(ClaimTypes.NameIdentifier);
		}
	}
}
