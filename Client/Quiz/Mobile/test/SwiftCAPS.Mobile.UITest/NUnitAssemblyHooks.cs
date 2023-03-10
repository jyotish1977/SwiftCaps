using NUnit.Framework;
using TechTalk.SpecFlow;

namespace SwiftCAPS.Mobile.UITest
{
    [SetUpFixture]
    public class NUnitAssemblyHooks
    {
        [OneTimeSetUp]
        public void AssemblyInitialize()
        {
            var currentAssembly = typeof(NUnitAssemblyHooks).Assembly;

            TestRunnerManager.OnTestRunStart(currentAssembly);
        }

        [OneTimeTearDown]
        public void AssemblyCleanup()
        {
            var currentAssembly = typeof(NUnitAssemblyHooks).Assembly;

            TestRunnerManager.OnTestRunEnd(currentAssembly);
        }
    }
}
