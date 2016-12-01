using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LonghornMusic.Models;


namespace LonghornMusic.Controllers
{
    public class SearchController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public enum GreaterOrLess { GreaterThan, LessThan };


        public ActionResult Index()
        {
            return View();
        }

        // GET: Search
        public ActionResult SongsSearch(string NameSearchString, string ArtistSearchString, string AlbumSearchString, int[] SelectedGenres, string RatingString, GreaterOrLess GorL)
        {
            List<Song> SelectedSongs = new List<Song>();
            List<Song> AllSongs = new List<Song>();

            var query = from s in db.Songs
                        select s;
            AllSongs = query.ToList();

            Int32 allcount = AllSongs.Count();
            ViewBag.AllSongCount = allcount.ToString();

            if (NameSearchString != null && NameSearchString != "")
            {
                query = query.Where(s => s.SongName.Contains(NameSearchString));
            }
            
            if (ArtistSearchString != null && ArtistSearchString != "")
            {
                query = query.Where(s => s.SongArtists.Any(a => a.ArtistName.Contains(ArtistSearchString)));
            }
            
            if (AlbumSearchString != null && AlbumSearchString != "")
            {
                query = query.Where(s => s.SongAlbums.Any(a => a.AlbumName.Contains(AlbumSearchString)));
            }

            decimal Rating;
            if(RatingString != null && RatingString != "")
            {
                try
                {
                    Rating = Convert.ToDecimal(RatingString);

                    if (GorL == GreaterOrLess.GreaterThan)
                    {
                        query = query.Where(s => s.SongRating >= Rating);
                    }
                    else if (GorL == GreaterOrLess.LessThan)
                    {
                        query = query.Where(c => c.SongRating <= Rating);
                    }
                }

                catch
                {
                    Rating = 0;
                }
            }

            List<Song> SongsToAdd = new List<Song>();

            if (SelectedGenres != null)
            {
                foreach (int i in SelectedGenres)
                {
                    var query2 = from s in db.Songs
                                 select s;

                    query2 = query.Where(s => s.SongGenres.Any(g => g.GenreId == i));
                    SongsToAdd = query2.ToList();
                    
                    foreach(Song song in SongsToAdd)
                    {
                        SelectedSongs.Add(song);
                    }
                }
            }

            SelectedSongs = query.OrderBy(c => c.SongName).ThenBy(c => c.SongPrice).ToList();
            Int32 viewcount = SelectedSongs.Count();
            ViewBag.SongCount = viewcount.ToString();
            return View(SelectedSongs);

        }


        public ActionResult AlbumsSearch(string SearchString)
        {

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
    }
}