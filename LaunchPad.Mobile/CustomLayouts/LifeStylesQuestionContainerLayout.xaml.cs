using LaunchPad.Mobile.Helpers;
using LaunchPad.Mobile.Models;
using LaunchPad.Mobile.Services;
using LaunchPad.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace LaunchPad.Mobile.CustomLayouts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LifeStylesQuestionContainerLayout : ContentView
    {
        private IDatabaseServices DatabaseServices => DependencyService.Get<IDatabaseServices>();
        private List<SurveySummary> SurveySummaries = new List<SurveySummary>();
        public string QuestionGuid { get; set; }
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }
        public string SubAnswerText { get; set; }
        public LifeStylesQuestionContainerLayout()
        {
            InitializeComponent();
            this.BindingContext = App.LifestylesSurveyViewModel;
        }

        private async void SaveAndContinue(object sender, EventArgs e)
        {
            var bindingContext = (this.BindingContext as LifestylesSurveyViewModel);
            if (bindingContext != null)
            {
                var surveyReviews = await DatabaseServices.Get<List<SurveyOverView>>("SurveyOverView" + Settings.ClientId);
                if (bindingContext.Counter < bindingContext.MaxCounter)
                {
                    if (surveyReviews.Count(a => a.Title.ToLower() == "you and your lifestyle") > 0)
                    {
                        surveyReviews.Where(a => a.Title.ToLower() == "you and your lifestyle").ForEach(x =>
                        {
                            foreach (var item in SurveySummaries)
                            {
                                if (x.SurveySummaries.Count(t => t.AnswerText.ToLower() == item.AnswerText.ToLower()) == 0)
                                {
                                    x.SurveySummaries.Add(item);
                                }
                            }
                            
                        });
                    }
                    else
                    {
                        var surveyOverView = new SurveyOverView();
                        surveyOverView.Title = "You and your lifestyle";
                        surveyOverView.SurveySummaries = new List<SurveySummary>(SurveySummaries);
                        surveyReviews.Add(surveyOverView);
                    }

                    await DatabaseServices.InsertData("SurveyOverView" + Settings.ClientId, surveyReviews);
                }
                else
                {
                    if (surveyReviews.Count(a => a.Title.ToLower() == "diet") > 0)
                    {
                        surveyReviews.Where(a => a.Title.ToLower() == "diet").ForEach(x =>
                        {
                            foreach (var item in SurveySummaries)
                            {
                                if (x.SurveySummaries.Count(t => t.AnswerText.ToLower() == item.AnswerText.ToLower()) == 0)
                                {
                                    x.SurveySummaries.Add(item);
                                }
                            }
                        });
                    }
                    else
                    {
                        var surveyOverView = new SurveyOverView();
                        surveyOverView.Title = "Diet";
                        surveyOverView.SurveySummaries = new List<SurveySummary>(SurveySummaries);
                        surveyReviews.Add(surveyOverView);
                    }

                    await DatabaseServices.InsertData("SurveyOverView" + Settings.ClientId, surveyReviews);
                }
            }
            (this.BindingContext as LifestylesSurveyViewModel).NextCommand.Execute(SurveySummaries);
            SurveySummaries = new List<SurveySummary>();
        }

        private void OptionTapped(object sender, EventArgs e)
        {
            try
            {
                var senderGrid = ((Grid)sender);
                var senderGridParent = (Grid)((Grid)sender).Parent;
                var label = senderGrid.Children[0] as Label;
                AnswerText = label.Text;
                var boxView = senderGrid.Children[1] as BoxView;
                var optionParent =((StackLayout)((Grid)(senderGrid.Parent)).Parent);
                QuestionGuid = ((Label)((StackLayout)(optionParent.Parent)).Children[0]).Text;
                var imageUrl = ((Label)((StackLayout)(optionParent.Parent)).Children[4]).Text;
                var topIIParent= ((StackLayout)((StackLayout)(optionParent.Parent)).Parent);
                var topParent= (StackLayout)(topIIParent.Parent);
                foreach (var item in optionParent.Children)
                {
                    var grid = item as Grid;
                    var gridII = grid.Children[0] as Grid;
                    var labelI = gridII.Children[0] as Label;
                    var boxViewI = gridII.Children[1] as BoxView;
                    if (labelI?.Text?.ToLower() == label?.Text?.ToLower())
                    {
                        gridII.BackgroundColor = Color.Black;
                        labelI.TextColor = Color.White;
                        boxViewI.BackgroundColor = Color.White;
                    }
                    else
                    {
                        gridII.BackgroundColor = Color.White;
                        labelI.TextColor = Color.Black;
                        boxViewI.BackgroundColor = Color.Black;
                    }

                }
                if (SurveySummaries.Count(a => a.QuestionGuid == QuestionGuid) > 0)
                {
                    SurveySummaries.Where(a => a.QuestionGuid == QuestionGuid).ForEach(a =>
                    {
                        a.AnswerText = AnswerText;
                        a.ImageUrl = imageUrl;
                    });
                }
                else
                {
                    SurveySummaries.Add(new SurveySummary
                    {
                        QuestionGuid = QuestionGuid,
                        AnswerText = AnswerText,
                        ImageUrl=imageUrl
                    });
                }
            }
            catch (Exception)
            {

            }
        }

        private void OptionSelected(object sender, EventArgs e)
        {
            try
            {
                QuestionGuid=((Label)((StackLayout)((Grid)((Button)sender).Parent).Parent).Children[0]).Text;
                var imageUrl=((Label)((StackLayout)((Grid)((Button)sender).Parent).Parent).Children[1]).Text;
                if (((Button)sender).BackgroundColor == Color.FromHex("#fff"))
                {
                    ((Button)sender).BackgroundColor = Color.FromHex("#000");
                    ((Button)sender).TextColor = Color.FromHex("#fff");
                    SurveySummaries.Add(new SurveySummary
                    {
                        QuestionGuid = QuestionGuid,
                        AnswerText = ((Button)sender).Text,
                        ImageUrl=imageUrl
                    });
                }
                else
                {
                    ((Button)sender).BackgroundColor = Color.FromHex("#fff");
                    ((Button)sender).TextColor = Color.FromHex("#000");
                    var surveySummary = SurveySummaries.FirstOrDefault(a => a.QuestionGuid ==QuestionGuid );
                    if (surveySummary != null)
                    {
                        SurveySummaries.Remove(surveySummary);
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            try
            {
                var senderElement = (Entry)sender;
                QuestionGuid=((Label)((StackLayout)((Grid)((Frame)((Grid)(senderElement.Parent)).Parent).Parent).Parent).Children[0]).Text;
                var questionText = ((Label)((Grid)((Frame)((Grid)(senderElement.Parent)).Parent).Parent).Children[0]).Text;
                var configText = ((Label)((Grid)((Frame)((Grid)(senderElement.Parent)).Parent).Parent).Children[2]).Text;
                var imageUrl=((Label)((StackLayout)((Grid)((Frame)((Grid)(senderElement.Parent)).Parent).Parent).Parent).Children[1]).Text;
                AnswerText = senderElement.Text;
                if (SurveySummaries.Count(a => a.QuestionGuid == QuestionGuid) > 0)
                {
                    SurveySummaries.Where(a => a.QuestionGuid == QuestionGuid).ForEach(a =>
                    {
                        a.AnswerText = AnswerText;
                    });
                }
                else
                {
                    SurveySummaries.Add(new SurveySummary
                    {
                        QuestionGuid = QuestionGuid,
                        QuestionText=questionText,
                        AnswerText = AnswerText,
                        ConfigAnswerText=configText,
                        ImageUrl=imageUrl
                    });
                }
            }
            catch (Exception)
            {

            }
        }

        private void OptionSelectedV2(object sender, EventArgs e)
        {
            try
            {
                QuestionGuid=((Label)((StackLayout)((StackLayout)((Grid)((Button)sender).Parent).Parent).Parent).Children[0]).Text;
                var imageUrl=((Label)((StackLayout)((StackLayout)((Grid)((Button)sender).Parent).Parent).Parent).Children[1]).Text;
                if (((Button)sender).BackgroundColor == Color.FromHex("#fff"))
                {
                    ((Button)sender).BackgroundColor = Color.FromHex("#000");
                    ((Button)sender).TextColor = Color.FromHex("#fff");
                    SurveySummaries.Add(new SurveySummary
                    {
                        QuestionGuid = QuestionGuid,
                        AnswerText = ((Button)sender).Text,
                        ImageUrl=imageUrl
                    });
                }
                else
                {
                    ((Button)sender).BackgroundColor = Color.FromHex("#fff");
                    ((Button)sender).TextColor = Color.FromHex("#000");
                    var surveySummary = SurveySummaries.FirstOrDefault(a => a.QuestionGuid == QuestionGuid);
                    if (surveySummary != null)
                    {
                        SurveySummaries.Remove(surveySummary);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}