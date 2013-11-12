namespace Gif89a
{
	using System.Drawing;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Explicit)]
	internal struct Color32
	{
		[FieldOffset(0)]
		public byte Blue;

		[FieldOffset(1)]
		public byte Green;

		[FieldOffset(2)]
		public byte Red;

		[FieldOffset(3)]
		public byte Alpha;

		[FieldOffset(0)]
		public readonly int ARGB;

		public Color Color
		{
			get { return Color.FromArgb(this.ARGB); }
		}

		public Color32(int argb)
		{
			this.Alpha = 0;
			this.Red = 0;
			this.Green = 0;
			this.Blue = 0;
			this.ARGB = argb;
		}

		public Color32(byte r, byte g, byte b)
			: this(255, r, g, b)
		{
		}

		public Color32(byte a, byte r, byte g, byte b)
		{
			this.ARGB = 0;
			this.Alpha = a;
			this.Red = r;
			this.Green = g;
			this.Blue = b;
		}
	}
}
