using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using Shouldly;
using SwiftCaps.Fake.Data;
using SwiftCaps.Functions.Tests;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using SwiftCaps.Services.Abstraction.Interfaces;
using SwiftCaps.Services.Quiz.Api;
using Xamariners.Core.Interface;
using Xamariners.Functions.Core.Extensions;
using Xamariners.RestClient.Helpers.Extensions;
using Xamariners.RestClient.Helpers.Models;
using Xunit;
using MILogger = Microsoft.Extensions.Logging.ILogger;
namespace SwiftCaps.Services.User.API.Tests
{
    public class GetAvailableUserQuizzesFunctionTests
    {
        JsonSerializerOptions serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        [Theory]
        [InlineData(null)] //No Auth Header
        [InlineData("")] //Auth header present, no token
        public async void InvalidToken_Should_DenyAccess(string token)
        {
            var quizService = new Mock<IQuizService>();
            var request = MockHelpers.CreateHttpRequestData(null,token);
            var result = await ExecuteQuizListFunction(null, quizService.Object, request);

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Unauthorized);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].ShouldBe("Access denied.");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void ValidToken_InvalidScope_Should_DenyAccess()
        {
            var quizService = new Mock<IQuizService>();
            var request = MockHelpers.CreateHttpRequestData(null,"validtoken");
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp","foo:bar")
            });
            var result = await ExecuteQuizListFunction(principal.Object, quizService.Object, request);

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Unauthorized);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].ShouldBe("Access denied.");
            result.Data.ShouldBeNull();
        }


        [Theory]
        [InlineData(null)]
        [InlineData(@"{}")]
        [InlineData(@"{""foo"":""bar""}")]
        [InlineData(@"{""ClientLocalDateTime"":""""}")]
        public async void ValidToken_ValidScope_InvalidPayload_Should_Error(string payload)
        {
            var quizService = new Mock<IQuizService>();
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp","read:userquiz"),
                new Claim("oid",FakeUserData.Data[0].Id.ToString())
            });
            var request = MockHelpers.CreateHttpRequestData(payload, "validtoken");
            var result = await ExecuteQuizListFunction(principal.Object, quizService.Object, request);

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.BadRequest);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].Contains("Missing payload or invalid payload provided");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void ValidToken_ValidScope_ValidPayload_InvalidData_Should_Error()
        {
            var quizService = new Mock<IQuizService>();
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp","read:userquiz"),
                new Claim("oid",FakeUserData.Data[0].Id.ToString())
            });
            var payload = JsonSerializer.Serialize(new UserQuizRequest
            {
                ClientLocalDateTime = DateTime.Now.AddDays(-1)
            }, serializerOptions);
            var request = MockHelpers.CreateHttpRequestData(payload,"validtoken");
            var result = await ExecuteQuizListFunction(principal.Object, quizService.Object, request);

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.BadRequest);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].Contains("Missing payload or invalid payload provided");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void ValidTokenAndScope_ValidPayload_Should_ReturnUserQuizzes()
        {
            IList<UserQuiz> expectedQuizzes = FakeUserQuizData.Data;
            var quizService = new Mock<IQuizService>();
            quizService.Setup(q => q.GetAvailableUserQuizzes(It.IsAny<UserQuizRequest>()))
                .ReturnsAsync(expectedQuizzes.AsSuccessServiceResponse("Ok"));
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp","read:userquiz"),
                new Claim("oid",FakeUserData.Data[0].Id.ToString())
            });
            var payload = JsonSerializer.Serialize(new UserQuizRequest
            {
                ClientLocalDateTime = DateTime.Now
            }, serializerOptions);
            var request = MockHelpers.CreateHttpRequestData(payload,"validtoken");
            var result = await ExecuteQuizListFunction(principal.Object, quizService.Object, request);

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.OK);
            result.ErrorMessage.ShouldBeNullOrEmpty();
            result.Errors.ShouldBeEmpty();
            result.Data.ShouldNotBeNull();
            result.Data.Count.ShouldBe(expectedQuizzes.Count);
        }

        private async Task<ServiceResponse<IList<UserQuiz>>> ExecuteQuizListFunction(
            ClaimsPrincipal principal, 
            IQuizService quizService, 
            HttpRequestData req)
        {
            var function = GetQuizListFunction(principal, quizService);
            var resultObject = await function.Run(req, req.FunctionContext, Mock.Of<MILogger>());
            resultObject.StatusCode.ShouldBeOneOf(HttpStatusCode.OK,
                HttpStatusCode.Unauthorized,
                HttpStatusCode.BadRequest);
            string resultString = resultObject.ReadHttpResponseData();
            var content = JsonSerializer.Deserialize<ServiceResponse<IList<UserQuiz>>>(resultString, serializerOptions);
            return content;
        }

        private UserQuizGetFunction GetQuizListFunction(ClaimsPrincipal principal, 
            IQuizService adminQuizService)
        {
            var baseService = FunctionsMock.GetBaseSvc(principal);
            var quizListFunction = new UserQuizGetFunction(adminQuizService, 
                Mock.Of<ILogger>(), 
                baseService.Auth.Object, 
                baseService.Env.Object);
            return quizListFunction;
        }
    }

}
