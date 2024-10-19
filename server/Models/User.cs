namespace server.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } // No need to specify max length for SQLite
        public string Email { get; set; }    // No need to specify max length for SQLite
        public string PasswordHash { get; set; }
        public string ProfilePic { get; set; }
        public string Bio { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Relationships
        public ICollection<Post> Posts { get; set; }
        public ICollection<Follow> Followers { get; set; }
        public ICollection<Follow> Following { get; set; }
    }
}
