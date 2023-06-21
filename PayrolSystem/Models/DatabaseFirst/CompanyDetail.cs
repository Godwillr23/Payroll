//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PayrolSystem.Models.DatabaseFirst
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class CompanyDetail
    {
        public int CompanyID { get; set; }
        public int MasterID { get; set; }

        [Display(Name = "Company Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide Company Code")]
        public string CompanyCode { get; set; }

        [Display(Name = "Company Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Company Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide Company Address")]
        public string CompanyAddress { get; set; }

        [Display(Name = "Payment Day")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provide Payment Day")]
        public string PaymentDay { get; set; }

        public string PaymentStatus { get; set; }

        [Display(Name = "Level")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Select Level")]
        public int Level { get; set; }

        public string ActiveStatus { get; set; }
        public string DateCreated { get; set; }
    }
}
