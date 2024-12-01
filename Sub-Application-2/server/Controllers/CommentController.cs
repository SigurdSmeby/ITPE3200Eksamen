using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.Models;
using server.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CommentsController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly ILogger<CommentsController> _logger;

		public CommentsController(AppDbContext context, ILogger<CommentsController> logger)
		{
			_context = context;
			_logger = logger;
		}

		// Adds a new comment to a post. User must be authenticated.
		[Authorize]
		[HttpPost()]
		public async Task<IActionResult> AddComment([FromBody] AddCommentDto commentDto)
		{
			_logger.LogInformation("User is attempting to add a comment to PostId: {PostId}.", commentDto.PostId);
			int userId = GetCurrentUserId();

			var comment = new Comment
			{
				UserId = userId,
				PostId = commentDto.PostId,
				Content = commentDto.Content
			};
			_context.Comments.Add(comment);
			await _context.SaveChangesAsync();
			_logger.LogInformation("Comment added successfully by UserId: {UserId} to PostId: {PostId}.", userId, commentDto.PostId);

			return Ok("Comment added successfully.");
		}

		// Retrieves all comments for a specific post.
		[HttpGet("post/{postId}")]
		public async Task<IActionResult> GetCommentsForPost(int postId)
		{
			_logger.LogInformation("Fetching comments for PostId: {PostId}.", postId);
			var comments = await _context.Comments
				.Where(c => c.PostId == postId)
				.Include(c => c.User)
				.OrderBy(c => c.DateCommented)
				.Select(c => new CommentDto
				{
					CommentId = c.CommentId,
					Content = c.Content,
					DateCommented = c.DateCommented,
					AuthorUsername = c.User == null ? "[Deleted]" : c.User.Username
				})
				.ToListAsync();
			_logger.LogInformation("Fetched {CommentCount} comments for PostId: {PostId}.", comments.Count, postId);

			return Ok(comments);
		}

		// Deletes a comment. Only the comment's author is allowed to delete it.
		[Authorize]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteComment(int id)
		{
			_logger.LogInformation("User is attempting to delete CommentId: {CommentId}.", id);
			int userId = GetCurrentUserId();

			var comment = await _context.Comments.FindAsync(id);

			if (comment == null)
			{
				_logger.LogWarning("DeleteComment failed: CommentId {CommentId} not found.", id);
				return NotFound();
			}


			if (comment.UserId != userId)
			{
				_logger.LogWarning("UserId {UserId} is not authorized to delete CommentId {CommentId}.", userId, id);
				return Forbid("You are not authorized to delete this comment.");
			}

			_context.Comments.Remove(comment);
			await _context.SaveChangesAsync();

			_logger.LogInformation("CommentId {CommentId} deleted successfully by UserId {UserId}.", id, userId);
			return Ok("Comment deleted successfully.");
		}

		// Retrieves the currently authenticated user's ID.
		private int GetCurrentUserId()
		{
			var userIdClaim = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
				?? throw new InvalidOperationException("User ID claim is missing."));
			return userIdClaim;
		}
	}
}
