using System;
using System.ComponentModel.DataAnnotations;

namespace PayrolSystem.Models.ViewModel
{
    public class ClientDetailsVM
    {
        public int ClientID { get; set; }

        [Display(Name = "Company Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Select Company Name")]
        public int CompanyID { get; set; }

        [Display(Name = "User Role")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Select User Role")]
        public string UserRole { get; set; }

        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Gender")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide Gender")]
        public string Gender { get; set; }

        [Display(Name = "Cell Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide Cell Number")]
        public string CellNo { get; set; }

        [Display(Name = "Email Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Display(Name = "Username")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide Username")]
        public string Username { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide Password")]
        public string LogPassword { get; set; }

        public string DateCreated { get; set; }
        public string ActiveStatus { get; set; }

        public ClientDetailsVM ClientDetails { get; set; }
    }
}