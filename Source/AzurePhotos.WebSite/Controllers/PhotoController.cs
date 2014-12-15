using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AzurePhotos.Data;

namespace AzurePhotos.WebSite.Controllers
{
	public class PhotoController : Controller
	{
		private AzurePhotosEntities db = new AzurePhotosEntities();

		// GET: /Photo/
		public ActionResult Index()
		{
			return View(db.Photos.ToList());
		}

		// GET: /Photo/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Photo photo = db.Photos.Find(id);

			if (photo == null)
			{
				return HttpNotFound();
			}

			return View(photo);
		}

		// GET: /Photo/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: /Photo/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Photo photo)
		{
			if (ModelState.IsValid)
			{
				photo.DateAdded = DateTime.Now;
				db.Photos.Add(photo);
				db.SaveChanges();
			}

			return RedirectToAction("Index");
		}


		// GET: /Photo/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Photo photo = db.Photos.Find(id);
			if (photo == null)
			{
				return HttpNotFound();
			}
			return View(photo);
		}

		// POST: /Photo/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "PhotoId,Title,Description,ThumbnailUrl,PhotoUrl,DateAdded,DateUpdated")] Photo photo)
		{
			if (ModelState.IsValid)
			{
				db.Entry(photo).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(photo);
		}

		// GET: /Photo/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Photo photo = db.Photos.Find(id);
			if (photo == null)
			{
				return HttpNotFound();
			}
			return View(photo);
		}

		// POST: /Photo/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Photo photo = db.Photos.Find(id);
			db.Photos.Remove(photo);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}