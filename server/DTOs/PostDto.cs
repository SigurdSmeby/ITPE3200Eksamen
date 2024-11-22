// File: DTOs/PostDto.cs

using System;
using System.Collections.Generic;

namespace server.DTOs
{
    public class PostDto
    {
        public int PostId { get; set; }
        
        public string? ImageUrl { get; set; } // Nullable for text-only posts
        
        public string? TextContent { get; set; } // Nullable for image-only posts

        public string Title { get; set; }
        
        public DateTime DateUploaded { get; set; }
        
        public UserDto Author { get; set; }
        
        public int LikesCount { get; set; }
        
        public List<CommentDto> Comments { get; set; }
        public int CommentsCount { get; set; }
        
        public int FontSize { get; set; } = 16; // Default font size
        
        public string TextColor { get; set; } = "#000000"; // Default text color (black)
        
        public string BackgroundColor { get; set; } = "#FFFFFF"; // Default background color (white)
    }
}



