using Android.Content;
using Android.Graphics.Drawables;
using SwiftCaps.Controls;
using SwiftCaps.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ButtonRenderer = Xamarin.Forms.Platform.Android.AppCompat.ButtonRenderer;

[assembly: ExportRenderer(typeof(DefaultButton), typeof(DefaultButtonRenderer))]
namespace SwiftCaps.Droid.Renderers
{
    public class DefaultButtonRenderer : ButtonRenderer
    {
        public DefaultButtonRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Control is null) return;

            Control.SetBackgroundResource(SwiftCAPS.Droid.Resource.Drawable.DefaultButtonShape);

            var bg = (GradientDrawable) Control.Background;
            bg.SetColor(e.NewElement.BackgroundColor.ToAndroid());
            
        }
    }
}