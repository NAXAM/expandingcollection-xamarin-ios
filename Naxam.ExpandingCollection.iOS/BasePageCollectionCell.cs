using System;
using System.ComponentModel;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Naxam.ExpandingCollection.iOS
{
	[Register(nameof(BasePageCollectionCell)), DesignTimeVisible(true)]
	public class BasePageCollectionCell : UICollectionViewCell
	{
		/// <summary>
		/// Animation oposition offset when cell is open
		/// </summary>
		/// <value>The Y Offset.</value>
		[Export("yOffset"), Browsable(true), DisplayName("Y Offset")]
		public nfloat YOffset { get; set; } = 40;

		const string BACK_CONTAINER_KEY = "backContainerViewKey";

		const string SHADOW_VIEW_KEY = "shadowViewKey";

		const string FRONT_CONTAINER = "frontContainerKey";

		const string BACK_CONTAINER_Y_KEY = "backContainerYKey";

		const string FRONT_CONTAINER_Y_KEY = "frontContainerYKey";

		/// <summary>
		/// The view used as the face of the cell must connectid from xib or storyboard.
		/// </summary>
		/// <value>The front container view.</value>
		[Outlet("frontContainerView")]
		public virtual UIView FrontContainerView { get; set; }

		/// <summary>
		/// The view used as the back of the cell must connectid from xib or storyboard.
		/// </summary>
		/// <value>The back container view.</value>
		[Outlet("backContainerView")]
		public virtual UIView BackContainerView { get; set; }

		/// <summary>
		/// constraints for backContainerView must connectid from xib or storyboard
		/// </summary>
		/// <value>The back constraint y.</value>
		[Outlet("backConstraintY")]
		public virtual NSLayoutConstraint BackConstraintY { get; set; }

		/// <summary>
		/// constraints for frontContainerView must connectid from xib or storyboard
		/// </summary>
		/// <value>The font constraint y.</value>
		[Outlet("frontConstraintY")]
		public virtual NSLayoutConstraint FrontConstraintY { get; set; }

		public UIView ShadowView { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Naxam.ExpandingCollection.IOS.BasePageCollectionCell"/>
		/// is opened.
		/// </summary>
		/// <value><c>true</c> if is opened; otherwise, <c>false</c>.</value>
		public bool IsOpened { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Naxam.ExpandingCollection.IOS.BasePageCollectionCell"/> class.
		/// </summary>
		/// <param name="coder">Coder.</param>
		public BasePageCollectionCell(NSCoder coder)
					: base(coder)
		{
			ConfigureOutletFromDecoder(coder);
		}
		public BasePageCollectionCell(CGRect frame) : base(frame)
		{
			CommonInit();
		}

		protected internal BasePageCollectionCell(IntPtr ptr) : base(ptr)
		{
		}

		protected BasePageCollectionCell(NSObjectFlag flag) : base(flag)
		{

		}
		public BasePageCollectionCell() : base()
		{

		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			CommonInit();
		}

		void CommonInit()
		{
			ConfigurationViews();

			ShadowView = CreateShadowViewOnView(FrontContainerView);
		}

		/// <summary>
		/// Open or close cell.
		/// </summary>
		/// <param name="isOpen">Contains the value true if the cell should display open state, if false should display close state.</param>
		/// <param name="animated">Set to true if the change in selection state is animated.</param>
		public void CellIsOpen(bool isOpen, bool animated = true)
		{
			if (isOpen == IsOpened) { return; }

			FrontConstraintY.Constant = isOpen == true ? -FrontContainerView.Bounds.Size.Height / 5 : 0;


			BackConstraintY.Constant = isOpen == true ? FrontContainerView.Bounds.Size.Height / 5 - YOffset / 2 : 0;

			//UIViewExtensions.GetConstraint(BackContainerView, NSLayoutAttribute.Right);
			var widthConstant = BackContainerView.GetConstraint(NSLayoutAttribute.Width);
			if (widthConstant != null)
			{
				widthConstant.Constant = isOpen == true ? FrontContainerView.Bounds.Size.Width + YOffset : FrontContainerView.Bounds.Size.Width;
			}

			var heightConstant = BackContainerView.GetConstraint(NSLayoutAttribute.Height);
			if (heightConstant != null)
			{
				heightConstant.Constant = isOpen == true ? FrontContainerView.Bounds.Size.Height + YOffset : FrontContainerView.Bounds.Size.Height;
			}

			IsOpened = isOpen;

			ConfigurationCell();

			if (animated == true)
			{
				Animate(
					duration: 0.3,
					delay: 0,
					options: UIViewAnimationOptions.TransitionNone,
					animation: ContentView.LayoutIfNeeded,
					completion: null);
			}
			else
			{
				ContentView.LayoutIfNeeded();
			}
		}

		void ConfigurationViews()
		{
			BackContainerView.Layer.MasksToBounds = true;

			BackContainerView.Layer.CornerRadius = 5;
			FrontContainerView.Layer.MasksToBounds = true;

			FrontContainerView.Layer.CornerRadius = 5;

			ContentView.Layer.MasksToBounds = false;

			Layer.MasksToBounds = false;
		}

		UIView CreateShadowViewOnView(UIView view)
		{
			if (view == null) return view;

			var shadow = new UIView(CGRect.Empty)
			{
				BackgroundColor = UIColor.FromWhiteAlpha(0, 0),
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			shadow.Layer.MasksToBounds = false;
			shadow.Layer.ShadowColor = UIColor.Black.CGColor;
			shadow.Layer.ShadowRadius = 10;
			shadow.Layer.ShadowOpacity = 0.3f;
			shadow.Layer.ShadowOffset = new CGSize(width: 0, height: 0);
			ContentView.InsertSubviewBelow(shadow, view);

			var deafaultSizeInfos = new[] {
				new ConstraintInfo {
					Attribute = NSLayoutAttribute.Width,
					Constant = 0.8f
				},
				new ConstraintInfo {
					Attribute = NSLayoutAttribute.Height,
					Constant = 0.9f
				}
			};
			foreach (var info in deafaultSizeInfos)
			{
				var frontViewConstraint = view.GetConstraint(info.Attribute);
				shadow.AddConstraint(x =>
				{
					x.Attribute = info.Attribute;
					x.Constant = frontViewConstraint.Constant * info.Constant;
				});
			}

			var deafaultCenterInfos = new[] {
				new ConstraintInfo {
					Attribute = NSLayoutAttribute.CenterX,
					Constant = 0
				},
				new ConstraintInfo {
					Attribute = NSLayoutAttribute.CenterY,
					Constant = 30
				}
			};

			foreach (var info in deafaultCenterInfos)
			{
				ContentView.AddConstraint(shadow, view, x =>
				{
					x.Attribute = info.Attribute;
					x.Constant = info.Constant;
				});
			}

			// size shadow
			var width = shadow.GetConstraint(NSLayoutAttribute.Width).Constant;
			var height = shadow.GetConstraint(NSLayoutAttribute.Height).Constant;

			shadow.Layer.ShadowPath = UIBezierPath.FromRoundedRect(new CGRect(x: 0, y: 0, width: width, height: height), 0).CGPath;

			return shadow;
		}

		internal void ConfigurationCell()
		{
			// Prevents indefinite growing of the cell issue
			nfloat i = IsOpened ? 1 : -1;
			var superHeight = Superview?.Frame.Size.Height ?? 0;

			Frame = new CGRect(
				Frame.X - i * YOffset / 2,
				Frame.Y - i * superHeight / 2,
				Frame.Size.Width + i * YOffset,
				Frame.Size.Height + i * superHeight
			);
		}

		void ConfigureCellViewConstraintsWithSize(CGSize size)
		{
			var frontConstraintConstant = FrontContainerView.GetConstraint(NSLayoutAttribute.Width)?.Constant;

			if (IsOpened || frontConstraintConstant == size.Width)
			{
				return;
			}

			foreach (var item in new[] { FrontContainerView, BackContainerView })
			{
				var constraintWidth = item.GetConstraint(NSLayoutAttribute.Width);
				if (constraintWidth != null)
				{
					constraintWidth.Constant = size.Width;
				}

				var constraintHeight = item.GetConstraint(NSLayoutAttribute.Height);
				if (constraintHeight != null)
				{
					constraintHeight.Constant = size.Height;
				}
			}
		}

		void HighlightedImageFalseOnView(UIView view)
		{
			foreach (var item in view.Subviews)
			{
				if (item is UIImageView)
				{
					((UIImageView)item).Highlighted = false;
				}

				if (item.Subviews.Length > 0)
				{
					HighlightedImageFalseOnView(item);
				}
			}
		}

		void CopyShadowFromView(UIView fromView, UIView toView)
		{
			fromView.Layer.ShadowPath = toView.Layer.ShadowPath;
			fromView.Layer.MasksToBounds = toView.Layer.MasksToBounds;
			fromView.Layer.ShadowColor = toView.Layer.ShadowColor;
			fromView.Layer.ShadowRadius = toView.Layer.ShadowRadius;
			fromView.Layer.ShadowOpacity = toView.Layer.ShadowOpacity;
			fromView.Layer.ShadowOffset = toView.Layer.ShadowOffset;
		}

		public BasePageCollectionCell CopyCell()
		{
			HighlightedImageFalseOnView(ContentView);

			var data = NSKeyedArchiver.ArchivedDataWithRootObject(this);

			var copyView = (BasePageCollectionCell)NSKeyedUnarchiver.UnarchiveObject(data);

			if (copyView == null || ShadowView == null)
			{
				return null;
			}

			// configure
			copyView.BackContainerView.Layer.MasksToBounds = BackContainerView.Layer.MasksToBounds;
			copyView.BackContainerView.Layer.CornerRadius = BackContainerView.Layer.CornerRadius;
			copyView.FrontContainerView.Layer.MasksToBounds = FrontContainerView.Layer.MasksToBounds;
			copyView.FrontContainerView.Layer.CornerRadius = FrontContainerView.Layer.CornerRadius;

			// copy shadow layer
			CopyShadowFromView(copyView.ShadowView, ShadowView);

			for (var index = 0; index < copyView.FrontContainerView.Subviews.Length; index++)
			{
				CopyShadowFromView(copyView.FrontContainerView.Subviews[index], FrontContainerView.Subviews[index]);
			}

			return copyView;
		}

		public override void EncodeTo(NSCoder encoder)
		{
			base.EncodeTo(encoder);

			encoder.Encode(BackContainerView, BACK_CONTAINER_KEY);
			encoder.Encode(FrontContainerView, FRONT_CONTAINER);
			encoder.Encode(FrontConstraintY, FRONT_CONTAINER_Y_KEY);
			encoder.Encode(BackConstraintY, BACK_CONTAINER_Y_KEY);
			encoder.Encode(ShadowView, SHADOW_VIEW_KEY);
		}

		void ConfigureOutletFromDecoder(NSCoder coder)
		{
			var shadowView = coder.DecodeObject(SHADOW_VIEW_KEY) as UIView;
			if (shadowView != null)
			{
				ShadowView = shadowView;
			}

			var backView = coder.DecodeObject(BACK_CONTAINER_KEY) as UIView;
			if (backView != null)
			{
				BackContainerView = backView;
			}

			var frontView = coder.DecodeObject(FRONT_CONTAINER) as UIView;
			if (frontView != null)
			{
				FrontContainerView = frontView;
			}

			var frontConstraintY = coder.DecodeObject(FRONT_CONTAINER_Y_KEY) as NSLayoutConstraint;
			if (frontConstraintY != null)
			{
				FrontConstraintY = frontConstraintY;
			}

			var backConstraintY = coder.DecodeObject(BACK_CONTAINER_Y_KEY) as NSLayoutConstraint;
			if (backConstraintY != null)
			{
				BackConstraintY = backConstraintY;
			}
		}
	}
}
