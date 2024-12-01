using System.Threading.Tasks;
using Sub_Application_1.Models;

namespace Sub_Application_1.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Like entity.
    /// </summary>
    public interface ILikeRepository : IRepository<Like>
    {
        // Get Like by User ID and Post ID (composite key)
        Task<Like?> GetLikeAsync(string userId, int postId);

        // Count likes for a specific post
        Task<int> CountLikesByPostIdAsync(int postId);

        // Additional methods specific to Like can be added here
    }
}
