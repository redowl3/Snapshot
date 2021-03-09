using System;
using System.Collections.Generic;
using LaunchPad.Mobile.Helpers;
using SkiaSharp;

namespace LaunchPad.Mobile.Models
{
    public static class DrawData
    {
        private static bool Configured = false;
        public static List<DrawItem> DrawnPathsFront { get; set; }
        public static List<DrawItem> DrawnPathsBack { get; set; }

        public static SKPicture Front_SVG { get; set; }
        public static SKPicture Back_SVG { get; set; }
        public static Dictionary<string, List<ConcernNote>> Notes { get; set; }

        public static void Init()
        {
            if (!Configured)
            {
                DrawnPathsFront = new List<DrawItem>();
                DrawnPathsBack = new List<DrawItem>();
                Notes = new Dictionary<string, List<ConcernNote>>();

                Front_SVG = DrawHelper.GetSvgData("front.svg");
                Back_SVG = DrawHelper.GetSvgData("back.svg");
                Configured = true;
            }
        }
    }
}