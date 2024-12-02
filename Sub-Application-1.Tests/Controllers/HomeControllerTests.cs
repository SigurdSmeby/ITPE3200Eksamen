using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sub_Application_1.Controllers;
using Sub_Application_1.DTOs;
using Sub_Application_1.Models;
using Sub_Application_1.Repositories.Interfaces;
using Xunit;
using Xunit.Abstractions;


namespace Sub_Application_1.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private readonly ITestOutputHelper _output;


        public HomeControllerTests(ITestOutputHelper output)
        {
            _postRepositoryMock = new Mock<IPostRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _output = output;
        }
        // Positive test case for CreatePost (Create in db)
        [Fact]
        public async Task CreatePost_ValidPost_SavesToDatabaseAndSavesFile()
        {
        _output.WriteLine("Testing Creating a post with valid data saves it to database");
        _output.WriteLine("-----------------------------------------");

            // Arrange

            var user = new User { Id = "testuser", UserName = "testuser" };
            _userRepositoryMock.Setup(repo => repo.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var posts = new List<Post>();
            _postRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Post>()))
                .Callback<Post>(post => posts.Add(post));
            _postRepositoryMock.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);

            var mockRootPath = Path.Combine(Path.GetTempPath(), "MockUploads");
            _webHostEnvironmentMock.Setup(env => env.WebRootPath).Returns(mockRootPath);

            var controller = new HomeController(_postRepositoryMock.Object, _userRepositoryMock.Object, _webHostEnvironmentMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id)
                        }))
                    }
                }
            };

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1024); // 1KB
            fileMock.Setup(f => f.FileName).Returns("testimage.jpg");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[1024]));

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
             _postRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Post>()), Times.Once);
            _postRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);

            Assert.Single(posts);
             _output.WriteLine("     ✅ Verified post was added to the database.");
            var post = posts.First();
            Assert.Equal(user.Id, post.UserId);
             _output.WriteLine("     ✅ Verified post has the correct UserId.");
            Assert.Equal("This is a test post.", post.TextContent);
             _output.WriteLine("     ✅ Verified post has the correct TextContent");
            Assert.Equal(14, post.FontSize);
                _output.WriteLine("     ✅ Verified post unchange FontSize.");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

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
            var user = new User { Id = "testuser", UserName = "testuser" };
            _userRepositoryMock.Setup(repo => repo.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var controller = new HomeController(_postRepositoryMock.Object, _userRepositoryMock.Object, _webHostEnvironmentMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id)
                    }))
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
            var user = new User { Id = "testuser", UserName = "testuser" };
            _userRepositoryMock.Setup(repo => repo.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            // Setup posts
            var posts = new List<Post>
            {
                new Post
                {
                    PostId = 1,
                    UserId = user.Id,
                    TextContent = "This is a test post.",
                    FontSize = 14,
                    TextColor = "#000000",
                    BackgroundColor = "#FFFFFF",
                    ImagePath = null, // Text post
                    DateUploaded = DateTime.Now,
                    User = user,
                    Likes = new List<Like>()
                }
            };

            _postRepositoryMock.Setup(repo => repo.GetAllPostsWithDetailsAsync())
                .ReturnsAsync(posts);

            var controller = new HomeController(_postRepositoryMock.Object, _userRepositoryMock.Object, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id),
                            new Claim(ClaimTypes.Name, user.UserName)
                        }))
                    }
                }
            };

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<PostDto>>(viewResult.Model);

            Assert.NotNull(model);
            _output.WriteLine("     ✅ Verified model is not null.");
            Assert.Single(model);
            _output.WriteLine("     ✅ Verified model contains a single post.");
            var retrievedPost = model.First();

            Assert.Equal(posts[0].PostId, retrievedPost.PostId);
            _output.WriteLine("     ✅ Verified PostId is correct.");

            Assert.Equal("This is a test post.", retrievedPost.TextContent);
            _output.WriteLine("     ✅ Verified TextContent is correct.");

            Assert.Equal(posts[0].DateUploaded, retrievedPost.DateUploaded);
            _output.WriteLine("     ✅ Verified DateUploaded is correct.");
            Assert.Equal(14, retrievedPost.FontSize);
            _output.WriteLine("     ✅ Verified FontSize is correct.");
            Assert.Equal("#000000", retrievedPost.TextColor);
            _output.WriteLine("     ✅ Verified TextColor is correct.");
            Assert.Equal("#FFFFFF", retrievedPost.BackgroundColor);
            _output.WriteLine("     ✅ Verified BackgroundColor is correct.");
            Assert.Null(retrievedPost.ImagePath);
            _output.WriteLine("     ✅ Verified it is a text post, by checking ImagePath is null.");
            _output.WriteLine("-----------------------------------------");

        }

        
        // Negative test case for readPost (No posts in db)
        [Fact]
        public async Task Index_NoPosts_ReturnsEmptyList()
        {
            _output.WriteLine("Testing Index with no posts returns an empty list");
            _output.WriteLine("-----------------------------------------");
            // Arrange
            var user = new User { Id = "testuser", UserName = "testuser" };
            _userRepositoryMock.Setup(repo => repo.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            // Setup empty posts
            var posts = new List<Post>();
            _postRepositoryMock.Setup(repo => repo.GetAllPostsWithDetailsAsync())
                .ReturnsAsync(posts);

            var controller = new HomeController(_postRepositoryMock.Object, _userRepositoryMock.Object, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id),
                            new Claim(ClaimTypes.Name, user.UserName)
                        }))
                    }
                }
            };

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            _output.WriteLine("     ✅ Verified result is a ViewResult.");
            var model = Assert.IsAssignableFrom<List<PostDto>>(viewResult.Model);
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
            var user = new User { Id = "testuser", UserName = "testuser" };
            _userRepositoryMock.Setup(repo => repo.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var post = new Post
            {
                PostId = 1,
                UserId = user.Id,
                TextContent = "Original content",
                FontSize = 14,
                TextColor = "#000000",
                BackgroundColor = "#FFFFFF",
                ImagePath = null // This is a text post
            };

            _postRepositoryMock.Setup(repo => repo.GetPostByIdAsync(post.PostId))
                .ReturnsAsync(post);
            _postRepositoryMock.Setup(repo => repo.Update(It.IsAny<Post>()));
            _postRepositoryMock.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);

            var controller = new HomeController(_postRepositoryMock.Object, _userRepositoryMock.Object, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id)
                        }))
                    }
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
            var result = await controller.EditPost(post.PostId, updatePostDto, null);

            // Assert
            _postRepositoryMock.Verify(repo => repo.Update(It.IsAny<Post>()), Times.Once);
            _postRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);

            Assert.Equal("Updated content", post.TextContent);
            _output.WriteLine("     ✅ Verified TextContent is updated.");
            Assert.Equal(16, post.FontSize);
            _output.WriteLine("     ✅ Verified FontSize is updated.");
            Assert.Equal("#FF5733", post.TextColor);
            _output.WriteLine("     ✅ Verified TextColor is updated.");
            Assert.Equal("#CCCCCC", post.BackgroundColor);
            _output.WriteLine("     ✅ Verified BackgroundColor is updated.");
            Assert.Null(post.ImagePath);
            _output.WriteLine("     ✅ Verified it is still a text post, by checking ImagePath is null.");

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
            var user = new User { Id = "testuser", UserName = "testuser" };
            _userRepositoryMock.Setup(repo => repo.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _postRepositoryMock.Setup(repo => repo.GetPostByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Post)null!); // Post not found, and supressing warning

            var controller = new HomeController(_postRepositoryMock.Object, _userRepositoryMock.Object, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id)
                        }))
                    }
                }
            };

            var updatePostDto = new UpdatePostDto
            {
                TextContent = "Updated content"
            };

            // Act
            var result = await controller.EditPost(999, updatePostDto, null);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            _output.WriteLine("     ✅ Verified result is NotFoundObjectResult.");
            Assert.Equal("Post with ID '999' not found.", notFoundResult.Value);
            _output.WriteLine("     ✅ Verified error message.");
            _output.WriteLine("-----------------------------------------");
        }

        // Positive test for DeletePost
        [Fact]
        public async Task DeletePost_ValidPost_DeletesPost()
        {
            _output.WriteLine("Testing DeletePost with valid post deletes post");
            _output.WriteLine("-----------------------------------------");
            // Arrange
            var user = new User { Id = "testuser", UserName = "testuser" };
            _userRepositoryMock.Setup(repo => repo.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var post = new Post
            {
                PostId = 1,
                UserId = user.Id,
                TextContent = "This post will be deleted",
                FontSize = 14,
                TextColor = "#000000",
                BackgroundColor = "#FFFFFF",
                ImagePath = null // Text post
            };

            _postRepositoryMock.Setup(repo => repo.GetPostByIdAsync(post.PostId))
                .ReturnsAsync(post);
            _postRepositoryMock.Setup(repo => repo.Delete(It.IsAny<Post>()));
            _postRepositoryMock.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);

            var controller = new HomeController(_postRepositoryMock.Object, _userRepositoryMock.Object, _webHostEnvironmentMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id)
                        }))
                    }
                }
            };

            // Act
            var result = await controller.DeletePost(post.PostId);

            // Assert
            _postRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Post>()), Times.Once);
            _postRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _output.WriteLine("     ✅ Verified post deletion and redirection.");
            _output.WriteLine("-----------------------------------------");
        }
        
        // Negative test for Delete post
        [Fact]
        public async Task DeletePost_UserNotOwner_ReturnsForbid()
        {
            _output.WriteLine("Testing DeletePost with user not owner returns Forbid");
            _output.WriteLine("-----------------------------------------");
            // Arrange
            var ownerUser = new User { Id = "owneruser", UserName = "owneruser" };
            var otherUser = new User { Id = "otheruser", UserName = "otheruser" };
            _userRepositoryMock.Setup(repo => repo.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(otherUser);

            var post = new Post
            {
                PostId = 1,
                UserId = ownerUser.Id,
                TextContent = "This post belongs to owneruser",
                FontSize = 14,
                TextColor = "#000000",
                BackgroundColor = "#FFFFFF",
                ImagePath = null
            };

            _postRepositoryMock.Setup(repo => repo.GetPostByIdAsync(post.PostId))
                .ReturnsAsync(post);

            var controller = new HomeController(_postRepositoryMock.Object, _userRepositoryMock.Object, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, otherUser.Id)
                        }))
                    }
                }
            };

            // Act
            var result = await controller.DeletePost(post.PostId);

            // Assert
            var forbidResult = Assert.IsType<ForbidResult>(result);
            _output.WriteLine("     ✅ Verified result is ForbidResult.");

            _postRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Post>()), Times.Never);
            _postRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Never);
            _output.WriteLine("     ✅ Verified post was not deleted.");
            _output.WriteLine("-----------------------------------------");
        }

        
    }
    
}
