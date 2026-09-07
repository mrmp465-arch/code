using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Card.CMS.Models
{
    public class OrderModel
    {
        public int Amount { get; set; }
        public string Mobile { get; set; }
        public string Telco { get; set; }
        public string FullName { get; set; }
        public int AmountMinAll { get; set; }
        public string Password { get; set; }
        public string Password2 { get; set; }
        public string TopupType { get; set; }
        public string OTP { get; set; }
        public int AmountMin { get; set; }
        public string OrderNo { get; set; }
        public int Ussd { get; set; }
    }
}