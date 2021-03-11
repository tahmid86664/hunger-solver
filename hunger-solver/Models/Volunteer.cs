using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace hunger_solver.Models
{
    public class Volunteer
    {
        [Key]
        public string _id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        [Required]
        [Display(Name = "NID")]
        public string NID { get; set; }

        public string UserType { get; set; }

        // edit in dashboard
        public string Designation { get; set; }
        public string Image { get; set; }
        public string Address { get; set; }
        public string BloodGroup { get; set; }
        public string AmountOfFoodDonation { get; set; }
        public string AmountOfClothDonation { get; set; }
        public string AmountOfMoneyDonation { get; set; }
        public string AmountOfBloodDonation { get; set; }
    }
}