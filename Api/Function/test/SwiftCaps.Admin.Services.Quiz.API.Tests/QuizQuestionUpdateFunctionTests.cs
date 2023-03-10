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
    public class QuizQuestionUpdateFunctionTests
    {
        Mock<IAdminQuizQuestionService> service;

        public QuizQuestionUpdateFunctionTests()
        {
            service = new Mock<IAdminQuizQuestionService>();
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
        public async void InvalidToken_Should_DenyAccess(string tokenData)
        {
            string payloadId = null;
            var request = MockHelpers.CreateHttpRequestData(token: tokenData, method: HttpMethod.Put.Method);
            var result = await ExecuteFunction(null, service.Object, request, payloadId);

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Unauthorized);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].ShouldBe("Access denied.");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void ValidToken_InvalidScope_Should_DenyAccess()
        {
            string payloadId = null;
            string payload = null;
            var request = MockHelpers.CreateHttpRequestData(payload: payload, token:"validtoken", method: HttpMethod.Put.Method);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", "foo")
            }); 
            var result = await ExecuteFunction(principal.Object, service.Object, request, payloadId);
            
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Unauthorized);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].ShouldBe("Access denied.");
            result.Data.ShouldBeNull();
        }

        [Theory]
        [MemberData(nameof(InvalidPayloadData))]
        public async void ValidTokenAndScope_InvalidPayload_Should_Error(string payloadId)
        {
            var request = MockHelpers.CreateHttpRequestData(token:"validtoken", method: HttpMethod.Put.Method);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });
            var result = await ExecuteFunction(principal.Object, service.Object, request, payloadId);
            
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.BadRequest);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].Contains("Missing payload or invalid payload provided.");
            result.Data.ShouldBeNull();
        }
                

        [Fact]
        public async void ValidToken_ValidScope_Should_UpdateQuestion()
        {
            var question = new Question { QuizSectionId = Guid.NewGuid(), Body="Body updated", Header="Header", Footer="Footer", QuizSectionIndex=1  };
            var payloadId = question.Id.ToString();
            service.Setup(x => x.UpdateQuestionAsync(It.IsAny<Guid>(), It.IsAny<Question>()))
                .ReturnsAsync(question.Id);
            var payload = JsonSerializer.Serialize(question, new JsonSerializerOptions{ PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var request = MockHelpers.CreateHttpRequestData(payload:payload, "token", HttpMethod.Put.Method);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });
            var result = await ExecuteFunction(principal.Object, service.Object, request, payloadId);

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Accepted);
            result.Message.ShouldBe("Ok");
            result.ErrorMessage.ShouldBeNullOrEmpty();
            result.Errors.ShouldBeEmpty();
            result.Data.ShouldBe(question.Id);
        }

        private static async Task<ServiceResponse<Guid?>> ExecuteFunction(
            ClaimsPrincipal principal,
            IAdminQuizQuestionService service,
            HttpRequestData req,
            string quizId
        )
        {
            var function = GetFunction(principal, service);
            var resultObject = await function.Run(req,quizId, req.FunctionContext, Mock.Of<MILogger>());
            resultObject.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Accepted, HttpStatusCode.BadRequest);
            var resultString = resultObject.ReadHttpResponseData();
            var content = JsonSerializer.Deserialize<ServiceResponse<Guid?>>(resultString, new JsonSerializerOptions{ PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
            return content;
        }

        private static QuizQuestionUpdateFunction GetFunction(ClaimsPrincipal principal, IAdminQuizQuestionService service)
        {
            var baseService = FunctionsMock.GetBaseSvc(principal);
            return new QuizQuestionUpdateFunction(service, Mock.Of<ILogger>(), baseService.Auth.Object, baseService.Env.Object);
        }
    }
}
