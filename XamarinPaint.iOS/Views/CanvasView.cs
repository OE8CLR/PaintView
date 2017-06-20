using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
using XamarinPaint.Helpers;
using XamarinPaint.iOS.Enum;
using XamarinPaint.iOS.Interfaces;
using XamarinPaint.iOS.Model;

namespace XamarinPaint.iOS.Views {

	public partial class CanvasView : UIView
    {
        private readonly bool _isPredictionEnabled = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;
		private bool _needsFullRedraw = true;

        // List containing all drawElement objects that need to be drawn in Draw(CGRect rect)
        private readonly List<IDrawElement> _drawElements = new List<IDrawElement>();

        // List containing all drawElement objects that have been completely drawn into the frozenContext.
        private readonly List<IDrawElement> _finishedDrawElements = new List<IDrawElement>();

        // Holds a map of UITouch objects to drawElement objects whose touch has not ended yet.
        private readonly Dictionary<UITouch, IDrawElement> _activeDrawElements = new Dictionary<UITouch, IDrawElement>();

		bool _usePreciseLocations;
		public bool UsePreciseLocations {
			get => _usePreciseLocations;
		    set {
				_usePreciseLocations = value;
				_needsFullRedraw = true;
				SetNeedsDisplay ();
			}
		}

        // An CGImage containing the last representation of drawElements no longer receiving updates.
        private CGImage _frozenImage;

		private CGBitmapContext _frozenContext;
		public CGBitmapContext FrozenContext {
			get {
				if (_frozenContext == null) {
					var scale = Window.Screen.Scale;
					var size = Bounds.Size;

					size.Width *= scale;
					size.Height *= scale;
					var colorSpace = CGColorSpace.CreateDeviceRGB();

					_frozenContext = new CGBitmapContext(null, (nint)size.Width, (nint)size.Height, 8, 0, colorSpace, CGImageAlphaInfo.PremultipliedLast);
					_frozenContext.SetLineCap(CGLineCap.Round);
					_frozenContext.ConcatCTM(CGAffineTransform.MakeScale (scale, scale));
				}

				return _frozenContext;
			}
		}

        // Configure draw properties
        public DrawMode DrawMode { get; set; }    = DrawMode.Line;
        public LineMode LineMode { get; set; }    = LineMode.Continuous;
        public TextMode TextMode { get; set; }    = TextMode.Medium;
        public UIColor DrawColor { get; set; }    = UIColor.Black;
        public CGSize DrawSize { get; set; }      = new CGSize(25.0, 25.0);
        public nfloat DrawLineWidth { get; set; } = 5.0f;
        public string Text { get; set; }          = null;


        [Export ("initWithCoder:")]
		public CanvasView (NSCoder coder) : base (coder)
		{
		}

		[Export ("initWithFrame:")]
		public CanvasView (CGRect frame) : base (frame)
		{
		}

		public CanvasView(IntPtr handle) : base(handle)
		{
		}


        #region View life cylce

        public override void Draw(CGRect rect)
        {
            var context = UIGraphics.GetCurrentContext();
            context.SetLineCap(CGLineCap.Round);

            if (_needsFullRedraw)
            {
                SetFrozenImageNeedsUpdate();
                FrozenContext.ClearRect(Bounds);

                foreach (var drawElement in _finishedDrawElements)
                {
                    var line = drawElement as Line;
                    if (line != null)
                    {
                        line.DrawCommitedPointsInContext(FrozenContext, UsePreciseLocations);
                    }
                    else
                    {
                        drawElement.DrawInContext(FrozenContext, UsePreciseLocations);
                    }
                }

                _needsFullRedraw = false;
            }

            _frozenImage = _frozenImage ?? FrozenContext.ToImage();

            if (_frozenImage != null)
            {
                context.DrawImage(Bounds, _frozenImage);
            }

            foreach (var drawElement in _drawElements)
            {
                drawElement.DrawInContext(context, UsePreciseLocations);
            }
        }

        #endregion

        #region Public methods

