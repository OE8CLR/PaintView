using System;
using CoreGraphics;
using Foundation;
using UIKit;
using XamarinPaint.iOS.Interfaces;

namespace XamarinPaint.iOS.Model
{
    public class Circle : NSObject, IDrawElement
    {
        public bool IsComplete => true;
        public UIColor LineColor { get; }
        public nfloat LineWidth { get; }
        public CGRect Frame => new CGRect(CenterPoint.X - Diameter, CenterPoint.Y - Diameter, Diameter * 2.0, Diameter * 2.0);

        public CGPoint CenterPoint { get; set; }
        public float Diameter { get; set; }

        public Circle(UIColor lineColor, nfloat lineWidth)
        {
            LineColor = lineColor ?? UIColor.Black;
            LineWidth = lineWidth != 0 ? lineWidth : 1.0f;
        }

        #region Public methods

        public void DrawInContext(CGContext context, bool usePreciseLocation)
        {
            context.SetFillColor(UIColor.Clear.CGColor);
            context.SetStrokeColor(LineColor.CGColor);
            context.SetLineWidth(LineWidth);
            context.AddEllipseInRect(new CGRect(Frame.X + (LineWidth / 2), Frame.Y + (LineWidth / 2), Frame.Width - LineWidth, Frame.Height - LineWidth));
            context.DrawPath(CGPathDrawingMode.FillStroke);
        }

        #endregion
    }
}