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
    public class ArtistsController : Controller
    {

        public SelectList GetAllGenres()
        {
            var query = from g in db.Genres
                        orderby g.GenreName
                        select g;

            List<Genre> allGenres = query.ToList();
            SelectList allGenresList = new SelectList(allGenres, "GenreId", "GenreName");
            return allGenresList;
        }

        public MultiSelectList GetAllGenres(Artist artist)
        {
            var query = from g in db.Genres
                        orderby g.GenreName
                        select g;

            List<Genre> allGenres = query.ToList();
            List<Genre> SelectedGenres = new List<Genre>();

            foreach (Genre g in artist.ArtistGenres)
            {
                //used to be SelectedMembers.Add(m.Id);
                SelectedGenres.Add(g);
            }

            MultiSelectList allGenresList = new MultiSelectList(allGenres, "GenreId", "GenreName", SelectedGenres);
            return allGenresList;
        }

        private AppDbContext db = new AppDbContext();

        // GET: Artists
        public ActionResult Index()
        {
            return View(db.Artists.ToList());
        }

        // GET: Artists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = db.Artists.Find(id);
            if (artist == null)
            {
                return HttpNotFound();
            }
            AverageRating(artist);
            return View(artist);
        }

        // GET: Artists/Create
        public ActionResult Create()
        {
            ViewBag.SelectedGenres = GetAllGenres();
            return View();
        }

        // POST: Artists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ArtistId,ArtistName,ArtistRating")] Artist artist, Int32[] SelectedGenres)
        {
            if (SelectedGenres != null)
            {
                foreach (Int32 Id in SelectedGenres)
                {
                    Genre GenreToAdd = db.Genres.Find(Id);
                    artist.ArtistGenres.Add(GenreToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                db.Artists.Add(artist);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            AverageRating(artist);
            return View(artist);
        }

        // GET: Artists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = db.Artists.Find(id);
            if (artist == null)
            {
                return HttpNotFound();
            }
            AverageRating(artist);
            return View(artist);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ArtistId,ArtistName,ArtistRating")] Artist artist)
        {
            if (ModelState.IsValid)
            {
                db.Entry(artist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            AverageRating(artist);
            return View(artist);
        }

        // GET: Artists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = db.Artists.Find(id);
            if (artist == null)
            {
                return HttpNotFound();
            }
            return View(artist);
        }

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Artist artist = db.Artists.Find(id);
            db.Artists.Remove(artist);
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

        public void AverageRating(Artist artist)
        {
            var query = from r in db.ArtistReviews
                        orderby r.ReviewedArtist.ArtistName
                        select r;

            decimal total_rating = 0;
            decimal avg_rating = 0;
            decimal count = 0;

            foreach (ArtistReview r in artist.ArtistReviews)
            {
                total_rating += r.ArtistScore;
                count += 1;
            }

            if (count > 0)
            {
                avg_rating = total_rating / count;
            }

            artist.ArtistRating = avg_rating;
        }
    }
}