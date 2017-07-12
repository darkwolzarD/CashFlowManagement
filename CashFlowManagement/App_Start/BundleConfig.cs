using System.Web;
using System.Web.Optimization;

namespace CashFlowManagement
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-datepicker.min.css",
                      "~/Content/selectize.bootstrap3.css",
                      "~/Content/themes/base/jquery-ui.min.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/layout").Include(
                      "~/Scripts/moment.js",
                      "~/Scripts/layout.js",
                      "~/Scripts/moment-range.js",
                      "~/Scripts/bootstrap-datepicker.min.js",
                      "~/Scripts/locales/bootstrap-datepicker.vi.min.js",
                      "~/Scripts/jquery.unobtrusive-ajax.js",
                      "~/Scripts/jquery.mask.min.js",
                      "~/Scripts/cldr.js",
                      "~/Scripts/globalize.js",
                      "~/Scripts/selectize.js",
                      "~/Scripts/jquery-ui-1.12.1.min.js"));
        }
    }
}
