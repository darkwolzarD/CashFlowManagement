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
            GlobalConfiguration.Configuration.UseSqlServerStorage("Server=ctbinh-pc;Database=CashFlowManagement_V2;User Id=sa;Password=123456;");

            app.UseHangfireDashboard();
            app.UseHangfireServer();
            RecurringJob.AddOrUpdate(() => AssetQueries.CreateCashFlowPerMonth(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => AssetQueries.BankDepositMaturity(), "0 10 * * *");
        }
    }
}
        