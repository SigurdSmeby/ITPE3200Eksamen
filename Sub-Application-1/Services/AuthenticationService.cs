using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sub_Application_1.Data;
using Sub_Application_1.Models;
using System.Threading.Tasks;

namespace Sub_Application_1.Services
{
    public class AuthenticationService
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthenticationService(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<User> AuthenticateAsync(string usernameOrEmail, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u =>
                u.UserName == usernameOrEmail || u.Email == usernameOrEmail);

            if (user == null)
                return null;

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            return verificationResult == PasswordVerificationResult.Success ? user : null;
        }

        public async Task<User> RegisterAsync(User newUser, string password)
        {
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, password);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        public async Task<User> GetUserByUsernameOrEmailAsync(string username, string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username || u.Email == email);
        }
    }
}
