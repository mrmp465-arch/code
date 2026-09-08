using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using APIMyViettel.Entity;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APIMyViettel
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
                NLogLogger.Info(new string[] { "GSMCallback", "Get 3rd", serializer.Serialize(messageDb) });
                int amountReal = 0;
                if (messageDb != null)
                {

                    if (messageDb.Status == 0 || messageDb.Status == -326) //Chỉ call 2 trạng thái này
                    {

                        var topupProcess = new TopupMobileLog() { TransactionID = Convert.ToInt64(messageDb.RequestNo) }.Get();
                        NLogLogger.Info(new string[] { "GSMCallback", "Get Order", serializer.Serialize(topupProcess) });

                        //if (!providers.Contains(messageDb.ProviderCode))
                        //{
                        //    context.Response.Write("{\"status\":\"0\",\"message\":\"Thất bại\"}");
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
                                    if (amountReal == 0)
                                    {

                                        if (messageDb.AmountUser == 10000) //Nghiễm nhiên thành công
                                        {
                                            amountReal = 10000;
                                            responseCode = (int)ResponseCode.TransactionSuccessful;
                                            //topupProcess.Topup(0, 0); // mở lại cho chạy
                                            topupProcess.Topup(1, amountReal, messageDb.BidRate); // Thanh công update Amount
                                        }
                                        else
                                        {
                                            //kiểm tra thẻ mệnh giá
                                            //var resCard = MyViettelService.CheckCard(messageDb.CardSerial);
                                            var resCard = WebService.CheckCard(messageDb.CardSerial);
                                            NLogLogger.Info(new string[] { "CheckCard ResponseContent", serializer.Serialize(resCard) });

                                            var tryAgain = 0;
                                            while (tryAgain < 3 && (resCard.ResponseCode == (int)ResponseCode.LoginFail
                                                                    || resCard.ResponseCode ==
                                                                    (int)ResponseCode.TransactionLimit
                                                                    || resCard.ResponseCode ==
                                                                    (int)ResponseCode.AccessDenied
                                                                    || resCard.ResponseCode ==
                                                                    (int)ResponseCode.SystemBusy
                                                                    || resCard.ResponseCode ==
                                                                    (int)ResponseCode.ParameterInvalid))
                                            {
                                                //resCard = MyViettelService.CheckCard(messageDb.CardSerial);
                                                resCard = WebService.CheckCard(messageDb.CardSerial);
                                                tryAgain++;
                                            }


                                            switch (resCard.ResponseCode)
                                            {
                                                case (int)ResponseCode.CardUsed:
                                                    {
                                                        var card = serializer.Deserialize<CheckSerialResponse>(resCard.ResponseContent);
                                                        DateTime timeCheck = DateTime.Now;
                                                        DateTime dateUsed = DateTime.ParseExact(card.dateUsed, "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture);
                                                        var totalsec = (timeCheck - dateUsed).TotalSeconds;

                                                        if (topupProcess.TopupType == 1 || topupProcess.TopupType == 2)
                                                        {
                                                            if (!topupProcess.Mobile.Contains(card.isdn.Replace("xxxx", ""))
                                                            )
                                                            {
                                                                NLogLogger.Info(new string[] { "CheckCard MissTime TT - TS (isdn missing)", totalsec.ToString(), resCard.ResponseContent });
                                                                responseCode = (int)ResponseCode.CardUsed;
                                                                jsonString = jsonString + " -> " + resCard.Description;
                                                                topupProcess.Topup(0, 0);
                                                            }
                                                            else
                                                            {
                                                                if (totalsec > 0 && totalsec <= 90
                                                                ) // && topupProcess.Mobile.Contains(card.isdn.Replace("xxxx", "")))
                                                                {
                                                                    NLogLogger.Info(new string[] { "CheckCard MissTime TT - TS (0s - 90s) TransactionSuccessful", totalsec.ToString(), resCard.ResponseContent });
                                                                    amountReal = Convert.ToInt32(card.cardValue);
                                                                    responseCode = (int)ResponseCode.TransactionSuccessful;
                                                                    //topupProcess.Topup(0, 0); // mở lại cho chạy
                                                                    topupProcess.Topup(1, amountReal,
                                                                        messageDb.BidRate); // Thanh công update Amount
                                                                    jsonString = jsonString + " -> " + resCard.ResponseContent;
                                                                }
                                                                else if (totalsec > 90 && totalsec <= 600)
                                                                {
                                                                    NLogLogger.Info(new string[] { "CheckCard MissTime TT - TS (91s - 600s) TransactionSuspicious", totalsec.ToString(), resCard.ResponseContent });
                                                                    responseCode = (int)ResponseCode.TransactionSuspicious;
                                                                    jsonString = jsonString + " -> " + resCard.Description;
                                                                    topupProcess.Topup(1, messageDb.AmountUser, messageDb.BidRate); //Cộng đơn
                                                                    topupProcess.Topup(-6, 0); // Stop đơn để review
                                                                    TelegramNotify.SendNotify(messageDb.ProviderCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, -2, messageDb.CardSerial);
                                                                }
                                                                else
                                                                {
                                                                    NLogLogger.Info(new string[] { "CheckCard MissTime TT - TS (601s - n/a) CardUsed", totalsec.ToString(), resCard.ResponseContent });
                                                                    responseCode = (int)ResponseCode.CardUsed;
                                                                    jsonString = jsonString + " -> " + resCard.Description;
                                                                    topupProcess.Topup(0, 0);
                                                                }
                                                            }

                                                        }
                                                        else
                                                        {
                                                            if (totalsec > 0 && totalsec <= 90
                                                            ) // && topupProcess.Mobile.Contains(card.isdn.Replace("xxxx", "")))
                                                            {
                                                                NLogLogger.Info(new string[] { "CheckCard MissTime (0s - 90s) TransactionSuccessful", totalsec.ToString(), resCard.ResponseContent });
                                                                amountReal = Convert.ToInt32(card.cardValue);
                                                                responseCode = (int)ResponseCode.TransactionSuccessful;
                                                                //topupProcess.Topup(0, 0); // mở lại cho chạy
                                                                topupProcess.Topup(1, amountReal, messageDb.BidRate); // Thanh công update Amount
                                                                jsonString = jsonString + " -> " + resCard.ResponseContent;
                                                            }
                                                            else if (totalsec > 90 && totalsec <= 600)
                                                            {
                                                                NLogLogger.Info(new string[] { "CheckCard MissTime (91s - 600s) TransactionSuspicious", totalsec.ToString(), resCard.ResponseContent });
                                                                responseCode = (int)ResponseCode.TransactionSuspicious;
                                                                jsonString = jsonString + " -> " + resCard.Description;
                                                                topupProcess.Topup(1, messageDb.AmountUser, messageDb.BidRate); //Cộng đơn
                                                                topupProcess.Topup(-6, 0); // Stop đơn để review
                                                                TelegramNotify.SendNotify(messageDb.ProviderCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, -2, messageDb.CardSerial);
                                                            }
                                                            else
                                                            {
                                                                NLogLogger.Info(new string[] { "CheckCard MissTime (601s - n/a) CardUsed", totalsec.ToString(), resCard.ResponseContent });
                                                                responseCode = (int)ResponseCode.CardSerialInvalid;
                                                                jsonString = jsonString + " -> " + resCard.Description;
                                                                topupProcess.Topup(0, 0);
                                                            }
                                                        }

                                                        break;
                                                    }

                                                case (int)ResponseCode.CardNotActivated:
                                                    responseCode = (int)ResponseCode.CardNotActivated;
                                                    topupProcess.Topup(0, 0); // mở lại cho chạy
                                                    jsonString = jsonString + " -> " + resCard.Description;
                                                    break;

                                                case (int)ResponseCode.CardSerialInvalid:
                                                    responseCode = (int)ResponseCode.CardSerialInvalid;
                                                    topupProcess.Topup(0, 0); // mở lại cho chạy
                                                    jsonString = jsonString + " -> " + resCard.Description;
                                                    break;

                                                case (int)ResponseCode.TransactionLimit:
                                                    responseCode = (int)ResponseCode.TransactionSuspicious;
                                                    topupProcess.Topup(1, messageDb.AmountUser, messageDb.BidRate);
                                                    topupProcess.Topup(-6, 0); // Stop đơn để review
                                                    jsonString = jsonString + " -> " + resCard.Description;
                                                    break;

                                                default:
                                                    responseCode = (int)ResponseCode.TransactionSuspicious;
                                                    topupProcess.Topup(1, messageDb.AmountUser, messageDb.BidRate);
                                                    topupProcess.Topup(-6, 0); // Stop đơn để review
                                                    jsonString = jsonString + " -> " + resCard.Description;
                                                    break;
                                            }

                                        }

                                    }
                                }
                                else
                                {
                                    responseCode = (int)ResponseCode.TransactionSuccessful;
                                    //topupProcess.Topup(0, 0); // mở lại cho chạy
                                    topupProcess.Topup(1, amountReal, messageDb.BidRate); // Thanh công update Amount
                                }

                                //Callback for Provider
                                if (!string.IsNullOrEmpty(topupProcess.CallbackUrl))
                                {
                                    if (amountReal > 0)
                                    {
                                        var callbackData = new DataCallbackOrder()
                                        {
                                            OrderId = topupProcess.TransactionID,
                                            Amount = amountReal,
                                            Status = 1,
                                            CardSerial = messageDb.CardSerial,
                                            CardCode = messageDb.CardCode,
                                            BidRate = messageDb.BidRate,
                                            UpdateTime = DateTime.Now,
                                            CreatTime = messageDb.CreateTime,
                                            Signature = string.Empty
                                        };
                                        Task.Run(async () => await new MyViettelBiz().CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.UserName, messageDb.Id, 1).ConfigureAwait(false));
                                    }
                                }

                                //topupProcess.Topup(0, 0); // mở lại cho chạy

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
                                //topupProcess.Topup(0, 0); // mở lại cho chạy
                                topupProcess.Topup(1, messageDb.AmountUser, messageDb.BidRate);
                                topupProcess.Topup(-6, 0); // Stop đơn để review
                                //Callback for Provider
                                if (!string.IsNullOrEmpty(topupProcess.CallbackUrl))
                                {
                                    if (amountReal > 0)
                                    {
                                        var callbackData = new DataCallbackOrder()
                                        {
                                            OrderId = topupProcess.TransactionID,
                                            Amount = 0,
                                            Status = (int)ResponseCode.TransactionReview,
                                            CardSerial = messageDb.CardSerial,
                                            CardCode = messageDb.CardCode,
                                            BidRate = messageDb.BidRate,
                                            UpdateTime = DateTime.Now,
                                            CreatTime = messageDb.CreateTime,
                                            Signature = string.Empty
                                        };
                                        Task.Run(async () => await new MyViettelBiz().CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.UserName, messageDb.Id, 1).ConfigureAwait(false));
                                    }
                                }

                                TelegramNotify.SendNotify(messageDb.ProviderCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, -2, messageDb.CardSerial);
                                responseCode = (int)ResponseCode.TransactionSuspicious;
                                break;
                            case (int)ResponseCode.TransactionLimit:
                                topupProcess.Topup(0, 0); // mở lại cho chạy
                                responseCode = (int)ResponseCode.TransactionLimit;
                                break;
                            case (int)ResponseCode.ParameterInvalid: //USSD terminated by network
                                topupProcess.Topup(0, 0); // mở lại cho chạy
                                responseCode = (int)ResponseCode.ParameterInvalid;
                                break;
                            case (int)ResponseCode.ServiceIsLocked:
                                topupProcess.Topup(-2, 0); // khóa đơn
                                //Callback for Provider
                                if (!string.IsNullOrEmpty(topupProcess.CallbackUrl))
                                {
                                    if (amountReal > 0)
                                    {
                                        var callbackData = new DataCallbackOrder()
                                        {
                                            OrderId = topupProcess.TransactionID,
                                            Amount = 0,
                                            Status = (int)ResponseCode.ServiceIsLocked,
                                            CardSerial = messageDb.CardSerial,
                                            CardCode = messageDb.CardCode,
                                            BidRate = messageDb.BidRate,
                                            UpdateTime = DateTime.Now,
                                            CreatTime = messageDb.CreateTime,
                                            Signature = string.Empty
                                        };
                                        Task.Run(async () => await new MyViettelBiz().CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.UserName, messageDb.Id, 1).ConfigureAwait(false));
                                    }
                                }

                                TelegramNotify.SendNotify(messageDb.ProviderCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, 2);
                                responseCode = (int)ResponseCode.ServiceIsLocked;
                                break;
                            case (int)ResponseCode.SystemBusy: //Telco Busy có thể nuốt thẻ
                                //topupProcess.Topup(0, 0); // mở lại cho chạy
                                topupProcess.Topup(1, messageDb.AmountUser, messageDb.BidRate);
                                responseCode = (int)ResponseCode.TransactionSuspicious;
                                break;
                            case (int)ResponseCode.AccessDenied:
                                topupProcess.Topup(-2, 0); // khóa đơn
                                TelegramNotify.SendNotify(messageDb.ProviderCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, 2);
                                responseCode = (int)ResponseCode.ServiceIsLocked;
                                break;
                            default:
                                topupProcess.Topup(0, 0);
                                responseCode = (int)ResponseCode.TransactionFailed;
                                break;
                        }


                        var resultUpdate = DataRequest.UpdateTopupCard(Convert.ToInt64(resObj.RefCode), amountReal,
                            responseCode, jsonString);

                        if (resultUpdate == 0)
                        {
                            var cardAPILog = new CardAPILog().Get(Convert.ToInt32(messageDb.TransactionId));

                            if (cardAPILog.Status == (int)ResponseCode.TransactionTimeout)
                            {
                                cardAPILog.Amount = amountReal;
                                cardAPILog.Description = "Callback " + responseCode + " " + amountReal;
                                cardAPILog.Status = responseCode;
                                cardAPILog.Update();

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

                                var privateKey = new Partners().Get(messageDb.PartnerCode).PrivateKey;
                                var datacb = new DataCallback()
                                {
                                    Amount = amountReal,
                                    RefCode = cardAPILog.RequestNo,
                                    Status = responseCode,
                                    Signature = Libs.Utils.Encrypts.MD5(cardAPILog.RequestNo + responseCode + amountReal + privateKey)
                                };

                                //Callback for partner
                                if (!string.IsNullOrEmpty(cardAPILog.CallbackUrl))
                                {
                                    Task.Run(async () => await new MyViettelBiz().CallbackJson(cardAPILog.CallbackUrl, serializer.Serialize(datacb), cardAPILog.PartnerCode, 0, 2).ConfigureAwait(false));
                                }
                                else
                                {
                                    NLogLogger.Info(new string[] { "GSMService", "Callback Partner", "Request", cardAPILog.PartnerCode + " " + messageDb.TransactionId, "Url Empty" });
                                }


                            }


                            //if (topupProcess.TopupType == 1) //Nếu là trả trước thay luôn
                            //{
                            //    NLogLogger.Info(new string[] { "GSMService", "Thay Sim tránh % nạp hộ trả trước", topupProcess.Mobile, topupProcess.TopupType.ToString() });
                            //    result = "{\"status\":\"2\",\"message\":\"Thay sim\"}";
                            //}
                            //else //Tra sau tự GSM Count đếm
                            //{
                            //    result = "{\"status\":\"1\",\"message\":\"Đã nhận\"}";
                            //}

                            result = "{\"status\":\"1\",\"message\":\"Đã nhận\"}";

                        }
                        else
                        {
                            NLogLogger.Info(new string[] { "GSMService", "UpdateTopupCard Failed", resObj.RefCode });
                            result = "{\"status\":\"0\",\"message\":\"Thất bại\"}";
                        }
                    }
                    else
                    {
                        context.Response.Write("{\"status\":\"0\",\"message\":\"Thất bại (Callback duplicate)\"}");
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

        //public async Task<string> CallbackJson(string url, string postData, string code)
        //{
        //    NLogLogger.Info(new string[] { "GSMService", "Callback JSON", "Request", code, url, postData });
        //    try
        //    {
        //        var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
        //        using (var client = new HttpClient())
        //        {
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            var response = await client.PostAsync(url, httpContent).ConfigureAwait(false);

        //            if (response.Content != null)
        //            {
        //                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        //                NLogLogger.Info(new string[] { "GSMService", "Callback Partner", "Response", code, url, postData, responseContent });
        //                return responseContent;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        NLogLogger.Info(new string[] { "GSMService", "Callback JSON", "Error", code, url, postData, e.Message });
        //        return string.Empty;
        //    }

        //    return string.Empty;

        //}
    }

    public class DataCallback
    {
        public string RefCode { get; set; }
        public int Status { get; set; }
        public int Amount { get; set; }
        public string Massage { get; set; }
        public string Signature { get; set; }

    }

}