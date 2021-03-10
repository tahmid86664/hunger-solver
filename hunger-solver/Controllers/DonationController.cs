using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hunger_solver.Controllers
{
    public class DonationController : Controller
    {
        // GET: Donation
        public ActionResult FoodDonation()
        {
            return View();
        }

        public ActionResult ClothDonation()
        {
            return View();
        }

        public ActionResult MoneyDonation()
        {
            return View();
        }

        public ActionResult BloodDonation()
        {
            return View();
        }
    }
}