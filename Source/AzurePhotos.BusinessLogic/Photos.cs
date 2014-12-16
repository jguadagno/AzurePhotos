using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AzurePhotos.Data;
using AzurePhotos.Domain.Messages;

namespace AzurePhotos.BusinessLogic
{
	public static class Photos
	{
		/// <summary>
		///	Returns a list of photos 
		/// </summary>
		/// <returns></returns>
		public static List<Photo> GetPhotos()
		{
			using (var db = new AzurePhotosEntities())
			{
				return db.Photos.ToList();
			}
		}

		/// <summary>
		/// Gets an individual photo
		/// </summary>
		/// <param name="photoId">The id of the photo to retrieve</param>
		/// <returns>A photo if found, otherwise null</returns>
		public static Photo GetPhoto(int photoId)
		{
			using (var db = new AzurePhotosEntities())
			{
				return db.Photos.FirstOrDefault(p => p.PhotoId == photoId);
			}
		}

		/// <summary>
		/// Updates the location of the photo's thumbnail
		/// </summary>
		/// <param name="photoId">The id of the photo to update</param>
		/// <param name="thumbnailLocation">The location of the thumbnail</param>
		public static void UpdatePhotoThumbnailLocation(int photoId, string thumbnailLocation)
		{
			using (var db = new AzurePhotosEntities())
			{
				var photo = db.Photos.FirstOrDefault(p => p.PhotoId == photoId);
				if (photo == null)
				{
					return;
				}
				photo.ThumbnailUrl = thumbnailLocation;
				photo.DateUpdated = DateTime.Now;
				db.SaveChanges();
			}
		}

		/// <summary>
		/// Updates the photo title and description
		/// </summary>
		/// <param name="photoId">The id of the photo to update</param>
		/// <param name="title">The title to use for the photo</param>
		/// <param name="description">The description to use for the photo</param>
		public static void UpdatePhoto(int photoId, string title, string description)
		{
			using (var db = new AzurePhotosEntities())
			{
				var photo = db.Photos.FirstOrDefault(p => p.PhotoId == photoId);
				if (photo == null)
				{
					return;
				}
				photo.Title = title;
				photo.Description = description;
				photo.DateUpdated = DateTime.Now;
				db.SaveChanges();
			}
		}

		/// <summary>
		/// Gets the most recent photos added to the repository
		/// </summary>
		/// <param name="number">The number of photos to retrieve. The default is 3</param>
		/// <returns>A list numbers of photos</returns>
		public static List<Photo> GetMostRecentPhotos(int number = 3)
		{
			using (var db = new AzurePhotosEntities())
			{
				var list = db.Photos.OrderByDescending(p => (p.DateUpdated ?? p.DateAdded)).Take(number).ToList();

				foreach (var photo in list)
				{
					photo.PhotoBlobUrl = GetBlobUrl(Constants.StorageContainers.PhotoGallery, photo.PhotoUrl);
				}

				return list;
			}
		}

		/// <summary>
		/// Adds a new photo
		/// </summary>
		/// <param name="title">The title of the photo</param>
		/// <param name="description">A description of the photo</param>
		/// <param name="filename">The filename of the photo</param>
		/// <param name="contentType">The content type of the file</param>
		/// <param name="stream">The stream to save for the photo</param>
		/// <returns></returns>
		public static int AddPhoto(string title, string description, string filename, string contentType, Stream stream)
		{

			var blobName = CloudServices.BlogStorage.GenerateUniqueFilename(filename);
			var blobLocation = CloudServices.BlogStorage.SendFileToBlob(Constants.StorageContainers.PhotoGallery,
				stream, blobName, contentType);

			using (var db = new AzurePhotosEntities())
			{
				var photo = new Photo
				{
					Title = title,
					Description = description,
					PhotoUrl = blobLocation,
					DateAdded = DateTime.Now
				};

				db.Photos.Add(photo);
				db.SaveChanges();

				var thumbnailMessage = new Thumbnail { PhotoName = blobLocation, PhotoId = photo.PhotoId };
				CloudServices.Queue.AddMessageToQueue(Constants.Queues.ThumbnailCreation, thumbnailMessage);

				return photo.PhotoId;
			}
		}

		/// <summary>
		/// Gets the url for the stored blob image
		/// </summary>
		/// <param name="containerName">The blob container where the image is stored</param>
		/// <param name="blobName">The name of the blob item</param>
		/// <returns></returns>
		public static string GetBlobUrl(string containerName, string blobName)
		{
			return CloudServices.BlogStorage.GetBlobUrl(containerName, blobName);
		}
	}
}