using System;
using CoreGraphics;
using UIKit;

namespace XamarinPaint.iOS.Interfaces
{
    public interface IDrawElement
    {
        bool IsComplete { get; }
        UIColor LineColor { get; }
        nfloat LineWidth { get; }
        CGRect Frame { get; }

        void DrawInContext(CGContext context, bool usePreciseLocation);
    }
}