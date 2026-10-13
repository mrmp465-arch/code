using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMobiNext.Entity
{
    public class TopupRequest
    {
        public string phoneNumber { get; set; }
        public string pin { get; set; }
        public string serial { get; set; }
        public string promoCode { get; set; }
        public string valueCaptcha { get; set; }
        public string token { get; set; }
        public string accountName { get; set; }
    }
}