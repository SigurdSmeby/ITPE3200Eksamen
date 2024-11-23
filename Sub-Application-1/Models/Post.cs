using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace server.Models
{
    public class Post
    {
        public int PostId { get; set; }

        [Required]
        public int UserId { get; set; } // Foreign key

        public string? ImageUrl { get; set; } // Optional for text-only posts

        [MaxLength(255)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? TextContent { get; set; } // Optional text content

        public DateTime DateUploaded { get; set; } = DateTime.Now;

        // Styling fields
        public int FontSize { get; set; } = 16; // Default font size
        public string TextColor { get; set; } = "#000000"; // Default text color (black)
        public string BackgroundColor { get; set; } = "#FFFFFF"; // Default background color (white)

        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        /// <summary>
        /// Checks if the current user has liked the post.
        /// </summary>
        /// <param name="currentUserId">ID of the current user.</param>
        /// <returns>True if the user has liked the post; otherwise, false.</returns>
        public bool IsLikedByCurrentUser(int currentUserId)
        {
            return Likes.Any(like => like.UserId == currentUserId);
        }
    }
}
