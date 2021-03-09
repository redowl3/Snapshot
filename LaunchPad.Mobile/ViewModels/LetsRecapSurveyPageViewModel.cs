using IIAADataModels.Transfer;
using IIAADataModels.Transfer.Survey;
using LaunchPad.Client;
using LaunchPad.Mobile.Helpers;
using LaunchPad.Mobile.Models;
using LaunchPad.Mobile.Services;
using LaunchPad.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
namespace LaunchPad.Mobile.ViewModels
{
    public class LetsRecapSurveyPageViewModel : ViewModelBase
    {
        private IDatabaseServices DatabaseServices => DependencyService.Get<IDatabaseServices>();
        private IToastServices ToastServices => DependencyService.Get<IToastServices>();

        private string _pageTitle;
        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }
        private bool _page1Visible;
        public bool Page1Visible
        {
            get => _page1Visible;
            set => SetProperty(ref _page1Visible, value);
        }
        private string _skinGoal;
        public string SkinGoal
        {
            get => _skinGoal;
            set => SetProperty(ref _skinGoal, value);
        }

        private bool _page2Visible;
        public bool Page2Visible
        {
            get => _page2Visible;
            set => SetProperty(ref _page2Visible, value);
        }

        private bool _page3Visible;
        public bool Page3Visible
        {
            get => _page3Visible;
            set => SetProperty(ref _page3Visible, value);
        }
        private bool _healthSummaryVisible;
        public bool HealthSummaryVisible
        {
            get => _healthSummaryVisible;
            set => SetProperty(ref _healthSummaryVisible, value);
        }
        private ObservableCollection<SurveySummary> healthSurveySummaries;
        public ObservableCollection<SurveySummary> HealthSurveySummaries
        {
            get => healthSurveySummaries;
            set => SetProperty(ref healthSurveySummaries, value);
        }
        private bool _concernSurveySummaryVisible;
        public bool ConcernSurveySummaryVisible
        {
            get => _concernSurveySummaryVisible;
            set => SetProperty(ref _concernSurveySummaryVisible, value);
        }
        private ObservableCollection<SurveySummary> concernSurveySummaries;
        public ObservableCollection<SurveySummary> ConcernSurveySummaries
        {
            get => concernSurveySummaries;
            set => SetProperty(ref concernSurveySummaries, value);
        }
        private bool _lifestyleSummaryVisible;
        public bool LifestyleSummaryVisible
        {
            get => _lifestyleSummaryVisible;
            set => SetProperty(ref _lifestyleSummaryVisible, value);
        }
        private ObservableCollection<SurveyOverView> lifeStyleSurveySummaries = new ObservableCollection<SurveyOverView>();
        public ObservableCollection<SurveyOverView> LifeStyleSurveySummaries
        {
            get => lifeStyleSurveySummaries;
            set => SetProperty(ref lifeStyleSurveySummaries, value);
        }
        private bool _dietSummaryVisible;
        public bool DietSummaryVisible
        {
            get => _dietSummaryVisible;
            set => SetProperty(ref _dietSummaryVisible, value);
        }
        private ObservableCollection<SurveyOverView> dietSurveySummaries = new ObservableCollection<SurveyOverView>();
        public ObservableCollection<SurveyOverView> DietSurveySummaries
        {
            get => dietSurveySummaries;
            set => SetProperty(ref dietSurveySummaries, value);
        }
        public ICommand Page1CompletedCommand => new Command(() =>
          {
              Page1Visible = false;
              Page2Visible = true;
              Page3Visible = false;
          });
        public ICommand Page2CompletedCommand => new Command(() =>
          {
              Page1Visible = false;
              Page2Visible = false;
              Page3Visible = true;
          });
        public ICommand Page3CompletedCommand => new Command(() =>
          {
              PostSurveyResponseAsync();
          });

        public LetsRecapSurveyPageViewModel()
        {
            PageTitle = $"{Settings.ClientFirstName}'s consultation overview";
            Page1Visible = true;
            Page2Visible = false;
            Task.Run(() =>
            {
                GetConsultationOverView();
            });
            HealthQuestionsSurveyViewModel.UpdateSurveyReview += GetConsultationOverView;
            ConcernsAndSkinCareSurveyViewModel.UpdateSurveyReview += GetConsultationOverView;
            LifestylesSurveyViewModel.UpdateSurveyReview += GetConsultationOverView;
        }

