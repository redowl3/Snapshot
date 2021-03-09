using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LaunchPad.Mobile.Models
{
    public class SvgData
    {
        public bool IsFront { get; set; }
        public string BodyRegion { get; set; }
        public string SvgPath { get; set; }
        public float StrokeWidth { get; set; }
        public Color StrokeColor { get; set; }
        public string ConcernName { get; set; }
    }
}
