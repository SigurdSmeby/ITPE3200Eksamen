using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sub_Application_1.Controllers;
using Sub_Application_1.Data;
using Sub_Application_1.DTOs;
using Sub_Application_1.Models;
using Sub_Application_1.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;
namespace Sub_Application_1.Tests.Controllers
{

    public class HomeControllerTests
    {
        private readonly ITestOutputHelper _output;

        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;
        private const string TEMP_FILE_DIR = "TestUploads"; // Centralized constant for test files

        public HomeControllerTests(ITestOutputHelper output)
        {
            // Initialize SQLite in-memory connection
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            // Configure DbContext to use SQLite in-memory
            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            // Create the schema in the database
            using var context = CreateDbContext();
            context.Database.EnsureCreated();

            // Ensure test upload directory exists
            Directory.CreateDirectory(TEMP_FILE_DIR);
            _output = output;
        }

        private AppDbContext CreateDbContext()
        {
            return new AppDbContext(_contextOptions);
        }

        // Positive test case for CreatePost (Create in db)
        [Fact]
        public async Task CreatePost_ValidPost_SavesToDatabaseAndSavesFile()
        {
        _output.WriteLine("Testing Creating a post with valid data saves it to database");
        _output.WriteLine("-----------------------------------------");
            // Arrange
            var userName = "testuser";
            var user = HelperMethods.CreateTestUser(userName);

            var userManagerMock = HelperMethods.CreateUserManagerMock();
            userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var webHostEnvironmentMock = HelperMethods.CreateWebHostEnvironmentMock();
            var mockRootPath = Path.Combine(Path.GetTempPath(), "MockUploads");
            webHostEnvironmentMock.Setup(env => env.WebRootPath).Returns(mockRootPath);

            ResetDatabase();

            using var context = CreateDbContext();
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var controller = new HomeController(userManagerMock.Object, context, webHostEnvironmentMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = HelperMethods.CreateAuthenticatedUser(user.Id, userName)
                }
            };

            // Create mock file
            var fileMock = HelperMethods.CreateMockFile("testimage.jpg", "Fake Image Content");

            var createPostDto = new CreatePostDto
            {
                TextContent = "This is a test post.",
                FontSize = 14,
                TextColor = "#FF5733",
                BackgroundColor = "#FFFFFF",
                Image = fileMock.Object
            };

            // Act
            var result = await controller.CreatePost(createPostDto);

            // Assert
            var post = await context.Posts.FirstOrDefaultAsync();
            Assert.NotNull(post);
            _output.WriteLine("     ✅ Verified post was saved in the database.");

            Assert.Equal(user.Id, post.UserId);
            _output.WriteLine("     ✅ Verified UserId is correct.");

            Assert.Equal("This is a test post.", post.TextContent);
            _output.WriteLine("     ✅ Verified TextContent is correct.");

            var relativeImagePath = $"/uploads/{Path.GetFileName(post.ImagePath)}";
            Assert.Equal(relativeImagePath, post.ImagePath);
            _output.WriteLine("     ✅ Verified ImagePath is correct.");

            webHostEnvironmentMock.Verify(env => env.WebRootPath, Times.Once);
            _output.WriteLine("     ✅ Verified WebRootPath was accessed once.");

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _output.WriteLine("     ✅ Verified redirect to Index action.");

            _output.WriteLine("     ✅ All assertions passed for CreatePost_ValidPost test.");
            _output.WriteLine("-----------------------------------------");
        }
        // Negative Test: CreatePost with invalid model state
        [Fact]
        public async Task CreatePost_InvalidModelState_ReturnsViewWithErrors()
        {
            _output.WriteLine("Testing CreatePost with invalid model state returns view with errors");
            _output.WriteLine("-----------------------------------------");
            // Arrange
            var user = HelperMethods.CreateTestUser("testuser");
            var userManagerMock = HelperMethods.CreateUserManagerMock();
            userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var webHostEnvironmentMock = HelperMethods.CreateWebHostEnvironmentMock();
            ResetDatabase();

            using var context = CreateDbContext();
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var controller = new HomeController(userManagerMock.Object, context, webHostEnvironmentMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = HelperMethods.CreateAuthenticatedUser(user.Id!, user.UserName!)
                }
            };

