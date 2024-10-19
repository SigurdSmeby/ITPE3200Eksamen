namespace server.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }  // Foreign key
        public string Caption { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Relationships
        public User User { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}
