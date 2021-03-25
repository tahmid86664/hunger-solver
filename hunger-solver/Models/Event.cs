using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace hunger_solver.Models
{
    public class Event
    {
        [Key]
        public string _id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Place { get; set; }
        public DateTime Date { get; set; }
        public string VolunteerName { get; set; }
        public string VolunteerEmail { get; set; }
        public string VolunteerPhone { get; set; }

        public bool IsCompleted { get; set; }

        // for blood donation events
        public string BloodGroup { get; set; }
        public int BloodBagNeed { get; set; }
        
    }
}