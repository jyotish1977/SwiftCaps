using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SwiftCaps.Admin.Services.Quiz.Extensions;
using SwiftCaps.Models.Constants;
using SwiftCaps.Models.Models;
using SwiftCaps.Services.Abstraction.Interfaces;
using Xamariners.Functions.Core.Configuration;
using Xamariners.Functions.Core.Extensions;
using Xamariners.Functions.Core.Infrastructure;
using Xamariners.Functions.Core.Interfaces;

namespace SwiftCaps.Admin.Services.Quiz.API
{
    public class QuizQuestionUpdateFunction : FunctionBase
    {
        private readonly IAdminQuizQuestionService _service;

        public QuizQuestionUpdateFunction(IAdminQuizQuestionService questionService,
                                       Xamariners.Core.Interface.ILogger logger,
                                       IFunctionAuthorizationService functionAuthorization,
                                       AzureADConfiguration azureAdConfiguration) 
            : base(azureAdConfiguration, functionAuthorization)
        {
            _service = questionService;
            Logger = logger;
        }

        [Function(nameof(QuizQuestionUpdateFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "quiz/question/{questionId}")] HttpRequestData req,
            string questionId,
            FunctionContext context,
            ILogger log)
        {
            Logger.LogInfo("Quiz question update function processed a request.");

            try
            {
                EnsureAuthorization(req, ScopeConstants.QuizRead);

                var payload = await req.DeserializePayloadAsync<Question>();
                var questionIdValue = EnsurePayload(questionId, payload);

                var result = await _service.UpdateQuestionAsync(questionIdValue, payload);

                return await req.CreateSuccessResponseAsync(result);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<Guid?>(ex);
            }
        }

        private Guid EnsurePayload(string questionId, Question payload)
        {
            _ = Guid.TryParse(questionId, out var questionIdValue);
            Guard.Against.InvalidQuestionUpdatePayload(questionIdValue, payload);
            return questionIdValue;
        }
    }
}
