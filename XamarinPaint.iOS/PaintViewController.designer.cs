// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace XamarinPaint.iOS
{
    [Register ("PaintViewController")]
    partial class PaintViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem ClearViewButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem DrawColorButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        XamarinPaint.UI.CanvasView DrawView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem UndoButton { get; set; }

        [Action ("ClearViewButton_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ClearViewButton_Activated (UIKit.UIBarButtonItem sender);

        [Action ("DrawColorButton_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void DrawColorButton_Activated (UIKit.UIBarButtonItem sender);

        [Action ("UndoButton_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UndoButton_Activated (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (ClearViewButton != null) {
                ClearViewButton.Dispose ();
                ClearViewButton = null;
            }

            if (DrawColorButton != null) {
                DrawColorButton.Dispose ();
                DrawColorButton = null;
            }

            if (DrawView != null) {
                DrawView.Dispose ();
                DrawView = null;
            }

            if (UndoButton != null) {
                UndoButton.Dispose ();
                UndoButton = null;
            }
        }
    }
}