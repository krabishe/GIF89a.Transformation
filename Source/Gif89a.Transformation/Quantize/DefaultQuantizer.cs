namespace Gif89a.Quantize
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Imaging;

	internal static class DefaultQuantizer
	{
		public static void Quantize(Bitmap bitmap, Color32[] colorsTable)
		{
			var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
			var colorsDictionary = new Dictionary<Color, Color32>();

			unsafe
			{
				var p = (int*)bitmapData.Scan0.ToPointer();
				for (var i = 0; i < bitmap.Height * bitmap.Width; i++)
				{
					var color = Color.FromArgb(p[i]);

					Color32 closestColor;
					if (!colorsDictionary.TryGetValue(color, out closestColor))
					{
						closestColor = FindClosestColor(color, colorsTable);
						colorsDictionary[color] = closestColor;
					}

					p[i] = closestColor.ARGB;
				}
			}

			bitmap.UnlockBits(bitmapData);
		}

		private static Color32 FindClosestColor(Color sourceColor, IList<Color32> colorsTable)
		{
			var minColor = int.MaxValue;
			var minColorIndex = 0;

			for (var index = 0; index < colorsTable.Count; ++index)
			{
				var currentColor = colorsTable[index].Color;

				var cr = Math.Abs(sourceColor.R - currentColor.R);
				var cg = Math.Abs(sourceColor.G - currentColor.G);
				var cb = Math.Abs(sourceColor.B - currentColor.B);
				var ca = Math.Abs(sourceColor.A - currentColor.A);
				var result = cr + cg + cb + ca;

				if (result == 0)
				{
					minColorIndex = index;
					break;
				}

				if (result < minColor)
				{
					minColor = result;
					minColorIndex = index;
				}
			}

			return colorsTable[minColorIndex];
		}
	}
}
