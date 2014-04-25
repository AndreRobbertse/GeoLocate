using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GeoLocate.Startup))]
namespace GeoLocate
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
