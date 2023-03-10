using Xamarin.UITest;

namespace SwiftCAPS.Mobile.UITest.Features
{
    public class BaseLocalFeature : Xamariners.EndToEnd.Xamarin.Features.FeatureBase
    {
        public BaseLocalFeature(Platform platform) : base(platform)
        {
#if __LOCAL__
            ConfigurationFile = "testsConfiguration.json";
#endif
            BaseAppInitializer = new Infrastructure.BaseAppInitializer();
        }
    }
}