            controller.ModelState.AddModelError("TextContent", "Required");

            var createPostDto = new CreatePostDto
            {
                // Missing TextContent
                FontSize = 14,
                TextColor = "#FF5733",
                BackgroundColor = "#FFFFFF"
            };

            // Act
            var result = await controller.CreatePost(createPostDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            _output.WriteLine("     ✅ Verified result is a ViewResult.");

            Assert.NotNull(viewResult.ViewData);
            Assert.NotNull(viewResult.ViewData.ModelState);
            _output.WriteLine("     ✅ Verified ViewData and ModelState are not null.");
            
            // Validate ModelState
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            _output.WriteLine("     ✅ Verified ModelState is not valid.");

            // Check if "TextContent" key exists and its Errors are not empty
            var textContentErrors = viewResult.ViewData.ModelState["TextContent"]?.Errors;
            Assert.NotNull(textContentErrors);
            _output.WriteLine("     ✅ Verified TextContent error is not null.");
            Assert.NotEmpty(textContentErrors);
            _output.WriteLine("     ✅ Verified TextContent error is not empty.");
            _output.WriteLine("-----------------------------------------");        
        }

        //Positive test for readPost
        [Fact]
        public async Task Index_WithPosts_ReturnsPostsList()
        {
            _output.WriteLine("Testing Index with posts returns a list of posts");
            _output.WriteLine("-----------------------------------------");
            // Arrange
            var user = HelperMethods.CreateTestUser("testuser");
            var userManagerMock = HelperMethods.CreateUserManagerMock();
            userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            ResetDatabase();

            using var context = CreateDbContext();
            context.Users.Add(user);

            // Add a post to the database
            var post = new Post
            {
                UserId = user.Id,
                TextContent = "This is a test post.",
                FontSize = 14,
                TextColor = "#000000",
                BackgroundColor = "#FFFFFF",
                ImagePath = null, // Text post
                DateUploaded = DateTime.Now
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();

            var controller = new HomeController(userManagerMock.Object, context, null!);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = HelperMethods.CreateAuthenticatedUser(user.Id!, user.UserName!)
                }
            };

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.List<PostDto>>(viewResult.Model);

            Assert.NotNull(model); 
            _output.WriteLine("     ✅ Verified model is not null.");
            Assert.Single(model); 
            _output.WriteLine("     ✅ Verified model contains a single post.");
            var retrievedPost = model.First();

            Assert.Equal(post.PostId, retrievedPost.PostId);
            _output.WriteLine("     ✅ Verified PostId is correct.");

            Assert.Equal("This is a test post.", retrievedPost.TextContent);
            _output.WriteLine("     ✅ Verified TextContent is correct.");
            
