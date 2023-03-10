using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SwiftCaps.Models.Constants;
using SwiftCaps.Services.Abstraction.Interfaces;
using Xamariners.Functions.Core.Configuration;
using Xamariners.Functions.Core.Extensions;
using Xamariners.Functions.Core.Infrastructure;
using Xamariners.Functions.Core.Interfaces;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Services.User.API
{
    public class UserGetFunction : FunctionBase
    {
        private readonly IUserService _userService;

        public UserGetFunction(IUserService userService, 
            Xamariners.Core.Interface.ILogger logger,
            IFunctionAuthorizationService functionAuthorization,
            AzureADConfiguration azureAdConfiguration) 
            : base(azureAdConfiguration, functionAuthorization)
        {
            Logger = logger;
            _userService = userService;
        }

        [Function(nameof(UserGetFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/getorcreateuser")] HttpRequestData req,
            FunctionContext context,
            ILogger log)
        {
            Logger.LogInfo("UserGet function processed a request.");

            try
            {
                var principal = EnsureAuthorization(req, ScopeConstants.UserRead);
                
                var aadUserId = ParseClaim<Guid>(principal,"oid");
                var userResponse = await _userService.GetOrCreateUser(aadUserId, AccessToken);
                if(userResponse.HttpStatus == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new UnauthorizedAccessException(userResponse.ErrorMessage);
                }
                return await req.CreateSuccessResponseAsync(userResponse.Data);
                
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error processing user retrieval.");
                return await req.CreateErrorResponseAsync<SCModels.User>(ex);
            }
        }
    }
}
