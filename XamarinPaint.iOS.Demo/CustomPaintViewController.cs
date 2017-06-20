using System;
using System.Collections.Generic;
using System.Linq;
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

        private void ColorButtonPressed(object sender, EventArgs args)
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
                DrawMode.Cross,
                DrawMode.Text
            };

            var drawModeView = new TableViewSelector<DrawMode>(drawModes, DrawMode);
            drawModeView.OnSelected += (o, mode) =>
            {
                DrawMode = mode;
                UpdateToolbarItems(false);

                if (mode == DrawMode.Text) UpdateTextValue();
            };

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

        private void TextModeButtonPressed(object sender, EventArgs args)
        {
            PresentedViewController?.DismissViewController(false, null);

            var textModes = new List<TextMode>
            {
                TextMode.Small,
                TextMode.Medium,
                TextMode.Large
            };

            var lineModeView = new TableViewSelector<TextMode>(textModes, TextMode);
            lineModeView.OnSelected += (o, mode) => TextMode = mode;

            var popoverView = lineModeView.PopoverPresentationController;
            if (popoverView != null)
            {
                popoverView.BarButtonItem = sender as UIBarButtonItem;
            }

            PresentViewController(lineModeView, true, null);
        }

        private void SetTextButtonPressed(object sender, EventArgs args)
        {
            UpdateTextValue();
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
            var flexibleSpace = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);

            var buttons = new List<UIBarButtonItem>();

            buttons.Add(flexibleSpace);

            var drawModeButton = new UIBarButtonItem("DrawMode", UIBarButtonItemStyle.Plain, DrawModeButtonPressed);
            buttons.Add(drawModeButton);

            if (DrawMode == DrawMode.Text)
            {
                var textModeButton = new UIBarButtonItem("TextMode", UIBarButtonItemStyle.Plain, TextModeButtonPressed);
                buttons.Add(textModeButton);

                var textButton = new UIBarButtonItem("SetText", UIBarButtonItemStyle.Plain, SetTextButtonPressed);
                buttons.Add(textButton);
            }
            else
            {
                var lineModeButton = new UIBarButtonItem("LineMode", UIBarButtonItemStyle.Plain, LineModeButtonPressed);
                buttons.Add(lineModeButton);
            }

            var colorButton = new UIBarButtonItem("Color", UIBarButtonItemStyle.Plain, ColorButtonPressed);
            buttons.Add(colorButton);

            buttons.Add(flexibleSpace);

            SetToolbarItems(buttons.ToArray(), animated);
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

        private void UpdateTextValue()
        {
            var alert = UIAlertController.Create("", "Set the text for the 'TextMode'", UIAlertControllerStyle.Alert);

            alert.AddTextField(textField =>
            {
                textField.Placeholder = "Enter text ...";
                textField.Text = !string.IsNullOrEmpty(Text) ? Text : null;
            });

            var okAction = UIAlertAction.Create("Ok", UIAlertActionStyle.Default, action =>
            {
                var text = alert.TextFields.FirstOrDefault()?.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    Text = text;
                }
            });
            alert.AddAction(okAction);

            var cancelAction = UIAlertAction.Create("Cancle", UIAlertActionStyle.Cancel, null);
            alert.AddAction(cancelAction);

            PresentViewController(alert, true, null);
        }

        #endregion
    }
}