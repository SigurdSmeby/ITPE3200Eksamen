using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sub_Application_1.Data;
using Sub_Application_1.Models;
using Sub_Application_1.Repositories.Interfaces;

namespace Sub_Application_1.Repositories
{
	public class PostRepository : Repository<Post>, IPostRepository
	{
		private readonly ILogger<PostRepository> _logger;
		public PostRepository(AppDbContext context, ILogger<PostRepository> logger) : base(context)
		{
			_logger = logger;
		}

		public async Task<IEnumerable<Post>> GetAllPostsWithDetailsAsync()
		{
			_logger.LogInformation("Fetching all posts with details.");

			var posts = await _dbSet
					.OrderByDescending(p => p.DateUploaded)
					.Include(p => p.User)
					.Include(p => p.Likes)
					.ThenInclude(l => l.User)
					.Include(p => p.Comments)
					.ThenInclude(c => c.User)
					.ToListAsync();

			_logger.LogInformation("Successfully fetched all posts with details. Count: {Count}", posts.Count);
			return posts;
		}

		public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(string userId)
		{
			_logger.LogInformation("Fetching posts for user with ID: {UserId}", userId);

			var posts = await _dbSet
					.Where(p => p.UserId == userId)
					.OrderByDescending(p => p.DateUploaded)
					.Include(p => p.User)
					.Include(p => p.Likes)
					.Include(p => p.Comments)
					.ThenInclude(c => c.User)
					.ToListAsync();

			_logger.LogInformation("Successfully fetched posts for user with ID: {UserId}. Count: {Count}", userId, posts.Count);
			return posts;
		}

		public async Task<Post?> GetPostByIdAsync(int postId)
		{
			_logger.LogInformation("Fetching post with ID: {PostId}", postId);

			var post = await _dbSet.FirstOrDefaultAsync(p => p.PostId == postId);

			if (post == null)
			{
					_logger.LogWarning("Post with ID: {PostId} not found.", postId);
			}
			else
			{
					_logger.LogInformation("Successfully fetched post with ID: {PostId}", postId);
			}

			return post;
		}

		public async Task<Post?> GetPostWithDetailsAsync(int postId)
		{
			_logger.LogInformation("Fetching post with details for ID: {PostId}", postId);

			var post = await _dbSet
					.Include(p => p.User)
					.Include(p => p.Likes)
					.Include(p => p.Comments)
					.ThenInclude(c => c.User)
					.FirstOrDefaultAsync(p => p.PostId == postId);

			if (post == null)
			{
					_logger.LogWarning("Post with details for ID: {PostId} not found.", postId);
			}
			else
			{
					_logger.LogInformation("Successfully fetched post with details for ID: {PostId}", postId);
			}

			return post;
		}
		public async Task<Post?> GetPostWithLikesAsync(int postId)
		{
			_logger.LogInformation("Fetching post with likes for ID: {PostId}", postId);

			var post = await _dbSet
					.Include(p => p.Likes)
					.ThenInclude(l => l.User)
					.FirstOrDefaultAsync(p => p.PostId == postId);

			if (post == null)
			{
					_logger.LogWarning("Post with likes for ID: {PostId} not found.", postId);
			}
			else
			{
					_logger.LogInformation("Successfully fetched post with likes for ID: {PostId}", postId);
			}

			return post;
		}
	}
}
