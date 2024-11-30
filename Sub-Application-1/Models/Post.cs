using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sub_Application_1.Models
{
    public class Post
    {
        public int PostId { get; set; }

        public required String UserId { get; set; }

        public string? ImagePath { get; set; }

        [MaxLength(1000)]
        public string? TextContent { get; set; }

        public DateTime DateUploaded { get; set; } = DateTime.Now;

        public int FontSize { get; set; } = 16; 
        public string TextColor { get; set; } = "#000000"; 
        public string BackgroundColor { get; set; } = "#FFFFFF"; 

        // Navigation properties
        public virtual User User { get; set; } = default!;
        public int FontSize { get; set; } = 16;
        public string TextColor { get; set; } = "#000000";
        public string BackgroundColor { get; set; } = "#FFFFFF";
        public virtual User User { get; set; } = default!;
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
