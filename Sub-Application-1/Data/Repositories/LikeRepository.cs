using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sub_Application_1.Data.Repositories.Interfaces;
using Sub_Application_1.Models;

namespace Sub_Application_1.Data.Repositories
{
	public class LikeRepository : ILikeRepository
	{
		private readonly AppDbContext _context;

		public LikeRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task AddLikeAsync(Like like)
		{
			_context.Likes.Add(like);
			await _context.SaveChangesAsync();
		}

		public async Task RemoveLikeAsync(Like like)
		{
			_context.Likes.Remove(like);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> LikeExistsAsync(string userId, int postId)
		{
			return await _context.Likes.AnyAsync(l => l.UserId == userId && l.PostId == postId);
		}

		public async Task<Like?> GetLikeAsync(string userId, int postId)
		{
			return await _context.Likes
				.FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
		}

		public async Task<Post?> GetPostWithLikesAsync(int postId)
		{
			return await _context.Posts
				.Include(p => p.Likes)
					.ThenInclude(l => l.User)
				.FirstOrDefaultAsync(p => p.PostId == postId);
		}
	}
}
