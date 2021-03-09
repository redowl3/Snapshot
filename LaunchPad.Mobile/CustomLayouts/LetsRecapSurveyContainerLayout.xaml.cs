
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LaunchPad.Mobile.CustomLayouts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LetsRecapSurveyContainerLayout : ContentView
    {
        public LetsRecapSurveyContainerLayout()
        {
            InitializeComponent();
        }

        private void signatureView_Focused(object sender, FocusEventArgs e)
        {
            PlaceholderContainer.IsVisible = false;
        }

        private void SaveAndContinue(object sender, System.EventArgs e)
        {

        }

        private void signaturechanged(object sender, System.EventArgs e)
        {
            PlaceholderContainer.IsVisible = false;
        }
    }
}