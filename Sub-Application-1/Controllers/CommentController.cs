using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sub_Application_1.DTOs;
using Sub_Application_1.Models;
using Sub_Application_1.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Sub_Application_1.Controllers
{
	[Route("Comments")]
	public class CommentController : Controller
	{
		private readonly ICommentRepository _commentRepository;
		private readonly IUserRepository _userRepository;
		private readonly ILogger<CommentController> _logger;

		public CommentController(
			ICommentRepository commentRepository,
			IUserRepository userRepository,
			ILogger<CommentController> logger)
		{
			_commentRepository = commentRepository;
			_userRepository = userRepository;
			_logger = logger;
		}

		// GET: Comments/CommentsListPartial?postId=5
		[HttpGet("CommentsListPartial")]
		public async Task<IActionResult> GetCommentsListPartial(int postId)
		{
			_logger.LogInformation("Fetching comments for post ID {PostId}", postId);
			var comments = await _commentRepository.GetCommentsByPostIdAsync(postId);

			var commentDtos = comments.Select(c => new CommentDto
			{
				CommentId = c.CommentId,
				Content = c.Content,
				DateCommented = c.DateCommented,
				AuthorUsername = c.User != null && c.User.UserName != null ? c.User.UserName : "[Deleted]"
			}).ToList();

			_logger.LogInformation("Successfully fetched {CommentCount} comments for post ID {PostId}", commentDtos.Count, postId);
			return PartialView("CommentsListPartial", commentDtos);
		}

		// POST: Comments/AddComment
		[Authorize]
		[HttpPost("AddComment")]
		public async Task<IActionResult> AddComment(AddCommentDto commentDto)
		{
			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Invalid comment data provided.");
				return BadRequest("Invalid comment data.");
			}

			var user = await _userRepository.GetUserAsync(User);
			if (user == null)
			{
				_logger.LogWarning("Unauthorized comment addition attempt.");
				return Unauthorized();
			}
			string userId = user.Id;

			_logger.LogInformation("User {UserId} is adding a comment to post ID {PostId}", user.Id, commentDto.PostId);
			var comment = new Comment
			{
				UserId = userId,
				PostId = commentDto.PostId,
				Content = commentDto.Content,
				DateCommented = DateTime.UtcNow
			};

			await _commentRepository.AddAsync(comment);
			await _commentRepository.SaveAsync();
			_logger.LogInformation("Comment added successfully by user {UserId} for post ID {PostId}", user.Id, commentDto.PostId);

			return RedirectToAction("GetCommentsListPartial", new { postId = commentDto.PostId });
		}

		// POST: Comments/DeleteComment
		[Authorize]
		[HttpPost("DeleteComment")]
		public async Task<IActionResult> DeleteComment(int id)
		{
			var user = await _userRepository.GetUserAsync(User);
			if (user == null)
			{
				_logger.LogWarning("Unauthorized comment deletion attempt.");
				return Unauthorized();
			}
			string userId = user.Id;
			_logger.LogInformation("User {UserId} is attempting to delete comment ID {CommentId}", user.Id, id);

			// Find the comment by ID
			var comment = await _commentRepository.GetCommentByIdAsync(id);

			if (comment == null)
			{
				_logger.LogWarning("Comment ID {CommentId} not found for deletion.", id);
				return NotFound("Comment not found.");
			}

			// Check if the user is authorized to delete the comment
			if (comment.UserId != userId)
			{
				_logger.LogWarning("User {UserId} is not authorized to delete comment ID {CommentId}", user.Id, id);
				return Forbid("You are not authorized to delete this comment.");
			}

			// Retrieve the Post ID from the comment
			var postId = comment.PostId;

			// Delete the comment
			_commentRepository.Delete(comment);
			await _commentRepository.SaveAsync();
			_logger.LogInformation("Comment ID {CommentId} deleted successfully by user {UserId}", id, user.Id);

			// Fetch the updated comments list for the post
			var comments = await _commentRepository.GetCommentsByPostIdAsync(postId);

			var commentDtos = comments.Select(c => new CommentDto
			{
				CommentId = c.CommentId,
				Content = c.Content,
				DateCommented = c.DateCommented,
				AuthorUsername = c.User != null && c.User.UserName != null ? c.User.UserName : "[Deleted]"
			}).ToList();

			// Return the updated partial view
			_logger.LogInformation("Returning updated comments list for post ID {PostId}", postId);
			return PartialView("CommentsListPartial", commentDtos);
		}

	}
}
