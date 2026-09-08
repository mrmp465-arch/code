using Libs.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIBB2D.Entity
{
    public class DataRequest
    {

        public static int UpdateTopupCard(long id, int amount, int status, string logContent, string sim)
        {
            var topup = new TopupMobile3rdLog();
            topup.Id = id;
            topup.Amount = amount;
            topup.Status = status;
            topup.LogContent = logContent;
            topup.Sim = sim;
            topup.RequestNo = 0;
            topup.SimTarget = string.Empty;
            topup.Update();
            return topup.ReturnValue;
        }

        public static TopupMobile3rdLog GetTopupCardLog(string messageId)
        {
            var topup = new TopupMobile3rdLog();
            topup.Id = Convert.ToInt64(messageId);
            return topup.Get();
        }

    }

    public class CardRequest
    {
        public string CardSerial { get; set; }
        public string CardCode { get; set; }
        public string CardType { get; set; }
        public string AccountName { get; set; }
        public string AppCode { get; set; }
        public string RefCode { get; set; }
        public int AmountUser { get; set; }
        public string CallBackUrl { get; set; }
    }

    public class RequestData
    {
        public string PartnerCode { get; set; }
        public string ServiceCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public string Signature { get; set; }

    }
    public class DataCallback
    {
        public string RefCode { get; set; }
        public int Status { get; set; }
        public int Amount { get; set; }
        public int RealAmount { get; set; }
        
        public string Signature { get; set; }

    }
    public class CardResponse
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string ResponseContent { get; set; }
        public string Signature { get; set; }
    }

}