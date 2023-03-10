using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using SwiftCaps.Client.Bootstrap;
using SwiftCaps.Client.Core.Enums;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Services.Infrastructure;
using SwiftCaps.Helpers;
using SwiftCaps.Mobile.Shared.Helpers;
using SwiftCaps.ViewModels;
using Unity;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamariners.Mobile.Core.Helpers;
using static SwiftCaps.Values.Constants;
using Application = Xamarin.Forms.Application;
using Device = Xamarin.Forms.Device;

namespace SwiftCaps
{
    public partial class App : Application
    {
        private static NavigableElement _navigationRoot;

        public static App Instance => Current as App;

        public static AppShell Shell => Instance.MainPage as AppShell;

        public static AppShellViewModel ShellViewModel => Shell.BindingContext as AppShellViewModel;

        public static NavigableElement NavigationRoot
        {
            get => NavigationHelpers.GetShellSection(_navigationRoot) ?? _navigationRoot;
            set => _navigationRoot = value;
        }

        public App()
        {
            InitAppCenter();

            OnContainerInitialized();

            InitializeComponent();

            MainPage = new AppShell();

            // Setting the keyboard popover behavior for Quiz Page (Adjust Layout to be scrollable)
            if (Device.RuntimePlatform == Device.Android)
            {
                Current?.On<Xamarin.Forms.PlatformConfiguration.Android>()
                    .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
            }

            BootStrapper.Container.Resolve<IAppCacheService<ClientState>>().State.AppDataPath =
                FileSystem.AppDataDirectory;
            BootStrapper.Container.Resolve<IAppCacheService<ClientState>>().Save();
        }

        private async Task InitAppCenter()
        {
            if (!AppCenter.Configured)
            {
                string logTag = BootStrapper.AppSettings.AppCenterLogTag;
                string androidKey = BootStrapper.AppSettings.AppCenterAndroidKey;
                string iOSKey = BootStrapper.AppSettings.AppCenterIosKey;

                AppCenter.LogLevel = LogLevel.Verbose;
                AppCenter.Start($"ios={iOSKey};android={androidKey};", typeof(Analytics), typeof(Crashes),
                    typeof(Distribute));

                await AppCenter.GetInstallIdAsync().ContinueWith(installId =>
                {
                    if (installId.Result.HasValue)
                    {
                        ThreadingHelpers.InvokeOnMainThread(async () =>
                            await AppPropertyHelpers.AddReplaceValue(AppProperty.DeviceId, installId.Result.Value));
                        AppCenterLog.Info(logTag, "AppCenter.InstallId=" + installId.Result);
                    }
                });

                await Crashes.HasCrashedInLastSessionAsync().ContinueWith(hasCrashed =>
                        AppCenterLog.Info(logTag, "Crashes.HasCrashedInLastSession=" + hasCrashed.Result))
                    .ConfigureAwait(false);

                await Crashes.GetLastSessionCrashReportAsync().ContinueWith(report =>
                        AppCenterLog.Info(logTag,
                            "Crashes.LastSessionCrashReport.Exception=" + report.Result?.StackTrace))
                    .ConfigureAwait(false);

                await Analytics.SetEnabledAsync(true).ConfigureAwait(false);
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            BootStrapper.Container.Resolve<IMainViewModel>().OnSleep();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            BootStrapper.Container.Resolve<IMainViewModel>().OnResume();
        }

        protected override async void OnStart()
        {
            await BootStrapper.Container.Resolve<IMainViewModel>().OnStart();

            if (!BootStrapper.Container.Resolve<IAppCacheService<ClientState>>().State.IsAuthenticated)
                return;

            // TODO: Set setting for app state as to tell the next viewmodel that app started
            var navigationService = BootStrapper.Container.Resolve<Xamariners.Mobile.Core.Interfaces.INavigationService>();

            await navigationService.GoToAsync(ShellNavigation.QuizListPagePath, false);
        }

        public static void OnContainerInitialized()
        {
#if FAKE
            SwiftCaps.Fake.Infrastructure.FakeContainerInitialiser.RegisterFakeService();
            SwiftCaps.Fake.Infrastructure.FakeContainerInitialiser.SetupFakeDatabase();
            SwiftCaps.Fake.Infrastructure.FakeContainerInitialiser.SeedFakeDatabase();
#endif
        }
    }
}
