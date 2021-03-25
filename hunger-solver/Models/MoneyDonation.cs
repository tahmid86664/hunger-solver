using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace hunger_solver.Models
{
    public class MoneyDonation
    {
        [Key]
        public string _id { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public string Amount { get; set; }
        [Required]
        [Display(Name = "Payment Type")]
        public string PaymentType { get; set; }

        [Required]
        public double latitude { get; set; }

        [Required]
        public double longitude { get; set; }

        public string DonatorName { get; set; }
        public string DonatorEmail { get; set; }
        public string DonatorImage { get; set; }
        public string DonatorPhone { get; set; }
        public string VolunteerName { get; set; }
        public string VolunteerEmail { get; set; }
        public string VolunteerPhone { get; set; }
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public bool IsTaken { get; set; }
        public bool IsDelivered { get; set; }
        public bool IsConfirmed { get; set; }
    }
}