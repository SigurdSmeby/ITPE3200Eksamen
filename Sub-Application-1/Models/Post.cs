using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sub_Application_1.Models
{
    public class Post
    {
        public int PostId { get; set; }

        public required String UserId { get; set; } // Foreign key

        public string? ImagePath { get; set; } // Path to the uploaded file in wwwroot/uploads

        [MaxLength(1000)]
        public string? TextContent { get; set; } // Optional text content

        public DateTime DateUploaded { get; set; } = DateTime.Now;

        // New fields for custom styling
        public int FontSize { get; set; } = 16; // Default font size
        public string TextColor { get; set; } = "#000000"; // Default text color (black)
        public string BackgroundColor { get; set; } = "#FFFFFF"; // Default background color (white)

        // Navigation properties
        
        public virtual User User { get; set; } = default!; // can be nullable because EF should populate it
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
