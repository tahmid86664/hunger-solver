using hunger_solver.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hunger_solver.Controllers
{
    public class DashboardController : Controller
    {
        private static Donator donator;
        // GET: Dashboard
        public ActionResult Index()
        {
            //Session["donator"] = (Donator)TempData["donator"];
            //donator = (Donator)Session["donator"];
            //Debug.WriteLine("from dashboard " + donator.Email);
            //ViewBag.donator = donator;
            return View();
        }

        public ActionResult Index2()
        {
            return View();
        }

        public ActionResult PersonalInfo()
        {
            return View();
        }
    }
}