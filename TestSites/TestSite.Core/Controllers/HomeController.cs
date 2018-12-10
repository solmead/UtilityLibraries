using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestSite.Core.Models;
using Utilities.Logging;

namespace TestSite.Core.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Log.Debug("Index Debug");
            Log.Error("Index Error");
            //Log.Fatal("Index Fatal");
            Log.Info("Index Info");
            Log.Trace("Index Trace");
            Log.Warn("Index Warn");


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
