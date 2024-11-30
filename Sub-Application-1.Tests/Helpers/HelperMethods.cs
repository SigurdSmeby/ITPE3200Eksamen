using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sub_Application_1.Controllers;
using Sub_Application_1.Data;
using Sub_Application_1.DTOs;
using Sub_Application_1.Models;
using Xunit;
using System.IO;



namespace Sub_Application_1.Tests.Helpers
{
    public static class HelperMethods
    {
        // method to create a mocked UserManager
        public static Mock<UserManager<User>> CreateUserManagerMock()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
                userStoreMock.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);
        }

        // method to create a mocked SignInManager
        public static Mock<SignInManager<User>> CreateSignInManagerMock(Mock<UserManager<User>> userManagerMock)
        {
            return new Mock<SignInManager<User>>(
                userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                null!,
                null!,
                null!,
                null!);
        }

        // Method to create a mocked Database using SQLite in-memory
        // instead of creating just a in-memory database, sqlite in-memory will be closer to the real thing
        public static DbContextOptions<AppDbContext> CreateInMemoryDatabaseOptions()
        {
            var connection = new Microsoft.Data.Sqlite.SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            return options;
        }

        // Method to create an authenticated user
        public static ClaimsPrincipal CreateAuthenticatedUser(string userId, string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            return new ClaimsPrincipal(identity);
        }

        public static User CreateTestUser (string userName)
        {
            return new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = userName,
                Email = userName + "@testuser.com",
            };
        }

        public static Mock<IFormFile> CreateMockFile(string fileName, string content)
        {
            var fileMock = new Mock<IFormFile>();
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream target, CancellationToken token) => ms.CopyToAsync(target, token));

            return fileMock;
        }
        public static Mock<IWebHostEnvironment> CreateWebHostEnvironmentMock()
        {
            var mockEnv = new Mock<IWebHostEnvironment>();
            var tempPath = Path.Combine(Path.GetTempPath(), "MockUploads"); // Simulate mock path
            Directory.CreateDirectory(tempPath); // Ensure it exists for the test

            mockEnv.Setup(m => m.WebRootPath).Returns(tempPath);
            return mockEnv;
        }

    }
}
