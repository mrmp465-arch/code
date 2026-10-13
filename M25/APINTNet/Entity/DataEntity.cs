using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.Report;

namespace APINTNet.Entity
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

    public class DataCallback
    {
        public string RefCode { get; set; }
        public int Status { get; set; }
        public int Amount { get; set; }
        public string Signature { get; set; }

    }
}

