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
    public class ScheduleGroupListFunction : FunctionBase
    {
        private readonly IScheduleGroupService _service;

        public ScheduleGroupListFunction(IScheduleGroupService service,
                                    Xamariners.Core.Interface.ILogger logger,
                                    IFunctionAuthorizationService functionAuthorization,
                                    AzureADConfiguration azureAdConfiguration)
            : base(azureAdConfiguration, functionAuthorization)
        {
            _service = service;
            Logger = logger;
        }

        [Function(nameof(ScheduleGroupListFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "schedules/{schedule}/groups")] HttpRequestData req,
            string schedule,
            FunctionContext context,
            ILogger log)
        {
            try
            {
                EnsureAuthorization(req, ScopeConstants.QuizCUD);
                var scheduleId = EnsurePayload(schedule);
                var schedules = await _service.GetGroupsAsync(scheduleId);
                return await req.CreateSuccessResponseAsync(schedules);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<IList<ScheduleGroupSummary>>(ex);
            }
        }

        private Guid EnsurePayload(string id)
        {
            _ = Guid.TryParse(id, out var payloadId);
            Guard.Against.InvalidScheduleGroupListPayload(payloadId);
            return payloadId;
        }
    }
}
