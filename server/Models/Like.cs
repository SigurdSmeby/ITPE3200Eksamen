using System;

namespace server.Models
{
    public class Like
    {
        public int UserId { get; set; } // Composite key
        public int PostId { get; set; } // Composite key

        public DateTime DateLiked { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Post Post { get; set; }
    }
}
