using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Naxam.ExpandingCollection.iOS
{
	public class PageCollectionLayout : UICollectionViewFlowLayout
	{
		private CGSize lastCollectionViewSize = CGSize.Empty;

		public nfloat ScalingOffset { get; set; } = 200f;
		public nfloat MinimumScaleFactor { get; set; } = 0.9f;
		public nfloat MinimumAlphaFactor { get; set; } = 0.3f;
		public bool ScaleItems { get; set; } = false;

		public PageCollectionLayout(NSCoder coder) : base(coder)
		{
			throw new InvalidOperationException("NOT_ALLOWED");
		}

		public PageCollectionLayout(CGSize itemSize) : base()
		{
			CommonInit(itemSize);
		}

		private void CommonInit(CGSize itemSize)
		{
			ScrollDirection = UICollectionViewScrollDirection.Horizontal;
			MinimumLineSpacing = 25;
			ItemSize = itemSize;
		}

		public override void InvalidateLayout(UICollectionViewLayoutInvalidationContext context)
		{
			base.InvalidateLayout(context);

			if (CollectionView == null) { return; }

			if (CollectionView.Bounds.Size != lastCollectionViewSize)
			{
				ConfigureInset();

				lastCollectionViewSize = CollectionView.Bounds.Size;
			}
		}

		public override CGPoint TargetContentOffset(CGPoint proposedContentOffset, CGPoint velocity)
		{
			if (CollectionView == null) { return proposedContentOffset; }

			var proposedRect = new CGRect(x: proposedContentOffset.X, y: 0, width: CollectionView.Bounds.Width, height: CollectionView.Bounds.Height);

			var layoutAttributes = LayoutAttributesForElementsInRect(proposedRect);

			if (layoutAttributes == null) { return proposedContentOffset; }

			UICollectionViewLayoutAttributes candidateAttributes = null;
			var proposedContentOffsetCenterX = proposedContentOffset.X + CollectionView.Bounds.Width / 2;

			foreach (var attributes in layoutAttributes)
			{
				if (attributes.RepresentedElementCategory != UICollectionElementCategory.Cell)
				{
					continue;
				}

				if (candidateAttributes == null)
				{
					candidateAttributes = attributes;
					continue;
				}

				if (Math.Abs(attributes.Center.X - proposedContentOffsetCenterX) < Math.Abs(candidateAttributes.Center.X - proposedContentOffsetCenterX))
				{
					candidateAttributes = attributes;
				}
			}

			if (candidateAttributes == null) { return proposedContentOffset; }


			var newOffsetX = candidateAttributes.Center.X - CollectionView.Bounds.Size.Width / 2;
			var offset = newOffsetX - CollectionView.ContentOffset.X;

			if ((velocity.X < 0 && offset > 0) || (velocity.X > 0 && offset < 0))
			{
				var pageWidth = this.ItemSize.Width + this.MinimumLineSpacing;
				newOffsetX += velocity.X > 0 ? pageWidth : -pageWidth;
			}

			return new CGPoint(x: newOffsetX, y: proposedContentOffset.Y);
		}

		public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
		{
			return true;
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
		{
			var superAttributes = base.LayoutAttributesForElementsInRect(rect);

			if (CollectionView == null || superAttributes == null || ScaleItems == false)
			{
				return superAttributes;
			}

			var contentOffset = CollectionView.ContentOffset;

			var size = CollectionView.Bounds.Size;

			var visibleRect = new CGRect(x: contentOffset.X, y: contentOffset.Y, width: size.Width, height: size.Height);

			var visibleCenterX = visibleRect.GetMidX();

			// TODO guard case let newAttributesArray as [UICollectionViewLayoutAttributes] = NSArray(array: superAttributes, copyItems: true) else {
			var newAttributesArray = superAttributes.Select(x => (UICollectionViewLayoutAttributes)x.Copy(new NSZone(x.Handle)))
			                                        .ToArray();

			foreach (var item in newAttributesArray)
			{
				var distanceFromCenter = visibleCenterX - item.Center.X;
				var absDistanceFromCenter = Math.Min(Math.Abs(distanceFromCenter), this.ScalingOffset);

				var scale = (nfloat)(absDistanceFromCenter * (this.MinimumScaleFactor - 1) / this.ScalingOffset + 1);
				item.Transform3D = CoreAnimation.CATransform3D.MakeScale(scale, scale, 1);

				var alpha = absDistanceFromCenter * (this.MinimumAlphaFactor - 1) / this.ScalingOffset + 1;
				item.Alpha = (nfloat)alpha;
			}

			return newAttributesArray;
		}

		void ConfigureInset()
		{
			if (CollectionView == null)
			{
				return;
			}

			var inset = CollectionView.Bounds.Size.Width / 2 - ItemSize.Width / 2;

			CollectionView.ContentInset = new UIEdgeInsets(0, inset, 0, inset);
			CollectionView.ContentOffset = new CGPoint(x: -inset, y: 0);
		}
	}
}