            Assert.Equal(post?.DateUploaded, retrievedPost?.DateUploaded);
            _output.WriteLine("     ✅ Verified DateUploaded is correct.");
            Assert.Equal(14, retrievedPost?.FontSize);
            _output.WriteLine("     ✅ Verified FontSize is correct.");
            Assert.Equal("#000000", retrievedPost?.TextColor);
            _output.WriteLine("     ✅ Verified TextColor is correct.");
            Assert.Equal("#FFFFFF", retrievedPost?.BackgroundColor);
            _output.WriteLine("     ✅ Verified BackgroundColor is correct.");
            Assert.Null(retrievedPost?.ImagePath); 
            _output.WriteLine("     ✅ Verified it is a text post, by checking ImagePath if is null.");
            _output.WriteLine("-----------------------------------------");

        }

        
        // Negative test case for readPost (No posts in db)
        [Fact]
        public async Task Index_NoPosts_ReturnsEmptyList()
        {
            _output.WriteLine("Testing Index with no posts returns an empty list");
            _output.WriteLine("-----------------------------------------");
            // Arrange
            var userName = "testuser";
            var user = HelperMethods.CreateTestUser(userName);

            // Create and configure UserManager mock
            var userManagerMock = HelperMethods.CreateUserManagerMock();
            userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            // Create and configure WebHostEnvironment mock
            var webHostEnvironmentMock = HelperMethods.CreateWebHostEnvironmentMock();
            var mockRootPath = Path.Combine(Path.GetTempPath(), "MockUploads");
            webHostEnvironmentMock.Setup(env => env.WebRootPath).Returns(mockRootPath);

            ResetDatabase();

            // Initialize database context
            using var context = new AppDbContext(_contextOptions);

            // Initialize HomeController with mocks
            var controller = new HomeController(userManagerMock.Object, context, webHostEnvironmentMock.Object);

            // Set up a mocked authenticated user
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = HelperMethods.CreateAuthenticatedUser(user.Id, userName)
                }
            };

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            _output.WriteLine("     ✅ Verified result is a ViewResult.");
            var model = Assert.IsAssignableFrom<System.Collections.Generic.List<PostDto>>(viewResult.Model);
            _output.WriteLine("     ✅ Verified model is a list of PostDto.");
            Assert.Empty(model);
            _output.WriteLine("     ✅ Verified model is empty.");
            _output.WriteLine("-----------------------------------------");

        }

        // Positive test for Update post
        [Fact]
        public async Task EditTextPost_WithValidData_UpdatesOtherFields()
        {
            _output.WriteLine("Testing EditPost with valid data updates other fields");
            _output.WriteLine("-----------------------------------------");
            // Arrange
            var user = HelperMethods.CreateTestUser("testuser");
            var userManagerMock = HelperMethods.CreateUserManagerMock();
            userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            ResetDatabase();

            using var context = CreateDbContext();
            context.Users.Add(user);

            var post = new Post
            {
                UserId = user.Id,
                TextContent = "Original content",
                FontSize = 14,
                TextColor = "#000000",
                BackgroundColor = "#FFFFFF",
                ImagePath = null! // This is a text post
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();

            var controller = new HomeController(userManagerMock.Object, context, null!);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = HelperMethods.CreateAuthenticatedUser(user.Id!, user.UserName!)
                }
            };

            var updatePostDto = new UpdatePostDto
            {
                TextContent = "Updated content",
                FontSize = 16,
                TextColor = "#FF5733",
                BackgroundColor = "#CCCCCC"
            };

            // Act
            var result = await controller.EditPost(post.PostId, updatePostDto, null!);

            // Assert
            // get the post from the db
            var updatedPost = await context.Posts.FirstOrDefaultAsync(p => p.PostId == post.PostId);
            
            Assert.NotNull(updatedPost);
            _output.WriteLine("     ✅ Verified post was updated in the database.");
            Assert.Equal("Updated content", updatedPost.TextContent);
            _output.WriteLine("     ✅ Verified TextContent is updated.");
            Assert.Equal(16, updatedPost.FontSize);
            _output.WriteLine("     ✅ Verified FontSize is updated.");
            Assert.Equal("#FF5733", updatedPost.TextColor);
            _output.WriteLine("     ✅ Verified TextColor is updated.");
            Assert.Equal("#CCCCCC", updatedPost.BackgroundColor);
            _output.WriteLine("     ✅ Verified BackgroundColor is updated.");
            Assert.Null(updatedPost.ImagePath); 
            _output.WriteLine("     ✅ Verified it is still a text post, by checking ImagePath if is null.");

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _output.WriteLine("     ✅ Verified redirect to Index action.");
            _output.WriteLine("-----------------------------------------");


        }

        // Negative test for Update post
        [Fact]
        public async Task EditPost_PostNotFound_ReturnsNotFound()
        {
            _output.WriteLine("Testing EditPost with non-existent post returns NotFound");
            _output.WriteLine("-----------------------------------------");
            // Arrange
            var user = HelperMethods.CreateTestUser("testuser");
            var userManagerMock = HelperMethods.CreateUserManagerMock();
            userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            ResetDatabase();

            using var context = CreateDbContext();
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var controller = new HomeController(userManagerMock.Object, context, null!);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = HelperMethods.CreateAuthenticatedUser(user.Id!, user.UserName!)
                }
            };

            var updatePostDto = new UpdatePostDto
            {
                TextContent = "Updated content"
            };

            // Act
            var result = await controller.EditPost(999, updatePostDto, null!); // Non-existent postId

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            _output.WriteLine("     ✅ Verified result is NotFoundObjectResult.");
            Assert.Equal("Post with ID '999' not found.", notFoundResult.Value);
            _output.WriteLine("     ✅ Verified error message.");
            _output.WriteLine("-----------------------------------------");

        }
        // Positive test for Delete post
        [Fact]
        public async Task DeletePost_ValidPost_DeletesPost()
        {
            // Arrange
            var user = HelperMethods.CreateTestUser("testuser");
            var userManagerMock = HelperMethods.CreateUserManagerMock();
            userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            ResetDatabase();

            using var context = CreateDbContext();
            context.Users.Add(user);

            var post = new Post
            {
                UserId = user.Id,
                TextContent = "This post will be deleted",
                FontSize = 14,
                TextColor = "#000000",
                BackgroundColor = "#FFFFFF",
                ImagePath = null! // Text post
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();

            var controller = new HomeController(userManagerMock.Object, context, null!);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = HelperMethods.CreateAuthenticatedUser(user.Id!, user.UserName!)
                }
            };

            // Act
            var result = await controller.DeletePost(post.PostId);

            // Assert
            var deletedPost = await context.Posts.FindAsync(post.PostId);
            Assert.Null(deletedPost); // Post should be deleted

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName); // Redirects to Index
        }


        // Negative test for Delete post
        [Fact]
        public async Task DeletePost_UserNotOwner_ReturnsForbid()
        {
            _output.WriteLine("Testing DeletePost with user not owner returns Forbid");
            _output.WriteLine("-----------------------------------------");
            // Arrange
            var ownerUser = HelperMethods.CreateTestUser("owneruser");
            var otherUser = HelperMethods.CreateTestUser("otheruser");
            var userManagerMock = HelperMethods.CreateUserManagerMock();
            userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(otherUser);

            ResetDatabase();

            using var context = CreateDbContext();
            context.Users.Add(ownerUser);
            context.Users.Add(otherUser);

            var post = new Post
            {
                UserId = ownerUser.Id, // Post belongs to ownerUser
                TextContent = "This post belongs to owneruser",
                FontSize = 14,
                TextColor = "#000000",
                BackgroundColor = "#FFFFFF",
                ImagePath = null!
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();

            var controller = new HomeController(userManagerMock.Object, context, null!);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = HelperMethods.CreateAuthenticatedUser(otherUser.Id!, otherUser.UserName!) // Authenticated as otherUser
                }
            };

            // Act
            var result = await controller.DeletePost(post.PostId);

            // Assert
            var forbidResult = Assert.IsType<ForbidResult>(result);
            _output.WriteLine("     ✅ Verified result is ForbidResult.");
            var existingPost = await context.Posts.FindAsync(post.PostId);
            _output.WriteLine("     ✅ Verified post still exists in the database.");
            Assert.NotNull(existingPost);
            _output.WriteLine("     ✅ Verified post still exists in the database.");
            Assert.Equal(post.TextContent, existingPost.TextContent); 
            _output.WriteLine("     ✅ Verified post content is unchanged.");
            _output.WriteLine("-----------------------------------------");
        }






        private void ResetDatabase()
        {
            using var context = CreateDbContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        
    }
    
}
