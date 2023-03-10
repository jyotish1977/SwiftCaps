using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SwiftCaps.Admin.Services.Quiz.Extensions;
using SwiftCaps.Models.Constants;
using SwiftCaps.Services.Abstraction.Interfaces;
using Xamariners.Functions.Core.Configuration;
using Xamariners.Functions.Core.Extensions;
using Xamariners.Functions.Core.Infrastructure;
using Xamariners.Functions.Core.Interfaces;

namespace SwiftCaps.Admin.Services.Quiz.API
{
    class QuizDeleteFunction : FunctionBase
    {
        private readonly IAdminQuizService _adminQuizService;

        public QuizDeleteFunction(
            IAdminQuizService adminQuizService,
            Xamariners.Core.Interface.ILogger logger,
            IFunctionAuthorizationService functionAuthorization,
            AzureADConfiguration azureAdConfiguration)
            : base(azureAdConfiguration, functionAuthorization)
        {
            _adminQuizService = adminQuizService;
            Logger = logger;
        }

        [Function(nameof(QuizDeleteFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "quiz/{quizId}")] HttpRequestData req,
            string quizId, 
            FunctionContext context,
            ILogger log)
        {
            Logger.LogInfo("Quiz delete function processed a request.");

            try
            {
                EnsureAuthorization(req, ScopeConstants.QuizCUD);

                var quizIdValue = EnsurePayload(quizId);
                var deleteSuccess = await _adminQuizService.DeleteQuizAsync(quizIdValue);
                return await req.CreateSuccessResponseAsync(deleteSuccess);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<bool>(ex);
            }
        }

        private static Guid EnsurePayload(string quizId)
        {
            _ = Guid.TryParse(quizId, out var quizIdValue);
            Guard.Against.InvalidQuizReadPayload(quizIdValue);
            return quizIdValue;
        }
    }
}
