using FormsControls.Base;
using LaunchPad.Mobile.ViewModels;
using Xamarin.Forms.Xaml;

namespace LaunchPad.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserHistoryPage : AnimationPage
    {
        public UserHistoryPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.BindingContext = new UserHistoryPageViewModel();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.BindingContext = null;
        }
    }
}