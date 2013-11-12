namespace Gif89a.Extensions
{
	using System.Collections.Generic;
	using System.Linq;

	internal static class EnumerableExtensions
	{
		public static IEnumerable<byte> ToBytes(this IEnumerable<char> source)
		{
			return source.Select(c => (byte) c);
		}
	}
}
