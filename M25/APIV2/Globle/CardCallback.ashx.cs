using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.CardTelco;
using Libs.Utils;

namespace APIV2.Globle
{
    /// <summary>
    /// Summary description for VPGJsonService
    /// </summary>
    public class CardCallback : IHttpHandler
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
                NLogLogger.Info(new string[] { "GlobleCard", "Callback", jsonString });
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                var callbackData = javaScriptSerializer.Deserialize<CallbackData>(jsonString);
                var cardAPILog = new CardAPILog().Get(Convert.ToInt64(callbackData.req));

                if (cardAPILog == null) result = "1|Unsuccess";

                switch (callbackData.req)
                {
                    case "1":
                        cardAPILog.Status = (int)ResponseCode.TransactionSuccessful;
                        break;
                    case "2":
                        cardAPILog.Status = (int)ResponseCode.CardCodeInvalid;
                        break;
                    case "3":
                        cardAPILog.Status = (int)ResponseCode.CardAmountInvalid;
                        break;
                    case "4":
                        cardAPILog.Status = (int)ResponseCode.TransactionSuspicious;
                        break;
                    default:
                        cardAPILog.Status = (int)ResponseCode.TransactionFailed;
                        break;
                }
                cardAPILog.Amount = Convert.ToInt64(callbackData.amount);
                cardAPILog.Update();

                //Open Post JSON
                var partner = new Partners().Get(cardAPILog.PartnerCode);
                var signature = Encrypts.MD5(cardAPILog.RequestNo + cardAPILog.Amount + cardAPILog.Status + partner.PrivateKey);
                var cbdPartner = new CallbackDataPartner()
                {
                    RefCode = cardAPILog.RequestNo,
                    Amount = Convert.ToInt32(cardAPILog.Amount),
                    Status = cardAPILog.Status,
                    Signature = signature
                };

                var partnerCallback = string.Empty;
                try
                {
                    NLogLogger.Info(new string[] { "GlobleCard", "CallbackPartner", cardAPILog.CallbackUrl, serializer.Serialize(cbdPartner) });
                    partnerCallback = PostData(cardAPILog.CallbackUrl, serializer.Serialize(cbdPartner));
                }
                catch (Exception ex)
                {
                    NLogLogger.Info(new string[] { "GlobleCard", "CallbackPartner", "Error", ex.Message });
                    context.Response.Write("3|Callback Failed Unable to connect to the remote server");
                }

                var dataCallback = partnerCallback.Split('|');
                var partnerResponse = new PartnerResponse()
                {
                    code = Convert.ToInt32(dataCallback[0]),
                    message = dataCallback[1]
                };

                if (partnerResponse.code == 1)
                {
                    result = "0|Success";
                }
                else
                {
                    cardAPILog.Status = (int)ResponseCode.CallbackFailed;
                    cardAPILog.Update();
                    result = "1|Unsuccess";
                }
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GlobleCard", "Callback", "Error", ex.Message });
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

        public string PostData(string uri, string postData)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST"; //GET
                request.Timeout = 30000;
                byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = postDatabytes.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(postDatabytes, 0, postDatabytes.Length);

                var webResponse = request.GetResponse();
                dataStream = webResponse.GetResponseStream();
                if (dataStream == null)
                {
                    webResponse.Close();
                    return "5|Timeout";
                }

                var sr = new StreamReader(dataStream);
                var response = sr.ReadToEnd().Trim();

                sr.Close();
                dataStream.Close();
                webResponse.Close();

                return response;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout)
                {

                    return "-1|failed";
                }

                throw;

            }
        }
    }

    public class CallbackData
    {
        public string req { get; set; }
        public string status { get; set; }
        public string amount { get; set; }
        public string sign { get; set; }
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