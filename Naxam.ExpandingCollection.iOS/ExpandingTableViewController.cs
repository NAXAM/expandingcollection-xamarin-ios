using UIKit;
using Foundation;
using System;
using System.ComponentModel;
using CoreGraphics;

namespace Naxam.ExpandingCollection.iOS
{
	[Register(nameof(ExpandingTableViewController)), DesignTimeVisible(true)]
	public class ExpandingTableViewController : UITableViewController
	{
		public ExpandingTableViewController(UITableViewStyle withStyle) : base(withStyle) { }

		public ExpandingTableViewController(string nibName, NSBundle bundle) : base(nibName, bundle) { }

		protected internal ExpandingTableViewController(IntPtr handle) : base(handle) { }

		public ExpandingTableViewController() : base() { }

		public ExpandingTableViewController(NSCoder coder) : base(coder) { }

		protected ExpandingTableViewController(NSObjectFlag t) : base(t) { }

		public nfloat HeaderHeight { get; set; } = 236;

		public TransitionDriver TransitionDriver { get; set; }

		UIImage GetScreen()
		{
			var height = (HeaderHeight - TableView.ContentOffset.Y) < 0 ? 0 : (HeaderHeight - TableView.ContentOffset.Y);

			var backImageSize = new CGSize(width: View.Bounds.Width, height: View.Bounds.Height - height + GetTabBarHeight());

			var backImageOrigin = new CGPoint(x: 0, y: height + TableView.ContentOffset.Y);
			return View.TakeSnapshot(new CGRect(location: backImageOrigin, size: backImageSize));
		}

		nfloat GetTabBarHeight()
		{
			if (NavigationController == null)
			{
				return 0;
			}

			var insets = AutomaticallyAdjustsScrollViewInsets;

			var tabBarHeight = insets == true ? NavigationController.NavigationBar.Frame.Size.Height : 0;
			var stausBarHeight = insets == true ? UIApplication.SharedApplication.StatusBarFrame.Size.Height : 0;

			return tabBarHeight + stausBarHeight;
		}
		/// <summary>
		///  Pops the top View controller from the navigation stack and updates the display with custom animation.
		/// </summary>
		public void PopTransitionAnimation()
		{
			if (TransitionDriver == null)
			{
				return;
			}
			var backImage = GetScreen();

			var offset = TableView.ContentOffset.Y > HeaderHeight ? HeaderHeight : TableView.ContentOffset.Y;


			offset += GetTabBarHeight();

			TransitionDriver.PopTransitionAnimationContantOffset(offset, backImage: backImage);

			NavigationController?.PopViewController(animated: false);
		}
	}
}