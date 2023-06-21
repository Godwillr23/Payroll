using System;
using System.ComponentModel.DataAnnotations;

namespace PayrolSystem.Models.ViewModel
{
    public class MasterLoginVM
    {
        public int MasterID { get; set; }

        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide Password")]
        public string LogPassword { get; set; }
        public string DateCreated { get; set; }

        public MasterLoginVM MasterLogins { get; set; }
    }
}