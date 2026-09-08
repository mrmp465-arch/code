using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyVNTP.Entity
{
    public class MyVNTPAppLoginResquest
    {
        public string device_info { get; set; }
        public string fcm_registration_token { get; set; }
        public string mode { get; set; }
        public string msisdn { get; set; }
        public string password { get; set; }

    }

    public class MyVNTPAppBalanceResquest
    {

        public string msisdn { get; set; }
        public string session { get; set; }

    }

    public class MyVNTPAppRechargeResquest
    {

        public string card_id { get; set; }
        public string fcm_otp { get; set; }
        public string fcm_token { get; set; }
        public string for_msisdn { get; set; }
        public string session { get; set; }
        //public string api_secret { get; set; }

    }

    public class MyVNTPAppFcmResquest
    {
        public string fcm_token { get; set; }
        public string otp_service { get; set; }
        public string session { get; set; }
        //public string api_secret { get; set; }
    }
}
