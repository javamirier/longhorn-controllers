using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
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

        //PURCHASE SONG
        public ActionResult AddSongToCart(int SongId)
        {
            Song song = db.Songs.Find(SongId);
            string uid = User.Identity.GetUserId();
            AppUser user = db.Users.Find(uid);

            ItemDetail detail = new ItemDetail();
            decimal PurchasePrice = song.SongPrice;
            detail.PurchasePrice = PurchasePrice;
            detail.Song = song;

            if (user.OrderHistory.Count == 0 || user.OrderHistory == null)
            {
                PurchaseUserDetail PurchaseUserDetail = new PurchaseUserDetail();
                PurchaseUserDetail.Customer = user;
                user.OrderHistory.Add(PurchaseUserDetail);

                Purchase p = new Purchase();
                p.IsComplete = false;
                p.Date = DateTime.Today;
                detail.Purchase = p;
                p.ItemDetails.Add(detail);
                PurchaseUserDetail.Purchase = p;
                p.PurchaseUserDetail.Add(PurchaseUserDetail);
            }
            else
            {
                PurchaseUserDetail PurchaseUserDetail = user.OrderHistory.Last();
                if (PurchaseUserDetail.Purchase.IsComplete == false)
                {
                    PurchaseUserDetail.Purchase.ItemDetails.Add(detail);
                    db.Entry(PurchaseUserDetail).State = EntityState.Modified;
                }
                else
                {
                    PurchaseUserDetail PurchaseUserDetailToAdd = new PurchaseUserDetail();
                    PurchaseUserDetailToAdd.Customer = user;
                    user.OrderHistory.Add(PurchaseUserDetailToAdd);

                    Purchase p = new Purchase();
                    p.IsComplete = false;
                    p.Date = DateTime.Today;
                    detail.Purchase = p;
                    p.ItemDetails.Add(detail);
                    PurchaseUserDetailToAdd.Purchase = p;
                    p.PurchaseUserDetail.Add(PurchaseUserDetailToAdd);
                    //db.Purchases.Add(p);
                    db.PurchaseUserDetails.Add(PurchaseUserDetailToAdd);

                }
            }
            db.Entry(user).State = EntityState.Modified;
            //db.ItemDetails.Add(detail);
            db.SaveChanges();
            return RedirectToAction("Index", "Songs");
        }

        //PURCHASE ALBUM
        public ActionResult AddAlbumToCart(int? AlbumId)
        {
            Song song = db.Songs.Find(AlbumId);
            string uid = User.Identity.GetUserId();
            AppUser user = db.Users.Find(uid);

            ItemDetail ItemDetail = new ItemDetail();
            decimal PurchasePrice = song.SongPrice;
            ItemDetail.PurchasePrice = PurchasePrice;
            ItemDetail.Song = song;

            if (user.OrderHistory.Count == 0 || user.OrderHistory == null)
            {
                PurchaseUserDetail PurchaseUserDetail = new PurchaseUserDetail();
                PurchaseUserDetail.Customer = user;

                Purchase purchase = new Purchase();
                purchase.IsComplete = false;
                purchase.Date = DateTime.Today;
                ItemDetail.Purchase = purchase;
                purchase.ItemDetails.Add(ItemDetail);
                PurchaseUserDetail.Purchase = purchase;
                purchase.PurchaseUserDetail.Add(PurchaseUserDetail);
            }
            else
            {
                PurchaseUserDetail PurchaseUserDetail = user.OrderHistory[user.OrderHistory.Count - 1];
                if (PurchaseUserDetail.Purchase.IsComplete == false)
                {
                    PurchaseUserDetail.Purchase.ItemDetails.Add(ItemDetail);
                }
                else
                {
                    PurchaseUserDetail PurchaseUserDetailToAdd = new PurchaseUserDetail();
                    PurchaseUserDetailToAdd.Customer = user;

                    Purchase Purchase = new Purchase();
                    Purchase.IsComplete = false;
                    Purchase.Date = DateTime.Today;
                    ItemDetail.Purchase = Purchase;
                    Purchase.ItemDetails.Add(ItemDetail);
                    PurchaseUserDetailToAdd.Purchase = Purchase;
                    Purchase.PurchaseUserDetail.Add(PurchaseUserDetailToAdd);
                    db.Purchases.Add(Purchase);
                    db.PurchaseUserDetails.Add(PurchaseUserDetailToAdd);

                }
            }
            db.ItemDetails.Add(ItemDetail);
            //Error here is most likely because of [Required] lines in models for values that should be null for now 
            db.SaveChanges();
            return RedirectToAction("Index", "Albums");
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
   
            List<Promotion> allPromotions = query.ToList();
            decimal sub = 0;

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
                }
            }

            sub = Math.Round(sub, 2);
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
