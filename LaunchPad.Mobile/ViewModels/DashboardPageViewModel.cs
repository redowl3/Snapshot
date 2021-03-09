using FormsControls.Base;
using LaunchPad.Mobile.Helpers;
using LaunchPad.Mobile.Models;
using LaunchPad.Mobile.Services;
using LaunchPad.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
namespace LaunchPad.Mobile.ViewModels
{
    public class DashboardPageViewModel : ViewModelBase
    {
        private IDatabaseServices DatabaseServices => DependencyService.Get<IDatabaseServices>();
        private SurveyPage _surveyPageInstance;
        public SurveyPage SurveyPageInstance
        {
            get
            {
                if (_surveyPageInstance == null)
                {
                    _surveyPageInstance = new SurveyPage();
                }

                return _surveyPageInstance;
            }          
        } 
        private SalonProductsPage _salonProductsPage;
        public SalonProductsPage SalonProductsPage
        {
            get => _salonProductsPage;
            set => SetProperty(ref _salonProductsPage, value);            
        }
        private ObservableCollection<CustomActivity> userActivities;
        public ObservableCollection<CustomActivity> UserActivities
        {
            get => userActivities;
            set => SetProperty(ref userActivities, value);
        }
        public ICommand ConsultationCommand => new Command(() =>
        {
            Task.Run(() =>
            {
                DatabaseServices.Delete<List<SurveyOverView>>("SurveyOverView" + Settings.ClientId);
                Device.BeginInvokeOnMainThread(async() =>
                {
                    await Application.Current.MainPage.Navigation.PushAsync(SurveyPageInstance);
                });
            });
        });
        public ICommand ViewHistoryCommand => new Command(() =>
        {
            Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage.Navigation.PushAsync(new UserHistoryPage());
                });
            });
        });
        public ICommand GoBackCommand => new Command(() => Application.Current.MainPage.Navigation.PopAsync());
        //public ICommand HomeCommand => new Command(() => Application.Current.MainPage = new AnimationNavigationPage(new SalonClientsPage()));
        public ICommand HomeCommand => new Command(() => Application.Current.MainPage.Navigation.PopToRootAsync());
        public DashboardPageViewModel()
        {
            FetchUserActivities();
        }

        private void FetchUserActivities()
        {
            try
            {
                Task.Run(() =>
                {
                    
                    Device.BeginInvokeOnMainThread(async() =>
                    {
                        UserActivities = new ObservableCollection<CustomActivity>();
                        var userHistory = await DatabaseServices.Get<List<UserActivity>>("userhistory"+Settings.ClientId);
                        foreach (var item in userHistory)
                        {
                            foreach (var colItem in item.Activity.Consultations.ItemsCollection)
                            {
                                UserActivities.Add(new CustomActivity
                                {
                                    PerformedOn = item.PerformedOn,
                                    Product = colItem.Product
                                });
                            }
                        }
                    });
                  
                });
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public class CustomActivity
    {
        public DateTime PerformedOn { get; set; }
        public CustomProduct Product { get; set; }
    }
}
