using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace server.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Follow> Follows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the self-referencing many-to-many relationship for Follow
            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Followed)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowedId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        // Seed sample data method
public void SeedDatabase()
{
    // Only seed data if there are no users
    if (!Users.Any())
    {
        // Use a plain-text password for demo purposes
        string plainTextPassword = "123"; // Temporary password for testing

        // Add sample users with plain-text passwords
        var users = new[]
        {
            new User { Username = "john_doe", Email = "john@example.com", Bio = "Hello, I'm John!", ProfilePic = "https://picsum.photos/200", PasswordHash = plainTextPassword },
            new User { Username = "jane_smith", Email = "jane@example.com", Bio = "Jane here, I love photography!", ProfilePic = "https://picsum.photos/200", PasswordHash = plainTextPassword },
            new User { Username = "michael_wilson", Email = "michael@example.com", Bio = "Michael the adventurer!", ProfilePic = "https://picsum.photos/200", PasswordHash = plainTextPassword },
        };

        Users.AddRange(users);
        SaveChanges();

        // Get the created user IDs
        var john = Users.FirstOrDefault(u => u.Username == "john_doe");
        var jane = Users.FirstOrDefault(u => u.Username == "jane_smith");
        var michael = Users.FirstOrDefault(u => u.Username == "michael_wilson");

        // Add sample posts for each user
        var posts = new[]
        {
            new Post { UserId = john.Id, Caption = "John's first post!", ImageUrl = "https://picsum.photos/600/400?random=1" },
            new Post { UserId = jane.Id, Caption = "Jane's amazing capture!", ImageUrl = "https://picsum.photos/600/400?random=2" },
            new Post { UserId = michael.Id, Caption = "Michael's adventure!", ImageUrl = "https://picsum.photos/600/400?random=3" },
            new Post { UserId = john.Id, Caption = "Another one from John!", ImageUrl = "https://picsum.photos/600/400?random=4" },
            new Post { UserId = jane.Id, Caption = "A scenic view by Jane.", ImageUrl = "https://picsum.photos/600/400?random=5" }
        };

        Posts.AddRange(posts);
        SaveChanges();
    }
}


    }
}
