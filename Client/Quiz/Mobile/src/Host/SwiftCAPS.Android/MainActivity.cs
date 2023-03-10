using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Microsoft.Identity.Client;
using Mindscape.Raygun4Net;
using PCLAppConfig;
using PCLAppConfig.FileSystemStream;
using Plugin.CurrentActivity;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using SwiftCaps.Client.Bootstrap;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Models;
using SwiftCaps.Client.Core.Services.Infrastructure;
using Unity;
using Xamariners.Core.Interface;
using Xamariners.Mobile.Core.Infrastructure;
using Xamariners.Mobile.Core.Interfaces;
using Xamariners.Shared.Services;
using Resource = SwiftCAPS.Droid.Resource;
using Rg.Plugins.Popup;
using SwiftCAPS.Droid.Services;
using Xamarin.Forms;

namespace SwiftCaps.Droid
{
    [Activity(Label = "SwiftCaps", Icon = "@mipmap/icon", Theme = "@style/MainTheme.Splash", MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.SetTheme(Resource.Style.MainTheme);
            base.OnCreate(savedInstanceState);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironmentOnUnhandledException;
            Window.SetSoftInputMode(Android.Views.SoftInput.AdjustResize);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // BE VERY VERY CAREFUL IF YOU HAVE TO CHANGE THE NEXT LINE - RG PLUGIN INIT
            // THIS MEANS YOU (OR SOMEBODY IN A REFERENCED NUGET) UPDATED TO THE LATEST PLUGIN AND YOUR ENTIRE SOLUTION TESTS WILL BREAK
            // ASKING YOU ABOUT -WINDOWS DIRECTIVE TO YOUR TESTPROJ FILE AND WASTING HOURS OF HEAD SCRATCHING
            // BEWARE ! BEWARE ! BEWARE ! BEWARE ! BEWARE ! BEWARE ! BEWARE ! BEWARE ! BEWARE ! BEWARE ! BEWARE !  
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            #region App Settings and bootstrapper
            try { ConfigurationManager.Initialise(PortableStream.Current); } catch (Exception) { }
            BootStrapper.Initialize(new AppSettings());
            RegisterPlatformSpecificTypes();
            #endregion

            Initialize();

            LoadApplication(new App());

            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            SwiftCaps.Client.Core.Helpers.CrossCurrentActivity.Current = this;
        }

        private void Initialize()
        {
            if (BootStrapper.AppSettings.RaygunApikey != null)
                RaygunClient.Initialize(BootStrapper.AppSettings.RaygunApikey).AttachCrashReporting().AttachPulse(this);
            else
                throw new Exception("Raygun license key not found in the config");

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            
            SQLitePCL.Batteries_V2.Init();
        }

        private void AndroidEnvironmentOnUnhandledException(object sender, RaiseThrowableEventArgs e)
        {
            Exception ex = e.Exception;
            HandleException(ex);
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
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
            if (isTerminating)
                Task.Run(() => BootStrapper.Container.Resolve<IAppCacheService<ClientState>>().Save());

            var innerEx = Xamariners.Core.Common.Helpers.ExceptionHelpers.GetInnermostException(ex);
            BootStrapper.Container.Resolve<IErrorService>().AddError(innerEx.Message,
                ViewModelError.ErrorAction.Log,
                ViewModelError.ErrorSeverity.Error,
                ex: innerEx);
            BootStrapper.Container.Resolve<IErrorService>().ProcessErrors(); 
        }

        private void RegisterPlatformSpecificTypes()
        {
            BootStrapper.Container.RegisterType<ILogger, Xamariners.Shared.Services.RaygunLogger>();
        
            // THIRD PARTY
            BootStrapper.Container.RegisterInstance<IPopupNavigation>(PopupNavigation.Instance);
            BootStrapper.Container.RegisterSingleton<INativeKeyboardVisibilityService, NativeKeyboardVisibilityService>();
        }

        public override void OnBackPressed()
        {
            try
            {
                if (Popup.SendBackPressed(base.OnBackPressed))
                {
                    // Do something if there are some pages in the `PopupStack`
                }
                else
                {
                    // Do something if there are not any pages in the `PopupStack`
                }
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }
    }
}
