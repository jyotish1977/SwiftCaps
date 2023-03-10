using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using BoDi;
using Shouldly;
using SwiftCaps.Client.Bootstrap;
using SwiftCaps.Client.Cache.Service.Data;
using SwiftCaps.Client.Cache.Service.Interfaces;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Services.Infrastructure;
using SwiftCaps.Fake.Infrastructure;
using SwiftCaps.Mobile.UnitTest;
using TechTalk.SpecFlow;
using Unity;
using Xamarin.Forms;

namespace SwiftCAPS.Mobile.UnitTest.Infrastructure
{
    [Binding]
    public class SetupHooks : Xamariners.UnitTest.Xamarin.Infrastructure.SetupHooks
    {
        private readonly ScenarioContext _scenarioContext;

        public SetupHooks(IObjectContainer objectContainer, ScenarioContext scenarioContext) : base(objectContainer)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public override void BeforeScenario()
        {
            base.BeforeScenario();
            App = new TestApp();
        }

        [AfterScenario("ClearSubscribers", Order = 1)]
        public void ClearSubscribers()
        {
            var subscriptions = (IDictionary)typeof(MessagingCenter)
                .GetField("_subscriptions", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(MessagingCenter.Instance)!;

            subscriptions.Clear();
        }

        [AfterScenario(Order = 10)]
        public override void AfterScenario()
        {
            App.Container.Resolve<ISwiftCapsCacheServices>()
                .DeleteDatabase(App.Container.Resolve<IAppCacheService<ClientState>>().State.AppDataPath);

            App.Container.Resolve<ISwiftCapsApiServices>().AppCache.Clear();

            base.AfterScenario(); // need to remove ScenarioContext.Current from base 
            App = null;

            FakeDbContext.Terminate();
            //BootStrapper.Dispose(); // container manages itself
            _scenarioContext.Clear();
        }
    }
}
