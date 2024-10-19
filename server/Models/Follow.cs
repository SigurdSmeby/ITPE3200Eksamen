namespace server.Models
{
    public class Follow
    {
        public int Id { get; set; }
        public int FollowerId { get; set; }  // Foreign key
        public int FollowedId { get; set; }  // Foreign key
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Relationships
        public User Follower { get; set; }
        public User Followed { get; set; }
    }
}
