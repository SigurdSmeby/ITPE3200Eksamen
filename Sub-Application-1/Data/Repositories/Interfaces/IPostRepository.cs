using System.Collections.Generic;
using System.Threading.Tasks;
using Sub_Application_1.Models;

namespace Sub_Application_1.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Post entity.
    /// </summary>
    public interface IPostRepository : IRepository<Post>
    {
        // Get posts by a specific user
        Task<IEnumerable<Post>> GetPostsByUserIdAsync(string userId);

        // Get posts with related data
        Task<IEnumerable<Post>> GetAllWithDetailsAsync();

        // Additional methods specific to Post can be added here
    }
}
