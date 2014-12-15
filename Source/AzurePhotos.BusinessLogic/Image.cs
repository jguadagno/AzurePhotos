using System.Drawing;
using System.IO;

namespace AzurePhotos.BusinessLogic
{
	/// <summary>
	/// Methods for manipulating images.
	/// </summary>
	public static class Image
	{
		/// <summary>
		/// Creates a thumbnail based on a stream
		/// </summary>
		/// <param name="input">The image stream to create a thumbnail from</param>
		/// <param name="width">The width to create the thumbnail. The default is 128 pixels.</param>
		/// <param name="height">The height to create the thumbnail. The default is 128 pixels.</param>
		/// <returns>Returns a stream with the thumbnail</returns>
		public static Stream CreateThumbnail(Stream input, int width = 128, int height = 128)
		{
			Bitmap orig = new Bitmap(input);

			if (orig.Width > orig.Height)
			{
				height = width * orig.Height / orig.Width;
			}
			else
			{
				width = height * orig.Width / orig.Height;
			}

			Bitmap thumb = new Bitmap(width, height);
			using (Graphics graphic = Graphics.FromImage(thumb))
			{
				graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

				graphic.DrawImage(orig, 0, 0, width, height);
				MemoryStream memoryStream = new MemoryStream();
				thumb.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

				memoryStream.Seek(0, SeekOrigin.Begin);
				return memoryStream;
			}
		}
	}
}
