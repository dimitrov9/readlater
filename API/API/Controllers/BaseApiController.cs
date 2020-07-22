using ReadLater.Data;
using ReadLater.Entities;
using ReadLater.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using ReadLater.Services;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using EntityFramework.DynamicFilters;
using Z.EntityFramework.Plus;
using API.Filters;

namespace API.Controllers
{
    [Authorize]
    [LoggingFilter]
    public class BaseApiController : ApiController
    {
    }
}