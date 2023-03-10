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
    public class QuizQuestionCreateFunction : FunctionBase
    {
        private readonly IAdminQuizQuestionService _service;

        public QuizQuestionCreateFunction(IAdminQuizQuestionService questionService,
                                       Xamariners.Core.Interface.ILogger logger,
                                       IFunctionAuthorizationService functionAuthorization,
                                       AzureADConfiguration azureAdConfiguration) 
            : base(azureAdConfiguration, functionAuthorization)
        {
            _service = questionService;
            Logger = logger;
        }

        [Function(nameof(QuizQuestionCreateFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "quiz/question")] HttpRequestData req,
            FunctionContext context,
            ILogger log)
        {
            Logger.LogInfo("Quiz question create function processed a request.");

            try
            {
                EnsureAuthorization(req, ScopeConstants.QuizRead);

                var payload = await req.DeserializePayloadAsync<Question>();
                EnsurePayload(payload);

                var result = await _service.CreateQuestionAsync(payload);

                return await req.CreateSuccessResponseAsync(result);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<Guid?>(ex);
            }
        }

        private void EnsurePayload(Question payload)
        {
            Guard.Against.InvalidQuestionCreatePayload(payload);
        }
    }
}
