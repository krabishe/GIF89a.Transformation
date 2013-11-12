namespace Gif89a.Components
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Extensions;

	internal sealed class GraphicControlExtension
	{
		public const byte GraphicControlLabel = 0xF9;
		private const byte BlockSize = 4;

		public byte PackedFields { get; set; }
		public int DisposalMethod { get; set; }
		public bool TransparentColorFlag { get; set; }

		public short DelayTime { get; set; }
		public byte TransparentColorIndex { get; set; }

		public byte[] ToBytes()
		{
			var result = new List<byte>
				{
					Const.ExtensionIntroducer,
					GraphicControlLabel,
					BlockSize
				};

			var transparencyFlag = this.TransparentColorFlag ? 1 : 0;
			this.PackedFields = (byte) ((this.DisposalMethod << 2) | transparencyFlag);

			result.Add(this.PackedFields);
			result.AddRange(BitConverter.GetBytes(this.DelayTime));
			result.Add(this.TransparentColorIndex);
			result.Add(Const.BlockTerminator);

			return result.ToArray();
		}

		public static GraphicControlExtension Read(Stream stream)
		{
			var blockSize = stream.ReadByte();
			if (blockSize != BlockSize)
			{
				throw new GifException("Graphic control extension data format error");
			}

			var result = new GraphicControlExtension
				{
					PackedFields = (byte) stream.ReadByte()
				};

			result.TransparentColorFlag = (result.PackedFields & 0x01) == 1;
			result.DisposalMethod = (result.PackedFields & 0x1C) >> 2;
			result.DelayTime = stream.ReadShort();
			result.TransparentColorIndex = (byte) stream.ReadByte();
			stream.ReadByte();

			return result;
		}
	}
}
