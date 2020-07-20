using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Owin;
using ReadLater.Entities;
using ReadLater.Repository;
using SimpleInjector;
using System.Linq;
using Z.EntityFramework.Plus;

[assembly: OwinStartupAttribute(typeof(MVC.Startup))]
namespace MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            Container container = new Container();
            //Configure Simple Injector
            ConfigureSimpleInjector(app, container);

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
            });
        }
    }
}
