using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libs.CardTelco.PayPlusAppVNTP
{
    public class TopupRequest
    {
        public string cardSerial { get; set; }
        public string cardCode { get; set; }
        public string telco { get; set; }
        public string partnerCode { get; set; }
        public string providerCode { get; set; }
        public long transactionId { get; set; }
        public int amount { get; set; }
        public string sim { get; set; }
        public string simTarget { get; set; }
        public string clientId { get; set; }
        public int slot { get; set; }
    }

    public class TopupResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public int amount { get; set; }
    }

    public class DataCallback
    {
        public string RefCode { get; set; }
        public int Status { get; set; }
        public int Amount { get; set; }
        public string Signature { get; set; }

    }


}

