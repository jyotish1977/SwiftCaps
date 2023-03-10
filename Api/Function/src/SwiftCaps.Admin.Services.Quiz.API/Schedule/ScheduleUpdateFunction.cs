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
    public class ScheduleUpdateFunction : FunctionBase
    {
        private readonly IScheduleService _service;

        public ScheduleUpdateFunction(IScheduleService service,
                                       Xamariners.Core.Interface.ILogger logger,
                                       IFunctionAuthorizationService functionAuthorization,
                                       AzureADConfiguration azureAdConfiguration) 
            : base(azureAdConfiguration, functionAuthorization)
        {
            _service = service;
            Logger = logger;
        }

        [Function(nameof(ScheduleUpdateFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "schedules/{schedule}")] HttpRequestData req,
            string schedule,
            FunctionContext context,
            ILogger log)
        {
            try
            {
                EnsureAuthorization(req, ScopeConstants.QuizCUD);

                var payload = await req.DeserializePayloadAsync<Schedule>();
                var sectionIdToUpdate = EnsurePayload(schedule, payload);

                var result = await _service.UpdateScheduleAsync(sectionIdToUpdate, payload);

                return await req.CreateSuccessResponseAsync(result);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<Guid?>(ex);
            }
        }

        private Guid EnsurePayload(string id, Schedule payload)
        {
            _ = Guid.TryParse(id, out var sectionId);
            Guard.Against.InvalidScheduleUpdatePayload(sectionId, payload);
            return sectionId;
        }
    }
}
