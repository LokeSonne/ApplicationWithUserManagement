using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SoniReports.Startup))]
namespace SoniReports
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
