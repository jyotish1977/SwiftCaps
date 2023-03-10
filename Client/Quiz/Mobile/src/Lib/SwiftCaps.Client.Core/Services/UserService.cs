using System;
using System.Threading.Tasks;
using SwiftCaps.Client.Core.Services.Infrastructure;
using SwiftCaps.Models.Models;
using SwiftCaps.Services.Abstraction.Interfaces;
using Xamariners.RestClient.Helpers.Models;
using Xamariners.RestClient.Infrastructure;

namespace SwiftCaps.Client.Core.Services
{
    public class UserService : ServiceBase, IUserService
    {
        public async Task<ServiceResponse<User>> GetOrCreateUser(Guid userId, string accessToken = null)
        {
            var response = await RestClient
                .ExecuteAsync<User, Guid>(
                    HttpVerb.GET,
                    nameof(GetOrCreateUser),
                    isServiceResponse: true,
                    apiRoutePrefix: nameof(User),
                    headers: AppSettings.Headers).ConfigureAwait(false);

            return response;
        }
    }
}
