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
        private readonly ILogger<CommentRepository> _logger;
        public CommentRepository(AppDbContext context, ILogger<CommentRepository> logger) : base(context)
        {
            _logger = logger;
            _logger.LogInformation("CommentRepository initialized.");
        }

        public async Task<List<Comment>> GetCommentsByPostIdAsync(int postId)
        {
            _logger.LogInformation("Fetching comments for PostId: {PostId}", postId);

            var comments = await _dbSet
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .OrderBy(c => c.DateCommented)
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} comments for PostId: {PostId}", comments.Count, postId);
            return comments;
        }

        public async Task<Comment?> GetCommentByIdAsync(int commentId)
        {
            _logger.LogInformation("Fetching comment with CommentId: {CommentId}", commentId);

            var comment = await _dbSet
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.CommentId == commentId);

            if (comment == null)
            {
                _logger.LogWarning("No comment found with CommentId: {CommentId}", commentId);
            }
            else
            {
                _logger.LogInformation("Successfully retrieved comment with CommentId: {CommentId}", commentId);
            }

            return comment;
        }
    }
}
