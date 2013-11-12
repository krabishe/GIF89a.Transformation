namespace Gif89a.Components
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Extensions;

	internal sealed class PlainTextExtension
	{
		public const byte PlainTextLabel = 0x01;
		private const byte BlockSize = 0x0C;

		public byte CharacterCellHeight { get; set; }
		public byte CharacterCellWidth { get; set; }
		public List<string> PlainTextData { get; set; }
		public byte TextBackgroundColorIndex { get; set; }
		public byte TextForegroundColorIndex { get; set; }
		public short TextGridHeight { get; set; }

		public short TextGridLeftPosition { get; set; }
		public short TextGridTopPosition { get; set; }
		public short TextGridWidth { get; set; }

		public byte[] ToBytes()
		{
			var result = new List<byte>
				{
					Const.ExtensionIntroducer,
					PlainTextLabel,
					BlockSize
				};

			result.AddRange(BitConverter.GetBytes(this.TextGridLeftPosition));
			result.AddRange(BitConverter.GetBytes(this.TextGridTopPosition));
			result.AddRange(BitConverter.GetBytes(this.TextGridWidth));
			result.AddRange(BitConverter.GetBytes(this.TextGridHeight));
			result.Add(this.CharacterCellWidth);
			result.Add(this.CharacterCellHeight);
			result.Add(this.TextForegroundColorIndex);
			result.Add(this.TextBackgroundColorIndex);

			foreach (var plainTextData in this.PlainTextData)
			{
				result.Add((byte) plainTextData.Length);
				result.AddRange(plainTextData.ToBytes());
			}

			result.Add(Const.BlockTerminator);

			return result.ToArray();
		}

		public static PlainTextExtension Read(Stream stream)
		{
			var blockSize = stream.ReadByte();
			if (blockSize != BlockSize)
			{
				throw new GifException("Plain text extension data format error");
			}

			var result = new PlainTextExtension
				{
					TextGridLeftPosition = stream.ReadShort(),
					TextGridTopPosition = stream.ReadShort(),
					TextGridWidth = stream.ReadShort(),
					TextGridHeight = stream.ReadShort(),
					CharacterCellWidth = (byte) stream.ReadByte(),
					CharacterCellHeight = (byte) stream.ReadByte(),
					TextForegroundColorIndex = (byte) stream.ReadByte(),
					TextBackgroundColorIndex = (byte) stream.ReadByte(),
					PlainTextData = new List<string>()
				};

			blockSize = stream.ReadByte();
			while (blockSize > 0)
			{
				var plainTextData = stream.ReadString(blockSize);
				result.PlainTextData.Add(plainTextData);

				blockSize = stream.ReadByte();
			}

			return result;
		}
	}
}
