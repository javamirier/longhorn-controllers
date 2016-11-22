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
    public class PurchasesController : Controller
    {
        // TO DO - not 100% sure where else we might need to call CalculateSubtotal to update the property

        private AppDbContext db = new AppDbContext();

        // GET: Purchases
        public ActionResult Index()
        {
            return View(db.Purchases.ToList());
        }

        // GET: Purchases/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            return View(purchase);
        }

        // GET: Purchases/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Purchases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PurchaseId,Date,Subtotal,IsComplete")] Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                db.Purchases.Add(purchase);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(purchase);
        }

        // GET: Purchases/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }

            CalculateSubtotal(purchase);
            ViewBag.Taxes = CalculateTax(purchase);
            ViewBag.Total = CalculateTotal(purchase);

            return View(purchase);
        }

        // POST: Purchases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PurchaseId,Date,Subtotal,IsComplete")] Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchase).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            CalculateSubtotal(purchase);
            ViewBag.Taxes = CalculateTax(purchase);
            ViewBag.Total = CalculateTotal(purchase);

            return View(purchase);
        }

        // GET: Purchases/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            return View(purchase);
        }

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Purchase purchase = db.Purchases.Find(id);
            db.Purchases.Remove(purchase);
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

        public void CalculateSubtotal(Purchase purchase)
        {
            var query = from p in db.Promotions
                        orderby p.DiscountedSongs
                        select p;

            decimal sub = 0;
            List<Promotion> allPromotions = query.ToList();

            foreach(ItemDetail id in purchase.ItemDetails)
            {
                foreach(Promotion p in allPromotions)
                {
                    if(p.DiscountedSongs.Contains(id.Song))
                    {
                        sub += (100 - p.DiscountPercentage) / 100 * id.Song.SongPrice;
                    }

                    else
                    {
                        sub += id.Song.SongPrice;
                    }

                    sub = Math.Round(sub, 2);
                }
            }

            purchase.Subtotal = sub;         
        }

        public decimal CalculateTax(Purchase purchase)
        {
            CalculateSubtotal(purchase);
            decimal taxes = 0;
            decimal TaxRate = 0.0825m;

            taxes = purchase.Subtotal * TaxRate;

            return taxes;
        }

        public decimal CalculateTotal(Purchase purchase)
        {
            CalculateSubtotal(purchase);
            decimal total = 0;

            total = purchase.Subtotal + CalculateTax(purchase);

            return total;
        }
    }
}