        public void DrawTouches(NSSet touches, UIEvent evt)
        {
            var updateRect = CGRectExtensions.CGRectNull();

            foreach (var touch in touches.Cast<UITouch>())
            {
                IDrawElement drawElement;

                // Retrieve a drawElement from activeLines. If no line exists, create one.
                if (!_activeDrawElements.TryGetValue(touch, out drawElement))
                {
                    drawElement = AddActiveDrawElement(touch);
                }

                var line = drawElement as Line;
                if (line != null)
                {
                    // Remove prior predicted points and update the updateRect based on the removals. The touches
                    // used to create these points are predictions provided to offer additional data. They are stale
                    // by the time of the next event for this touch.
                    updateRect = updateRect.UnionWith(line.RemovePointsWithType(PointType.Predicted));

                    // Incorporate coalesced touch data. The data in the last touch in the returned array will match
                    // the data of the touch supplied to GetCoalescedTouches
                    var coalescedTouches = evt.GetCoalescedTouches(touch) ?? new UITouch[0];
                    var coalescedRect = AddPointsOfType(PointType.Coalesced, coalescedTouches, line);
                    updateRect = updateRect.UnionWith(coalescedRect);

                    // Incorporate predicted touch data. This sample draws predicted touches differently; however, 
                    // you may want to use them as inputs to smoothing algorithms rather than directly drawing them. 
                    // Points derived from predicted touches should be removed from the line at the next event for this touch.
                    if (_isPredictionEnabled)
                    {
                        var predictedTouches = evt.GetPredictedTouches(touch) ?? new UITouch[0];
                        var predictedRect = AddPointsOfType(PointType.Predicted, predictedTouches, line);
                        updateRect = updateRect.UnionWith(predictedRect);
                    }
                }
                else
                {
                    CommitDrawElement(drawElement);
                    updateRect = updateRect.UnionWith(drawElement.Frame);
                } 
            }
            SetNeedsDisplayInRect(updateRect);
        }

        public void EndTouches(NSSet touches)
        {
            var updateRect = CGRectExtensions.CGRectNull();

            foreach (var touch in touches.Cast<UITouch>())
            {
                // Skip over touches that do not correspond to an active drawElement.
                IDrawElement drawElement;
                if (!_activeDrawElements.TryGetValue(touch, out drawElement)) continue;

                // If the drawElement is complete (no points needing updates) or updating isn't enabled, move the drawElement to the frozenImage.
                if (drawElement.IsComplete)
                {
                    var line = drawElement as Line;
                    if (line != null)
                    {
                        FinishLine(line);
                    }
                    else
                    {
                        FinishDrawElement(drawElement);
                    }
                }

                // This touch is ending, remove the drawElement corresponding to it from `activeDrawElements`.
                _activeDrawElements.Remove(touch);
            }

            SetNeedsDisplayInRect(updateRect);
        }

        public void Undo()
        {
            var lastElement = _finishedDrawElements.LastOrDefault();
            if (lastElement != null)
            {
                _finishedDrawElements.Remove(lastElement);
                _needsFullRedraw = true;
                SetNeedsDisplay();
            }
        }

        public void Clear()
        {
            _activeDrawElements.Clear();
            _drawElements.Clear();
            _finishedDrawElements.Clear();
            _needsFullRedraw = true;
            SetNeedsDisplay();
        }

        #endregion

        #region Private methods

        private void SetFrozenImageNeedsUpdate()
		{
			_frozenImage?.Dispose ();
			_frozenImage = null;
		}

