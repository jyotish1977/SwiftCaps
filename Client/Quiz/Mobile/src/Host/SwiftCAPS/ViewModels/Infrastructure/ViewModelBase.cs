using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommonServiceLocator;
using Xamarin.Forms;
using Xamariners.Core.Common.Enum;
using Xamariners.Core.Common.Helpers;
using Xamariners.Mobile.Core.Infrastructure;
using Xamariners.Mobile.Core.Interfaces;
using Xamariners.Mvvm;
using Unity;
using System.ComponentModel;
using SwiftCaps.Client.Bootstrap;
using SwiftCaps.Client.Cache.Service.Interfaces;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Services.Infrastructure;
using SwiftCaps.Values;
using Xamariners.Mobile.Core.Helpers;

namespace SwiftCaps.ViewModels
{
    public abstract partial class ViewModelBase : ValidatableBindableBase, INotifyPropertyChanged
    {
        protected static ShellNavigatingEventArgs ShellNavigatingEventArgs { get; set; }
        protected static ShellNavigatedEventArgs ShellNavigatedEventArgs { get; set; }

        internal delegate void InitializedViewModelBase();

        public bool IsInitialized { get; protected set; }

        public Action OnNavigatedComplete { get; set; }

        protected INavigationService _navigationService => Client.Bootstrap.BootStrapper.Container.Resolve<INavigationService>();

        protected IInitialiserService _initialiserService => Client.Bootstrap.BootStrapper.Container.Resolve<IInitialiserService>();

        protected IMainViewModel _mainViewModel => Client.Bootstrap.BootStrapper.Container.Resolve<IMainViewModel>();

        protected ISpinner _spinner => Client.Bootstrap.BootStrapper.Container.Resolve<ISpinner>();

        public static Type CurrentViewModelType { get; private set; }

        public string CurrentPageType => CurrentViewModelType.Name.Replace("ViewModel", string.Empty);

        public string CurrentRoute => GetCurrentRoute();

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the HasNavigationBar.
        /// </summary>
        public bool HasNavigationBar { get; set; }

        /// <summary>
        /// Gets or sets the HasNavigationBarTitleImage.
        /// </summary>
        public bool HasNavigationBarTitleImage { get; set; }

        /// <summary>
        /// Gets or sets the HasBackButton.
        /// </summary>
        public bool HasBackButton { get; set; }

        /// <summary>
        /// Gets or sets the NavigationBarTitleImage.
        /// </summary>
        public ImageSource NavigationBarTitleImage { get; set; } // TODO: should be a string path

        /// <summary>
        /// Gets or sets the BackButtonText.
        /// </summary>
        public string BackButtonTitle { get; set; }

        /// <summary>
        /// Gets or sets the HasLeftCustomButton.
        /// </summary>
        public bool HasLeftCustomButton { get; set; }

        /// <summary>
        /// Gets or Sets the iOS Status bar hidden/visible
        /// </summary>
        public bool IsStatusBarHidden { get; set; }

        /// <summary>
        /// For setting the background color of page
        /// </summary>
        public Color PageBackgroundColor { get; set; }


        /// <summary>
        /// Gets a value indicating whether this instance is current view model.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is current view model; otherwise, <c>false</c>.
        /// </value>
        public bool IsCurrentViewModel => this.GetType() == CurrentViewModelType;

        public XDelegateCommand GoBackCommand { get; set; }

        protected ViewModelBase()
        {
            GoBackCommand = new XDelegateCommand(async () => await GoBack(), () => true);
            
            // initialize only if has pushed
            Initialize();
        }

