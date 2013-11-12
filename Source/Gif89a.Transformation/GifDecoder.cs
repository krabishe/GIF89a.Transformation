namespace Gif89a
{
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.IO;
	using Components;
	using Compression;
	using Extensions;
	using Image = Components.Image;

	internal static class GifDecoder
	{
		public static Image Decode(string filePath)
		{
			using (var fileStream = File.Open(filePath, FileMode.Open))
			{
				return Decode(fileStream);
			}
		}

		public static Image Decode(Stream stream)
		{
			var result = new Image
				{
					Header = stream.ReadString(6),
					LogicalScreenDescriptor = LogicalScreenDescriptor.Read(stream)
				};

			if (result.LogicalScreenDescriptor.GlobalColorTableFlag)
			{
				result.GlobalColorTable = stream.ReadBytes(result.LogicalScreenDescriptor.GlobalColorTableSize * 3);
			}

			GraphicControlExtension graphicControlExtension = null;

			var componentType = stream.ReadByte();
			while (componentType != 0)
			{
				if (componentType == ImageDescriptor.ImageSeparator)
				{
					var imageFrame = ReadImageFrame(stream, result.GlobalColorTable, graphicControlExtension);
					result.Frames.Add(imageFrame);
				}
				else if (componentType == Const.ExtensionIntroducer)
				{
					var extensionType = stream.ReadByte();
					switch (extensionType)
					{
						case GraphicControlExtension.GraphicControlLabel:
							{
								graphicControlExtension = GraphicControlExtension.Read(stream);
								break;
							}
						case CommentExtension.CommentLabel:
							{
								var commentExtension = CommentExtension.Read(stream);
								result.CommentExtensions.Add(commentExtension);
								break;
							}
						case ApplicationExtension.ExtensionLabel:
							{
								var applicationExtension = ApplicationExtension.Read(stream);
								result.ApplictionExtensions.Add(applicationExtension);
								break;
							}
						case PlainTextExtension.PlainTextLabel:
							{
								var plainTextExtension = PlainTextExtension.Read(stream);
								result.PlainTextEntensions.Add(plainTextExtension);
								break;
							}
					}
				}
				else if (componentType == Const.EndIntroducer)
				{
					break;
				}

				componentType = stream.ReadByte();
			}

			return result;
		}

		private static ImageFrame ReadImageFrame(Stream stream, byte[] globalColorTable, GraphicControlExtension graphicControlExtension)
		{
			var imageDescriptor = ImageDescriptor.Read(stream);

			var imageFrame = new ImageFrame
				{
					ImageDescriptor = imageDescriptor,
					LocalColorTable = globalColorTable,
					GraphicControlExtension = graphicControlExtension
				};

			if (imageDescriptor.LocalColorTableFlag)
			{
				imageFrame.LocalColorTable = stream.ReadBytes(imageDescriptor.LocalColorTableSize * 3);
			}

			imageFrame.ColorDepth = stream.ReadByte();

			var lzwDecoder = new LzwDecoder(stream);
			var imageData = lzwDecoder.DecodeImageData(imageDescriptor.ImageWidth, imageDescriptor.ImageHeight, imageFrame.ColorDepth);

			ApplicationData.Read(stream);

			imageFrame.Bitmap = CreateBitmap(
				imageData,
				imageFrame.GetPalette(),
				imageDescriptor.InterlaceFlag,
				imageDescriptor.ImageWidth,
				imageDescriptor.ImageHeight);

			return imageFrame;
		}

		private static Bitmap CreateBitmap(byte[] imageData, Color32[] colorsTable, bool interlaceFlag, int imageWidth, int imageHeight)
		{
			var result = new Bitmap(imageWidth, imageHeight);

			var rectangle = new Rectangle(0, 0, imageWidth, imageHeight);
			var bitmapData = result.LockBits(rectangle, ImageLockMode.ReadWrite, result.PixelFormat);

			if (interlaceFlag)
			{
				FillBitmapFromInterlacedImageData(imageData, colorsTable, bitmapData);
			}
			else
			{
				FillBitmapFromImageData(imageData, colorsTable, bitmapData);
			}

			result.UnlockBits(bitmapData);
			return result;
		}

		private static unsafe void FillBitmapFromImageData(byte[] imageData, Color32[] colorsTable, BitmapData bitmapData)
		{
			var p = (Color32*) bitmapData.Scan0.ToPointer();

			var i = 0;
			while (i < imageData.Length)
			{
				*p++ = colorsTable[imageData[i++]];
			}
		}

		private static unsafe void FillBitmapFromInterlacedImageData(byte[] imageData, Color32[] colorsTable, BitmapData bitmapData)
		{
			var p = (Color32*) bitmapData.Scan0.ToPointer();

			var rates = new[] { 8, 8, 4, 2, 1 };
			var startPosition = p;
			var offset = 0;

			var i = 0;
			var pass = 0;

			while (pass < 4)
			{
				if (pass > 0)
				{
					var diff = rates[pass + 1] * bitmapData.Width;
					p = startPosition + diff;
					offset += diff;
				}

				var rate = rates[pass];

				while (i < imageData.Length)
				{
					*p++ = colorsTable[imageData[i++]];
					offset++;

					if (i % bitmapData.Width != 0)
					{
						continue;
					}

					p += bitmapData.Width * (rate - 1);
					offset += bitmapData.Width * (rate - 1);

					if (offset >= imageData.Length)
					{
						pass++;
						offset = 0;
						break;
					}
				}
			}
		}
	}
}
