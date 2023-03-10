using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using Shouldly;
using SwiftCaps.Fake.Data;
using SwiftCaps.Functions.Tests;
using SwiftCaps.Models.Constants;
using SwiftCaps.Services.Abstraction.Interfaces;
using Xamariners.Core.Interface;
using Xamariners.Functions.Core.Extensions;
using Xamariners.RestClient.Helpers.Models;
using Xunit;
using MILogger = Microsoft.Extensions.Logging.ILogger;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.API.Tests
{
    public class QuizCreateFunctionTests
    {

        [Fact]
        public async void EmptyToken_Should_DenyAccess()
        {
            // Arrange

            var adminQuizService = new Mock<IAdminQuizService>();
            var request = MockHelpers.CreateHttpRequestData();

            // Act
            var result = await ExecuteQuizCreateFunction(null, adminQuizService.Object, request);

            // Assert
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Unauthorized);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].ShouldBe("Access denied.");
            result.Data.ShouldBe(null);
        }

        [Fact]
        public async void ValidTokenAndScope_ValidPayload_Should_CreateQuiz()
        {
            // Arrange
            var expectedData = FakeQuizData.Data[0];
            Guid? expectedQuizId = expectedData.Id;
            var expectedUserId = FakeUserData.Data[0].Id.ToString();
            var adminQuizService = new Mock<IAdminQuizService>();
            adminQuizService.Setup(x => x.CreateQuizAsync(It.IsAny<SCModels.Quiz>()))
                .ReturnsAsync(expectedQuizId);

            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizCUD),
                new Claim("oid", expectedUserId)
            });

            var payload = JsonSerializer.Serialize(expectedData, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            var request = MockHelpers.CreateHttpRequestData(payload, "token", HttpMethod.Post.Method);

            // Act
            var result = await ExecuteQuizCreateFunction(principal.Object, adminQuizService.Object, request);

            // Assert
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Created);
            result.Message.ShouldBe("Ok");
            result.ErrorMessage.ShouldBeNullOrEmpty();
            result.Errors.ShouldBeEmpty();
            result.Data.ShouldBe(expectedQuizId.Value);
        }

        private QuizCreateFunction GetQuizCreateFunction(
            ClaimsPrincipal principal, 
            IAdminQuizService adminQuizService)
        {
            var baseService = FunctionsMock.GetBaseSvc(principal);
            var quizCreateFunction = new QuizCreateFunction(
                adminQuizService, Mock.Of<ILogger>(), baseService.Auth.Object, baseService.Env.Object);
            return quizCreateFunction;
        }

        private async Task<ServiceResponse<Guid?>> ExecuteQuizCreateFunction(
            ClaimsPrincipal principal,
            IAdminQuizService adminQuizService,
            HttpRequestData req)
        {
            var function = GetQuizCreateFunction(principal, adminQuizService);
            var resultObject = await function.Run(req, req.FunctionContext, Mock.Of<MILogger>());

            // Assert for the original HttpStatusCode
            resultObject.StatusCode.ShouldBeOneOf(HttpStatusCode.Created, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);

            var resultString = resultObject.ReadHttpResponseData();
            var content = JsonSerializer.Deserialize<ServiceResponse<Guid?>>(resultString, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            return content;
        }

    }
}
