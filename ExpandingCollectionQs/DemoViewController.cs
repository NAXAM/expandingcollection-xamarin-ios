using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using Naxam.ExpandingCollection.iOS;
using UIKit;

namespace ExpandingCollectionQs
{
	public class ItemInfo
	{
		public ItemInfo(string imageName, string title)
		{
			ImageName = imageName;
			Title = title;
		}

		public string ImageName { get; }

		public string Title { get; }
	}


	public partial class DemoViewController : ExpandingViewController
	{
		protected DemoViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic	
		}

		bool[] cellsIsOpen = new bool[0];

		ItemInfo[] items = new[] {
			new ItemInfo ("item0", "Boston"),
			new ItemInfo ("item1", "New York"),
			new ItemInfo ("item2", "San Francisco"),
			new ItemInfo ("item3", "Washington")
		};

		public UILabel PageLabel
		{
			get { return ctestLabel; }
		}

		public override void ViewDidLoad()
		{
			ItemSize = new CGSize(width: 256, height: 335);

			base.ViewDidLoad();

			RegisterCell();

			FillCellIsOpeenArry();

			AddGestureToView(CollectionView);

			ConfigureNavBar();
		}

		void RegisterCell()
		{
			CollectionView.RegisterNibForCell(DemoCollectionViewCell.Nib, DemoCollectionViewCell.Key);
		}

		void FillCellIsOpeenArry()
		{
			cellsIsOpen = items.Select(x => false).ToArray();
		}

		ExpandingTableViewController GetViewController()
		{
			var storyboard = UIStoryboard.FromName("Main", null);

			var toViewController = (DemoTableViewController)storyboard.InstantiateViewController("DemoTableViewController");

			return toViewController;
		}

		void ConfigureNavBar()
		{
			NavigationItem.LeftBarButtonItem.Image = NavigationItem.LeftBarButtonItem.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
		}

		void AddGestureToView(UIView toView)
		{
			var gesutereUp = new UISwipeGestureRecognizer(SwipeHandler);
			gesutereUp.Direction = UISwipeGestureRecognizerDirection.Up;

			var gesutereDown = new UISwipeGestureRecognizer(SwipeHandler);
			gesutereDown.Direction = UISwipeGestureRecognizerDirection.Down;

			toView.AddGestureRecognizer(gesutereUp);
			toView.AddGestureRecognizer(gesutereDown);
		}

		void SwipeHandler(UISwipeGestureRecognizer sender)
		{
			var indexPath = NSIndexPath.FromRowSection(row: CurrentIndex, section: 0);

			var cell = CollectionView.CellForItem(indexPath) as DemoCollectionViewCell;

			if (cell == null) return;

			// double swipe Up transition
			if (cell.IsOpened == true && sender.Direction == UISwipeGestureRecognizerDirection.Up)
			{
				PushToViewController(GetViewController());

				var rightButton = NavigationItem.RightBarButtonItem as AnimatingBarButton;
				rightButton?.AnimationSelected(true);
			}

			var open = sender.Direction == UISwipeGestureRecognizerDirection.Up ? true : false;

			cell.CellIsOpen(open);

			cellsIsOpen[(indexPath as NSIndexPath).Row] = cell.IsOpened;
		}

		[Export("scrollViewDidScroll:")]
		public void scrollViewDidScroll(UIScrollView scrollView)
		{
			PageLabel.Text = $"{CurrentIndex + 1}/{items.Length}";
		}

		public override void WillDisplayCell(UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath)
		{
			base.WillDisplayCell(collectionView, cell, indexPath);
			var xcell = cell as DemoCollectionViewCell;

			if (xcell == null)
			{
				return;
			}

			var index = (indexPath as NSIndexPath).Row % items.Length;

			var info = items[index];

			xcell.BackgroundImageView.Image = Images.FromName(info.ImageName);

			xcell.CustomTitle.Text = info.Title;

			xcell.CellIsOpen(cellsIsOpen[index], animated: false);
		}

		[Export("collectionView:didSelectItemAtIndexPath:")]
		public void DidSelectItemAtIndexPath(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem(indexPath) as DemoCollectionViewCell;

			if (cell == null || CurrentIndex != indexPath.Row) { return; }

			if (cell.IsOpened == false)
			{
				cell.CellIsOpen(true);
			}
			else
			{
				PushToViewController(GetViewController());

				var rightButton = NavigationItem.RightBarButtonItem as AnimatingBarButton;
				rightButton?.AnimationSelected(true);
			}
		}

		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			return items.Length;
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			return (UICollectionViewCell)collectionView.DequeueReusableCell(DemoCollectionViewCell.Key, indexPath);
		}
	}
}

