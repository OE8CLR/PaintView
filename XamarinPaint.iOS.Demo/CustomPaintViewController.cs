using System;
using AdvancedColorPicker;
using Foundation;
using UIKit;

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

            var flexibleSpace = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);

            SetToolbarItems(new[] { trashButton, flexibleSpace, undoButton, drawColorButton, takeSnapshotButton }, false);
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

        #endregion
    }
}