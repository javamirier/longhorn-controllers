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
    public class SongsController : Controller
    {
        //TO DO: figure out where else to call AverageRating() - possibly in gets and before search methods

        public SelectList GetAllAlbums()
        {
            var query = from a in db.Albums
                        orderby a.AlbumName
                        select a;

            List<Album> allAlbums = query.ToList();
            SelectList allAlbumsList = new SelectList(allAlbums, "AlbumId", "AlbumName");
            return allAlbumsList;
        }

        public SelectList GetAllArtists()
        {
            var query = from a in db.Artists
                        orderby a.ArtistName
                        select a;

            List<Artist> allArtists = query.ToList();
            SelectList allArtistsList = new SelectList(allArtists, "ArtistId", "ArtistName");
            return allArtistsList;
        }

        public SelectList GetAllGenres()
        {
            var query = from g in db.Genres
                        orderby g.GenreName
                        select g;

            List<Genre> allGenres = query.ToList();
            SelectList allGenresList = new SelectList(allGenres, "GenreId", "GenreName");
            return allGenresList;
        }

        public MultiSelectList GetAllArtists(Song song)
        {
            var query = from a in db.Artists
                         orderby a.ArtistName
                         select a;

            List<Artist> allArtists = query.ToList();
            List<Artist> SelectedArtists = new List<Artist>();

            foreach (Artist a in song.SongArtists)
            {
                //used to be SelectedMembers.Add(m.Id);
                SelectedArtists.Add(a);
            }

            MultiSelectList allArtistsList = new MultiSelectList(allArtists, "ArtistId", "ArtistName", SelectedArtists);
            return allArtistsList;
        }

        public MultiSelectList GetAllGenres(Song song)
        {
            var query = from g in db.Genres
                        orderby g.GenreName
                        select g;

            List<Genre> allGenres = query.ToList();
            List<Genre> SelectedGenres = new List<Genre>();

            foreach (Genre g in song.SongGenres)
            {
                //used to be SelectedMembers.Add(m.Id);
                SelectedGenres.Add(g);
            }

            MultiSelectList allGenresList = new MultiSelectList(allGenres, "GenreId", "GenreName", SelectedGenres);
            return allGenresList;
        }

        public decimal AverageRating(Song song)
        {
            var query = from r in db.SongReviews
                        orderby r.ReviewedSong.SongName
                        select r;

            decimal total_rating = 0;
            decimal avg_rating = 0;
            decimal count = 0;
            
            foreach(SongReview r in song.SongReviews)
            {
                total_rating += r.SongScore;
                count += 1;
            }

            if(count > 0)
            {
                avg_rating = total_rating / count;
            }
           
            return avg_rating;

        }


        private AppDbContext db = new AppDbContext();

        // GET: Songs
        public ActionResult Index()
        {
            return View(db.Songs.ToList());
        }

        // GET: Songs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }

            song.SongRating = AverageRating(song);
            return View(song);
        }

        // GET: Songs/Create
        public ActionResult Create()
        {
            ViewBag.SelectedArtists = GetAllArtists();
            ViewBag.SelectedGenres = GetAllGenres();
            ViewBag.AllAlbums = GetAllAlbums();
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SongId,SongName,SelectedArtists")] Song song, string[] SelectedArtists, string[] SelectedGenres, Int32 AlbumId)
        {
            if (ModelState.IsValid)
            {
                db.Songs.Add(song);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SelectedArtists = GetAllArtists(song);
            ViewBag.SelectedGenres = GetAllGenres();
            ViewBag.AllAlbums = GetAllAlbums();
            song.SongRating = AverageRating(song);

            return View(song);
        }

        // GET: Songs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SongId,SongName")] Song song)
        {
            if (ModelState.IsValid)
            {
                db.Entry(song).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(song);
        }

        // GET: Songs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Song song = db.Songs.Find(id);
            db.Songs.Remove(song);
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
