using System;
using Android.Content;
using SwiftCaps.Controls;
using SwiftCaps.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(GradientFrame), typeof(GradientFrameRenderer))]
namespace SwiftCaps.Droid.Renderers
{
    public class GradientFrameRenderer : VisualElementRenderer<Frame>
    {
        public GradientFrameRenderer(Context context) : base(context)
        {

        }

        private Color StartColor { get; set; }
        private Color EndColor { get; set; }

        protected override void DispatchDraw(Android.Graphics.Canvas canvas)
        {

            var gradient = new Android.Graphics.LinearGradient(0, 0, 0, Height,
            StartColor.ToAndroid(),
                EndColor.ToAndroid(),
                Android.Graphics.Shader.TileMode.Mirror);

            var paint = new Android.Graphics.Paint()
            {
                Dither = true,
            };
            paint.SetShader(gradient);
            canvas.DrawPaint(paint);
            base.DispatchDraw(canvas);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null) return;

            try
            {
                if (!(e.NewElement is GradientFrame stack)) return;

                StartColor = stack.StartColor;
                EndColor = stack.EndColor;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"ERROR:", ex.Message);
            }
        }
    }
}