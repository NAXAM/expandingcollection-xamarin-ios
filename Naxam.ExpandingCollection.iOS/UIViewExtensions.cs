using System;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Naxam.ExpandingCollection.iOS
{
	public class ConstraintInfo
	{
		public NSLayoutAttribute Attribute { get; set; } = NSLayoutAttribute.Left;

		public NSLayoutAttribute SecondAttribute { get; set; } = NSLayoutAttribute.NoAttribute;

		public nfloat Constant { get; set; } = 0;

		public string Identifier { get; set; }

		public NSLayoutRelation Relation { get; set; } = NSLayoutRelation.Equal;
	}

	public static class UIViewExtensions
	{
		static NSLayoutAttribute[] scaleToFillAttributes = {
			NSLayoutAttribute.Left,
			NSLayoutAttribute.Right,
			NSLayoutAttribute.Top,
			NSLayoutAttribute.Bottom
		};

		public static void AddScaleToFillConstraintsOnView(this UIView view, UIView secondView)
		{
			foreach (var attribute in scaleToFillAttributes)
			{
				view.AddConstraint(secondView, info => { info.Attribute = attribute; });
			}
		}

		public static NSLayoutConstraint GetConstraint(this UIView view, NSLayoutAttribute attr)
		{
			return view.Constraints.FirstOrDefault(x => x.FirstAttribute == attr && x.SecondItem == null);
		}

		public static NSLayoutConstraint AddConstraint(this UIView view, Action<ConstraintInfo> action)
		{
			var info = new ConstraintInfo();
			action(info);

			var constraint = NSLayoutConstraint.Create(
													view1: view,
													attribute1: info.Attribute,
													relation: info.Relation,
													view2: null,
													attribute2: info.Attribute,
													multiplier: 1,
													constant: info.Constant);
			constraint.SetIdentifier(info.Identifier);

			view.AddConstraint(constraint);

			return constraint;
		}

		public static NSLayoutConstraint AddConstraint(this UIView view, UIView secondView, Action<ConstraintInfo> action)
		{
			var info = new ConstraintInfo();
			action(info);

			info.SecondAttribute = info.SecondAttribute == NSLayoutAttribute.NoAttribute ? info.Attribute : info.SecondAttribute;

			var constraint = NSLayoutConstraint.Create(
													view1: secondView,
													attribute1: info.Attribute,
													relation: info.Relation,
													view2: view,
													attribute2: info.SecondAttribute,
													multiplier: 1,
													constant: info.Constant);
			constraint.SetIdentifier(info.Identifier);

			view.AddConstraint(constraint);

			return constraint;
		}


		public static NSLayoutConstraint AddConstraint(this UIView view, UIView secondView, UIView thirdView, Action<ConstraintInfo> action)
		{
			var info = new ConstraintInfo();
			action(info);

			info.SecondAttribute = info.SecondAttribute == NSLayoutAttribute.NoAttribute ? info.Attribute : info.SecondAttribute;

			var constraint = NSLayoutConstraint.Create(
													view1: secondView,
													attribute1: info.Attribute,
													relation: info.Relation,
													view2: thirdView,
													attribute2: info.SecondAttribute,
													multiplier: 1,
													constant: info.Constant);
			constraint.SetIdentifier(info.Identifier);

			view.AddConstraint(constraint);

			return constraint;
		}

		public static UIImage TakeSnapshot(this UIView view, CGRect frame)
		{
			UIGraphics.BeginImageContextWithOptions(frame.Size, false, 0.0f);

			var context = UIGraphics.GetCurrentContext();

			context?.TranslateCTM(tx: frame.Location.X * -1, ty: frame.Location.Y * -1);

			context = UIGraphics.GetCurrentContext();
			if (context == null)
			{
				return null;
			}

			view.Layer.RenderInContext(context);

			var image = UIGraphics.GetImageFromCurrentImageContext();

			UIGraphics.EndImageContext();

			return image;
		}

		public static void SubviewsForEach(this UIView view, Action<UIView> action)
		{
			ForEachView(view, action);
		}

		static void ForEachView(UIView view, Action<UIView> action)
		{
			foreach (var item in view.Subviews)
			{
				action(item);

				if (item.Subviews.Length > 0)
				{
					ForEachView(item, action);
				}
			}
		}

		public static void AnimationCornerRadius(this UIView view, nfloat radius, nfloat duration)
		{
			var animation = CABasicAnimation.FromKeyPath("cornerRadius");
			animation.Duration = duration;
			animation.To = NSNumber.FromNFloat(radius);
			animation.FillMode = CAFillMode.Forwards;
			animation.RemovedOnCompletion = false;

			view.Layer.AddAnimation(animation, null);
		}

	}
}
