using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Report;

namespace APITopupMobile
{
    public class DataRequest
    {
        public string call_id { get; set; }
        public int call_type { get; set; }
        public string call_number { get; set; }
        public string target_Number { get; set; }
        public string send_message { get; set; }
        public string other_message { get; set; }
        public string call_provider { get; set; }
        public int call_slot { get; set; }
        public string url { get; set; }
        public int quota { get; set; }

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

        public static string GetListSimUSSDStatus(string clientId)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var response = serializer.Serialize(new SimUSSD().GetListSimUSSDStatus(clientId));
            return response;
        }

        public static int UpdateSim(string clientId, int slot, string sim, int? status)
        {
            return new SimUSSD().UpdateStatus(clientId, slot, sim, status);
        }

    }

    public class DataCallback
    {
        public string RefCode { get; set; }
        public int Status { get; set; }
        public int Amount { get; set; }
        public string Signature { get; set; }

    }

    public class DataCallbackOrder
    {
        public long OrderId { get; set; }
        public int Status { get; set; }
        public int Amount { get; set; }
        public string Signature { get; set; }
        public string CardSerial { get; set; }
        public string CardCode { get; set; }
        public decimal? BidRate { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime? CreatTime { get; set; }

    }
}

