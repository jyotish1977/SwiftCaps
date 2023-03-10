using Xamarin.Forms;

namespace SwiftCaps.Controls
{
    public class GradientFrame : Frame
    {
        public static readonly BindableProperty EndColorProperty = BindableProperty.Create(
            nameof(EndColor),
            typeof(Color),
            typeof(GradientFrame),
            default(Color));

        public Color StartColor { get; set; }
        public Color EndColor
        {
            get => (Color)GetValue(EndColorProperty);
            set => SetValue(EndColorProperty, value);
        }

    }
}