using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Sub_Application_1.DTOs;
using Sub_Application_1.Models;

namespace Sub_Application_1.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        // methods for user retrieval (R)
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserAsync(System.Security.Claims.ClaimsPrincipal principal);

        // methods for user management (CUD)
        Task<IdentityResult> RegisterUserAsync(RegisterDto registerDto);
        Task<SignInResult> LoginAsync(LoginDto loginDto);
        Task SignInAsync(User user, bool isPersistent); 
        Task LogoutAsync();
        Task SignOutAsync();
        Task<IdentityResult> UpdateUserAsync(User user);
        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
        Task<IdentityResult> DeleteUserAsync(User user);
       
    }
}
