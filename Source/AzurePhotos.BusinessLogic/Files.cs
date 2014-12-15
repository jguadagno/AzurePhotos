using System;

namespace AzurePhotos.BusinessLogic
{
	public static class Files
	{
		public static string GetContentType(string filename)
		{
			const string defaultContentType = "application/octet-stream";
			if (string.IsNullOrEmpty(filename))
			{
				return defaultContentType;
			}

			try
			{
				string ext = System.IO.Path.GetExtension(filename).ToLowerInvariant();
				Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
				if (key != null)
				{
					string contentType = key.GetValue("Content Type") as String;
					if (!String.IsNullOrEmpty(contentType))
					{
						return contentType;
					}
				}
			}
			catch
			{
			}
			return defaultContentType;
		}

		public static string GetFileExtension(string filename)
		{
			if (string.IsNullOrEmpty(filename))
			{
				return "";
			}

			try
			{
				return System.IO.Path.GetExtension(filename).ToLowerInvariant();
			}
			catch
			{
			}
			return "";
		}
	}
}
