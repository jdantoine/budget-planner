using JDBudgetPlanner.Helpers;
using JDBudgetPlanner.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JDBudgetPlanner.Controllers
{
    [RequireHttps]
    public class IndexBController : Controller
    {
        

        public ActionResult IndexB()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }





    }
}