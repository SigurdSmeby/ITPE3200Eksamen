using System.Collections.Generic;
using System.Threading.Tasks;
using Sub_Application_1.Models;

namespace Sub_Application_1.Repositories.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<List<Comment>> GetCommentsByPostIdAsync(int postId);
        Task<Comment?> GetCommentByIdAsync(int commentId);
    }
}
