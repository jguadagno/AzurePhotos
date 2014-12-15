#Introduction



#Adding Blob Storage
Explain AzurePhotos.CloudServices/BlobStorage

## AzurePhotos.BusinessLogic
Add reference to AzurePhotos.BusinessLogic

###Photos.cs
Add method
```cs
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
```cs
var blobName = CloudServices.BlogStorage.GenerateUniqueFilename(filename);
var blobLocation = CloudServices.BlogStorage.SendFileToBlob(Constants.StorageContainers.PhotoGallery,
    stream, blobName, contentType);
```

Change the `"TODO: Get From Blob Storage"` to `blobLocation`


####GetMostRecentPhotos
Add to the foreach loop
```cs
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
```cs
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
* Visit Photos/Index
* Visit Photos/Create
* Save
* Visit Photos/Detail of the new item

## Demostrate the Azure Explorer
Nuedesic Storage Explorer: 
http://azurestorageexplorer.codeplex.com/

#Adding Queues

##AzurePhotos.WorkerRole


##AzurePhotos.ThumbnailCreator
###app.config
```xml
<connectionStrings>
		<add name="AzurePhotosEntities" connectionString="metadata=res://*/PhotoModel.csdl|res://*/PhotoModel.ssdl|res://*/PhotoModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=(LocalDb)\v11.0;attachdbfilename=C:\My\Presentations\AzurePhotos\Source\AzurePhotos.WebSite\App_Data\AzurePhotos.mdf;initial catalog=AzurePhotos;integrated security=True;multipleactiveresultsets=True;application name=EntityFramework&quot;" providerName="System.Data.EntityClient" />
	</connectionStrings>
```

## AzurePhotos.BusinessLogic
###Photos.cs
####GetMostRecentPhotos
Add to the foreach loop
```cs
photo.ThumbnailBlobUrl = GetBlobUrl(Constants.StorageContainers.ThumbnailsGallery, photo.ThumbnailUrl);
```

####AddPhoto
Add after the `db.SaveChanges();`
```cs
var thumbnailMessage = new Thumbnail { PhotoName = blobLocation, PhotoId = photo.PhotoId };
CloudServices.Queue.AddMessageToQueue(Constants.Queues.ThumbnailCreation, thumbnailMessage);
```

##AzurePhotos.Website
##Controllers\PhotoController
Details before return View(photo) add
```html
photo.ThumbnailBlobUrl = BusinessLogic.Photos.GetBlobUrl(BusinessLogic.Constants.StorageContainers.ThumbnailsGallery, photo.ThumbnailUrl);
```


Changing to Azure Tables
======

Add Package
Add Reference to Windows.Azure.Runtime



Thumbnails
Add new project AzurePhotos.WorkerRole
Choose WorkerRole
Name it AzurePhotos.ThumbnailCreator

Add reference to AzurePhotos.BusinessLogic, AzurePhotos.CloudServices, AzurePhotos.Domain
Goto AzurePhotos.WorkerRole / Roles, View Properties on Roles/AzurePhotos.TumbnailCreator
Add NuGet EF
Add ConnectionString 'AzureBlobStorageConnectionString'
Local = UseDevelopmentStorage=true
Add to the App.config the AzurePhotoEntities

Show AzurePhotos.WorkerRole properties

Web Application Changes
Uncomment out BusinessLogic AddPhoto




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


Notes
=======
Update the AzurePhotosEntities
