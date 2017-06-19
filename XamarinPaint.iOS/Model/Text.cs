using CoreGraphics;
using CoreText;
using Foundation;
using UIKit;
using XamarinPaint.iOS.Interfaces;

namespace XamarinPaint.iOS.Model
{
    public class Text : NSObject, IDrawElement
    {
        public bool IsComplete => true;
        public UIColor TextColor { get; }
        public UIFont Font { get; }
        //public CGRect Frame => new CGRect(CenterPoint.X, CenterPoint.Y, 100.0, 100.0);
        public CGRect Frame => new CGRect(CenterPoint.X - 100.0, CenterPoint.Y - 100.0, 200.0, 200.0);

        public CGPoint CenterPoint { get; set; }
        public string Value { get; set; }

        public Text(string text, UIColor textColor, UIFont font)
        {
            Value = text;
            TextColor = textColor;
            Font = font;
        }

        public void DrawInContext(CGContext context, bool usePreciseLocation)
        {
            var stringAttributes = new CTStringAttributes
            {
                ForegroundColor = TextColor.CGColor,
                Font = new CTFont(Font.FamilyName, Font.PointSize)
            };

            //var attributedString = new NSAttributedString(Value, stringAttributes);
            //using (var framesetter = new CTFramesetter(attributedString))
            //{
            //    var rect = GetRectForText();

            //    var path = new CGPath();
            //    path.AddRect(new CGRect(rect.X, -rect.Y, rect.Width, rect.Height));

            //    var stringRange = new NSRange(0, attributedString.Length);
            //    using (var frame = framesetter.GetFrame(stringRange, path, null))
            //    {
            //        context.SaveState();

            //        context.TranslateCTM(0, rect.Height);
            //        context.ScaleCTM(1, -1);
            //        frame.Draw(context);

            //        context.RestoreState();
            //    }
            //}


            // SEE https://www.raywenderlich.com/153591/core-text-tutorial-ios-making-magazine-app

            var rect = GetRectForText();

            var path = new CGPath();
            path.AddRect(new CGRect(rect.X, rect.Y, rect.Width, rect.Height));

            var attributedString = new NSAttributedString(Value, stringAttributes);
            using (var framesetter = new CTFramesetter(attributedString))
            {
                var stringRange = new NSRange(0, attributedString.Length);
                using (var frame = framesetter.GetFrame(stringRange, path, null))
                {
                    context.SaveState();

                    context.TextMatrix = CGAffineTransform.MakeIdentity();
                    context.TranslateCTM(0, rect.Height);
                    //context.ScaleCTM(1, -1);

                    frame.Draw(context);

                    context.RestoreState();
                }
            }
        }

        private CGRect GetRectForText()
        {
            const double padding = 50.0;
            var size = Value.StringSize(Font);
            return new CGRect(CenterPoint.X, CenterPoint.Y, size.Width + padding, size.Height + padding);
        }
    }
}