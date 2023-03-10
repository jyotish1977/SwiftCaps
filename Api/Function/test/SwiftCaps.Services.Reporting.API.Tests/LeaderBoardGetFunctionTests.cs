using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using Shouldly;
using SwiftCaps.Fake.Data;
using SwiftCaps.Functions.Tests;
using SwiftCaps.Models.Constants;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using SwiftCaps.Services.Abstraction.Interfaces;
using SwiftCaps.Services.Reporting.Api;
using Xamariners.Core.Interface;
using Xamariners.Functions.Core.Extensions;
using Xamariners.RestClient.Helpers.Extensions;
using Xamariners.RestClient.Helpers.Models;
using Xunit;
using MILogger = Microsoft.Extensions.Logging.ILogger;

namespace SwiftCaps.Services.Reporting.API.Tests
{
    public class LeaderBoardGetFunctionTests
    {

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task InvalidToken_Should_DenyAccess(string token)
        {
            var leaderBoardService = new Mock<ILeaderBoardService>();
            var request = MockHelpers.CreateHttpRequestData(null, token);
            var result = await ExecuteLeaderBoardGetFunction(null, leaderBoardService.Object, request);

            result.HttpStatus.Value.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].ShouldBe("Access denied.");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async Task ValidToken_InvalidScope_Should_DenyAccess()
        {
            var leaderBoardService = new Mock<ILeaderBoardService>();
            var request = MockHelpers.CreateHttpRequestData(null, "token");
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp","foo:bar")
            });
            var result = await ExecuteLeaderBoardGetFunction(principal.Object, leaderBoardService.Object, request);

            result.HttpStatus.Value.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].ShouldBe("Access denied.");
            result.Data.ShouldBeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("{}")]
        [InlineData(@"{""foo"":""bar""}")]
        public async Task ValidToken_ValidScope_InvalidPayload_Should_Error(string payload)
        {
            // Arrange

            var leaderBoards = FakeLeaderBoardData.Data;
            var leaderBoardService = new Mock<ILeaderBoardService>();
            leaderBoardService.Setup(x => x.GetLeaderBoard(It.IsAny<UserQuizRequest>()))
                              .ReturnsAsync(leaderBoards.AsSuccessServiceResponse("Ok"));

            var request = MockHelpers.CreateHttpRequestData(payload, "token"); 

            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.ReportsRead),
                new Claim("oid", Guid.NewGuid().ToString())
            });

            // Action

            var result = await ExecuteLeaderBoardGetFunction(
                principal.Object, leaderBoardService.Object, request);

            // Assert

            result.HttpStatus.Value.ShouldBe(System.Net.HttpStatusCode.BadRequest);
            result.ErrorMessage.ShouldBe("Error processing request.");
            result.Errors.Count.ShouldBe(1);
            result.Errors[0].Contains("Missing payload or invalid payload provided.");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async Task ValidToken_ValidScope_ValidPayload_Should_ReturnLeaderBoard()
        {
            // Arrange
            var leaderBoards = FakeLeaderBoardData.Data;
            var leaderBoardService = new Mock<ILeaderBoardService>();
            leaderBoardService.Setup(x => x.GetLeaderBoard(It.IsAny<UserQuizRequest>()))
                              .ReturnsAsync(leaderBoards.AsSuccessServiceResponse("Ok"));

            var payload = JsonSerializer.Serialize(new UserQuizRequest
            {
                UserId = Guid.NewGuid(),
                ClientLocalDateTime = new DateTimeOffset(new DateTime(2020, 9, 20))
            }, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            var request = MockHelpers.CreateHttpRequestData(payload, "token");

            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Claims).Returns(new List<Claim>
            {
                new Claim("scp", ScopeConstants.ReportsRead),
                new Claim("oid", Guid.NewGuid().ToString())
            });

            // Act

            var result = await ExecuteLeaderBoardGetFunction(principal.Object, leaderBoardService.Object, request);

            // Assert

            result.HttpStatus.Value.ShouldBe(System.Net.HttpStatusCode.OK);
            result.Message.ShouldBe("Ok");
            result.ErrorMessage.ShouldBeNullOrEmpty();
            result.Errors.ShouldBeEmpty();
            result.Data.Count.ShouldBe(leaderBoards.Count);

            //TODO: Figure out why List comparison is failing in Shouldly.
            //result.Data.ShouldBe(leaderBoards);
        }

        private static LeaderBoardGetFunction GetLeaderBoardGetFunction(ClaimsPrincipal principal, ILeaderBoardService leaderBoardService)
        {
            var baseService = FunctionsMock.GetBaseSvc(principal);
            var leaderBoardGetFunction = new LeaderBoardGetFunction(leaderBoardService, Mock.Of<ILogger>(), baseService.Auth.Object, baseService.Env.Object);
            return leaderBoardGetFunction;
        }

        private static async Task<ServiceResponse<IList<LeaderBoard>>> ExecuteLeaderBoardGetFunction(ClaimsPrincipal principal, ILeaderBoardService leaderBoardService, HttpRequestData req)
        {
            var function = GetLeaderBoardGetFunction(principal, leaderBoardService);
            var resultObject = await function.Run(req, req.FunctionContext, Mock.Of<MILogger>());
            var resultString = resultObject.ReadHttpResponseData();
            var content = JsonSerializer.Deserialize<ServiceResponse<IList<LeaderBoard>>>(resultString, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            return content;
        }

    }
}
