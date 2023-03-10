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
using SwiftCaps.Services.Abstraction.Interfaces;
using Xamariners.Core.Interface;
using Xamariners.Functions.Core.Extensions;
using Xamariners.RestClient.Helpers.Models;
using Xunit;
using MILogger = Microsoft.Extensions.Logging.ILogger;

namespace SwiftCaps.Admin.Services.Quiz.API.Tests
{
    public class QuizQuestionDeleteFunctionTests
    {
        Mock<IAdminQuizQuestionService> service;

        public QuizQuestionDeleteFunctionTests()
        {
            service = new Mock<IAdminQuizQuestionService>();
        }

        public static IEnumerable<object[]> InvalidTokenData()
        {
            yield return new object[] { null };
            yield return new object[] { string.Empty };
            yield return new object[] { "foo" };
        }

        public static IEnumerable<object[]> InvalidPayloadData()
        {
            yield return new object[] { null };
            yield return new object[] { Guid.Empty.ToString() };
            yield return new object[] { "foo" };
        }

        [Theory]
        [MemberData(nameof(InvalidTokenData))]
        public async void InvalidToken_Should_DenyAccess(string tokenData)
        {
            string payloadId = Guid.NewGuid().ToString();
            var request = MockHelpers.CreateHttpRequestData(token:tokenData, method: HttpMethod.Delete.Method);
            var result = await ExecuteFunction(null, service.Object, request, payloadId);

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Unauthorized);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].ShouldBe("Access denied.");
            result.Data.ShouldBeFalse();
        }

        [Fact]
        public async void ValidToken_InvalidScope_Should_DenyAccess()
        {
            string payloadId = null;
            string payload = null;
            var request = MockHelpers.CreateHttpRequestData(payload: payload, token: "validtoken", method: HttpMethod.Delete.Method);
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
            result.Data.ShouldBeFalse();
        }

        [Theory]
        [MemberData(nameof(InvalidPayloadData))]
        public async void ValidTokenAndScope_InvalidPayload_Should_Error(string payloadId)
        {
            var request = MockHelpers.CreateHttpRequestData(token: "validtoken", method: HttpMethod.Delete.Method);
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
            result.Data.ShouldBeFalse();
        }

        [Fact]
        public async void ValidToken_ValidScope_ValidPayload_Should_DeleteSection()
        {
            var payloadId = Guid.NewGuid().ToString();
            service.Setup(x => x.DeleteQuestionAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);
            var request = MockHelpers.CreateHttpRequestData(token: "token", method: HttpMethod.Delete.Method);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });

            await ExecuteFunction(principal.Object, service.Object, request, payloadId);

        }

        private static async Task<ServiceResponse<bool>> ExecuteFunction(
            ClaimsPrincipal principal,
            IAdminQuizQuestionService service,
            HttpRequestData req,
            string section
        )
        {
            var function = GetFunction(principal, service);
            var resultObject = await function.Run(req, section, req.FunctionContext, Mock.Of<MILogger>());
            resultObject.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.NoContent, HttpStatusCode.BadRequest);
            if (resultObject.StatusCode != HttpStatusCode.NoContent)
            {
                var resultString = resultObject.ReadHttpResponseData();
                var content = JsonSerializer.Deserialize<ServiceResponse<bool>>(resultString, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                return content;
            }
            return null;
        }

        private static QuizQuestionDeleteFunction GetFunction(ClaimsPrincipal principal, IAdminQuizQuestionService service)
        {
            var baseService = FunctionsMock.GetBaseSvc(principal);
            return new QuizQuestionDeleteFunction(service, Mock.Of<ILogger>(), baseService.Auth.Object, baseService.Env.Object);
        }
    }
}
