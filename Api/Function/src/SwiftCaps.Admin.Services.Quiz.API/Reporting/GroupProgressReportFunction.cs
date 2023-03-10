using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SwiftCaps.Models.Constants;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using SwiftCaps.Services.Abstraction.Interfaces;
using Xamariners.Functions.Core.Configuration;
using Xamariners.Functions.Core.Extensions;
using Xamariners.Functions.Core.Infrastructure;
using Xamariners.Functions.Core.Interfaces;

namespace SwiftCaps.Admin.Services.Quiz.API
{
    public class GroupProgressReportFunction : FunctionBase
    {
        private readonly IAdminReportingService _service;

        public GroupProgressReportFunction(IAdminReportingService service,
                                    Xamariners.Core.Interface.ILogger logger,
                                    IFunctionAuthorizationService functionAuthorization,
                                    AzureADConfiguration azureAdConfiguration)
            : base(azureAdConfiguration, functionAuthorization)
        {
            _service = service;
            Logger = logger;
        }

        [Function(nameof(GroupProgressReportFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reporting/groupprogress")] HttpRequestData req,
            FunctionContext context,
            ILogger log)
        {
            try
            {
                EnsureAuthorization(req, ScopeConstants.QuizCUD);
                var request = await req.DeserializePayloadAsync<AdminReportingRequest>();
                var reportItems = await _service.GetGroupProgressReport(request);
                return await req.CreateSuccessResponseAsync(reportItems, true);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing request.");
                return await req.CreateErrorResponseAsync<IList<GroupProgressReportItem>>(ex);
            }
        }
    }
}
