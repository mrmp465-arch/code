using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using IZISoft.Entity;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace IZISoft
{
    /// <summary>
    /// Summary description for VPGJsonService
    /// </summary>
    public class CardCallback : IHttpHandler
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {

            context.Response.ContentType = "text/plain";
            var result = string.Empty;
            try
            {
                var uuid = context.Request.QueryString["uuid"];
                var tranid = context.Request.QueryString["tranid"];
                var cardCode = context.Request.QueryString["cardCode"];
                var cardSerial = context.Request.QueryString["cardSerial"];
                var amount = context.Request.QueryString["amount"];
                var status = context.Request.QueryString["status"];

                NLogLogger.Info(new string[] { "IZISoftCard", "Callback", tranid, status, uuid, cardCode, cardSerial, amount });

                if (string.IsNullOrEmpty(uuid) || string.IsNullOrEmpty(tranid) || string.IsNullOrEmpty(cardCode) || string.IsNullOrEmpty(cardSerial) || string.IsNullOrEmpty(amount))
                {
                    context.Response.Write("03|Data empty");
                    return;
                }


                var messageDb = DataRequest.GetTopupCardLog(tranid);
                if (messageDb != null)
                {
                    var responseCode = (int)ResponseCode.UndefinedError;
                    switch (status)
                    {

                        case "1":
                            responseCode = (int)ResponseCode.TransactionSuccessful;
                            break;
                        case "2":
                            responseCode = (int)ResponseCode.CardUsed;
                            break;
                        default:
                            responseCode = (int)ResponseCode.TransactionFailed;
                            break;
                    }

                    var resultUpdate = DataRequest.UpdateTopupCard(Convert.ToInt64(tranid), Convert.ToInt32(amount), responseCode, "IZISoftCard Callback: " + tranid + "|" + status + "|" + uuid + "|" + cardCode + "|" + cardSerial + "|" + amount, string.Empty);
                    if (resultUpdate == 0)
                    {

                        if (messageDb.Status == (int)ResponseCode.TransactionTimeout)
                        {
                            var cardAPILog = new CardAPILog().Get(Convert.ToInt32(messageDb.TransactionId));
                            cardAPILog.Amount = Convert.ToInt32(amount);
                            cardAPILog.Description = "Callback " + responseCode + " " + amount;
                            cardAPILog.Status = responseCode;
                            //cardAPILog.Update();

                            //Begin Callback for partner
                            if (responseCode == (int)ResponseCode.TransactionSuccessful)
                            {
                                if (cardAPILog.AmountUser != Convert.ToInt32(amount))
                                {
                                    responseCode = (int)ResponseCode.CardAmountInvalid;
                                    if (Convert.ToInt32(amount) < 50000)
                                        cardAPILog.Status = (int)ResponseCode.CardAmountInvalid;
                                }
                            }
                            else
                            {
                                responseCode = (int)ResponseCode.TransactionFailed;
                            }

                            cardAPILog.Update();

                            var privateKey = new Partners().Get(messageDb.PartnerCode).PrivateKey;
                            var datacb = new DataCallback()
                            {
                                Amount = Convert.ToInt32(amount),
                                RefCode = cardAPILog.RequestNo,
                                Status = responseCode,
                                Signature = Libs.Utils.Encrypts.MD5(cardAPILog.RequestNo + responseCode + Convert.ToInt32(amount) + privateKey)
                            };

                            var tcallback = Task.Run(() => CallbackJson(cardAPILog.CallbackUrl, serializer.Serialize(datacb)));
                            NLogLogger.Info(new string[] { "IZISoftCard", "Callback Partner", messageDb.TransactionId.ToString(), tcallback.Result });

                            //End Callback for partner



                        }

                        result = "00|Callback Success";

                    }
                    else
                    {
                        result = "01|Callback Failed";
                    }
                }


            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "IZISoftCard", "Callback", "Error", ex.Message });
                context.Response.Write("02|Invalid value");
                return;
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

        public async Task<string> CallbackJson(string url, string postData)
        {
            NLogLogger.Info(new string[] { "IZISoftCard", "Callback Partner", "Request", url, postData });

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
                NLogLogger.Info(new string[] { "IZISoftCard", "Callback Partner", "Error", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }
    }



}