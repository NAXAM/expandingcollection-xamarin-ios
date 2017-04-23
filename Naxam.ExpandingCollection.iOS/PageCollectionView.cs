using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Naxam.ExpandingCollection.iOS
{
	[Register(nameof(PageCollectionView)), DesignTimeVisible(true)]
	public class PageCollectionView : UICollectionView
	{
		protected PageCollectionView(NSObjectFlag t) : base(t) { }

		public PageCollectionView(NSCoder coder) : base(coder) { }

		protected internal PageCollectionView(IntPtr handle) : base(handle) { }

		public PageCollectionView(CGRect frame, UICollectionViewLayout layout) : base(frame, layout) { }

		public static PageCollectionView CreateOnView(UIView view, UICollectionViewLayout layout, nfloat height, IUICollectionViewDataSource dataSource, IUICollectionViewDelegate @delegate)
		{
			var collectionView = new PageCollectionView(CGRect.Empty, layout);

			collectionView.TranslatesAutoresizingMaskIntoConstraints = false;
			collectionView.DecelerationRate = DecelerationRateFast;
			collectionView.ShowsHorizontalScrollIndicator = false;
			collectionView.DataSource = dataSource;
			collectionView.Delegate = @delegate;
			collectionView.BackgroundColor = UIColor.FromWhiteAlpha(0, 0);

			view.AddSubview(collectionView);

			collectionView.AddConstraint(x =>
			{
				x.Attribute = NSLayoutAttribute.Height;
				x.Constant = height;
			});

			foreach (var item in new[] { NSLayoutAttribute.Left, NSLayoutAttribute.Right, NSLayoutAttribute.CenterY })
			{
				view.AddConstraint(collectionView, x =>
				{
					x.Attribute = item;
				});
			}

			return collectionView;
		}
	}
}
