using server.Models;

namespace server.DTOs
{
    public class UpdateProfileDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
