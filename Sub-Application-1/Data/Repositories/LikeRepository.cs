using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sub_Application_1.Data;
using Sub_Application_1.Models;
using Sub_Application_1.Repositories.Interfaces;

namespace Sub_Application_1.Repositories
{
    /// <summary>
    /// Repository implementation for Like entity.
    /// </summary>
    public class LikeRepository : Repository<Like>, ILikeRepository
    {
        public LikeRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Get a Like by User ID and Post ID.
        /// </summary>
        public async Task<Like?> GetLikeAsync(string userId, int postId)
        {
            return await _dbSet
                .Include(l => l.User)
                .Include(l => l.Post)
                .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
        }

        /// <summary>
        /// Count the number of likes for a specific post.
        /// </summary>
        public async Task<int> CountLikesByPostIdAsync(int postId)
        {
            return await _dbSet.CountAsync(l => l.PostId == postId);
        }

        // Implement additional methods specific to Like here
    }
}
