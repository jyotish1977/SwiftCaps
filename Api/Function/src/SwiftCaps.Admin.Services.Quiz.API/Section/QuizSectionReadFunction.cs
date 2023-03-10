using System;
using System.Collections.Generic;
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
    public class QuizSectionReadFunction : FunctionBase
    {
        private readonly IAdminQuizSectionService _service;

        public QuizSectionReadFunction(IAdminQuizSectionService quizSectionService,
                                       Xamariners.Core.Interface.ILogger logger,
                                       IFunctionAuthorizationService functionAuthorization,
                                       AzureADConfiguration azureAdConfiguration) 
            : base(azureAdConfiguration, functionAuthorization)
        {
            _service = quizSectionService;
            Logger = logger;
        }

        [Function(nameof(QuizSectionReadFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "quiz/section/{section}")] HttpRequestData req,
            string section,
            FunctionContext context,
            ILogger log)
        {
            Logger.LogInfo("Quiz list function processed a request.");

            try
            {
                EnsureAuthorization(req, ScopeConstants.QuizRead);
                var sectionId = EnsurePayload(section);
                var result = await _service.GetSectionAsync(sectionId);

                return await req.CreateSuccessResponseAsync(result);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<QuizSection>(ex);
            }
        }

        private Guid EnsurePayload(string payload)
        {
            _ = Guid.TryParse(payload, out var quizId);
            Guard.Against.InvalidSectionReadPayload(quizId);
            return quizId;
        }
    }
}
