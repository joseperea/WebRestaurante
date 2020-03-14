using System.Web;
using System.Web.Optimization;

namespace WebRestaurante
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

            bundles.Add(new ScriptBundle("~/bundles/vue").Include(
                     "~/Scripts/vue.js"));

            bundles.Add(new ScriptBundle("~/Content/fontawesome-free-5.12.1-web/js").Include(
                "~/Content/fontawesome-free-5.12.1-web/js/brands.js",
                "~/Content/fontawesome-free-5.12.1-web/js/conflict-detection.js",
                "~/Content/fontawesome-free-5.12.1-web/js/fontawesome.js",
                "~/Content/fontawesome-free-5.12.1-web/js/regular.js",
                "~/Content/fontawesome-free-5.12.1-web/js/solid.js",
                "~/Content/fontawesome-free-5.12.1-web/js/v4-shims.js",
               "~/Content/fontawesome-free-5.12.1-web/js/all.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.bundle.js",
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-grid.css",
                      "~/Content/bootstrap-reboot.css",
                      "~/Content/bootstrap-reboot.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/fontawesome-free-5.12.1-web/css").Include(
                    "~/Content/fontawesome-free-5.12.1-web/css/brands.css",
                    "~/Content/fontawesome-free-5.12.1-web/css/svg-with-js.css",
                    "~/Content/fontawesome-free-5.12.1-web/css/fontawesome.css",
                    "~/Content/fontawesome-free-5.12.1-web/css/regular.css",
                    "~/Content/fontawesome-free-5.12.1-web/css/solid.css",
                    "~/Content/fontawesome-free-5.12.1-web/css/v4-shims.css",
                   "~/Content/fontawesome-free-5.12.1-web/css/all.css"));
        }
    }
}
