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
            GlobalConfiguration.Configuration.UseSqlServerStorage("Server=thiennq\\SQLEXPRESS;Database=CashFlowManagement_V2; User Id=sa;Password=zxcvbnm;");

            app.UseHangfireDashboard();
            app.UseHangfireServer();
            RecurringJob.AddOrUpdate(() => AssetQueries.CreateCashFlowPerMonth(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => AssetQueries.BankDepositMaturity(), "0 10 * * *");
        }
    }
}
        