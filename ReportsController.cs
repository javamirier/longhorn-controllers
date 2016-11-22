using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LonghornMusic.Models;

namespace LonghornMusic.Controllers
{
    public class ReportsController : Controller
    {
        // TO DO - create the views

        private AppDbContext db = new AppDbContext();

        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SongsReport()
        {
            ViewBag.AllSongs = GetSongsReport();
            return View("SongsReport");
        }

        public ActionResult AlbumsReport()
        {
            ViewBag.AllAlbums = GetAlbumsReport();
            return View("AlbumsReport");
        }

        public ActionResult ArtistsReport()
        {
            ViewBag.AllArtists = GetArtistsReport();
            return View("ArtistsReport");
        }


        public List<List<string>> GetSongsReport()
        {
            var query = from s in db.Songs
                        orderby s.SongName
                        select s;

            List<Song> allSongs = query.ToList();

            List<string> songDetail = new List<string>();
            List<List<string>> allSongDetails = new List<List<string>>();

            foreach (Song s in allSongs)
            {
                string songName = s.SongName;
                string songFreqStr = "";
                string songRevStr = "";

                Decimal freq = 0;
                Decimal sumPrice = 0;

                foreach (ItemDetail id in s.SongPurchaseDetails)
                {
                    if (s.SongId == id.Song.SongId)
                    {
                        freq += 1;
                        sumPrice += id.PurchasePrice;
                    }
                }

                sumPrice = Math.Round(sumPrice, 2);
                songFreqStr = freq.ToString();
                songRevStr = "$" + sumPrice.ToString();

                songDetail.Add(songName);
                songDetail.Add(songFreqStr);
                songDetail.Add(songRevStr);

                allSongDetails.Add(songDetail);
            }

            return allSongDetails;
        }


        public List<List<string>> GetAlbumsReport()
        {
            var query = from a in db.Albums
                        orderby a.AlbumName
                        select a;

            List<Album> allAlbums = query.ToList();

            List<string> albumDetail = new List<string>();
            List<List<string>> allAlbumDetails = new List<List<string>>();

            foreach (Album a in allAlbums)
            {
                string albumName = a.AlbumName;
                string albumFreqStr = "";
                string albumRevStr = "";

                Decimal freq = 0;
                Decimal sumPrice = 0;

                foreach (ItemDetail id in a.AlbumPurchaseDetails)
                {
                    if (a.AlbumId == id.Album.AlbumId)
                    {
                        freq += 1;
                        sumPrice += id.PurchasePrice;
                    }
                }

                sumPrice = Math.Round(sumPrice, 2);
                albumFreqStr = freq.ToString();
                albumRevStr = "$" + sumPrice.ToString();

                albumDetail.Add(albumName);
                albumDetail.Add(albumFreqStr);
                albumDetail.Add(albumRevStr);

                allAlbumDetails.Add(albumDetail);
            }

            return allAlbumDetails;
        }


        public List<List<string>> GetArtistsReport()
        {
            // TO DO - this shit
            List<List<string>> allArtistDetails = new List<List<string>>();
            return allArtistDetails;
        }

    }
}