using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LonghornMusic.Models;
//using LonghornMusic.Models.IdentityModels.AppDbContext;


namespace longhornmusic.Controllers
{
    public class SearchController : Controller
    {
        private AppDbContext db = new AppDbContext();
        // GET: Search
        public ActionResult SongsSearch(string SearchString)
        {
            List<Song> SelectedSongs = new List<Song>();
            List<Song> AllSongs = new List<Song>();

            if (SearchString == null || SearchString == "")
            {
                var allcount = db.Songs.Count();
                AllSongs = db.Songs.Where(c => c.SongName.Contains("")).ToList();
                string countstring = "Displaying " + allcount + " Records";
                ViewBag.countstring = countstring;
                return View(AllSongs.OrderBy(c => c.SongName).ThenBy(c => c.SongPrice).ToList());
            }
            else
            {
                SelectedSongs = db.Songs.Where(c => c.SongName.Contains(SearchString)).ToList();
                SelectedSongs.OrderBy(c => c.SongName).ThenBy(c => c.SongPrice);

                var viewcount = SelectedSongs.Count();
                var allcount = db.Songs.Count();

                string countstring = "Displaying " + viewcount + " of " + allcount + " Records";
                ViewBag.countstring = countstring;
                return View(SelectedSongs.OrderBy(c => c.SongName).ThenBy(c => c.SongPrice).ToList());
            }
            
        }

    }
}