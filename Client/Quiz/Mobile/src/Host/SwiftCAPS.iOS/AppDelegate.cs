using System;
using System.Threading.Tasks;
using Foundation;
using Microsoft.AppCenter.Distribute;
using Microsoft.Identity.Client;
using Mindscape.Raygun4Net;
using PCLAppConfig;
using PCLAppConfig.FileSystemStream;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using SwiftCaps.Client.Bootstrap;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Models;
using SwiftCaps.Client.Core.Services;
using SwiftCaps.Client.Core.Services.Infrastructure;
using SwiftCAPS.iOS.Services;
using UIKit;
using Unity;
using Xamarin.Forms;
using Xamariners.Core.Interface;
using Xamariners.Mobile.Core.Infrastructure;
using Xamariners.Mobile.Core.Interfaces;
using Xamariners.Shared.Services;

namespace SwiftCaps.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += UnobservedTaskException;

            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.Forms.FormsMaterial.Init();
            Rg.Plugins.Popup.Popup.Init();

            #region App Settings and bootstrapper
            try { ConfigurationManager.Initialise(PortableStream.Current); } catch (Exception) { }
            BootStrapper.Initialize(new AppSettings());
            RegisterPlatformSpecificTypes();
            #endregion

            Initialize();

            LoadApplication(new App());

            SwiftCaps.Client.Core.Helpers.CrossCurrentActivity.Current = null;

            return base.FinishedLaunching(app, options);
        }

        private void Initialize()
        {
            // Disable App Update check for Debug // req only for iOS
            Distribute.DontCheckForUpdatesInDebug();

            if (BootStrapper.AppSettings.RaygunApikey != null)
                RaygunClient.Initialize(BootStrapper.AppSettings.RaygunApikey).AttachCrashReporting().AttachPulse();
            else
                throw new Exception("Raygun license key not found in the config");

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            SQLitePCL.Batteries_V2.Init();

#if ENABLE_TEST_CLOUD
            // requires Xamarin Test Cloud Agent
            Xamarin.Calabash.Start();
#endif
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
            return true;
        }

        private void RegisterPlatformSpecificTypes()
        {
            BootStrapper.Container.RegisterType<ILogger, Xamariners.Shared.Services.RaygunLogger>();

            // THIRD PARTY
            BootStrapper.Container.RegisterInstance<IPopupNavigation>(PopupNavigation.Instance);
            BootStrapper.Container.RegisterSingleton<INativeKeyboardVisibilityService, NativeKeyboardVisibilityService>();
        }


        private void UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            HandleException(ex);
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            HandleException(ex, e.IsTerminating);
        }

        private void HandleException(Exception ex, bool isTerminating = false)
        {
            if(isTerminating)
                Task.Run(() => BootStrapper.Container.Resolve<IAppCacheService<ClientState>>().Save());

            var innerEx = Xamariners.Core.Common.Helpers.ExceptionHelpers.GetInnermostException(ex);
            BootStrapper.Container.Resolve<IErrorService>().AddError(innerEx.Message,
                ViewModelError.ErrorAction.Log,
                ViewModelError.ErrorSeverity.Error,
                ex: innerEx);
            BootStrapper.Container.Resolve<IErrorService>().ProcessErrors();
        }
    }
}