        private async void GetConsultationOverView()
        {
            try
            {
                var surveyReviews = await DatabaseServices.Get<List<SurveyOverView>>("SurveyOverView" + Settings.ClientId);
                var healthSurveyOverview = surveyReviews.FirstOrDefault(a => a.Title.ToLower() == "health");
                HealthSummaryVisible = healthSurveyOverview?.SurveySummaries?.Count > 0;
                if (HealthSummaryVisible)
                    HealthSurveySummaries = new ObservableCollection<SurveySummary>(healthSurveyOverview.SurveySummaries);
                var concernSurveyOverview = surveyReviews.FirstOrDefault(a => a.Title.ToLower() == "skin type");
                ConcernSurveySummaryVisible = concernSurveyOverview?.SurveySummaries?.Count > 0;
                if (ConcernSurveySummaryVisible)
                    ConcernSurveySummaries = new ObservableCollection<SurveySummary>(concernSurveyOverview.SurveySummaries);
                var lifeStyleOverview = surveyReviews.FirstOrDefault(a => a.Title.ToLower() == "you and your lifestyle");
                LifestyleSummaryVisible = lifeStyleOverview?.SurveySummaries?.Count > 0;
                if (LifestyleSummaryVisible)
                    LifeStyleSurveySummaries = new ObservableCollection<SurveyOverView>(surveyReviews.Where(a => a.Title.ToLower() == "you and your lifestyle"));
                var dietStyleOverview = surveyReviews.FirstOrDefault(a => a.Title.ToLower() == "diet");
                DietSummaryVisible = dietStyleOverview?.SurveySummaries?.Count > 0;
                if (DietSummaryVisible)
                    DietSurveySummaries = new ObservableCollection<SurveyOverView>(surveyReviews.Where(a => a.Title.ToLower() == "diet"));
            }
            catch (Exception)
            {

            }
        }
        private async void PostSurveyResponseAsync()
        {
            try
            {
                var dbSurveyResponse = await DatabaseServices.Get<List<FormResponse>>("SurveyResponse");
                var consumer = await DatabaseServices.Get<Consumer>("current_consumer" + Settings.CurrentTherapistId);
                if (consumer != null && consumer.Id != Guid.Empty)
                {
                    var list = new List<FormResponse>();
                    foreach (var item in dbSurveyResponse)
                    {
                        if (list.Count(a => a.FormId == item.FormId) == 0)
                        {
                            var formResponse = new FormResponse
                            {
                                Id = Guid.NewGuid(),
                                Created = DateTime.Now,
                                Version = item.Version,
                                FormId = item.FormId
                            };
                            formResponse.Answers = new List<FormQuestionResponse>();
                            formResponse.Answers.AddRange(item.Answers);
                            list.Add(formResponse);
                        }
                        else
                        {
                            list.Where(a => a.FormId == item.FormId).ForEach(x => x.Answers.AddRange(item.Answers));
                        }
                    }
                    var saloConsumer = new SalonConsumer
                    {
                        Id = consumer.Id,
                        Firstname = consumer.Firstname,
                        Lastname = consumer.Lastname,
                        Email = consumer.Email,
                        Mobile = consumer.Mobile,
                        TherapistId = new Guid(Settings.CurrentTherapistId),
                        DateOfBirth = consumer.DateOfBirth,
                        CurrentConsultation = new Consultation
                        {
                            Id = Guid.NewGuid(),
                            SurveyResponses = new List<FormResponse>(list)
                        }
                    };

                    var isCompleted = await ApiServices.Client.PostAsync<bool>("salon/consumer/consultation", saloConsumer);
                    if (isCompleted)
                    {
                        await Task.Run(async () =>
                        {
                            await DatabaseServices.InsertData<List<FormResponse>>("survey_response_" + consumer.Id + "_" + Settings.CurrentTherapistId, list);
                            App.ConcernsAndSkinCareSurveyViewModel = new ConcernsAndSkinCareSurveyViewModel();
                            App.HealthQuestionsSurveyViewModel = new HealthQuestionsSurveyViewModel();
                            App.LifestylesSurveyViewModel = new LifestylesSurveyViewModel();
                            await SecureStorage.SetAsync("SurveyDone" + Settings.ClientId, "true");
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Application.Current.MainPage.Navigation.PushAsync(new SalonProductsPage());
                                Application.Current.MainPage.Navigation.PopModalAsync();
                            });
                        });
                    }
                    else
                    {
                        ToastServices.ShowToast("Survey Post request failed");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
