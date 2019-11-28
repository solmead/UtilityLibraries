using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestSite.Core.Asp.Startup))]
namespace TestSite.Core.Asp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
