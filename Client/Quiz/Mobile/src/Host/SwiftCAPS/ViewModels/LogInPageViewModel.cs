using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using SwiftCaps.Client.Cache.Service.Interfaces;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Services.Infrastructure;
using SwiftCaps.Client.Core.Services.Interfaces;
using SwiftCaps.Values;
using Unity;
using Xamarin.Essentials;
using Xamariners.Mobile.Core.Helpers;
using Xamariners.Mobile.Core.Infrastructure;

namespace SwiftCaps.ViewModels
{
    public class LoginPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
       
        private readonly ISwiftCapsCacheServices _cacheServices;
        private readonly IAppCacheService<ClientState> _appCacheService;

        public XDelegateTimerCommand LoginCommand { get; }

        public LoginPageViewModel(ISwiftCapsCacheServices cacheServices, IAppCacheService<ClientState> appCacheService)
        {
            _cacheServices = cacheServices;
            _appCacheService = appCacheService;

            LoginCommand = new XDelegateTimerCommand(async () => await Login(), () =>  true);
        }

        private async Task<bool> Login()
        {
            try
            {
                _spinner.ShowLoading();

                var response = await _authService.Login().ConfigureAwait(false);
                
                if (response.IsOK() && response.Data)
                {   
                    // reset database location
                    // Ignore on test runner
                    if (DeviceInfo.DeviceType != DeviceType.Unknown)
                    {
                        _appCacheService.State.AppDataPath = FileSystem.AppDataDirectory;

                        // refresh cache if not restarting / resuming
                        if (!_mainViewModel.PendingLifeCycleAction)
                            await _cacheServices.Refresh(_appCacheService.State.AppDataPath, _appCacheService.State.Member.Id).ConfigureAwait(false);
                    }

                    _spinner.HideLoading();

                    ThreadingHelpers.InvokeOnMainThread(async () => await _navigationService.GoToAsync(Constants.ShellNavigation.QuizListPagePath).ConfigureAwait(true));
                }
                else if (response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    _spinner.HideLoading();

                    await _popupInputService.ShowMessageOkAlertPopup(
                        response.Message,
                        response.ErrorMessage, "OK").ConfigureAwait(false);
                }
                else
                {
                    _errorService.AddError(response);
                }

                _errorService.ProcessErrors();
                return response.IsOK();
            }
            catch (Exception ex)
            {
                HandleUIError(ex);
                return false;
            }
            finally
            {
                LoginCommand.ResetTimer();
            }
        }
    }
}
