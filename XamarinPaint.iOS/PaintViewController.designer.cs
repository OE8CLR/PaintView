// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using XamarinPaint.iOS.Views;

namespace XamarinPaint.iOS
{
    [Register ("PaintViewController")]
    partial class PaintViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ContentView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        XamarinPaint.iOS.Views.CanvasView DrawView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint ImageHeightConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint ImageWidthConstraint { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ContentView != null) {
                ContentView.Dispose ();
                ContentView = null;
            }

            if (DrawView != null) {
                DrawView.Dispose ();
                DrawView = null;
            }

            if (ImageHeightConstraint != null) {
                ImageHeightConstraint.Dispose ();
                ImageHeightConstraint = null;
            }

            if (ImageView != null) {
                ImageView.Dispose ();
                ImageView = null;
            }

            if (ImageWidthConstraint != null) {
                ImageWidthConstraint.Dispose ();
                ImageWidthConstraint = null;
            }
        }
    }
}