using Microsoft.Owin;
using Owin;
using SimpleInjector;

[assembly: OwinStartup(typeof(API.Startup))]
namespace API
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
            Container container = new Container();
            //Configure Simple Injector
            ConfigureSimpleInjector(app, container);
        }
    }
}