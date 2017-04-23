using System;
using System.ComponentModel;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;
using Naxam.ExpandingCollection.iOS;

namespace ExpandingCollectionQs
{
	public interface IRotatable
	{
		void RotateAnimationFrom(UIView fromItem, UIView toItem, nfloat duration);
	}

	public static class RotatableExtensions
	{
		public static void RotateAnimationFrom(this IRotatable rotable, UIView fromItem, UIView toItem, nfloat duration)
		{
			var fromRotate = AnimationFrom(0, to: (nfloat)Math.PI, key: "transform.rotation", duration: duration);
			var fromOpacity = AnimationFrom(1, to: 0, key: "opacity", duration: duration);
			var toRotate = AnimationFrom(-(nfloat)Math.PI, to: 0, key: "transform.rotation", duration: duration);
			var toOpacity = AnimationFrom(0, to: 1, key: "opacity", duration: duration);

			fromItem.Layer.AddAnimation(fromRotate, null);
			fromItem.Layer.AddAnimation(fromOpacity, null);
			toItem.Layer.AddAnimation(toRotate, null);
			toItem.Layer.AddAnimation(toOpacity, null);
		}

		static CABasicAnimation AnimationFrom(nfloat from, nfloat to, string key, nfloat duration)
		{
			var animation = CABasicAnimation.FromKeyPath(key);

			animation.Duration = duration;
			animation.From = NSNumber.FromNFloat(from);
			animation.To = NSNumber.FromNFloat(to);
			animation.FillMode = CAFillMode.Forwards;
			animation.RemovedOnCompletion = false;

			return animation;
		}
	}


	[Register(nameof(AnimatingBarButton)), DesignTimeVisible(true)]
	public class AnimatingBarButton : UIBarButtonItem, IRotatable
	{
		protected AnimatingBarButton(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic		
		}

		[Export("normalImageName"), Browsable(true), DisplayName("Normal Image")]
		public NSString NormalImageName { get; set; } = new NSString(string.Empty);

		[Export("selectedImageName"), Browsable(true), DisplayName("Selected Image")]
		public NSString SelectedImageName { get; set; } = new NSString(string.Empty);

		[Export("duration"), Browsable(true), DisplayName("Duration")]
		public nfloat Duration { get; set; } = 1;

		public UIImageView NormalView { get; private set; } = new UIImageView(CGRect.Empty);
		public UIImageView SelectedView { get; private set; } = new UIImageView(CGRect.Empty);

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			CustomView = new UIView(frame: new CGRect(x: 0, y: 0, width: 44, height: 44));

			ConfigurateImageViews();
		}


		public void AnimationSelected(bool selected)
		{
			if (selected)
			{
				RotateAnimationFrom(NormalView, toItem: SelectedView, duration: Duration);
			}
			else
			{
				RotateAnimationFrom(SelectedView, toItem: NormalView, duration: Duration);
			}
		}

		void ConfigurateImageViews()
		{
			ConfigureImageView(NormalView, imageName: NormalImageName);
			ConfigureImageView(SelectedView, imageName: SelectedImageName);

			SelectedView.Alpha = 0;
		}

		void ConfigureImageView(UIImageView imageView, string imageName)
		{
			if (CustomView == null) { return; }

			imageView.TranslatesAutoresizingMaskIntoConstraints = false;
			imageView.Image = UIImage.FromBundle(imageName);

			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;

			CustomView.AddSubview(imageView);

			foreach (var item in new[] {
				new Tuple<NSLayoutAttribute, nfloat>(NSLayoutAttribute.CenterX, 12),
				new Tuple<NSLayoutAttribute, nfloat>(NSLayoutAttribute.CenterY, -1)
			})
			{
				CustomView.AddConstraint(imageView, x =>
				{
					x.Attribute = item.Item1;
					x.Constant = item.Item2;
				});
			}

			foreach (var item in new[] { NSLayoutAttribute.Height, NSLayoutAttribute.Width })
			{
				imageView.AddConstraint(x =>
				{
					x.Attribute = item;
					x.Constant = 20;
				});
			}
		}

		public void RotateAnimationFrom(UIView fromItem, UIView toItem, nfloat duration)
		{
			RotatableExtensions.RotateAnimationFrom(this, fromItem, toItem, duration);
		}
	}

}
