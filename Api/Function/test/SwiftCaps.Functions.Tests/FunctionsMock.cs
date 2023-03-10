using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using Moq;
using Xamariners.Functions.Core.Configuration;
using Xamariners.Functions.Core.Interfaces;

namespace SwiftCaps.Functions.Tests
{
    public class FunctionsMock
    {
        public static (Mock<IFunctionAuthorizationService> Auth, Mock<AzureADConfiguration> Env) GetBaseSvc(ClaimsPrincipal principal)
        {
            var mockFunctionAuthorizationService = new Mock<IFunctionAuthorizationService>();
            mockFunctionAuthorizationService.Setup(x => x.Authorize(It.IsAny<AuthenticationHeaderValue>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync(Guid.NewGuid());
            mockFunctionAuthorizationService.Setup(x => x.ValidateToken(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>())).
                Returns(principal);

            return (mockFunctionAuthorizationService, new Mock<AzureADConfiguration>());
        }
    }
}
