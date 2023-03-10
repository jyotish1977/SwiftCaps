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
using System.Web;

namespace SwiftCaps.Admin.Services.Quiz.API
{
    public class ScheduleGroupSearchFunction : FunctionBase
    {
        private readonly IScheduleGroupService _service;

        public ScheduleGroupSearchFunction(IScheduleGroupService service,
                                    Xamariners.Core.Interface.ILogger logger,
                                    IFunctionAuthorizationService functionAuthorization,
                                    AzureADConfiguration azureAdConfiguration)
            : base(azureAdConfiguration, functionAuthorization)
        {
            _service = service;
            Logger = logger;
        }

        [Function(nameof(ScheduleGroupSearchFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "schedules/{schedule}/groups/search")] HttpRequestData req,
            string schedule,
            FunctionContext context,
            ILogger log)
        {
            try
            {
                EnsureAuthorization(req, ScopeConstants.QuizCUD);
                
                var queryStrings = HttpUtility.ParseQueryString(req.Url.Query);
                var searchString = queryStrings["q"];
                var scheduleId = Guid.Parse(schedule);

                var groups = await _service.SearchGroupsAsync(scheduleId, searchString);

                return await req.CreateSuccessResponseAsync(groups);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<IList<ScheduleGroupSummary>>(ex);
            }
        }
    }
}
