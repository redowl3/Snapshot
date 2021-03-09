using LaunchPad.Mobile.ViewModels;
using System.Collections.Generic;

namespace LaunchPad.Mobile.Models
{
    public class SurveyOverView
    {
        public string Title { get; set; }
        public List<SurveySummary> SurveySummaries { get; set; }
        public SurveyOverView()
        {
            SurveySummaries = new List<SurveySummary>();
        }
    }
}
