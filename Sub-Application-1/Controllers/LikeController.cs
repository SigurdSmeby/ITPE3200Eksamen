using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Sub_Application_1.Data.Repositories.Interfaces;
using Sub_Application_1.DTOs;
using Sub_Application_1.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Sub_Application_1.Controllers
{
	[Route("Likes")]
	public class LikeController : Controller
	{
		private readonly ILikeRepository _likeRepository;
		private readonly UserManager<User> _userManager;

		public LikeController(ILikeRepository likeRepository, UserManager<User> userManager)
		{
			_likeRepository = likeRepository;
			_userManager = userManager;
		}

		// GET: Likes/LikeButtonPartial?postId=5
		[HttpGet("LikeButtonPartial")]
		public async Task<IActionResult> GetLikeButtonPartial(int postId)
		{
			var post = await _likeRepository.GetPostWithLikesAsync(postId);

			if (post == null)
			{
				return NotFound("Post not found.");
			}

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
				return Unauthorized("User not found");
			}

			string userId = user.Id;

			// Prevent duplicate likes
			if (await _likeRepository.LikeExistsAsync(userId, postId))
			{
				return BadRequest("You have already liked this post.");
			}

			var like = new Like
			{
				UserId = userId,
				PostId = postId
			};

			await _likeRepository.AddLikeAsync(like);

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
				return Unauthorized("User not found");
			}

			string userId = user.Id;

			var like = await _likeRepository.GetLikeAsync(userId, postId);

			if (like == null)
			{
				return NotFound("You have not liked this post.");
			}

			await _likeRepository.RemoveLikeAsync(like);

			return RedirectToAction("GetLikeButtonPartial", new { postId });
		}
	}
}
