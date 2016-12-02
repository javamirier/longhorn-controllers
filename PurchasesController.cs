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
        [Authorize(Roles = "Customer")]
        public ActionResult Index()
        {
            return View(db.Purchases.ToList());
        }

        // GET: Purchases/Details/5
        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Customer")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Purchases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Customer")]
        public ActionResult DeleteConfirmed(int id)
        {
            Purchase purchase = db.Purchases.Find(id);
            db.Purchases.Remove(purchase);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //PURCHASE SONG
        [Authorize(Roles = "Customer")]
        public ActionResult AddSongToCart(int SongId)
        {
            Song song = db.Songs.Find(SongId);
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            if(user.MusicOwned.Contains(song))
            {
                user.MusicOwned.Add(song);
            }         

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "Songs");
        }


        //PURCHASE ALBUM
        [Authorize(Roles = "Customer")]
        public ActionResult AddAlbumToCart(int? AlbumId)
        {
            Album album = db.Albums.Find(AlbumId);
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            foreach (Song song in album.AlbumSongs)
            {
                Song songToAdd = db.Songs.Find(song.SongId);

                if(!user.MusicOwned.Contains(song))
                {
                    user.MusicOwned.Add(songToAdd);
                }
            }

            db.Entry(user).State = EntityState.Modified;
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

            foreach (ItemDetail id in purchase.ItemDetails)
            {
                foreach (Promotion p in allPromotions)
                {
                    if (p.DiscountedSongs.Contains(id.Song))
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