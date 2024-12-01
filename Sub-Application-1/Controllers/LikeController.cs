using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sub_Application_1.DTOs;
using Sub_Application_1.Models;
using Sub_Application_1.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Sub_Application_1.Controllers
{
    [Route("Likes")]
    public class LikeController : Controller
    {
        private readonly ILikeRepository _likeRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;

        public LikeController(
            ILikeRepository likeRepository,
            IPostRepository postRepository,
            IUserRepository userRepository)
        {
            _likeRepository = likeRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        // GET: Likes/LikeButtonPartial?postId=5
        [HttpGet("LikeButtonPartial")]
public async Task<IActionResult> GetLikeButtonPartial(int postId)
{
    var post = await _postRepository.GetPostWithDetailsAsync(postId);

    if (post == null)
    {
        return NotFound("Post not found.");
    }

    if (post.Likes == null)
    {
        post.Likes = new List<Like>();
    }

    var postDto = new PostDto
    {
        PostId = post.PostId,
        LikesCount = post.Likes.Count,
        Likes = post.Likes
            .Where(l => l?.User != null) // Add null check for l itself
            .Select(l => new UserDto
            {
                UserId = l.User.Id,
                Username = l.User.UserName,
                ProfilePictureUrl = l.User.ProfilePictureUrl
            })
            .ToList()
    };

    return PartialView("LikeButtonPartial", postDto);
}


        // POST: Likes/LikePost
        [Authorize]
        [HttpPost("LikePost")]
        public async Task<IActionResult> LikePost(int postId)
        {
            var user = await _userRepository.GetUserAsync(User);			
            if (user == null)
			{
                return Unauthorized();
			}
            string userId = user.Id;
            // Prevent duplicate likes from the same user on the same post. shouldnt be possible, but in case
            if (await _likeRepository.UserHasLikedPostAsync(userId, postId))
            {
                return BadRequest("You have already liked this post.");
            }

            var like = new Like
            {
                UserId = userId,
                PostId = postId
            };

            await _likeRepository.AddAsync(like);
            await _likeRepository.SaveAsync();

            return RedirectToAction("GetLikeButtonPartial", new { postId });
        }

        // POST: Likes/UnlikePost
        [Authorize]
        [HttpPost("UnlikePost")]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            var user = await _userRepository.GetUserAsync(User);
            
			if (user == null)
			{
				ModelState.AddModelError("User", "User not found");
				Console.WriteLine("User not found");
				return View();
			}
            string userId = user.Id;


            var like = await _likeRepository.GetLikeAsync(userId, postId);                        
            // Prevent user from unliking a post they have not liked. shouldnt be possible, but just in case
            if (like == null)
            {
                return NotFound("You have not liked this post.");
            }

            _likeRepository.Delete(like);
            await _likeRepository.SaveAsync();

            return RedirectToAction("GetLikeButtonPartial", new { postId });
        }

    }
}
