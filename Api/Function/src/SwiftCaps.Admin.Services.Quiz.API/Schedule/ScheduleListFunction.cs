using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

namespace SwiftCaps.Admin.Services.Quiz.API
{
    public class ScheduleListFunction : FunctionBase
    {
        private readonly IScheduleService _service;

        public ScheduleListFunction(IScheduleService service,
                                    Xamariners.Core.Interface.ILogger logger,
                                    IFunctionAuthorizationService functionAuthorization,
                                    AzureADConfiguration azureAdConfiguration)
            : base(azureAdConfiguration, functionAuthorization)
        {
            _service = service;
            Logger = logger;
        }

        [Function(nameof(ScheduleListFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "schedules")] HttpRequestData req,
            FunctionContext context,
            ILogger log)
        {
            try
            {
                EnsureAuthorization(req, ScopeConstants.QuizRead);
                var schedules = await _service.GetSchedulesAsync();
                return await req.CreateSuccessResponseAsync(schedules);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<IList<ScheduleSummary>>(ex);
            }
        }
    }
}
