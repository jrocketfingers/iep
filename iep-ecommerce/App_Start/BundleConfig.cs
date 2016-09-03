using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;

namespace iep_ecommerce
{
    public class NonOrderingBundleOrderer : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;

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

            /** Material */
            bundles.Add(new ScriptBundle("~/bundles/material").Include(
                        "~/Scripts/materialize.js",
                        "~/Scripts/initialize-material.js"));

            bundles.Add(new StyleBundle("~/Content/material").Include(
                        "~/Content/materialize.css"));
            /** */

            bundles.Add(new StyleBundle("~/Content/custom").Include(
                        "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/nouislider").Include(
                        "~/Scripts/nouislider.js",
                        "~/Scripts/wNumb.js"));

            bundles.Add(new StyleBundle("~/Content/nouislider").Include(
                        "~/Content/nouislider.css",
                        "~/Content/nouislider.pips.css",
                        "~/Content/nouislider.tooltips.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.css"));


            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                        "~/bower_components/underscore/underscore.js",
                        "~/Scripts/price-slider.js"));

            bundles.Add(new ScriptBundle("~/bundles/signalr").Include(
                        "~/Scripts/jquery.signalR-2.2.1.js"));

            var auctions_bundle = new ScriptBundle("~/bundles/auctions").Include(
                        "~/Client/Auction/auction.hub.js",
                        "~/Client/Auction/auction.component.js",
                        "~/Client/auctionsIndex.js");
            auctions_bundle.Orderer = new NonOrderingBundleOrderer();
            bundles.Add(auctions_bundle);

            bundles.Add(new StyleBundle("~/Content/animation").Include(
                        "~/Content/animate.css",
                        "~/Content/morphist.css"));

            bundles.Add(new ScriptBundle("~/bundles/animation").Include(
                        "~/Scripts/morphist.js"));

            bundles.Add(new ScriptBundle("~/bundles/utils").Include(
                        "~/Scripts/moment.js",
                        "~/Scripts/countdown.js"));
        }
   }
}
