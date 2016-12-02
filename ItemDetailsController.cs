using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LonghornMusic.Models;

namespace LonghornMusic.Controllers
{
    public class ItemDetailsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: ItemDetails
        public ActionResult Index()
        {
            return View(db.ItemDetails.ToList());
        }

        // GET: ItemDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemDetail itemDetail = db.ItemDetails.Find(id);
            if (itemDetail == null)
            {
                return HttpNotFound();
            }
            return View(itemDetail);
        }

        // GET: ItemDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ItemDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemDetailId,PurchasePrice")] ItemDetail itemDetail)
        {
            if (ModelState.IsValid)
            {
                db.ItemDetails.Add(itemDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(itemDetail);
        }

        // GET: ItemDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemDetail itemDetail = db.ItemDetails.Find(id);
            if (itemDetail == null)
            {
                return HttpNotFound();
            }
            return View(itemDetail);
        }

        // POST: ItemDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemDetailId,PurchasePrice")] ItemDetail itemDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itemDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(itemDetail);
        }

        // GET: ItemDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemDetail itemDetail = db.ItemDetails.Find(id);
            if (itemDetail == null)
            {
                return HttpNotFound();
            }
            return View(itemDetail);
        }

        // POST: ItemDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ItemDetail itemDetail = db.ItemDetails.Find(id);
            db.ItemDetails.Remove(itemDetail);
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
