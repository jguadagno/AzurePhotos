namespace AzurePhotos.BusinessLogic
{
	public static class Constants
	{
		
		// For naming restrictions for Blob, Table and Queue
		// See: http://weblogs.asp.net/vblasberg/archive/2009/02/17/azure-details-and-limitations-blobs-tables-and-queues.aspx

		/// <summary>
		/// A list of the known storage containers
		/// </summary>
		public static class StorageContainers
		{
			/// <summary>
			/// The container for the photo
			/// </summary>
			public const string PhotoGallery = "photo-gallery";
			/// <summary>
			/// The container for the thumbnails
			/// </summary>
			public const string ThumbnailsGallery = "thumbnails";
		}

		/// <summary>
		/// A list of known queues
		/// </summary>
		public static class Queues
		{
			/// <summary>
			/// The queue for the thumbnail creation
			/// </summary>
			public const string ThumbnailCreation = "thumbnail-creation";
		}
		
	}
}
