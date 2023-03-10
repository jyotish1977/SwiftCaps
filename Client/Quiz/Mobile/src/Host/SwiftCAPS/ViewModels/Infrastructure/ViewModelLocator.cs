using System;
using SwiftCaps.Client.Bootstrap;
using SwiftCaps.Values;
using SwiftCaps.Views;
using Unity;
using Unity.Lifetime;
using Xamarin.Forms;

namespace SwiftCaps.ViewModels.Infrastructure
{
    public class ViewModelLocator
    {
        /// <summary>
        /// ViewModelLocator constructor
        /// </summary>
        static ViewModelLocator()
        {
            Init();
        }

        // CORE
        public AppShellViewModel AppShell => BootStrapper.Container.Resolve<AppShellViewModel>();
        public QuizPageViewModel QuizPage => BootStrapper.Container.Resolve<QuizPageViewModel>();
        public QuizListPageViewModel QuizListPage => BootStrapper.Container.Resolve<QuizListPageViewModel>();
        public QuizTrackerPageViewModel QuizTrackerPage => BootStrapper.Container.Resolve<QuizTrackerPageViewModel>();
        public LoginPageViewModel LoginPage => BootStrapper.Container.Resolve<LoginPageViewModel>();

        public static void Init()
        {
            // Register anything related to this shared projects including *ViewModels.
            // After registering the page to Unity container, make sure to define the
            // property of the view model as well. See below part.

            // CORE
            BootStrapper.Container.RegisterSingleton<AppShellViewModel>();
            BootStrapper.Container.RegisterSingleton<IMainViewModel, MainViewModel>();

            RegisterViewModel<QuizListPageViewModel, QuizListPage>();
            RegisterViewModel<LoginPageViewModel, LoginPage>();
            RegisterViewModel<QuizPageViewModel, QuizPage>();
            RegisterViewModel<QuizTrackerPageViewModel, QuizTrackerPage>();

            void RegisterViewModel<TViewModel, TPage>()
            {
                BootStrapper.Container.RegisterType<TViewModel>(new SingletonLifetimeManager());
                BootStrapper.Container.RegisterType<TPage>();
            }
        }

        public static void RegisterRoutes()
        {
            // REGISTER ROUTES

            RegisterRoute<QuizPage>(Constants.ShellNavigation.QuizPagePath); 
            RegisterRoute<QuizTrackerPage>(Constants.ShellNavigation.QuizTrackerPagePath);
        }

        private static void RegisterRoute<TPage>(string route) where TPage : class
        {
            // Hack: Need For SpecFlow Tests 
            Routing.UnRegisterRoute(route);

            // Register the Route
            var typeFacInstance = new TypeRouteFactory(typeof(TPage));
            Routing.RegisterRoute(route, typeFacInstance);
            typeFacInstance.GetOrCreate();
        }

        private class TypeRouteFactory : RouteFactory
        {
            private readonly Type _type;

            public TypeRouteFactory(Type type)
            {
                _type = type;
            }

            public override Element GetOrCreate()
            {
                return (Element) BootStrapper.Container.Resolve(this._type);
            }
        }
    }
}
