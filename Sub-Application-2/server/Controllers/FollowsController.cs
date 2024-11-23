using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.DTOs;
using server.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FollowsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FollowsController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Follows/follow/5
        [Authorize]
        [HttpPost("follow/{followingId}")]
        public async Task<IActionResult> FollowUser(int followingId)
        {
            int followerId = GetCurrentUserId();

            if (followerId == followingId)
                return BadRequest("You cannot follow yourself.");

            if (await _context.Follows.AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId))
            {
                return BadRequest("You are already following this user.");
            }

            var follow = new Follow
            {
                FollowerId = followerId,
                FollowingId = followingId
            };

            _context.Follows.Add(follow);
            await _context.SaveChangesAsync();

            return Ok("User followed successfully.");
        }

        // DELETE: api/Follows/unfollow/5
        [Authorize]
        [HttpDelete("unfollow/{followingId}")]
        public async Task<IActionResult> UnfollowUser(int followingId)
        {
            int followerId = GetCurrentUserId();

            var follow = await _context.Follows.FindAsync(followerId, followingId);

            if (follow == null)
                return NotFound("You are not following this user.");

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();

            return Ok("User unfollowed successfully.");
        }

        // GET: api/Follows/followers/5
        [HttpGet("followers/{userId}")]
        public async Task<IActionResult> GetFollowers(int userId)
        {
            var followers = await _context.Follows
                .Where(f => f.FollowingId == userId)
                .Include(f => f.Follower)
                .Select(f => new UserDto
                {
                    UserId = f.Follower.UserId,
                    Username = f.Follower.Username,
                    ProfilePictureUrl = f.Follower.ProfilePictureUrl
                })
                .ToListAsync();

            return Ok(followers);
        }

        // GET: api/Follows/following/5
        [HttpGet("following/{userId}")]
        public async Task<IActionResult> GetFollowing(int userId)
        {
            var following = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Include(f => f.Following)
                .Select(f => new UserDto
                {
                    UserId = f.Following.UserId,
                    Username = f.Following.Username,
                    ProfilePictureUrl = f.Following.ProfilePictureUrl
                })
                .ToListAsync();

            return Ok(following);
        }

        // Helper method
        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
