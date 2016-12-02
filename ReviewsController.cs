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
    public class ReviewsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Reviews
        [Authorize(Roles = "Customer")]
        public ActionResult Index()
        {
            return View(db.Reviews.ToList());
        }


        [Authorize(Roles = "Customer")]
        public ActionResult CreateSongReview(Int32 songID)
        {
            AppUser author = db.Users.Find(User.Identity.GetUserId());
            Song song = db.Songs.Find(songID);
            ViewBag.SongName = song.SongName;
            ViewBag.ArtistName = song.SongArtistString;

            if(author.MusicOwned.Contains(song))
            {
                foreach(SongReview sr in author.SongReviews)
                {
                    if (sr.ReviewedSong.SongId == song.SongId)
                    {
                        return View("AccessDenied");
                    }
                }

                return View();
            }
            return View("AccessDenied");
        }

        [Authorize(Roles = "Customer")]
        public ActionResult CreateAlbumReview(Int32 albumID)
        {
            AppUser author = db.Users.Find(User.Identity.GetUserId());
            Album album = db.Albums.Find(albumID);
            ViewBag.AlbumName = album.AlbumName;
            ViewBag.ArtistName = album.AlbumArtistString;

            foreach(Song s in album.AlbumSongs)
            {
                if(!author.MusicOwned.Contains(s))
                {
                    return View("AccessDenied");
                }
            }

            foreach (AlbumReview ar in author.AlbumReviews)
            {
                if (ar.AlbumToBeReviewed.AlbumId == album.AlbumId)
                {
                    return View("AccessDenied");
                }
                else
                {
                    return View();
                }
            }

            return View("AccessDenied");
        }

        [Authorize(Roles = "Customer")]
        public ActionResult CreateArtistReview(Int32 artistID)
        {
            AppUser author = db.Users.Find(User.Identity.GetUserId());
            Artist artist = db.Artists.Find(artistID);
            ViewBag.ArtistName = artist.ArtistName;

            foreach(Song s in artist.ArtistSongs)
            {
                if(author.MusicOwned.Contains(s))
                {
                    return View();
                }
            }

            return View("AccessDenied");
        }


        // POST: Reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult CreateSongReview([Bind(Include = "SongReviewId, SongScore, SongReviewText, SongReviewAuthor")] SongReview review, Int32 songID)
        {
            Song song = db.Songs.Find(songID);
            AppUser author = db.Users.Find(User.Identity.GetUserId());
            review.ReviewedSong = song;
            review.SongReviewAuthor = author;

            if (ModelState.IsValid)
            {
                db.SongReviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Index", "Songs");
            }

            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult CreateAlbumReview([Bind(Include = "AlbumReviewId, AlbumScore, AlbumReviewText, AlbumReviewAuthor")] AlbumReview review, Int32 albumID)
        {
            Album album = db.Albums.Find(albumID);
            AppUser author = db.Users.Find(User.Identity.GetUserId());
            review.AlbumToBeReviewed = album;
            review.AlbumReviewAuthor = author;

            if (ModelState.IsValid)
            {
                db.AlbumReviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Index", "Albums");
            }

            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult CreateArtistReview([Bind(Include = "ArtistReviewId, ArtistScore, ArtistReviewText, ArtistReviewAuthor")] ArtistReview review, Int32 artistID)
        {
            Artist artist = db.Artists.Find(artistID);
            AppUser author = db.Users.Find(User.Identity.GetUserId());
            review.ReviewedArtist = artist;
            review.ArtistReviewAuthor = author;

            if (ModelState.IsValid)
            {
                db.ArtistReviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Index", "Artists");
            }

            return View(review);
        }

        [Authorize(Roles = "Customer")]
        public ActionResult SongReviewDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SongReview review = db.SongReviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        [Authorize(Roles = "Customer")]
        public ActionResult AlbumReviewDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlbumReview review = db.AlbumReviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        [Authorize(Roles = "Customer")]
        public ActionResult ArtistReviewDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArtistReview review = db.ArtistReviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }



        // GET: Reviews/Edit/5
        [Authorize(Roles = "Customer")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult Edit([Bind(Include = "ReviewId")] SongReview review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(review);
        }


        // GET: Reviews/Delete/5
        [Authorize(Roles = "Customer")]
        public ActionResult SongReviewDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SongReview review = db.SongReviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult SongReviewDeleteConfirmed(int id)
        {
            SongReview review = db.SongReviews.Find(id);
            db.SongReviews.Remove(review);
            db.SaveChanges();
            return RedirectToAction("Index", "Songs");
        }


        // GET: Reviews/Delete/5
        [Authorize(Roles = "Customer")]
        public ActionResult AlbumReviewDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlbumReview review = db.AlbumReviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult AlbumReviewDeleteConfirmed(int id)
        {
            AlbumReview review = db.AlbumReviews.Find(id);
            db.AlbumReviews.Remove(review);
            db.SaveChanges();
            return RedirectToAction("Index", "Albums");
        }


        // GET: Reviews/Delete/5
        [Authorize(Roles = "Customer")]
        public ActionResult ArtistReviewDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArtistReview review = db.ArtistReviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult ArtistReviewDeleteConfirmed(int id)
        {
            ArtistReview review = db.ArtistReviews.Find(id);
            db.ArtistReviews.Remove(review);
            db.SaveChanges();
            return RedirectToAction("Index", "Artists");
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
