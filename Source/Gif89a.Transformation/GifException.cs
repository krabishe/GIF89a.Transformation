namespace Gif89a
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class GifException : Exception
	{
		public GifException()
		{
		}

		public GifException(string message)
			: base(message)
		{
		}

		public GifException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected GifException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
