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
using Sub_Application_1.Tests.Helpers;
using Xunit;

namespace Sub_Application_1.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IWebHostEnvironment> _webHostEnvMock;
        private readonly AppDbContext _dbContext;

        public HomeControllerTests()
        {
            _userManagerMock = HelperMethods.CreateUserManagerMock();
            _webHostEnvMock = new Mock<IWebHostEnvironment>();
            _webHostEnvMock.Setup(env => env.WebRootPath).Returns("wwwroot");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("DataSource=:memory:") // SQLite in-memory database
                .Options;

            _dbContext = new AppDbContext(options);
            _dbContext.Database.OpenConnection(); // Open connection to the in-memory SQLite database
            _dbContext.Database.EnsureCreated();  // Ensure schema is created

        }

        [Fact]
        public async Task Index_ReturnsPostsForView()
        {
            // Arrange
            var user = new User { Id = "test-user-id", UserName = "testuser" };
            var post = new Post
            {
                PostId = 1,
                UserId = user.Id,
                TextContent = "Sample Post",
                DateUploaded = DateTime.Now,
                User = user
            };

            _dbContext.Users.Add(user);
            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();

            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync(user);

            var controller = new HomeController(_userManagerMock.Object, _dbContext, _webHostEnvMock.Object);

            // Set up the User property in the controller
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<PostDto>>(viewResult.Model);
            Assert.Single(model);

            // Additional assertions (optional)
            var postDto = model.First();
            Assert.Equal(post.PostId, postDto.PostId);
            Assert.Equal(post.TextContent, postDto.TextContent);
            Assert.Equal(user.UserName, postDto.Author.Username);
            Assert.Equal(user.Id, postDto.Author.UserId);
        }
    }
}
