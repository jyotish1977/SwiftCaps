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
using SwiftCaps.Functions.Tests;
using SwiftCaps.Models.Constants;
using SwiftCaps.Models.Models;
using SwiftCaps.Services.Abstraction.Interfaces;
using Xamariners.Core.Interface;
using Xamariners.Functions.Core.Extensions;
using Xamariners.RestClient.Helpers.Models;
using Xunit;
using MILogger = Microsoft.Extensions.Logging.ILogger;

namespace SwiftCaps.Admin.Services.Quiz.API.Tests
{
    public class QuizQuestionCreateFunctionTests
    {
        Mock<IAdminQuizQuestionService> _service;

        public QuizQuestionCreateFunctionTests()
        {
            _service = new Mock<IAdminQuizQuestionService>();
        }

        public static IEnumerable<object[]> InvalidTokenData()
        {
            yield return new object[] { null };
            yield return new object[] { string.Empty};
            yield return new object[] { "foo" };
        }

        public static IEnumerable<object[]> InvalidPayloadData()
        {
            yield return new object[] { null };
            yield return new object[] { Guid.Empty.ToString()};
            yield return new object[] { "foo" };
        }

        [Theory]
        [MemberData(nameof(InvalidTokenData))]
        public async void EmptyToken_Should_DenyAccess(string tokenData)
        {
            var request = MockHelpers.CreateHttpRequestData(token: tokenData, method: HttpMethod.Post.Method);
            var result = await ExecuteFunction(null, _service.Object, request);

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Unauthorized);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].ShouldBe("Access denied.");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void ValidToken_InvalidScope_Should_DenyAccess()
        {
            string payload = null;
            var request = MockHelpers.CreateHttpRequestData(payload: payload, token:"validtoken", method: HttpMethod.Post.Method);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", "foo")
            }); 
            var result = await ExecuteFunction(principal.Object, _service.Object, request);
            
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Unauthorized);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].ShouldBe("Access denied.");
            result.Data.ShouldBeNull();
        }

        [Theory]
        [MemberData(nameof(InvalidPayloadData))]
        public async void ValidTokenAndScope_InvalidPayload_Should_Error(string payload)
        {
            var request = MockHelpers.CreateHttpRequestData(payload: payload, token:"validtoken", method: HttpMethod.Post.Method);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });; 
            var result = await ExecuteFunction(principal.Object, _service.Object, request);
            
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.BadRequest);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].Contains("Missing payload or invalid payload provided.");
            result.Data.ShouldBeNull();
        }
        

        [Fact]
        public async void ValidToken_ValidScope_Should_CreateQuestion()
        {
            var question = new Question { QuizSectionId = Guid.NewGuid(), Body="Body", Header="Header", Footer="Footer" };
            _service.Setup(x => x.CreateQuestionAsync(It.IsAny<Question>()))
                .ReturnsAsync(question.Id);
            var payload = JsonSerializer.Serialize(question, new JsonSerializerOptions{ PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var request = MockHelpers.CreateHttpRequestData(payload:payload, "token", HttpMethod.Post.Method);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });
            var result = await ExecuteFunction(principal.Object, _service.Object, request);

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Created);
            result.Message.ShouldBe("Ok");
            result.ErrorMessage.ShouldBeNullOrEmpty();
            result.Errors.ShouldBeEmpty();
            result.Data.ShouldBe(question.Id);
        }

        private static async Task<ServiceResponse<Guid?>> ExecuteFunction(
            ClaimsPrincipal principal,
            IAdminQuizQuestionService service,
            HttpRequestData req
        )
        {
            var function = GetFunction(principal, service);
            var resultObject = await function.Run(req, req.FunctionContext, Mock.Of<MILogger>());
            resultObject.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Created, HttpStatusCode.OK,HttpStatusCode.BadRequest);
            var resultString = resultObject.ReadHttpResponseData();
            var content = JsonSerializer.Deserialize<ServiceResponse<Guid?>>(resultString, new JsonSerializerOptions{ PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
            return content;
        }

        private static QuizQuestionCreateFunction GetFunction(ClaimsPrincipal principal, IAdminQuizQuestionService service)
        {
            var baseService = FunctionsMock.GetBaseSvc(principal);
            return new QuizQuestionCreateFunction(service, Mock.Of<ILogger>(), baseService.Auth.Object, baseService.Env.Object);
        }
    }
}
