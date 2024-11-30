using Xunit;
using Microsoft.AspNetCore.Identity;
using Sub_Application_1.Controllers;
using Sub_Application_1.DTOs;
using Sub_Application_1.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit.Abstractions;
using System.Linq;
using Sub_Application_1.Tests.Helpers;
// Alias to avoid conflict with SignInResult from Microsoft.AspNetCore.Mvc
using IdentitySignInResult = Microsoft.AspNetCore.Identity.SignInResult; 

namespace Sub_Application_1.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly ITestOutputHelper _output;

        public UsersControllerTests(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public async Task Register_ValidData_ReturnsRedirectToAction()
        {
            _output.WriteLine("Testing register with valid data:");
            _output.WriteLine("-----------------------------------------");

            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "Password123!",
                confirmPassword = "Password123!"
            };

            var userManagerMock = HelperMethods.CreateUserManagerMock();
            var signInManagerMock = HelperMethods.CreateSignInManagerMock(userManagerMock);

            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null!);
            userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null!);
            signInManagerMock.Setup(sm => sm.SignInAsync(It.IsAny<User>(), It.IsAny<bool>(), null!))
                .Returns(Task.CompletedTask);

            var controller = new UsersController(
                null!, null!, null!, 
                signInManagerMock.Object, 
                userManagerMock.Object
            );

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            _output.WriteLine("     ✅ Asserted result is of type RedirectToActionResult.");

            Assert.Equal("Index", redirectResult.ActionName);
            _output.WriteLine("     ✅ Redirected to action 'Index'.");

            Assert.Equal("Home", redirectResult.ControllerName);
            _output.WriteLine("     ✅ Redirected to controller 'Home'.");

            userManagerMock.Verify(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            _output.WriteLine("     ✅ Verified that UserManager.CreateAsync was called once.");

            _output.WriteLine("     ✅ All assertions passed for valid data test.");
            _output.WriteLine("-----------------------------------------");
        }

        [Fact]
        public async Task Register_ModelStateErrors_ReturnsViewWithErrors()
        {
            _output.WriteLine("\nTesting register with model state errors:");
            _output.WriteLine("-----------------------------------------");

            // Arrange 
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Email = "invalidemail", // Invalid email format
                Password = "Password123!",
                confirmPassword = "DifferentPassword123!" // Passwords do not match
            };

            var userManagerMock = HelperMethods.CreateUserManagerMock();
            var signInManagerMock = HelperMethods.CreateSignInManagerMock(userManagerMock);

            var controller = new UsersController(
                null!, null!, null!, 
                signInManagerMock.Object, 
                userManagerMock.Object
            );

            controller.ModelState.AddModelError("Email", "Invalid email format.");
            controller.ModelState.AddModelError("Password", "Passwords do not match.");

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            _output.WriteLine("     ✅ Asserted result is of type ViewResult.");

            var model = Assert.IsType<RegisterDto>(viewResult.Model);
            _output.WriteLine($"     ✅ Asserted ViewResult contains model of type {model.GetType().Name}.");

            Assert.False(controller.ModelState.IsValid);
            _output.WriteLine("     ✅ Verified that ModelState is invalid.");

            Assert.True(controller.ModelState.ContainsKey("Email"));
            _output.WriteLine("     ✅ Verified that 'Email' error exists in ModelState.");

            Assert.True(controller.ModelState.ContainsKey("Password"));
            _output.WriteLine("     ✅ Verified that 'Password' error exists in ModelState.");

            userManagerMock.Verify(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _output.WriteLine("     ✅ Verified that UserManager.CreateAsync was not called.");

            _output.WriteLine("     ✅ All assertions passed for model state errors test.");
            _output.WriteLine("-----------------------------------------");
        }

        [Fact]
        public async Task Register_UserManagerErrors_ReturnsViewWithErrors()
        {
            _output.WriteLine("Testing register with UserManager errors:");
            _output.WriteLine("-----------------------------------------");

            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "Password123!",
                confirmPassword = "Password123!"
            };

            var userManagerMock = HelperMethods.CreateUserManagerMock();
            var signInManagerMock = HelperMethods.CreateSignInManagerMock(userManagerMock);

            userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User)null!);
            userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null!);
            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password is too weak." }));

            var controller = new UsersController(
                null!, null!, null!, 
                signInManagerMock.Object, 
                userManagerMock.Object
            );

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            _output.WriteLine("     ✅ Asserted result is of type ViewResult.");

            var model = Assert.IsType<RegisterDto>(viewResult.Model);
            _output.WriteLine($"     ✅ Asserted ViewResult contains model of type {model.GetType().Name}.");

            Assert.False(controller.ModelState.IsValid);
            _output.WriteLine("     ✅ Verified that ModelState is invalid.");

            Assert.Contains("Password is too weak.", controller.ModelState[string.Empty]?.Errors?.Select(e => e.ErrorMessage) ?? Enumerable.Empty<string>()
);

            _output.WriteLine("     ✅ Verified ModelState contains 'Password is too weak.' error.");

            userManagerMock.Verify(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            _output.WriteLine("     ✅ Verified that UserManager.CreateAsync was called once.");

            _output.WriteLine("     ✅ All assertions passed for UserManager errors test.");
            _output.WriteLine("-----------------------------------------");
        }
        [Fact]
        public async Task Login_ValidCredentials_ReturnsRedirectToAction()
        {
            _output.WriteLine("Testing login with valid credentials:");
            _output.WriteLine("-----------------------------------------");

            // Arrange
            var loginDto = new LoginDto
            {
                Username = "testuser",
                Password = "ValidPassword123!"
            };

            var userManagerMock = HelperMethods.CreateUserManagerMock();
            var signInManagerMock = HelperMethods.CreateSignInManagerMock(userManagerMock);

            signInManagerMock.Setup(sm => sm.PasswordSignInAsync(
                loginDto.Username, 
                loginDto.Password, 
                It.IsAny<bool>(), 
                It.IsAny<bool>())
            ).ReturnsAsync(IdentitySignInResult.Success);

            var controller = new UsersController(
                null!, null!, null!, 
                signInManagerMock.Object, 
                userManagerMock.Object
            );

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            _output.WriteLine("     ✅ Asserted result is of type RedirectToActionResult.");

            Assert.Equal("Index", redirectResult.ActionName);
            _output.WriteLine("     ✅ Redirected to action 'Index'.");

            Assert.Equal("Home", redirectResult.ControllerName);
            _output.WriteLine("     ✅ Redirected to controller 'Home'.");

            signInManagerMock.Verify(sm => sm.PasswordSignInAsync(
                loginDto.Username, 
                loginDto.Password, 
                It.IsAny<bool>(), 
                It.IsAny<bool>()), 
                Times.Once);
            _output.WriteLine("     ✅ Verified that SignInManager.PasswordSignInAsync was called once.");
        
            _output.WriteLine("     ✅ All assertions passed for valid credentials test.");
            _output.WriteLine("-----------------------------------------");
        }
        [Fact]
        public async Task Login_WrongPassword_ReturnsViewWithError()
        {
            _output.WriteLine("Testing login with wrong password:");
            _output.WriteLine("-----------------------------------------");

            // Arrange
            var loginDto = new LoginDto
            {
                Username = "testuser",
                Password = "WrongPassword123!"
            };

            var userManagerMock = HelperMethods.CreateUserManagerMock();
            var signInManagerMock = HelperMethods.CreateSignInManagerMock(userManagerMock);

            signInManagerMock.Setup(sm => sm.PasswordSignInAsync(
                loginDto.Username, 
                loginDto.Password, 
                It.IsAny<bool>(), 
                It.IsAny<bool>())
            ).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var controller = new UsersController(
                null!, null!, null!, 
                signInManagerMock.Object, 
                userManagerMock.Object
            );

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            _output.WriteLine("     ✅ Asserted result is of type ViewResult.");

            Assert.False(controller.ModelState.IsValid);
            _output.WriteLine("     ✅ Verified that ModelState is invalid.");

            Assert.True(controller.ModelState.ContainsKey("Login"));
            _output.WriteLine("     ✅ Verified that 'Login' error exists in ModelState.");

            var error = controller.ModelState["Login"]?.Errors.FirstOrDefault()?.ErrorMessage;
            Assert.Equal("Invalid username or password.", error);
            _output.WriteLine("     ✅ Verified ModelState contains 'Invalid username or password.' error.");

            signInManagerMock.Verify(sm => sm.PasswordSignInAsync(
                loginDto.Username, 
                loginDto.Password, 
                It.IsAny<bool>(), 
                It.IsAny<bool>()), 
                Times.Once);
            _output.WriteLine("     ✅ Verified that SignInManager.PasswordSignInAsync was called once.");

            _output.WriteLine("     ✅ All assertions passed for wrong password test.");
            _output.WriteLine("-----------------------------------------");
        }        

    }
}
