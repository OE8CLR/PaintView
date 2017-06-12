using System;
using CoreGraphics;
using UIKit;
using XamarinPaint.iOS.Enum;

namespace XamarinPaint.iOS.Interfaces
{
    public interface IDrawElement
    {
        bool IsComplete { get; }
        UIColor LineColor { get; }
        nfloat LineWidth { get; }
        nfloat[] LineDash { get; }
        CGRect Frame { get; }

        void DrawInContext(CGContext context, bool usePreciseLocation);
    }
}