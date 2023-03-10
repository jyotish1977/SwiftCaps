using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SwiftCaps.Models.Constants;
using SwiftCaps.Models.Requests;
using SwiftCaps.Services.Abstraction.Interfaces;
using SwiftCaps.Services.Reporting.Extensions;
using Xamariners.Functions.Core.Configuration;
using Xamariners.Functions.Core.Extensions;
using Xamariners.Functions.Core.Infrastructure;
using Xamariners.Functions.Core.Interfaces;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Services.Reporting.Api
{

    public class LeaderBoardGetFunction : FunctionBase
    {
        private readonly ILeaderBoardService _leaderBoardService;

        public LeaderBoardGetFunction(
            ILeaderBoardService leaderBoardService, 
            Xamariners.Core.Interface.ILogger logger, 
            IFunctionAuthorizationService functionAuthorization,
            AzureADConfiguration azureAdConfiguration) 
            : base(azureAdConfiguration, functionAuthorization)
        {
            _leaderBoardService = leaderBoardService;
            Logger = logger;
        }

        [Function("LeaderBoardGetFunction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reporting/getleaderboard")] HttpRequestData req,
            FunctionContext context, 
            ILogger log)
        {
            Logger.LogInfo("LeaderBoardGetFunction processed a request.");

            try
            {
                var userPrincipal = EnsureAuthorization(req, ScopeConstants.ReportsRead);

                var quizRequest = await req.DeserializePayloadAsync<UserQuizRequest>();
                EnsurePayload(quizRequest);

                quizRequest.UserId = ParseClaim<Guid>(userPrincipal, "oid");
                var leaderBoard = await _leaderBoardService.GetLeaderBoard(quizRequest);

                return await req.CreateSuccessResponseAsync(leaderBoard.Data, returnOkForPost: true);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return await req.CreateErrorResponseAsync<IList<SCModels.LeaderBoard>>(ex);
            }
        }

        private static void EnsurePayload(UserQuizRequest quizRequest)
        {
            Guard.Against.InvalidLeaderBoardReadPayload(quizRequest, false);
        }
    }
}
