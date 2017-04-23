using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Naxam.ExpandingCollection.iOS
{
	public class TransitionDriver
	{
		const int HIDE_KEY = 101;

		private UIView view;
		private nfloat duration = 0.4f;

		// for push animation
		private BasePageCollectionCell copyCell;
		private BasePageCollectionCell currentCell;
		private UIImageView backImageView;

		private UICollectionViewCell leftCell;
		private UICollectionViewCell rightCell;
		private nfloat step = 0;

		private CGRect frontViewFrame = CGRect.Empty;
		private CGRect backViewFrame = CGRect.Empty;

		public TransitionDriver(UIView view)
		{
			this.view = view;
		}

		public void PushTransitionAnimationIndex(int currentIndex,
									UICollectionView collecitionView,
									UIImage backImage,
									nfloat headerHeight,
									nfloat insets,
									Action<UIView> completion)
		{
			var cell = collecitionView.CellForItem(NSIndexPath.FromItemSection(currentIndex, 0)) as BasePageCollectionCell;
			var copyView = cell.CopyCell();

			if (cell == null || copyView == null)
			{
				return;
			}

			copyCell = copyView;

			// move cells
			MoveCellsCurrentIndex(currentIndex, collectionView: collecitionView);

			currentCell = cell;
			cell.Hidden = true;

			ConfigurateCell(copyView, backImage: backImage);

			backImageView = AddImageToView(copyView.BackContainerView, image: backImage);

			OpenBackViewConfigureConstraints(copyView, height: headerHeight, insets: insets);

			OpenFrontViewConfigureConstraints(copyView, height: headerHeight, insets: insets);

			// corner animation 
			copyView.BackContainerView.AnimationCornerRadius(0, duration: duration);

			copyView.FrontContainerView.AnimationCornerRadius(0, duration: duration);

			copyView.Center = view.Center;

			// constraints animation
			UIView.Animate(
				duration: duration,
				delay: 0,
				options: default(UIViewAnimationOptions),
				animation: () =>
				{
					view.LayoutIfNeeded();
					backImageView.Alpha = 1;
					copyCell.ShadowView.Alpha = 0;

					copyView.FrontContainerView.SubviewsForEach(v =>
					{
						if (v.Tag == HIDE_KEY)
						{
							v.Alpha = 0;
						}
					});
				}, completion: () =>
				{
					var data = NSKeyedArchiver.ArchivedDataWithRootObject(copyView.FrontContainerView);
					var headerView = NSKeyedUnarchiver.UnarchiveObject(data) as UIView;
					if (headerView == null)
					{
						throw new InvalidOperationException("must copy");
					}

					completion(headerView);
				});
		}

		public void PopTransitionAnimationContantOffset(nfloat offset, UIImage backImage)
		{
			if (copyCell == null)
			{
				return;
			}

			backImageView.Image = backImage;
			// configuration start position
			ConfigureCellBeforeClose(copyCell, offset: offset);

			CloseBackViewConfigurationConstraints(copyCell);

			CloseFrontViewConfigurationConstraints(copyCell);

			// corner animation
			copyCell.BackContainerView.AnimationCornerRadius(copyCell.BackContainerView.Layer.CornerRadius, duration: duration);
			copyCell.FrontContainerView.AnimationCornerRadius(copyCell.FrontContainerView.Layer.CornerRadius, duration: duration);

			UIView.Animate(duration: duration, delay: 0, options: default(UIViewAnimationOptions), animation: () =>
			{
				if (rightCell != null)
				{
					rightCell.Center = new CGPoint(rightCell.Center.X - step, rightCell.Center.Y);
				}

				if (leftCell != null)
				{
					leftCell.Center = new CGPoint(leftCell.Center.X + step, leftCell.Center.Y);
				}

				view.LayoutIfNeeded();

				backImageView.Alpha = 0;

				copyCell.ShadowView.Alpha = 1;

				copyCell.FrontContainerView.SubviewsForEach(v =>
				{
					if (v.Tag == HIDE_KEY)
					{
						v.Alpha = 1;
					}
				});
			}, completion: () =>
			{
				if (currentCell == null) {
					return;
				}

				currentCell.Hidden = false;

				RemoveCurrentCell();
			});
		}

		private void RemoveCurrentCell()
		{
			copyCell?.RemoveFromSuperview();
		}

		private void ConfigurateCell(BasePageCollectionCell cell, UIImage backImage)
		{
			cell.TranslatesAutoresizingMaskIntoConstraints = false;

			view.AddSubview(cell);

			foreach (var item in new [] {
				new Tuple<NSLayoutAttribute, nfloat> (NSLayoutAttribute.Width, cell.Bounds.Size.Width),
				new Tuple<NSLayoutAttribute, nfloat> (NSLayoutAttribute.Height, cell.Bounds.Size.Height)
			})
			{
				cell.AddConstraint(x =>
				{
					x.Attribute = item.Item1;
					x.Constant = item.Item2;
				});
			}

			foreach (var item in new[] { NSLayoutAttribute.CenterX, NSLayoutAttribute.CenterY })
			{
				view.AddConstraint(cell, x =>
				{
					x.Attribute = item;
				});
			}
			cell.LayoutIfNeeded();
		}

		private static UIImageView AddImageToView(UIView view, UIImage image)
		{
			if (image == null) { return null; }

			var imageView = new UIImageView(image);

			imageView.TranslatesAutoresizingMaskIntoConstraints = false;
			imageView.Alpha = 0;
			view.AddSubview(imageView);

			foreach (var item in new[] { NSLayoutAttribute.Left, NSLayoutAttribute.Right, NSLayoutAttribute.Top, NSLayoutAttribute.Bottom })
			{
				view.AddConstraint(imageView, x =>
				{
					x.Attribute = item;
				});
			}
			imageView.LayoutIfNeeded();

			return imageView;
		}

		private void MoveCellsCurrentIndex(int currentIndex, UICollectionView collectionView)
		{
			leftCell = null;
			rightCell = null;

			var xleftCell = collectionView.CellForItem(NSIndexPath.FromRowSection(currentIndex - 1, 0));
			if (xleftCell != null)
			{
				var xstep = xleftCell.Frame.Size.Width + (xleftCell.Frame.Location.X - collectionView.ContentOffset.X);

				UIView.Animate(duration: 0.2, animation: () =>
				{
					xleftCell.Center = new CGPoint(xleftCell.Center.X - xstep, xleftCell.Center.Y);
				});
				leftCell = xleftCell;
				step = xstep;
			}

			var xrightCell = collectionView.CellForItem(NSIndexPath.FromRowSection(currentIndex + 1, 0));
			if (xrightCell != null)
			{
				var xstep = collectionView.Frame.Size.Width - (xrightCell.Frame.Location.X - collectionView.ContentOffset.X);

				UIView.Animate(duration: 0.2, animation: () =>
				{
					xrightCell.Center = new CGPoint(xrightCell.Center.X + xstep, xrightCell.Center.Y);
				});

				rightCell = xrightCell;
				step = xstep;
			}
		}

		private void OpenFrontViewConfigureConstraints(BasePageCollectionCell cell, nfloat height, nfloat insets)
		{
			var xheight = frontViewFrame.Height;
			var width = frontViewFrame.Width;

			var heightConstraint = cell.FrontContainerView.GetConstraint(NSLayoutAttribute.Height);
			if (heightConstraint != null)
			{
				xheight = heightConstraint.Constant;
				heightConstraint.Constant = height;
			}
			var widthConstraint = cell.FrontContainerView.GetConstraint(NSLayoutAttribute.Width);
			if (widthConstraint != null)
			{
				width = frontViewFrame.Size.Width;

				widthConstraint.Constant = view.Bounds.Size.Width;
			}

			frontViewFrame = new CGRect(frontViewFrame.X, cell.FrontConstraintY.Constant, width, xheight);

			cell.FrontConstraintY.Constant = -view.Bounds.Size.Height / 2 + height / 2 + insets;

		}

		private void OpenBackViewConfigureConstraints(BasePageCollectionCell cell, nfloat height, nfloat insets)
		{
			var xheight = backViewFrame.Height;
			var width = backViewFrame.Width;

			var heightConstraint = cell.BackContainerView.GetConstraint(NSLayoutAttribute.Height);
			if (heightConstraint != null)
			{
				xheight = heightConstraint.Constant;
				heightConstraint.Constant = view.Bounds.Size.Height - height;
			}
			var widthConstraint = cell.BackContainerView.GetConstraint(NSLayoutAttribute.Width);
			if (widthConstraint != null)
			{
				width = backViewFrame.Size.Width;

				widthConstraint.Constant = view.Bounds.Size.Width;
			}

			backViewFrame = new CGRect(backViewFrame.X, cell.BackConstraintY.Constant, width, xheight);

			cell.BackConstraintY.Constant = view.Bounds.Size.Height / 2 - (view.Bounds.Size.Height - 236) / 2 + insets;
		}

		private void CloseBackViewConfigurationConstraints(BasePageCollectionCell cell)
		{
			if (cell == null) { return; }

			var heightConstraint = cell.BackContainerView.GetConstraint(NSLayoutAttribute.Height);
			heightConstraint.Constant = backViewFrame.Size.Height;

			var widthConstraint = cell.BackContainerView.GetConstraint(NSLayoutAttribute.Width);
			widthConstraint.Constant = backViewFrame.Size.Width;

			cell.BackConstraintY.Constant = backViewFrame.Location.Y;
		}

		private void CloseFrontViewConfigurationConstraints(BasePageCollectionCell cell)
		{
			if (cell == null) { return; }

			var heightConstraint = cell.FrontContainerView.GetConstraint(NSLayoutAttribute.Height);
			heightConstraint.Constant = frontViewFrame.Size.Height;

			var widthConstraint = cell.FrontContainerView.GetConstraint(NSLayoutAttribute.Width);
			widthConstraint.Constant = frontViewFrame.Size.Width;

			cell.FrontConstraintY.Constant = frontViewFrame.Location.Y;
		}

		private void ConfigureCellBeforeClose(BasePageCollectionCell cell, nfloat offset)
		{
			cell.FrontConstraintY.Constant -= offset;
			cell.BackConstraintY.Constant -= offset / 2.0f;

			var heightConstraint = cell.BackContainerView.GetConstraint(NSLayoutAttribute.Height);
			if (heightConstraint != null)
			{
				heightConstraint.Constant += offset;

			}
			cell.ContentView.LayoutIfNeeded();
		}
	}
}