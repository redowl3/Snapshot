using Android.Content;
using LaunchPad.Mobile.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
[assembly: ExportRenderer(typeof(Editor), typeof(BorderlessEditorRenderer))]
namespace LaunchPad.Mobile.Droid.CustomRenderers
{
    public class BorderlessEditorRenderer : EditorRenderer
    {
        public BorderlessEditorRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            if (Control == null) return;
            Control.Background = null;
            Control.SetTextSize(Android.Util.ComplexUnitType.Dip, (float)e.NewElement.FontSize);
            Control.SetPadding(15, PaddingTop, PaddingRight, PaddingBottom);
        }
    }
}