using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required, MaxLength(100), EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string ProfilePictureUrl { get; set; } = "default_profile_pic.jpg";

        public ThemePreference ThemePreference { get; set; } = ThemePreference.Day;

        public DateTime DateJoined { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Post> Posts { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public ICollection<Follow> Followers { get; set; } // Users who follow this user
        public ICollection<Follow> Following { get; set; } // Users this user follows
    }

    public enum ThemePreference
    {
        Day,
        Night
    }
}
