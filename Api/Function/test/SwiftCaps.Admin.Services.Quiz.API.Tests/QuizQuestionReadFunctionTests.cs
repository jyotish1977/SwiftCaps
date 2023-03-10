using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    public class QuizQuestionReadFunctionTests
    {
        Mock<IAdminQuizQuestionService> _service;

        public QuizQuestionReadFunctionTests()
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
        public async void InvalidToken_Should_DenyAccess(string tokenPayload)
        {
            var request = MockHelpers.CreateHttpRequestData(token: tokenPayload);
            var result = await ExecuteQuizSectionListFunction(null, _service.Object, request,Guid.NewGuid().ToString());

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
            var result = await ExecuteQuizSectionListFunction(principal.Object, _service.Object, request, payload);
            
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
            var request = MockHelpers.CreateHttpRequestData(token:"validtoken");
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });; 
            var result = await ExecuteQuizSectionListFunction(principal.Object, _service.Object, request, payload);
            
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.BadRequest);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].Contains("Missing payload or invalid payload provided.");
            result.Data.ShouldBeNull();
        }
        

        [Fact]
        public async void ValidToken_ValidScope_Should_ReturnSection()
        {
            var questionId = FakeQuestionData.Data[0].Id;
            var question = FakeQuestionData.Data[0];
            _service.Setup(x => x.GetQuestionAsync(It.IsAny<Guid>()))
                .ReturnsAsync(question);

            var request = MockHelpers.CreateHttpRequestData(string.Empty, "token", HttpMethod.Get.Method);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });
            var result = await ExecuteQuizSectionListFunction(principal.Object, _service.Object, request, questionId.ToString());

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.OK);
            result.Message.ShouldBe("Ok");
            result.ErrorMessage.ShouldBeNullOrEmpty();
            result.Errors.ShouldBeEmpty();
            result.Data.ShouldNotBeNull();
        }

        private static async Task<ServiceResponse<Question>> ExecuteQuizSectionListFunction(
            ClaimsPrincipal principal,
            IAdminQuizQuestionService service,
            HttpRequestData req,
            string quiz)
        {
            var function = GetQuizListFunction(principal, service);
            var resultObject = await function.Run(req, quiz, req.FunctionContext, Mock.Of<MILogger>());
            resultObject.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.OK,HttpStatusCode.BadRequest);
            var resultString = resultObject.ReadHttpResponseData();
            var content = JsonConvert.DeserializeObject<ServiceResponse<Question>>(resultString);
            return content;
        }

        private static QuizQuestionReadFunction GetQuizListFunction(ClaimsPrincipal principal, IAdminQuizQuestionService service)
        {
            var baseService = FunctionsMock.GetBaseSvc(principal);
            return new QuizQuestionReadFunction(service, Mock.Of<ILogger>(), baseService.Auth.Object, baseService.Env.Object);
         }
    }
}
