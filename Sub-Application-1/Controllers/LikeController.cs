using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Sub_Application_1.Data;
using Sub_Application_1.DTOs;
using Sub_Application_1.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;


namespace Sub_Application_1.Controllers
{
    [Route("Likes")]
    public class LikeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public LikeController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Likes/LikeButtonPartial?postId=5
        [HttpGet("LikeButtonPartial")]
        public async Task<IActionResult> GetLikeButtonPartial(int postId)
        {
            // Fetch the post along with its likes and associated users.
            var post = await _context.Posts
                .Include(p => p.Likes)
                .ThenInclude(l => l.User)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
            {
                return NotFound("Post not found.");
            }
            //add the data into a PostDto object
            var postDto = new PostDto
            {
                PostId = post.PostId,
                LikesCount = post.Likes.Count,
                Likes = post.Likes.Select(l => new UserDto
                {
                    UserId = l.User.Id,
                    Username = l.User.UserName,
                    ProfilePictureUrl = l.User.ProfilePictureUrl
                }).ToList()
            };

            return PartialView("LikeButtonPartial", postDto);
        }

        // POST: Likes/LikePost
        [Authorize]
        [HttpPost("LikePost")]
        public async Task<IActionResult> LikePost(int postId)
        {
            var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				ModelState.AddModelError("User", "User not found");
				Console.WriteLine("User not found");
				return View();
			}
            string userId = user.Id;
            // Prevent duplicate likes from the same user on the same post. shouldnt be possible, but in case
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

            return RedirectToAction("GetLikeButtonPartial", new { postId });
        }

        // POST: Likes/UnlikePost
        [Authorize]
        [HttpPost("UnlikePost")]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				ModelState.AddModelError("User", "User not found");
				Console.WriteLine("User not found");
				return View();
			}
            string userId = user.Id;


            var like = await _context.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
            
            // Prevent user from unliking a post they have not liked. shouldnt be possible, but just in case
            if (like == null)
            {
                return NotFound("You have not liked this post.");
            }

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return RedirectToAction("GetLikeButtonPartial", new { postId });
        }

    }
}
