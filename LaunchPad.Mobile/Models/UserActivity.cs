using IIAADataModels.Transfer;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchPad.Mobile.Models
{
    public class UserActivityByYear
    {
        public int Year { get; set; }
        public List<UserActivity> UserActivities { get; set; }
    }
    public class UserActivity
    {
        public Guid Id { get; set; }
        public DateTime PerformedOn { get; set; }
        public Activity Activity { get; set; }
    }

    public class Activity
    {
        public List<Product> HealthPlans { get; set; }
        public CustomBasket Consultations { get; set; }
        public DateTime CreateOrUpdateDate { get; set; }
    }
}
