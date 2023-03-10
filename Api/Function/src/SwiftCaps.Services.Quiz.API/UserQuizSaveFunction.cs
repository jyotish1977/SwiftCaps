using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SwiftCaps.Models.Constants;
using SwiftCaps.Models.Models;
using SwiftCaps.Services.Abstraction.Interfaces;
using SwiftCaps.Services.Quiz.Extensions;
using Xamariners.Functions.Core.Configuration;
using Xamariners.Functions.Core.Extensions;
using Xamariners.Functions.Core.Infrastructure;
using Xamariners.Functions.Core.Interfaces;

namespace SwiftCaps.Services.Quiz.Api
{
    public class UserQuizSaveFunction : FunctionBase
    {
        private readonly IQuizService _quizService;

        public UserQuizSaveFunction(
            IQuizService quizService, 
            Xamariners.Core.Interface.ILogger logger,
            IFunctionAuthorizationService functionAuthorization,
            AzureADConfiguration azureAdConfiguration) 
            : base(azureAdConfiguration, functionAuthorization)
        {
            _quizService = quizService;
            Logger = logger;
        }

        [Function("SaveUserQuizFunction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "userquiz/saveuserquiz")] HttpRequestData req,
            FunctionContext context, 
            ILogger log)
        {
            Logger.LogInfo("Quiz save function processed a request.");

            try
            {
                var userPrincipal  = EnsureAuthorization(req, ScopeConstants.UserQuizCUD);
                
                var userQuizRequest = await req.DeserializePayloadAsync<UserQuiz>();
                EnsurePayload(userQuizRequest);
                userQuizRequest.UserId = ParseClaim<Guid>(userPrincipal, "oid");
                var result = await _quizService.SaveUserQuiz(userQuizRequest);

                return await req.CreateSuccessResponseAsync(result.Data);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<bool>(ex);
            }
        }

        private bool EnsurePayload(UserQuiz payload)
        {
            Guard.Against.InvalidQuizSavePayload(payload);
            return true;
        }
    }
}
