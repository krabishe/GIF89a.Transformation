namespace Gif89a.Extensions
{
	using System;
	using System.IO;
	using System.Linq;

	internal static class StreamExtensions
	{
		public static byte[] ReadBytes(this Stream stream, int length)
		{
			var result = new byte[length];
			stream.Read(result, 0, length);

			return result;
		}

		public static short ReadShort(this Stream stream)
		{
			var buffer = new byte[2];
			stream.Read(buffer, 0, buffer.Length);

			return BitConverter.ToInt16(buffer, 0);
		}

		public static string ReadString(this Stream stream, int length)
		{
			return new string(stream.ReadChars(length));
		}

		public static char[] ReadChars(this Stream stream, int length)
		{
			var buffer = new byte[length];
			stream.Read(buffer, 0, length);

			var charBuffer = new char[length];
			buffer.CopyTo(charBuffer, 0);

			return charBuffer;
		}

		public static void WriteString(this Stream stream, string value)
		{
			var bytes = value.ToCharArray().ToBytes().ToArray();
			stream.WriteBytes(bytes);
		}

		public static void WriteBytes(this Stream stream, byte[] bytes)
		{
			stream.Write(bytes, 0, bytes.Length);
		}
	}
}
