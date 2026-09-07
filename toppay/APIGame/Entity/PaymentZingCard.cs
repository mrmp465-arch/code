using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIGame.Entity
{
    public class PaymentZingCard
    {
        public int returnCode { get; set; }
        public string returnMessage { get; set; }
        public string redirectUrl { get; set; }
        public string smsDetail { get; set; }
        public string servicePhone { get; set; }
        public long transID { get; set; }
    }

    public class PaymentZingCardResult
    {
        public int returnCode { get; set; }
        public string returnMessage { get; set; }
        public int transID { get; set; }
        public string grossValue { get; set; }
        public string netValue { get; set; }
        public string productValue { get; set; }
        public string productCashName { get; set; }
        public string productCode { get; set; }
        public string serverName { get; set; }
        public string roleName { get; set; }
        public int pmcID { get; set; }
        public int isShowPoupAdvertise { get; set; }
        public int isShowPromotionZaloPay { get; set; }
        public string promotionTransID { get; set; }
    }

    public class TransHistory
    {
        public long transID { get; set; }
        public long requestTimeStamp { get; set; }
        public string productCode { get; set; }
        public string roleName { get; set; }
        public int pmcID { get; set; }
        public int pmOptID { get; set; }
        public int pmcGrossChargeAmt { get; set; }
        public int pmcNetChargeAmt { get; set; }
        public double productCashAmt { get; set; }
        public int sID { get; set; }
        public string serverID { get; set; }
    }

 
}