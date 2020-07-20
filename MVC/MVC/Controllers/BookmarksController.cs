using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    [Authorize]
    public class BookmarksController : Controller
    {
        // GET: Bookmarks
        public ActionResult Index()
        {
            ViewBag.ApiUrl = ConfigurationManager.AppSettings["apiUrl"];
            return View();
        }
    }
}