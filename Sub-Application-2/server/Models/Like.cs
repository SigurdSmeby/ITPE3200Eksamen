using System;

namespace server.Models
{
    public class Like
    {
        public int UserId { get; set; } // Composite key
        public int PostId { get; set; } // Composite key

        public DateTime DateLiked { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Post Post { get; set; } = null!;
    }
}
