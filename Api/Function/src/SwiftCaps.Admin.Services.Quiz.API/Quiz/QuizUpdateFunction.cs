using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SwiftCaps.Models.Constants;
using SwiftCaps.Services.Abstraction.Interfaces;
using Xamariners.Functions.Core.Configuration;
using Xamariners.Functions.Core.Extensions;
using Xamariners.Functions.Core.Infrastructure;
using Xamariners.Functions.Core.Interfaces;
using SCModels = SwiftCaps.Models.Models;
using SwiftCaps.Admin.Services.Quiz.Extensions;


namespace SwiftCaps.Admin.Services.Quiz.API
{
    public class QuizUpdateFunction : FunctionBase
    {
        private readonly IAdminQuizService _adminQuizService;

        public QuizUpdateFunction(
            IAdminQuizService adminQuizService,
            Xamariners.Core.Interface.ILogger logger,
            IFunctionAuthorizationService functionAuthorization,
            AzureADConfiguration azureAdConfiguration)
            : base(azureAdConfiguration, functionAuthorization)
        {
            _adminQuizService = adminQuizService;
            Logger = logger;
        }

        [Function(nameof(QuizUpdateFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "quiz/{quizId}")] HttpRequestData req,
            string quizId,
            FunctionContext context,
            ILogger log)
        {
            Logger.LogInfo("Quiz update function processed a request.");

            try
            {
                EnsureAuthorization(req, ScopeConstants.QuizCUD);

                var quiz = await req.DeserializePayloadAsync<SCModels.Quiz>();
                var quizIdValue = EnsurePayload(quizId, quiz);
                var updatedQuizId = await _adminQuizService.UpdateQuizAsync(quizIdValue, quiz);
                
                return await req.CreateSuccessResponseAsync(updatedQuizId);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<Guid?>(ex);
            }
        }

        private static Guid EnsurePayload(string quizId, SCModels.Quiz quiz)
        {
            Guard.Against.NullOrWhiteSpace(quizId,"quiz id", "Missing payload or invalid payload provided.");
            _ = Guid.TryParse(quizId, out var quizIdValue);
            Guard.Against.InvalidQuizUpdatePayload(quizIdValue, quiz);
            return quizIdValue;
        }
    }
}
