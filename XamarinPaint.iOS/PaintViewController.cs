using CoreGraphics;
using Foundation;
using UIKit;

namespace XamarinPaint.iOS
{
    public partial class PaintViewController : UIViewController
    {
        private UIImage _cachedBackgroundImage;
        public UIImage BackgroundImage
        {
            get => ImageView?.Image;
            set
            {
                if (ImageView != null)
                {
                    ImageView.Image = value;
                }
                else
                {
                    _cachedBackgroundImage = value;
                }
            }
        }

        private readonly UIColor _defaultColor = UIColor.Black;
        private UIColor _cachedDrawColor;
        public UIColor DrawColor
        {
            get => DrawView?.DrawColor ?? _defaultColor;
            set
            {
                if (DrawView != null)
                {
                    DrawView.DrawColor = value ?? _defaultColor;
                }
                else
                {
                    _cachedDrawColor = value;
                }
            }
        }

        public PaintViewController() : base("PaintViewController", null)
        {
        }

        #region View life cylce

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Configure view
            ImageView.Image = _cachedBackgroundImage ?? BackgroundImage;
            DrawView.DrawColor = _cachedDrawColor ?? DrawColor;
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

        #region Public methods

        public void Undo() => DrawView?.Undo();

        public void Clear() => DrawView?.Clear();

        public UIImage TakeShnapshotFromView()
        {
            var viewSize = ContentView.Frame.Size;
            var clippedRect = new CGRect(0, 0, viewSize.Width, viewSize.Height);

            // Check if navigationBar is visible and adjust clippedRect
            if (NavigationController != null && !NavigationController.NavigationBarHidden)
            {
                clippedRect.Y += NavigationController.NavigationBar.Frame.Height;
                clippedRect.Height -= NavigationController.NavigationBar.Frame.Height;
            }

            // Check if toolbar is visible and adjust clippedRect
            if (NavigationController != null && !NavigationController.ToolbarHidden)
            {
                clippedRect.Height -= NavigationController.Toolbar.Frame.Height;
            }

            UIGraphics.BeginImageContext(viewSize);
            try
            {
                var context = UIGraphics.GetCurrentContext();
                context.ClipToRect(clippedRect);

                ContentView.Layer.RenderInContext(context);

                var image = UIGraphics.GetImageFromCurrentImageContext();
                return !image.Size.IsEmpty ? image : null;
            }
            finally
            {
                UIGraphics.EndImageContext();
            }
        }

        #endregion
    }
}