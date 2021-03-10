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

        public string DonatorName { get; set; }
        public string DonatorEmail { get; set; }
        public string DonatorImage { get; set; }
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public bool IsTaken { get; set; }
        public bool IsDelivered { get; set; }
    }
}