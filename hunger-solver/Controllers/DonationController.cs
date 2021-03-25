using FireSharp.Interfaces;
using FireSharp.Response;
using hunger_solver.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace hunger_solver.Controllers
{
    public class DonationController : Controller
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

        [HttpGet]
        public ActionResult FoodDonation()
        {
            if (User.Identity.IsAuthenticated)
                return View();
            TempData["alertMsg"] = "Please login first";
            return Redirect("/User/Login");
        }

        [HttpGet]
        public ActionResult ClothDonation()
        {
            if (User.Identity.IsAuthenticated)
                return View();
            TempData["alertMsg"] = "Please login first";
            return Redirect("/User/Login");
        }

        [HttpGet]
        public ActionResult MoneyDonation()
        {
            if (User.Identity.IsAuthenticated)
                return View();
            TempData["alertMsg"] = "Please login first";
            return Redirect("/User/Login");
        }

        public ActionResult BloodDonation()
        {
            if (User.Identity.IsAuthenticated)
                return View();
            TempData["alertMsg"] = "Please login first";
            return Redirect("/User/Login");
        }

        [HttpPost]
        public async Task<ActionResult> FoodDonation(FoodDonation model)
        {
            try
            {
                RootObject userLoc = getAddress(model.latitude, model.longitude);

                /* Debug.WriteLine(userLoc.address.road);
                Debug.WriteLine(userLoc.address.suburb);
                Debug.WriteLine(userLoc.address.city);
                Debug.WriteLine(userLoc.address.state_district);
                Debug.WriteLine(userLoc.address.state);
                Debug.WriteLine(userLoc.address.postcode);
                Debug.WriteLine(userLoc.address.country);
                Debug.WriteLine(userLoc.address.country_code); */

                Donator donator = (Donator)Session["donator"];
                model.DonatorName = donator.Name;
                model.DonatorEmail = donator.Email;
                model.DonatorPhone = donator.Mobile;
                model.Date = DateTime.Now;
                model.Place = userLoc.display_name;
                model.IsTaken = false;
                model.IsDelivered = false;
                model.IsConfirmed = false;
                

                /* ================== send e-mail to volunteer ================ */
                var bodyMessage = "You have a notification for food donation";
                var body = "<p>Email From: {0} ({1})</p><p>Message: </p><p>{2}</p>";
                var message = new MailMessage();
                var volunteerList = ReadVolunteerFromFirebase();
                // recipient mail (we have to send the notification to all volunteer)
                foreach (var v in volunteerList)
                {
                    message.To.Add(new MailAddress(v.Email));
                }
                message.From = new MailAddress("techtoon526628@gmail.com");  // sender mail
                message.Subject = "Notifications for food donation from Hunger Solver";
                message.Body = string.Format(body, model.DonatorName, model.DonatorEmail, bodyMessage);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    // the following information is fixed for gmail
                    // for outlook the host should be "smtp-mail.outlook.com"
                    // configuration for the Client 
                    var credential = new NetworkCredential
                    {
                        UserName = "techtoon526628@gmail.com",  // sender mail
                        Password = "#526628Tahmid"  // sender pass
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                }

                CreateFoodDonationToFirebase(model);

                return this.Redirect("/Donation/FoodDonation");
            }
            catch(Exception e)
            {
                Debug.WriteLine("Exception from FoodDonation Submit: " + e);
            }

            return View();
        }

        public void CreateFoodDonationToFirebase(FoodDonation food)
        {
            try
            {
                AddFoodDonationToFirebase(food);
                ModelState.AddModelError(string.Empty, "Submitted Successfully");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Debug.WriteLine("exception from Create food donation: " + ex.Message);
            }
        }

        public void AddFoodDonationToFirebase(FoodDonation food)
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            var data = food;
            PushResponse response = firebaseClient.Push("FoodDonation/", data);
            data._id = response.Result.name;
            SetResponse setResponse = firebaseClient.Set("FoodDonation/" + data._id, data);
        }

        // cloth
        [HttpPost]
        public async Task<ActionResult> ClothDonation(ClothDonation model)
        {
            try
            {
                RootObject userLoc = getAddress(model.latitude, model.longitude);

                Donator donator = (Donator)Session["donator"];
                model.DonatorName = donator.Name;
                model.DonatorEmail = donator.Email;
                model.Date = DateTime.Now;
                model.Place = userLoc.display_name;
                model.IsTaken = false;
                model.IsDelivered = false;
                model.IsConfirmed = false;


                /* ================== send e-mail to volunteer ================ */
                var bodyMessage = "You have a notification for cloth donation";
                var body = "<p>Email From: {0} ({1})</p><p>Message: </p><p>{2}</p>";
                var message = new MailMessage();
                var volunteerList = ReadVolunteerFromFirebase();
                // recipient mail (we have to send the notification to all volunteer)
                foreach (var v in volunteerList)
                {
                    message.To.Add(new MailAddress(v.Email));
                }
                message.From = new MailAddress("techtoon526628@gmail.com");  // sender mail
                message.Subject = "Notifications for cloth donation from Hunger Solver";
                message.Body = string.Format(body, model.DonatorName, model.DonatorEmail, bodyMessage);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    // the following information is fixed for gmail
                    // for outlook the host should be "smtp-mail.outlook.com"
                    // configuration for the Client 
                    var credential = new NetworkCredential
                    {
                        UserName = "techtoon526628@gmail.com",  // sender mail
                        Password = "#526628Tahmid"  // sender pass
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                }

                CreateClothDonationToFirebase(model);

                return this.Redirect("/Donation/ClothDonation");
            }
            catch(Exception e)
            {
                Debug.WriteLine("Exception from ClothDonation Submit: " + e);
            }

            return View();
        }

        public void CreateClothDonationToFirebase(ClothDonation cloth)
        {
            try
            {
                AddClothDonationToFirebase(cloth);
                ModelState.AddModelError(string.Empty, "Submitted Successfully");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Debug.WriteLine("exception from Create cloth donation: " + ex.Message);
            }
        }

        public void AddClothDonationToFirebase(ClothDonation cloth)
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            var data = cloth;
            PushResponse response = firebaseClient.Push("ClothDonation/", data);
            data._id = response.Result.name;
            SetResponse setResponse = firebaseClient.Set("ClothDonation/" + data._id, data);
        }

        // money
        [HttpPost]
        public async Task<ActionResult> MoneyDonation(MoneyDonation model)
        {
            try
            {
                RootObject userLoc = getAddress(model.latitude, model.longitude);

                Donator donator = (Donator)Session["donator"];
                model.DonatorName = donator.Name;
                model.DonatorEmail = donator.Email;
                model.Date = DateTime.Now;
                model.Place = userLoc.display_name;
                model.IsTaken = false;
                model.IsDelivered = false;
                model.IsConfirmed = false;


                /* ================== send e-mail to volunteer ================ */
                var bodyMessage = "You have a notification for money donation";
                var body = "<p>Email From: {0} ({1})</p><p>Message: </p><p>{2}</p>";
                var message = new MailMessage();
                var volunteerList = ReadVolunteerFromFirebase();
                // recipient mail (we have to send the notification to all volunteer)
                foreach (var v in volunteerList)
                {
                    message.To.Add(new MailAddress(v.Email));
                }
                message.From = new MailAddress("techtoon526628@gmail.com");  // sender mail
                message.Subject = "Notifications for money donation from Hunger Solver";
                message.Body = string.Format(body, model.DonatorName, model.DonatorEmail, bodyMessage);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    // the following information is fixed for gmail
                    // for outlook the host should be "smtp-mail.outlook.com"
                    // configuration for the Client 
                    var credential = new NetworkCredential
                    {
                        UserName = "techtoon526628@gmail.com",  // sender mail
                        Password = "#526628Tahmid"  // sender pass
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                }

                CreateMoneyDonationToFirebase(model);

                return this.Redirect("/Donation/MoneyDonation");
            }
            catch(Exception e)
            {
                Debug.WriteLine("Exception from MoneyDonation Submit: " + e);
            }

            return View();
        }

        public void CreateMoneyDonationToFirebase(MoneyDonation money)
        {
            try
            {
                AddMoneyDonationToFirebase(money);
                ModelState.AddModelError(string.Empty, "Submitted Successfully");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Debug.WriteLine("exception from Create money donation: " + ex.Message);
            }
        }

        public void AddMoneyDonationToFirebase(MoneyDonation money)
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            var data = money;
            PushResponse response = firebaseClient.Push("MoneyDonation/", data);
            data._id = response.Result.name;
            SetResponse setResponse = firebaseClient.Set("MoneyDonation/" + data._id, data);
        }




        // get the volunteers
        private List<Volunteer> ReadVolunteerFromFirebase()
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            FirebaseResponse response = firebaseClient.Get("Volunteer");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Volunteer>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Volunteer>(((JProperty)item).Value.ToString()));
            }

            return list;
        }









        // tracking the location
        public RootObject getAddress(double lat, double lon)
        {
            WebClient webClient = new WebClient();

            webClient.Headers.Add("user-agent", "Mozilla/4.0(compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            webClient.Headers.Add("Referer", "http://www.microsoft.com");

            var jsonData = webClient.DownloadData("http://nominatim.openstreetmap.org/reverse?format=json&lat=" + lat + "&lon=" + lon);

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(RootObject));

            RootObject rootObject = (RootObject)ser.ReadObject(new MemoryStream(jsonData));

            return rootObject;
        }

    }
}