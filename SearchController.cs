using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LonghornMusic.Models;


namespace LonghornMusic.Controllers
{
    public enum GreaterOrLess { GreaterThan, LessThan };

    public class SearchController : Controller
    {

        private AppDbContext db = new AppDbContext();

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }


        // POST: Song Search
        [AllowAnonymous]
        public ActionResult SongsSearch(string NameSearchString, string ArtistSearchString, string AlbumSearchString, int[] SelectedGenres, string RatingString, GreaterOrLess? GorL)
        {
            ViewBag.AllGenres = GetAllGenres();
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
            if (RatingString != null && RatingString != "")
            {
                try
                {
                    Rating = Convert.ToDecimal(RatingString);
                    if (Rating > 5 || Rating < 1)
                    {
                        ViewBag.ErrorMsg = ViewBag.ErrorMsg + "RATING IS OUT OF BOUND - IT HAS NOT BEEN USED TO REFINE THIS SEARCH";
                    }

                    else
                    {
                        if (GorL == GreaterOrLess.GreaterThan)
                        {
                            query = query.Where(s => s.SongRating >= Rating);
                        }
                        else if (GorL == GreaterOrLess.LessThan)
                        {
                            query = query.Where(c => c.SongRating <= Rating);
                        }
                    }
                }

                catch
                {
                    ViewBag.ErrorMsg = ViewBag.ErrorMsg + "RATING HAS NON-NUMERIC CHARACTERS - TRY AGAIN";
                    Rating = 0;
                }
            }

            List<Song> SongsToAdd = new List<Song>();

            if (SelectedGenres != null)
            {
                foreach (int i in SelectedGenres)
                {
                    var query2 = from s in db.Songs select s;
                    query2 = query.Where(s => s.SongGenres.Any(g => g.GenreId == i));
                    SongsToAdd = query2.OrderBy(c => c.SongName).ThenBy(c => c.SongPrice).ThenBy(c => c.SongRating).ToList();

                    foreach (Song song in SongsToAdd)
                    {
                        if (!SelectedSongs.Contains(song))
                        {
                            SelectedSongs.Add(song);
                        }
                    }
                }
            }

            else
            {
                SelectedSongs = query.ToList();
            }

            Int32 viewcount = SelectedSongs.Count();
            ViewBag.SongCount = viewcount.ToString();
            return View(SelectedSongs);
        }



        // POST: Album Search
        [AllowAnonymous]
        public ActionResult AlbumsSearch(string NameSearchString, string ArtistSearchString, int[] SelectedGenres, string RatingString, GreaterOrLess? GorL)
        {
            ViewBag.AllGenres = GetAllGenres();
            List<Album> SelectedAlbums = new List<Album>();
            List<Album> AllAlbums = new List<Album>();

            var query = from a in db.Albums
                        select a;
            AllAlbums = query.ToList();

            Int32 allcount = AllAlbums.Count();
            ViewBag.AllAlbumCount = allcount.ToString();

            if (NameSearchString != null && NameSearchString != "")
            {
                query = query.Where(a => a.AlbumName.Contains(NameSearchString));
            }

            if (ArtistSearchString != null && ArtistSearchString != "")
            {
                query = query.Where(a => a.AlbumArtists.Any(x => x.ArtistName.Contains(ArtistSearchString)));
            }

            decimal Rating;
            if (RatingString != null && RatingString != "")
            {
                try
                {
                    Rating = Convert.ToDecimal(RatingString);
                    if (Rating > 5 || Rating < 1)
                    {
                        ViewBag.ErrorMsg = ViewBag.ErrorMsg + "RATING IS OUT OF BOUND - IT HAS NOT BEEN USED TO REFINE THIS SEARCH";
                    }

                    else
                    {
                        if (GorL == GreaterOrLess.GreaterThan)
                        {
                            query = query.Where(s => s.AlbumRating >= Rating);
                        }
                        else if (GorL == GreaterOrLess.LessThan)
                        {
                            query = query.Where(c => c.AlbumRating <= Rating);
                        }
                    }
                }

                catch
                {
                    ViewBag.ErrorMsg = ViewBag.ErrorMsg + "RATING HAS NON-NUMERIC CHARACTERS - TRY AGAIN";
                    Rating = 0;
                }
            }

            List<Album> AlbumsToAdd = new List<Album>();

            if (SelectedGenres != null)
            {
                foreach (int i in SelectedGenres)
                {
                    var query2 = from a in db.Albums select a;
                    query2 = query.Where(s => s.AlbumGenres.Any(g => g.GenreId == i));
                    AlbumsToAdd = query2.OrderBy(a => a.AlbumName).ThenBy(a => a.AlbumPrice).ThenBy(a => a.AlbumRating).ToList();

                    foreach (Album album in AlbumsToAdd)
                    {
                        if(!SelectedAlbums.Contains(album))
                        {
                            SelectedAlbums.Add(album);
                        }
                    }
                }
            }

            else
            {
                SelectedAlbums = query.ToList();
            }

            Int32 viewcount = SelectedAlbums.Count();
            ViewBag.AlbumCount = viewcount.ToString();
            return View(SelectedAlbums);
        }



        // POST: Artist Search
        [AllowAnonymous]
        public ActionResult ArtistsSearch(string NameSearchString, int[] SelectedGenres, string RatingString, GreaterOrLess? GorL)
        {
            ViewBag.AllGenres = GetAllGenres();
            List<Artist> SelectedArtists = new List<Artist>();
            List<Artist> AllArtists = new List<Artist>();

            var query = from a in db.Artists
                        select a;
            AllArtists = query.ToList();

            Int32 allcount = AllArtists.Count();
            ViewBag.AllArtistCount = allcount.ToString();

            if (NameSearchString != null && NameSearchString != "")
            {
                query = query.Where(a => a.ArtistName.Contains(NameSearchString));
            }

            decimal Rating;
            if (RatingString != null && RatingString != "")
            {
                try
                {
                    Rating = Convert.ToDecimal(RatingString);
                    if (Rating > 5 || Rating < 1)
                    {
                        ViewBag.ErrorMsg = ViewBag.ErrorMsg + "RATING IS OUT OF BOUND - IT HAS NOT BEEN USED TO REFINE THIS SEARCH";
                    }

                    else
                    {
                        if (GorL == GreaterOrLess.GreaterThan)
                        {
                            query = query.Where(s => s.ArtistRating >= Rating);
                        }
                        else if (GorL == GreaterOrLess.LessThan)
                        {
                            query = query.Where(c => c.ArtistRating <= Rating);
                        }
                    }
                }

                catch
                {
                    ViewBag.ErrorMsg = ViewBag.ErrorMsg + "RATING HAS NON-NUMERIC CHARACTERS - TRY AGAIN";
                    Rating = 0;
                }
            }

            List<Artist> ArtistsToAdd = new List<Artist>();

            if (SelectedGenres != null)
            {
                foreach (int i in SelectedGenres)
                {
                    var query2 = from a in db.Artists select a;
                    query2 = query.Where(s => s.ArtistGenres.Any(g => g.GenreId == i));
                    ArtistsToAdd = query2.OrderBy(a => a.ArtistName).ThenBy(a => a.ArtistRating).ToList();

                    foreach (Artist artist in ArtistsToAdd)
                    {
                        if (!SelectedArtists.Contains(artist))
                        {
                            SelectedArtists.Add(artist);
                        }
                    }
                }
            }

            else
            {
                SelectedArtists = query.ToList();
            }

            Int32 viewcount = SelectedArtists.Count();
            ViewBag.ArtistCount = viewcount.ToString();
            return View(SelectedArtists);
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