namespace Gif89a.Transformation
{
	using System.Drawing;

	public interface IBitmapProcessor
	{
		Bitmap Process(Bitmap source, Color bgColor);
	}
}
