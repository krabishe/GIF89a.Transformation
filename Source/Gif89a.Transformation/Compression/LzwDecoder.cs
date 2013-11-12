namespace Gif89a.Compression
{
	using System.Collections.Generic;
	using System.IO;
	using Extensions;

	internal class LzwDecoder
	{
		private const int NullCode = -1;
		private const int MaxStackSize = 4096;
		private readonly Stream stream;

		public LzwDecoder(Stream stream)
		{
			this.stream = stream;
		}

		public byte[] DecodeImageData(int width, int height, int dataSize)
		{
			var pixelsCount = width * height;
			var imageData = new byte[pixelsCount];

			var codeSize = dataSize + 1;

			var clearCode = 1 << dataSize;
			var endCode = clearCode + 1;
			var available = endCode + 1;

			int code;
			var oldCode = NullCode;
			var codeMask = (1 << codeSize) - 1;
			var bits = 0;

			var prefix = new int[MaxStackSize];
			var suffix = new int[MaxStackSize];

			var pixelsStack = new Stack<int>(MaxStackSize + 1);

			var count = 0;
			var bi = 0;

			var data = 0;
			var first = 0;

			for (code = 0; code < clearCode; code++)
			{
				prefix[code] = 0;
				suffix[code] = (byte) code;
			}

			byte[] buffer = null;

			var i = 0;
			while (i < pixelsCount)
			{
				if (pixelsStack.Count == 0)
				{
					if (bits < codeSize)
					{
						if (count == 0)
						{
							buffer = this.ReadData();
							count = buffer.Length;
							if (count == 0)
							{
								break;
							}

							bi = 0;
						}

						data += buffer[bi] << bits;
						bits += 8;
						bi++;
						count--;
						continue;
					}

					code = data & codeMask;
					data >>= codeSize;
					bits -= codeSize;

					if (code > available || code == endCode)
					{
						break;
					}

					if (code == clearCode)
					{
						codeSize = dataSize + 1;
						codeMask = (1 << codeSize) - 1;
						available = clearCode + 2;
						oldCode = NullCode;
						continue;
					}

					if (oldCode == NullCode)
					{
						pixelsStack.Push(suffix[code]);
						oldCode = code;
						first = code;
						continue;
					}

					var inCode = code;
					if (code == available)
					{
						pixelsStack.Push((byte) first);
						code = oldCode;
					}

					while (code > clearCode)
					{
						pixelsStack.Push(suffix[code]);
						code = prefix[code];
					}

					first = suffix[code];
					if (available > MaxStackSize)
					{
						break;
					}

					pixelsStack.Push(suffix[code]);
					prefix[available] = oldCode;
					suffix[available] = first;

					available++;
					if (available == codeMask + 1 && available < MaxStackSize)
					{
						codeSize++;
						codeMask = (1 << codeSize) - 1;
					}

					oldCode = inCode;
				}

				imageData[i++] = (byte) pixelsStack.Pop();
			}

			return imageData;
		}

		private byte[] ReadData()
		{
			var blockSize = this.stream.ReadByte();
			return this.stream.ReadBytes(blockSize);
		}
	}
}
