using server.Models;

namespace server.DTOs
{
    public class UpdateProfileDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public string? Bio { get; set; }
    }
}
