using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Sub_Application_1.Controllers;
using Sub_Application_1.DTOs;
using Sub_Application_1.Models;
using Microsoft.AspNetCore.Mvc;


namespace Sub_Application_1.Controllers;

public class UsersControllerTests{
    private Mock<UserManager<User>> _userManagerMock;
    private Mock<SignInManager<User>> _signInManagerMock;
    private UsersController _controller;

//Constructor
public UsersControllerTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        _signInManagerMock = new Mock<SignInManager<User>>(
            _userManagerMock.Object, 
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<User>>(),
            null, null, null, null
        );
    
        _controller = new UsersController(null, null, null, _signInManagerMock.Object, _userManagerMock.Object);
    }

    // Test for Register method, testing if Valid data returns a redirect to action (Positive test)
    [Fact]   
    public async Task Register_ValidDataReturnsRedirectToAction()
    {
        // making a fake user
        var registerDto = new RegisterDto
        {
            Username = "testuser",
            Email = "testuser@example.com",
            Password = "Password123!",
            confirmPassword = "Password123!"
        };

         _userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<User>(), registerDto.Password))
            .ReturnsAsync(IdentityResult.Success);

        _signInManagerMock
            .Setup(sm => sm.SignInAsync(It.IsAny<User>(), It.IsAny<bool>(), null))
            .Returns(Task.CompletedTask);

        // running the test and saving the result
        var result = await _controller.Register(registerDto);
        
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }
}