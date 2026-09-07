using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;

namespace APIMobiNext
{
    /// <summary>
    /// Summary description for VPGJsonService
    /// </summary>
    public class VPGJsonService : IHttpHandler
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Request.ContentType = "application/json";
            context.Response.ContentType = "application/json";
            var jsonString = String.Empty;
            var PartnerKey = "2fc427ed4bcc70e1f20861ecaba9cdde";
            var ProviderCode = "ApiNext";
            context.Request.InputStream.Position = 0;
            try
            {
                using (var inputStream = new StreamReader(context.Request.InputStream))
                {
                    jsonString = inputStream.ReadToEnd();
                }
                NLogLogger.Info(new string[] { "TopupMobiNext API", "ProcessRequest", jsonString });
                var request = serializer.Deserialize<RequestData>(jsonString);
                var sign = Encrypts.MD5(string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", request.RequestNo, request.PartnerCode, request.CardSerial, request.CardCode, request.AmountUser, request.Mobile, request.Token, PartnerKey));
                if (request.RequestNo == 0 || string.IsNullOrEmpty(request.PartnerCode) || string.IsNullOrEmpty(request.CardSerial)
                    || string.IsNullOrEmpty(request.CardCode) || string.IsNullOrEmpty(request.CardCode) || request.AmountUser == 0
                    || string.IsNullOrEmpty(request.Mobile) || string.IsNullOrEmpty(request.Token))
                {
                    context.Response.Write(ResponseUtils.Response((int)ResponseCode.ParameterInvalid));
                }

                context.Response.Write(sign != request.Signature ? serializer.Serialize(ResponseUtils.Response((int)ResponseCode.SignatureInvalid)) : new MobiNextBiz().SendWebTopupCardAPI(Convert.ToInt64(request.RequestNo), request.PartnerCode, request.CardSerial, request.CardCode, request.Mobile, request.AmountUser, request.Token));
                //context.Response.Write(new MobiNextBiz().SendWebTopupCardAPI(Convert.ToInt64(request.RequestNo), request.PartnerCode, request.CardSerial, request.CardCode, request.Mobile, request.AmountUser, request.Token));
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "VPGJsonService", "ProcessRequest", exp.Message });
                context.Response.Write(ResponseUtils.Response((int)ResponseCode.ParameterInvalid));
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    public class RequestData
    {
        public Int64 RequestNo { get; set; }
        public string PartnerCode { get; set; }
        public string CardSerial { get; set; }
        public string CardCode { get; set; }
        public int AmountUser { get; set; }
        public string Mobile { get; set; }
        public string Token { get; set; }
        public string Signature { get; set; }

    }
}