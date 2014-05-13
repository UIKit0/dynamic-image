﻿using System.Collections.Generic;
using System.Windows;
using SoundInTheory.DynamicImage.Caching;
using SoundInTheory.DynamicImage.Filters;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage
{
	public abstract class Layer : DirtyTrackingObject
	{
		#region Properties

		public bool Visible
		{
			get { return (bool)(this["Visible"] ?? true); }
			set { this["Visible"] = value; }
		}

		public AnchorStyles Anchor
		{
			get { return (AnchorStyles)(this["Anchor"] ?? AnchorStyles.None); }
			set { this["Anchor"] = value; }
		}

		public int X
		{
			get { return (int)(this["X"] ?? 0); }
			set { this["X"] = value; }
		}

		public int Y
		{
			get { return (int)(this["Y"] ?? 0); }
			set { this["Y"] = value; }
		}

		public int AnchorPadding
		{
			get 
			{
				if (this["AnchorPadding"] == null)
				{
					this["AnchorPadding"] = Unit.Pixel(0);
				}
				return (int)((Unit)this["AnchorPadding"]).Value;
			}
			set 
			{
				this["AnchorPadding"] = (this["AnchorPadding"] == null || ((Unit)this["AnchorPadding"]).Type == UnitType.Pixel) ?
					Unit.Pixel(value) :
					Unit.Percentage(value);
			}
		}

		public UnitType AnchorPaddingType
		{
			get
			{
				if (this["AnchorPadding"] == null)
				{
					this["AnchorPadding"] = Unit.Pixel(0);
				}
				return ((Unit)this["AnchorPadding"]).Type;
			}
			set
			{
				var currentValue = AnchorPadding;
				this["AnchorPadding"] = value == UnitType.Pixel ? Unit.Pixel(currentValue) : Unit.Percentage(currentValue);
			}
		}

		public Padding Padding
		{
			get { return (Padding)(this["Padding"] ?? (this["Padding"] = new Padding())); }
		}

		public FilterCollection Filters
		{
			get { return (FilterCollection)(this["Filters"] ?? (this["Filters"] = new FilterCollection())); }
			set { this["Filters"] = value; }
		}

		public BlendMode BlendMode
		{
			get { return (BlendMode)(this["BlendMode"] ?? BlendMode.Normal); }
			set { this["BlendMode"] = value; }
		}

		public FastBitmap Bitmap
		{
			get;
			protected set;
		}

		public Int32Rect? Bounds
		{
			get
			{
				if (Bitmap != null)
					return new Int32Rect(X, Y,
						Bitmap.Width + Padding.Left + Padding.Right,
						Bitmap.Height + Padding.Top + Padding.Bottom);
				return null;
			}
		}

		public Int32Size? Size
		{
			get
			{
				if (Bitmap != null)
					return new Int32Size(
						Bitmap.Width + Padding.Left + Padding.Right,
						Bitmap.Height + Padding.Top + Padding.Bottom);
				return null;
			}
		}

		public abstract bool HasFixedSize
		{
			get;
		}

		internal int CalculatedWidth
		{
			get;
			set;
		}

		internal int CalculatedHeight
		{
			get;
			set;
		}

		#endregion

        public void Process(ImageGenerationContext context)
		{
			CreateImage(context);

			if (Bitmap != null)
				foreach (Filter filter in Filters)
					if (filter.Enabled)
						filter.ApplyFilter(context, Bitmap);
		}

		protected abstract void CreateImage(ImageGenerationContext context);

		public virtual void PopulateDependencies(List<Dependency> dependencies) { }
	}
}
