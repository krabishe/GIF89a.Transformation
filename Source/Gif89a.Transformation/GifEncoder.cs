namespace Gif89a
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.IO;
	using Components;
	using Compression;
	using Extensions;
	using Image = Components.Image;

	internal static class GifEncoder
	{
		public static void Encode(Image image, string filePath)
		{
			using (var fileStream = File.Open(filePath, FileMode.Create))
			{
				Encode(image, fileStream);
			}
		}

		public static void Encode(Image image, Stream stream)
		{
			stream.WriteString(image.Header);
			stream.WriteBytes(image.LogicalScreenDescriptor.ToBytes());

			if (image.LogicalScreenDescriptor.GlobalColorTableFlag)
			{
				stream.WriteBytes(image.GlobalColorTable);
			}

			foreach (var applicationExtension in image.ApplictionExtensions)
			{
				stream.WriteBytes(applicationExtension.ToBytes());
			}

			foreach (var commentExtension in image.CommentExtensions)
			{
				stream.WriteBytes(commentExtension.ToBytes());
			}

			WriteFrames(image.Frames, stream);
		}

		private static void WriteFrames(IEnumerable<ImageFrame> frames, Stream stream)
		{
			foreach (var frame in frames)
			{
				var frameBytes = new List<byte>();
				if (frame.GraphicControlExtension != null)
				{
					frameBytes.AddRange(frame.GraphicControlExtension.ToBytes());
				}

				frame.ImageDescriptor.SortFlag = false;
				frame.ImageDescriptor.InterlaceFlag = false;

				frameBytes.AddRange(frame.ImageDescriptor.ToBytes());
				if (frame.ImageDescriptor.LocalColorTableFlag)
				{
					frameBytes.AddRange(frame.LocalColorTable);
				}

				stream.WriteBytes(frameBytes.ToArray());

				var transparentColorIndex = -1;
				if (frame.GraphicControlExtension != null && frame.GraphicControlExtension.TransparentColorFlag)
				{
					transparentColorIndex = frame.GraphicControlExtension.TransparentColorIndex;
				}

				var imageData = GetImageData(frame.Bitmap, frame.LocalColorTable, transparentColorIndex);

				var lzwEncoder = new LzwEncoder(imageData, (byte) frame.ColorDepth);
				lzwEncoder.Encode(stream);

				stream.WriteByte(Const.BlockTerminator);
			}

			stream.WriteByte(Const.EndIntroducer);
		}

		private static byte[] GetImageData(Bitmap image, byte[] bytesColorTable, int transparentColorIndex)
		{
			var result = new byte[image.Width * image.Height];

			var colorsTable = GetColorsTable(bytesColorTable, transparentColorIndex);
			var bmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);

			unsafe
			{
				var p = (int*) bmpData.Scan0.ToPointer();
				for (var i = 0; i < image.Width * image.Height; ++i)
				{
					result[i] = Convert.ToByte(colorsTable[p[i]]);
				}
			}

			image.UnlockBits(bmpData);
			return result;
		}

		private static IDictionary<int, int> GetColorsTable(byte[] bytesColorTable, int transparentColorIndex)
		{
			var result = new Dictionary<int, int>();

			var bytesTableIndex = 0;
			var resultTableIndex = 0;
			while (bytesTableIndex < bytesColorTable.Length)
			{
				var color = 0;
				if (resultTableIndex == transparentColorIndex)
				{
					bytesTableIndex += 3;
				}
				else
				{
					int r = bytesColorTable[bytesTableIndex++];
					int g = bytesColorTable[bytesTableIndex++];
					int b = bytesColorTable[bytesTableIndex++];
					color = (255 << 24) | (r << 16) | (g << 8) | b;
				}

				if (!result.ContainsKey(color))
				{
					result.Add(color, resultTableIndex++);
				}
			}

			return result;
		}
	}
}
