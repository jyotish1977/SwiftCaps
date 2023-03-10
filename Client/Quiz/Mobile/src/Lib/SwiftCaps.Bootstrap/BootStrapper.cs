using System;
using CommonServiceLocator;
using MoreLinq;
using SwiftCaps.Client.Cache.Service.Data;
using SwiftCaps.Client.Cache.Service.Interfaces;
using SwiftCaps.Client.Cache.Service.Services;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Services;
using SwiftCaps.Client.Core.Services.Infrastructure;
using SwiftCaps.Client.Core.Services.Interfaces;
using SwiftCaps.Helpers.DateTime;
using SwiftCaps.Helpers.DateTime.Interfaces;
using SwiftCaps.Mobile.Shared.Services;
using SwiftCaps.Services.Abstraction.Interfaces;
using Unity;
using Unity.Lifetime;
using Unity.ServiceLocation;
using Xamariners.Mobile.Core.Interfaces;
using Xamariners.Mobile.Core.Services;
using Xamariners.RestClient.Interfaces;
using Xamariners.RestClient.Models;
using Xamariners.RestClient.Services;

namespace SwiftCaps.Client.Bootstrap
{
    public sealed class BootStrapper
    {
        private readonly IAppSettings _appSettings;
        private static Lazy<IUnityContainer> _container;

        public static IAppSettings AppSettings { get; private set; }

        public static void Initialize(IAppSettings appSettings)
        {
            AppSettings = appSettings;
            new BootStrapper(appSettings);
        }

        public static void Dispose()
        {
            if (Container != null)
            {
                Container.Registrations.ForEach(x => x.LifetimeManager.RemoveValue());
                Container.Dispose();
                _container = null;

                ServiceLocator.SetLocatorProvider(null);
            }
        }

        private BootStrapper(IAppSettings appSettings)
        {
            _appSettings = appSettings;

            _container = new Lazy<IUnityContainer>(() =>
            {
                UnityContainer container = new UnityContainer();
                container.RegisterInstance(appSettings);

                // Register other services
                RegisterCommonServices(container);

#if !FAKE
                RegisterServices(container);
#endif

                var locator = new UnityServiceLocator(container);
                ServiceLocator.SetLocatorProvider(() => locator);

                return container;
            });
        }

        /// <summary>
        /// Unity Container
        /// </summary>
        public static IUnityContainer Container => _container?.Value;

        /// <summary>
        /// Use this method to register anything shared external services in non shared projects.
        /// Example: register for OTPServices, etc.
        /// </summary>
        /// <param name="container"></param>
        public void RegisterCommonServices(IUnityContainer container)
        {
            container.RegisterInstance(_appSettings);

            // TODO: should take timeout and enable rewrite val from _appSettings
            var apiConfiguration = new ApiConfiguration(_appSettings.ApiEndpoint, int.Parse(_appSettings.ApiTimeOut), false);
            var idsConfiguration = new IDSConfiguration(null, _appSettings.IdentityClientId, _appSettings.IdentityTenantId, null, null,
                _appSettings.IdentityScope, null);

            container.RegisterInstance<IApiConfiguration>(apiConfiguration);
            container.RegisterInstance<IIDSConfiguration>(idsConfiguration);
            container.RegisterSingleton<IRestConfiguration, RestConfiguration>();
            container.RegisterSingleton<INavigationService, NavigationService>();
            container.RegisterSingleton<IInitialiserService, InitialiserService>();
            container.RegisterSingleton<IServiceBase, ServiceBase>();
            
            container.RegisterSingleton<IErrorService, ErrorService>();
            container.RegisterSingleton<IPopupInputService, PopupInputService>();
            container.RegisterSingleton<ISpinner, Spinner>();
            container.RegisterSingleton<IAppCacheService<ClientState>, AppCacheService>();

            container.RegisterType<QuizCacheService>();
            container.RegisterType<LeaderBoardCacheService>();

            container.RegisterType<ISwiftCapsApiServices, SwiftCapsApiServices>();
            container.RegisterSingleton<ISwiftCapsCacheServices, SwiftCapsCacheServices>();
            container.RegisterSingleton<IAuthService, AuthService>();
            container.RegisterSingleton<IAdAuthService, AdAuthService>();

            container.RegisterSingleton<IDateTimeOffsetProvider, DateTimeOffsetProvider>();

            // register rest client singleton to two interfaces
            var rc = container.Resolve<RestClientMSAL>();
            container.RegisterInstance<ITokenClient>(rc);
            container.RegisterInstance<IRestClient>(rc);
        }

        /// <summary>
        /// Use this method to register anything concrete external services in non shared projects.
        /// Example: register for OTPServices, etc.
        /// </summary>
        /// <param name="container"></param>
        public void RegisterServices(IUnityContainer container)
        {
            container.RegisterType<IQuizService, Client.Core.Services.QuizService>();
            container.RegisterType<IUserService, Client.Core.Services.UserService>();
            container.RegisterType<ILeaderBoardService, Client.Core.Services.LeaderBoardService>();
        }
    }
}
