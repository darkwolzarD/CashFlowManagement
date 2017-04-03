using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CashFlowManagement.Startup))]
namespace CashFlowManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
