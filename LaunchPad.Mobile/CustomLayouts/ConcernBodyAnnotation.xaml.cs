using System;
using System.Collections.Generic;
using LaunchPad.Mobile.Enums;
using LaunchPad.Mobile.Helpers;
using LaunchPad.Mobile.Models;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using FormsControls.Base;

namespace LaunchPad.Mobile.CustomLayouts
{
    public partial class ConcernBodyAnnotation : AnimationPage
    {
        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        List<DrawItem> completedPaths = new List<DrawItem>();
        private SKColor brushColourToBeUsed = SKColors.Black;
        private int brushThickness;
        private string brushName;
        private Color brushUnselectedButtonColour = Color.LightGray;
        private BodyArea areaUsed;
        private bool isFrontBody;
        private SKPicture picture = null;
        private float brushRatio;
        private SKImage imageData;
        private string brushTypeText;

        public ConcernBodyAnnotation(SKColor colorOfBrushToUse, string brushType, bool useFrontOfBodyImage, BodyArea areaSelection)
        {
            InitializeComponent();

            brushColourToBeUsed = colorOfBrushToUse;
            brushName = brushType;
            areaUsed = areaSelection;
            isFrontBody = useFrontOfBodyImage;
            picture = isFrontBody ? DrawData.Front_SVG : DrawData.Back_SVG;
            brushTypeText = brushType;
            txtSubject.Text = brushType;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            string bodyDesc = isFrontBody ? "Front" : "Back";
            Title = $"Body Annotation ( {bodyDesc} / {areaUsed} )";

            brushRatio = (int)(canvasView.Height / 120);
            InitialiseBrushSetup();
            canvasView.InvalidateSurface();
        }

        private void InitialiseBrushSetup()
        {
            brushThickness = (int)((int)BrushThickness.Small * brushRatio);
            ResetBrushButtons();
            brushSmall.BorderColor = brushColourToBeUsed.ToFormsColor();
        }

        void OnTouchEffectAction(System.Object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    if (!inProgressPaths.ContainsKey(e.Id))
                    {
                        SKPath path = new SKPath();
                        path.MoveTo(e.Location);
                        inProgressPaths.Add(e.Id, path);
                        canvasView.InvalidateSurface();
                    }
                    break;

                case SKTouchAction.Moved:
                    if (inProgressPaths.ContainsKey(e.Id))
                    {
                        SKPath path = inProgressPaths[e.Id];
                        path.LineTo(e.Location);
                        canvasView.InvalidateSurface();
                    }
                    break;

                case SKTouchAction.Released:
                    if (inProgressPaths.ContainsKey(e.Id))
                    {
                        completedPaths.Add(new DrawItem()
                        {
                            Path = inProgressPaths[e.Id],
                            Paint = DrawHelper.GetCurrentPaintObject(false, brushColourToBeUsed, brushThickness),
                            Area = areaUsed,
                            Name = brushName
                        });

                        inProgressPaths.Remove(e.Id);
                        canvasView.InvalidateSurface();
                    }
                    break;
            }

            e.Handled = true;
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKCanvas canvas = args.Surface.Canvas;
            canvas.Clear();

            SKPaint tempPaint = DrawHelper.GetCurrentPaintObject(true, brushColourToBeUsed, brushThickness);
            SKMatrix matrix = DrawHelper.GetAreaSvgScaledMatrix(args.Info.Width, args.Info.Height, picture, areaUsed);

            canvas.DrawPicture(picture, ref matrix);

            foreach (DrawItem item in isFrontBody ? DrawData.DrawnPathsFront : DrawData.DrawnPathsBack)
            {
                if (areaUsed == item.Area)
                    canvas.DrawPath(item.Path, item.Paint);
            }

            foreach (DrawItem drawItem in completedPaths)
            {
                canvas.DrawPath(drawItem.Path, drawItem.Paint);
            }

            foreach (SKPath path in inProgressPaths.Values)
            {
                canvas.DrawPath(path, tempPaint);
            }

            btnSave.IsVisible = completedPaths.Count > 0;
            btnUndo.IsVisible = completedPaths.Count > 0;
            btnMirror.IsVisible = completedPaths.Count > 0;

            imageData = args.Surface.Snapshot();
        }



