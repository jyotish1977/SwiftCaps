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
    public class QuizSectionDeleteFunction : FunctionBase
    {
        private readonly IAdminQuizSectionService _service;

        public QuizSectionDeleteFunction(IAdminQuizSectionService quizSectionService,
                                       Xamariners.Core.Interface.ILogger logger,
                                       IFunctionAuthorizationService functionAuthorization,
                                       AzureADConfiguration azureAdConfiguration) 
            : base(azureAdConfiguration, functionAuthorization)
        {
            _service = quizSectionService;
            Logger = logger;
        }

        [Function(nameof(QuizSectionDeleteFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "quiz/section/{section}")] HttpRequestData req,
            string section,
            FunctionContext context,
            ILogger log)
        {
            Logger.LogInfo("Quiz list function processed a request.");

            try
            {
                EnsureAuthorization(req, ScopeConstants.QuizRead);

                var sectionIdToDelete = EnsurePayload(section);

                var result = await _service.DeleteSectionAsync(sectionIdToDelete);

                return await req.CreateSuccessResponseAsync(result);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<bool>(ex);
            }
        }

        private Guid EnsurePayload(string id)
        {
            _ = Guid.TryParse(id, out var sectionId);
            Guard.Against.InvalidSectionDeletePayload(sectionId);
            return sectionId;
        }
    }
}
