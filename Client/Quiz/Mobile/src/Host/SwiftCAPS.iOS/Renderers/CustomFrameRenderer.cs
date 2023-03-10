using CoreAnimation;
using CoreGraphics;
using Xamariners.Mobile.Core.Controls;
using SwiftCaps.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly: ExportRenderer(typeof(CustomFrame), typeof(CustomFrameRenderer))]
namespace SwiftCaps.iOS.Renderers
{
    public class CustomFrameRenderer : VisualElementRenderer<Frame>
    {
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            var frame = (CustomFrame)Element;

            var startColor = frame.StartColor.ToCGColor();
            var endColor = frame.EndColor.ToCGColor();

            var gradientLayer = new CAGradientLayer { Frame = rect, Colors = new[] { startColor, endColor } };
            gradientLayer.ShadowOffset = new CGSize(0f, 2f);
            gradientLayer.ShadowColor = new UIColor(0f, 1f).CGColor;

            NativeView.Layer.InsertSublayer(gradientLayer, 0);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            UpdateCornerRadius();
        }

        private double RetrieveCommonCornerRadius(CornerRadius cornerRadius)
        {
            var commonCornerRadius = cornerRadius.TopLeft;
            if (commonCornerRadius <= 0)
            {
                commonCornerRadius = cornerRadius.TopRight;
                if (commonCornerRadius <= 0)
                {
                    commonCornerRadius = cornerRadius.BottomLeft;
                    if (commonCornerRadius <= 0)
                    {
                        commonCornerRadius = cornerRadius.BottomRight;
                    }
                }
            }

            return commonCornerRadius;
        }

        private UIRectCorner RetrieveRoundedCorners(CornerRadius cornerRadius)
        {
            var roundedCorners = default(UIRectCorner);

            if (cornerRadius.TopLeft > 0)
            {
                roundedCorners |= UIRectCorner.TopLeft;
            }

            if (cornerRadius.TopRight > 0)
            {
                roundedCorners |= UIRectCorner.TopRight;
            }

            if (cornerRadius.BottomLeft > 0)
            {
                roundedCorners |= UIRectCorner.BottomLeft;
            }

            if (cornerRadius.BottomRight > 0)
            {
                roundedCorners |= UIRectCorner.BottomRight;
            }

            return roundedCorners;
        }

        private void UpdateCornerRadius()
        {
            var cornerRadius = (Element as CustomFrame)?.CornerRadius;
            if (!cornerRadius.HasValue)
            {
                return;
            }

            var roundedCornerRadius = RetrieveCommonCornerRadius(cornerRadius.Value);
            if (roundedCornerRadius <= 0)
            {
                return;
            }

            var roundedCorners = RetrieveRoundedCorners(cornerRadius.Value);

            var path = UIBezierPath.FromRoundedRect(Bounds, roundedCorners, new CGSize(roundedCornerRadius, roundedCornerRadius));
            var mask = new CAShapeLayer { Path = path.CGPath };
            NativeView.Layer.Mask = mask;
            this.ClipsToBounds = true;
        }
    }
}