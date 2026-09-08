using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;
namespace BankGateV2
{
    /// <summary>
    /// Summary description for GetMoMo
    /// </summary>
    public class MoMoCashV2 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Request.ContentType = "application/json";
            context.Response.ContentType = "application/json";
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            var jsonString = String.Empty;
            var result = string.Empty;
            context.Request.InputStream.Position = 0;
            try
            {
                using (var inputStream = new StreamReader(context.Request.InputStream))
                {
                    jsonString = inputStream.ReadToEnd();
                }
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                var request = javaScriptSerializer.Deserialize<RequestDataV2>(jsonString);
                NLogLogger.Info(new string[] { "VPGJsonService", "Request", jsonString });
                if (request.Amount >= 3000000)
                {
                    TelegramNotify.SendWarning("1690000254", $"Có lệnh rút tiền lớn từ {request.PartnerCode} số tiền {request.Amount}");
                }
                result = VPGUtils.RequestMomoCash(request.PartnerCode, request.AccountNumber, request.AccountName, request.Amount, request.RefCode, request.BankCode, request.Signature, request.CallbackUrl);
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "VPGJsonService", "Exeption", exp.Message });
                context.Response.Write(ResponseUtils.Response((int)ResponseCode.ParameterInvalid));
            }
            context.Response.Write(result);
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public class RequestDataV2
        {
            public string PartnerCode { get; set; }
            public string AccountNumber { get; set; }
            public string AccountName { get; set; }
            public string CallbackUrl { get; set; }
            public int Amount { get; set; }
            public string RefCode { get; set; }
             public string BankCode { get; set; }
            public string Signature { get; set; }

        }
    }
}