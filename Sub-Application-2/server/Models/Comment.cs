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
        [MaxLength(1000)] // Optional: Limit the length of the content
        public string Content { get; set; } = string.Empty;

        public DateTime DateCommented { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual User User { get; set; } = new User();
        public virtual Post Post { get; set; } = new Post();
    }
}
