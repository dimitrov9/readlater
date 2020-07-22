using API.Models.Bookmarks;
using ReadLater.Entities;
using ReadLater.Repository;
using ReadLater.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace API.Controllers
{
    public class BookmarksController : BaseApiController
    {
        private readonly IBookmarkService bookmarkService;
        private readonly ICategoryService categoryService;

        public BookmarksController(
            IBookmarkService bookmarkService,
            ICategoryService categoryService
            )
        {
            this.bookmarkService = bookmarkService;
            this.categoryService = categoryService;
        }

        public IHttpActionResult Get()
        {
            List<Bookmark> bookmarks = bookmarkService.GetBookmarks(string.Empty);
            return Ok(bookmarks);
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] CreateBookmarkModel model)
        {
            if (model == null || !ModelState.IsValid) return BadRequest(ModelState);

            var bookmark = new Bookmark
            {
                URL = model.Url,
                ShortDescription = string.IsNullOrWhiteSpace(model.Description) ? model.Url : model.Description
            };

            try
            {
                bookmark = CategorizeBookmark(model, bookmark);
            }
            catch (ObjectNotFoundException)
            {
                return NotFound();
            }

            bookmark = bookmarkService.CreateBookmark(bookmark);

            return Ok(bookmark);
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var bookmark = bookmarkService.GetBookmark(id);

            if (bookmark == null) return NotFound();

            bookmarkService.DeleteBookmark(bookmark);

            return Ok();
        }

        [HttpPut]
        public IHttpActionResult Put(int id, [FromBody] CreateBookmarkModel model)
        {
            if (model == null || !ModelState.IsValid) return BadRequest(ModelState);

            var bookmark = bookmarkService.GetBookmark(id);

            if (bookmark == null) return NotFound();

            bookmark.ShortDescription = string.IsNullOrWhiteSpace(model.Description) ? model.Url : model.Description;
            bookmark.URL = model.Url;

            try
            {
                bookmark = CategorizeBookmark(model, bookmark);
            }
            catch (ObjectNotFoundException)
            {
                return NotFound();
            }
            
            bookmarkService.UpdateBookmark(bookmark);

            return Ok(bookmark);
        }

        private Bookmark CategorizeBookmark(CreateBookmarkModel model, Bookmark bookmark)
        {
            if (model.CategoryId.HasValue && model.CategoryId.Value != 0)
            {
                var category = categoryService.GetCategory(model.CategoryId.Value);
                bookmark.Category = category ?? throw new ObjectNotFoundException();
                bookmark.CategoryId = category.ID;
            }
            else if (!string.IsNullOrWhiteSpace(model.NewCategory))
            {
                var category = new Category { Name = model.NewCategory };
                category = categoryService.CreateCategory(category);
                category.ObjectState = ObjectState.Unchanged;
                bookmark.Category = category;
                bookmark.CategoryId = category.ID;
            }
            else
            {
                bookmark.Category = null;
                bookmark.CategoryId = null;
            }

            return bookmark;
        }
    }
}
