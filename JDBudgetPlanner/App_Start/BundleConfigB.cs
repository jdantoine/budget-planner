using System.Web;
using System.Web.Optimization;

namespace JDBudgetPlanner
{
    public class BundleConfigB
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundlesB/jquery").Include(
                        "~/ScriptsB/jquery-{version}.js",
                        "~/ScriptsB/jqueryui-1.11.4.min.js",
                        "~/Site TemplateB/js/jquery.slimscroll.min.js",
                        "~/Site TemplateB/js/classie.js",
                        "~/Site TemplateB/js/sortable.min.js",
                        "~/Site TemplateB/js/bootstrap-select.min.js",
                        "~/Site TemplateB/js/summernote.min.js",
                        "~/Site TemplateB/js/jquery.magnific-popup.min.js",
                        "~/Site TemplateB/js/bootstrap.file-input.js",
                        "~/Site TemplateB/js/bootstrap-datepicker.js",
                        "~/Site TemplateB/js/ickeck.min.js",
                        "~/Site TemplateB/js/jquery.snippet.js",
                        "~/Site TemplateB/js/jquery.easyWizard.js"));

            bundles.Add(new ScriptBundle("~/bundlesB/jqueryval").Include(
                        "~/ScriptsB/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundlesB/modernizr").Include(
                        "~/ScriptsB/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundlesB/bootstrap").Include(
                      "~/ScriptsB/bootstrap.min.js",
                      "~/ScriptsB/respond.js"));

            bundles.Add(new StyleBundle("~/ContentB/css").Include(
                      "~/ContentB/bootstrapB.min.css",
                      "~/ContentB/bootstrapB.css",
                      "~/ContentB/SiteB.css",
                      "~/Site TemplateB/css/style.css",
                      "~/Site TemplateB/css/style-responsive.css",
                      "~/Site TemplateB/css/animate.css",
                      "~/Site TemplateB/css/morris.css",
                      "~/Site TemplateB/css/component.css",
                      "~/Site TemplateB/css/sortable-theme-bootstrap.css",
                      "~/Site TemplateB/css/green.css",
                      "~/Site TemplateB/css/bootstrapB-select.min.css",
                      "~/Site TemplateB/css/summernote.css",
                      "~/Site TemplateB/css/magnific-popup.css",
                      "~/Site TemplateB/css/datepicker.css",
                      "~/ContentB/bootstrapB-social.css"
                      ));
        }
    }
}
