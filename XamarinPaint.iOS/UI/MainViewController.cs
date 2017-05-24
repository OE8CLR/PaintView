using System;
using AdvancedColorPicker;
using CoreGraphics;
using Foundation;
using UIKit;
using XamarinPaint.Helpers;

namespace XamarinPaint.UI {

	public partial class MainViewController : UIViewController
    {
		public CanvasView CanvasView => (CanvasView)View;

        [Export ("initWithCoder:")]
		public MainViewController (NSCoder coder) : base (coder)
		{
		}

		public MainViewController (IntPtr handle) : base (handle)
		{
		}

        #region View life cylce

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            CanvasView.DrawColor = UIColor.Black;
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
            CanvasView.DrawTouches(touches, evt);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            CanvasView.DrawTouches(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            CanvasView.DrawTouches(touches, evt);
            CanvasView.EndTouches(touches, false);
        }

        #endregion

        #region button actions

        partial void ClearView(UIBarButtonItem sender)
		{
			CanvasView.Clear();
		}

        partial void ColorPickerButton_Activated(UIBarButtonItem sender)
        {
            ColorPickerViewController.Present(
                NavigationController,
                "Pick a color!",
                CanvasView.DrawColor,
                color => {
                    CanvasView.DrawColor = color ?? UIColor.Black;
                });
        }

        partial void UndoButton_Activated(UIBarButtonItem sender)
        {
            CanvasView.Undo();
        }

        #endregion
    }
}
