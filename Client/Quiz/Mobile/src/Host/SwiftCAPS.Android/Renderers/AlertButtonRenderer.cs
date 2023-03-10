using Android.Content;
using Android.Graphics.Drawables;
using Xamariners.Mobile.Core.Controls;
using SwiftCaps.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ButtonRenderer = Xamarin.Forms.Platform.Android.AppCompat.ButtonRenderer;

[assembly: ExportRenderer(typeof(AlertButton), typeof(AlertButtonRenderer))]
namespace SwiftCaps.Droid.Renderers
{
    public class AlertButtonRenderer : ButtonRenderer
    {
        public AlertButtonRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Control is null) return;

            Control.SetBackgroundResource(SwiftCAPS.Droid.Resource.Drawable.DefaultButtonShape);

            var bg = (GradientDrawable)Control.Background;
            bg.SetColor(e.NewElement.BackgroundColor.ToAndroid());
        }
    }

}