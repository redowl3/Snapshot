using System;
using System.Collections.Generic;
using LaunchPad.Mobile.Helpers;
using LaunchPad.Mobile.Models;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using FormsControls.Base;

namespace LaunchPad.Mobile.CustomLayouts
{
    public partial class ConcernBodySelection : AnimationPage
    {
        private SKColor brushColourToUse;
        private string brushType;
        private SKPicture pictureFront;
        private SKPicture pictureBack;
        private SKImage imageDataF;
        private SKImage imageDataB;
        private List<SvgData> _data;

        public ConcernBodySelection(ConcernItem concernSelected)
        {
            InitializeComponent();

            DrawData.Init();
            SetConcernCriteria(concernSelected);
            pictureFront = DrawData.Front_SVG;
            pictureBack = DrawData.Back_SVG;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _data = new List<SvgData>();
            canvasFront.InvalidateSurface();
            canvasBack.InvalidateSurface();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send(_data, "svg_data");
        }

        async void TapGestureRecognizer_Front_Tapped(System.Object sender, System.EventArgs e)
        {
            int bodySection = int.Parse((e as TappedEventArgs).Parameter.ToString());
            await Navigation.PushAsync(new ConcernBodyAnnotation(brushColourToUse, brushType, true,
                DrawHelper.GetSelectedSection(bodySection)));
        }

        async void TapGestureRecognizer_Back_Tapped(System.Object sender, System.EventArgs e)
        {
            int bodySection = int.Parse((e as TappedEventArgs).Parameter.ToString());
            await Navigation.PushAsync(new ConcernBodyAnnotation(brushColourToUse, brushType, false,
                DrawHelper.GetSelectedSection(bodySection)));
        }

        private void SetConcernCriteria(ConcernItem concern)
        {
            btnColorSelected.BackgroundColor = concern.Color;
            brushColourToUse = concern.Color.ToSKColor();
            brushType = concern.Description;
            txtSubject.Text = concern.Description;
        }

        void canvasFront_PaintSurface(System.Object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;
            canvas.Clear();

            SKMatrix matrix = DrawHelper.GetFullSvgScaledMatrix(e.Info.Width, e.Info.Height, pictureFront);
            canvas.DrawPicture(pictureFront, ref matrix);
            float offsetY = e.Info.Height / 5;

            foreach (DrawItem item in DrawData.DrawnPathsFront)
            {
                SKPath p = new SKPath(item.Path);
                float offsetCalcY = (float)DrawHelper.GetOffsetCalculationsForFullDisplay(item.Area, offsetY);
                float offsetCalcX = ((e.Info.Width - matrix.TransX) / 2.0f) * 0.375f;

                p.Transform(SKMatrix.CreateScaleTranslation(0.25f, 0.25f, offsetCalcX, offsetCalcY));

                _data.Add(new SvgData
                {
                    BodyRegion = item.Area.ToString(),
                    IsFront = true,
                    StrokeColor = item.Paint.Color.ToFormsColor(),
                    StrokeWidth = item.Paint.StrokeWidth,
                    SvgPath = p.ToSvgPathData(),
                    ConcernName = item.Name
                });

                canvas.DrawPath(p, DrawHelper.GetScaledPaintObject(item.Paint.Color, item.Paint.StrokeWidth));
            }

            imageDataF = e.Surface.Snapshot();
        }

        void canvasBack_PaintSurface(System.Object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;
            canvas.Clear();

            SKMatrix matrix = DrawHelper.GetFullSvgScaledMatrix(e.Info.Width, e.Info.Height, pictureBack);
            canvas.DrawPicture(pictureBack, ref matrix);
            float offsetY = e.Info.Height / 5;

            foreach (DrawItem item in DrawData.DrawnPathsBack)
            {
                SKPath p = new SKPath(item.Path);
                float offsetCalcY = (float)DrawHelper.GetOffsetCalculationsForFullDisplay(item.Area, offsetY);
                float offsetCalcX = ((e.Info.Width - matrix.TransX) / 2.0f) * 0.375f;

                p.Transform(SKMatrix.CreateScaleTranslation(0.25f, 0.25f, offsetCalcX, offsetCalcY));

                _data.Add(new SvgData
                {
                    BodyRegion = item.Area.ToString(),
                    IsFront = false,
                    StrokeColor = item.Paint.Color.ToFormsColor(),
                    StrokeWidth = item.Paint.StrokeWidth,
                    SvgPath = p.ToSvgPathData(),
                    ConcernName = item.Name
                });

                canvas.DrawPath(p, DrawHelper.GetScaledPaintObject(item.Paint.Color, item.Paint.StrokeWidth));
            }

            imageDataB = e.Surface.Snapshot();
        }

        async void Save_Button_Clicked(System.Object sender, System.EventArgs e)
        {
            if (imageDataF != null && imageDataB != null)
            {
                ImageSource srcObjF = DrawHelper.GenerateBitmapPopulateImageSource(imageDataF);
                ImageSource srcObjB = DrawHelper.GenerateBitmapPopulateImageSource(imageDataB);
                await Navigation.PushAsync(new ConcernAnnotationSummary(srcObjF, srcObjB));
            }
        }

        async void btnErase_Clicked(System.Object sender, System.EventArgs e)
        {
            string action = await DisplayActionSheet("Clear Data", null, null, "No Action",
                "Clear All Body Data", "Clear Front Body Data", "Clear Back Body Data");

            switch (action)
            {
                case "Clear All Body Data":
                    DrawData.DrawnPathsFront = new List<DrawItem>();
                    DrawData.DrawnPathsBack = new List<DrawItem>();
                    _data = new List<SvgData>();
                    canvasFront.InvalidateSurface();
                    canvasBack.InvalidateSurface();
                    break;
                case "Clear Front Body Data":
                    DrawData.DrawnPathsFront = new List<DrawItem>();
                    _data = new List<SvgData>();
                    canvasFront.InvalidateSurface();
                    canvasBack.InvalidateSurface();
                    break;
                case "Clear Back Body Data":
                    DrawData.DrawnPathsBack = new List<DrawItem>();
                    _data = new List<SvgData>();
                    canvasFront.InvalidateSurface();
                    canvasBack.InvalidateSurface();
                    break;
                default:
                    break;
            }
        }

        private void btnExit_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}
