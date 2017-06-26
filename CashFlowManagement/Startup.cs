using CashFlowManagement.Queries;
using Hangfire;
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
            GlobalConfiguration.Configuration.UseSqlServerStorage("Server=127.0.0.1;Database=giatrico_cfmgmt;User Id=giatrico_user;Password=Zxcvbnm1@;");

            app.UseHangfireDashboard();
            app.UseHangfireServer();
            RecurringJob.AddOrUpdate(() => AssetQueries.CreateCashFlowPerMonth(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => AssetQueries.BankDepositMaturity(), "0 10 * * *");
        }
    }
}
        