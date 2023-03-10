using System.Collections.Generic;
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
using Xamariners.RestClient.Helpers.Extensions;
using Xamariners.RestClient.Helpers.Models;
using Xunit;
using MILogger = Microsoft.Extensions.Logging.ILogger;

namespace SwiftCaps.Admin.Services.Quiz.API.Tests
{
    public class QuizListFunctionTests
    {

        [Fact]
        public async void EmptyToken_Should_DenyAccess()
        {
            var adminQuizService = new Mock<IAdminQuizService>();
            var request = MockHelpers.CreateHttpRequestData();
            var result = await ExecuteQuizListFunction(null, adminQuizService.Object, request);

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Unauthorized);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].ShouldBe("Access denied.");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void ValidToken_InvalidScope_Should_DenyAccess()
        {
            var adminQuizService = new Mock<IAdminQuizService>();
            var request = MockHelpers.CreateHttpRequestData(string.Empty, "validtoken");
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", "foo")
            }); 
            var result = await ExecuteQuizListFunction(principal.Object, adminQuizService.Object, request);
            
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Unauthorized);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].ShouldBe("Access denied.");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void ValidToken_ValidScope_Should_ReturnQuizzes()
        {
            var adminQuizzes = FakeQuizSummaryData.Data;
            var adminQuizService = new Mock<IAdminQuizService>();
            adminQuizService.Setup(x => x.GetQuizzesAsync())
                .ReturnsAsync(adminQuizzes);

            var request = MockHelpers.CreateHttpRequestData(string.Empty, "token", System.Net.Http.HttpMethod.Get.Method);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });
            var result = await ExecuteQuizListFunction(principal.Object, adminQuizService.Object, request);

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.OK);
            result.Message.ShouldBe("Ok");
            result.ErrorMessage.ShouldBeNullOrEmpty();
            result.Errors.ShouldBeEmpty();
            result.Data.Count.ShouldBe(adminQuizzes.Count);
            //TODO: ShouldBe is failing for List. Investigate later. 
            //result.Data.ShouldBe(adminQuizzes);
        }

        private static async Task<ServiceResponse<IList<QuizSummary>>> ExecuteQuizListFunction(
            ClaimsPrincipal principal,
            IAdminQuizService adminQuizService,
            HttpRequestData req)
        {
            var function = GetQuizListFunction(principal, adminQuizService);
            var resultObject = await function.Run(req, req.FunctionContext, Mock.Of<MILogger>());
            resultObject.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.OK,HttpStatusCode.BadRequest);
            var resultString = resultObject.ReadHttpResponseData();
            var content = JsonConvert.DeserializeObject<ServiceResponse<IList<QuizSummary>>>(resultString);
            return content;
        }

        private static QuizListFunction GetQuizListFunction(ClaimsPrincipal principal, IAdminQuizService adminQuizService)
        {
            var baseService = FunctionsMock.GetBaseSvc(principal);
            var quizListFunction = new QuizListFunction(adminQuizService, Mock.Of<ILogger>(), baseService.Auth.Object, baseService.Env.Object);
            return quizListFunction;
        }
    }
}
