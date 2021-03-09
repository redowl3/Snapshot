using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using LaunchPad.Mobile.Enums;
using LaunchPad.Mobile.Models;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Svg.Skia;
using Xamarin.Forms;

namespace LaunchPad.Mobile.Helpers
{
    public static class DrawHelper
    {
        public static BodyArea GetSelectedSection(int section)
        {
            switch (section)
            {
                case 1: return BodyArea.Head;
                case 2: return BodyArea.Chest;
                case 3: return BodyArea.Hips;
                case 4: return BodyArea.Knees;
                case 5: return BodyArea.Feet;
                default: return BodyArea.Head;
            }
        }

        public static SKPaint GetCurrentPaintObject(bool currentDraw, SKColor brushColourToBeUsed, int brushThickness)
        {
            return new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = currentDraw ? new SKColor(brushColourToBeUsed.Red, brushColourToBeUsed.Green, brushColourToBeUsed.Blue, 80)
                                : brushColourToBeUsed,
                StrokeWidth = brushThickness,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round,
                BlendMode = SKBlendMode.SoftLight,
                IsAntialias = true,
                IsDither = true
            };
        }

        public static SKPaint GetScaledPaintObject(SKColor color, float strokeWidth)
        {
            return new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = color,
                StrokeWidth = strokeWidth / 4.0f,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round,
                BlendMode = SKBlendMode.SoftLight,
                IsAntialias = true,
                IsDither = true
            };
        }

        public static SKPicture GetSvgData(string svgName)
        {
            SKSvg svg = new SKSvg();

            using (var stream = GetImageStream(svgName))
            {
                return svg.Load(stream);
            }
        }

        private static Stream GetImageStream(string svgName)
        {
            var type = typeof(App).GetTypeInfo();
            var assembly = type.Assembly;

            return assembly.GetManifestResourceStream($"LaunchPad.Mobile.Vectors.{svgName}");
        }

        public static SKMatrix GetFullSvgScaledMatrix(int width, int height, SKPicture picture)
        {
            float imageScale = height / picture.CullRect.Height;
            float xPosition = (picture.CullRect.Width * imageScale) / 2;

            return SKMatrix.CreateScaleTranslation(imageScale, imageScale, (width / 2) - xPosition, 0);
        }

        public static SKMatrix GetAreaSvgScaledMatrix(int width, int height, SKPicture picture, BodyArea areaUsed)
        {
            float imageScale = (height / picture.CullRect.Height) * 4.0f;
            float tY = (float)GetOffsetCalculations(areaUsed, height);

            float axisScale = picture.CullRect.Width * imageScale;
            float xPosition = (width - axisScale) / 2;

            return SKMatrix.CreateScaleTranslation(imageScale, imageScale, xPosition, tY);
        }

        public static double GetOffsetCalculations(BodyArea bodyArea, double height)
        {
            float scaler = 0.75f;
            switch (bodyArea)
            {
                case BodyArea.Head:
                    return 0;
                case BodyArea.Chest:
                    return (height * -1) * scaler;
                case BodyArea.Hips:
                    return (height * -2) * scaler;
                case BodyArea.Knees:
                    return (height * -3) * scaler;
                case BodyArea.Feet:
                    return (height * -4) * scaler;
                default:
                    return 0;
            }
        }

        public static double GetOffsetCalculationsForFullDisplay(BodyArea bodyArea, double height)
        {
            float scaler = 0.93f;
            switch (bodyArea)
            {
                case BodyArea.Head:
                    return 0;
                case BodyArea.Chest:
                    return (height * 1) * scaler;
                case BodyArea.Hips:
                    return (height * 2) * scaler;
                case BodyArea.Knees:
                    return (height * 3) * scaler;
                case BodyArea.Feet:
                    return (height * 4) * scaler;
                default:
                    return 0;
            }
        }

        public static ImageSource GenerateBitmapPopulateImageSource(SKImage skImage)
        {
            ImageSource ImgSrc = ImageSource.FromStream(() => {
                SKData skData = skImage.Encode(SKEncodedImageFormat.Png, 100);
                Stream stream = skData.AsStream(true);
                return stream;
            });

            return ImgSrc;
        }

        public static Stream GenerateBitmapStream(SKImage skImage)
        {
            SKData skData = skImage.Encode(SKEncodedImageFormat.Png, 100);
            Stream stream = skData.AsStream(true);
            return stream;
        }

        public static DrawItem CopyLastDrawPathListItem(List<DrawItem> drawItems)
        {
            drawItems.Reverse();

            DrawItem tmpItem = new DrawItem()
            {
                Path = new SKPath(drawItems[0].Path),
                Paint = drawItems[0].Paint,
                Area = drawItems[0].Area,
                Name = drawItems[0].Name
            };

            drawItems.Reverse();
            return tmpItem;
        }

        public static void PerformMirrorOperation(DrawItem tmpItem, float canvasWidth)
        {
            float midPoint = canvasWidth / 2.0f;
            float drawMidXPoint = tmpItem.Path.Bounds.MidX;
            float x = (midPoint - drawMidXPoint) * 2;

            SKMatrix translateXaxis = SKMatrix.CreateTranslation(x, 0);
            tmpItem.Path.Transform(translateXaxis);
        }

        public static void InvertBoundBrushPaths(SKPath p)
        {
            var midX = p.Bounds.MidX;
            List<SKPoint> tmpPoints = new List<SKPoint>();

            foreach (SKPoint item in p.Points)
            {
                double p1 = item.X - midX;
                double p2 = (p1 * -1) + midX;

                tmpPoints.Add(new SKPoint
                {
                    X = (float)p2,
                    Y = item.Y
                });
            }

            p.Rewind();
            p.MoveTo(tmpPoints[0]);

            foreach (SKPoint pointObj in tmpPoints)
            {
                p.LineTo(pointObj);
            }
        }

        public static string GetStringBasedSvgPathData(List<SvgData> pathData, bool isFront)
        {
            StringBuilder sb = new StringBuilder();

            foreach (SvgData item in pathData)
            {
                if (item.IsFront == isFront)
                    sb.Append(item.SvgPath);
            }

            return sb.ToString();
        }
    }
}
