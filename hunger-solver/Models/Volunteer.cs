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
        [RegularExpression("[0-9]", ErrorMessage = "Please enter digit")]
        [Display(Name = "NID")]
        public string NID { get; set; }

        // edit in dashboard
        public string Image { get; set; }
        public string Address { get; set; }
        public string BloodGroup { get; set; }
        public string AmountOfFoodDonation { get; set; }
        public string AmountOfClothDonation { get; set; }
        public string AmountOfMoneyDonation { get; set; }
        public string AmountOfBloodDonation { get; set; }
    }
}