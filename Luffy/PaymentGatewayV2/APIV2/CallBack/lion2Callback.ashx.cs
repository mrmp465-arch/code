using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.CardTelco;
using Libs.Utils;

namespace APIV2.CallBack
{
    /// <summary>
    /// Summary description for VPGJsonService
    /// </summary>
    public class lion2Callback : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Request.ContentType = "application/json";
            context.Response.ContentType = "text/plain";
            var jsonString = String.Empty;
            var result = string.Empty;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            context.Request.InputStream.Position = 0;
            try
            {
                using (var inputStream = new StreamReader(context.Request.InputStream))
                {
                    jsonString = inputStream.ReadToEnd();
                }
                NLogLogger.Info(new string[] { "Lion2", "Callback", jsonString });
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                var callbackData = javaScriptSerializer.Deserialize<CallbackData>(jsonString);
                var cardAPILog = new CardAPILog().Get(Convert.ToInt64(callbackData.RefCode));

                if (cardAPILog == null) result = "1|Unsuccess";

                var callbackStatus = 0;
                switch (callbackData.Status)
                {
                    case 1:
                        cardAPILog.Status = (int)ResponseCode.TransactionSuccessful;
                        callbackStatus = (int)ResponseCode.TransactionSuccessful;
                        break;
                    case -327:
                        cardAPILog.Status = (int)ResponseCode.TransactionSuccessful;
                        callbackStatus = (int)ResponseCode.CardAmountInvalid;
                        break;
                    default:
                        cardAPILog.Status = callbackData.Status;
                        callbackStatus = callbackData.Status;
                        break;
                }
                cardAPILog.Amount = Convert.ToInt64(callbackData.Amount);
                cardAPILog.Update();

                //Open Post JSON
                var partner = new Partners().Get(cardAPILog.PartnerCode);
                var signature = Encrypts.MD5(cardAPILog.RequestNo + callbackStatus + cardAPILog.Amount + partner.PrivateKey);
                var cbdPartner = new CallbackDataPartner()
                {
                    RefCode = cardAPILog.RequestNo,
                    Amount = Convert.ToInt32(cardAPILog.Amount),
                    Status = callbackStatus,
                    Signature = signature
                };

                
                try
                {
                    NLogLogger.Info(new string[] { "Lion2", "CallbackPartner", cardAPILog.CallbackUrl, serializer.Serialize(cbdPartner) });
                    Task.Run(async () => await CallbackJson(cardAPILog.CallbackUrl, serializer.Serialize(cbdPartner), cardAPILog.PartnerCode + " " + cardAPILog.TransactionID).ConfigureAwait(false));
                }
                catch (Exception ex)
                {
                    NLogLogger.Info(new string[] { "Lion2", "CallbackPartner", "Error", ex.Message });
                    context.Response.Write("3|Callback Failed Unable to connect to the remote server");
                }

                context.Response.Write("0|Success");

            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Lion2", "Callback", "Error", ex.Message, jsonString });
                context.Response.Write("2|Invalid value");
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

        public async Task<string> CallbackJson(string url, string postData, string code)
        {
            NLogLogger.Info(new string[] { "Lion2", "Callback Partner", "Request", code, url, postData });

            try
            {
                var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.PostAsync(url, httpContent).ConfigureAwait(false);

                    if (response.Content != null)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        NLogLogger.Info(new string[] { "Lion2", "Callback Partner", "Response", code, url, postData, responseContent });
                        return responseContent;
                    }
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "Lion2", "Callback Partner", "Error", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }
    }

    public class CallbackData
    {
        public string RefCode { get; set; }
        public int Status { get; set; }
        public string Amount { get; set; }
        public string Signature { get; set; }
    }

    public class PartnerResponse
    {
        public int code { get; set; }
        public string message { get; set; }
    }


    public class CallbackDataPartner
    {
        public string RefCode { get; set; }
        public int Status { get; set; }
        public int Amount { get; set; }
        public string Signature { get; set; }
    }


}