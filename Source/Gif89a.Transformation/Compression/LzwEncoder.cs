namespace Gif89a.Compression
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Extensions;

	internal class LzwEncoder
	{
		private const int MaxStackSize = 4096;
		private const byte Nullcode = 0;
		private readonly int clearCode;
		private readonly int endCode;
		private readonly byte[] imageData;
		private readonly byte initialColorsDepth;
		private byte colorsDepth;

		public LzwEncoder(byte[] imageData, byte colorsDepth)
		{
			this.imageData = imageData;
			this.colorsDepth = Math.Max((byte) 2, colorsDepth);
			this.initialColorsDepth = this.colorsDepth;
			this.clearCode = (1 << this.colorsDepth);
			this.endCode = this.clearCode + 1;
		}

		public void Encode(Stream stream)
		{
			var first = true;
			int suffix = Nullcode;

			var codeTable = new Dictionary<string, int>();

			var codeSize = (byte) (this.colorsDepth + 1);
			var availableCode = this.endCode + 1;

			var bitEncoder = new BitEncoder(codeSize);

			stream.WriteByte(this.colorsDepth);
			bitEncoder.Add(this.clearCode);

			var releaseCount = 0;
			while (releaseCount < this.imageData.Length)
			{
				if (first)
				{
					suffix = this.imageData[releaseCount++];
					if (releaseCount == this.imageData.Length)
					{
						this.EncodeBits(stream, bitEncoder, suffix);
						break;
					}

					first = false;
					continue;
				}

				var prefix = suffix;
				suffix = this.imageData[releaseCount++];
				var entry = string.Format("{0},{1}", prefix, suffix);

				if (!codeTable.ContainsKey(entry))
				{
					bitEncoder.Add(prefix);
					codeTable.Add(entry, availableCode++);

					if (availableCode > (MaxStackSize - 3))
					{
						codeTable.Clear();
						this.colorsDepth = this.initialColorsDepth;
						codeSize = (byte) (this.colorsDepth + 1);
						availableCode = this.endCode + 1;

						bitEncoder.Add(this.clearCode);
						bitEncoder.InitialBit = codeSize;
					}
					else if (availableCode > (1 << codeSize))
					{
						this.colorsDepth++;
						codeSize = (byte) (this.colorsDepth + 1);
						bitEncoder.InitialBit = codeSize;
					}

					if (bitEncoder.Length >= 255)
					{
						stream.WriteByte(255);
						stream.Write(bitEncoder.ToArray(), 0, 255);
						if (bitEncoder.Length > 255)
						{
							var leftBuffer = new byte[bitEncoder.Length - 255];
							bitEncoder.CopyTo(255, leftBuffer, 0, leftBuffer.Length);
							bitEncoder.Clear();
							bitEncoder.AddRange(leftBuffer);
						}
						else
						{
							bitEncoder.Clear();
						}
					}
				}
				else
				{
					suffix = codeTable[entry];
				}

				if (releaseCount != this.imageData.Length)
				{
					continue;
				}

				this.EncodeBits(stream, bitEncoder, suffix);
				break;
			}
		}

		private void EncodeBits(Stream stream, BitEncoder bitEncoder, int suffix)
		{
			bitEncoder.Add(suffix);
			bitEncoder.Add(this.endCode);
			bitEncoder.End();

			if (bitEncoder.Length > 255)
			{
				var leftBuffer = new byte[bitEncoder.Length - 255];
				bitEncoder.CopyTo(255, leftBuffer, 0, leftBuffer.Length);
				bitEncoder.Clear();
				bitEncoder.AddRange(leftBuffer);
				stream.WriteByte((byte) leftBuffer.Length);
				stream.WriteBytes(leftBuffer);
			}
			else
			{
				stream.WriteByte((byte) (bitEncoder.Length));
				stream.WriteBytes(bitEncoder.ToArray());
				bitEncoder.Clear();
			}
		}
	}
}
