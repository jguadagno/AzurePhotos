#Introduction



#Adding Blob Storage
Explain AzurePhotos.CloudServices/BlobStorage

## AzurePhotos.BusinessLogic
Add reference to AzurePhotos.BusinessLogic

###Photos.cs
Add method
```csharp
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
```

####AddPhoto
Add in the beginning of the method
```csharp
var blobName = CloudServices.BlogStorage.GenerateUniqueFilename(filename);
var blobLocation = CloudServices.BlogStorage.SendFileToBlob(Constants.StorageContainers.PhotoGallery,
    stream, blobName, contentType);
```

Change the `"TODO: Get From Blob Storage"` to `blobLocation`


####GetMostRecentPhotos
Add to the foreach loop
```csharp
photo.PhotoBlobUrl = GetBlobUrl(Constants.StorageContainers.PhotoGallery, photo.PhotoUrl);
```

## AzurePhotos.Website

##AzurePhotos.Website
###Controllers\PhotoController
####Details 
Before return View(photo) add
```html
photo.PhotoBlobUrl = BusinessLogic.Photos.GetBlobUrl(BusinessLogic.Constants.StorageContainers.PhotoGallery, photo.PhotoUrl);
```

####Create
Replace Create(Photo) with
```csharp
public ActionResult Create(Photo photo, HttpPostedFileBase imageFile)
{

    int photoId = BusinessLogic.Photos.AddPhoto(photo.Title, photo.Description, imageFile.FileName,
        imageFile.ContentType, imageFile.InputStream);

    return RedirectToAction("Details", new { id = photoId });
}
```

##Views\Photo\Create.cshtml

Replace lines 41-47 with 
```html
<div class="form-group">
    <label class="control-label col-sm-2">File</label>
	<div class="col-sm-10">
		<input type="file" name="imageFile" class="form-control" />
		@Html.ValidationMessageFor(model => model.PhotoUrl)
	</div>
</div>
```

## Run the application
* Show the Storage Explorer from Azure Server Explorer add in
* Make sure the Storage Emulator is running
* Visit Photos/Index
* Visit Photos/Create
* Save
* Visit Photos/Detail of the new item

## Demostrate the Azure Explorer
Nuedesic Storage Explorer: 
http://azurestorageexplorer.codeplex.com/

#Adding Queues
Add Queue.cs to AzurePhotos.CloudServices

##AzurePhotos.WorkerRole
* File | Add new project
* Select Visual C# | Cloud | Azure Cloud Service
* Name = AzurePhotos.WorkerRole
* Explain the tpes
* Choose Worker Role
* Name it "AzurePhotos.ThumbnailCreator"

##AzurePhotos.ThumbnailCreator

* Explain the 4 Methods: Run, OnStart, OnStop and RunAsync
* Expand Roles
* ALT + ENTER or Properties of AzurePhoto.ThumbnailCreator
..* Explain - Configuration
..* Explain - Setting
..* Add Configuration: `AzureBlobStorageConnectionString`, Value: `UseDevelopmentStorage=true`



### References
1. Add References to
..* AzurePhotos.BusinessLogic
..* AzurePhotos.CloudServices
..* AzurePhotos.Data
..* AzurePhotos.Domain
..* C:\Program Files\Microsoft SDKs\Azure\.NET SDK\v2.5\bin\runtimes\base\Microsoft.WindowsAzure.ServiceRuntime.dll
...* Microsoft.WindowsAzure.ServiceRuntime
2. Add NuGet Reference to EntityFramework
..* Update NuGet references for all
..* Remove from web.config

###app.config
```xml
<connectionStrings>
    <add name="AzurePhotosEntities"  connectionString="metadata=res://*/PhotoModel.csdl|res://*/PhotoModel.ssdl|res://*/PhotoModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=(LocalDb)\v11.0;attachdbfilename=C:\My\Presentations\AzurePhotos\Source\AzurePhotos.WebSite\App_Data\AzurePhotos.mdf;initial catalog=AzurePhotos;integrated security=True;multipleactiveresultsets=True;application name=EntityFramework&quot;" providerName="System.Data.EntityClient" />
</connectionStrings>
```

## AzurePhotos.BusinessLogic

####AddPhoto
Add after the `db.SaveChanges();`
```csharp
var thumbnailMessage = new Thumbnail { PhotoName = blobLocation, PhotoId = photo.PhotoId };
CloudServices.Queue.AddMessageToQueue(Constants.Queues.ThumbnailCreation, thumbnailMessage);
```

* Set the Startup projects to both the AzurePhotos.WorkerRole and AzurePhotos.Website
* Start the application, show the queue after you create a new image

##AzurePhotos.ThumbnailCreator
Add using for `using AzurePhotos.BusinessLogic;`
Add using for `using System.IO;`

Replace Run method with
```csharp
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
```

Run the application, show the queue with the message gone.
Show the photo's thumbnail has been updated.
Stop Application


## AzurePhotos.BusinessLogic
###Photos.cs
####GetMostRecentPhotos
Add to the foreach loop
```csharp
photo.ThumbnailBlobUrl = GetBlobUrl(Constants.StorageContainers.ThumbnailsGallery, photo.ThumbnailUrl);
```

##AzurePhotos.Website
##Controllers\PhotoController
Details before return View(photo) add
```csharp
photo.ThumbnailBlobUrl = BusinessLogic.Photos.GetBlobUrl(BusinessLogic.Constants.StorageContainers.ThumbnailsGallery, photo.ThumbnailUrl);
```

* Run the Application
* Show Home (Recent)
* Show Details of Photo.
* Add a new photo


Changing to Azure Tables
======


References
======


Blog Storage in .NET
http://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-blobs/

Configuration Settings for Storage Accounts:
http://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-blobs-20/#setup-connection-string

Access Blog:
http://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-blobs-20/#configure-access

Website Diagnostics:
http://azure.microsoft.com/en-us/documentation/articles/web-sites-enable-diagnostic-log/
http://azure.microsoft.com/en-us/documentation/articles/web-sites-dotnet-troubleshoot-visual-studio

Hotfix 2588507: https://support.microsoft.com/kb/2588507

Notes
=======
Update the AzurePhotosEntities
