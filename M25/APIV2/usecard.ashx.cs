using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;


namespace APIV2
{
    /// <summary>
    /// Summary description for usecard
    /// </summary>
    public class usecard : IHttpHandler
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
                NLogLogger.Info(new string[] { "API", "Request", jsonString });
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                var request = javaScriptSerializer.Deserialize<RequestDataV2>(jsonString);

                result = VPGUtils.RequestV2(request.PartnerCode, request.CardCode, request.CardSerial, request.Amount, request.CardType,  request.RefCode, request.CallbackUrl, request.Signature);
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "VPGJsonService", "ProcessRequest", exp.Message });
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
    }
    public class RequestDataV2
    {
        public string PartnerCode { get; set; }
        public string CardCode { get; set; }
        public string CardSerial { get; set; }
        public int Amount { get; set; }
        public string CardType { get; set; }
     
        public string CallbackUrl { get; set; }
        public string RefCode { get; set; }
        public string Signature { get; set; }

    }
}