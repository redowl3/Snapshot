using FormsControls.Base;
using LaunchPad.Mobile.Models;
using LaunchPad.Mobile.ViewModels;
using Xamarin.Forms.Xaml;
namespace LaunchPad.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConsultationSummaryPage : AnimationPage
    {
        private UserActivity param;

        public ConsultationSummaryPage()
        {
            InitializeComponent();
        }

        public ConsultationSummaryPage(UserActivity param)
        {
            this.param = param;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.BindingContext = new ConsultationSummaryPageViewModel(param);
        }
    }
}