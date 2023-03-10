using System;
using System.Threading.Tasks;
using SwiftCaps.Client.Core.Enums;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Services.Infrastructure;
using SwiftCaps.Mobile.Shared.Helpers;
using Xamariners.Mobile.Core.Exceptions;
using Xamariners.RestClient.Interfaces;

namespace SwiftCaps.Mobile.Shared.Services
{
    public class AppCacheService : IAppCacheService<ClientState>
    {
        private readonly ITokenClient _tokenClient;
        private delegate void RestoreCurrentState();

        public AppCacheService(ITokenClient tokenClient)
        {
            _tokenClient = tokenClient;

            new RestoreCurrentState(async () => await Restore().ConfigureAwait(false)).Invoke();
        }

        /// <summary>
        /// Gets the client state from cache
        /// </summary>
        public ClientState State { get; private set; }

        public async Task Save()
        {
            await AppPropertyHelpers.AddReplaceJson(AppProperty.State, State).ConfigureAwait(false);
        }

        public async Task<bool> Restore()
        {
            try
            {
                State = await AppPropertyHelpers.GetJson<ClientState>(AppProperty.State).ConfigureAwait(false) ?? new ClientState();

                if (State.Credential.AuthToken == null || State.Credential.AuthToken.ExpiresAt.ToUniversalTime() < DateTime.UtcNow)
                    return false;
                else
                    _tokenClient.SetCurrentAuthToken(State.Credential.AuthToken);
            }
            catch (Exception ex)
            {
                throw new HandledException(ex.Message, false);
            }

            return true;
        }

        public async Task Save<T>(AppProperty key, T value) where T : class
        {
            await AppPropertyHelpers.AddReplaceJson<T>(key, value).ConfigureAwait(false);
        }

        public async Task Clear()
        {
            _tokenClient.SetCurrentAuthToken(null);

            State = new ClientState();
            await AppPropertyHelpers.Clear().ConfigureAwait(false);
        }

        public async Task Clear(AppProperty key)
        {
            await AppPropertyHelpers.Clear(key).ConfigureAwait(false);
        }

        public async Task<T> Get<T>(AppProperty key) where T : class
        {
            return await AppPropertyHelpers.GetJson<T>(key).ConfigureAwait(false);
        }

    }
}
