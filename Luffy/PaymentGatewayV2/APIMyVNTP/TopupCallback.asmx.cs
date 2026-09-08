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
using System.Web.Services;
using APIMyVNTP.Entity;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APIMyVNTP
{
    /// <summary>
    /// Summary description for TopupCallback
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class TopupCallback : System.Web.Services.WebService
    {

        [WebMethod]

        public string Callback(string transactionId, int status, int amount, string message, string logContent, string sign)
        {
            string privatKey = "1e7f56ca5fcbf781fa022f4f5dff74d6";
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            if (!string.IsNullOrEmpty(transactionId))
            {

                NLogLogger.Info(new string[] { "TopupCallback", "Request", transactionId, amount.ToString(), status.ToString() });
                var topupMobile3rdLog = new TopupMobile3rdLog();
                topupMobile3rdLog.Id = Convert.ToInt64(transactionId);
                var topup3Rd = topupMobile3rdLog.Get();

                //var messageDb = DataRequest.GetTopupCardLog(transactionId);
                if (topup3Rd != null)
                {
                    var signature = Encrypts.MD5(string.Format("{0}|{1}|{2}|{3}|{4}|{5}", transactionId, status, amount, message, logContent, privatKey));
                    if (signature == sign)
                    //if (1 == 1)
                    {
                        var topupmobile = new TopupMobileLog();
                        topupmobile.TransactionID = Convert.ToInt64(topup3Rd.RequestNo);

                        var cardAPILog = new CardAPILog().Get(Convert.ToInt32(topup3Rd.TransactionId));
                        //cardAPILog.TransactionID = topup3Rd.TransactionId;

                        var privateKey = new Partners().Get(topup3Rd.PartnerCode).PrivateKey;

                        switch (status)
                        {
                            case (int)ResponseCode.TransactionSuspicious:

                                if (topup3Rd.Status != 0)
                                {
                                    var data = new DataCallback()
                                    {
                                        Amount = amount,
                                        RefCode = cardAPILog.RequestNo,
                                        Status = (int)ResponseCode.TransactionSuspicious,
                                        Signature = Libs.Utils.Encrypts.MD5(cardAPILog.RequestNo + (int)ResponseCode.TransactionSuspicious + amount + privateKey)
                                    };
                                    var tcallback = Task.Run(() => CallbackJson(cardAPILog.CallbackUrl, serializer.Serialize(data)));
                                    NLogLogger.Info(new string[] { "TopupCallback", "Callback TransactionSuspicious Partner", topup3Rd.TransactionId.ToString(), tcallback.Result });
                                }

                                //var tms = Task.Run(() => TelegramClient.TelegramSendMessage(-280811434, string.Format("Có thẻ nghi vấn Id: {0}, các anh check giúp em (^_^)", transactionId)));
                                //tms.Wait();

                                topupmobile.Topup(1, amount == 0 ? topup3Rd.AmountUser : amount); // + So tien don hang
                                NLogLogger.Info(new string[] { "TopupCallback", "Request", "TransactionSuspicious", topupmobile.TransactionID.ToString(), amount.ToString() });
                                cardAPILog.Amount = 0;
                                break;
                            case (int)ResponseCode.TransactionSuccessfullNotConfirmYet:

                                var lastAmount = amount - topup3Rd.AmountUser;
                                if (topup3Rd.Status == -2) topupmobile.Topup(1, lastAmount);

                                //Callback KH
                                if (!string.IsNullOrEmpty(cardAPILog.CallbackUrl))
                                {
                                    var cbStatus = amount != cardAPILog.AmountUser ? (int)ResponseCode.CardAmountInvalid : (int)ResponseCode.TransactionSuccessful;

                                    var data = new DataCallback()
                                    {
                                        Amount = amount,
                                        RefCode = cardAPILog.RequestNo,
                                        Status = cbStatus,
                                        Signature = Libs.Utils.Encrypts.MD5(cardAPILog.RequestNo + cbStatus + amount + privateKey)
                                    };

                                    var response = CallbackJson(cardAPILog.CallbackUrl, serializer.Serialize(data)).Result;
                                    NLogLogger.Info(new string[] { "TopupCallback", "Callback ConfirmYet Partner", topup3Rd.TransactionId.ToString(), response });
                                }
                                cardAPILog.Amount = amount;
                                break;
                            case (int)ResponseCode.TransactionCancel:
                                if (topup3Rd.Status == -2) topupmobile.Topup(-5, topup3Rd.AmountUser); //- So tien don hang
                                //Callback KH

                                if (!string.IsNullOrEmpty(cardAPILog.CallbackUrl))
                                {
                                    var data = new DataCallback()
                                    {
                                        Amount = amount,
                                        RefCode = cardAPILog.RequestNo,
                                        Status = (int)ResponseCode.TransactionFailed,
                                        Signature = Libs.Utils.Encrypts.MD5(cardAPILog.RequestNo + (int)ResponseCode.TransactionSuccessful + amount + privateKey)
                                    };

                                    var response = CallbackJson(cardAPILog.CallbackUrl, serializer.Serialize(data)).Result;
                                    NLogLogger.Info(new string[] { "TopupCallback", "Callback Cancel Partner", topup3Rd.TransactionId.ToString(), response });
                                }

                                cardAPILog.Amount = 0;
                                break;
                            case (int)ResponseCode.TransactionRejected:
                                topupmobile.Topup(-1, 0); // Lock don hang
                                break;

                            case (int)ResponseCode.TransactionSuccessful:
                                if (topup3Rd.Status != 0)
                                {
                                    NLogLogger.Info(new string[] { "TopupCallback", "Request", "Transaction Successful", transactionId, amount.ToString(), status.ToString() });
                                    topupmobile.Topup(1, amount); // + So tien don hang
                                    cardAPILog.Amount = amount; // Cap nhat amount
                                    status = (int)ResponseCode.TransactionSuspicious; // Chuyen trang thai
                                }
                                else
                                {
                                    topupmobile.Topup(1, amount); // + So tien don hang
                                }

                                break;
                                //default:
                                //    cardAPILog.Amount = 0;
                                //    topup3Rd.Amount = 0;
                                //    break;


                        }

                        cardAPILog.Description = "Callback " + status + " " + amount;
                        cardAPILog.Status = status;
                        cardAPILog.Update();

                        topup3Rd.Amount = amount;
                        topup3Rd.Status = status;
                        topup3Rd.LogContent = message;
                        topup3Rd.Update();

                        topupmobile.Topup(0, 0); //Mở ra chạy tiếp

                        return "{\"status\":1,\"message\":\"Callback success\"}";
                    }
                    else
                    {
                        return "{\"status\":-1,\"message\":\"Callback failed, Sign invalid\"}";
                    }
                }

                return "{\"status\":1,\"message\":\"Callback success\"}";

            }
            NLogLogger.Info(new string[] { "TopupCallback", "Response", transactionId, amount.ToString(), status.ToString() });
            return "{\"status\":0,\"message\":\"Callback failed\"}";
        }

        private string PostJson(string uri, string postData)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Method = "POST";//GET
            //request.Accept = "JSON";
            using (Stream requestStream = request.GetRequestStream())
            {
                byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                requestStream.Write(postDatabytes, 0, postDatabytes.Length);
            }
            var webResponse = request.GetResponse();
            if (webResponse == null)
            {
                return "Unable to connect to the remote server";
            }
            var sr = new StreamReader(webResponse.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        public async Task<string> CallbackJson(string url, string postData)
        {
            NLogLogger.Info(new string[] { "TopupCallback", "Callback Partner", "Request", url, postData });

            try
            {
                var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.PostAsync(url, httpContent);

                    if (response.Content != null)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        return responseContent;
                    }
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "TopupCallback", "Callback Partner", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }

    }
}
