namespace Gif89a.Components
{
	using System.Collections.Generic;
	using System.IO;
	using Extensions;

	internal sealed class ApplicationExtension
	{
		public const byte ExtensionLabel = 0xFF;
		private const byte BlockSize = 0x0B;

		public char[] ApplicationIdentifier { get; set; }
		public char[] ApplicationAuthenticationCode { get; set; }
		public List<ApplicationData> ApplicationData { get; set; }

		public byte[] ToBytes()
		{
			var result = new List<byte>
				{
					Const.ExtensionIntroducer,
					ExtensionLabel,
					BlockSize
				};

			if (this.ApplicationIdentifier == null)
			{
				this.ApplicationIdentifier = "NETSCAPE".ToCharArray();
			}

			result.AddRange(this.ApplicationIdentifier.ToBytes());

			if (this.ApplicationAuthenticationCode == null)
			{
				this.ApplicationAuthenticationCode = "2.0".ToCharArray();
			}

			result.AddRange(this.ApplicationAuthenticationCode.ToBytes());

			foreach (var applicationData in this.ApplicationData)
			{
				result.AddRange(applicationData.ToBytes());
			}

			result.Add(Const.BlockTerminator);

			return result.ToArray();
		}

		public static ApplicationExtension Read(Stream stream)
		{
			var blockSize = stream.ReadByte();
			if (blockSize != BlockSize)
			{
				throw new GifException("Application extension data format error");
			}

			var result = new ApplicationExtension
				{
					ApplicationIdentifier = stream.ReadChars(8),
					ApplicationAuthenticationCode = stream.ReadChars(3),
					ApplicationData = new List<ApplicationData>()
				};

			var applicationData = Components.ApplicationData.Read(stream);
			while (applicationData != null)
			{
				result.ApplicationData.Add(applicationData);
				applicationData = Components.ApplicationData.Read(stream);
			}

			return result;
		}
	}
}
