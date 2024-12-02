using System.Threading.Tasks;
using System.Collections.Generic;
using Sub_Application_1.Models;

namespace Sub_Application_1.Data.Repositories.Interfaces
{
	public interface ILikeRepository
	{
		Task AddLikeAsync(Like like);
		Task RemoveLikeAsync(Like like);
		Task<bool> LikeExistsAsync(string userId, int postId);
		Task<Like?> GetLikeAsync(string userId, int postId);
		Task<Post?> GetPostWithLikesAsync(int postId);
	}
}