        ~ViewModelBase()
        {
            try
            {
                MessagingCenter.Unsubscribe<string, ShellNavigatingEventArgs>(nameof(AppShell),
                    $"{GetCurrentRoute()}{nameof(AppShell.OnShellNavigating)}In");
                MessagingCenter.Unsubscribe<string, ShellNavigatingEventArgs>(nameof(AppShell),
                    $"{GetCurrentRoute()}{nameof(AppShell.OnShellNavigating)}Out");
                MessagingCenter.Unsubscribe<string, ShellNavigatedEventArgs>(nameof(AppShell),
                    GetCurrentRoute() + nameof(AppShell.OnShellNavigated));
            }
            catch
            {}
        }

        private void Initialize()
        {
            if (IsInitialized || CurrentRoute == "appshell")
                return;
            
            MessagingCenter.Subscribe<string, ShellNavigatingEventArgs>(nameof(AppShell), $"{GetCurrentRoute()}{nameof(AppShell.OnShellNavigating)}Out",
                async (sender, args) => await OnShellNavigatingOut(sender, args));

            MessagingCenter.Subscribe<string, ShellNavigatingEventArgs>(nameof(AppShell), $"{GetCurrentRoute()}{nameof(AppShell.OnShellNavigating)}In",
                 async (sender, args) => await OnShellNavigatingIn(sender, args));

            MessagingCenter.Subscribe<string, ShellNavigatedEventArgs>(nameof(AppShell), GetCurrentRoute() + nameof(AppShell.OnShellNavigated),
                 async (sender, args) => await OnShellNavigated(sender, args));

            IsInitialized = true;
        }

        protected virtual async Task OnShellNavigatingOut(string sender, ShellNavigatingEventArgs args)
        {
            ShellNavigatingEventArgs = args;

            _errorService.ClearInlineValidators();
        }

        protected virtual async Task OnShellNavigatingIn(string sender, ShellNavigatingEventArgs args)
        {
            ShellNavigatingEventArgs = args;

            if (this.GetType() != typeof(AppShellViewModel) && this.GetType() != typeof(MainViewModel))
                CurrentViewModelType = this.GetType();

            await _mainViewModel.ExecuteLifeCycleAction()
                .ContinueWith((x) => {
                    // handle ExecuteLifeCycleAction failure as needed
                    _initialiserService.Initialise(this);
                    })
                .ConfigureAwait(false);
        }

        protected virtual async Task OnShellNavigated(string sender, ShellNavigatedEventArgs args)
        {
            ShellNavigatedEventArgs = args;

            if (_navigationService.NavigationDirection == NavigationDirection.Backwards)
                _initialiserService.Initialise(this);

            OnNavigatedComplete?.Invoke();

            // reset navigation direction
            _navigationService.NavigationDirection = NavigationDirection.Forward;
        }

        private IEnumerable<PropertyInfo> GetNestedViewModelsInfo()
        {
            return GetType().GetProperties(ReflectionExtensions.BindingFlags.Public | ReflectionExtensions.BindingFlags.Instance)
                .Where(x => x.PropertyType.Name.Contains("ViewModel"));
        }

        protected virtual async Task GoBack()
        {
            try
            {
                await _navigationService.GoBackAsync();
            }
            finally
            {
                GoBackCommand.Reset();
            }
        }

        public async void OnSleep()
        {
            // In the case of Upgrade alert popup, we are closing it down when the app goes to background.
            await _popupInputService.CloseLastPopup();
        }

        private string GetCurrentRoute()
        {
            return this.GetType().Name.Replace("ViewModel", string.Empty).ToLower();
        }

        public async Task Logout()
        {
            await _popupInputService
                .ShowMessageOkAlertPopup("Authentication Error", "Please click here to continue", "Logout").ConfigureAwait(true);

            await BootStrapper.Container.Resolve<ISwiftCapsCacheServices>()
                .DeleteDatabase(BootStrapper.Container.Resolve<IAppCacheService<ClientState>>().State.AppDataPath)
                .ConfigureAwait(false);

            await _authService.Logout().ConfigureAwait(false);

            ThreadingHelpers.InvokeOnMainThread(() => _navigationService.GoToAsync(Constants.ShellNavigation.LoginPagePath));
        }
    }
}
