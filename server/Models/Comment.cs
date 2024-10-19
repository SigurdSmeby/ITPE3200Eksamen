namespace server.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }  // Foreign key
        public int UserId { get; set; }  // Foreign key
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Relationships
        public Post Post { get; set; }
        public User User { get; set; }
    }
}
