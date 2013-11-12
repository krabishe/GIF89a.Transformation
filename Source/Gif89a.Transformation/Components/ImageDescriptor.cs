namespace Gif89a.Components
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Extensions;

	internal sealed class ImageDescriptor
	{
		public const byte ImageSeparator = 0x2C;

		public short ImageLeftPosition { get; set; }
		public short ImageTopPosition { get; set; }
		public short ImageWidth { get; set; }
		public short ImageHeight { get; set; }

		public byte PackedFields { get; set; }
		public bool LocalColorTableFlag { get; set; }
		public bool InterlaceFlag { get; set; }
		public bool SortFlag { get; set; }
		public int LocalColorTableSize { get; set; }

		public byte[] ToBytes()
		{
			var result = new List<byte>
				{
					ImageSeparator
				};

			result.AddRange(BitConverter.GetBytes(this.ImageLeftPosition));
			result.AddRange(BitConverter.GetBytes(this.ImageTopPosition));
			result.AddRange(BitConverter.GetBytes(this.ImageWidth));
			result.AddRange(BitConverter.GetBytes(this.ImageHeight));

			var lctFlag = this.LocalColorTableFlag ? 1 : 0;
			var interlaceFlag = this.InterlaceFlag ? 1 : 0;
			var sortFlat = this.SortFlag ? 1 : 0;
			var lctSize = (byte) (Math.Log(this.LocalColorTableSize, 2) - 1);

			var packedFields = (byte) (lctSize | (sortFlat << 5) | (interlaceFlag << 6) | (lctFlag << 7));
			result.Add(packedFields);

			return result.ToArray();
		}

		public static ImageDescriptor Read(Stream stream)
		{
			var result = new ImageDescriptor
				{
					ImageLeftPosition = stream.ReadShort(),
					ImageTopPosition = stream.ReadShort(),
					ImageWidth = stream.ReadShort(),
					ImageHeight = stream.ReadShort(),
					PackedFields = (byte) stream.ReadByte()
				};

			result.LocalColorTableFlag = ((result.PackedFields & 0x80) >> 7) == 1;
			result.InterlaceFlag = ((result.PackedFields & 0x40) >> 6) == 1;
			result.SortFlag = ((result.PackedFields & 0x20) >> 5) == 1;
			result.LocalColorTableSize = (2 << (result.PackedFields & 0x07));

			return result;
		}
	}
}
