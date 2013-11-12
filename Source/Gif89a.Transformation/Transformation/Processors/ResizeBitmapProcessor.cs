namespace Gif89a.Transformation.Processors
{
	using System;
	using System.Drawing;
	using System.Drawing.Drawing2D;

	public class ResizeBitmapProcessor : IBitmapProcessor
	{
		private readonly Func<int, int, Size> resizeAreaProvider;
		private readonly Size? size;

		public ResizeBitmapProcessor(int width, int height)
		{
			this.size = new Size(width, height);
		}

		public ResizeBitmapProcessor(Func<int, int, Size> resizeAreaProvider)
		{
			this.resizeAreaProvider = resizeAreaProvider;
		}

		public Bitmap Process(Bitmap source, Color bgColor)
		{
			var destSize = this.size ?? this.resizeAreaProvider(source.Width, source.Height);

			var destWidth = Math.Min(destSize.Width, source.Width);
			var destHeight = Math.Min(destSize.Height, source.Height);

			if (destWidth == source.Width && destHeight == source.Height)
			{
				return (Bitmap)source.Clone();
			}

			var offsetX = 0.0f;
			var offsetY = 0.0f;
			var actualRectangle = new Rectangle();

			if (destWidth < destHeight)
			{
				actualRectangle.Width = destWidth;
				actualRectangle.Height = (int)(1.0 * source.Height / source.Width * destWidth);

				if (actualRectangle.Height > destHeight)
				{
					offsetY = (int)((source.Height - (source.Height * (1.0 * destHeight / actualRectangle.Height))) / 2);
					actualRectangle.Height = destHeight;
				}
			}
			else
			{
				actualRectangle.Height = destHeight;
				actualRectangle.Width = (int)((1.0 * source.Width / source.Height) * destHeight);

				if (actualRectangle.Width > destWidth)
				{
					offsetX = (int)((source.Width - (source.Width * (1.0 * destWidth / actualRectangle.Width))) / 2);
					actualRectangle.Width = destWidth;
				}
			}

			var result = new Bitmap(actualRectangle.Width, actualRectangle.Height);
			using (var graphics = Graphics.FromImage(result))
			{
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;

				graphics.FillRectangle(new SolidBrush(bgColor), actualRectangle);
				graphics.DrawImage(source, actualRectangle, offsetX, offsetY, source.Width - offsetX, source.Height - offsetY, GraphicsUnit.Pixel);
			}

			return result;
		}
	}
}
