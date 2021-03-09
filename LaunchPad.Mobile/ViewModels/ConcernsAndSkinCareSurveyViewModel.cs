using IIAADataModels.Transfer.Survey;
using LaunchPad.Mobile.Helpers;
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
    public class ConcernsAndSkinCareSurveyViewModel : ViewModelBase
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
        private ObservableCollection<IndexedQuestions> _concernAndSkinCareQuestions = new ObservableCollection<IndexedQuestions>();
        public ObservableCollection<IndexedQuestions> ConcernAndSkinCareQuestions
        {
            get => _concernAndSkinCareQuestions;
            set => SetProperty(ref _concernAndSkinCareQuestions, value);
        }

        private FlexBasis _basis = new FlexBasis(1f, true);
        public FlexBasis Basis
        {
            get => _basis;
            set => SetProperty(ref _basis, value);
        }

        private bool _page1;
        public bool Page1
        {
            get => _page1;
            set => SetProperty(ref _page1, value);
        }
        private List<IndexModel> _surveyIndexList = new List<IndexModel>();
        public List<IndexModel> SurveyIndexList
        {
            get => _surveyIndexList;
            set => SetProperty(ref _surveyIndexList, value);
        }
        private ObservableCollection<SurveySummary> surveySummaries = new ObservableCollection<SurveySummary>();
        public ObservableCollection<SurveySummary> SurveySummaries
        {
            get => surveySummaries;
            set => SetProperty(ref surveySummaries, value);
        }
        public ICommand ContinueCommand => new Command<List<SurveySummary>>(async(param) =>
          {
              try
              {
                  UpdateSurveyReview?.Invoke();
                  if (Counter < MaxCounter)
                  {
                      ConcernAndSkinCareQuestions[Counter].IsSelected = false;
                      ++Counter;
                      if (ConcernAndSkinCareQuestions[Counter].Questions?.Count == 3)
                      {
                          Basis = new FlexBasis(0.333f, true);
                      }
                      else if (ConcernAndSkinCareQuestions[Counter].Questions?.Count == 2)
                      {
                          Basis = new FlexBasis(0.5f, true);
                      }
                      else if (ConcernAndSkinCareQuestions[Counter].Questions?.Count == 1)
                      {
                          Basis = new FlexBasis(1f, true);
                      }
                      Page1 = false;
                      ConcernAndSkinCareQuestions[Counter].IsSelected = true;
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
                                  x.ImageUrl = item.ImageUrl;
                                  x.QuestionText = item.QuestionText;
                              });
                          }

                      }
                  }
                  else
                  {
                      await Task.Run(async () =>
                      {
                          var surveResponse = param.GroupBy(a => a.QuestionGuid).Select(a => new FormResponse
                          {
                              Id = Guid.NewGuid(),
                              Created = DateTime.Now,
                              FormId = SplashPageViewModel.ConcernsFormId,
                              Version = SplashPageViewModel.ConcernsFormVersion,
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
                      });
                      Next?.Invoke();
                  }
              }
              catch (Exception)
              {

              }
             
          });
        public ConcernsAndSkinCareSurveyViewModel()
        {
            GetConcernsQuestionsAsync();
            //SurveyPage.StartCounter += StartCounter;
        }

        private void StartCounter()
        {
            GetConcernsQuestionsAsync();
        }

        public void GetConcernsQuestionsAsync()
        {
            try
            {
                if (ConcernAndSkinCareQuestions != null || ConcernAndSkinCareQuestions.Count == 0)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                {

                    foreach (var survey in App.surveyPageViewModelInstance.SurveyCollection.Where(a => a.Form.Title.ToLower() == "concerns + skin type"))
                    {
                        foreach (var page in survey.Form.Pages)
                        {
                            var questions = await DatabaseServices.Get<List<CustomFormQuestion>>("survey_page" + page.Id + "_" + survey.Form.Id);

                            ConcernAndSkinCareQuestions.Add(new IndexedQuestions
                            {
                                SurveyGuid = survey.Form.Id,
                                PageGuid = page.Id,
                                Questions = new ObservableCollection<CustomFormQuestion>(questions)
                            });
                        }
                        Counter = 0;
                        MaxCounter = survey.Form.Pages.Count - 1;
                    }
                    Page1 = true;
                    if (ConcernAndSkinCareQuestions[0].Questions?.Count == 3)
                    {
                        Basis = new FlexBasis(0.333f, true);
                    }
                    else if (ConcernAndSkinCareQuestions[0].Questions?.Count == 2)
                    {
                        Basis = new FlexBasis(0.5f, true);
                    }
                    else if (ConcernAndSkinCareQuestions[0].Questions?.Count == 1)
                    {
                        Basis = new FlexBasis(1f, true);
                    }
                    ConcernAndSkinCareQuestions[0].IsSelected = true;
                  
                });

                    //Task.Run(async () =>
                    //{
                    //    foreach (var survey in App.surveyPageViewModelInstance.SurveyCollection.Where(a => a.Form.Title.ToLower() == "concerns + skin type"))
                    //    {
                    //        foreach (var page in survey.Form.Pages.Skip(1))
                    //        {
                    //            var questions = await DatabaseServices.Get<List<CustomFormQuestion>>("survey_page" + page.Id + "_" + survey.Form.Id);

                    //            ConcernAndSkinCareQuestions.Add(new IndexedQuestions
                    //            {
                    //                SurveyGuid = survey.Form.Id,
                    //                PageGuid = page.Id,
                    //                Questions = new ObservableCollection<CustomFormQuestion>(questions)
                    //            });
                    //        }
                    //    }
                    //    Counter = 0;
                    //    MaxCounter = ConcernAndSkinCareQuestions.Count - 1;
                    //});
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
