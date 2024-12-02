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
    /// Repository implementation for Comment entity.
    /// </summary>
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Comment>> GetCommentsByPostIdAsync(int postId)
        {
            return await _dbSet
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .OrderBy(c => c.DateCommented)
                .ToListAsync();
        }

        public async Task<Comment?> GetCommentByIdAsync(int commentId)
        {
            return await _dbSet
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.CommentId == commentId);
        }
    }
}
