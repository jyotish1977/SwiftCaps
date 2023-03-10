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
    public class QuizSectionCreateFunctionTests
    {
        Mock<IAdminQuizSectionService> service;

        public QuizSectionCreateFunctionTests()
        {
            service = new Mock<IAdminQuizSectionService>();
        }

        [Fact]
        public async void EmptyToken_Should_DenyAccess()
        {
            string payload = null;
            var request = MockHelpers.CreateHttpRequestData(payload: payload, method: HttpMethod.Post.Method);
            var result = await ExecuteQuizSectionCreateFunction(null, service.Object, request);

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
            var result = await ExecuteQuizSectionCreateFunction(principal.Object, service.Object, request);
            
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Unauthorized);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].ShouldBe("Access denied.");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void ValidToken_ValidScope_EmptyPayload_Should_Error()
        {
            string payload = null;
            var request = MockHelpers.CreateHttpRequestData(payload: payload, token:"validtoken", method: HttpMethod.Post.Method);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });; 
            var result = await ExecuteQuizSectionCreateFunction(principal.Object, service.Object, request);
            
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.BadRequest);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].Contains("Missing payload or invalid payload provided.");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void ValidToken_ValidScope_InvalidPayload_Should_Error()
        {
            string payload = "{}";
            var request = MockHelpers.CreateHttpRequestData(payload: payload, token:"validtoken", method: HttpMethod.Post.Method);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });
            var result = await ExecuteQuizSectionCreateFunction(principal.Object, service.Object, request);
            
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.BadRequest);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].Contains("Missing payload or invalid payload provided.");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void ValidToken_ValidScope_Should_ReturnSection()
        {
            var section = new QuizSection { QuizId = Guid.NewGuid(), Description="Section 1" };
            service.Setup(x => x.CreateSectionAsync(It.IsAny<QuizSection>()))
                .ReturnsAsync(section.Id);
            var payload = JsonSerializer.Serialize(section, new JsonSerializerOptions{ PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var request = MockHelpers.CreateHttpRequestData(payload:payload, "token", HttpMethod.Post.Method);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });
            var result = await ExecuteQuizSectionCreateFunction(principal.Object, service.Object, request);

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Created);
            result.Message.ShouldBe("Ok");
            result.ErrorMessage.ShouldBeNullOrEmpty();
            result.Errors.ShouldBeEmpty();
            result.Data.ShouldBe(section.Id);
        }

        private static async Task<ServiceResponse<Guid?>> ExecuteQuizSectionCreateFunction(
            ClaimsPrincipal principal,
            IAdminQuizSectionService sectionService,
            HttpRequestData req
        )
        {
            var function = GetFunction(principal, sectionService);
            var resultObject = await function.Run(req, req.FunctionContext, Mock.Of<MILogger>());
            resultObject.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Created, HttpStatusCode.OK,HttpStatusCode.BadRequest);
            var resultString = resultObject.ReadHttpResponseData();
            var content = JsonSerializer.Deserialize<ServiceResponse<Guid?>>(resultString, new JsonSerializerOptions{ PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
            return content;
        }

        private static QuizSectionCreateFunction GetFunction(ClaimsPrincipal principal, IAdminQuizSectionService sectionService)
        {
            var baseService = FunctionsMock.GetBaseSvc(principal);
            return new QuizSectionCreateFunction(sectionService, Mock.Of<ILogger>(), baseService.Auth.Object, baseService.Env.Object);
        }
    }
}
