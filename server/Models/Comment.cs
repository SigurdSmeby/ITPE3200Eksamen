using System;
using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class Comment
    {
        public int CommentId { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime DateCommented { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; }
        public Post Post { get; set; }
    }
}
