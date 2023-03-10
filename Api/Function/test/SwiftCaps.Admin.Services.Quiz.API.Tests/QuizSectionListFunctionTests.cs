using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using Newtonsoft.Json;
using Shouldly;
using SwiftCaps.Fake.Data;
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
    public class QuizSectionListFunctionTests
    {
        Mock<IAdminQuizSectionService> service;

        public QuizSectionListFunctionTests()
        {
            service = new Mock<IAdminQuizSectionService>();
        }

        [Fact]
        public async void EmptyToken_Should_DenyAccess()
        {
            string payload = null;
            var request = MockHelpers.CreateHttpRequestData();
            var result = await ExecuteQuizSectionListFunction(null, service.Object, request,payload);

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
            var request = MockHelpers.CreateHttpRequestData(token:"validtoken");
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", "foo")
            }); 
            var result = await ExecuteQuizSectionListFunction(principal.Object, service.Object, request, payload);
            
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
            var request = MockHelpers.CreateHttpRequestData(token:"validtoken");
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });; 
            var result = await ExecuteQuizSectionListFunction(principal.Object, service.Object, request, payload);
            
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.BadRequest);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].Contains("Missing payload or invalid payload provided.");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void ValidToken_ValidScope_InvalidPayload_Should_Error()
        {
            string payload = "abc";
            var request = MockHelpers.CreateHttpRequestData(token:"validtoken");
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });; 
            var result = await ExecuteQuizSectionListFunction(principal.Object, service.Object, request, payload);
            
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.BadRequest);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].Contains("Missing payload or invalid payload provided.");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void ValidToken_ValidScope_Should_ReturnSections()
        {
            var quiz = FakeQuizData.Data[0].Id;
            var quizSections = FakeQuizSectionData.Data.Where(section => section.QuizId == quiz).ToList();
            service.Setup(x => x.GetSectionsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(quizSections);

            var request = MockHelpers.CreateHttpRequestData(string.Empty, "token", System.Net.Http.HttpMethod.Get.Method);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });
            var result = await ExecuteQuizSectionListFunction(principal.Object, service.Object, request, quiz.ToString());

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.OK);
            result.Message.ShouldBe("Ok");
            result.ErrorMessage.ShouldBeNullOrEmpty();
            result.Errors.ShouldBeEmpty();
            result.Data.Count.ShouldBe(quizSections.Count);
        }

        private static async Task<ServiceResponse<IList<QuizSection>>> ExecuteQuizSectionListFunction(
            ClaimsPrincipal principal,
            IAdminQuizSectionService sectionService,
            HttpRequestData req,
            string quiz)
        {
            var function = GetQuizListFunction(principal, sectionService);
            var resultObject = await function.Run(req, quiz, req.FunctionContext, Mock.Of<MILogger>());
            resultObject.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.OK,HttpStatusCode.BadRequest);
            var resultString = resultObject.ReadHttpResponseData();
            var content = JsonConvert.DeserializeObject<ServiceResponse<IList<QuizSection>>>(resultString);
            return content;
        }

        private static QuizSectionListFunction GetQuizListFunction(ClaimsPrincipal principal, IAdminQuizSectionService sectionService)
        {
            var baseService = FunctionsMock.GetBaseSvc(principal);
            return new QuizSectionListFunction(sectionService, Mock.Of<ILogger>(), baseService.Auth.Object, baseService.Env.Object);
        }
    }
}
