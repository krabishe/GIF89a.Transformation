namespace Gif89a.Components
{
	using System.Collections.Generic;
	using System.IO;
	using Extensions;

	internal sealed class CommentExtension
	{
		public const byte CommentLabel = 0xFE;
		public List<string> CommentData { get; set; }

		public byte[] ToBytes()
		{
			var result = new List<byte>
				{
					Const.ExtensionIntroducer,
					CommentLabel
				};

			foreach (var commentData in this.CommentData)
			{
				var commentCharArray = commentData.ToCharArray();
				result.Add((byte) commentCharArray.Length);
				result.AddRange(commentCharArray.ToBytes());
			}

			result.Add(Const.BlockTerminator);

			return result.ToArray();
		}

		public static CommentExtension Read(Stream stream)
		{
			var result = new CommentExtension
				{
					CommentData = new List<string>()
				};

			var blockSize = stream.ReadByte();
			while (blockSize > 0)
			{
				var commentData = stream.ReadString(blockSize);
				result.CommentData.Add(commentData);

				blockSize = stream.ReadByte();
			}

			return result;
		}
	}
}
