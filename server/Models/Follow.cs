using System;

namespace server.Models
{
    public class Follow
    {
        public int FollowerId { get; set; } // Composite key
        public int FollowingId { get; set; } // Composite key

        public DateTime DateFollowed { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User Follower { get; set; }
        public User Following { get; set; }
    }
}
