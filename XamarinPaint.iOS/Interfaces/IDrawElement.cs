using CoreGraphics;

namespace XamarinPaint.iOS.Interfaces
{
    public interface IDrawElement
    {
        bool IsComplete { get; }
        CGRect Frame { get; }

        void DrawInContext(CGContext context, bool usePreciseLocation);
    }
}