using System;
using System.Collections.Generic;
using System.Linq;
using LaunchPad.Mobile.Enums;
using LaunchPad.Mobile.Models;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using FormsControls.Base;
namespace LaunchPad.Mobile.CustomLayouts
{
    public partial class ConcernAnnotationSummary : AnimationPage
    {
        private ImageSource _src1;
        private ImageSource _src2;
        private string notesKey = null;
        private bool isFront;
        private bool wholeBody;
        private BodyArea bodyArea;

        public ConcernAnnotationSummary(ImageSource src, bool isFrontBody, BodyArea areaUsed)
        {
            InitializeComponent();
            string bodyDesc = isFrontBody ? "Front" : "Back";
            Title = $"Body Section Review ( {bodyDesc} / {areaUsed} )";
            wholeBody = false;

            _src1 = src;
            _src2 = null;

            notesKey = bodyDesc + "/" + areaUsed;
            isFront = isFrontBody;
            bodyArea = areaUsed;
        }

        public ConcernAnnotationSummary(ImageSource srcF, ImageSource srcB)
        {
            InitializeComponent();
            Title = "Whole Body Review";
            wholeBody = true;
            _src1 = srcF;
            _src2 = srcB;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            imgPreviewF.Source = _src1 != null ? _src1 : null;
            imgPreviewB.Source = _src2 != null ? _src2 : null;
            imgPreviewF.IsVisible = _src1 != null;
            imgPreviewB.IsVisible = _src2 != null;

            listNotes.ItemsSource = SetUpNotesListItemSource();

            List<ConcernListItem> itemsInList = new List<ConcernListItem>();

            if (wholeBody)
            {
                foreach (DrawItem item in DrawData.DrawnPathsFront)
                {
                    bool isInList = itemsInList.Any(x => x.ItemName == item.Name);
                    if (!isInList)
                    {
                        itemsInList.Add(
                         new ConcernListItem
                         {
                             ItemColour = item.Paint.Color.ToFormsColor(),
                             ItemName = item.Name
                         });
                    }
                }
                foreach (DrawItem item in DrawData.DrawnPathsBack)
                {
                    bool isInList = itemsInList.Any(x => x.ItemName == item.Name);
                    if (!isInList)
                    {
                        itemsInList.Add(
                         new ConcernListItem
                         {
                             ItemColour = item.Paint.Color.ToFormsColor(),
                             ItemName = item.Name
                         });
                    }
                }
            }
            else
            {
                foreach (DrawItem item in isFront ? DrawData.DrawnPathsFront : DrawData.DrawnPathsBack)
                {
                    bool isInList = itemsInList.Any(x => x.ItemName == item.Name);
                    if (item.Area == bodyArea && !isInList)
                    {
                        itemsInList.Add(
                         new ConcernListItem
                         {
                             ItemColour = item.Paint.Color.ToFormsColor(),
                             ItemName = item.Name
                         });
                    }
                }
            }

            listItems.ItemsSource = itemsInList;
        }

        private List<ConcernNote> SetUpNotesListItemSource()
        {
            if (notesKey != null)
            {
                DrawData.Notes.TryGetValue(notesKey, out List<ConcernNote> info);
                return info?.OrderByDescending(x => x.Time).ToList();
            }
            else
            {
                List<ConcernNote> noteListTemp = new List<ConcernNote>();
                List<string> noteKeyListData = DrawData.Notes.Keys.ToList();

                foreach (string keyUsed in noteKeyListData)
                {
                    DrawData.Notes.TryGetValue(keyUsed, out List<ConcernNote> info);
                    List<ConcernNote> t2 = info?.OrderByDescending(x => x.Time).ToList();

                    foreach (ConcernNote noteItem in t2)
                    {
                        ConcernNote nn = new ConcernNote
                        {
                            Message = keyUsed + " - " + noteItem.Message,
                            Time = noteItem.Time
                        };

                        noteListTemp.Add(nn);
                    }
                }
                return noteListTemp;
            }
        }

        async void listNotes_ItemSelected(System.Object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null && e.SelectedItem is ConcernNote)
            {
                ConcernNote note = e.SelectedItem as ConcernNote;

                if (note.Key == null)
                    return;

                string action = await DisplayActionSheet("Note selection action", null, null, "No Action", "Edit", "Delete");

                switch (action)
                {
                    case "Edit":
                        EditNote(note);
                        break;
                    case "Delete":
                        DeleteNote(note);
                        break;
                    default:
                        break;
                }
            }

            listNotes.ItemsSource = null;
            listNotes.ItemsSource = SetUpNotesListItemSource();
        }

        private async void EditNote(ConcernNote note)
        {
            bool ok = DrawData.Notes.TryGetValue(note.Key, out List<ConcernNote> notesFound);
            if (ok)
            {
                ConcernNote matched = notesFound.Find(x => x.ID == note.ID);
                if (matched != null)
                {
                    string result = await DisplayPromptAsync("Edit Information", $"Notes on {note.Key} Area?",
                    placeholder: "Add more information", keyboard: Keyboard.Text, initialValue: matched.Message);

                    if (!string.IsNullOrEmpty(result))
                    {
                        matched.Message = result;
                    }
                }
            }
        }

        private void DeleteNote(ConcernNote note)
        {
            bool ok = DrawData.Notes.TryGetValue(note.Key, out List<ConcernNote> notesFound);
            if (ok)
            {
                ConcernNote matched = notesFound.Find(x => x.ID == note.ID);
                if (matched != null)
                    notesFound.Remove(matched);
            }
        }

        void listNotes_Refreshing(System.Object sender, System.EventArgs e)
        {
            listNotes.ItemsSource = SetUpNotesListItemSource();
            listNotes.IsRefreshing = false;
        }
    }
}
