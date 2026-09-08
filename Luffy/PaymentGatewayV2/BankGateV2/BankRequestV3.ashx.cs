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
    public class BankRequestV3 : IHttpHandler
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
                var request = javaScriptSerializer.Deserialize<RequestDataV4>(jsonString);
                //if (string.IsNullOrEmpty(request.AccountName))
                //{
                //    request.AccountName = request.RefCode;
                //}
                string type = "banktranfer";
                if (request.BankCode.ToUpper() == "VTP")
                {
                    type = "vtpay";
                    //context.Response.Write(ResponseUtils.Response((int)ResponseCode.BankCodeInvalid));
                }

                if (request.BankCode.ToUpper() == "MOMO")
                    type = "momo";
                NLogLogger.Info(new string[] { "VPGJsonService", "Request", jsonString });
                result = VPGUtils.Order(request.MCode, request.Amount, request.RefCode, request.BankCode, request.CallbackUrl, request.RefCode, request.Signature, type);
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

        public class RequestDataV4
        {
            public string MCode { get; set; }

            //public string AccountName { get; set; }

            public int Amount { get; set; }
            public string RefCode { get; set; }
            public string CallbackUrl { get; set; }
            public string Signature { get; set; }
            public string BankCode { get; set; }
        }
    }
}