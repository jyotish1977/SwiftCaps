using System;
using System.Linq;
using System.Threading.Tasks;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Data.Context;
using Xamariners.RestClient.Helpers.Extensions;
using Xamariners.RestClient.Helpers.Models;
using Xamariners.RestClient.Interfaces;

namespace SwiftCaps.Fake.Services
{
    public class AdAuthService : IAdAuthService
    {
        private readonly SwiftCapsContext _swiftCapsContext;

        public AdAuthService(IAppSettings appSettings, SwiftCapsContext swiftCapsContext)
        {
            _swiftCapsContext = swiftCapsContext;
        }

        public async Task<ServiceResponse<bool>> Unauthenticate()
        {
            return true.AsServiceResponse();
        }

        public async Task<AuthToken> GetTokenFromCache()
        {
            return new AuthToken();
        }

        public async Task<ServiceResponse<(AuthToken authToken, Guid UniqueId, string Username)>> Authenticate(object parent)
        {
            var user = _swiftCapsContext.Users.FirstOrDefault();

            return (new AuthToken(), user.Id, user.Email).AsServiceResponse();
        }
    }
}
