using server.Models;
using System.IO;
using System.Linq;

namespace server.Data
{
    public static class SeedData
    {
        // Initializes the database with seed data for Users, Posts, Comments, and Likes
        public static void Initialize(AppDbContext context)
        {
            // Seed Users if none exist
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User { Username = "barbie", Email = "barbie@example.com", ProfilePictureUrl = "/images/seed/user1.jpg" },
                    new User { Username = "Loony Racoony", Email = "Racoony@example.com", ProfilePictureUrl = "/images/seed/user2.jpg", Bio = "Im the Grapist and im gonna Grape you" },
                    new User { Username = "duckface", Email = "ducky@example.com", ProfilePictureUrl = "/images/seed/user3.jpg", Bio = "I'm a duck, quack quack" },
                    new User { Username = "Mr Turtle", Email = "Turtle.Md@example.com", ProfilePictureUrl = "/images/seed/user4.jpg", Bio = "Hate straws, but love a milkshake" },
                    new User { Username = "Red", Email = "sara@example.com", ProfilePictureUrl = "/images/seed/user5.jpg" }
                );
                context.SaveChanges();
            }

            // Fetch seeded users
            var barbie = context.Users.SingleOrDefault(u => u.Username == "barbie");
            var racoony = context.Users.SingleOrDefault(u => u.Username == "Loony Racoony");
            var duckface = context.Users.SingleOrDefault(u => u.Username == "duckface");
            var turtle = context.Users.SingleOrDefault(u => u.Username == "Mr Turtle");
            var red = context.Users.SingleOrDefault(u => u.Username == "Red");

            // Ensure all users were found
            if (barbie == null || racoony == null || duckface == null || turtle == null || red == null)
            {
                throw new InvalidOperationException("Seeding failed: one or more users could not be found.");
            }

            // Seed Posts if none exist
            if (!context.Posts.Any())
            {
                context.Posts.AddRange(
                    // Barbie's posts
                    new Post { UserId = barbie.UserId, ImagePath = "/images/seed/user1img1.jpg", DateUploaded = DateTime.Now.AddHours(-5) },
                    new Post { UserId = barbie.UserId, ImagePath = "/images/seed/user1img2.jpg", DateUploaded = DateTime.Now.AddHours(-3) },
                    new Post { UserId = barbie.UserId, TextContent = "Exploring the outdoors!", FontSize = 18, TextColor = "#000000", BackgroundColor = "#FFFFFF", DateUploaded = DateTime.Now.AddHours(-4) },

                    // Racoony's posts
                    new Post { UserId = racoony.UserId, ImagePath = "/images/seed/user2img1.webp", DateUploaded = DateTime.Now.AddHours(-6) },
                    new Post { UserId = racoony.UserId, ImagePath = "/images/seed/user2img2.gif", DateUploaded = DateTime.Now.AddHours(-3) },
                    new Post { UserId = racoony.UserId, TextContent = "I love the beach!", FontSize = 16, TextColor = "#000000", BackgroundColor = "#FFFFFF", DateUploaded = DateTime.Now.AddHours(-1) },

                    // Turtle's deeply emotional post
                    new Post { UserId = turtle.UserId, TextContent = "Living a life as a turtle can be profoundly isolating and melancholic, a slow existence where each step feels like an eternity. Imagine carrying the weight of your home on your back, always a part of you, yet never a source of warmth or comfort. You move at a pace that allows the world to pass you by, and though you are surrounded by nature’s beauty, you can rarely interact with it in any meaningful way. Predators see you as nothing more than a hard meal to crack, and even the creatures you might call companions view you with indifference. Your life is a constant struggle between survival and insignificance, where the vastness of the ocean or the emptiness of the land only serves as a reminder of how small and vulnerable you truly are. And worst of all, your lifespan, though long, stretches out the loneliness, ensuring that every year feels like a decade, every moment a lifetime of sadness, as you crawl through a world that often seems like it has forgotten you exist.", FontSize = 18, TextColor = "#2ECC71", BackgroundColor = "#FFFFFF", DateUploaded = DateTime.Now.AddHours(-7) },

                    // Red's posts
                    new Post { UserId = red.UserId, TextContent = "Running into the weekend!", FontSize = 16, TextColor = "#E74C3C", BackgroundColor = "#F8F8F8", DateUploaded = DateTime.Now.AddHours(-9) }
                );
                context.SaveChanges();
            }

            // Seed Comments if none exist
            if (!context.Comments.Any())
            {
                var posts = context.Posts.ToList();

                context.Comments.AddRange(
                    new Comment { PostId = posts[0].PostId, UserId = racoony.UserId, Content = "Looks great, Barbie!" },
                    new Comment { PostId = posts[3].PostId, UserId = barbie.UserId, Content = "Amazing photo!" }
                );
                context.SaveChanges();
            }

            // Seed Likes if none exist
            if (!context.Likes.Any())
            {
                var posts = context.Posts.ToList();
                var users = new[] { barbie, racoony, duckface, turtle, red };
                var random = new Random();

                foreach (var user in users)
                {
                    foreach (var post in posts)
                    {
                        if (random.NextDouble() > 0.5)
                        {
                            context.Likes.Add(new Like
                            {
                                UserId = user.UserId,
                                PostId = post.PostId,
                            });
                        }
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
