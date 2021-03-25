using Firebase.Auth;
using FirebaseAdmin.Auth;
using FireSharp.Interfaces;
using FireSharp.Response;
using hunger_solver.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FirebaseAuth = FirebaseAdmin.Auth.FirebaseAuth;

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



        /* =============================== volunteer section =========================== */

        // =========== confirm donation request =============
        [HttpPost]
        public ActionResult ConfirmDonationByVolunteer(ConfirmDonation model)
        {
            string item_id = model.item_id;
            Debug.WriteLine(item_id);
            Debug.WriteLine("i'm triggering");
            try
            {
                firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);

                if (model.item_type == "food")
                {
                    FirebaseResponse response = firebaseClient.Get("FoodDonation/" + item_id);
                    FoodDonation data = JsonConvert.DeserializeObject<FoodDonation>(response.Body);
                    UpdateFoodConfirmationStatus(data);
                }else if(model.item_type == "cloth")
                {
                    FirebaseResponse response = firebaseClient.Get("ClothDonation/" + item_id);
                    ClothDonation data = JsonConvert.DeserializeObject<ClothDonation>(response.Body);
                    UpdateClothConfirmationStatus(data);
                }
                else if (model.item_type == "money")
                {
                    FirebaseResponse response = firebaseClient.Get("MoneyDonation/" + item_id);
                    MoneyDonation data = JsonConvert.DeserializeObject<MoneyDonation>(response.Body);
                    UpdateMoneyConfirmationStatus(data);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception from Food Donation confirmation: " + ex);
            }

            return RedirectToAction("/Index2");

        }

        public void UpdateFoodConfirmationStatus(FoodDonation foodData)
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            Volunteer volunteer = (Volunteer)Session["volunteer"];
            foodData.IsConfirmed = true;
            foodData.VolunteerName = volunteer.Name;
            foodData.VolunteerPhone = volunteer.Mobile;
            foodData.VolunteerEmail = volunteer.Email;
            SetResponse setResponse = firebaseClient.Set("FoodDonation/" + foodData._id, foodData);

            SendConfirmationMessageToDonor("food", foodData.Name, foodData.VolunteerName, foodData.VolunteerPhone, foodData.VolunteerEmail, foodData.DonatorEmail);
            Debug.WriteLine("updated response");
        }

        public void UpdateClothConfirmationStatus(ClothDonation data)
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            Volunteer volunteer = (Volunteer)Session["volunteer"];
            data.IsConfirmed = true;
            data.VolunteerName = volunteer.Name;
            data.VolunteerPhone = volunteer.Mobile;
            data.VolunteerEmail = volunteer.Email;
            SetResponse setResponse = firebaseClient.Set("ClothDonation/" + data._id, data);

            SendConfirmationMessageToDonor("cloth", data.Name, data.VolunteerName, data.VolunteerPhone, data.VolunteerEmail, data.DonatorEmail);
            Debug.WriteLine("updated response");
        }

        public void UpdateMoneyConfirmationStatus(MoneyDonation data)
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            Volunteer volunteer = (Volunteer)Session["volunteer"];
            data.IsConfirmed = true;
            data.VolunteerName = volunteer.Name;
            data.VolunteerPhone = volunteer.Mobile;
            data.VolunteerEmail = volunteer.Email;
            SetResponse setResponse = firebaseClient.Set("MoneyDonation/" + data._id, data);
            
            SendConfirmationMessageToDonor("money", data.Amount, data.VolunteerName, data.VolunteerPhone, data.VolunteerEmail, data.DonatorEmail);
            Debug.WriteLine("updated response");
        }

        // ============= send confirmation message to donor =================
        private async void SendConfirmationMessageToDonor(string type, string itemName, string volunteerName, string volunteerPhone, string volunteerEmail, string donorEmail)
        {
            /* ================== send e-mail to volunteer ================ */
            var bodyMessage = "You have a notification for your" + type + "donation <br />Donate Item or Amount(in TK): " + itemName +
                "<br />Volunteer Name: " + volunteerName + "Volunteer Mobile: " + volunteerPhone;
            var body = "<p>Email From: {0} ({1})</p><p>Message: </p><p>{2}</p>";
            var message = new MailMessage();

            message.To.Add(new MailAddress(donorEmail));
            message.From = new MailAddress(volunteerEmail);  // sender mail
            message.Subject = "Notifications for money donation from Hunger Solver";
            message.Body = string.Format(body, volunteerEmail, volunteerName, bodyMessage);
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
        }


        // =========== take donation =============
        [HttpPost]
        public ActionResult TakenDonationByVolunteer(ConfirmDonation model)
        {
            string item_id = model.item_id;
            Debug.WriteLine(item_id);
            Debug.WriteLine("i'm triggering");
            try
            {
                firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);

                if (model.item_type == "food")
                {
                    FirebaseResponse response = firebaseClient.Get("FoodDonation/" + item_id);
                    FoodDonation data = JsonConvert.DeserializeObject<FoodDonation>(response.Body);
                    UpdateFoodTakenStatus(data);
                }
                else if (model.item_type == "cloth")
                {
                    FirebaseResponse response = firebaseClient.Get("ClothDonation/" + item_id);
                    ClothDonation data = JsonConvert.DeserializeObject<ClothDonation>(response.Body);
                    UpdateClothTakenStatus(data);
                }
                else if (model.item_type == "money")
                {
                    FirebaseResponse response = firebaseClient.Get("MoneyDonation/" + item_id);
                    MoneyDonation data = JsonConvert.DeserializeObject<MoneyDonation>(response.Body);
                    UpdateMoneyTakenStatus(data);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception from Food Donation confirmation: " + ex);
            }

            return RedirectToAction("/Index2");

        }

        public void UpdateFoodTakenStatus(FoodDonation data)
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            data.IsTaken = true;

            SetResponse setResponse = firebaseClient.Set("FoodDonation/" + data._id, data);

            Debug.WriteLine("updated response");
        }

        public void UpdateClothTakenStatus(ClothDonation data)
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            data.IsTaken = true;

            SetResponse setResponse = firebaseClient.Set("ClothDonation/" + data._id, data);

            Debug.WriteLine("updated response");
        }

        public void UpdateMoneyTakenStatus(MoneyDonation data)
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            data.IsTaken = true;

            SetResponse setResponse = firebaseClient.Set("MoneyDonation/" + data._id, data);

            Debug.WriteLine("updated response");
        }




        // ================== deliver donation ==============
        public ActionResult DeliveredDonationByVolunteer(ConfirmDonation model)
        {
            string item_id = model.item_id;
            Debug.WriteLine(item_id);
            Debug.WriteLine("i'm triggering");
            try
            {
                firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);

                if (model.item_type == "food")
                {
                    FirebaseResponse response = firebaseClient.Get("FoodDonation/" + item_id);
                    FoodDonation data = JsonConvert.DeserializeObject<FoodDonation>(response.Body);
                    UpdateFoodDeliveredStatus(data);
                }
                else if (model.item_type == "cloth")
                {
                    FirebaseResponse response = firebaseClient.Get("ClothDonation/" + item_id);
                    ClothDonation data = JsonConvert.DeserializeObject<ClothDonation>(response.Body);
                    UpdateClothDeliveredStatus(data);
                }
                else if (model.item_type == "money")
                {
                    FirebaseResponse response = firebaseClient.Get("MoneyDonation/" + item_id);
                    MoneyDonation data = JsonConvert.DeserializeObject<MoneyDonation>(response.Body);
                    UpdateMoneyDeliveredStatus(data);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception from Food Donation confirmation: " + ex);
            }

            return RedirectToAction("/Index2");
        }

        public void UpdateFoodDeliveredStatus(FoodDonation data)
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            data.IsDelivered = true;

            SetResponse setResponse = firebaseClient.Set("MoneyDonation/" + data._id, data);

            Debug.WriteLine("updated response");
        }

        public void UpdateClothDeliveredStatus(ClothDonation data)
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            data.IsDelivered = true;

            SetResponse setResponse = firebaseClient.Set("MoneyDonation/" + data._id, data);

            Debug.WriteLine("updated response");
        }

        public void UpdateMoneyDeliveredStatus(MoneyDonation data)
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            data.IsDelivered = true;

            SetResponse setResponse = firebaseClient.Set("MoneyDonation/" + data._id, data);

            Debug.WriteLine("updated response");
        }




        /* ========================= change password ======================== */
        public async Task<ActionResult> ChangePassword(ChangePersonalInfo model)
        {
            try
            {
                firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
                Debug.WriteLine(model.UserID);
                Debug.WriteLine(model.UserType);


                if (model.UserType == "donor")
                {
                    Donator donor = (Donator)Session["donator"];
                    
                    if (model.Password.Equals(model.ConfirmPassword))
                    {
                        FirebaseResponse response = firebaseClient.Get("Donator/" + model.UserID);
                        Donator data = JsonConvert.DeserializeObject<Donator>(response.Body);
                        data.Password = model.Password;
                        Debug.WriteLine(data.Email);

                        // delet auth first
                        UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(data.Email);
                        Debug.WriteLine(userRecord);
                        //await FirebaseAuth.DefaultInstance.DeleteUserAsync(userRecord.Uid);
                        
                        // set again auth
                        var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                        var a = await auth.CreateUserWithEmailAndPasswordAsync(data.Email, data.Password, data.Name);

                        SetResponse setResponse = firebaseClient.Set("Donator/" + data._id, data);

                        ModelState.AddModelError(string.Empty, "Password changed successfully");
                        Debug.WriteLine("Password changed successfully");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Password did not match");
                    }
                }
                else if (model.UserType == "volunteer")
                {
                    Volunteer volunteer = (Volunteer)Session["volunteer"];
                    if (model.Password.Equals(model.ConfirmPassword))
                    {
                        FirebaseResponse response = firebaseClient.Get("Volunteer/" + model.UserID);
                        Volunteer data = JsonConvert.DeserializeObject<Volunteer>(response.Body);
                        data.Password = model.Password;

                        SetResponse setResponse = firebaseClient.Set("Volunteer/" + data._id, data);

                        ModelState.AddModelError(string.Empty, "Password changed successfully");
                        Debug.WriteLine("Password changed successfully");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Password did not match");
                    }
                }
            }catch(Exception ex)
            {
                Debug.WriteLine("Exception from pass change " + ex);
            }

            return RedirectToAction("/PersonalInfo");
        }


        /* ========================== change mobile ========================= */
        public ActionResult ChangeMobile(ChangePersonalInfo model)
        {
            try
            {
                firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
                Debug.WriteLine(model.UserID);
                Debug.WriteLine(model.UserType);

                if (model.UserType == "donor")
                {
                    FirebaseResponse response = firebaseClient.Get("Donator/" + model.UserID);
                    Donator data = JsonConvert.DeserializeObject<Donator>(response.Body);
                    data.Mobile = model.Mobile;

                    SetResponse setResponse = firebaseClient.Set("Donator/" + data._id, data);

                    ModelState.AddModelError(string.Empty, "Mobile number changed successfully");
                    Debug.WriteLine("Mobile number changed successfully");

                }
                else if (model.UserType == "volunteer")
                {
                    FirebaseResponse response = firebaseClient.Get("Volunteer/" + model.UserID);
                    Volunteer data = JsonConvert.DeserializeObject<Volunteer>(response.Body);
                    data.Mobile = model.Mobile;

                    SetResponse setResponse = firebaseClient.Set("Volunteer/" + data._id, data);

                    ModelState.AddModelError(string.Empty, "Mobile number changed successfully");
                    Debug.WriteLine("Mobile number changed successfully");
                }
            }catch(Exception ex)
            {
                Debug.WriteLine("Exception from change mobile " + ex);
            }

            return RedirectToAction("/PersonalInfo");
        }
    }
}