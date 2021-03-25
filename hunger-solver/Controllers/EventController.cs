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
    public class EventController : Controller
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


        // GET: Event
        public ActionResult Index()
        {
            var eventList = ReadEventsFromFirebase();
            ViewBag.events = eventList;

            return View();
        }

        private List<Event> ReadEventsFromFirebase()
        {
            firebaseClient = new FireSharp.FirebaseClient(firebaseConfig);
            FirebaseResponse response = firebaseClient.Get("Events");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Event>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Event>(((JProperty)item).Value.ToString()));
            }

            return list;
        }
    }
}