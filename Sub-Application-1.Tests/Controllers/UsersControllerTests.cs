using Xunit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit.Abstractions;
using System.Linq;
using Sub_Application_1.Controllers;
using Sub_Application_1.DTOs;
using Sub_Application_1.Models;
using Sub_Application_1.Repositories.Interfaces;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

//we need this because signinresult is a part of both Mvc and Identity
using IdentitySignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Sub_Application_1.Tests.Controllers
{
	public class UsersControllerTests
	{
		private readonly ITestOutputHelper _output;
		private readonly Mock<ILogger<UsersController>> _loggerMock;

		public UsersControllerTests(ITestOutputHelper output)
		{
			_loggerMock = new Mock<ILogger<UsersController>>();
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

			var userRepositoryMock = new Mock<IUserRepository>();
			var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();

			userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>()))
				.ReturnsAsync((User)null!);
			userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync((User)null!);
			userRepositoryMock.Setup(repo => repo.RegisterUserAsync(It.IsAny<RegisterDto>()))
				.ReturnsAsync(IdentityResult.Success);

			var controller = new UsersController(userRepositoryMock.Object, webHostEnvironmentMock.Object, _loggerMock.Object);

			// Act
			var result = await controller.Register(registerDto);

			// Assert
			var redirectResult = Assert.IsType<RedirectToActionResult>(result);
			_output.WriteLine("     ✅ Asserted result is of type RedirectToActionResult.");

			Assert.Equal("Index", redirectResult.ActionName);
			_output.WriteLine("     ✅ Redirected to action 'Index'.");

			Assert.Equal("Home", redirectResult.ControllerName);
			_output.WriteLine("     ✅ Redirected to controller 'Home'.");

			userRepositoryMock.Verify(repo => repo.GetUserByUsernameAsync(registerDto.Username), Times.Once);
			_output.WriteLine("     ✅ Verified that GetUserByUsernameAsync was called once, to check if username already exists.");

			userRepositoryMock.Verify(repo => repo.GetUserByEmailAsync(registerDto.Email), Times.Once);
			_output.WriteLine("     ✅ Verified that GetUserByEmailAsync was called once, to check if email is already registered.");

			userRepositoryMock.Verify(repo => repo.RegisterUserAsync(registerDto), Times.Once);
			_output.WriteLine("     ✅ Verified that RegisterUserAsync was called once.");

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

			var userRepositoryMock = new Mock<IUserRepository>();
			var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();

			var controller = new UsersController(userRepositoryMock.Object, webHostEnvironmentMock.Object, _loggerMock.Object);

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

			userRepositoryMock.Verify(repo => repo.RegisterUserAsync(It.IsAny<RegisterDto>()), Times.Never);
			_output.WriteLine("     ✅ Verified that RegisterUserAsync was not called.");

			_output.WriteLine("     ✅ All assertions passed for model state errors test.");
			_output.WriteLine("-----------------------------------------");
		}

		[Fact]
		public async Task Register_UserRepositoryErrors_ReturnsViewWithErrors()
		{
			_output.WriteLine("Testing register with UserRepository errors:");
			_output.WriteLine("-----------------------------------------");

			// Arrange
			var registerDto = new RegisterDto
			{
				Username = "testuser",
				Email = "testuser@example.com",
				Password = "Password123!",
				confirmPassword = "Password123!"
			};

			var userRepositoryMock = new Mock<IUserRepository>();
			var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();

			userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>()))
				.ReturnsAsync((User)null!);
			userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync((User)null!);
			userRepositoryMock.Setup(repo => repo.RegisterUserAsync(It.IsAny<RegisterDto>()))
				.ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password is too weak." }));

			var controller = new UsersController(userRepositoryMock.Object, webHostEnvironmentMock.Object, _loggerMock.Object);

			// Act
			var result = await controller.Register(registerDto);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			_output.WriteLine("     ✅ Asserted result is of type ViewResult.");

			var model = Assert.IsType<RegisterDto>(viewResult.Model);
			_output.WriteLine($"     ✅ Asserted ViewResult contains model of type {model.GetType().Name}.");

			Assert.False(controller.ModelState.IsValid);
			_output.WriteLine("     ✅ Verified that ModelState is invalid.");

			Assert.Contains("Password is too weak.", controller.ModelState[string.Empty]?.Errors?.Select(e => e.ErrorMessage) ?? Enumerable.Empty<string>());

			_output.WriteLine("     ✅ Verified ModelState contains 'Password is too weak.' error.");

			userRepositoryMock.Verify(repo => repo.RegisterUserAsync(registerDto), Times.Once);
			_output.WriteLine("     ✅ Verified that RegisterUserAsync was called once.");

			_output.WriteLine("     ✅ All assertions passed for UserRepository errors test.");
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

			var userRepositoryMock = new Mock<IUserRepository>();
			var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();

			userRepositoryMock.Setup(repo => repo.LoginAsync(It.IsAny<LoginDto>()))
				.ReturnsAsync(IdentitySignInResult.Success);

			var controller = new UsersController(userRepositoryMock.Object, webHostEnvironmentMock.Object, _loggerMock.Object);

			// Act
			var result = await controller.Login(loginDto);

			// Assert
			var redirectResult = Assert.IsType<RedirectToActionResult>(result);
			_output.WriteLine("     ✅ Asserted result is of type RedirectToActionResult.");

			Assert.Equal("Index", redirectResult.ActionName);
			_output.WriteLine("     ✅ Redirected to action 'Index'.");

			Assert.Equal("Home", redirectResult.ControllerName);
			_output.WriteLine("     ✅ Redirected to controller 'Home'.");

			userRepositoryMock.Verify(repo => repo.LoginAsync(loginDto), Times.Once);
			_output.WriteLine("     ✅ Verified that LoginAsync was called once.");

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

			var userRepositoryMock = new Mock<IUserRepository>();
			var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();

			userRepositoryMock.Setup(repo => repo.LoginAsync(It.IsAny<LoginDto>()))
				.ReturnsAsync(IdentitySignInResult.Failed);

			var controller = new UsersController(userRepositoryMock.Object, webHostEnvironmentMock.Object, _loggerMock.Object);

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

			userRepositoryMock.Verify(repo => repo.LoginAsync(loginDto), Times.Once);
			_output.WriteLine("     ✅ Verified that LoginAsync was called once.");

			_output.WriteLine("     ✅ All assertions passed for wrong password test.");
			_output.WriteLine("-----------------------------------------");
		}

		// with more time we would have added more tests here        

	}
}
