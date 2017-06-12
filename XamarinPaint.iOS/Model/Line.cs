using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
using XamarinPaint.Helpers;
using XamarinPaint.iOS.Enum;
using XamarinPaint.iOS.Interfaces;

namespace XamarinPaint.iOS.Model {

	public class Line : NSObject, IDrawElement
    {
		private readonly Dictionary<NSNumber, LinePoint> _pointsWaitingForUpdatesByEstimationIndex = new Dictionary<NSNumber, LinePoint>();

		public List<LinePoint> Points { get; } = new List<LinePoint> ();
        public List<LinePoint> CommittedPoints { get; } = new List<LinePoint> ();

        public bool IsComplete => _pointsWaitingForUpdatesByEstimationIndex.Count == 0;
        public UIColor LineColor { get; }
        public nfloat LineWidth { get; }
        public nfloat[] LineDash { get; }

        public CGRect Frame => CGRectExtensions.CGRectNull();

        public Line(UIColor color, nfloat[] lineDash, nfloat width)
        {
            LineColor = color ?? UIColor.Black;
            LineWidth = width != 0 ? width : 1.0f;
            LineDash = lineDash ?? new[] { new nfloat(1.0), new nfloat(0) };
        }

        #region public methods

        public CGRect AddPointOfType(PointType pointType, UITouch touch)
        {
            var previousPoint = Points.LastOrDefault();
            var previousSequenceNumber = previousPoint?.SequenceNumber ?? -1;
            var point = new LinePoint(touch, previousSequenceNumber + 1, pointType);

            if (point.EstimationUpdateIndex != null && point.EstimatedPropertiesExpectingUpdates != 0)
            {
                _pointsWaitingForUpdatesByEstimationIndex[point.EstimationUpdateIndex] = point;
            }

            Points.Add(point);

            return UpdateRectForLinePoint(point, previousPoint);
        }

        public CGRect RemovePointsWithType(PointType type)
        {
            var updateRect = CGRectExtensions.CGRectNull();

            LinePoint priorPoint = null;
            for (var i = Points.Count - 1; i >= 0; i--)
            {
                var point = Points[i];
                if (point.PointType.HasFlag(type))
                {
                    Points.RemoveAt(i);
                    updateRect = updateRect.UnionWith(CalcUpdateRectFor(point));
                    updateRect = updateRect.UnionWith(CalcUpdateRectFor(priorPoint));
                }
                priorPoint = point;
            }

            return updateRect;
        }

        public void DrawInContext(CGContext context, bool usePreciseLocation)
        {
            LinePoint maybePriorPoint = null;

            foreach (var point in Points)
            {
                if (maybePriorPoint == null)
                {
                    maybePriorPoint = point;
                    continue;
                }

                var priorPoint = maybePriorPoint;

                var location = usePreciseLocation ? point.PreciseLocation : point.Location;
                var priorLocation = usePreciseLocation ? priorPoint.PreciseLocation : priorPoint.Location;

                context.BeginPath();

                context.MoveTo(priorLocation.X, priorLocation.Y);
                context.AddLineToPoint(location.X, location.Y);

                context.SetStrokeColor(LineColor.CGColor);
                context.SetLineWidth(LineWidth);
                context.SetLineDash(0, LineDash);

                context.StrokePath();

                maybePriorPoint = point;
            }
        }

        public void DrawFixedPointsInContext(CGContext context, bool usePreciseLocation, bool commitAll = false)
        {
            var allPoints = new List<LinePoint>(Points);
            var committing = new List<LinePoint>();

            if (commitAll)
            {
                committing.AddRange(allPoints);
                Points.Clear();
            }
            else
            {
                for (var i = 0; i < allPoints.Count; i++)
                {
                    var point = allPoints[i];
                    if (!point.PointType.HasFlag(PointType.NeedsUpdate | PointType.Predicted) || i >= (allPoints.Count - 2))
                    {
                        committing.Add(Points.First());
                        break;
                    }

                    if (i > 0)
                    {
                        committing.Add(Points[0]);
                        Points.RemoveAt(0);
                    }
                }
            }

            if (committing.Count <= 1) return;

            var committedLine = new Line(LineColor, LineDash, LineWidth);
            committedLine.Points.AddRange(committing);
            committedLine.DrawInContext(context, usePreciseLocation);

            var last = CommittedPoints.Count - 1;
            if (last >= 0)
            {
                CommittedPoints.RemoveAt(last);
            }

            // Store the points being committed for redrawing later in a different style if needed.
            CommittedPoints.AddRange(committing);
        }

        public void DrawCommitedPointsInContext(CGContext context, bool usePreciseLocation)
        {
            var committedLine = new Line(LineColor, LineDash, LineWidth);
            committedLine.Points.AddRange(CommittedPoints);
            committedLine.DrawInContext(context, usePreciseLocation);
        }

        #endregion

        #region Private methods

        private CGRect CalcUpdateRectFor(LinePoint point)
		{
		    if (point == null) return CGRectExtensions.CGRectNull();

            var rect = new CGRect(point.Location, CGSize.Empty);

			// The negative magnitude ensures an outset rectangle
			var magnitude = -3 * LineWidth - 2;
			rect = rect.Inset(magnitude, magnitude);

			return rect;
		}

		private CGRect UpdateRectForLinePoint(LinePoint point, LinePoint previousPoint)
		{
			var rect = new CGRect(point.Location, CGSize.Empty);
			var pointMagnitude = LineWidth;

			if (previousPoint != null)
            {
				pointMagnitude = NMath.Max(pointMagnitude, LineWidth);
				rect = rect.UnionWith(new CGRect (previousPoint.Location, CGSize.Empty));
			}

			// The negative magnitude ensures an outset rectangle.
			var magnitude = -3 * pointMagnitude - 2;
			rect = rect.Inset(magnitude, magnitude);

			return rect;
		}

        #endregion
    }
}