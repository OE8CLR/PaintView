using System;
using UIKit;

namespace XamarinPaint.iOS.Interfaces
{
    public interface IDrawLineElement
    {
        UIColor LineColor { get; }
        nfloat LineWidth { get; }
        nfloat[] LineDash { get; }
    }
}