using Foundation;
using UIKit;

namespace XamarinPaint.iOS.Demo
{
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
    {
		public override UIWindow Window { get; set; }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            var image = UIImage.FromFile("Hilux.png");
            var paintView = new PaintViewController(image);
            Window.RootViewController = new UINavigationController(paintView);

            Window.MakeKeyAndVisible();

            return true;
        }
    }
}

