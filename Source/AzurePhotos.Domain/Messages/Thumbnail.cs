using System;

namespace AzurePhotos.Domain.Messages
{
	/// <summary>
	/// Message that is passed between queues that describe a Thumbnail
	/// </summary>
	[Serializable]
	public class Thumbnail
	{

		private const int DefaultWidth = 120;
		private const int DefaultHeight = 120;
		public Thumbnail()
		{
			Width = DefaultWidth;
			Height = DefaultHeight;
		}

		/// <summary>
		/// The name of the photo to create a thumbnail from 
		/// </summary>
		public string PhotoName { get; set; }

		/// <summary>
		/// The id of the photo that needs to be updated
		/// </summary>
		public int PhotoId { get; set; }

		/// <summary>
		/// The width for the thumbnail
		/// </summary>
		public int Width { get; set; }
		/// <summary>
		/// The height of the thumbnail
		/// </summary>
		public int Height { get; set; }
	}
}
