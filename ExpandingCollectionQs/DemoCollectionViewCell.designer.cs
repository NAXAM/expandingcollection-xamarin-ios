// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace ExpandingCollectionQs
{
	[Register ("DemoCollectionViewCell")]
	partial class DemoCollectionViewCell
	{
		//[Outlet]
		//[GeneratedCode ("iOS Designer", "1.0")]
		//UIKit.NSLayoutConstraint backConstraintY { get; set; }

		//[Outlet]
		//[GeneratedCode ("iOS Designer", "1.0")]
		//UIKit.UIView backContainerView { get; set; }

		//[Outlet]
		//[GeneratedCode ("iOS Designer", "1.0")]
		//UIKit.NSLayoutConstraint frontConstraintY { get; set; }

		//[Outlet]
		//[GeneratedCode ("iOS Designer", "1.0")]
		//UIKit.UIView frontContainerView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIImageView xbackgroundImageView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel xcustomTitle { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			//if (backConstraintY != null) {
			//	backConstraintY.Dispose ();
			//	backConstraintY = null;
			//}

			//if (backContainerView != null) {
			//	backContainerView.Dispose ();
			//	backContainerView = null;
			//}

			//if (frontConstraintY != null) {
			//	frontConstraintY.Dispose ();
			//	frontConstraintY = null;
			//}

			//if (frontContainerView != null) {
			//	frontContainerView.Dispose ();
			//	frontContainerView = null;
			//}

			if (xbackgroundImageView != null) {
				xbackgroundImageView.Dispose ();
				xbackgroundImageView = null;
			}

			if (xcustomTitle != null) {
				xcustomTitle.Dispose ();
				xcustomTitle = null;
			}
		}
	}
}
