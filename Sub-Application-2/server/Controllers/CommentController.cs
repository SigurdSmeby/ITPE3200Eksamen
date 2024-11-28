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

        public CommentsController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Comments
        // Adds a comment to a post. Requires the user to be logged in.
        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> AddComment([FromBody] AddCommentDto commentDto)
        {
            int userId = GetCurrentUserId();

            // Create and save the new comment
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

        // GET: api/Comments/post/{postId}
        // Fetches all comments for a specific post.
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetCommentsForPost(int postId)
        {
            // Query and return comments for the given post, including author info
            var comments = await _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .OrderBy(c => c.DateCommented)
                .Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    Content = c.Content,
                    DateCommented = c.DateCommented,
                    AuthorUsername = c.User.Username
                })
                .ToListAsync();

            return Ok(comments);
        }


        // DELETE: api/Comments/{id}
        // Deletes a comment. Only the author can delete their own comments.
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            int userId = GetCurrentUserId();

            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
                return NotFound();

            if (comment.UserId != userId)
                return Forbid("You are not authorized to delete this comment."); // Unauthorized action

            // Remove the comment
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok("Comment deleted successfully.");
        }

        // Gets the ID of the currently authenticated user.
        private int GetCurrentUserId()
        {
            var userIdClaim = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User ID claim is missing."));
            return userIdClaim;
        }
    }
}
