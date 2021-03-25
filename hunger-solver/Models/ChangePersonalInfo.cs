using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace hunger_solver.Models
{
    public class ChangePersonalInfo
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string UserID { get; set; }
        public string UserType { get; set; }

        public string Mobile { get; set; }

    }
}