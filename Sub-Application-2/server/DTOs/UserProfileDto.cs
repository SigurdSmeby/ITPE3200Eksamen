using System;

namespace server.DTOs
{
    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public DateTime DateJoined { get; set; }
        public int PostCount { get; set; }
        public string Bio { get; set; } = string.Empty;
    }
}
