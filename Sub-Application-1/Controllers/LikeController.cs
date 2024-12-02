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
		private readonly ILogger<LikeController> _logger;

		public LikeController(ILikeRepository likeRepository, UserManager<User> userManager, ILogger<LikeController> logger)
		{
			_likeRepository = likeRepository;
			_userManager = userManager;
			_logger = logger;
		}

		// GET: Likes/LikeButtonPartial?postId=5
		[HttpGet("LikeButtonPartial")]
		public async Task<IActionResult> GetLikeButtonPartial(int postId)
		{
			_logger.LogInformation("GetLikeButtonPartial action invoked for postId {PostId}.", postId);
			var post = await _likeRepository.GetPostWithLikesAsync(postId);

			if (post == null)
			{
				_logger.LogWarning("Post with ID {PostId} not found.", postId);
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
			_logger.LogInformation("Returning LikeButtonPartial view for postId {PostId} with {LikesCount} likes.", postId, postDto.LikesCount);
			return PartialView("LikeButtonPartial", postDto);
		}

		// POST: Likes/LikePost
		[Authorize]
		[HttpPost("LikePost")]
		public async Task<IActionResult> LikePost(int postId)
		{
			_logger.LogInformation("LikePost action invoked for postId {PostId}.", postId);
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				_logger.LogWarning("Unauthorized attempt to like post {PostId}.", postId);
				return Unauthorized("User not found");
			}

			string userId = user.Id;

			// Prevent duplicate likes
			if (await _likeRepository.LikeExistsAsync(userId, postId))
			{
				_logger.LogWarning("User {UserId} has already liked post {PostId}.", userId, postId);
				return BadRequest("You have already liked this post.");
			}

			var like = new Like
			{
				UserId = userId,
				PostId = postId
			};

			await _likeRepository.AddLikeAsync(like);
			_logger.LogInformation("User {UserId} liked post {PostId}.", userId, postId);

			return RedirectToAction("GetLikeButtonPartial", new { postId });
		}

		// POST: Likes/UnlikePost
		[Authorize]
		[HttpPost("UnlikePost")]
		public async Task<IActionResult> UnlikePost(int postId)
		{
			_logger.LogInformation("UnlikePost action invoked for postId {PostId}.", postId);
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				_logger.LogWarning("Unauthorized attempt to unlike post {PostId}.", postId);
				return Unauthorized("User not found");
			}

			string userId = user.Id;

			var like = await _likeRepository.GetLikeAsync(userId, postId);

			if (like == null)
			{
				_logger.LogWarning("User {UserId} has not liked post {PostId}.", userId, postId);
				return NotFound("You have not liked this post.");
			}

			await _likeRepository.RemoveLikeAsync(like);
			_logger.LogInformation("User {UserId} unliked post {PostId}.", userId, postId);

			return RedirectToAction("GetLikeButtonPartial", new { postId });
		}
	}
}
