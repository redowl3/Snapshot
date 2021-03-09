using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace LaunchPad.Mobile.Converters
{
    public class DateTimeFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? value : ((DateTime)value).ToString("dd MMMM yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class DaysGetter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? value : (DateTime.Now.Date - ((DateTime)value).Date).TotalDays>1? $"{(DateTime.Now.Date-((DateTime)value).Date).TotalDays} days": $"{(DateTime.Now.Date - ((DateTime)value).Date).TotalDays} day";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
