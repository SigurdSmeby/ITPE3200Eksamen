using server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        // Retrieve all users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // GET: api/Users/{id}
        // Retrieve a single user by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/Users
        // Create a new user
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User newUser)
        {
            // Ensure unique email and username
            if (_context.Users.Any(u => u.Email == newUser.Email || u.Username == newUser.Username))
            {
                return BadRequest("Email or Username already in use.");
            }

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
        }

        // PUT: api/Users/{id}
        // Update an existing user by ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.Id)
            {
                return BadRequest("User ID mismatch.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Update fields
            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.Bio = updatedUser.Bio;
            user.ProfilePic = updatedUser.ProfilePic;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(u => u.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Users/{id}
        // Delete a user by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Users/login
        // Simple login API for testing purposes (using plain-text password)
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == request.Email);
            
            if (user == null)
            {
                return Unauthorized("User not found");
            }

            // Compare the plain-text password for testing (should use hashing in production)
            if (user.PasswordHash != request.Password)
            {
                return Unauthorized("Invalid credentials");
            }

            // Return success for login (You can implement token or session management here)
            return Ok(new { message = "Login successful", user = user.Username });
        }
    }

    // DTO for login request
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
