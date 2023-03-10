using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
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
using SwiftCaps.Admin.Services.Quiz.Extensions;

namespace SwiftCaps.Admin.Services.Quiz.API
{
    public class ScheduleGroupCreateFunction : FunctionBase
    {
        private readonly IScheduleGroupService _service;

        public ScheduleGroupCreateFunction(IScheduleGroupService service,
                                    Xamariners.Core.Interface.ILogger logger,
                                    IFunctionAuthorizationService functionAuthorization,
                                    AzureADConfiguration azureAdConfiguration)
            : base(azureAdConfiguration, functionAuthorization)
        {
            _service = service;
            Logger = logger;
        }

        [Function(nameof(ScheduleGroupCreateFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "schedules/{schedule}/groups")] HttpRequestData req,
            string schedule,
            FunctionContext context,
            ILogger log)
        {
            try
            {
                EnsureAuthorization(req, ScopeConstants.QuizCUD);

                var payload = await req.DeserializePayloadAsync<Group>();
                var scheduleId = EnsurePayload(schedule, payload);
                
                var scheduleGroupId = await _service.CreateGroupAsync(scheduleId, payload);
                return await req.CreateSuccessResponseAsync(scheduleGroupId);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<Guid?>(ex);
            }
        }

        private Guid EnsurePayload(string id, Group payload)
        {
            _ = Guid.TryParse(id, out var payloadId);
            Guard.Against.InvalidScheduleGroupCreatePayload(payloadId, payload);
            return payloadId;
        }
    }
}
