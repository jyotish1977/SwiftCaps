using NUnit.Framework;
using SwiftCAPS.Mobile.UITest.Features;
using Xamarin.UITest;

namespace SwiftCAPS.Mobile.UITest.Tests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public partial class LaunchApplicationFeature : BaseLocalFeature
    {
        public LaunchApplicationFeature(Platform platform) : base(platform)
        {

        }
    }
}
