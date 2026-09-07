using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;

namespace BankGateV2.bankin
{
    /// <summary>
    /// Summary description for Order
    /// </summary>
    public class Order : IHttpHandler
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
                var request = javaScriptSerializer.Deserialize<RequestOrder>(jsonString);
                string type = "banktranfer";

                if (request.BankCode.ToUpper() == "MOMO")
                    type = "momov2";
                NLogLogger.Info(new string[] { "VPGJsonService", "Request", jsonString });
                
                result = VPGUtils.Order(request.PartnerCode, request.Amount, request.RefCode, request.BankCode, request.CallbackUrl, request.Signature, type);
                
                result = result.Replace(@"\u0026", "&");
                result = result.Replace("+", " ");
                NLogLogger.Info(new string[] { "Order",request.RefCode, result });
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
        public class RequestOrder
        {
            public string PartnerCode { get; set; }
            public int Amount { get; set; }
            public string RefCode { get; set; }
            public string CallbackUrl { get; set; }
            public string Signature { get; set; }
            public string BankCode { get; set; }
        }
    }
}