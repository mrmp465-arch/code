using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using APIMyViettel;
using Libs.API;
using Libs.Utils;

namespace APITopupMobile
{
    /// <summary>
    /// Summary description for CheckCard
    /// </summary>
    public class CheckCard : IHttpHandler
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Request.ContentType = "application/json";
            context.Response.ContentType = "application/json";
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            var jsonString = String.Empty;
            var result = string.Empty;
            context.Request.InputStream.Position = 0;

            var providerList = new Dictionary<string, string>();
            providerList.Add("team1", "674756b0f307ab613975e30a4aa024eb");
            providerList.Add("team2", "3a2c450907816b307563396c0ee9e54e");
            providerList.Add("team3", "92e7de06b63d1f940f3a56223bd717f4");

            var partnerKey = string.Empty;

            try
            {
                using (var inputStream = new StreamReader(context.Request.InputStream))
                {
                    jsonString = inputStream.ReadToEnd();
                }



                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                var request = javaScriptSerializer.Deserialize<CheckCardRequest>(jsonString);

                foreach (KeyValuePair<string, string> item in providerList)
                {
                    if (item.Key == request.PartnerCode)
                    {
                        partnerKey = item.Value;
                        break;
                    }
                }

                NLogLogger.Info(new string[] { "APICheckCard", request.PartnerCode, "Request", jsonString });

                if (string.IsNullOrEmpty(partnerKey))
                {
                    result = serializer.Serialize(new APIResponse((int)ResponseCode.PartnerNotExistsNotActive));

                }
                else if (string.IsNullOrEmpty(request.Serial) || string.IsNullOrEmpty(request.Serial))
                {
                    result = serializer.Serialize(new APIResponse((int)ResponseCode.TransactionFailed));
                }
                else
                {
                    var signature = Encrypts.MD5(string.Format("{0}|{1}", request.Serial, partnerKey));
                    if (signature != request.Signature)
                    {
                        result = serializer.Serialize(new APIResponse((int)ResponseCode.SignatureInvalid));
                    }
                    else
                    {
                        //result = serializer.Serialize(MyViettelService.CheckCard(request.Serial));
                        result = serializer.Serialize(WebService.CheckCard(request.Serial));
                        NLogLogger.Info(new string[] { "APICheckCard", request.PartnerCode, "Response",  result });
                    }
                }

            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "APICheckCard", "Exception", exp.Message });
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

        public class CheckCardRequest
        {
            public string PartnerCode { get; set; }
            public string Serial { get; set; }
            public string Signature { get; set; }
        }


    }

}