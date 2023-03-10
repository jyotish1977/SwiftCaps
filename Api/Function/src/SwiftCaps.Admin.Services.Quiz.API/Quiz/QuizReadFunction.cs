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
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.API
{
    public class QuizReadFunction : FunctionBase
    {
        private readonly IAdminQuizService _adminQuizService;

        public QuizReadFunction(
            IAdminQuizService adminQuizService, 
            Xamariners.Core.Interface.ILogger logger,
            IFunctionAuthorizationService functionAuthorization,
            AzureADConfiguration azureAdConfiguration) 
            : base(azureAdConfiguration, functionAuthorization)
        {
            _adminQuizService = adminQuizService;
            Logger = logger;
        }

        [Function(nameof(QuizReadFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "quiz/{quizId}")] HttpRequestData req, 
            string quizId,
            FunctionContext context,
            ILogger log)
        {
            Logger.LogInfo("Quiz read function processed a request.");

            try
            {
                EnsureAuthorization(req, ScopeConstants.QuizRead);

                var quizIdValue = EnsurePayload(quizId);
                var userQuiz = await _adminQuizService.GetQuizAsync(quizIdValue);

                return await req.CreateSuccessResponseAsync(userQuiz);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<SCModels.Quiz>(ex);
            }
        }

        private static Guid EnsurePayload(string quizId)
        {
            Guard.Against.NullOrWhiteSpace(quizId,"quiz id","Missing payload or invalid payload provided.");
            _ = Guid.TryParse(quizId, out var quizIdValue);
            Guard.Against.InvalidQuizReadPayload(quizIdValue);
            return quizIdValue;
        }
    }
}
