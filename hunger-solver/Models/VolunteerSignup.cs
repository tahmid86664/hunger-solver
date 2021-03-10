using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace hunger_solver.Models
{
    public class VolunteerSignup
    {
        [Key]
        public string id { get; set; }

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
        [RegularExpression("[0][1][1-9]{1}[0-9]{8}", ErrorMessage = "Please enter 11 digit mobile number")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        [Required]
        [RegularExpression("[0-9]", ErrorMessage = "Please enter digit")]
        [Display(Name = "NID")]
        public string NID { get; set; }
    }
}