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

        public CommentController(
            ICommentRepository commentRepository,
            IUserRepository userRepository)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
        }

        // GET: Comments/CommentsListPartial?postId=5
        [HttpGet("CommentsListPartial")]
        public async Task<IActionResult> GetCommentsListPartial(int postId)
        {
            var comments = await _commentRepository.GetCommentsByPostIdAsync(postId);
            
            var commentDtos = comments.Select(c => new CommentDto
            {
                CommentId = c.CommentId,
                Content = c.Content,
                DateCommented = c.DateCommented,
                AuthorUsername = c.User != null && c.User.UserName != null ? c.User.UserName : "[Deleted]"
            }).ToList();

            return PartialView("CommentsListPartial", commentDtos);
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

            var user = await _userRepository.GetUserAsync(User);
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

            await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveAsync();
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
                return Unauthorized();
            }
            string userId = user.Id;

			// Find the comment by ID
			var comment = await _commentRepository.GetCommentByIdAsync(id);

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
			_commentRepository.Delete(comment);
            await _commentRepository.SaveAsync();

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
			return PartialView("CommentsListPartial", commentDtos);
		}

    }
}
