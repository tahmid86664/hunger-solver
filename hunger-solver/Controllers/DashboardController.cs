using FireSharp.Interfaces;
using FireSharp.Response;
using hunger_solver.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        // Variables
        private static string ApiKey = "AIzaSyDmBVtxgVhAJUokqpqid2UC2Gwc3gRsGG8";
        private static string Bucket = "https://hunger-solver-3237d-default-rtdb.firebaseio.com/";

        // firebase config
        IFirebaseConfig firebaseConfig = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = "MyHyxpAMW7Y08rDvUIAYyEwFfFpe9VgcpfvlGWT8",
            BasePath = Bucket
        };
        IFirebaseClient firebaseClient;

        // donator dashboard index
        public ActionResult Index()
        {
            //Session["donator"] = (Donator)TempData["donator"];
            //donator = (Donator)Session["donator"];
            //Debug.WriteLine("from dashboard " + donator.Email);
            //ViewBag.donator = donator;
            GetAllDonations();
            return View();
        }

        // this volunteer dashboard index 
        public ActionResult Index2()
        {
            GetAllDonations();
            return View();
        }

        public ActionResult PersonalInfo()
        {
            return View();
        }

        public ActionResult DonateFoodForUser()
        {
            return View();
        }

        public ActionResult DonateClothForUser()
        {
            return View();
        }

        public ActionResult DonateMoneyForUser()
        {
            return View();
        }
        
        public ActionResult DonateBloodForUser()
        {
            return View();
        }

        // get the food donations
        private List<FoodDonation> ReadFoodDonationsFromFirebase()
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            FirebaseResponse response = firebaseClient.Get("FoodDonation");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<FoodDonation>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<FoodDonation>(((JProperty)item).Value.ToString()));
            }

            return list;
        }

        // get the food donations
        private List<ClothDonation> ReadClothDonationsFromFirebase()
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            FirebaseResponse response = firebaseClient.Get("ClothDonation");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<ClothDonation>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<ClothDonation>(((JProperty)item).Value.ToString()));
            }

            return list;
        }

        // get the food donations
        private List<MoneyDonation> ReadMoneyDonationsFromFirebase()
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            FirebaseResponse response = firebaseClient.Get("MoneyDonation");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<MoneyDonation>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<MoneyDonation>(((JProperty)item).Value.ToString()));
            }

            return list;
        }

        private void GetAllDonations()
        {
            var foodList = ReadFoodDonationsFromFirebase();
            Session["foodDonationList"] = foodList;

            var clothList = ReadClothDonationsFromFirebase();
            Session["clothDonationList"] = clothList;

            var moneyList = ReadMoneyDonationsFromFirebase();
            Session["moneyDonationList"] = moneyList;

            var currentFoodList = new List<FoodDonation>();
            foreach(var f in foodList)
            {
                if(f.IsTaken == false)
                {
                    currentFoodList.Add(f);
                }
            }
            Session["currentFoodList"] = currentFoodList;

            var currentClothList = new List<ClothDonation>();
            foreach (var c in clothList)
            {
                if (c.IsTaken == false)
                {
                    currentClothList.Add(c);
                }
            }
            Session["currentClothList"] = currentClothList;

            var currentMoneyList = new List<MoneyDonation>();
            foreach (var m in moneyList)
            {
                if (m.IsTaken == false)
                {
                    currentMoneyList.Add(m);
                }
            }
            Session["currentMoneyList"] = currentMoneyList;
        }
    }
}