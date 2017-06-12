using System;
using System.Collections.Generic;
using AdvancedColorPicker;
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
            UpdateToolbarItems(false);

            UpdateNavigationBarItems(false);
        }

        #endregion

        #region Button actions

        private void DrawColorButtonPressed(object sender, EventArgs args)
        {
            PresentedViewController?.DismissViewController(false, null);

            ColorPickerViewController.Present(
                NavigationController,
                "Pick a color!",
                DrawColor,
                color =>
                {
                    DrawColor = color ?? UIColor.Black;
                });
        }

        private void DrawModeButtonPressed(object sender, EventArgs args)
        {
            PresentedViewController?.DismissViewController(false, null);

            var drawModes = new List<DrawMode>
            {
                DrawMode.Line,
                DrawMode.Circle,
                DrawMode.Cross
            };

            var drawModeView = new TableViewSelector<DrawMode>(drawModes, DrawMode);
            drawModeView.OnSelected += (o, mode) => DrawMode = mode;

            var popoverView = drawModeView.PopoverPresentationController;
            if (popoverView != null)
            {
                popoverView.BarButtonItem = sender as UIBarButtonItem;
            }

            PresentViewController(drawModeView, true, null);
        }

        private void LineModeButtonPressed(object sender, EventArgs args)
        {
            PresentedViewController?.DismissViewController(false, null);

            var lineModes = new List<LineMode>
            {
                LineMode.Continuous,
                LineMode.Dashed,
                LineMode.Dotted,
                LineMode.DashedDotted
            };

            var lineModeView = new TableViewSelector<LineMode>(lineModes, LineMode);
            lineModeView.OnSelected += (o, mode) => LineMode = mode;

            var popoverView = lineModeView.PopoverPresentationController;
            if (popoverView != null)
            {
                popoverView.BarButtonItem = sender as UIBarButtonItem;
            }

            PresentViewController(lineModeView, true, null);
        }

        private void SnapshotButtonPressed(object sender, EventArgs args)
        {
            var image = TakeShnapshotFromView();
            if (image != null)
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
        }

        #endregion

        #region Private methods

        private void UpdateToolbarItems(bool animated = true)
        {
            var drawColorButton = new UIBarButtonItem("DrawColor", UIBarButtonItemStyle.Plain, DrawColorButtonPressed);

            var drawModeButton = new UIBarButtonItem("DrawMode", UIBarButtonItemStyle.Plain, DrawModeButtonPressed);

            var lineModeButton = new UIBarButtonItem("LineMode", UIBarButtonItemStyle.Plain, LineModeButtonPressed);

            var flexibleSpace = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);

            SetToolbarItems(new[] { flexibleSpace, lineModeButton, drawModeButton, drawColorButton, flexibleSpace }, animated);
        }

        private void UpdateNavigationBarItems(bool animated = true)
        {
            var snapshotButton = new UIBarButtonItem("Snapshot", UIBarButtonItemStyle.Done, SnapshotButtonPressed);
            NavigationItem.SetRightBarButtonItem(snapshotButton, false);

            var trashButton = new UIBarButtonItem(UIBarButtonSystemItem.Trash);
            trashButton.Clicked += (sender, args) => Clear();

            var undoButton = new UIBarButtonItem("Undo", UIBarButtonItemStyle.Plain, (o, args) => Undo());

            NavigationItem.SetLeftBarButtonItems(new[] { trashButton, undoButton }, animated);
        }

        #endregion
    }
}