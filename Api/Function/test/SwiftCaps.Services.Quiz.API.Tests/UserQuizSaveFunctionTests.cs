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
    public class UserQuizSaveFunctionTests
    {

        [Theory]
        [InlineData(null)] //No Auth Header
        [InlineData("")] //Auth header present, no token
        public async void InvalidToken_Should_DenyAccess(string token)
        {
            var quizService = new Mock<IQuizService>();
            var request = MockHelpers.CreateHttpRequestData(null, token);
            var result = await ExecuteUserQuizAddFunction<bool>(null, quizService.Object, request);

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Unauthorized);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].ShouldBe("Access denied.");
            result.Data.ShouldBeFalse();
        }

        [Fact]
        public async void ValidToken_InvalidScope_Should_DenyAccess()
        {
            var quizService = new Mock<IQuizService>();
            var request = MockHelpers.CreateHttpRequestData(null, "validtoken");
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp","foo:bar")
            });
            var result = await ExecuteUserQuizAddFunction<bool>(principal.Object, quizService.Object, request);

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Unauthorized);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].ShouldBe("Access denied.");
            result.Data.ShouldBeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("{}")]
        [InlineData(@"{""foo"":""bar""}")]
        public async void ValidToken_ValidScope_InvalidPayload_Should_Error(string payload)
        {
            var quizService = new Mock<IQuizService>();
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp","create:userquiz"),
                new Claim("oid",FakeUserData.Data[0].Id.ToString())
            });
            var request = MockHelpers.CreateHttpRequestData(payload, "validtoken");
            var result = await ExecuteUserQuizAddFunction<bool>(principal.Object, quizService.Object, request);

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.BadRequest);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].Contains("Missing payload or invalid payload provided.");
            result.Data.ShouldBeFalse();
        }

        [Fact]
        public async void ValidTokenAndScope_ValidPayload_Should_SaveUserQuiz()
        {
            UserQuiz expectedUserQuiz = FakeUserQuizData.Data[0];

            var quizService = new Mock<IQuizService>();
            quizService.Setup(q => q.SaveUserQuiz(It.IsAny<UserQuiz>()))
                .ReturnsAsync(true.AsSuccessServiceResponse("Ok"));
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp","create:userquiz"),
                new Claim("oid",FakeUserData.Data[0].Id.ToString())
            });
            expectedUserQuiz.Completed = DateTime.UtcNow;
            var payload = JsonSerializer.Serialize(expectedUserQuiz, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            var request = MockHelpers.CreateHttpRequestData(payload, "validtoken");
            var result = await ExecuteUserQuizAddFunction<bool>(principal.Object, quizService.Object, request);

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.OK);
            result.ErrorMessage.ShouldBeNullOrEmpty();
            result.Errors.ShouldBeEmpty();
            result.Data.ShouldBeTrue();
        }

        private async Task<ServiceResponse<T>> ExecuteUserQuizAddFunction<T>(
            ClaimsPrincipal principal,
            IQuizService quizService,
            HttpRequestData req)
        {
            var function = GetUserQuizSaveFunction(principal, quizService);
            var resultObject = await function.Run(req, req.FunctionContext, Mock.Of<MILogger>());
            string resultString = resultObject.ReadHttpResponseData();
            var content = JsonSerializer.Deserialize<ServiceResponse<T>>(resultString, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            return content;
        }

        private UserQuizSaveFunction GetUserQuizSaveFunction(ClaimsPrincipal principal,
            IQuizService adminQuizService)
        {
            var baseService = FunctionsMock.GetBaseSvc(principal);
            var function = new UserQuizSaveFunction(adminQuizService,
                Mock.Of<ILogger>(),
                baseService.Auth.Object,
                baseService.Env.Object);
            return function;
        }
    }
}
