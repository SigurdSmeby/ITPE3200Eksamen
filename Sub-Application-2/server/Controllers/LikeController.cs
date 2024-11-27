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
    public class LikesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LikesController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Likes/like/{postId}
        // Adds a like to a post for the currently logged-in user.
        [Authorize]
        [HttpPost("like/{postId}")]
        public async Task<IActionResult> LikePost(int postId)
        {
            int userId = GetCurrentUserId();

            // Check if the user has already liked the post
            if (await _context.Likes.AnyAsync(l => l.PostId == postId && l.UserId == userId))
            {
                return BadRequest("You have already liked this post.");
            }

            // Create a new like record
            var like = new Like
            {
                UserId = userId,
                PostId = postId
            };

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            return Ok("Post liked successfully.");
        }

        // DELETE: api/Likes/unlike/{postId}
        // Removes a like from a post for the currently logged-in user.
        [Authorize]
        [HttpDelete("unlike/{postId}")]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            int userId = GetCurrentUserId();

            // Find the like record for the user and post
            var like = await _context.Likes.FindAsync(userId, postId);

            // Return 404 if no like exists
            if (like == null)
                return NotFound("You have not liked this post.");

            // Remove the like record
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return Ok("Post unliked successfully.");
        }

        // GET: api/Likes/hasLiked/{postId}
        // Checks if the currently logged-in user has liked a specific post.
        [Authorize]
        [HttpGet("hasLiked/{postId}")]
        public async Task<IActionResult> checkIfLiked(int postId)
        {
            int userId = GetCurrentUserId();

            // Check if a like exists for the user and post
            var like = await _context.Likes.FindAsync(userId, postId);

            // Return true or false based on the like existence
            return Ok(like != null);
        }

        // Helper method to get the ID of the currently logged-in user.
        private int GetCurrentUserId()
        {
            var userIdClaim = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User ID claim is missing."));
            return userIdClaim;
        }
    }
}
