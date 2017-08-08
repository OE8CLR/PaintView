using System;
using CoreGraphics;
using Foundation;
using UIKit;
using XamarinPaint.iOS.Enum;

namespace XamarinPaint.iOS
{
    public partial class PaintViewController : UIViewController
    {
        private bool _isFirstViewDidAppear;

        private UIImage _backgroundImage;
        public UIImage BackgroundImage
        {
            get => _backgroundImage;
            set
            {
                _backgroundImage = value;

                if (ImageView != null)
                {
                    ImageView.Image = value;
                    UpdateBackgroundImageConstraints();
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
            _isFirstViewDidAppear = true;
        }


        #region View life cylce

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            DrawView.DrawColor = _cachedDrawColor ?? DrawColor;
            DrawView.DrawMode = _drawMode;
            DrawView.DrawLineWidth = _drawLineWidth;

            ImageView.Image = BackgroundImage;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (_isFirstViewDidAppear)
            {
                _isFirstViewDidAppear = false;
                UpdateBackgroundImageConstraints();
            }
        }

        #endregion

        #region Public methods

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

        #region Private methods

        private void UpdateBackgroundImageConstraints()
        {
            // Get a square with the min size from height or width
            var maxSize = View.Frame.Size;
            if (maxSize.Width > maxSize.Height)
            {
                maxSize = new CGSize(maxSize.Height, maxSize.Height);
            }
            else
            {
                maxSize = new CGSize(maxSize.Width, maxSize.Width);
            }

            // Set image view constraints
            ImageHeightConstraint.Constant = maxSize.Height;
            ImageWidthConstraint.Constant = maxSize.Width;

            View.LayoutIfNeeded();
        }

        #endregion
    }
}