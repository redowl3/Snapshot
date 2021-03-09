using System;
using LaunchPad.Mobile.Enums;
using SkiaSharp;

namespace LaunchPad.Mobile.Models
{
    [Serializable]
    public class DrawItem
    {
        public SKPath Path { get; set; }
        public SKPaint Paint { get; set; }
        public BodyArea Area { get; set; }
        public string Name { get; set; }
    }
}
