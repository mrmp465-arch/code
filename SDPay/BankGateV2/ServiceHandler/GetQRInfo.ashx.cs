using Libs.API;
using Libs.BankDirect.MDrum;
using Libs.BankGate.Entity;
using Libs.Utils;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using static BankGateV2.bankin.Info;
using static BankGateV2.ServiceHandler.GetQRInfo;
using static BankGateV2.ServiceHandler.GetQRInfo2;
using static Libs.BankDirect.MDrum.MDrumBankLib;

namespace BankGateV2.ServiceHandler
{
    /// <summary>
    /// Summary description for GetQRInfo
    /// </summary>
    public class GetQRInfo : IHttpHandler
    {
        private const string urlBaseServiceBank = "http://45.32.115.186:1592/ServiceHandler/GetQRInfo.ashx";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            context.Request.ContentType = "application/json";
            context.Response.ContentType = "application/json";
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            var jsonString = String.Empty;
            var result = string.Empty;
            context.Request.InputStream.Position = 0;

            using (var inputStream = new StreamReader(context.Request.InputStream))
            {
                jsonString = inputStream.ReadToEnd();
            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            var response = Task.Run(async () => await MDrumBankLib.PostTask(urlBaseServiceBank, jsonString)).Result;

            NLogLogger.Info(new string[] { "MDrum", "GetBanks Request",jsonString, urlBaseServiceBank,response
                    });

            if (!string.IsNullOrEmpty(response))
            {
                var bankinfo = serializer.Deserialize<BankInfo>(response);
                context.Response.Write(serializer.Serialize(bankinfo));

            }
        }
        public class RequestAccount
        {
            public string BankCode { get; set; }
            public string BankId { get; set; }

        }
        public string GetBankAccount(string bankcode, string bankId)
        {

            var key = string.Format("{0}:{1}", "BankReceivedAccount", bankcode + bankId);
            var result = DataCaching.GetCache<string>(key);
            //if (result != null)
            //{
            //    return result;
            //}
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
            var bankrequest = new RequestAccount
            {
                BankCode = bankcode,
                BankId = bankId
            };
            //var Partner = new Partners().Get("order");
            var requestContent = serializer.Serialize(bankrequest);
            //var signature = Encrypts.MD5(Partner.PartnerCode + "GETLIST" + requestContent + Partner.PublicKey);
            var requestData = new RequestData()
            {
                PartnerCode = "order",
                CommandCode = "ACCOUNT_QUERY",
                RequestContent = requestContent,
                Signature = ""
            };

            //var Partner = new Partners().Get(partnercode);
            ////check theo partner
            //if (!string.IsNullOrEmpty(Partner.SMSPlusCheckUrl))
            //{
            //    requestData.PartnerCode = Partner.SMSPlusCheckUrl;
            //}

            var response = Task.Run(async () => await MDrumBankLib.PostTask(urlBaseServiceBank, serializer.Serialize(requestData))).Result;

            //NLogLogger.Info(new string[] { "MDrum", "GetBanks Request",serializer.Serialize(requestData), urlBaseServiceBank,response
            //        });

            if (!string.IsNullOrEmpty(response))
            {
                var resObj = serializer.Deserialize<MDrumBankLib.BankResponse>(response);
                if (resObj.ResponseCode > 0)
                {
                    var account = resObj.ResponseContent;
                    DataCaching.SetCache(key, account, 86400 * 7);
                    return account;


                }
                return "";
            }
            return "";

        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        public class RequestGetQR
        {
            public string qrtext { get; set; }
            // public int type { get; set; }

            //public string Signature { get; set; }

        }
        public class BankInfo
        {
            public string bankAccountNumber { get; set; }
            public string bankAccountName { get; set; }
            public string bankCode { get; set; }
            public string bankName { get; set; }
            public int amount { get; set; }
            public string content { get; set; }
            public string countryCode { get; set; }
            public int responsecode { get; set; }
        }
        
    }

}

