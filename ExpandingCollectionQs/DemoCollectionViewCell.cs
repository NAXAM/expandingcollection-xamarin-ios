using System;
using CoreGraphics;
using Foundation;
using Naxam.ExpandingCollection.iOS;
using UIKit;

namespace ExpandingCollectionQs
{
	public partial class DemoCollectionViewCell : BasePageCollectionCell
	{
		public static readonly NSString Key = new NSString("DemoCollectionViewCell");
		public static readonly UINib Nib;

		static DemoCollectionViewCell()
		{
			Nib = UINib.FromName("DemoCollectionViewCell", NSBundle.MainBundle);
		}

		[Export("initWithCoder:")]
		public DemoCollectionViewCell(NSCoder coder) : base(coder)
		{

		}

		protected DemoCollectionViewCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			//CustomTitle.Layer.ShadowRadius = 2;
			//CustomTitle.Layer.ShadowOffset = new CGSize(width: 0, height: 3);
			//CustomTitle.Layer.ShadowOpacity = 0.2f;
		}

		public UILabel CustomTitle { get { return xcustomTitle; } }

		public UIImageView BackgroundImageView
		{
			get { return xbackgroundImageView; }
		}
	}
}
