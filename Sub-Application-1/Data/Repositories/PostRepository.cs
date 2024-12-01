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
        public PostRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Post>> GetAllPostsWithDetailsAsync()
        {
            return await _dbSet
                .OrderByDescending(p => p.DateUploaded)
                .Include(p => p.User)
                .Include(p => p.Likes)
                .ThenInclude(l => l.User)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(string userId)
        {
            return await _dbSet
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.DateUploaded)
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .ToListAsync();
        }

        public async Task<Post?> GetPostByIdAsync(int postId)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.PostId == postId);
        }

        public async Task<Post?> GetPostWithDetailsAsync(int postId)
        {
            return await _dbSet
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.PostId == postId);
        }
        public async Task<Post?> GetPostWithLikesAsync(int postId)
        {
            return await _dbSet
                .Include(p => p.Likes)
                .ThenInclude(l => l.User)
                .FirstOrDefaultAsync(p => p.PostId == postId);
        }
    }
}
