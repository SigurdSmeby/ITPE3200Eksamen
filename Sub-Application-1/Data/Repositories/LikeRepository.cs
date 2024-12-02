using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sub_Application_1.Data.Repositories.Interfaces;
using Sub_Application_1.Models;

namespace Sub_Application_1.Data.Repositories
{
	public class LikeRepository : ILikeRepository
	{
		private readonly AppDbContext _context;
		private readonly ILogger<LikeRepository> _logger;
		public LikeRepository(AppDbContext context, ILogger<LikeRepository> logger)
		{
			_context = context;
			_logger = logger; // Initialize the logger
			_logger.LogInformation("LikeRepository initialized.");
		}

		public async Task AddLikeAsync(Like like)
		{
			_context.Likes.Add(like);
			await _context.SaveChangesAsync();
			_logger.LogInformation("Like added successfully for UserId: {UserId}, PostId: {PostId}", like.UserId, like.PostId);
		}

		public async Task RemoveLikeAsync(Like like)
		{
			_context.Likes.Remove(like);
			await _context.SaveChangesAsync();
			_logger.LogInformation("Like removed successfully for UserId: {UserId}, PostId: {PostId}", like.UserId, like.PostId);
		}

		public async Task<bool> LikeExistsAsync(string userId, int postId)
		{
			_logger.LogInformation("Checking if like exists for UserId: {UserId}, PostId: {PostId}", userId, postId);

			bool exists = await _context.Likes.AnyAsync(l => l.UserId == userId && l.PostId == postId);

			_logger.LogInformation("Like exists for UserId: {UserId}, PostId: {PostId}: {Exists}", userId, postId, exists);
			return exists;
		}

		public async Task<Like?> GetLikeAsync(string userId, int postId)
		{
			_logger.LogInformation("Fetching like for UserId: {UserId}, PostId: {PostId}", userId, postId);
			return await _context.Likes
				.FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
		}

		public async Task<Post?> GetPostWithLikesAsync(int postId)
		{
			_logger.LogInformation("Fetching post with likes for PostId: {PostId}", postId);
			return await _context.Posts
				.Include(p => p.Likes)
					.ThenInclude(l => l.User)
				.FirstOrDefaultAsync(p => p.PostId == postId);
		}
	}
}
