using ReadLater.Entities;
using ReadLater.Repository;
using ReadLater.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class CategoriesController : BaseApiController
    {
        private readonly ICategoryService categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public IHttpActionResult Get()
        {
            List<Category> categories = categoryService.GetCategories();
            return Ok(categories);
        }
    }
}
