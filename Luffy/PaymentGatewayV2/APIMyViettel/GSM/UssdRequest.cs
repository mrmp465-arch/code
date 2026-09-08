using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyViettel.GSM
{
    public class UssdRequest
    {
        public string transId { get; set; }
        public string telco { get; set; }
        public string sim { get; set; }
        public string simTarget { get; set; }
        public string cardSerial { get; set; }
        public string cardCode { get; set; }
        public int actionType { get; set; }
        public string callbackUrl { get; set; }
        public int cardValue { get; set; }
        public int quota { get; set; }
        public string accountName { get; set; }
        public string userName { get; set; }
    }
}