using System.Threading.Tasks;
using SwiftCaps.Client.Core.Helpers;
using SwiftCaps.Client.Core.Services.Infrastructure;
using SwiftCaps.Client.Core.Services.Interfaces;
using SwiftCaps.Models.Models;
using SwiftCaps.Services.Abstraction.Interfaces;
using Xamariners.RestClient.Helpers.Extensions;
using Xamariners.RestClient.Helpers.Infrastructure;
using Xamariners.RestClient.Helpers.Models;
using Xamariners.RestClient.Interfaces;

namespace SwiftCaps.Client.Core.Services
{
    public class AuthService : ServiceBase, IAuthService
    {
        private readonly IUserService _userService;
        private readonly IAdAuthService _adAuthService;
        private readonly ITokenClient _tokenClient;

        public AuthService(IUserService userService, IAdAuthService adAuthService, ITokenClient tokenClient)
        {
            _userService = userService;
            _adAuthService = adAuthService;
            _tokenClient = tokenClient;
        }

        public async Task<ServiceResponse<bool>> Login()
        {
            // get token
            var resultAuth = await _adAuthService.Authenticate(CrossCurrentActivity.Current).ConfigureAwait(false);

            if (resultAuth == null)
                return false.AsServiceResponse();

            // Crucial part : tie up with rest client
            _tokenClient.SetCurrentAuthToken(resultAuth.Data.authToken);

            var resultUser = await _userService.GetOrCreateUser(resultAuth.Data.UniqueId).ConfigureAwait(false);

            if (!resultUser.IsOK() || resultUser.Data == null)
            {
                await _adAuthService.Unauthenticate().ConfigureAwait(false);
                return resultUser.ToServiceResponse<User, bool>();
            }

            // TODO: Temp until we go the good stuff
            AppCache.State.Member = resultUser.Data;

            AppCache.State.Credential.AuthToken = resultAuth.Data.authToken;
            AppCache.State.Credential.Username = resultAuth.Data.Username;
            AppCache.State.Credential.UserID = resultAuth.Data.UniqueId.ToString();

            await AppCache.Save().ConfigureAwait(false);

            return true.AsServiceResponse();
        }

        public async Task<ServiceResponse<bool>> Logout()
        {
            if (AppCache.State.IsAuthenticated)
            {
                await _adAuthService.Unauthenticate().ConfigureAwait(false);

                _tokenClient.SetCurrentAuthToken(null);

                await AppCache.Clear().ConfigureAwait(false);

            }

            return new ServiceResponse<bool>(ServiceStatus.Success, data: true);
        }
    }
}