        void brush_Clicked(System.Object sender, System.EventArgs e)
        {
            int size = int.Parse((sender as Button).CommandParameter.ToString());
            ;

            switch (size)
            {
                case 1:
                    brushThickness = (int)((int)BrushThickness.Small * brushRatio);
                    ResetBrushButtons();
                    brushSmall.BorderColor = brushColourToBeUsed.ToFormsColor();
                    break;
                case 2:
                    brushThickness = (int)((int)BrushThickness.Medium * brushRatio);
                    ResetBrushButtons();
                    brushMedium.BorderColor = brushColourToBeUsed.ToFormsColor();
                    break;
                case 3:
                    brushThickness = (int)((int)BrushThickness.Large * brushRatio);
                    ResetBrushButtons();
                    brushLarge.BorderColor = brushColourToBeUsed.ToFormsColor();
                    break;
                case 4:
                    brushThickness = (int)((int)BrushThickness.VeryLarge * brushRatio);
                    ResetBrushButtons();
                    brushVeryLarge.BorderColor = brushColourToBeUsed.ToFormsColor();
                    break;
                default:
                    brushThickness = 200;
                    ResetBrushButtons();
                    break;
            }
        }

        private void ResetBrushButtons()
        {
            brushVeryLarge.BorderColor = brushUnselectedButtonColour;
            brushLarge.BorderColor = brushUnselectedButtonColour;
            brushMedium.BorderColor = brushUnselectedButtonColour;
            brushSmall.BorderColor = brushUnselectedButtonColour;
        }

        void Undo_Button_Clicked(System.Object sender, System.EventArgs e)
        {
            if (completedPaths.Count > 0)
            {
                completedPaths.Reverse();
                completedPaths.RemoveAt(0);
                completedPaths.Reverse();
                canvasView.InvalidateSurface();
            }
        }

        void Mirror_Button_Clicked(System.Object sender, System.EventArgs e)
        {
            if (completedPaths.Count > 0)
            {
                DrawItem tmpItem = DrawHelper.CopyLastDrawPathListItem(completedPaths);
                DrawHelper.PerformMirrorOperation(tmpItem, canvasView.CanvasSize.Width);
                DrawHelper.InvertBoundBrushPaths(tmpItem.Path);

                completedPaths.Add(tmpItem);
                canvasView.InvalidateSurface();
            }
        }

        void Save_Button_Clicked(System.Object sender, System.EventArgs e)
        {
            if (completedPaths.Count > 0)
            {
                foreach (DrawItem item in completedPaths)
                {
                    if (isFrontBody)
                    {
                        DrawData.DrawnPathsFront.Add(item);
                    }
                    else
                    {
                        DrawData.DrawnPathsBack.Add(item);
                    }
                }
                completedPaths.Clear();
            }
            canvasView.InvalidateSurface();
        }

        async void Exit_Button_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async void Note_Button_Clicked(System.Object sender, System.EventArgs e)
        {
            string bodyDesc = isFrontBody ? "Front" : "Back";
            string key = bodyDesc + "/" + areaUsed;

            if (DrawData.Notes.ContainsKey(key))
            {
                DrawData.Notes.TryGetValue(key, out List<ConcernNote> info);
                string result = await DisplayPromptAsync("Update Information", $"Notes on {key} Area?",
                    placeholder: "Add more information", keyboard: Keyboard.Text, initialValue: string.Empty);

                if (!string.IsNullOrEmpty(result))
                {
                    info.Add(new ConcernNote()
                    {
                        ID = Guid.NewGuid(),
                        Key = key,
                        Message = result,
                        Time = DateTime.UtcNow
                    });

                    DrawData.Notes.Remove(key);
                    DrawData.Notes.Add(key, info);
                }
            }
            else
            {
                string result = await DisplayPromptAsync("Add Information", $"Notes on {key} Area?",
                    placeholder: "Enter relevant information...", keyboard: Keyboard.Text, initialValue: string.Empty);

                var noteItem = new List<ConcernNote>();
                noteItem.Add(new ConcernNote()
                {
                    ID = Guid.NewGuid(),
                    Key = key,
                    Message = result,
                    Time = DateTime.UtcNow
                });

                if (!string.IsNullOrEmpty(result))
                    DrawData.Notes.Add(key, noteItem);
            }

        }

        async void btnSavePicture_Clicked(System.Object sender, System.EventArgs e)
        {
            if (imageData != null && imageData is SKImage)
            {
                ImageSource srcObj = DrawHelper.GenerateBitmapPopulateImageSource(imageData);
                await Navigation.PushAsync(new ConcernAnnotationSummary(srcObj, isFrontBody, areaUsed));
            }
        }
    }
}
