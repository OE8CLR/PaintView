using CoreGraphics;
using CoreText;
using Foundation;
using UIKit;
using XamarinPaint.iOS.Interfaces;

namespace XamarinPaint.iOS.Model
{
    public class Text : NSObject, IDrawElement
    {
        private readonly Cross _locationMarker;

        public bool IsComplete => true;
        public UIColor TextColor { get; }
        public UIFont Font { get; }
        public string Value { get; set; }

        public CGRect Frame
        {
            get
            {
                var rect = GetRectForText();
                return new CGRect(_locationMarker.Frame.X, _locationMarker.Frame.Y, rect.Width + _locationMarker.Frame.Width, rect.Height + _locationMarker.Frame.Height);
            }
        }

        private CGPoint _location;
        public CGPoint Location
        {
            get => _location;
            set
            {
                _location = value;
                _locationMarker.CenterPoint = value;
            }
        }

        public Text(string text, UIColor textColor, UIFont font)
        {
            Value = text;
            TextColor = textColor;
            Font = font;

            _locationMarker = new Cross(textColor, 1.0f)
            {
                Size = new CGSize(5.0, 5.0)
            };
        }

        public void DrawInContext(CGContext context, bool usePreciseLocation)
        {
            _locationMarker.DrawInContext(context, usePreciseLocation);

            var rect = GetRectForText();

            var path = new CGPath();
            path.AddRect(new CGRect(rect.X, -rect.Y, rect.Width, rect.Height));

            var attributedString = new NSAttributedString(Value, new CTStringAttributes
            {
                ForegroundColor = TextColor.CGColor,
                Font = new CTFont(Font.FamilyName, Font.PointSize)
            });

            using (var framesetter = new CTFramesetter(attributedString))
            {
                var stringRange = new NSRange(0, attributedString.Length);
                using (var frame = framesetter.GetFrame(stringRange, path, null))
                {
                    context.SaveState();

                    context.TranslateCTM(0, rect.Height);
                    context.ScaleCTM(1, -1);
                    frame.Draw(context);

                    context.RestoreState();
                }
            }
        }

        private CGRect GetRectForText()
        {
            const double padding = 5.0;
            var size = Value.StringSize(Font);
            return new CGRect(Location.X, Location.Y, size.Width + padding, size.Height + padding);
        }
    }
}