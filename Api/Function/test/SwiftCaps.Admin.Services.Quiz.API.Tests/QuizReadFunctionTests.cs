using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using MoreLinq;
using Newtonsoft.Json;
using Shouldly;
using SwiftCaps.Fake.Data;
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
    public class QuizReadFunctionTests
    {
        private const string EmptyQuizGuid = "{00000000-0000-0000-0000-000000000000}";
        private const string InvalidQuizGuid = "invalid guid";

        [Fact]
        public async void EmptyToken_Should_DenyAccess()
        {
            var adminQuizService = new Mock<IAdminQuizService>();
            var request = MockHelpers.CreateHttpRequestData();
            var result = await ExecuteQuizReadFunction(null, adminQuizService.Object, request, Guid.NewGuid().ToString());

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
            var request = MockHelpers.CreateHttpRequestData(null, "validtoken");
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", "foo")
            });
            var result = await ExecuteQuizReadFunction(principal.Object, adminQuizService.Object, request, Guid.NewGuid().ToString());

            result.HttpStatus.Value.ShouldBe(HttpStatusCode.Unauthorized);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].ShouldBe("Access denied.");
            result.Data.ShouldBeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(EmptyQuizGuid)]
        [InlineData(InvalidQuizGuid)]
        public async void ValidToken_ValidScope_InvalidPayload_Should_Error(string quizIdentifier)
        {
            // Arrange
            var adminQuizService = new Mock<IAdminQuizService>();
            var request = MockHelpers.CreateHttpRequestData(string.Empty, "token");
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });

            // Act
            var result = await ExecuteQuizReadFunction(
                principal.Object, adminQuizService.Object, request, quizIdentifier);

            // Assert
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.BadRequest);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].Contains("Missing payload or invalid payload provided.");
            result.Data.ShouldBeNull();
        }


        [Fact]
        public async void ValidToken_ValidScope_ValidQuizId_Should_ReturnQuizSummaries()
        {
            // Arrange

            var adminQuizzes = FakeQuizSummaryData.Data;
            var adminQuizService = new Mock<IAdminQuizService>();
            var quizzesFaker = new Faker<Models.Models.Quiz>()
                .RuleFor(x => x.Id, v => adminQuizzes.First().Id);
            var quizzesFakerResult = quizzesFaker.Generate(1);
            adminQuizService.Setup(x => x.GetQuizAsync(adminQuizzes.First().Id))
                            .ReturnsAsync(quizzesFakerResult.First());

            var request = MockHelpers.CreateHttpRequestData(string.Empty, "token");
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.QuizRead)
            });

            // Act
            var result = await ExecuteQuizReadFunction(principal.Object, adminQuizService.Object, request, adminQuizzes.First().Id.ToString());

            // Assert
            result.HttpStatus.Value.ShouldBe(HttpStatusCode.OK);
            result.Message.ShouldBe("Ok");
            result.ErrorMessage.ShouldBeNullOrEmpty();
            result.Errors.ShouldBeEmpty();
            result.Data.Id.ShouldBe(adminQuizzes.First().Id);
        }

        private static async Task<ServiceResponse<Models.Models.Quiz>> ExecuteQuizReadFunction(
            ClaimsPrincipal principal,
            IAdminQuizService adminQuizService,
            HttpRequestData req,
            string quizId)
        {
            var function = GetQuizReadFunction(principal, adminQuizService);
            var resultObject = await function.Run(req, quizId, req.FunctionContext, Mock.Of<MILogger>());
            resultObject.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.OK, HttpStatusCode.BadRequest);
            var resultString = resultObject.ReadHttpResponseData();
            var content = JsonConvert.DeserializeObject<ServiceResponse<Models.Models.Quiz>>(resultString);
            return content;
        }

        private static QuizReadFunction GetQuizReadFunction(ClaimsPrincipal principal, IAdminQuizService adminQuizService)
        {
            var baseService = FunctionsMock.GetBaseSvc(principal);
            var quizReadFunction = new QuizReadFunction(adminQuizService, Mock.Of<ILogger>(), baseService.Auth.Object, baseService.Env.Object);
            return quizReadFunction;
        }

    }
}
