using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Shouldly;
using SwiftCaps.Client.Cache.Service.Interfaces;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Mobile.UnitTest;
using SwiftCaps.ViewModels;
using TechTalk.SpecFlow;
using Unity;
using Xamariners.RestClient.Helpers;

namespace SwiftCAPS.Mobile.UnitTest.Infrastructure
{
    public class StepBase : Xamariners.UnitTest.Xamarin.Infrastructure.StepBase
    {
        public new SwiftCaps.Mobile.UnitTest.TestApp App => (SwiftCaps.Mobile.UnitTest.TestApp)SetupHooks.App;

        public ISwiftCapsApiServices Services => App.Container.Resolve<ISwiftCapsApiServices>();
        public ISwiftCapsCacheServices CacheServices => App.Container.Resolve<ISwiftCapsCacheServices>();

        public StepBase(ScenarioContext scenarioContext) : base(scenarioContext)
        {
          
        }

        public ViewModelBase GetCurrentViewModel()
        {
            var currentViewModelName = ViewModelBase.CurrentViewModelType?.Name;

            if (string.IsNullOrEmpty(currentViewModelName))
                return null;

            var type = typeof(ViewModelBase).Assembly.GetExportedTypes().First(x => x.Name == ViewModelBase.CurrentViewModelType?.Name);
            var result = (ViewModelBase)App.Container.Resolve(type);

            RetryHelpers.Retry(() => result.IsInitialized, 200);
            return result;
        }

        public  TViewModel GetCurrentViewModel<TViewModel>() where TViewModel : ViewModelBase
        {
            var vm = TestApp.GetCurrentViewModelImpl();

            if (vm != null && vm is TViewModel)
                return (TViewModel)vm;

            var vm2 = App.Container.Resolve<TViewModel>();
            RetryHelpers.Retry(() => vm2.IsInitialized, 200);
            return App.Container.Resolve<TViewModel>();
        }
    }
}
