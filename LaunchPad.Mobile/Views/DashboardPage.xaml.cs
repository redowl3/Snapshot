using FormsControls.Base;
using LaunchPad.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace LaunchPad.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardPage : AnimationPage
    {
        public DashboardPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.BindingContext = new DashboardPageViewModel();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.BindingContext = null;
        }
    }
}