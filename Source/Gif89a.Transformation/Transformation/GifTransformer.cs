namespace Gif89a.Transformation
{
	using System;
	using System.Drawing;
	using System.IO;
	using Components;
	using Quantize;
	using Image = Components.Image;

	public static class GifTransformer
	{
		public static void Transform(Stream inputImage, Stream outputImage, IBitmapProcessor bitmapProcessor)
		{
			var image = GifDecoder.Decode(inputImage);

			ProcessDisposalMethod(image);

			image.LogicalScreenDescriptor.LogicalScreenWidth = 1;
			image.LogicalScreenDescriptor.LogicalScreenHeight = 1;

			foreach (var frame in image.Frames)
			{
				TransformFrame(bitmapProcessor, frame);

				image.LogicalScreenDescriptor.LogicalScreenWidth = Math.Max(image.LogicalScreenDescriptor.LogicalScreenWidth, frame.ImageDescriptor.ImageWidth);
				image.LogicalScreenDescriptor.LogicalScreenHeight = Math.Max(image.LogicalScreenDescriptor.LogicalScreenHeight, frame.ImageDescriptor.ImageHeight);
			}

			GifEncoder.Encode(image, outputImage);
		}

		private static void TransformFrame(IBitmapProcessor bitmapProcessor, ImageFrame frame)
		{
			var bitmapCopy = (Bitmap)frame.Bitmap.Clone();
			frame.Bitmap.Dispose();

			frame.Bitmap = bitmapProcessor.Process(bitmapCopy, frame.GetBackgroundColor().Color);

			DefaultQuantizer.Quantize(frame.Bitmap, frame.GetPalette());

			frame.ImageDescriptor.ImageWidth = (short)frame.Bitmap.Width;
			frame.ImageDescriptor.ImageHeight = (short)frame.Bitmap.Height;
		}

		private static void ProcessDisposalMethod(Image image)
		{
			Bitmap lastBitmap = null;
			var lastDisposal = 0;
			var index = 0;

			foreach (var frame in image.Frames)
			{
				var offsetX = frame.ImageDescriptor.ImageLeftPosition;
				var offsetY = frame.ImageDescriptor.ImageTopPosition;
				var frameWidth = frame.ImageDescriptor.ImageWidth;
				var frameHeight = frame.ImageDescriptor.ImageHeight;

				if (frame.Bitmap.Width != image.Width || frame.Bitmap.Height != image.Height)
				{
					frame.ImageDescriptor.ImageLeftPosition = 0;
					frame.ImageDescriptor.ImageTopPosition = 0;
					frame.ImageDescriptor.ImageWidth = image.Width;
					frame.ImageDescriptor.ImageHeight = image.Height;
				}

				var transparentColorIndex = -1;
				if (frame.GraphicControlExtension != null && frame.GraphicControlExtension.TransparentColorFlag)
				{
					transparentColorIndex = frame.GraphicControlExtension.TransparentColorIndex;
				}

				if (frameWidth != image.Width || frameHeight != image.Height || index != 0)
				{
					var bgColor = Convert.ToInt32(image.GlobalColorIndexedTable[transparentColorIndex]);
					var color = Color.FromArgb(bgColor);

					var bitmap = new Bitmap(image.Width, image.Height);
					using (var graphics = Graphics.FromImage(bitmap))
					{
						if (lastBitmap != null)
						{
							graphics.DrawImageUnscaled(lastBitmap, new Point(0, 0));
						}

						if (frame.GraphicControlExtension != null && frame.GraphicControlExtension.DisposalMethod == 1)
						{
							graphics.DrawRectangle(new Pen(new SolidBrush(color)), new Rectangle(offsetX, offsetY, frameWidth, frameHeight));
						}

						if (frame.GraphicControlExtension != null && frame.GraphicControlExtension.DisposalMethod == 2 && lastDisposal != 1)
						{
							graphics.Clear(color);
						}

						graphics.DrawImageUnscaled(frame.Bitmap, new Point(offsetX, offsetY));
					}

					frame.Bitmap.Dispose();
					frame.Bitmap = bitmap;
				}

				lastBitmap = frame.Bitmap;
				DefaultQuantizer.Quantize(frame.Bitmap, frame.GetPalette());

				if (frame.GraphicControlExtension != null)
				{
					lastDisposal = frame.GraphicControlExtension.DisposalMethod;
				}

				index++;
			}
		}
	}
}
