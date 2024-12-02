using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sub_Application_1.Data;
using Sub_Application_1.DTOs;
using Sub_Application_1.Models;
using Sub_Application_1.Repositories.Interfaces;

namespace Sub_Application_1.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        //User managers from identity framework
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<UserRepository> _logger;

        // Constructor
        public UserRepository(AppDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, ILogger<UserRepository> logger) : base(context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _logger.LogInformation("UserRepository initialized.");
        }

        //methods are self-explanatory in their naming
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            _logger.LogInformation("Fetching user by username: {Username}", username);
            var user = await _dbSet.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                _logger.LogWarning("User with username {Username} not found.", username);
            }
            else
            {
                _logger.LogInformation("User with username {Username} fetched successfully.", username);
            }
            return user;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            _logger.LogInformation("Fetching user by email: {Email}", email);
            var user = await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found.", email);
            }
            else
            {
                _logger.LogInformation("User with email {Email} fetched successfully.", email);
            }
            return user;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterDto registerDto)
        {
            _logger.LogInformation("Registering new user with username: {Username}", registerDto.Username);
            var user = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User with username {Username} registered successfully.", registerDto.Username);
                await _signInManager.SignInAsync(user, isPersistent: true);
            }
            else
            {
                _logger.LogWarning("User registration failed for username {Username}. Errors: {Errors}",
                    registerDto.Username, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return result;
        }

        public async Task<SignInResult> LoginAsync(LoginDto loginDto)
        {
            _logger.LogInformation("User attempting login with username: {Username}", loginDto.Username);
            var result = await _signInManager.PasswordSignInAsync(
                loginDto.Username,
                loginDto.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Username} logged in successfully.", loginDto.Username);
            }
            else
            {
                _logger.LogWarning("Login failed for username {Username}.", loginDto.Username);
            }

            return result;
        }

        public async Task LogoutAsync()
        {
            _logger.LogInformation("User logging out.");
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out successfully.");
        }
        public async Task SignInAsync(User user, bool isPersistent)
        {
            await _signInManager.SignInAsync(user, isPersistent);
        }

        public async Task SignOutAsync()
        {
            _logger.LogInformation("User sign-out initiated.");

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User signed out successfully.");
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            _logger.LogInformation("Changing password for user with ID: {UserId}", user.Id);
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation("Password changed successfully for user with ID: {UserId}", user.Id);
            }
            else
            {
                _logger.LogWarning("Failed to change password for user with ID: {UserId}. Errors: {Errors}",
                    user.Id, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            return result;
        }
        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            _logger.LogInformation("Updating user with ID: {UserId}", user.Id);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID: {UserId} updated successfully.", user.Id);
            }
            else
            {
                _logger.LogWarning("Failed to update user with ID: {UserId}. Errors: {Errors}",
                    user.Id, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            return result;
        }
        public async Task<IdentityResult> DeleteUserAsync(User user)
        {
            _logger.LogInformation("Attempting to delete user with ID: {UserId}", user.Id);

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("Successfully deleted user with ID: {UserId}", user.Id);
            }
            else
            {
                _logger.LogWarning("Failed to delete user with ID: {UserId}. Errors: {Errors}",
                    user.Id, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return result;
        }

        public async Task<User?> GetUserAsync(System.Security.Claims.ClaimsPrincipal principal)
        {
            var userId = _userManager.GetUserId(principal);
            if (userId == null)
            {
                _logger.LogWarning("User ID from claims principal is null.");
                return null;
            }

            var user = await _userManager.Users
                .Include(u => u.Likes)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
            }
            else
            {
                _logger.LogInformation("User with ID {UserId} fetched successfully.", userId);
            }

            return user;
        }
    }
}