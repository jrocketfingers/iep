using Microsoft.Owin;
using Owin;
using Hangfire;

[assembly: OwinStartupAttribute(typeof(iep_ecommerce.Startup))]
namespace iep_ecommerce
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            GlobalConfiguration.Configuration.UseSqlServerStorage("AzureConnection");

            app.UseHangfireServer();
            app.UseHangfireDashboard();
            app.MapSignalR();
        }

    }
}
