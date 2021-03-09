using FormsControls.Base;
using IIAADataModels.Transfer;
using IIAADataModels.Transfer.Survey;
using LaunchPad.Client;
using LaunchPad.Mobile.Helpers;
using LaunchPad.Mobile.Models;
using LaunchPad.Mobile.Services;
using LaunchPad.Mobile.Views;
using Newtonsoft.Json;
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
    public class LifestylesSurveyViewModel : ViewModelBase
    {
        public static Action UpdateSurveyReview;
        public static void OnUpdateSurveyReview()
        {
            UpdateSurveyReview?.Invoke();
        }
        public static Action Next;
        public static void OnNext()
        {
            Next?.Invoke();
        }
        public int Counter { get; set; }
        public int MaxCounter { get; set; }
        private IDatabaseServices DatabaseServices => DependencyService.Get<IDatabaseServices>();
        private ObservableCollection<IndexedQuestions> _lifeStylesQuestions = new ObservableCollection<IndexedQuestions>();
        public ObservableCollection<IndexedQuestions> LifeStylesQuestions
        {
            get => _lifeStylesQuestions;
            set => SetProperty(ref _lifeStylesQuestions, value);
        }

        private FlexBasis _basis = new FlexBasis(1f, true);
        public FlexBasis Basis
        {
            get => _basis;
            set => SetProperty(ref _basis, value);
        }


        private List<IndexModel> _surveyIndexList = new List<IndexModel>();
        public List<IndexModel> SurveyIndexList
        {
            get => _surveyIndexList;
            set => SetProperty(ref _surveyIndexList, value);
        }
        private bool _isDone;
        public bool IsDone
        {
            get => _isDone;
            set => SetProperty(ref _isDone, value);
        }
        private ObservableCollection<SurveySummary> surveySummaries=new ObservableCollection<SurveySummary>();
        public ObservableCollection<SurveySummary> SurveySummaries
        {
            get => surveySummaries;
            set => SetProperty(ref surveySummaries, value);
        }
        public ICommand NextCommand => new Command<List<SurveySummary>>(async(param) =>
        {
            try
            {
                if (Counter < MaxCounter)
                {
                    LifeStylesQuestions[Counter].IsSelected = false;
                    ++Counter;
                    if (LifeStylesQuestions[Counter].Questions?.Count == 3)
                    {
                        Basis = new FlexBasis(0.333f, true);
                    }
                    else if (LifeStylesQuestions[Counter].Questions?.Count == 2)
                    {
                        Basis = new FlexBasis(0.5f, true);
                    }
                    else if (LifeStylesQuestions[Counter].Questions?.Count == 1)
                    {
                        Basis = new FlexBasis(1f, true);
                    }
                    LifeStylesQuestions[Counter].IsSelected = true;
                    

                    if (SurveySummaries == null)
                    {
                        SurveySummaries = new ObservableCollection<SurveySummary>();
                    }

                    foreach (var item in param)
                    {
                        var surveySummary = SurveySummaries.First(a => a.QuestionGuid == item.QuestionGuid);
                        if (surveySummary == null)
                        {
                            SurveySummaries.Add(item);
                        }
                        else
                        {
                            SurveySummaries.Where(a => a.QuestionGuid == item.QuestionGuid).ForEach(x =>
                            {
                                x.AnswerText = item.AnswerText;
                                x.ConfigAnswerText = item.ConfigAnswerText;
                            });
                        }
                        
                    }
                }
                else
                {
                    await Task.Run(async () =>
                    {
                        var surveResponse = SurveySummaries.GroupBy(a => a.QuestionGuid).Select(a => new FormResponse
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            FormId = SplashPageViewModel.LifeStyleFormId,
                            Version = SplashPageViewModel.LifestyleFormVersion,
                            Answers = a.Distinct().Select(x => new FormQuestionResponse
                            {
                                QuestionId = new Guid(a.Key),
                                Answer = string.Join("|", param.Where(t => t.QuestionGuid == a.Key).Select(t => string.IsNullOrEmpty(t.SubAnswerText) ? t.AnswerText : string.IsNullOrEmpty(t.ConfigAnswerText) ? t.AnswerText + "-" + t.SubAnswerText : t.AnswerText + "-" + t.SubAnswerText + "-" + t.ConfigAnswerText)),
                                Notes=x.Notes
                            }).ToList().Take(1).ToList()
                        });

                        var dbSurveyResponse = await DatabaseServices.Get<List<FormResponse>>("SurveyResponse");
                        dbSurveyResponse.AddRange(surveResponse);
                        await DatabaseServices.InsertData<List<FormResponse>>("SurveyResponse", dbSurveyResponse);
                        var surveyReview= await DatabaseServices.Get<SurveyOverView>("SurveyOverView" + Settings.ClientId);
                    });

                    
                    Next?.Invoke();
                }

                UpdateSurveyReview?.Invoke();
            }
            catch (Exception)
            {

            }
        });
        
       

        public LifestylesSurveyViewModel()
        {
            GetConcernsQuestionsAsync();
        }
        public void GetConcernsQuestionsAsync()
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (LifeStylesQuestions != null || LifeStylesQuestions.Count == 0)
                    {
                        foreach (var survey in App.surveyPageViewModelInstance.SurveyCollection.Where(a => a.Form.Title.ToLower() == "you + your lifestyle"))
                        {
                            foreach (var page in survey.Form.Pages)
                            {
                                var questions = await DatabaseServices.Get<List<CustomFormQuestion>>("survey_page" + page.Id + "_" + survey.Form.Id);

                                LifeStylesQuestions.Add(new IndexedQuestions
                                {
                                    SurveyGuid = survey.Form.Id,
                                    PageGuid = page.Id,
                                    Questions = new ObservableCollection<CustomFormQuestion>(questions)
                                });
                            }
                            Counter = 0;
                            MaxCounter = survey.Form.Pages.Count - 1;
                        }
                    }
                    LifeStylesQuestions[0].IsSelected = true;
                });
            }
            catch (Exception)
            {

            }
        }
    }
}
