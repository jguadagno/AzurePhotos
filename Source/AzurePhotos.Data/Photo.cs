//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AzurePhotos.Data
{
	using System;
	using System.Collections.Generic;
	
	public partial class Photo
	{
		public int PhotoId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string ThumbnailUrl { get; set; }
		public string PhotoUrl { get; set; }
		public System.DateTime DateAdded { get; set; }
		public Nullable<System.DateTime> DateUpdated { get; set; }
	}
}
