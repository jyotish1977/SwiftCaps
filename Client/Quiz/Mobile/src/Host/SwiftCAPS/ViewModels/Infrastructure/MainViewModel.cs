using System;
using System.Threading.Tasks;
using SwiftCaps.Client.Bootstrap;
using SwiftCaps.Client.Cache.Service.Interfaces;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Services.Infrastructure;
using Xamariners.Core.Common.Enum;
using Xamariners.Mobile.Core.Interfaces;
using Xamariners.Mvvm;
using Unity;
using Xamarin.Essentials;
using DeviceType = Xamarin.Essentials.DeviceType;

namespace SwiftCaps.ViewModels
{

    public class MainViewModel : ValidatableBindableBase, IMainViewModel
    {
        private IErrorService _errorService => BootStrapper.Container.Resolve<IErrorService>();

        private readonly ISwiftCapsCacheServices _cacheServices;
        private readonly IAppCacheService<ClientState> _appCacheService;

        public Func<Task<bool>> OnSleepAction { get; set; }
        public Func<Task<bool>> OnResumeAction { get; set; }
        public Func<Task<bool>> OnStartAction { get; set; }

        public bool PendingLifeCycleAction => OnSleepAction != null && OnResumeAction != null && OnStartAction != null;

        public MainViewModel(ISwiftCapsCacheServices cacheServices, IAppCacheService<ClientState> appCacheService) : base()
        {
            _cacheServices = cacheServices;
            _appCacheService = appCacheService;
        }

        public async Task OnSleep()
        {
           _appCacheService.State.AppStatus = AppStatus.GoingToSleep;
            await _appCacheService.Save().ConfigureAwait(false);
        }

        public async Task OnResume()
        {
           _appCacheService.State.AppStatus = AppStatus.JustWokeUp;
           await _appCacheService.Save().ConfigureAwait(false);

            // TODO: we may want to force a refresh as page will not reload on resume
            OnResumeAction = async () =>
            {
                if (!IsConnected())
                    return false;

                await _appCacheService.Save().ConfigureAwait(false);

                var result = await _cacheServices.Refresh(_appCacheService.State.AppDataPath, _appCacheService.State.Member.Id).ConfigureAwait(false);

                if(!result.IsOK())
                    _errorService.AddError(result);

                return result.IsOK();
            };
        }
    

        public async Task OnStart()
        {
           _appCacheService.State.AppStatus = AppStatus.JustStarted;
           await _appCacheService.Save().ConfigureAwait(false);

            OnStartAction = async () =>
            {
                if (!IsConnected())
                    return false;
                
                var result = await _cacheServices.Refresh(_appCacheService.State.AppDataPath, _appCacheService.State.Member.Id).ConfigureAwait(false);

                if (!result.IsOK())
                    _errorService.AddError(result);

                return result.IsOK();

            };
        }

        public async Task OnRun()
        {
           _appCacheService.State.AppStatus = AppStatus.Running;
            await BootStrapper.Container.Resolve<IAppCacheService<ClientState>>().Save().ConfigureAwait(false);
        }

        public async Task<bool> ExecuteLifeCycleAction()
        {
            bool result = false;

            try
            {
                switch (BootStrapper.Container.Resolve<IAppCacheService<ClientState>>().State.AppStatus)
                {
                    case AppStatus.JustStarted:
                        if(OnStartAction != null)
                            result = await OnStartAction().ConfigureAwait(false);
                        OnStartAction = null;
                        break;

                    case AppStatus.JustWokeUp:
                        if (OnResumeAction != null)
                            result = await OnResumeAction().ConfigureAwait(false);
                        OnResumeAction = null;
                        break;

                    case AppStatus.GoingToSleep:
                        if (OnSleepAction != null)
                            result = await OnSleepAction().ConfigureAwait(false);
                        OnSleepAction = null;
                        break;

                    default:
                        break;
                }
            }
            finally
            {
                if(BootStrapper.Container.Resolve<IAppCacheService<ClientState>>().State.AppStatus != AppStatus.Running)
                    await OnRun().ConfigureAwait(false);
            }

            return result;
        }

        public bool IsConnected()
        {
            if (DeviceInfo.DeviceType == DeviceType.Unknown)
                return true;
            
            return Connectivity.NetworkAccess != NetworkAccess.None;
        }
    }
}
