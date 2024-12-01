using System.Collections.Generic;
using System.Threading.Tasks;
using Sub_Application_1.Models;

namespace Sub_Application_1.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Comment entity.
    /// </summary>
    public interface ICommentRepository : IRepository<Comment>
    {
        // Get comments by Post ID
        Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId);

        // Additional methods specific to Comment can be added here
    }
}
