using ReadLater.Entities;
using ReadLater.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class BookmarksController : ApiController
    {
        private readonly IBookmarkService bookmarkService;

        public BookmarksController(IBookmarkService bookmarkService)
        {
            this.bookmarkService = bookmarkService;
        }

        public IHttpActionResult Get()
        {
            List<Bookmark> bookmarks = bookmarkService.GetBookmarks(string.Empty);
            return Ok(bookmarks);
        }
    }
}
