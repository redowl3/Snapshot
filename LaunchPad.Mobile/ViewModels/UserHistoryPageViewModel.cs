using FormsControls.Base;
using LaunchPad.Mobile.Helpers;
using LaunchPad.Mobile.Models;
using LaunchPad.Mobile.Services;
using LaunchPad.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace LaunchPad.Mobile.ViewModels
{
    public class UserHistoryPageViewModel:ViewModelBase
    {
        private IDatabaseServices DatabaseServices => DependencyService.Get<IDatabaseServices>();
        private ObservableCollection<UserActivityByYear> userActivities;
        public ObservableCollection<UserActivityByYear> UserActivities
        {
            get => userActivities;
            set => SetProperty(ref userActivities, value);
        }
        public ICommand GoBackCommand => new Command(() => Application.Current.MainPage.Navigation.PopAsync() );
        public ICommand HomeCommand => new Command(() => Application.Current.MainPage.Navigation.PopToRootAsync());
        public ICommand ViewConsultationCommand => new Command<UserActivity>((param) => Application.Current.MainPage.Navigation.PushAsync(new ConsultationSummaryPage(param)));
        public UserHistoryPageViewModel()
        {
            UserActivities = new ObservableCollection<UserActivityByYear>();
            FetchUserActivities();
        }

        private async void FetchUserActivities()
        {
            try
            {
                var userHistory= await DatabaseServices.Get<List<UserActivity>>("userhistory"+Settings.ClientId);
                //UserActivities = new ObservableCollection<UserActivity>(userHistory);
                UserActivities =new ObservableCollection<UserActivityByYear>(userHistory.GroupBy(a => a.PerformedOn.Year).Select(x=>new UserActivityByYear
                {
                    Year=x.Key,
                    UserActivities=new List<UserActivity>(x.Select(a=>a))
                }));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
