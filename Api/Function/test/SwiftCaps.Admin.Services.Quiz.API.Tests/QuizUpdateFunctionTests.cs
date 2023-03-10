using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using ScenarioTests;
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
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.API.Tests
{
    public partial class QuizUpdateFunctionTests
    {

        [Scenario]
        public async Task UsageTests(ScenarioContext scenario)
        {
            var service = new Mock<IAdminQuizService>();

            await scenario.Fact("Empty token should deny access", async () =>
            {
                var request = MockHelpers.CreateHttpRequestData(method:HttpMethod.Put.Method);

                var result = await ExecuteQuizUpdateFunction(null, service.Object, request);

                result.HttpStatus.Value.ShouldBe(HttpStatusCode.Unauthorized);
                result.ErrorMessage.ShouldBe("Error processing request.");
                result.Errors.Count.ShouldBe(1);
                result.Errors[0].ShouldBe("Access denied.");
                result.Data.ShouldBe(null);
            });

            await scenario.Fact("Valid token & scope with valid payload should update quiz", async () =>
            {
                // Arrange
                var expectedData = FakeQuizData.Data[0];
                Guid? expectedQuizId = expectedData.Id;
                var expectedUserId = FakeUserData.Data[0].Id.ToString();
                service.Setup(x => x.UpdateQuizAsync(It.IsAny<Guid>(), It.IsAny<SCModels.Quiz>()))
                    .ReturnsAsync(expectedQuizId);

                var principal = new Mock<ClaimsPrincipal>();
                principal.Setup(x => x.Claims).Returns(new List<Claim>
                {
                    new Claim("scp", ScopeConstants.QuizCUD),
                    new Claim("oid", expectedUserId)
                });

                var payload = JsonSerializer.Serialize(expectedData, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                var request = MockHelpers.CreateHttpRequestData(payload, "token", HttpMethod.Put.Method);

                // Act
                var result = await ExecuteQuizUpdateFunction(principal.Object, service.Object, request, expectedQuizId.ToString());

                // Assert
                result.HttpStatus.Value.ShouldBe(HttpStatusCode.Accepted);
                result.Message.ShouldBe("Ok");
                result.ErrorMessage.ShouldBeNullOrEmpty();
                result.Errors.ShouldBeEmpty();
                result.Data.ShouldBe(expectedQuizId.Value);
            });


        }
              
        private async Task<ServiceResponse<Guid?>> ExecuteQuizUpdateFunction(
            ClaimsPrincipal principal,
            IAdminQuizService adminQuizService,
            HttpRequestData req,
            string quizId = null)
        {
            var function = GetQuizUpdateFunction(principal, adminQuizService);
            var resultObject = await function.Run(req, quizId, req.FunctionContext, Mock.Of<MILogger>());

            // Assert for the original HttpStatusCode
            resultObject.StatusCode.ShouldBeOneOf(HttpStatusCode.Accepted, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);

            var resultString = resultObject.ReadHttpResponseData();
            var content = JsonSerializer.Deserialize<ServiceResponse<Guid?>>(resultString, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            return content;
        }

        private QuizUpdateFunction GetQuizUpdateFunction(
           ClaimsPrincipal principal,
           IAdminQuizService adminQuizService)
        {
            var baseService = FunctionsMock.GetBaseSvc(principal);
            var quizCreateFunction = new QuizUpdateFunction(adminQuizService, Mock.Of<ILogger>(),
                                                            baseService.Auth.Object, baseService.Env.Object);
            return quizCreateFunction;
        }
    }
}
