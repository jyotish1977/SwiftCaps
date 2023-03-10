using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SwiftCaps.Models.Models;
using SwiftCaps.Services.Abstraction.Interfaces;
using Xamariners.Functions.Core.Configuration;
using Xamariners.Functions.Core.Infrastructure;
using Xamariners.Functions.Core.Interfaces;
using Xamariners.RestClient.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using Xamariners.Functions.Core.Extensions;
using SwiftCaps.Models.Constants;
using Ardalis.GuardClauses;
using SwiftCaps.Services.Quiz.Extensions;

namespace SwiftCaps.Services.Quiz.Api
{
    public class UserQuizSetupFunction : FunctionBase
    {
        private readonly IQuizService _quizService;

        public UserQuizSetupFunction(
            IQuizService quizService, 
            Xamariners.Core.Interface.ILogger logger, 
            IFunctionAuthorizationService functionAuthorization,
            AzureADConfiguration azureAdConfiguration) 
            : base(azureAdConfiguration, functionAuthorization)
        {
            _quizService = quizService;
            Logger = logger;
        }

        [Function("AddUserQuizFunction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "userquiz/adduserquiz")] HttpRequestData req,
            FunctionContext context, 
            ILogger log)
        {
            Logger.LogInfo("Quiz add function processed a request.");

            try
            {
                EnsureAuthorization(req, ScopeConstants.UserQuizCUD);

                var userQuizRequest = await EnsurePayload(req);
                var userQuiz = await _quizService.AddUserQuiz(userQuizRequest);

                return await req.CreateSuccessResponseAsync(userQuiz.Data, returnOkForPost: true);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<UserQuiz>(ex); 
            }
        }

        private static async Task<UserQuiz> EnsurePayload(HttpRequestData req)
        {
            var userQuizRequest = await req.DeserializePayloadAsync<UserQuiz>();
            Guard.Against.InvalidQuizSetupPayload(userQuizRequest);
            return userQuizRequest;
        }
    }
}
