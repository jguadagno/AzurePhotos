using System.Web.Mvc;
using AzurePhotos.Data;
using AzurePhotos.WebSite.Models;

namespace AzurePhotos.WebSite.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			var model = new RecentThumbnailsModel
			{
				FirstThumbnail = new Photo(),
				SecondThumbnail = new Photo(),
				ThirdThumbnail = new Photo()
			};

			#region With Business Logic
			var mostRecentThree = BusinessLogic.Photos.GetMostRecentPhotos();

			if (mostRecentThree == null)
			{
				return View(model);
			}

			if (mostRecentThree.Count >= 1 && mostRecentThree[0] != null)
			{
				model.FirstThumbnail = mostRecentThree[0];
			}
			if (mostRecentThree.Count >= 2 && mostRecentThree[1] != null)
			{
				model.SecondThumbnail = mostRecentThree[1];
			}
			if (mostRecentThree.Count >= 3 && mostRecentThree[2] != null)
			{
				model.ThirdThumbnail = mostRecentThree[2];
			}

			#endregion With Business Logic

			return View(model);
		}

		public ActionResult About()
		{
			return View();
		}

		public ActionResult Contact()
		{
			return View();
		}
	}
}