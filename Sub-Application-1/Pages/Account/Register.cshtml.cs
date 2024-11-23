using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using server.Data;
using server.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace server.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;

        public RegisterModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            // Check if passwords match
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return Page();
            }

            // Check if username already exists
            if (_context.Users.Any(u => u.Username == Username))
            {
                ErrorMessage = "Username already exists.";
                return Page();
            }

            // Check if email already exists
            if (_context.Users.Any(u => u.Email == Email))
            {
                ErrorMessage = "Email already registered.";
                return Page();
            }

            // Hash the password
            var hashedPassword = HashPassword(Password);

            // Create a new user
            var newUser = new User
            {
                Username = Username,
                Email = Email,
                PasswordHash = hashedPassword
            };

            // Save the user to the database
            _context.Users.Add(newUser);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Account created successfully! Please log in.";
            return RedirectToPage("/Account/Login");
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
