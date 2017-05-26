using Foundation;
using UIKit;

namespace XamarinPaint.iOS
{
    public partial class PaintViewController : UIViewController
    {
        private UIImage _cachedBackgroundImage;
        protected UIImage BackgroundImage
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
        protected UIColor DrawColor
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

        #region Protected methods 

        protected void Undo() => DrawView?.Undo();

        protected void Clear() => DrawView?.Clear();

        #endregion 

        #region Public methods

        public UIImage TakeShnapshotFromView()
        {
            UIGraphics.BeginImageContext(ContentView.Frame.Size);
            try
            {
                var context = UIGraphics.GetCurrentContext();

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