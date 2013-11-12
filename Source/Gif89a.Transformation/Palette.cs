namespace Gif89a
{
	internal static class Palette
	{
		public static Color32[] Get(byte[] table)
		{
			var result = new Color32[table.Length / 3];

			var tableIndex = 0;
			var resultIndex = 0;
			while (tableIndex < table.Length)
			{
				byte r = table[tableIndex++];
				byte g = table[tableIndex++];
				byte b = table[tableIndex++];

				result[resultIndex++] = new Color32(r, g, b);
			}

			return result;
		}
	}
}
