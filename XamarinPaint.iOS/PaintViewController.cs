using AdvancedColorPicker;
using Foundation;
using UIKit;

namespace XamarinPaint.iOS
{
    public partial class PaintViewController : UIViewController
    {
        public UIImage BackgroundImage { get; }

        public PaintViewController(UIImage backgroundImage = null) : base("PaintViewController", null)
        {
            BackgroundImage = backgroundImage;
        }

        #region View life cylce

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Configure BackgroundView
            ImageView.Image = BackgroundImage;

            // Configure CanvasView
            DrawView.BackgroundColor = UIColor.Clear;
            DrawView.DrawColor = UIColor.Black;
        }

        public override bool ShouldAutorotate()
        {
            return true;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIInterfaceOrientationMask.LandscapeRight | UIInterfaceOrientationMask.LandscapeLeft;
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            DrawView.DrawTouches(touches, evt);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            DrawView.DrawTouches(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            DrawView.DrawTouches(touches, evt);
            DrawView.EndTouches(touches, false);
        }

        #endregion

        #region button actions

        partial void DrawColorButton_Activated(UIBarButtonItem sender)
        {
            ColorPickerViewController.Present(
                NavigationController,
                "Pick a color!",
                DrawView.DrawColor,
                color =>
                {
                    DrawView.DrawColor = color ?? UIColor.Black;
                });
        }

        partial void UndoButton_Activated(UIBarButtonItem sender)
        {
            DrawView.Undo();
        }

        partial void ClearViewButton_Activated(UIBarButtonItem sender)
        {
            DrawView.Clear();
        }

        #endregion
    }
}