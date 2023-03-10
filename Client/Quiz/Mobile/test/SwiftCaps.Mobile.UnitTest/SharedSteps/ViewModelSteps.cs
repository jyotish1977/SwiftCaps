using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Services.Infrastructure;
using SwiftCAPS.Mobile.UnitTest.Infrastructure;
using TechTalk.SpecFlow;
using Unity;
using Xamariners.RestClient.Helpers;

namespace SwiftCAPS.Mobile.UnitTest.SharedSteps
{
    [Binding]
    public class ViewModelSteps : StepBase
    {
        public ViewModelSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"I can see the page title as ""(.*)""")]
        [When(@"I can see the page title as ""(.*)""")]
        [Then(@"I can see the page title as ""(.*)""")]
        public void ThenICanSeeThePageTitleAs(string title)
        {
            var vm = GetCurrentViewModel();
            vm.Title.ShouldBe(title);
        }

        [When(@"I refresh the cache")]
        public void WhenIRefreshTheCache()
        {
            var path = App.Container.Resolve<IAppCacheService<ClientState>>().State.AppDataPath;
            var memberId = App.Container.Resolve<IAppCacheService<ClientState>>().State.Member.Id;
            int? lastCount = null;
            
            // deterministic cycling through the empty refresh (0), app launch refresh, and test fired refresh

            CacheServices.WaitHandle.WaitOne();

            var result = RetryHelpers.Retry(() =>
            {
                CacheServices.Refresh(path, memberId);
                CacheServices.WaitHandle.WaitOne();

                var currentCount = CacheServices.QuizCacheService
                    .GetUserQuizzesCache(path).Result.Data.Count();

                if (lastCount == currentCount)
                    return true;

                lastCount = currentCount;

                return false;
            }, 200);

            result.ShouldBeTrue();

        }
    }
}
