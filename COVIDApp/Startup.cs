using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(COVIDApp.Startup))]
namespace COVIDApp
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
