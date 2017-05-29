using System;
using AdvancedColorPicker;
using CoreGraphics;
using Foundation;
using UIKit;
using XamarinPaint.iOS.Enum;

namespace XamarinPaint.iOS.Demo
{
    public class CustomPaintViewController : PaintViewController
    {
        #region View life cylce

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "XamarinPaint.iOS Demo";

            // Configure paint view
            BackgroundImage = UIImage.FromFile("Hilux.png");
            DrawColor = UIColor.Red;

            // Configure toolbar
            NavigationController?.SetToolbarHidden(false, false);
            UpdateToolbarItems();
        }

        #endregion

        #region Button actions

        private void DrawColorButtonPressed(object sender, EventArgs args)
        {
            ColorPickerViewController.Present(
                NavigationController,
                "Pick a color!",
                DrawColor,
                color =>
                {
                    DrawColor = color ?? UIColor.Black;
                });
        }

        private void SnapshotButtonPressed(object sender, EventArgs args)
        {
            var image = TakeShnapshotFromView();
            if (image != null)
            {
                SaveImage(image);
            }
        }

        #endregion

        #region Private methods

        private void UpdateToolbarItems()
        {
            var trashButton = new UIBarButtonItem(UIBarButtonSystemItem.Trash);
            trashButton.Clicked += (sender, args) => Clear();

            var undoButton = new UIBarButtonItem("Undo", UIBarButtonItemStyle.Plain, (o, args) => Undo());

            var drawColorButton = new UIBarButtonItem("Draw Color", UIBarButtonItemStyle.Plain, DrawColorButtonPressed);

            var takeSnapshotButton = new UIBarButtonItem("Snapshot", UIBarButtonItemStyle.Done, SnapshotButtonPressed);

            var drawModeButton = new UIBarButtonItem(CreateDrawModeSegmentControl());

            var flexibleSpace = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);

            SetToolbarItems(new[] { trashButton, flexibleSpace, drawModeButton, flexibleSpace, undoButton, drawColorButton, takeSnapshotButton }, false);
        }

        private void SaveImage(UIImage image)
        {
            var docsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filename = System.IO.Path.Combine(docsDir, "Snapshot.png");

            var imageData = image.AsPNG();
            if (imageData.Save(filename, false, out NSError error))
            {
                Console.WriteLine($"Saved snapshot at '{filename}'");
            }
            else
            {
                Console.WriteLine($"Can't save image => {error}");
            }
        }

        private UISegmentedControl CreateDrawModeSegmentControl()
        {
            var segmentedControl = new UISegmentedControl("Line", "Cross", "Circle")
            {
                Frame = new CGRect(0, 0, 200.0, 44.0),
                ControlStyle = UISegmentedControlStyle.Bar,
                SelectedSegment = 0
            };

            segmentedControl.ValueChanged += delegate (object sender, EventArgs args)
            {
                if (segmentedControl.SelectedSegment == 0)
                {
                    DrawMode = DrawMode.Line;
                }
                else if (segmentedControl.SelectedSegment == 1)
                {
                    DrawMode = DrawMode.Cross;
                }
                else if (segmentedControl.SelectedSegment == 2)
                {
                    DrawMode = DrawMode.Circle;
                }
            };

            return segmentedControl;
        }

        #endregion
    }
}