using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using ReadLater.Entities;
using ReadLater.Repository;
using SimpleInjector;
using Z.EntityFramework.Plus;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using API.Handlers;

[assembly: OwinStartup(typeof(API.Startup))]
namespace API
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            ConfigureAuth(app);
            Container container = new Container();
            //Configure Simple Injector
            ConfigureSimpleInjector(app, container);

            HttpConfiguration configuration = new HttpConfiguration();
            HttpServer httpServer = new HttpServer(configuration);

            configuration.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());

            app.Use((context, next) =>
            {
                if (context.Request.User != null && context.Request.User.Identity != null && context.Request.User.Identity.IsAuthenticated)
                {
                    var currentUserId = context.Request.User.Identity.GetUserId();

                    if (!string.IsNullOrWhiteSpace(currentUserId))
                    {
                        container.GetInstance<IUnitOfWork>().SetCurrentUserId(currentUserId);
                        QueryFilterManager.Filter<IMustHaveUser>(q => q.Where(x => x.UserId == currentUserId));
                    }

                }

                return next.Invoke();
            }).UseWebApi(httpServer);
        }
    }
}