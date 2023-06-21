using System;
using System.ComponentModel.DataAnnotations;

namespace PayrolSystem.Models.ViewModel
{
    public class CompanyDetailsVM
    {
        public int CompanyID { get; set; }
        public int MasterID { get; set; }

        [Display(Name = "Company Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide Company Address")]
        public string CompanyAddress { get; set; }

        [Display(Name = "Telephone Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide Telephone Number")]
        public string TelNumber { get; set; }

        public string DateCreated { get; set; }
        public string ActiveStatus { get; set; }

        public CompanyDetailsVM CompanyDetailsVMs { get; set; }
    }
}