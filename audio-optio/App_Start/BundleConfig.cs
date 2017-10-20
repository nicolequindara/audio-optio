﻿using System.Web;
using System.Web.Optimization;

namespace audio_optio
{
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

            bundles.Add(new ScriptBundle("~/bundles/analytics").Include("~/Scripts/GoogleAnalytics.js"));

            bundles.Add(new ScriptBundle("~/bundles/fancybox").Include(
                     "~/Scripts/jquery.fancybox.js"));

            
            bundles.Add(new ScriptBundle("~/bundles/landing").Include(
                     "~/Scripts/landing.js"));

            bundles.Add(new StyleBundle("~/Content/fonts").Include(
                      "~/Content/fonts.css"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css", 
                      "~/Content/site.css",
                      "~/Content/jquery.fancybox.css"));

            bundles.Add(new StyleBundle("~/Content/landing").Include(
                      "~/Content/landing.css"));
        }
    }
}
