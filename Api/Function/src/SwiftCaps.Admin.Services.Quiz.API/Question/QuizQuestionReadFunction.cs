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
    public class QuizQuestionReadFunction : FunctionBase
    {
        private readonly IAdminQuizQuestionService _service;

        public QuizQuestionReadFunction(
            IAdminQuizQuestionService questionService, 
            Xamariners.Core.Interface.ILogger logger,
            IFunctionAuthorizationService functionAuthorization,
            AzureADConfiguration azureAdConfiguration) 
            : base(azureAdConfiguration, functionAuthorization)
        {
            _service = questionService;
            Logger = logger;
        }

        [Function(nameof(QuizQuestionReadFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "quiz/question/{questionId}")] HttpRequestData req, 
            string questionId,
            FunctionContext context,
            ILogger log)
        {
            Logger.LogInfo("Quiz read function processed a request.");

            try
            {
                EnsureAuthorization(req, ScopeConstants.QuizRead);

                var quizIdValue = EnsurePayload(questionId);
                var question = await _service.GetQuestionAsync(quizIdValue);

                return await req.CreateSuccessResponseAsync(question);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<SCModels.Question>(ex);
            }
        }

        private static Guid EnsurePayload(string questionId)
        {
            _ = Guid.TryParse(questionId, out var questionIdValue);
            Guard.Against.InvalidQuestionReadPayload(questionIdValue);
            return questionIdValue;
        }
    }
}
