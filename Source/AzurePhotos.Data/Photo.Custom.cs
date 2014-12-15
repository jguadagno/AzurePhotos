using System;

namespace AzurePhotos.Data
{
	public partial class Photo
	{
		public DateTime ModifiedDate
		{
			get { return DateUpdated ?? DateAdded; }
		}

		public string PhotoBlobUrl { get; set; }
		public string ThumbnailBlobUrl { get; set; }

	}
}