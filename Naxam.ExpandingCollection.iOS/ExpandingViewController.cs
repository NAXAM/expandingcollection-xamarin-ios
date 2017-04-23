using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Naxam.ExpandingCollection.iOS
{
	[Register(nameof(ExpandingViewController)), DesignTimeVisible(true)]
	public abstract class ExpandingViewController : UIViewController, IUICollectionViewDataSource, IUICollectionViewDelegate
	{
		public ExpandingViewController(string nibName, NSBundle bundle) : base(nibName, bundle) { }

		public ExpandingViewController() : base() { }

		public ExpandingViewController(NSCoder coder) : base(coder) { }

		protected ExpandingViewController(NSObjectFlag t) : base(t) { }

		protected internal ExpandingViewController(IntPtr handle) : base(handle) { }

		/// <summary>
		/// The default size to use for cells.
		/// </summary>
		/// <value>The size of the item.</value>
		public CGSize ItemSize { get; set; } = new CGSize(width: 256, height: 335);

		/// <summary>
		/// The collection view object managed by this view controller.
		/// </summary>
		/// <value>The collection view.</value>
		public UICollectionView CollectionView { get; set; }

		private TransitionDriver transitionDriver = null;

		/// <summary>
		/// Index of current cell
		/// </summary>
		/// <value>The index of the current.</value>
		public int CurrentIndex
		{
			get
			{

				if (CollectionView == null)
				{
					return 0;
				}

				var startOffset = (CollectionView.Bounds.Size.Width - ItemSize.Width) / 2;

				var collectionLayout = CollectionView.CollectionViewLayout as UICollectionViewFlowLayout;

				if (collectionLayout == null)
				{
					return 0;
				}

				var minimumLineSpacing = collectionLayout.MinimumLineSpacing;

				var a = CollectionView.ContentOffset.X + startOffset + ItemSize.Width / 2;
				var b = ItemSize.Width + minimumLineSpacing;

				return (int)(a / b);
			}
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			CommonInit();
		}

		/**
		 Pushes a view controller onto the receiver’s stack and updates the display with custom animation.
		 
		 - parameter viewController: The table view controller to push onto the stack. 
		 */
		public void PushToViewController(ExpandingTableViewController viewController)
		{
			if (CollectionView == null || NavigationController == null)
			{
				return;
			}

			viewController.TransitionDriver = transitionDriver;

			var insets = viewController.AutomaticallyAdjustsScrollViewInsets;
			var tabBarHeight = insets == true ? NavigationController.NavigationBar.Frame.Size.Height : 0;
			var stausBarHeight = insets == true ? UIApplication.SharedApplication.StatusBarFrame.Size.Height : 0;
			var backImage = GetBackImage(viewController, headerHeight: viewController.HeaderHeight);

			transitionDriver?.PushTransitionAnimationIndex(CurrentIndex,
												   collecitionView: CollectionView,
												   backImage: backImage,
												   headerHeight: viewController.HeaderHeight,
												   insets: tabBarHeight + stausBarHeight,
												   completion: headerView =>
												   {
													   viewController.TableView.TableHeaderView = headerView;

													   NavigationController.PushViewController(viewController, animated: false);
												   });
		}

		void CommonInit()
		{
			var layout = new PageCollectionLayout(itemSize: ItemSize);

			CollectionView = PageCollectionView.CreateOnView(View,
													 layout: layout,
													 height: ItemSize.Height + ItemSize.Height / 5 * 2,
													 dataSource: this,
													 @delegate: this);


			if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
			{
				CollectionView.PrefetchingEnabled = false;
			}

			transitionDriver = new TransitionDriver(view: View);
		}

		UIImage GetBackImage(UIViewController viewController, nfloat headerHeight)
		{
			var imageSize = new CGSize(width: viewController.View.Bounds.Width, height: viewController.View.Bounds.Height - headerHeight);

			var imageFrame = new CGRect(location: new CGPoint(x: 0, y: 0), size: imageSize);

			return viewController.View.TakeSnapshot(imageFrame);
		}

		public abstract nint GetItemsCount(UICollectionView collectionView, nint section);

		public abstract UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath);

		[Export("collectionView:willDisplayCell:forItemAtIndexPath:")]
		public virtual void WillDisplayCell(UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath)
		{

		}

		public void scrollViewDidEndDecelerating(UIScrollView scrollView)
		{
			var indexPath = NSIndexPath.FromRowSection(row: CurrentIndex, section: 0);

			var currentCell = CollectionView.CellForItem(indexPath) as BasePageCollectionCell;
			currentCell?.ConfigurationCell();
		}
	}
}