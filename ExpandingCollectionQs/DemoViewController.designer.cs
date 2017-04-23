// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace ExpandingCollectionQs
{
    [Register ("DemoViewController")]
    partial class DemoViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ctestLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ctestLabel != null) {
                ctestLabel.Dispose ();
                ctestLabel = null;
            }
        }
    }
}