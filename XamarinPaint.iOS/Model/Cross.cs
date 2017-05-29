using System;
using CoreGraphics;
using Foundation;
using UIKit;
using XamarinPaint.iOS.Interfaces;

namespace XamarinPaint.iOS.Model
{
    public class Cross : NSObject, IDrawElement
    {
        public bool IsComplete => true;
        public UIColor LineColor { get; }
        public nfloat LineWidth { get; }
        public CGRect Frame => new CGRect(CenterPoint.X - (Size.Height / 2), CenterPoint.Y - (Size.Width / 2), Size.Width, Size.Height);

        public CGPoint CenterPoint { get; set; }
        public CGSize Size { get; set; }

        public Cross(UIColor lineColor, nfloat lineWidth)
        {
            LineColor = lineColor ?? UIColor.Black;
            LineWidth = lineWidth != 0 ? lineWidth : 1.0f;
        }

        #region Public methods

        public void DrawInContext(CGContext context, bool usePreciseLocation)
        {
            // Draw first line from top left to bottom right
            context.BeginPath();
            context.MoveTo(Frame.GetMinX(), Frame.GetMinY());
            context.AddLineToPoint(Frame.GetMaxX(), Frame.GetMaxY());
            context.SetStrokeColor(LineColor.CGColor);
            context.SetLineWidth(LineWidth);
            context.StrokePath();

            // Draw second line from top right to bottom left
            context.BeginPath();
            context.MoveTo(Frame.GetMaxX(), Frame.GetMinY());
            context.AddLineToPoint(Frame.GetMinX(), Frame.GetMaxY());
            context.SetStrokeColor(LineColor.CGColor);
            context.SetLineWidth(LineWidth);
            context.StrokePath();
        }

        #endregion
    }
}