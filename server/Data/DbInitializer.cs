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
        context.Database.Migrate(); // Ensure migrations are applied

        // Look for any users. If there are already users, return.
        if (context.Users.Any())
        {
            return;   // DB has been seeded
        }

        var users = new List<User>
            {
                new User
                {
                    ProfileName = "JohnDoe",
                    Email = "johndoe@example.com",
                    Password = "password123",
                    ProfilePicture = "https://picsum.photos/51",
                    Images = new List<Image>
                    {
                        new Image
                        {
                            ImageUrl = "https://picsum.photos/511",
                            UploadedAt = DateTime.Now.AddDays(-2)
                        },
                        new Image
                        {
                            ImageUrl = "https://picsum.photos/522",
                            UploadedAt = DateTime.Now.AddDays(-7)
                        }
                    }
                },
                new User
                {
                    ProfileName = "JaneSmith",
                    Email = "janesmith@example.com",
                    Password = "password456",
                    ProfilePicture = "https://picsum.photos/90",
                    Images = new List<Image>
                    {
                        new Image
                        {
                            ImageUrl = "https://picsum.photos/900",
                            UploadedAt = DateTime.Now.AddDays(-3)
                        }
                    }
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
