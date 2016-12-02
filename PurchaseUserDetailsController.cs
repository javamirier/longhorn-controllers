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
    public class PurchaseUserDetailsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: PurchaseUserDetails
        public ActionResult Index()
        {
            return View(db.PurchaseUserDetails.ToList());
        }

        // GET: PurchaseUserDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseUserDetail purchaseUserDetail = db.PurchaseUserDetails.Find(id);
            if (purchaseUserDetail == null)
            {
                return HttpNotFound();
            }
            return View(purchaseUserDetail);
        }

        // GET: PurchaseUserDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PurchaseUserDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PurchaseUserDetailId,CreditCard")] PurchaseUserDetail purchaseUserDetail)
        {
            if (ModelState.IsValid)
            {
                db.PurchaseUserDetails.Add(purchaseUserDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(purchaseUserDetail);
        }

        // GET: PurchaseUserDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseUserDetail purchaseUserDetail = db.PurchaseUserDetails.Find(id);
            if (purchaseUserDetail == null)
            {
                return HttpNotFound();
            }
            return View(purchaseUserDetail);
        }

        // POST: PurchaseUserDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PurchaseUserDetailId,CreditCard")] PurchaseUserDetail purchaseUserDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchaseUserDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(purchaseUserDetail);
        }

        // GET: PurchaseUserDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseUserDetail purchaseUserDetail = db.PurchaseUserDetails.Find(id);
            if (purchaseUserDetail == null)
            {
                return HttpNotFound();
            }
            return View(purchaseUserDetail);
        }

        // POST: PurchaseUserDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PurchaseUserDetail purchaseUserDetail = db.PurchaseUserDetails.Find(id);
            db.PurchaseUserDetails.Remove(purchaseUserDetail);
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
