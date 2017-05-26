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
            Window.RootViewController = new UINavigationController(new CustomPaintViewController());
            Window.MakeKeyAndVisible();

            return true;
        }
    }
}

