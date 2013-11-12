namespace IntegrationTests
{
	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.IO;
	using System.Text;
	using Gif89a.Transformation;

	internal static class Program
	{
		private const string SourceGifsDir = "Assets";
		private const string DestGifsDir = "Output";
		private const string BrowserPath = @"C:\Program Files\Internet Explorer\iexplore.exe";

		private static void Main(string[] args)
		{
			Setup();

			var html = new StringBuilder();

			var sourceGifs = Directory.GetFiles(SourceGifsDir, "*.gif");
			foreach (var sourceGif in sourceGifs)
			{
				html.AppendFormat("<img src=\"{0}\" />", sourceGif);
				html.AppendFormat("<img src=\"{0}\" />", Transform(sourceGif, (source, dest) => DefaultGifTransformer.ResizeByWidth(source, dest, 100)));
				html.AppendFormat("<img src=\"{0}\" />", Transform(sourceGif, (source, dest) => DefaultGifTransformer.Crop(source, dest, new Rectangle(50, 50, 200, 200))));

				Console.WriteLine("{0} processed", sourceGif);
			}

			OpenInBrowser(html);
		}

		private static void OpenInBrowser(StringBuilder html)
		{
			var htmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "result.html");
			File.WriteAllText(htmlFilePath, html.ToString());

			Process.Start(BrowserPath, string.Format("file:///{0}", htmlFilePath));
		}

		private static string Transform(string file, Action<string, string> transformer)
		{
			var destFileName = string.Format("{0}{1}", Guid.NewGuid(), Path.GetExtension(file));
			var destFilePath = Path.Combine(DestGifsDir, destFileName);

			transformer(file, destFilePath);

			return destFilePath;
		}

		private static void Setup()
		{
			Directory.CreateDirectory(DestGifsDir);
			CleanDestDir();
		}

		private static void CleanDestDir()
		{
			var oldGifs = Directory.GetFiles(DestGifsDir, "*.gif");
			Array.ForEach(oldGifs, File.Delete);
		}
	}
}
