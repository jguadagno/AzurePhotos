using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AzurePhotos.BusinessLogic;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace AzurePhotos.ThumbnailCreator
{
	public class WorkerRole : RoleEntryPoint
	{
		private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

		public override void Run()
		{
			Trace.TraceInformation("ThumbnailCreator WorkerRole Started", "Information");

			var queue = CloudServices.Queue.GetQueue(Constants.Queues.ThumbnailCreation);

			while (true)
			{
				try
				{
					// Get Message
					var cloudMessage = queue.GetMessage();
					if (cloudMessage != null)
					{
						var message = CloudServices.ByteArraySerializer<Domain.Messages.Thumbnail>.Deserialize(cloudMessage.AsBytes);

						// Get Original Photo
						var originalPhoto = CloudServices.BlogStorage.GetStream(Constants.StorageContainers.PhotoGallery, message.PhotoName);
						originalPhoto.Seek(0, SeekOrigin.Begin);

						// Create Thumbnail
						var thumbnail = BusinessLogic.Image.CreateThumbnail(originalPhoto, message.Width, message.Height);
						// Commit to Thumbnail Blob
						var thumbnailLocation = CloudServices.BlogStorage.SendFileToBlob(Constants.StorageContainers.ThumbnailsGallery, thumbnail, message.PhotoName, "image/jpeg");

						// Save Thumbnail
						BusinessLogic.Photos.UpdatePhotoThumbnailLocation(message.PhotoId, thumbnailLocation);

						// Delete Message
						queue.DeleteMessage(cloudMessage);
					}
					else
					{
						Thread.Sleep(1000);
					}
				}
				catch (Exception exception)
				{
					System.Threading.Thread.Sleep(5000);
					Trace.TraceError("Exception while trying to process a thumbnail queue item. Message: {0}", exception.Message);
					throw;
				}

				Trace.TraceInformation("Waiting for 10 seconds", "Information");
				Thread.Sleep(10000);

			}
		}

		public override bool OnStart()
		{
			// Set the maximum number of concurrent connections
			ServicePointManager.DefaultConnectionLimit = 12;

			// For information on handling configuration changes
			// see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

			bool result = base.OnStart();

			Trace.TraceInformation("AzurePhotos.ThumbnailCreator has been started");

			return result;
		}

		public override void OnStop()
		{
			Trace.TraceInformation("AzurePhotos.ThumbnailCreator is stopping");

			this.cancellationTokenSource.Cancel();
			this.runCompleteEvent.WaitOne();

			base.OnStop();

			Trace.TraceInformation("AzurePhotos.ThumbnailCreator has stopped");
		}

		private async Task RunAsync(CancellationToken cancellationToken)
		{
			// TODO: Replace the following with your own logic.
			while (!cancellationToken.IsCancellationRequested)
			{
				Trace.TraceInformation("Working");
				await Task.Delay(1000);
			}
		}
	}
}
