using System;
using System.Collections.Generic;

namespace server.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;  // Ensure it's initialized
        public DateTime UploadedAt { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string ProfileName { get; set; } = string.Empty;  // Ensure it's initialized
        public string Email { get; set; } = string.Empty;        // Ensure it's initialized
        public string Password { get; set; } = string.Empty;     // Ensure it's initialized
        public string ProfilePicture { get; set; } = string.Empty; // Ensure it's initialized
        public List<Image> Images { get; set; } = new List<Image>(); // Initialize as an empty list
    }
}
