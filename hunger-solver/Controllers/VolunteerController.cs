using Firebase.Auth;
using FireSharp.Interfaces;
using FireSharp.Response;
using hunger_solver.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace hunger_solver.Controllers
{
    public class VolunteerController : Controller
    {
        // variables
        private static string ApiKey = "AIzaSyDmBVtxgVhAJUokqpqid2UC2Gwc3gRsGG8";
        private static string Bucket = "https://hunger-solver-3237d-default-rtdb.firebaseio.com/";

        // firebase authentication
        IFirebaseConfig firebaseConfig = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = "MyHyxpAMW7Y08rDvUIAYyEwFfFpe9VgcpfvlGWT8",
            BasePath = Bucket
        };
        IFirebaseClient firebaseClient;

        public ActionResult Index()
        {
            // get all volunteers
            var volunteersList = ReadVolunteerFromFirebase();
            ViewBag.volunteersList = volunteersList;

            return View();
        }


        // signup
        [HttpGet]
        public ActionResult Signup()
        {
            return View();
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SignUp(Models.VolunteerSignup model, Models.Volunteer volunteer)
        {
            try
            {
                volunteer.Email = model.Email;
                volunteer.Name = model.Name;
                volunteer.Password = model.Password;
                volunteer.Mobile = model.Mobile;
                volunteer.NID = model.NID;
                volunteer.Designation = "member";
                volunteer.UserType = "volunteer";

                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

                var a = await auth.CreateUserWithEmailAndPasswordAsync(model.Email, model.Password, model.Name, true);
                ModelState.AddModelError(string.Empty, "Please verify your email then login please.");
                CreateVolunteer(volunteer);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View();
        }


        // database store
        public void CreateVolunteer(Models.Volunteer volunteer)
        {
            try
            {
                AddVolunteerToFirebase(volunteer);
                Console.WriteLine("Added Successfully");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Console.WriteLine("exception from Create User: " + ex.Message);
            }
        }

        public void AddVolunteerToFirebase(Models.Volunteer volunteer)
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            var data = volunteer;
            PushResponse response = firebaseClient.Push("Volunteer/", data);
            data._id = response.Result.name;
            SetResponse setResponse = firebaseClient.Set("Volunteer/" + data._id, data);
        }


        // login authentication
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                // Verification.
                if (Request.IsAuthenticated)
                {

                    //  return this.RedirectToLocal(returnUrl);
                }
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }

            // Info.
            return this.View();
        }



        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(Models.VolunteerLogin model, string returnUrl)
        {

            try
            {
                // Verification.
                if (ModelState.IsValid)
                {
                    var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                    var ab = await auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);
                    string token = ab.FirebaseToken;
                    var volunteer = ab.User;
                    bool isVolunteer = false;
                    Volunteer volunteerData = null;
                    if (token != "")
                    {
                        var list = ReadVolunteerFromFirebase();
                        foreach (var v in list)
                        {
                            //Debug.WriteLine(i.Email);
                            if (volunteer.Email == v.Email)
                            {
                                volunteerData = v;
                                isVolunteer = true;
                            }
                        }

                        if (isVolunteer)
                        {
                            Debug.WriteLine("logged in");
                            Debug.WriteLine(ab);
                            Debug.WriteLine(token);
                            ViewBag.User = volunteer;
                            this.SignInUser(volunteer.Email, token, false);
                            Session["volunteer"] = volunteerData;
                            return this.RedirectToLocal(string.Format("/Dashboard/Index2?volunteer={0}", volunteerData), volunteer);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "If you're not volunteer then login from home page or user login page");
                        }
                    }
                    else
                    {
                        // Setting.
                        Debug.Write("Invalid username and pass");
                        ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Info
                Debug.WriteLine("I'm from LoginAction exception");
                //throw ex;
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        private void SignInUser(string email, string token, bool isPersistent)
        {
            // Initialization.
            var claims = new List<Claim>();

            try
            {
                // Setting
                claims.Add(new Claim(ClaimTypes.Email, email));
                claims.Add(new Claim(ClaimTypes.Authentication, token));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                // Sign In.
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimIdenties);
            }
            catch (Exception ex)
            {
                // Info
                Debug.WriteLine("I'm from signInUser exception");
                //throw ex;
            }
        }

        private void ClaimIdentities(string username, bool isPersistent)
        {
            // Initialization.
            var claims = new List<Claim>();
            try
            {
                // Setting
                claims.Add(new Claim(ClaimTypes.Name, username));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            }
            catch (Exception ex)
            {
                // Info
                throw ex;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl, User user)
        {
            try
            {
                ViewBag.User = user;
                Debug.WriteLine(returnUrl);
                // Verification.
                if (Url.IsLocalUrl(returnUrl))
                {
                    // Info.
                    return this.Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                // Info
                throw ex;
            }

            // Info.
            return this.RedirectToAction("LogOff", "User");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogOff()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Debug.WriteLine("Logged out");
            Session["volunteer"] = null;
            return RedirectToAction("Index", "Home");
        }

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


        
        

    }
}