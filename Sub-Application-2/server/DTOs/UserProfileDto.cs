using System;

namespace server.DTOs
{
    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string ProfilePictureUrl { get; set; }
        public DateTime DateJoined { get; set; }
        public int PostCount { get; set; }
        public string Bio { get; set; }
    }
}
