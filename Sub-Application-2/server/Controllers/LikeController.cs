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

        // Adds a like to a post by the current user.
        [Authorize]
        [HttpPost("like/{postId}")]
        public async Task<IActionResult> LikePost(int postId)
        {
            int userId = GetCurrentUserId();

            // Check if the user already liked the post
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

        // Removes a like from a post by the current user.
        [Authorize]
        [HttpDelete("unlike/{postId}")]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            int userId = GetCurrentUserId();

            var like = await _context.Likes.FindAsync(userId, postId);

            if (like == null)
                return NotFound("You have not liked this post.");

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return Ok("Post unliked successfully.");
        }

        // Checks if the current user has liked a specific post.
        [Authorize]
        [HttpGet("hasLiked/{postId}")]
        public async Task<IActionResult> CheckIfLiked(int postId)
        {
            int userId = GetCurrentUserId();

            var like = await _context.Likes.FindAsync(userId, postId);

            return Ok(like != null);
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
