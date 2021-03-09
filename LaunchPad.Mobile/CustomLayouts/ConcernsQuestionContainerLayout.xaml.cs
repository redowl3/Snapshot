using LaunchPad.Mobile.Helpers;
using LaunchPad.Mobile.Models;
using LaunchPad.Mobile.Services;
using LaunchPad.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
namespace LaunchPad.Mobile.CustomLayouts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConcernsQuestionContainerLayout : ContentView
    {
        private List<SurveySummary> SurveySummaries = new List<SurveySummary>();
        private IDatabaseServices DatabaseServices => DependencyService.Get<IDatabaseServices>();
        public string QuestionGuid { get; set; }
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }
        public string SubAnswerText { get; set; }
        private List<SvgData> _body_svg_data = new List<SvgData>();
        public ConcernsQuestionContainerLayout()
        {
            InitializeComponent();
            this.BindingContext = App.ConcernsAndSkinCareSurveyViewModel;
            MessagingCenter.Subscribe<List<SvgData>>(this, "svg_data", (svgdata) =>
            {
                _body_svg_data = svgdata;
            });
        }

        private void ToggleContainer(object sender, EventArgs e)
        {
            try
            {
                var toggleContainerParent = ((StackLayout)((StackLayout)((Frame)((Grid)((Grid)sender).Parent).Parent).Parent).Parent);
                var senderParent = (Grid)sender;
                var parentOfSenderParent = (Grid)senderParent.Parent;
                (parentOfSenderParent.Children[1] as StackLayout).IsVisible = !(parentOfSenderParent.Children[1] as StackLayout).IsVisible;
                if ((parentOfSenderParent.Children[1] as StackLayout).IsVisible)
                {
                    senderParent.BackgroundColor = Color.FromHex("#000");
                    ((Label)senderParent.Children[1]).TextColor = Color.FromHex("#fff");
                    ((Image)senderParent.Children[2]).Source = "icon_arrow_top";
                    ((Image)senderParent.Children[2]).WidthRequest = 30;
                    ((Image)senderParent.Children[2]).HeightRequest = 30;
                }
                else
                {
                    ((Image)senderParent.Children[2]).Source = "icon_down_arrow_white";
                    ((Image)senderParent.Children[2]).WidthRequest = 30;
                    ((Image)senderParent.Children[2]).HeightRequest = 30;
                }
                QuestionText = (e as TappedEventArgs)?.Parameter as string;
                QuestionGuid = ((Label)senderParent.Children[0]).Text;
                foreach (var item in toggleContainerParent.Children)
                {
                    var child1 = item as StackLayout;
                    var child2 = child1.Children[2] as Frame;
                    var child3 = child2.Content as Grid;
                    var grid = child3.Children[0] as Grid;
                    var label = grid.Children[1] as Label;
                    if (label?.Text?.ToLower() != QuestionText.ToLower())
                    {
                        ((StackLayout)child3.Children[1]).IsVisible = false;
                    }
                }
            }
            catch (Exception)
            {

            }
            
        }

        private void editor_unfocused(object sender, FocusEventArgs e)
        {
            try
            {
                var topParent = (StackLayout)((Grid)((Frame)((Grid)((Editor)sender).Parent).Parent).Parent).Parent;
                if (topParent != null)
                {
                    QuestionText = ((topParent.Children[0] as Grid)?.Children[0] as Label)?.Text;
                    SubAnswerText = ((Editor)sender).Text;
                    QuestionGuid = (((Grid)((Editor)sender).Parent).Children[0] as Label).Text;
                    if (!string.IsNullOrEmpty(SubAnswerText))
                    {
                        SurveySummaries.Add(new SurveySummary
                        {
                            QuestionGuid = QuestionGuid,
                            AnswerText = SubAnswerText
                        });
                    }
                    else
                    {
                        SurveySummaries.Remove(SurveySummaries.FirstOrDefault(a => a.QuestionGuid == QuestionGuid));
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void Finish(object sender, EventArgs e)
        {

        }

        private async void SaveAndContinue(object sender, EventArgs e)
        {
            // Use rich path data from '_body_svg_data' posted back from Warpaint section
            if (_body_svg_data != null && _body_svg_data.Count > 0)
            {
                int frontActions = _body_svg_data.Where(x => x.IsFront).Count();
                int backActions = _body_svg_data.Where(x => x.IsFront == false).Count();

                int frontConcerns = _body_svg_data.Where(x => x.IsFront).Select(y => y.ConcernName).Distinct().Count();
                int backConcerns = _body_svg_data.Where(x => x.IsFront == false).Select(y => y.ConcernName).Distinct().Count();

                int totalActions = _body_svg_data.Where(x => x.IsFront).Count();
                int totalConcernsCount = _body_svg_data.Select(x => x.ConcernName).Distinct().Count();

                string pathValuesFront = DrawHelper.GetStringBasedSvgPathData(_body_svg_data, true);
                string pathValuesBack = DrawHelper.GetStringBasedSvgPathData(_body_svg_data, false);
            }
            var bindingContext = this.BindingContext as ConcernsAndSkinCareSurveyViewModel;
            if (bindingContext != null)
            {
                var surveyReviews = await DatabaseServices.Get<List<SurveyOverView>>("SurveyOverView" + Settings.ClientId);
                if (bindingContext.Counter == bindingContext.MaxCounter)
                {
                    if (surveyReviews.Count(a => a.Title.ToLower() == "skin type") > 0)
                    {
                        surveyReviews.Where(a => a.Title.ToLower() == "skin type").ForEach(x =>
                        {                           
                            x.SurveySummaries = new List<SurveySummary>(SurveySummaries);
                        });
                    }
                    else
                    {
                        var surveyOverView = new SurveyOverView();
                        surveyOverView.Title = "Skin Type";
                        surveyOverView.SurveySummaries = new List<SurveySummary>(SurveySummaries);
                        surveyReviews.Add(surveyOverView);
                    }

                    await DatabaseServices.InsertData("SurveyOverView" + Settings.ClientId, surveyReviews);
                    //if (surveyReviews.Count(a => a.Title.ToLower() == "you and your lifestyle") > 0)
                    //{
                    //    surveyReviews.Where(a => a.Title.ToLower() == "you and your lifestyle").ForEach(x =>
                    //    {
                    //        x.SurveySummaries.AddRange(SurveySummaries);
                    //    });
                    //}
                    //else
                    //{
                    //    var surveyOverView = new SurveyOverView();
                    //    surveyOverView.Title = "You and your lifestyle";
                    //    surveyOverView.SurveySummaries = new List<SurveySummary>(SurveySummaries);
                    //    surveyReviews.Add(surveyOverView);
                    //}

                    //await DatabaseServices.InsertData("SurveyOverView" + Settings.ClientId, surveyReviews);
                }
            }

            (this.BindingContext as ConcernsAndSkinCareSurveyViewModel)?.ContinueCommand.Execute(SurveySummaries);            
        }
        private void GetSvgPathsFromData(out string frontSvgPaths, out string backSvgPaths)
        {
            StringBuilder sbFront = new StringBuilder();
            StringBuilder sbBack = new StringBuilder();
            foreach (SvgData item in _body_svg_data)
            {
                if (item.IsFront)
                    sbFront.Append(item.SvgPath);
                else
                    sbBack.Append(item.SvgPath);
            }

            frontSvgPaths = sbFront.ToString();
            backSvgPaths = sbBack.ToString();
        }
        private void OptionTapped(object sender, EventArgs e)
        {
            try
            {
                var senderParent = ((Grid)sender).Parent as StackLayout;
                SubAnswerText = (((Grid)sender).Children[0] as Label).Text;
                QuestionGuid=((Label)((Grid)((Grid)(senderParent.Parent)).Children[0]).Children[0]).Text;
                var imageUrl =((Label)((StackLayout)((StackLayout)((StackLayout)((Frame)((Grid)(senderParent.Parent)).Parent).Parent).Parent).Children[0]).Children[3]).Text;
                AnswerText=((Label)((Grid)((Grid)(senderParent.Parent)).Children[0]).Children[1]).Text;
                if (((Grid)sender).BackgroundColor != Color.FromHex("#000"))
                {
                    ((Grid)sender).BackgroundColor = Color.FromHex("#000");
                    (((Grid)sender).Children[0] as Label).TextColor = Color.FromHex("#fff");

                    SubAnswerText = (((Grid)sender).Children[0] as Label).Text;

                    if (SurveySummaries.Count(a => a.QuestionGuid == QuestionGuid) > 0)
                    {
                        SurveySummaries.Where(a => a.QuestionGuid == QuestionGuid).ForEach(a =>
                        {
                            if (a.AnswerText == AnswerText && a.SubAnswerText == SubAnswerText) return;
                            a.AnswerText = a.AnswerText;
                            a.SubAnswerText = SubAnswerText;
                        });
                    }
                    else
                    {
                        SurveySummaries.Add(new SurveySummary
                        {
                            QuestionGuid = QuestionGuid,
                            AnswerText = AnswerText,
                            SubAnswerText = SubAnswerText,
                            ImageUrl=imageUrl
                        });
                    }
                }
                else
                {
                    ((Grid)sender).BackgroundColor = Color.FromHex("#fff");
                    (((Grid)sender).Children[0] as Label).TextColor = Color.FromHex("#000");
                    SubAnswerText = "";
                }

                foreach (var item in senderParent.Children)
                {
                    var grid = item as Grid;
                    var label = grid.Children[0] as Label;
                    if (label?.Text?.ToLower() != SubAnswerText.ToLower())
                    {
                        grid.BackgroundColor = Color.FromHex("#fff");
                        label.TextColor = Color.FromHex("#000");
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void YesNoButtonClicked(object sender, EventArgs e)
        {
            try
            {
                ((Button)sender).BackgroundColor = Color.FromHex("#000");
                ((Button)sender).TextColor = Color.FromHex("#fff");
                var container = ((StackLayout)((Button)sender).Parent);
                foreach (var item in container.Children)
                {
                    var button = item as Button;
                    if (button?.Text?.ToLower() != ((Button)sender).Text.ToLower())
                    {
                        button.BackgroundColor = Color.FromHex("#fff");
                        button.TextColor = Color.FromHex("#000");
                    }
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
                var senderParent = ((StackLayout)((Grid)((Button)sender).Parent).Parent);
                var topParent = ((StackLayout)(senderParent.Parent));
                ((Button)sender).BackgroundColor = Color.FromHex("#000");
                ((Button)sender).TextColor = Color.FromHex("#fff");
                QuestionGuid = (topParent.Children[0] as Label).Text;
                AnswerText = ((Button)sender).Text;
                //foreach (var item in senderParent.Children)
                //{
                //    var grid = item as Grid;
                //    var button = grid.Children[0] as Button;

                //    if (button?.Text?.ToLower() != ((Button)sender).Text.ToLower())
                //    {
                //        button.BackgroundColor = Color.FromHex("#fff");
                //        button.TextColor = Color.FromHex("#000");
                //    }
                //}

                if (SurveySummaries.Count(a => a.QuestionGuid == QuestionGuid) > 0)
                {
                    SurveySummaries.Where(a => a.QuestionGuid == QuestionGuid).ForEach(a => a.AnswerText = AnswerText);
                }
                else
                {
                    SurveySummaries.Add(new SurveySummary
                    {
                        QuestionGuid=QuestionGuid,
                        AnswerText=AnswerText
                    });
                }
            }
            catch (Exception)
            {

            }
           
        }

        private void UnselectThis(object sender, EventArgs e)
        {
            try
            {
                var toggleContainerParent = ((StackLayout)((StackLayout)((Frame)((Grid)((Grid)sender).Parent).Parent).Parent).Parent);
                var senderParent = (Grid)sender;
                var parentOfSenderParent = (Grid)senderParent.Parent;
                (parentOfSenderParent.Children[1] as StackLayout).IsVisible = false;
                senderParent.BackgroundColor = Color.FromHex("#fff");
                ((Frame)((Grid)((Grid)sender).Parent).Parent).BorderColor = Color.FromHex("#000");
                ((Label)senderParent.Children[1]).TextColor = Color.FromHex("#000");
                ((Image)senderParent.Children[2]).Source = "icon_down_arrow";
                QuestionGuid = ((Label)senderParent.Children[0]).Text;
                SurveySummaries.Remove(SurveySummaries.FirstOrDefault(a => a.QuestionGuid == QuestionGuid));
            }
            catch (Exception)
            {

            }
        }
        private void AllowNotes(object sender, EventArgs e)
        {
            try
            {
                var senderElement = (ImageButton)sender;
                var parentOfSenderElement = (StackLayout)(senderElement.Parent);
                ((Frame)parentOfSenderElement.Children[1]).IsVisible = !((Frame)parentOfSenderElement.Children[1]).IsVisible;
                if (((Frame)parentOfSenderElement.Children[1]).IsVisible)
                {
                    senderElement.Source = "icon_notes_active";
                }
                else
                {
                    senderElement.Source = "icon_notes";
                }
            }
            catch (Exception)
            {

            }
        }

        private void notes_added(object sender, FocusEventArgs e)
        {
            try
            {
                var senderElement = (Editor)sender;
                var parent_sender_element = (Grid)(senderElement.Parent);
                QuestionGuid = ((Label)(parent_sender_element.Children[0])).Text;

                if (SurveySummaries.Count(a => a.QuestionGuid == QuestionGuid) > 0)
                {
                    SurveySummaries.Where(a => a.QuestionGuid == QuestionGuid).ForEach(a => a.Notes = senderElement.Text);
                }
                else
                {
                    SurveySummaries.Add(new SurveySummary
                    {
                        QuestionGuid = QuestionGuid,
                        Notes = senderElement.Text
                    });
                }
            }
            catch (Exception)
            {

            }
        }

        private async void ConcernTypeSelected(object sender, EventArgs e)
        {
            List<Answer> concernSelection = sender is ImageButton ? (sender as ImageButton).CommandParameter as List<Answer> : (sender as Button).CommandParameter as List<Answer>;
            if (concernSelection != null && concernSelection.Count == 1)
            {
                string concernText = concernSelection[0].ResponseText;
                ConcernItem concernItem = ConcernHelper.GetConcernDetailsByText(concernText);
                await Navigation.PushAsync(new ConcernBodySelection(concernItem));
            }
        }
    }
}