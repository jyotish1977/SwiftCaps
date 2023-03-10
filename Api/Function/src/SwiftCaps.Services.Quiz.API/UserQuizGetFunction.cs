using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SwiftCaps.Models.Constants;
using SwiftCaps.Models.Requests;
using SwiftCaps.Services.Abstraction.Interfaces;
using SwiftCaps.Services.Quiz.Extensions;
using Xamariners.Functions.Core.Configuration;
using Xamariners.Functions.Core.Extensions;
using Xamariners.Functions.Core.Infrastructure;
using Xamariners.Functions.Core.Interfaces;
using Xamariners.RestClient.Helpers;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Services.Quiz.Api
{
    public class UserQuizGetFunction : FunctionBase
    {
        private readonly IQuizService _quizService;
     
        public UserQuizGetFunction(
            IQuizService quizService, 
            Xamariners.Core.Interface.ILogger logger, 
            IFunctionAuthorizationService functionAuthorization,
            AzureADConfiguration azureAdConfiguration) 
            : base(azureAdConfiguration, functionAuthorization)
        {
            _quizService = quizService;
            Logger = logger;
        }

        [Function("GetAvailableUserQuizzesFunction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "userquiz/getavailableuserquizzes")] HttpRequestData req,
            FunctionContext context, 
            ILogger log)
        {
            Logger.LogInfo("Quiz get function processed a request.");

            try
            {
                var userPrincipal = EnsureAuthorization(req, ScopeConstants.UserQuizRead);
                
                var quizRequest = await EnsurePayload(req);
                quizRequest.UserId = ParseClaim<Guid>(userPrincipal, "oid");
                var userQuiz = await _quizService.GetAvailableUserQuizzes(quizRequest);
                return await req.CreateSuccessResponseAsync(userQuiz.Data, returnOkForPost: true);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<IList<SCModels.UserQuiz>>(ex);
            }
        }

        private static async Task<UserQuizRequest> EnsurePayload(HttpRequestData req)
        {
            var quizRequest = await req.DeserializePayloadAsync<UserQuizRequest>();
            Guard.Against.InvalidQuizListPayload(quizRequest, false);
            return quizRequest;
        }
    }
}
