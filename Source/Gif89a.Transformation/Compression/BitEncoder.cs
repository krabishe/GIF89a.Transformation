namespace Gif89a.Compression
{
	using System.Collections.Generic;

	internal class BitEncoder
	{
		private int currentBit;
		private int currentVal;
		private readonly List<byte> outList = new List<byte>();

		public BitEncoder(int initialBit)
		{
			this.InitialBit = initialBit;
		}

		public int InitialBit { get; set; }

		public int Length
		{
			get { return this.outList.Count; }
		}

		public void Add(int inByte)
		{
			this.currentVal |= (inByte << (this.currentBit));
			this.currentBit += this.InitialBit;

			while (this.currentBit >= 8)
			{
				var outVal = (byte) (this.currentVal & 0XFF);
				this.currentVal = this.currentVal >> 8;
				this.currentBit -= 8;
				this.outList.Add(outVal);
			}
		}

		public void End()
		{
			while (this.currentBit > 0)
			{
				var outVal = (byte) (this.currentVal & 0XFF);
				this.currentVal = this.currentVal >> 8;
				this.currentBit -= 8;
				this.outList.Add(outVal);
			}
		}

		public void Clear()
		{
			this.outList.Clear();
		}

		public void AddRange(IEnumerable<byte> bytes)
		{
			this.outList.AddRange(bytes);
		}

		public void CopyTo(int index, byte[] array, int arrayIndex, int count)
		{
			this.outList.CopyTo(index, array, arrayIndex, count);
		}

		public byte[] ToArray()
		{
			return this.outList.ToArray();
		}
	}
}
