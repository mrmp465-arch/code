using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WebApi.Models
{
    // Models used as parameters to AccountController actions.
    public class VTTAcountBindingModel
    {
        [Required]
        [StringLength(12, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 9)]
        [Display(Name = "AccountName")]
        public string AccountName { get; set; }
        
        [Required]
        [StringLength(15, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Type")]
        public int Type { get; set; }
    }    

    public class AddOrderBindingModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        [Display(Name = "FullName")]
        public string FullName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "OrderNo")]
        public string OrderNo { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "Telco")]
        public string Telco { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "TopupType")]
        public int TopupType { get; set; }

        [Required]
        [Range(5000, int.MaxValue, ErrorMessage = "Please enter a value bigger than {5000}")]
        [Display(Name = "Amount")]
        public int Amount { get; set; }

        [Required]
        [Range(-1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {-1}")]
        [Display(Name = "AmountMin")]
        public int AmountMin { get; set; }

        [Required]
        [Range(-1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {-1}")]
        [Display(Name = "AmountMinAll")]
        public int AmountMinAll { get; set; }

        [Required]
        [Range(0, 50, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Priority")]
        public int Priority { get; set; }

        [Display(Name = "AccountName")]
        public string AccountName { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Range(-9, 9, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [Required]
        [Range(0, 2, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Ussd")]
        public int Ussd { get; set; }

        [Display(Name = "CallbackUrl")]
        public string CallbackUrl { get; set; }

        [Display(Name = "SubUser")]
        public string SubUser { get; set; }

        [Display(Name = "ExtData")]
        public string ExtData { get; set; }
    }

    public class EditOrderBindingModel
    {
        [Required]
        [Display(Name = "TransactionID")]
        public long TransactionID { get; set; }

        [Required]
        [Range(-1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {-1}")]
        [Display(Name = "AmountMin")]
        public int AmountMin { get; set; }

        [Required]
        [Range(-1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {-1}")]
        [Display(Name = "AmountMinAll")]
        public int AmountMinAll { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Range(-6, 3, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [Required]
        [Range(0, 50, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Priority")]
        public int Priority { get; set; }
        
        [Display(Name = "IsConfirm")]
        public int? IsConfirm { get; set; }

        [Display(Name = "BidRate")]
        public decimal? BidRate { get; set; }

        [Display(Name = "BidFee")]
        public int? BidFee { get; set; }

        
        [Range(0, 2, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Ussd")]
        public int Ussd { get; set; }

        [Display(Name = "SubUser")]
        public string SubUser { get; set; }

    }

    public class StartPauseOrderBindingModel
    {
        [Required]
        [Range(-9, 1, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Status")]
        public int Status { get; set; }



    }

    //public class OrderHistoryBindingModel
    //{
    //    [Required]
    //    [Display(Name = "TransactionID")]
    //    public long TransactionID { get; set; }

    //    [Required]
    //    [Range(-1, 1, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    //    [Display(Name = "Status")]
    //    public int Status { get; set; }

    //}

}
