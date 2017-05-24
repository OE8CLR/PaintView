using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
using XamarinPaint.Helpers;
using XamarinPaint.Model;

namespace XamarinPaint.UI {

	public partial class CanvasView : UIView
    {
        private readonly bool _isPredictionEnabled = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;
		private bool _needsFullRedraw = true;

		// List containing all line objects that need to be drawn in Draw(CGRect rect)
		private readonly List<Line> _lines = new List<Line>();

		// List containing all line objects that have been completely drawn into the frozenContext.
		private readonly List<Line> _finishedLines = new List<Line>();

		// Holds a map of UITouch objects to Line objects whose touch has not ended yet.
		private readonly Dictionary<UITouch, Line> _activeLines = new Dictionary<UITouch, Line>();

		// Holds a map of UITouch objects to Line objects whose touch has ended but still has points awaiting updates.
		private readonly Dictionary<UITouch, Line> _pendingLines = new Dictionary<UITouch, Line>();

		bool _usePreciseLocations;
		public bool UsePreciseLocations {
			get => _usePreciseLocations;
		    set {
				_usePreciseLocations = value;
				_needsFullRedraw = true;
				SetNeedsDisplay ();
			}
		}

		// An CGImage containing the last representation of lines no longer receiving updates.
		private CGImage _frozenImage;

		CGBitmapContext _frozenContext;
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

        public UIColor DrawColor { get; set; }

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

                foreach (var line in _finishedLines)
                {
                    line.DrawCommitedPointsInContext(FrozenContext, UsePreciseLocations);
                }

                _needsFullRedraw = false;
            }

            _frozenImage = _frozenImage ?? FrozenContext.ToImage();

            if (_frozenImage != null)
            {
                context.DrawImage(Bounds, _frozenImage);
            }

            foreach (var line in _lines)
            {
                line.DrawInContext(context, UsePreciseLocations);
            }
        }

        #endregion

        #region Public methods

        public void DrawTouches(NSSet touches, UIEvent evt)
        {
            var updateRect = CGRectExtensions.CGRectNull();

            foreach (var touch in touches.Cast<UITouch>())
            {
                Line line;

                // Retrieve a line from activeLines. If no line exists, create one.
                if (!_activeLines.TryGetValue(touch, out line))
                {
                    line = AddActiveLineForTouch(touch);
                }

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
            SetNeedsDisplayInRect(updateRect);
        }

        public void EndTouches(NSSet touches, bool cancel)
        {
            var updateRect = CGRectExtensions.CGRectNull();

            foreach (var touch in touches.Cast<UITouch>())
            {
                // Skip over touches that do not correspond to an active line.
                Line line;
                if (!_activeLines.TryGetValue(touch, out line)) continue;

                // If this is a touch cancellation, cancel the associated line.
                if (cancel) updateRect = updateRect.UnionWith(line.Cancel());

                // If the line is complete (no points needing updates) or updating isn't enabled, move the line to the frozenImage.
                if (line.IsComplete)
                {
                    FinishLine(line);
                }
                else
                {
                    _pendingLines.Add(touch, line);
                }

                // This touch is ending, remove the line corresponding to it from `activeLines`.
                _activeLines.Remove(touch);
            }

            SetNeedsDisplayInRect(updateRect);
        }

        public void Undo()
        {
            var lastElement = _finishedLines.LastOrDefault();
            if (lastElement != null)
            {
                _finishedLines.Remove(lastElement);
                _needsFullRedraw = true;
                SetNeedsDisplay();
            }
        }

        public void Clear()
        {
            _activeLines.Clear();
            _pendingLines.Clear();
            _lines.Clear();
            _finishedLines.Clear();
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

		private Line AddActiveLineForTouch(UITouch touch)
		{
			var newLine = new Line(DrawColor, 5.0f);
			_activeLines.Add(touch, newLine);
			_lines.Add(newLine);
			return newLine;
		}

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
			_lines.Remove(line);
			_finishedLines.Add(line);
		}

        #endregion
    }
}
