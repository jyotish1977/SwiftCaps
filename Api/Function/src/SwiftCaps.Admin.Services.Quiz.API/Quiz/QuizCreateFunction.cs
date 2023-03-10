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
    public class QuizCreateFunction : FunctionBase
    {
        private readonly IAdminQuizService _adminQuizService;

        public QuizCreateFunction(
            IAdminQuizService adminQuizService, 
            Xamariners.Core.Interface.ILogger logger,
            IFunctionAuthorizationService functionAuthorization,
            AzureADConfiguration azureAdConfiguration) 
            : base(azureAdConfiguration, functionAuthorization)
        {
            _adminQuizService = adminQuizService;
            Logger = logger;
        }

        [Function(nameof(QuizCreateFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "quiz")] HttpRequestData req,
            FunctionContext context,
            ILogger log)
        {
            Logger.LogInfo("Quiz create function processed a request.");

            try
            {
                var userPrincipal = EnsureAuthorization(req, ScopeConstants.QuizCUD);

                var quiz = await EnsurePayload(req);
                quiz.CreatedBy = ParseClaim<Guid>(userPrincipal, "oid");
                var newQuizId = await _adminQuizService.CreateQuizAsync(quiz);
                return await req.CreateSuccessResponseAsync(newQuizId);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<Guid?>(ex);
            }
        }

        private static async Task<SCModels.Quiz> EnsurePayload(HttpRequestData req)
        {
            var quiz = await req.DeserializePayloadAsync<SCModels.Quiz>();
            Guard.Against.InvalidQuizCreatePayload(quiz);
            return quiz;
        }
    }
}
