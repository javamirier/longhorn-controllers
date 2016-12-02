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
            var query2 = from a in db.Artists
                        orderby a.ArtistName
                        select a;

            List<Artist> allArtists = query2.ToList();
            SelectList allArtistsList = new SelectList(allArtists, "ArtistId", "ArtistName");
            return allArtistsList;
        }

        public SelectList GetAllGenres()
        {
            var query3 = from g in db.Genres
                        orderby g.GenreName
                        select g;

            List<Genre> allGenres = query3.ToList();
            SelectList allGenresList = new SelectList(allGenres, "GenreId", "GenreName");
            return allGenresList;
        }

        public MultiSelectList GetAllArtists(Song song)
        {
            var query4 = from a in db.Artists
                         orderby a.ArtistName
                         select a;

            List<Artist> allArtists = query4.ToList();
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
            var query5 = from g in db.Genres
                        orderby g.GenreName
                        select g;

            List<Genre> allGenres = query5.ToList();
            List<Genre> SelectedGenres = new List<Genre>();

            foreach (Genre g in song.SongGenres)
            {
                //used to be SelectedMembers.Add(m.Id);
                SelectedGenres.Add(g);
            }

            MultiSelectList allGenresList = new MultiSelectList(allGenres, "GenreId", "GenreName", SelectedGenres);
            return allGenresList;
        }

        public MultiSelectList GetAllAlbums(Song song)
        {
            var query6 = from a in db.Albums
                        orderby a.AlbumName
                        select a;

            List<Album> allAlbums = query6.ToList();
            List<Album> SelectedAlbums = new List<Album>();

            foreach (Album a in song.SongAlbums)
            {
                //used to be SelectedMembers.Add(m.Id);
                SelectedAlbums.Add(a);
            }

            MultiSelectList allAlbumsList = new MultiSelectList(allAlbums, "AlbumId", "AlbumName", SelectedAlbums);
            return allAlbumsList;
        }

        public void AverageRating(Song song)
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
           
            song.SongRating = avg_rating;
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

            AverageRating(song);
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
        public ActionResult Create([Bind(Include = "SongId,SongName,SongPrice,SongArtists,SongGenres,SongAlbums")] Song song, Int32[] SelectedArtists, Int32[] SelectedGenres, Int32[] SelectedAlbums, string SongPrice)
        {
            //DONE: Fix method attaching; this only works with 1 album per song, not many 
            Decimal SongPriceDec = System.Convert.ToDecimal(SongPrice);
            song.SongPrice = SongPriceDec;
            foreach (Int32 Id in SelectedGenres)
            {
                Genre GenreToAdd = db.Genres.Find(Id);
                song.SongGenres.Add(GenreToAdd);
            }
            //DONE: Implement the GET/POST viewbag lines and view changes to make this appropriate 
            foreach (Int32 Id in SelectedArtists)
            {
                Artist ArtistToAdd = db.Artists.Find(Id);
                song.SongArtists.Add(ArtistToAdd);
            }
            if (SelectedAlbums != null)
            {
                foreach (Int32 Id in SelectedAlbums)
                {
                    Album AlbumToAdd = db.Albums.Find(Id);
                    song.SongAlbums.Add(AlbumToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                db.Songs.Add(song);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            AverageRating(song);
            ViewBag.SelectedArtists = GetAllArtists(song);
            ViewBag.SelectedGenres = GetAllGenres(song);
            ViewBag.AllAlbums = GetAllAlbums(song);

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
            AverageRating(song);
            ViewBag.AllArtists = GetAllArtists(song);
            ViewBag.AllAlbums = GetAllAlbums(song);
            ViewBag.AllGenres = GetAllGenres(song);
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SongId,SongName,SelectedArtists,SongPrice")] Song song, Int32[] SelectedArtists, Int32[] SelectedGenres, Int32[] SelectedAlbums, Decimal SongPrice)
        {
            if (ModelState.IsValid)
            {
                Song songToChange = db.Songs.Find(song.SongId);
                //DONE: Y u no work 
                if (SelectedArtists != null && SelectedArtists.Count() > 0)
                {
                    songToChange.SongArtists.Clear();
                    foreach (Int32 Id in SelectedArtists)
                    {
                        Artist artistToAdd = db.Artists.Find(Id);
                        songToChange.SongArtists.Add(artistToAdd);
                    }
                }
                if (SelectedGenres != null && SelectedGenres.Count() > 0)
                {
                    songToChange.SongGenres.Clear();
                    foreach (Int32 Id in SelectedGenres)
                    {
                        Genre genreToAdd = db.Genres.Find(Id);
                        songToChange.SongGenres.Add(genreToAdd);
                    }
                }
                if (SelectedAlbums != null && SelectedAlbums.Count() > 0)
                {
                    songToChange.SongAlbums.Clear();
                    foreach (Int32 Id in SelectedAlbums)
                    {
                        Album albumToAdd = db.Albums.Find(Id);
                        songToChange.SongAlbums.Add(albumToAdd);
                    }
                }

                songToChange.SongName = song.SongName;
                songToChange.SongPrice = song.SongPrice;

                db.Entry(songToChange).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            AverageRating(song);
            ViewBag.AllArtists = GetAllArtists(song);
            ViewBag.AllAlbums = GetAllAlbums(song);
            ViewBag.AllGenres = GetAllGenres(song);
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

        //Get rid of this once purchase controller is working 
        //PURCHASE A SONG
        [HttpPost, ActionName("AddToCart")]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(int? Id)
        {
            Song song = db.Songs.Find(Id);
            string uid = User.Identity.GetUserId();
            AppUser user = db.Users.Find(uid);

            ItemDetail ItemDetail = new ItemDetail();
            decimal PurchasePrice = song.SongPrice;
            ItemDetail.PurchasePrice = PurchasePrice;
            ItemDetail.Song = song;
            
            if (user.OrderHistory.Count == 0)
            {
                PurchaseUserDetail PurchaseUserDetail = new PurchaseUserDetail();
                PurchaseUserDetail.Customer = user;
                
                Purchase purchase = new Purchase();
                purchase.IsComplete = false;
                ItemDetail.Purchase = purchase;
                purchase.ItemDetails.Add(ItemDetail);
                PurchaseUserDetail.Purchase = purchase;
                purchase.PurchaseUserDetail = PurchaseUserDetail;                
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
                    ItemDetail.Purchase = Purchase;
                    Purchase.ItemDetails.Add(ItemDetail);
                    PurchaseUserDetailToAdd.Purchase = Purchase;
                    Purchase.PurchaseUserDetail = PurchaseUserDetailToAdd;
                    db.Purchases.Add(Purchase);
                    db.PurchaseUserDetails.Add(PurchaseUserDetailToAdd);

                }
            }
            db.ItemDetails.Add(ItemDetail);
            return View(song);
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
