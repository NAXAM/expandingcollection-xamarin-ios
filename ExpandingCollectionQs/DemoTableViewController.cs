using System;
using System.Linq;
using Foundation;
using Naxam.ExpandingCollection.iOS;
using UIKit;

namespace ExpandingCollectionQs
{
	public static class Images
	{
		public static UIImage BackgroundImage { get { return FromName("BackgroundImage"); } }
		public static UIImage CloseButton { get { return FromName("CloseButton"); } }
		public static UIImage Dots { get { return FromName("dots"); } }
		public static UIImage Face1 { get { return FromName("face1"); } }
		public static UIImage Face2 { get { return FromName("face2"); } }
		public static UIImage Heand { get { return FromName("heand"); } }
		public static UIImage Icons { get { return FromName("icons"); } }
		public static UIImage Image { get { return FromName("image"); } }
		public static UIImage Item0 { get { return FromName("item0"); } }
		public static UIImage Item1 { get { return FromName("item1"); } }
		public static UIImage Item2 { get { return FromName("item2"); } }
		public static UIImage Item3 { get { return FromName("item3"); } }
		public static UIImage LocationButton { get { return FromName("locationButton"); } }
		public static UIImage Map { get { return FromName("map"); } }
		public static UIImage PinIcon { get { return FromName("pinIcon"); } }
		public static UIImage SearchIcon { get { return FromName("searchIcon"); } }
		public static UIImage Stars { get { return FromName("stars"); } }
		public static UIImage Title { get { return FromName("Title"); } }

		public static UIImage FromName(string name)
		{
			return UIImage.FromBundle(name);
		}
	}

	public partial class DemoTableViewController : ExpandingTableViewController
	{
		protected DemoTableViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic		
		}

		nfloat scrollOffsetY = 0;
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			ConfigureNavBar();

			TableView.BackgroundView = new UIImageView(image: Images.Image);
		}

		void ConfigureNavBar()
		{
			NavigationItem.LeftBarButtonItem.Image = NavigationItem.LeftBarButtonItem.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
			NavigationItem.RightBarButtonItem.Image = NavigationItem.RightBarButtonItem.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
		}

		[Action("backButtonHandler:")]
		void BackButtonHandler(UIButton sender)
		{
			// buttonAnimation
			var viewControllers = NavigationController.ViewControllers.Where(x => x is DemoViewController)
													  .Cast<DemoViewController>().ToArray();

			foreach (var viewController in viewControllers)
			{
				var rightButton = viewController?.NavigationItem.RightBarButtonItem as AnimatingBarButton;
				rightButton?.AnimationSelected(false);
			}

			PopTransitionAnimation();
		}

		[Export("scrollViewDidScroll:")]
		public void ScrollViewDidScroll(UIScrollView scrollView)
		{
			scrollOffsetY = scrollView.ContentOffset.Y;
		}
	}
}

