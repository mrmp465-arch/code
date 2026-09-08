using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.Report;

namespace APIMon.Entity
{
    public class DataRequest
    {

        public static int UpdateTopupCard(long id, int amount, int status, string logContent, long requestNo)
        {
            var topup = new TopupMobile3rdLog();
            topup.Id = id;
            topup.Amount = amount;
            topup.Status = status;
            topup.LogContent = logContent;
            topup.Sim = string.Empty;
            topup.RequestNo = 0;
            topup.SimTarget = requestNo.ToString();
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

    public class DataCallback
    {
        public string RefCode { get; set; }
        public int Status { get; set; }
        public int Amount { get; set; }
        public string Signature { get; set; }

    }
}

