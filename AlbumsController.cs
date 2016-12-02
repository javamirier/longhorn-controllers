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
    public class AlbumsController : Controller
    {

        private AppDbContext db = new AppDbContext();

        // GET: Albums
        public ActionResult Index()
        {
            return View(db.Albums.ToList());
        }

        // GET: Albums/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            AverageRating(album.AlbumId);
            return View(album);
        }

        // GET: Albums/Create
        public ActionResult Create()
        {
            ViewBag.AllArtists = GetAllArtists();
            ViewBag.AllGenres = GetAllGenres();
            ViewBag.AllSongs = GetAllSongs();
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AlbumId,AlbumName,AlbumPrice,AlbumSongs,AlbumArtists,AlbumGenres")] Album album, int[] SelectedSongs, int[] SelectedArtists, int[] SelectedGenres, string AlbumPrice)
        {
            Decimal AlbumPriceDec = System.Convert.ToDecimal(AlbumPrice);
            album.AlbumPrice = AlbumPriceDec;
            foreach (int Id in SelectedGenres)
            {
                Genre GenreToAdd = db.Genres.Find(Id);
                album.AlbumGenres.Add(GenreToAdd);
            }
            foreach (int Id in SelectedArtists)
            {
                Artist ArtistToAdd = db.Artists.Find(Id);
                album.AlbumArtists.Add(ArtistToAdd);
            }
            foreach (int Id in SelectedSongs)
            {
                Song SongToAdd = db.Songs.Find(Id);
                album.AlbumSongs.Add(SongToAdd);
            }
            if (ModelState.IsValid)
            {
                db.Albums.Add(album);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            AverageRating(album.AlbumId);
            ViewBag.AllArtists = GetAllArtists(album);
            ViewBag.AllGenres = GetAllGenres(album);
            ViewBag.AllSongs = GetAllSongs(album);
            return View(album);
        }

        // GET: Albums/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            AverageRating(album.AlbumId);
            ViewBag.SelectedArtists = GetAllArtists(album);
            ViewBag.SelectedGenres = GetAllGenres(album);
            ViewBag.SelectedSongs = GetAllSongs(album);
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AlbumId,AlbumName,AlbumPrice")] Album album, Int32[] SelectedArtists, Int32[] SelectedSongs, Int32[] SelectedGenres, Decimal AlbumPrice)
        {
            if (ModelState.IsValid)
            {
                Album albumToChange = db.Albums.Find(album.AlbumId);
                if (SelectedArtists != null && SelectedArtists.Count() > 0)
                {
                    albumToChange.AlbumArtists.Clear();
                    foreach (Int32 Id in SelectedArtists)
                    {
                        Artist artistToAdd = db.Artists.Find(Id);
                        albumToChange.AlbumArtists.Add(artistToAdd);
                    }
                }
                if (SelectedGenres != null && SelectedGenres.Count() > 0)
                {
                    albumToChange.AlbumGenres.Clear();
                    foreach (Int32 Id in SelectedGenres)
                    {
                        Genre genreToAdd = db.Genres.Find(Id);
                        albumToChange.AlbumGenres.Add(genreToAdd);
                    }
                }
                if (SelectedSongs != null && SelectedSongs.Count() > 0)
                {
                    albumToChange.AlbumSongs.Clear();
                    foreach (Int32 Id in SelectedSongs)
                    {
                        Song songToAdd = db.Songs.Find(Id);
                        albumToChange.AlbumSongs.Add(songToAdd);
                    }
                }

                albumToChange.AlbumName = album.AlbumName;
                albumToChange.AlbumPrice = album.AlbumPrice;

                db.Entry(albumToChange).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            AverageRating(album.AlbumId);
            ViewBag.SelectedArtists = GetAllArtists(album);
            ViewBag.SelectedGenres = GetAllGenres(album);
            ViewBag.SelectedSongs = GetAllSongs(album);
            return View(album);
        }

        // GET: Albums/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Album album = db.Albums.Find(id);
            db.Albums.Remove(album);
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
        public MultiSelectList GetAllSongs()
        {
            var query = from s in db.Songs
                        orderby s.SongName
                        select s;

            List<Song> allSongs = query.ToList();
            SelectList allSongsList = new SelectList(allSongs, "SongId", "SongName");
            return allSongsList;
        }

        public MultiSelectList GetAllSongs(Album album)
        {
            var query = from s in db.Songs
                        orderby s.SongName
                        select s;

            List<Song> allSongs = query.ToList();
            List<Song> SelectedSongs = new List<Song>();

            foreach (Song s in album.AlbumSongs)
            {
                SelectedSongs.Add(s);
            }

            MultiSelectList allSongsList = new MultiSelectList(allSongs, "SongId", "SongName", SelectedSongs);
            return allSongsList;
        }

        public MultiSelectList GetAllArtists()
        {
            var query = from a in db.Artists
                        orderby a.ArtistName
                        select a;

            List<Artist> allArtists = query.ToList();
            SelectList allArtistsList = new SelectList(allArtists, "ArtistId", "ArtistName");
            return allArtistsList;
        }

        public MultiSelectList GetAllArtists(Album album)
        {
            var query = from a in db.Artists
                        orderby a.ArtistName
                        select a;

            List<Artist> allArtists = query.ToList();
            List<Artist> SelectedArtists = new List<Artist>();

            foreach (Artist a in album.AlbumArtists)
            {
                SelectedArtists.Add(a);
            }

            MultiSelectList allArtistsList = new MultiSelectList(allArtists, "ArtistId", "ArtistName", SelectedArtists);
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

        public MultiSelectList GetAllGenres(Album album)
        {
            var query = from g in db.Genres
                        orderby g.GenreName
                        select g;

            List<Genre> allGenres = query.ToList();
            List<Genre> SelectedGenres = new List<Genre>();

            foreach (Genre g in album.AlbumGenres)
            {
                SelectedGenres.Add(g);
            }

            MultiSelectList allGenresList = new MultiSelectList(allGenres, "GenreId", "GenreName", SelectedGenres);
            return allGenresList;
        }

        public void AverageRating(Int32 albumID)
        {
            Album album = db.Albums.Find(albumID);

            decimal total_rating = 0;
            decimal avg_rating = 0;
            decimal count = 0;

            foreach (AlbumReview r in album.AlbumReviews)
            {
                total_rating += r.AlbumScore;
                count += 1;
            }

            if (count > 0)
            {
                avg_rating = total_rating / count;
            }

            album.AlbumRating = avg_rating;
            db.Entry(album).State = EntityState.Modified;    
            db.SaveChanges();
        }
    }
}
