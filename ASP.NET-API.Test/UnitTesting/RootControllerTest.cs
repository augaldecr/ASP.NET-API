using ASP.NET_API.Controllers.V1;
using ASP.NET_API.Test.Mocks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.Test.UnitTesting
{
    [TestClass]
    public class RootControllerTest
    {
        [TestMethod]
        public async Task IfUserIsAdmin_Get4Links()
        {
            //Preparation
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Result = AuthorizationResult.Success();
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHerlperMock();

            //Execution
            var result = await rootController.Get();

            //Verification
            Assert.AreEqual(4, result.Value.Count());
        }

        [TestMethod]
        public async Task IfUserIsNoAdmin_Get2Links()
        {
            //Preparation
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Result = AuthorizationResult.Failed();
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHerlperMock();

            //Execution
            var result = await rootController.Get();

            //Verification
            Assert.AreEqual(2, result.Value.Count());
        }


        [TestMethod]
        public async Task IfUserIsNoAdmin_Get2Links_UsingMoq()
        {
            //Preparation
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            mockAuthorizationService.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<string>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            var mockURLHelper = new Mock<IUrlHelper>();
            mockURLHelper.Setup(x => 
                                x.Link(It.IsAny<string>(), 
                                       It.IsAny<object>()))
                         .Returns(string.Empty);

            var rootController = new RootController(mockAuthorizationService.Object);
            rootController.Url = mockURLHelper.Object;

            //Execution
            var result = await rootController.Get();

            //Verification
            Assert.AreEqual(2, result.Value.Count());
        }
    }
}
