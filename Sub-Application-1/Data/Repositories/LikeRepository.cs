using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sub_Application_1.Data;
using Sub_Application_1.Models;
using Sub_Application_1.Repositories.Interfaces;

namespace Sub_Application_1.Repositories
{
    public class LikeRepository : Repository<Like>, ILikeRepository
    {
        public LikeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Like?> GetLikeAsync(string userId, int postId)
        {
            return await _dbSet.FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
        }

        public async Task<bool> UserHasLikedPostAsync(string userId, int postId)
        {
            return await _dbSet.AnyAsync(l => l.UserId == userId && l.PostId == postId);
        }

    }
}
