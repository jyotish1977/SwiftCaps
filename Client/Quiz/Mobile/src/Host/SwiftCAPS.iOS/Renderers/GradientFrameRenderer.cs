using CoreAnimation;
using CoreGraphics;
using SwiftCaps.Controls;
using SwiftCaps.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(GradientFrame), typeof(GradientFrameRenderer))]
namespace SwiftCaps.iOS.Renderers
{
    public class GradientFrameRenderer : VisualElementRenderer<Frame>
    {
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            var frame = (GradientFrame)Element;

            var startColor = frame.StartColor.ToCGColor();
            var endColor = frame.EndColor.ToCGColor();

            var gradientLayer = new CAGradientLayer {Frame = rect, Colors = new[] {startColor, endColor}};

            NativeView.Layer.InsertSublayer(gradientLayer, 0);
        }
    }
}