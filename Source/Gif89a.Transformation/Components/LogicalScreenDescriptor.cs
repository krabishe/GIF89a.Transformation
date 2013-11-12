namespace Gif89a.Components
{
	using System;
	using System.IO;
	using Extensions;

	internal sealed class LogicalScreenDescriptor
	{
		public short LogicalScreenWidth { get; set; }
		public short LogicalScreenHeight { get; set; }

		public byte PackedFields { get; set; }
		public bool GlobalColorTableFlag { get; set; }
		public byte ColorResolution { get; set; }
		public int SortFlag { get; set; }
		public int GlobalColorTableSize { get; set; }

		public byte BackgroundColorIndex { get; set; }
		public byte PixelAspectRatio { get; set; }

		public byte[] ToBytes()
		{
			var result = new byte[7];

			Array.Copy(BitConverter.GetBytes(this.LogicalScreenWidth), 0, result, 0, 2);
			Array.Copy(BitConverter.GetBytes(this.LogicalScreenHeight), 0, result, 2, 2);

			var gctFlag = this.GlobalColorTableFlag ? 1 : 0;
			var gctSize = (byte) (Math.Log(this.GlobalColorTableSize, 2) - 1);
			this.PackedFields = (byte) (gctSize | (this.SortFlag << 4) | (this.ColorResolution << 5) | (gctFlag << 7));

			result[4] = this.PackedFields;
			result[5] = this.BackgroundColorIndex;
			result[6] = this.PixelAspectRatio;

			return result;
		}

		public static LogicalScreenDescriptor Read(Stream stream)
		{
			var result = new LogicalScreenDescriptor
				{
					LogicalScreenWidth = stream.ReadShort(),
					LogicalScreenHeight = stream.ReadShort(),
					PackedFields = (byte) stream.ReadByte()
				};

			result.GlobalColorTableFlag = ((result.PackedFields & 0x80) >> 7) == 1;
			result.ColorResolution = (byte) ((result.PackedFields & 0x60) >> 5);
			result.SortFlag = (byte) (result.PackedFields & 0x10) >> 4;
			result.GlobalColorTableSize = 2 << (result.PackedFields & 0x07);
			result.BackgroundColorIndex = (byte) stream.ReadByte();
			result.PixelAspectRatio = (byte) stream.ReadByte();

			return result;
		}
	}
}
