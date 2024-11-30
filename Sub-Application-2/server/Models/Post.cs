using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class Post //
    {
        public int PostId { get; set; }

        [Required]
        public int UserId { get; set; }

        public string? ImagePath { get; set; } 

        [MaxLength(1000)]
        public string? TextContent { get; set; }

        public DateTime DateUploaded { get; set; } = DateTime.Now;

        // New fields for custom styling
        public int FontSize { get; set; } = 16;
        public string TextColor { get; set; } = "#000000";
        public string BackgroundColor { get; set; } = "#FFFFFF";

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Like> Likes { get; set; } = new HashSet<Like>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
