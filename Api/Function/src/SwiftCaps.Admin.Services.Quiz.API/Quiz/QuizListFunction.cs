using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SwiftCaps.Models.Constants;
using SwiftCaps.Models.Models;
using SwiftCaps.Services.Abstraction.Interfaces;
using Xamariners.Functions.Core.Configuration;
using Xamariners.Functions.Core.Extensions;
using Xamariners.Functions.Core.Infrastructure;
using Xamariners.Functions.Core.Interfaces;

namespace SwiftCaps.Admin.Services.Quiz.API
{
    public class QuizListFunction : FunctionBase
    {
        private readonly IAdminQuizService _adminQuizService;

        public QuizListFunction(IAdminQuizService adminQuizService, Xamariners.Core.Interface.ILogger logger,
            IFunctionAuthorizationService functionAuthorization,
            AzureADConfiguration azureAdConfiguration) : base(azureAdConfiguration, functionAuthorization)
        {
            _adminQuizService = adminQuizService;
            Logger = logger;
        }

        [Function(nameof(QuizListFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "quiz/list")] HttpRequestData req,
            FunctionContext context,
            ILogger log)
        {
            Logger.LogInfo("Quiz list function processed a request.");

            try
            {
                EnsureAuthorization(req, ScopeConstants.QuizRead);

                var userQuiz = await _adminQuizService.GetQuizzesAsync();

                return await req.CreateSuccessResponseAsync(userQuiz);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<IList<QuizSummary>>(ex);
            }
        }
    }
}
