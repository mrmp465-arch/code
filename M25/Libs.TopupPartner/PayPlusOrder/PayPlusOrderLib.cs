using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libs.TopupPartner.PayPlusOrder
{
    public class CashRequest
    {
        public string telco { get; set; }
        public string partnerCode { get; set; }
        public string providerCode { get; set; }
        public string transactionId { get; set; }
        public int amount { get; set; }
        public int quantity { get; set; }
        public string clientId { get; set; }
    }

    public class CashResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public string content { get; set; }
    }

    public class CardPPOrg
    {
        public string Name { get; set; }
        public string Serial { get; set; }
        public string Pin { get; set; }
    }
    public class CardDVO
    {
        public string Serial { get; set; }
        public string Pin { get; set; }
        public DateTime ExpireDate { get; set; }
    }

    public class TranferRequest
    {
        public string telco { get; set; }
        public string partnerCode { get; set; }
        public string providerCode { get; set; }
        public long transactionId { get; set; }
        public int amount { get; set; }
        public string simTarget { get; set; }
        public string clientId { get; set; }
    }

    public class TranferResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        
    }

}

