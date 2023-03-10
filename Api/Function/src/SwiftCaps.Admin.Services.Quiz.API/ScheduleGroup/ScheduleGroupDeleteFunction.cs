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
    public class ScheduleGroupDeleteFunction : FunctionBase
    {
        private readonly IScheduleGroupService _service;

        public ScheduleGroupDeleteFunction(IScheduleGroupService service,
                                           Xamariners.Core.Interface.ILogger logger,
                                           IFunctionAuthorizationService functionAuthorization,
                                           AzureADConfiguration azureAdConfiguration)
            : base(azureAdConfiguration, functionAuthorization)
        {
            _service = service;
            Logger = logger;
        }

        [Function(nameof(ScheduleGroupDeleteFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "schedules/{schedule}/groups/{group}")] HttpRequestData req,
            string schedule,
            string group,
            FunctionContext context,
            ILogger log)
        {
            try
            {
                EnsureAuthorization(req, ScopeConstants.QuizCUD);

                var (scheduleId, groupId) = EnsurePayload(schedule, group);
                
                var result = await _service.DeleteGroupAsync(scheduleId, groupId);
                return await req.CreateSuccessResponseAsync(result);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<bool>(ex);
            }
        }

        private (Guid scheduleId, Guid groupId) EnsurePayload(string schedule, string group)
        {
            _ = Guid.TryParse(schedule, out var scheduleId);
            _ = Guid.TryParse(group, out var groupId);
            Guard.Against.InvalidScheduleGroupDeletePayload(scheduleId, groupId);
            return (scheduleId,groupId);
        }
    }
}
