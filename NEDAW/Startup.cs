using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NEDAW.Startup))]
namespace NEDAW
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
