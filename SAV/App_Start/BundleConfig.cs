﻿using System.Web;
using System.Web.Optimization;

namespace SAV
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/Waiting").Include(
                        "~/Scripts/spin.js", 
                        "~/Scripts/Waiting.js"));

            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                        "~/Scripts/commonFase2.js",
                        "~/Scripts/jquery.placeholder.js",
                        "~/Scripts/jquery.tablesorter.js",
                        "~/Scripts/jquery.datePicker.js",
                        "~/Scripts/Jquery.customFunctions.js",
                        "~/Scripts/Common.js"));

            bundles.Add(new ScriptBundle("~/bundles/ClienteCreate").Include(
                        "~/Scripts/ClienteCreate.js"));

            bundles.Add(new ScriptBundle("~/bundles/ViajeDetails").Include(
                        "~/Scripts/ViajeDetails.js"));


            bundles.Add(new ScriptBundle("~/bundles/Domicilio").Include(
                        "~/Scripts/Domicilio.js"));

            bundles.Add(new ScriptBundle("~/bundles/Gasto").Include(
                        "~/Scripts/Gasto.js"));

            bundles.Add(new ScriptBundle("~/bundles/Comisiones").Include(
                        "~/Scripts/ComisionSearch.js",
                        "~/Scripts/ComisionCreate.js",
                        "~/Scripts/ComisionesResponsable.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                       "~/Content/Site.css",
                        "~/Content/date-picker.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}