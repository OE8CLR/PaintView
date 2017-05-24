// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace XamarinPaint.UI
{
    [Register ("MainViewController")]
    partial class MainViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        XamarinPaint.UI.CanvasView CanvasView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem ColorPickerButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem UndoButton { get; set; }
        [Action ("ClearView:")]
        partial void ClearView (UIKit.UIBarButtonItem sender);


        [Action ("ToggleDebugDrawing:")]
        partial void ToggleDebugDrawing (UIKit.UIButton sender);


        [Action ("ToggleUsePreciseLocations:")]
        partial void ToggleUsePreciseLocations (UIKit.UIButton sender);

        [Action ("ColorPickerButton_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ColorPickerButton_Activated (UIKit.UIBarButtonItem sender);

        [Action ("UndoButton_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UndoButton_Activated (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (CanvasView != null) {
                CanvasView.Dispose ();
                CanvasView = null;
            }

            if (ColorPickerButton != null) {
                ColorPickerButton.Dispose ();
                ColorPickerButton = null;
            }

            if (UndoButton != null) {
                UndoButton.Dispose ();
                UndoButton = null;
            }
        }
    }
}