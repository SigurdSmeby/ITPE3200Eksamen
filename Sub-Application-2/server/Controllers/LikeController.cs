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

        // POST: api/Likes/like/5
        [Authorize]
        [HttpPost("like/{postId}")]
        public async Task<IActionResult> LikePost(int postId)
        {
            int userId = GetCurrentUserId();

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

        // DELETE: api/Likes/unlike/5
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

        // GET: api/Likes/post/5
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetLikesForPost(int postId)
        {
            var likes = await _context.Likes
                .Where(l => l.PostId == postId)
                .Include(l => l.User)
                .Select(l => new UserDto
                {
                    UserId = l.User.UserId,
                    Username = l.User.Username,
                    ProfilePictureUrl = l.User.ProfilePictureUrl
                })
                .ToListAsync();

            return Ok(likes);
        }

        [Authorize]
        [HttpGet("hasLiked/{postId}")]
        public async Task<IActionResult> checkIfLiked(int postId)
        {
            int userId = GetCurrentUserId();

            var like = await _context.Likes.FindAsync(userId, postId);

            if (like == null)
                return Ok(false);

            else
                return Ok(true);
        }

        // Helper method
        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
