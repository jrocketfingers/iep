using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(iep_ecommerce.Startup))]
namespace iep_ecommerce
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

    }
}
