namespace Gif89a.Components
{
	using System;
	using System.IO;
	using Extensions;

	internal sealed class ApplicationData
	{
		private ApplicationData(byte blockSize, byte[] data)
		{
			this.BlockSize = blockSize;
			this.Data = data;
		}

		public byte BlockSize { get; private set; }

		public byte[] Data { get; private set; }

		public byte[] ToBytes()
		{
			var result = new byte[this.BlockSize + 1];

			result[0] = this.BlockSize;
			Array.Copy(this.Data, 0, result, 1, this.BlockSize);

			return result;
		}

		public static ApplicationData Read(Stream stream)
		{
			var blockSize = stream.ReadByte();
			if (blockSize <= 0)
			{
				return null;
			}

			var data = stream.ReadBytes(blockSize);
			return new ApplicationData((byte) blockSize, data);
		}
	}
}
