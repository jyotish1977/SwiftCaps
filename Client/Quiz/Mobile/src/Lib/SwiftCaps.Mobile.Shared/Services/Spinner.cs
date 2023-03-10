using System;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Extensions;
using SwiftCaps.Mobile.Shared.Controls;
using Xamariners.Mobile.Core.Extensions;
using Xamariners.Mobile.Core.Helpers;
using Xamariners.Mobile.Core.Interfaces;

namespace SwiftCaps.Mobile.Shared.Services
{
    public class Spinner : ISpinner, IDisposable
    {
        private readonly INavigationService _navigationService;
        private readonly IPopupNavigation _popupNavigation;
        private SpinnerPage _spinnerPageInstance;

        public bool IsSpinning { get; private set; }

        public Spinner(INavigationService navigationService, IPopupNavigation popupNavigation)
        {
            _navigationService = navigationService;
            _popupNavigation = popupNavigation;
        }

        public void HideLoading()
        {
            if (_spinnerPageInstance == null)
                return;

            ThreadingHelpers.InvokeOnMainThread(async () =>
            {
                await _navigationService.Navigation
                    .RemovePopupPageAsyncSafe(_spinnerPageInstance, _popupNavigation)
                    .ContinueWith((t) => { IsSpinning = false; });
            });
        }

        public void ShowLoading(bool isCancellable = true)
        {
            ThreadingHelpers.InvokeOnMainThread(async () =>
            {
                _spinnerPageInstance ??= new SpinnerPage(isCancellable);

                // check if already spinning
                if (IsSpinning)
                    return;

                await _navigationService.Navigation.PushPopupAsyncSafe(_spinnerPageInstance, _popupNavigation)
                    .ContinueWith((t) => IsSpinning = true); 
            });
        }

        public void Dispose()
        {
        }
    }
}
