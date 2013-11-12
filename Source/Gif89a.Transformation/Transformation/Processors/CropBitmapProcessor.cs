namespace Gif89a.Transformation.Processors
{
	using System;
	using System.Drawing;

	public class CropBitmapProcessor : IBitmapProcessor
	{
		private readonly Rectangle? cropArea;
		private readonly Func<int, int, Rectangle> cropAreaProvider;

		public CropBitmapProcessor(Rectangle cropArea)
		{
			this.cropArea = cropArea;
		}

		public CropBitmapProcessor(Func<int, int, Rectangle> cropAreaProvider)
		{
			this.cropAreaProvider = cropAreaProvider;
		}

		public Bitmap Process(Bitmap source, Color bgColor)
		{
			var actualCropArea = this.cropArea ?? this.cropAreaProvider(source.Width, source.Height);

			var left = Math.Max(0, actualCropArea.Left);
			var top = Math.Max(0, actualCropArea.Top);
			var width = Math.Min(source.Width - left, actualCropArea.Width);
			var height = Math.Min(source.Height - top, actualCropArea.Height);

			var safeCropArea = new Rectangle(left, top, width, height);

			return source.Clone(safeCropArea, source.PixelFormat);
		}
	}
}
