using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sub_Application_1.Data;
using Sub_Application_1.Models;
using Sub_Application_1.Repositories.Interfaces;

namespace Sub_Application_1.Repositories
{
    /// <summary>
    /// Repository implementation for Post entity.
    /// </summary>
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Get all posts with related comments and likes.
        /// </summary>
        public async Task<IEnumerable<Post>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .Include(p => p.User)
                .ToListAsync();
        }

        /// <summary>
        /// Get posts by a specific user.
        /// </summary>
        public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(string userId)
        {
            return await _dbSet
                .Where(p => p.UserId == userId)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .ToListAsync();
        }

        // Implement additional methods specific to Post here
    }
}
