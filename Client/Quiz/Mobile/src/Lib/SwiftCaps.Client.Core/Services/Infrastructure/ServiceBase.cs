using SwiftCaps.Client.Core.Interfaces;
using Unity;
using Xamariners.RestClient.Interfaces;


namespace SwiftCaps.Client.Core.Services.Infrastructure
{
    public abstract class ServiceBase : IServiceBase
    {
        #region Public Properties
        /// <summary>
        /// Gets the application settings.
        /// </summary>
        [Dependency]
        public IAppSettings AppSettings { get; set; }

        /// <summary>
        /// Gets the REST client helper.
        /// </summary>
        [Dependency]
        public IRestClient RestClient { get; set; }

        /// <summary>
        /// Gets the Client State related properties.
        /// </summary>
        [Dependency]
        public IAppCacheService<ClientState> AppCache { get; set; }
        #endregion

    }
}
