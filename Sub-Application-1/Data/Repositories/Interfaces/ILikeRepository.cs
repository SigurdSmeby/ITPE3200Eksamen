using System.Threading.Tasks;
using Sub_Application_1.Models;

namespace Sub_Application_1.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Like entity.
    /// </summary>
    public interface ILikeRepository : IRepository<Like>
    {
    Task<Like?> GetLikeAsync(string userId, int postId);
    Task<bool> UserHasLikedPostAsync(string userId, int postId);
    }
}
