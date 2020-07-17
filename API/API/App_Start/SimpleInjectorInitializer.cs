
using Owin;
using ReadLater.Data;
using ReadLater.Repository;
using ReadLater.Services;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.WebApi;
using System.Web.Http;

namespace API
{
    public partial class Startup
    {
        public static void ConfigureSimpleInjector(IAppBuilder app, Container container)
        {
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            InitializeContainer(container);

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }
        private static void InitializeContainer(Container container)
        {
            // For instance:
            // container.Register<IUserRepository, SqlUserRepository>();

            container.Register<IDbContext, ReadLaterDataContext>(Lifestyle.Scoped);
            container.Register<IUnitOfWork, UnitOfWork>(Lifestyle.Scoped);

            //services

            container.Register<ICategoryService, CategoryService>(Lifestyle.Scoped);
            container.Register<IBookmarkService, BookmarkService>(Lifestyle.Scoped);
        }

    }
}