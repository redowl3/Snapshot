using FormsControls.Base;
using LaunchPad.Mobile.Models;
using LaunchPad.Mobile.ViewModels;
using LaunchPad.Mobile.Views;
using System.Windows.Input;
using Xamarin.Forms;

namespace LaunchPad.Mobile.ViewModels
{
    public class ConsultationSummaryPageViewModel : ViewModelBase
    {
        private UserActivity _userActivity;
        public UserActivity UserActivity
        {
            get => _userActivity;
            set => SetProperty(ref _userActivity, value);
        }
        public ICommand GoBackCommand => new Command(() => Application.Current.MainPage.Navigation.PopAsync());
        public ICommand HomeCommand => new Command(() => Application.Current.MainPage.Navigation.PopToRootAsync());
        public ConsultationSummaryPageViewModel(UserActivity param)
        {
            UserActivity = param;
        }
    }
}
