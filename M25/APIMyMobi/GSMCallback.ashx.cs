using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using APIMyMobi.Entity;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APIMyMobi
{
    /// <summary>
    /// Summary description for GSMCallback
    /// </summary>
    public class GSMCallback : IHttpHandler
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private string privateKey = "6e8a32079a53a89bc14f23f85cbe2dcc";
        public void ProcessRequest(HttpContext context)
        {

            context.Request.ContentType = "application/json";
            context.Response.ContentType = "application/json";
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            var jsonString = String.Empty;
            var result = string.Empty;
            context.Request.InputStream.Position = 0;

            //string[] providers = { "vanappmobi", "ppvmsgsm" };

            try
            {
                using (var inputStream = new StreamReader(context.Request.InputStream))
                {
                    jsonString = inputStream.ReadToEnd();
                }

                NLogLogger.Info(new string[] { "GSMService", "Callback", jsonString });
                if (string.IsNullOrEmpty(jsonString))
                {
                    context.Response.Write("{\"status\":\"0\",\"message\":\"Thất bại\"}");
                    return;
                }
                var resObj = javaScriptSerializer.Deserialize<DataCallback>(jsonString);

                var messageDb = DataRequest.GetTopupCardLog(resObj.RefCode);
                int amountReal = 0;
                if (messageDb != null)
                {
                    var topupProcess = new TopupMobileLog() { TransactionID = Convert.ToInt64(messageDb.RequestNo) }.Get();

                    //if (!providers.Contains(messageDb.ProviderCode))
                    //{
                    //    context.Response.Write("{\"status\":\"0\",\"message\":\"Thất bại (Không lấy được transaction) \"}");
                    //    return;
                    //}

                    var responseCode = (int)ResponseCode.UndefinedError;
                    switch (resObj.Status)
                    {
                        case (int)ResponseCode.TransactionSuccessful:

                            amountReal = Convert.ToInt32(resObj.Amount);
                            int[] listValue = { 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000 };
                            if (!listValue.Contains(amountReal))
                            {
                                responseCode = (int)ResponseCode.TransactionSuspicious;
                                topupProcess.Topup(1, messageDb.AmountUser);
                            }
                            else
                            {
                                responseCode = (int)ResponseCode.TransactionSuccessful;
                                topupProcess.Topup(1, amountReal); // Thanh công update Amount
                            }

                            break;
                        case (int)ResponseCode.CardUsed:
                            topupProcess.Topup(0, 0); // mở lại cho chạy
                            responseCode = (int)ResponseCode.CardUsed;
                            break;
                        case (int)ResponseCode.CardCodeInvalid:
                            topupProcess.Topup(0, 0); // mở lại cho chạy
                            responseCode = (int)ResponseCode.CardCodeInvalid;
                            break;
                        case (int)ResponseCode.TransactionSuspicious:
                            topupProcess.Topup(1, messageDb.AmountUser);
                            responseCode = (int)ResponseCode.TransactionSuspicious;
                            break;
                        case (int)ResponseCode.ServiceIsLocked:
                            topupProcess.Topup(-2, 0); // khóa đơn
                            TelegramNotify.SendNotify(messageDb.ProviderCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, 2);
                            responseCode = (int)ResponseCode.ServiceIsLocked;
                            break;
                        case (int)ResponseCode.AccessDenied:
                            topupProcess.Topup(-2, 0); // khóa đơn
                            TelegramNotify.SendNotify(messageDb.ProviderCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, 2);
                            responseCode = (int)ResponseCode.ServiceIsLocked;
                            break;
                        default:
                            responseCode = (int)ResponseCode.TransactionFailed;
                            break;
                    }


                    var resultUpdate = DataRequest.UpdateTopupCard(Convert.ToInt64(resObj.RefCode), amountReal, responseCode, jsonString);

                    if (resultUpdate == 0)
                    {

                        if (messageDb.Status == (int)ResponseCode.TransactionTimeout)
                        {
                            var cardAPILog = new CardAPILog().Get(Convert.ToInt32(messageDb.TransactionId));
                            cardAPILog.Amount = amountReal;
                            cardAPILog.Description = "Callback " + responseCode + " " + amountReal;
                            cardAPILog.Status = responseCode;
                            //cardAPILog.Update();

                            //Begin Callback for partner
                            if (responseCode == (int)ResponseCode.TransactionSuccessful)
                            {
                                if (cardAPILog.AmountUser != amountReal)
                                {
                                    responseCode = (int)ResponseCode.CardAmountInvalid;
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
                                Amount = amountReal,
                                RefCode = cardAPILog.RequestNo,
                                Status = responseCode,
                                Signature = Libs.Utils.Encrypts.MD5(cardAPILog.RequestNo + responseCode + amountReal + privateKey)
                            };

                            var tcallback = Task.Run(() => CallbackJson(cardAPILog.CallbackUrl, serializer.Serialize(datacb)));
                            NLogLogger.Info(new string[] { "GSMService", "Callback Partner", messageDb.TransactionId.ToString(), tcallback.Result });

                            //End Callback for partner
                        }

                        if (topupProcess.AmountTopupSuccess + amountReal >= topupProcess.Amount)
                        {

                            NLogLogger.Info(new string[] { "GSMService", "Thay Sim", topupProcess.AmountTopupSuccess.ToString(), amountReal.ToString(), topupProcess.Amount.ToString() });
                            TelegramNotify.SendNotify(messageDb.ProviderCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, 4);
                            result = "{\"status\":\"2\",\"message\":\"Thay sim\"}";
                        }
                        else
                        {
                            NLogLogger.Info(new string[] { "GSMService", "Đã nhận", topupProcess.AmountTopupSuccess.ToString(), amountReal.ToString(), topupProcess.Amount.ToString() });
                            result = "{\"status\":\"1\",\"message\":\"Đã nhận\"}";
                        }


                    }
                    else
                    {
                        NLogLogger.Info(new string[] { "GSMService", "UpdateTopupCard Failed", resObj.RefCode });
                        result = "{\"status\":\"0\",\"message\":\"Thất bại\"}";
                    }
                }
                else
                {
                    NLogLogger.Info(new string[] { "GSMService", "Callback", "Transaction not found", resObj.RefCode });
                    context.Response.Write("{\"status\":\"0\",\"message\":\"Thất bại\"}");
                    return;
                }


            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "GSMService", "Callback", "Error", exp.Message });
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

        public async Task<string> CallbackJson(string url, string postData)
        {
            NLogLogger.Info(new string[] { "GSMService", "Callback Partner", "Request", url, postData });

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
                NLogLogger.Info(new string[] { "GSMService", "Callback Partner", "Error", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }
    }

    public class DataCallback
    {
        public string RefCode { get; set; }
        public int Status { get; set; }
        public int Amount { get; set; }
        public string Signature { get; set; }

    }

}