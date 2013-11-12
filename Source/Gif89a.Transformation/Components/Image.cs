namespace Gif89a.Components
{
	using System.Collections;
	using System.Collections.Generic;

	internal sealed class Image
	{
		public Image()
		{
			this.Header = string.Empty;
			this.GlobalColorIndexedTable = new Hashtable();
			this.CommentExtensions = new List<CommentExtension>();
			this.ApplictionExtensions = new List<ApplicationExtension>();
			this.PlainTextEntensions = new List<PlainTextExtension>();
			this.Frames = new List<ImageFrame>();
		}

		public short Width
		{
			get { return this.LogicalScreenDescriptor.LogicalScreenWidth; }
		}

		public short Height
		{
			get { return this.LogicalScreenDescriptor.LogicalScreenHeight; }
		}

		public string Header { get; set; }

		public byte[] GlobalColorTable { get; set; }

		public Hashtable GlobalColorIndexedTable { get; private set; }

		public List<CommentExtension> CommentExtensions { get; set; }

		public List<ApplicationExtension> ApplictionExtensions { get; set; }

		public List<PlainTextExtension> PlainTextEntensions { get; set; }

		public LogicalScreenDescriptor LogicalScreenDescriptor { get; set; }

		public List<ImageFrame> Frames { get; set; }
	}
}
