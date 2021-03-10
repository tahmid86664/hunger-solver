using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using Firebase.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security;
using hunger_solver.Models;
using System.Diagnostics;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace hunger_solver.Controllers
{
    public class UserController : Controller
    {
        private static string ApiKey = "AIzaSyDmBVtxgVhAJUokqpqid2UC2Gwc3gRsGG8";
        private static string Bucket = "https://hunger-solver-3237d-default-rtdb.firebaseio.com/";

        // firebase configurattion
        IFirebaseConfig firebaseConfig = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = "MyHyxpAMW7Y08rDvUIAYyEwFfFpe9VgcpfvlGWT8",
            BasePath = Bucket
        };
        IFirebaseClient firebaseClient;

        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        // edited 05 March
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SignUp(SignUpModel model, Models.Donator user)
        {
            try
            {
                user.Email = model.Email;
                user.Name = model.Name;
                user.Password = model.Password;
                user.Mobile = model.Mobile;

                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

                var a = await auth.CreateUserWithEmailAndPasswordAsync(model.Email, model.Password, model.Name, true);
                ModelState.AddModelError(string.Empty, "Please verify your email then login please.");
                CreateUser(user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View();
        }

        //after signing up the data should be in the database
        public void CreateUser(Models.Donator user)
        {
            try
            {
                AddUserToFirebase(user);
                ModelState.AddModelError(string.Empty, "Added Successfully");
                Console.WriteLine("Added Successfully");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Console.WriteLine("exception from Create User: " + ex.Message);
            }
        }

        public void AddUserToFirebase(Models.Donator user)
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            var data = user;
            PushResponse response = firebaseClient.Push("Donator/", data);
            data._id = response.Result.name;
            SetResponse setResponse = firebaseClient.Set("Donator/" + data._id, data);
        } 


        //edited 05 March
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
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {

            try
            {
                // Verification.
                if (ModelState.IsValid)
                {
                    var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                    var ab = await auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);
                    string token = ab.FirebaseToken;
                    var user = ab.User;
                    Donator donator = null;
                    bool isDonator = false;
                    if (token != "")
                    {
                        // need to read user data for varifying user or volunteer
                        var list = ReadUserFromFirebase();
                        foreach(var i in list)
                        {
                            //Debug.WriteLine(i.Email);
                            if(user.Email == i.Email){
                                donator = i;
                                isDonator = true;
                            }
                        }

                        if (isDonator) { 
                            // complete then login
                            Debug.WriteLine("logged in");
                            Debug.WriteLine(ab);
                            Debug.WriteLine(token);
                            Debug.WriteLine(donator);
                            ViewBag.User = user;
                            this.SignInUser(user.Email, token, false);
                            Session["donator"] = donator;
                            return this.RedirectToLocal(string.Format("/Dashboard?user={0}", donator), user);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "If you're volunteer then login from volunteer login page");
                        }
                    }
                    else
                    {
                        // Setting.
                        Debug.Write("Invalid username and pass");
                        ModelState.AddModelError(string.Empty, "Invalid username or password");
                    }
                }
            }
            catch (Exception ex)
            {
                // Info
                Debug.WriteLine("I'm from LoginAction exception");
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
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
            Session["donator"] = null;
            return RedirectToAction("Index", "Home");
        }

        private List<Donator> ReadUserFromFirebase()
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            FirebaseResponse response = firebaseClient.Get("Donator");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Donator>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Donator>(((JProperty)item).Value.ToString()));
            }

            return list;
        }
    }
}
