using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(100), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string ProfilePictureUrl { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Bio { get; set; } = "Welcome to my page";

        public DateTime DateJoined { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();
        public virtual ICollection<Like> Likes { get; set; } = new HashSet<Like>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
