using System;
using CoreGraphics;
using Foundation;
using UIKit;
using XamarinPaint.iOS.Enum;

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

        private DrawMode _drawMode = DrawMode.Line;
        public DrawMode DrawMode
        {
            get => DrawView?.DrawMode ?? _drawMode;
            set
            {
                if (DrawView != null)
                {
                    DrawView.DrawMode = value;
                }
                else
                {
                    _drawMode = value;
                }
            }
        }

        private LineMode _lineMode = LineMode.Continuous;
        public LineMode LineMode
        {
            get => DrawView?.LineMode ?? _lineMode;
            set
            {
                if (DrawView != null)
                {
                    DrawView.LineMode = value;
                }
                else
                {
                    _lineMode = value;
                }
            }
        }

        private TextMode _textMode = TextMode.Medium;
        public TextMode TextMode
        {
            get => DrawView?.TextMode ?? _textMode;
            set
            {
                if (DrawView != null)
                {
                    DrawView.TextMode = value;
                }
                else
                {
                    _textMode = value;
                }
            }
        }

        private nfloat _drawLineWidth = 2.0f;
        public nfloat DrawLineWidth
        {
            get => DrawView?.DrawLineWidth ?? _drawLineWidth;
            set
            {
                if (DrawView != null)
                {
                    DrawView.DrawLineWidth = value;
                }
                else
                {
                    _drawLineWidth = value;
                }
            }
        }

        private string _text;
        public string Text
        {
            get => DrawView?.Text ?? _text;
            set
            {
                if (DrawView != null)
                {
                    DrawView.Text = value;
                }
                else
                {
                    _text = value;
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
            DrawView.DrawMode = _drawMode;
            DrawView.DrawLineWidth = _drawLineWidth;
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
            DrawView.EndTouches(touches);
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