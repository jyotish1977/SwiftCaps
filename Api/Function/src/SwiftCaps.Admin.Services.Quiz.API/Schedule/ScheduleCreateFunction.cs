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
    public class ScheduleCreateFunction : FunctionBase
    {
        private readonly IScheduleService _service;

        public ScheduleCreateFunction(IScheduleService service,
                                       Xamariners.Core.Interface.ILogger logger,
                                       IFunctionAuthorizationService functionAuthorization,
                                       AzureADConfiguration azureAdConfiguration) 
            : base(azureAdConfiguration, functionAuthorization)
        {
            _service = service;
            Logger = logger;
        }

        [Function(nameof(ScheduleCreateFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "schedules")] HttpRequestData req,
            FunctionContext context,
            ILogger log)
        {
            try
            {
                EnsureAuthorization(req, ScopeConstants.QuizCUD);

                var payload = await req.DeserializePayloadAsync<Schedule>();
                EnsurePayload(payload);

                var result = await _service.CreateScheduleAsync(payload);

                return await req.CreateSuccessResponseAsync(result);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<Guid?>(ex);
            }
        }

        private void EnsurePayload(Schedule payload)
        {
            Guard.Against.InvalidScheduleCreatePayload(payload);
        }
    }
}
