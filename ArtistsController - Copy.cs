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
        public SelectList GetAllAlbums()
        {
            var query = from a in db.Albums
                        orderby a.AlbumName
                        select a;

            List<Album> allAlbums = query.ToList();
            SelectList allAlbumsList = new SelectList(allAlbums, "AlbumId", "AlbumName");
            return allAlbumsList;
        }

        public MultiSelectList GetAllAlbums(Artist artist)
        {
            var query = from a in db.Albums
                        orderby a.AlbumName
                        select a;

            List<Album> allAlbums = query.ToList();
            List<Album> SelectedAlbums = new List<Album>();

            foreach (Album a in artist.ArtistAlbums)
            {
                //used to be SelectedMembers.Add(m.Id);
                SelectedAlbums.Add(a);
            }

            MultiSelectList allAlbumsList = new MultiSelectList(allAlbums, "AlbumId", "AlbumName", SelectedAlbums);
            return allAlbumsList;
        }
        public SelectList GetAllSongs()
        {
            var query = from s in db.Songs
                        orderby s.SongName
                        select s;

            List<Song> allSongs = query.ToList();
            SelectList allSongsList = new SelectList(allSongs, "SongId", "SongName");
            return allSongsList;
        }

        public MultiSelectList GetAllSongs(Artist artist)
        {
            var query = from s in db.Songs
                        orderby s.SongName
                        select s;

            List<Song> allSongs = query.ToList();
            List<Song> SelectedSongs = new List<Song>();

            foreach (Song s in artist.ArtistSongs)
            {
                //used to be SelectedMembers.Add(m.Id);
                SelectedSongs.Add(s);
            }

            MultiSelectList allSongsList = new MultiSelectList(allSongs, "SongId", "SongName", SelectedSongs);
            return allSongsList;
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
            ViewBag.SelectedAlbums = GetAllAlbums(artist);
            ViewBag.SelectedSongs = GetAllSongs(artist);
            ViewBag.SelectedGenres = GetAllGenres(artist);
            return View(artist);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ArtistId,ArtistName")] Artist artist, Int32[] SelectedGenres, Int32[] SelectedAlbums, Int32[] SelectedSongs)
        {
            if (ModelState.IsValid)
            {
                Artist artistToChange = db.Artists.Find(artist.ArtistId);
                if (SelectedGenres != null && SelectedGenres.Count() > 0)
                {
                    foreach (Int32 Id in SelectedGenres)
                    {
                        artistToChange.ArtistGenres.Clear();
                        Genre genreToAdd = db.Genres.Find(Id);
                        artistToChange.ArtistGenres.Add(genreToAdd);
                    }
                }
                if (SelectedSongs != null && SelectedSongs.Count() > 0)
                {
                    foreach (Int32 Id in SelectedSongs)
                    {
                        artistToChange.ArtistSongs.Clear();
                        Song songToAdd = db.Songs.Find(Id);
                        artistToChange.ArtistSongs.Add(songToAdd);
                    }
                }
                if (SelectedAlbums != null && SelectedAlbums.Count() > 0)
                {
                    foreach (Int32 Id in SelectedAlbums)
                    {
                        artistToChange.ArtistAlbums.Clear();
                        Album albumToAdd = db.Albums.Find(Id);
                        artistToChange.ArtistAlbums.Add(albumToAdd);
                    }
                }
                artistToChange.ArtistName = artist.ArtistName;

                db.Entry(artistToChange).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            AverageRating(artist);
            ViewBag.SelectedAlbums = GetAllAlbums(artist);
            ViewBag.SelectedSongs = GetAllSongs(artist);
            ViewBag.SelectedGenres = GetAllGenres(artist);
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