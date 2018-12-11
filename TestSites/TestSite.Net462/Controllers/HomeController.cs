using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utilities.Logging;

namespace TestSite.Net462.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Log.Debug("Index Debug");
            Log.Error("Index Error");
            //Log.Fatal("Index Fatal");
            Log.Info("Index Info");
            Log.Trace("Index Trace");
            Log.Warn("Index Warn");

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}