using Xamarin.Forms;

namespace SwiftCaps.Helpers
{
    public static class FontSizeHelpers
    {
        public static double TitleFontSize
        {
            get
            {
                var fontSize = 20;

                // TODO: use DI as test ain't like it

                if (Device.RuntimePlatform == "Test") return fontSize;
#if !FAKE
                var density = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density;
               
                if (Device.RuntimePlatform == Device.iOS)
                {
                    switch (density)
                    {
                        case 2:
                            fontSize = 30;
                            break;
                        case 3:
                            fontSize = 38;
                            break;
                    }
                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    switch (density)
                    {
                        case 2:
                            fontSize = 36;
                            break;
                        case 3:
                            fontSize = 40;
                            break;
                        case 3.5:
                            fontSize = 44;
                            break;
                    }
                }

#endif
                return fontSize;
            }
        }
    }
}