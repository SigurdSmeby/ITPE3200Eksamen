using System;

namespace server.Models
{
    public class Like
    {
        public int UserId { get; set; } // Composite key
        public int PostId { get; set; } // Composite key

        public DateTime DateLiked { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; }
        public Post Post { get; set; }
    }
}
