using System;
using Foundation;
using UIKit;
using XamarinPaint.iOS;

namespace XamarinPaint {

	public partial class MainViewController : UIViewController
    {
        [Export ("initWithCoder:")]
		public MainViewController (NSCoder coder) : base (coder)
		{
		}

		public MainViewController (IntPtr handle) : base (handle)
		{
		}

        #region View life cylce

        public override bool ShouldAutorotate()
        {
            return true;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIInterfaceOrientationMask.LandscapeRight | UIInterfaceOrientationMask.LandscapeLeft;
        }

        partial void UIButton3QL7Vowf_TouchUpInside(UIButton sender)
        {
            NavigationController.PushViewController(new PaintViewController(), true);
        }

        #endregion
    }
}
