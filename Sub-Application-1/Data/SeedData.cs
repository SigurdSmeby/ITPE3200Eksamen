using Sub_Application_1.Models;
using System.IO;
using System.Linq;

namespace Sub_Application_1.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            // Seed Users
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User { UserName = "barbie", Email = "barbie@example.com", ProfilePictureUrl = "/images/seed/user1.jpg" },
                    new User { UserName = "Loony Racoony", Email = "Racoony@example.com", ProfilePictureUrl = "/images/seed/user2.jpg", Bio = "Im the Grapist and im gonna Grape you" },
                    new User { UserName = "duckface", Email = "ducky@example.com", ProfilePictureUrl = "/images/seed/user3.jpg", Bio = "I'm a duck, quack quack" },
                    new User { UserName = "Mr Turtle", Email = "Turtle.Md@example.com", ProfilePictureUrl = "/images/seed/user4.jpg", Bio = "Hate straws, but love a milkshake" },
                    new User { UserName = "Red", Email = "sara@example.com", ProfilePictureUrl = "/images/seed/user5.jpg" }
                );
                context.SaveChanges();
            }
            var barbie = context.Users.SingleOrDefault(u => u.UserName == "barbie");
            var racoony = context.Users.SingleOrDefault(u => u.UserName == "Loony Racoony");
            var duckface = context.Users.SingleOrDefault(u => u.UserName == "duckface");
            var turtle = context.Users.SingleOrDefault(u => u.UserName == "Mr Turtle");
            var red = context.Users.SingleOrDefault(u => u.UserName == "Red");

              if (barbie == null || racoony == null || duckface == null || turtle == null || red == null)
              {
                  throw new InvalidOperationException("Seeding failed: one or more users could not be found.");
              }

            // Seed Posts
             if (!context.Posts.Any())
            {
                context.Posts.AddRange(
                    // Posts for barbie
                    new Post { UserId = barbie.Id, ImagePath = "/images/seed/user1img1.jpg", DateUploaded = DateTime.Now.AddHours(-5) },
                    new Post { UserId = barbie.Id, ImagePath = "/images/seed/user1img2.jpg", DateUploaded = DateTime.Now.AddHours(-3) },
                    new Post { UserId = barbie.Id, TextContent = "Exploring the outdoors!", FontSize = 18, TextColor = "#000000", BackgroundColor = "#FFFFFF", DateUploaded = DateTime.Now.AddHours(-4) },

                    // Posts for Loony Racoony
                    new Post { UserId = racoony.Id, ImagePath = "/images/seed/user2img1.webp", DateUploaded = DateTime.Now.AddHours(-6) },
                    new Post { UserId = racoony.Id, ImagePath = "/images/seed/user2img2.gif", DateUploaded = DateTime.Now.AddHours(-3) },
                    new Post { UserId = racoony.Id, ImagePath = "/images/seed/user2img3.gif", DateUploaded = DateTime.Now.AddHours(-2) },
                    new Post { UserId = racoony.Id, ImagePath = "/images/seed/user2img4.gif", DateUploaded = DateTime.Now.AddHours(-2) },
                    new Post { UserId = racoony.Id, TextContent = "I love the beach!", FontSize = 16, TextColor = "#000000", BackgroundColor = "#FFFFFF", DateUploaded = DateTime.Now.AddHours(-1) },
                    new Post { UserId = racoony.Id, TextContent = "Grapes are my new favorite thing", FontSize = 16, TextColor = "#FF5733", BackgroundColor = "#EFEFEF", DateUploaded = DateTime.Now.AddHours(-2) },

                    // Posts for duckface
                    new Post { UserId = duckface.Id, TextContent = "Today was amazing!", FontSize = 20, TextColor = "#3498DB", BackgroundColor = "#FDFDFD", DateUploaded = DateTime.Now.AddHours(-10) },
                    new Post { UserId = duckface.Id, TextContent = "I'm feeling great!", FontSize = 18, TextColor = "#3498DB", BackgroundColor = "#FDFDFD", DateUploaded = DateTime.Now.AddHours(-6) },
                    new Post { UserId = duckface.Id, ImagePath = "/images/seed/user3img1.jpg", DateUploaded = DateTime.Now.AddHours(-8) },
                    new Post { UserId = duckface.Id, ImagePath = "/images/seed/user3img2.jpg", DateUploaded = DateTime.Now.AddHours(-8) },
                    new Post { UserId = duckface.Id, ImagePath = "/images/seed/user3img3.jpg", DateUploaded = DateTime.Now.AddHours(-8) },
                    new Post { UserId = duckface.Id, ImagePath = "/images/seed/user3img4.jpg", DateUploaded = DateTime.Now.AddHours(-8) },
                    // Posts for Mr Turtle
                    new Post { UserId = turtle.Id, ImagePath = "/images/seed/user4img1.jpg", DateUploaded = DateTime.Now.AddHours(-3) },
                    new Post { UserId = turtle.Id, ImagePath = "/images/seed/user4img2.jpg", DateUploaded = DateTime.Now.AddHours(-5) },
                    new Post { UserId = turtle.Id, TextContent = "Living a life as a turtle can be profoundly isolating and melancholic, a slow existence where each step feels like an eternity. Imagine carrying the weight of your home on your back, always a part of you, yet never a source of warmth or comfort. You move at a pace that allows the world to pass you by, and though you are surrounded by nature’s beauty, you can rarely interact with it in any meaningful way. Predators see you as nothing more than a hard meal to crack, and even the creatures you might call companions view you with indifference. Your life is a constant struggle between survival and insignificance, where the vastness of the ocean or the emptiness of the land only serves as a reminder of how small and vulnerable you truly are. And worst of all, your lifespan, though long, stretches out the loneliness, ensuring that every year feels like a decade, every moment a lifetime of sadness, as you crawl through a world that often seems like it has forgotten you exist.", FontSize = 18, TextColor = "#2ECC71", BackgroundColor = "#FFFFFF", DateUploaded = DateTime.Now.AddHours(-7) },

                    // Posts for mr. Red
                    new Post { UserId = red.Id, TextContent = "Running into the weekend!", FontSize = 16, TextColor = "#E74C3C", BackgroundColor = "#F8F8F8", DateUploaded = DateTime.Now.AddHours(-9) },
                    new Post { UserId = red.Id, TextContent = "i have a great buisniss idea, i just need some money, slide into my dms and ill hit you up with the greatest scheme ever bro", FontSize = 16, TextColor = "#E74C3C", BackgroundColor = "#F8F8F8", DateUploaded = DateTime.Now.AddHours(-9) }
                );
                context.SaveChanges();
            }

            // Seeding Comments
            if (!context.Comments.Any())
            {
                var posts = context.Posts.ToList();

                context.Comments.AddRange(
                    new Comment { PostId = posts[0].PostId, UserId = racoony.Id, Content = "Looks great, Barbie!" },
                    new Comment { PostId = posts[3].PostId, UserId = barbie.Id, Content = "Amazing photo!" },
                    new Comment { PostId = posts[4].PostId, UserId = duckface.Id, Content = "Looks like a great day!" },
                    new Comment { PostId = posts[5].PostId, UserId = duckface.Id, Content = "Grapes are the best!" },
                    new Comment { PostId = posts[6].PostId, UserId = duckface.Id, Content = "wow" },
                    new Comment { PostId = posts[7].PostId, UserId = duckface.Id, Content = "amazing" },
                    new Comment { PostId = posts[8].PostId, UserId = duckface.Id, Content = "I love the beach too!" },
                    new Comment { PostId = posts[5].PostId, UserId = duckface.Id, Content = "I love grapes" },
                    new Comment { PostId = posts[9].PostId, UserId = turtle.Id, Content = "I feel you, Mr. Turtle" },
                    new Comment { PostId = posts[10].PostId, UserId = red.Id, Content = "Haha, you're such a joker" }
                );
                context.SaveChanges();
            }

            // Seed Likes
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
                                UserId = user.Id,
                                PostId = post.PostId,
                                DateLiked = DateTime.Now.AddMinutes(-random.Next(1, 60))
                            });
                        }
                    }
                }
                context.SaveChanges();
            }
        }
    }
}