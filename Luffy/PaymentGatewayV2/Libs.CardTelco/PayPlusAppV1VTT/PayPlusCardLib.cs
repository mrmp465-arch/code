using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libs.CardTelco.PayPlusAppV1VTT
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
        public string simTarget { get; set; }
    }

    public class TopupResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public int amount { get; set; }
    }


}