        private IDrawElement AddActiveDrawElement(UITouch touch)
        {
            IDrawElement newElement;

            switch (DrawMode)
            {
                case DrawMode.Line:
                {
                    newElement = new Line(DrawColor, GetDashPattern(), DrawLineWidth);
                    break;
                }
                case DrawMode.Cross:
                {
                    var view = touch.View;
                    var location = touch.LocationInView(view);

                    newElement = new Cross(DrawColor, DrawLineWidth)
                    {
                        CenterPoint = new CGPoint(location.X, location.Y),
                        Size = DrawSize
                    };
                    break;
                }
                case DrawMode.Circle:
                {
                    var view = touch.View;
                    var location = touch.LocationInView(view);

                    newElement = new Circle(DrawColor, DrawLineWidth)
                    {
                        CenterPoint = new CGPoint(location.X, location.Y),
                        Diameter = (float)(DrawSize.Width / 2.0)
                    };
                    break;
                }
                case DrawMode.Text:
                {
                    var view = touch.View;
                    var location = touch.LocationInView(view);
                        
                    newElement = new Text(Text ?? "<NO_VALUE>", DrawColor, GetTextFont())
                    {
                        Location = new CGPoint(location.X, location.Y)
                    };

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _activeDrawElements.Add(touch, newElement);
            _drawElements.Add(newElement);

            return newElement;
        }

        private void CommitDrawElement(IDrawElement drawElement)
        {
            // Have the drawElement draw any segments between points no longer being updated into the frozenContext and remove them from the drawElement.
            drawElement.DrawInContext(FrozenContext, UsePreciseLocations);

            SetFrozenImageNeedsUpdate();
        }

        private void FinishDrawElement(IDrawElement drawElement)
        {
            drawElement.DrawInContext(FrozenContext, UsePreciseLocations);

            SetFrozenImageNeedsUpdate();

            _drawElements.Remove(drawElement);
            _finishedDrawElements.Add(drawElement);
        }

        private nfloat[] GetDashPattern()
        {
            // Patterns be like: "new[] { LINEWIDTH, LINESPACE }"

            switch (LineMode)
            {
                case LineMode.Dashed:
                    return new[] { new nfloat(5.0), new nfloat(10.0) };
                case LineMode.Dotted:
                    return new[] { new nfloat(1.0), new nfloat(10.0) };
                case LineMode.DashedDotted:
                    return new[] { new nfloat(5.0), new nfloat(10.0), new nfloat(1.0), new nfloat(10.0) };
                default:
                    return null;
            }
        }

        private UIFont GetTextFont()
        {
            switch (TextMode)
            {
                case TextMode.Small:
                    return UIFont.FromName("Helvetica-Bold", 14.0f);
                case TextMode.Large:
                    return UIFont.FromName("Helvetica-Bold", 30.0f);
                default:
                    return UIFont.FromName("Helvetica-Bold", 20.0f);
            }
        }

        #region Lines

        private CGRect AddPointsOfType(PointType type, UITouch[] touches, Line line)
		{
			var accumulatedRect = CGRectExtensions.CGRectNull();

			for (var i = 0; i < touches.Length; i++)
            {
				var touch = touches[i];
				// The visualization displays non-`.Stylus` touches differently.
				if (touch.Type != UITouchType.Stylus) type |= PointType.Finger;

				// Touches with estimated properties require updates; add this information to the `PointType`.
				if (touch.EstimatedProperties != 0) type |= PointType.NeedsUpdate;

				// The last touch in a set of .Coalesced touches is the originating touch. Track it differently.
				bool isLast = (i == touches.Length - 1);
				if (type.HasFlag (PointType.Coalesced) && isLast)
                {
					type &= ~PointType.Coalesced;
					type |= PointType.Standard;
				}

				var touchRect = line.AddPointOfType(type, touch);
				accumulatedRect = accumulatedRect.UnionWith(touchRect);
				CommitLine(line);
			}

			return accumulatedRect;
		}

		private void CommitLine(Line line)
		{
            // Have the line draw any segments between points no longer being updated into the frozenContext and remove them from the line.
            line.DrawFixedPointsInContext(FrozenContext, false, UsePreciseLocations);
            SetFrozenImageNeedsUpdate();
		}

		private void FinishLine(Line line)
		{
            line.DrawFixedPointsInContext(FrozenContext, UsePreciseLocations, true);
			SetFrozenImageNeedsUpdate();

			_drawElements.Remove(line);
			_finishedDrawElements.Add(line);
		}

        #endregion

        #endregion
    }
}
