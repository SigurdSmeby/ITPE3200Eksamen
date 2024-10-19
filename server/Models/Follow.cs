namespace server.Models
{
    public class Follow
    {
        public int Id { get; set; }

        // The user who is following someone
        public int FollowerId { get; set; }  // Foreign key to the Follower
        public User Follower { get; set; }

        // The user who is being followed
        public int FollowedId { get; set; }  // Foreign key to the Followed user
        public User Followed { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
