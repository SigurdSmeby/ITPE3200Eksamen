using System.Collections.Generic;
using System.Threading.Tasks;
using Sub_Application_1.Models;

namespace Sub_Application_1.Repositories.Interfaces
{
    public interface IPostRepository : IRepository<Post>
    {
        // this is all read bby
        Task<IEnumerable<Post>> GetAllPostsWithDetailsAsync();
        Task<IEnumerable<Post>> GetPostsByUserIdAsync(string userId);
        Task<Post?> GetPostByIdAsync(int postId);
        Task<Post?> GetPostWithDetailsAsync(int postId);
    }
}
