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
    public class QuizSectionDeleteFunctionTests
    {
        Mock<IAdminQuizSectionService> service;

        public QuizSectionDeleteFunctionTests()
        {
            service = new Mock<IAdminQuizSectionService>();
        }

        [Fact]
        public async void EmptyToken_Should_DenyAccess()
        {
            string payloadId = null;
            string payload = null;
            var request = MockHelpers.CreateHttpRequestData(payload: payload, method: HttpMethod.Delete.Method);
            var result = await ExecuteQuizSectionDeleteFunction(null, service.Object, request, payloadId);

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
            var request = MockHelpers.CreateHttpRequestData(payload: payload, token:"validtoken", method: HttpMethod.Delete.Method);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", "foo")
            }); 
            var result = await ExecuteQuizSectionDeleteFunction(principal.Object, service.Object, request, payloadId);
            
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Unauthorized);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].ShouldBe("Access denied.");
            result.Data.ShouldBeFalse();
        }

        [Fact]
        public async void ValidToken_ValidScope_EmptyPayload_Should_Error()
        {
            string payloadId = null;
            string payload = null;
            var request = MockHelpers.CreateHttpRequestData(payload: payload, token:"validtoken", method: HttpMethod.Delete.Method);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });
            var result = await ExecuteQuizSectionDeleteFunction(principal.Object, service.Object, request, payloadId);
            
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.BadRequest);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].Contains("Missing payload or invalid payload provided.");
            result.Data.ShouldBeFalse();
        }

        [Fact]
        public async void ValidToken_ValidScope_InvalidPayload_Should_Error()
        {
            string payloadId = "abc";
            string payload = "{}";
            var request = MockHelpers.CreateHttpRequestData(payload: payload, token:"validtoken", method: HttpMethod.Delete.Method);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });
            var result = await ExecuteQuizSectionDeleteFunction(principal.Object, service.Object, request, payloadId);
            
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.BadRequest);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].Contains("Missing payload or invalid payload provided.");
            result.Data.ShouldBeFalse();
        }

        [Fact]
        public async void ValidToken_ValidScope_Should_ReturnSection()
        {
            var payloadId = Guid.NewGuid().ToString();
            service.Setup(x => x.DeleteSectionAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);
            var request = MockHelpers.CreateHttpRequestData(token:"token", method:HttpMethod.Delete.Method);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });
            
            await ExecuteQuizSectionDeleteFunction(principal.Object, service.Object, request, payloadId);

        }

        private static async Task<ServiceResponse<bool>> ExecuteQuizSectionDeleteFunction(
            ClaimsPrincipal principal,
            IAdminQuizSectionService sectionService,
            HttpRequestData req,
            string section
        )
        {
            var function = GetFunction(principal, sectionService);
            var resultObject = await function.Run(req,section, req.FunctionContext, Mock.Of<MILogger>());
            resultObject.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.NoContent, HttpStatusCode.OK,HttpStatusCode.BadRequest);
            if(resultObject.StatusCode != HttpStatusCode.NoContent)
            {
                var resultString = resultObject.ReadHttpResponseData();
                var content = JsonSerializer.Deserialize<ServiceResponse<bool>>(resultString, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                return content;
            }
            return null;
        }

        private static QuizSectionDeleteFunction GetFunction(ClaimsPrincipal principal, IAdminQuizSectionService sectionService)
        {
            var baseService = FunctionsMock.GetBaseSvc(principal);
            return new QuizSectionDeleteFunction(sectionService, Mock.Of<ILogger>(), baseService.Auth.Object, baseService.Env.Object);
        }
    }
}
