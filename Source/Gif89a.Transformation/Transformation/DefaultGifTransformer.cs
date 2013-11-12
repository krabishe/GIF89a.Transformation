namespace Gif89a.Transformation
{
	using System;
	using System.Drawing;
	using System.IO;
	using Processors;

	public static class DefaultGifTransformer
	{
		public static void Crop(string sourceImagePath, string destImagePath, Rectangle cropArea)
		{
			Crop(sourceImagePath, destImagePath, (width, height) => cropArea);
		}

		public static void Crop(Stream sourceImage, Stream destImage, Rectangle cropArea)
		{
			Crop(sourceImage, destImage, (width, height) => cropArea);
		}

		public static void Crop(string sourceImagePath, string destImagePath, Func<int, int, Rectangle> cropAreaProvider)
		{
			using (var sourceImage = new FileStream(sourceImagePath, FileMode.Open))
			using (var destImage = new FileStream(destImagePath, FileMode.Create))
			{
				Crop(sourceImage, destImage, cropAreaProvider);
			}
		}

		public static void Crop(Stream sourceImage, Stream destImage, Func<int, int, Rectangle> cropAreaProvider)
		{
			GifTransformer.Transform(sourceImage, destImage, new CropBitmapProcessor(cropAreaProvider));
		}

		public static void Resize(string sourceImagePath, string destImagePath, Size resizeArea)
		{
			Resize(sourceImagePath, destImagePath, (width, height) => resizeArea);
		}

		public static void Resize(Stream sourceImage, Stream destImage, Size resizeArea)
		{
			Resize(sourceImage, destImage, (width, height) => resizeArea);
		}

		public static void Resize(string sourceImagePath, string destImagePath, Func<int, int, Size> resizeAreaProvider)
		{
			using (var sourceImage = new FileStream(sourceImagePath, FileMode.Open))
			using (var destImage = new FileStream(destImagePath, FileMode.Create))
			{
				Resize(sourceImage, destImage, resizeAreaProvider);
			}
		}

		public static void Resize(Stream sourceImage, Stream destImage, Func<int, int, Size> resizeAreaProvider)
		{
			GifTransformer.Transform(sourceImage, destImage, new ResizeBitmapProcessor(resizeAreaProvider));
		}

		public static void ResizeByWidth(string sourceImagePath, string destImagePath, int width)
		{
			Resize(sourceImagePath, destImagePath, new Size(width, int.MaxValue));
		}

		public static void ResizeByWidth(Stream sourceImage, Stream destImage, int width)
		{
			Resize(sourceImage, destImage, new Size(width, int.MaxValue));
		}

		public static void ResizeByHeight(string sourceImagePath, string destImagePath, int height)
		{
			Resize(sourceImagePath, destImagePath, new Size(int.MaxValue, height));
		}

		public static void ResizeByHeight(Stream sourceImage, Stream destImage, int height)
		{
			Resize(sourceImage, destImage, new Size(int.MaxValue, height));
		}
	}
}
