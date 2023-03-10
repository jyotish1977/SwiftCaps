using Xamarin.UITest;
using Xamariners.EndToEnd.Xamarin.Features;
using Xamariners.EndToEnd.Xamarin.Infrastructure;

namespace SwiftCAPS.Mobile.UITest.Infrastructure
{
    public class BaseAppInitializer : IBaseAppInitializer
    {
        public IApp StartApp(Platform platform)
        {
            return AppInitializer.StartApp(platform);
        }

        public IApp StartApp(Platform platform, RunnerConfiguration configuration)
        {
            return AppInitializer.StartApp(platform, configuration);
        }
    }
}
