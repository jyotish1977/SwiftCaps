using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.XPath;
using CommonServiceLocator;
using Shouldly;
using SwiftCaps.Client.Bootstrap;
using SwiftCaps.Client.Cache.Service.Interfaces;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Models;
using SwiftCaps.Client.Core.Services.Infrastructure;
using SwiftCaps.Fake.Helpers;
using SwiftCaps.Helpers;
using SwiftCaps.Helpers.DateTime.Interfaces;
using SwiftCAPS.Mobile.UnitTest.Services;
using SwiftCaps.ViewModels;
using SwiftCaps.ViewModels.Infrastructure;
using Unity;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamariners.UnitTest.Xamarin.SharedSteps;
using Xamariners.Utilities.Helpers;

namespace SwiftCaps.Mobile.UnitTest
{
    public partial class TestApp : Xamariners.UnitTest.Xamarin.TestApp
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

        public TestApp()
            : base(GetCurrentViewModelImpl,
                () => ViewModelBase.CurrentViewModelType,
                 GoBackImpl, "SwiftCaps.Mobile.UnitTest.dll.config")
        {
            BootStrapper.Initialize(new AppSettings());

            Container = BootStrapper.Container;

            RegisterTestServices();

            OnContainerInitialized();

            InitCaching();
           
            InitializeComponent();

            ViewModelLocator.Init();

            // Set Start Page
            MainPage = new AppShell();

            OnStart();

        }

        private void InitCaching()
        {
            // Init Caching
            Container.Resolve<IAppCacheService<ClientState>>().State.AppDataPath = AppDomain.CurrentDomain.BaseDirectory;
            Container.Resolve<IAppCacheService<ClientState>>().Save();
        }

        public static object? GetCurrentViewModelImpl()
        {
            var success = Xamariners.RestClient.Helpers.RetryHelpers.Retry(() =>
                    ViewModelBase.CurrentViewModelType?.Name ==
                    ((AppShell)Current?.MainPage!)?.CurrentPage?.BindingContext.GetType().Name
                , 200);
         
            return ((AppShell)Current?.MainPage!)?.CurrentPage?.BindingContext as ViewModelBase;
        }

        public static async Task GoBackImpl()
        {
            var vm = GetCurrentViewModelImpl();

            var cmd = ((ViewModelBase)vm!)?.GoBackCommand;
            cmd.Execute();

            if (cmd is { IsValid: true })
                cmd.WaitHandle.WaitOne();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            Container.Resolve<IMainViewModel>().OnStart();
        }

        protected override void OnSleep()
        {
            Container.Resolve<IMainViewModel>().OnSleep();
        }

        protected override void OnResume()
        {
            Container.Resolve<IMainViewModel>().OnResume();
        }

        protected override void RegisterTestServices()
        {
            base.RegisterTestServices();
            Container.RegisterSingleton<INativeBrowserService, TestNativeBrowserService>();
            Container.RegisterSingleton<IDateTimeOffsetProvider, DateTimeOffsetProvider>();

        }

        public static void OnContainerInitialized()
        {
            Fake.Infrastructure.FakeContainerInitialiser.RegisterFakeService();
            Fake.Infrastructure.FakeContainerInitialiser.SetupFakeDatabase();
            Fake.Infrastructure.FakeContainerInitialiser.ResetFakeData();
            Fake.Infrastructure.FakeContainerInitialiser.SeedFakeDatabase();
        }
    }
}
