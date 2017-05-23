using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace TouchCanvas.Model {

	[Flags]
	public enum PointType {
		Standard = 0,
		Coalesced = 1 << 0,
		Predicted = 1 << 1,
		NeedsUpdate = 1 << 2,
		Updated = 1 << 3,
		Cancelled = 1 << 4,
		Finger = 1 << 5
	};

	public class LinePoint {

		public int SequenceNumber { get; }

		public double Timestamp { get; set; }

		public CGPoint Location { get; set; }

		public CGPoint PreciseLocation { get; set; }

		public PointType PointType { get; set; }

		public UITouchProperties EstimatedPropertiesExpectingUpdates { get; }

		public UITouchType Type { get; }

		public nfloat AltitudeAngle { get; }

		public nfloat AzimuthAngle { get; }

		public NSNumber EstimationUpdateIndex { get; }

	    public LinePoint(UITouch touch, int sequenceNumber, PointType pointType)
		{
			SequenceNumber = sequenceNumber;
			Type = touch.Type;
			PointType = pointType;
			Timestamp = touch.Timestamp;

			var view = touch.View;
			Location = touch.LocationInView(view);
			PreciseLocation = touch.GetPreciseLocation(view);
			AzimuthAngle = touch.GetAzimuthAngle(view);

			EstimatedPropertiesExpectingUpdates = touch.EstimatedPropertiesExpectingUpdates;
			AltitudeAngle = touch.AltitudeAngle;

		    if (EstimatedPropertiesExpectingUpdates != 0)
		    {
		        PointType |= PointType.NeedsUpdate;
		    }

            EstimationUpdateIndex = touch.EstimationUpdateIndex;
		}
	}
}