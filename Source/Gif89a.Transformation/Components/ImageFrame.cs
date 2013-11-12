namespace Gif89a.Components
{
	using System.Drawing;

	internal sealed class ImageFrame
	{
		public ImageFrame()
		{
			this.ColorDepth = 3;
		}

		public short Delay
		{
			get { return this.GraphicControlExtension.DelayTime; }
			set { this.GraphicControlExtension.DelayTime = value; }
		}

		public Bitmap Bitmap { get; set; }

		public ImageDescriptor ImageDescriptor { get; set; }

		public int ColorDepth { get; set; }

		public byte[] LocalColorTable { get; set; }

		public GraphicControlExtension GraphicControlExtension { get; set; }

		public Color32[] GetPalette()
		{
			var localColorTable = Palette.Get(this.LocalColorTable);
			if (this.GraphicControlExtension != null && this.GraphicControlExtension.TransparentColorFlag)
			{
				localColorTable[this.GraphicControlExtension.TransparentColorIndex] = new Color32(0);
			}

			return localColorTable;
		}

		public Color32 GetBackgroundColor()
		{
			if (this.GraphicControlExtension == null)
			{
				return new Color32(0);
			}

			var localColorTable = Palette.Get(this.LocalColorTable);
			return localColorTable[this.GraphicControlExtension.TransparentColorIndex];
		}
	}
}
