using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using server.Models;

namespace server.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Apply pending migrations if any
            context.Database.Migrate();

            // Look for any users. If there are already users, return.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            // Helper method to generate random images
            List<Image> GenerateRandomImages(int count)
            {
                var images = new List<Image>();
                var random = new Random();
                for (int i = 0; i < count; i++)
                {
                    images.Add(new Image
                    {
                        ImageUrl = $"https://picsum.photos/300/{random.Next(100, 500)}",
                        UploadedAt = DateTime.Now.AddDays(-random.Next(1, 30))
                    });
                }
                return images;
            }

            // Original users from the client-side example
            var users = new List<User>
            {
                new User
                {
                    ProfileName = "Baifan",
                    Email = "baifan@example.com",
                    Password = "password1",
                    ProfilePicture = "https://picsum.photos/102",
                    Images = GenerateRandomImages(new Random().Next(5, 15))
                },
                new User
                {
                    ProfileName = "Raifan",
                    Email = "raifan@example.com",
                    Password = "password2",
                    ProfilePicture = "https://picsum.photos/50",
                    Images = GenerateRandomImages(new Random().Next(5, 15))
                },
                new User
                {
                    ProfileName = "Saifan",
                    Email = "saifan@example.com",
                    Password = "password3",
                    ProfilePicture = "https://picsum.photos/51",
                    Images = GenerateRandomImages(new Random().Next(5, 15))
                },
                new User
                {
                    ProfileName = "Taifan",
                    Email = "taifan@example.com",
                    Password = "password4",
                    ProfilePicture = "https://picsum.photos/52",
                    Images = GenerateRandomImages(new Random().Next(5, 15))
                },
                new User
                {
                    ProfileName = "Taifan 2 electricbogaloo",
                    Email = "taifan2@example.com",
                    Password = "password5",
                    ProfilePicture = "", // No profile image
                    Images = GenerateRandomImages(new Random().Next(5, 15))
                }
            };

            // New users with random names and more images
            var newUsers = new List<User>
            {
                new User
                {
                    ProfileName = "Mofan",
                    Email = "mofan@example.com",
                    Password = "password6",
                    ProfilePicture = "https://picsum.photos/53",
                    Images = GenerateRandomImages(new Random().Next(5, 15))
                },
                new User
                {
                    ProfileName = "Zifan",
                    Email = "zifan@example.com",
                    Password = "password7",
                    ProfilePicture = "https://picsum.photos/54",
                    Images = GenerateRandomImages(new Random().Next(5, 15))
                },
                new User
                {
                    ProfileName = "Gofan",
                    Email = "gofan@example.com",
                    Password = "password8",
                    ProfilePicture = "https://picsum.photos/55",
                    Images = GenerateRandomImages(new Random().Next(5, 15))
                },
                new User
                {
                    ProfileName = "Nafan",
                    Email = "nafan@example.com",
                    Password = "password9",
                    ProfilePicture = "https://picsum.photos/56",
                    Images = GenerateRandomImages(new Random().Next(5, 15))
                },
                new User
                {
                    ProfileName = "Kifan",
                    Email = "kifan@example.com",
                    Password = "password10",
                    ProfilePicture = "https://picsum.photos/57",
                    Images = GenerateRandomImages(new Random().Next(5, 15))
                }
            };

            // Add new users to the existing list
            users.AddRange(newUsers);

            // Save the data to the database
            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
