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
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);
        }

        // method to create a mocked SignInManager
        public static Mock<SignInManager<User>> CreateSignInManagerMock(Mock<UserManager<User>> userManagerMock)
        {
            return new Mock<SignInManager<User>>(
                userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                null,
                null,
                null,
                null);
        }
    }
}
