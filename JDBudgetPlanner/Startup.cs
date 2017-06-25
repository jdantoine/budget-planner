using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JDBudgetPlanner.Startup))]
namespace JDBudgetPlanner
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
