using System;
using System.CodeDom;
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
using APIVinaPay.Entity;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APIVinaPay
{
    /// <summary>
    /// Summary description for VPGJsonService
    /// </summary>
    public class CardCallbackV2 : IHttpHandler
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const int ClientID = 20;

        private const string SecretKey = "5Vc167ztb8XtfJhf5KpqemigaCZpSQ8ZPik1p8gfpDx1oEbGEegNk15CieRgWh7k";
        public void ProcessRequest(HttpContext context)
        {

            context.Request.ContentType = "application/json";
            context.Response.ContentType = "text/plain";
            var jsonString = String.Empty;
            var result = string.Empty;
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            using (var inputStream = new StreamReader(context.Request.InputStream))
            {
                jsonString = inputStream.ReadToEnd();
            }
            NLogLogger.Info(new string[] { "Lion2", "Callback", jsonString });
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            var resObj = javaScriptSerializer.Deserialize<DataCallback>(jsonString);
            //var resObj = new PostGetHelper().GetFromQueryString<Callback>();
            NLogLogger.Info(new string[] { "APIBB2D", "Callback", serializer.Serialize(resObj) });

            if (GlobalHelper.IsAnyNullOrEmpty(resObj))
            {
                context.Response.Write("-99|Data empty");
                return;
            }

            
            try
            {

                var messageDb = DataRequest.GetTopupCardLog(resObj.RefCode);
                //NLogLogger.Info(new string[] { "APIVinaPays", "messageDb", serializer.Serialize(messageDb) });
                if (messageDb != null)
                {
                    var responseCode = (int)ResponseCode.UndefinedError;
                    switch (resObj.Status)
                    {
                        case 1:
                        case 2:
                            responseCode = (int)ResponseCode.TransactionSuccessful;

                            break;
                        case -330:
                            responseCode = (int)ResponseCode.CardUsed;
                            break;
                        default:
                            responseCode = (int)ResponseCode.TransactionFailed;


                            break;
                    }
                    var amount = Math.Min(resObj.Amount, resObj.Amount);
                    var resultUpdate = DataRequest.UpdateTopupCard(Convert.ToInt64(resObj.RefCode), Convert.ToInt32(amount), responseCode, serializer.Serialize(resObj), string.Empty);

                    if (resultUpdate == 0)
                    {

                        if (messageDb.Status != (int)ResponseCode.TransactionSuccessful)
                        {
                            var cardAPILog = new CardAPILog().Get(Convert.ToInt32(messageDb.TransactionId));
                            cardAPILog.Amount = Convert.ToInt32(resObj.Amount);
                            cardAPILog.Description = "Callback " + responseCode + " " + amount;
                            cardAPILog.Status = responseCode;
                            cardAPILog.AccountID = 0;
                            if (responseCode > 0)
                            {
                                var ck = GetCK(cardAPILog.PartnerCode, cardAPILog.CardType);
                                var rw = GetRW(cardAPILog.PartnerCode, cardAPILog.CardType);
                                var feeProvider = getfeeProvider(cardAPILog.CardType);
                                cardAPILog.Fee = Convert.ToInt64(amount * ck);
                                cardAPILog.Reward = Convert.ToInt32(amount * rw);
                                cardAPILog.FeeProvider = Convert.ToInt32(amount * feeProvider);
                            }
                            //cardAPILog.Update();



                            //Begin Callback for partner
                            if (responseCode == (int)ResponseCode.TransactionSuccessful)
                            {
                                if (cardAPILog.AmountUser != Convert.ToInt32(amount))
                                {
                                    responseCode = (int)ResponseCode.TransactionSuccessful;
                                    cardAPILog.Status = (int)ResponseCode.TransactionSuccessful;
                                }
                                //NLogLogger.Info(new string[] { "CardTelco Topup", transaction.PartnerCode, result.ResponseContent, request.CardType.ToLower() });

                                //Action<string, long, string> send = UpdatePartnerBalance;
                                //var asynSend = send.BeginInvoke(cardAPILog.PartnerCode, Math.Min(cardAPILog.Amount, cardAPILog.AmountUser), cardAPILog.CardType.ToLower(), null, null);

                            }
                            //else
                            //{
                            //    responseCode = (int)ResponseCode.TransactionFailed;
                            //}

                            cardAPILog.Update();
                            Partners _Partner = new Partners().GetCache(messageDb.PartnerCode);

                            if (cardAPILog.Status == 1)
                            {

                                Action<string, long, string, string, string> send = UpdatePartnerBalance;
                                var asynSend = send.BeginInvoke(_Partner.PartnerCode, amount, cardAPILog.CardType.ToLower(), String.Format("Topup to recharge card {4} transId: {0}-{1}-{2}-{3}", cardAPILog.TransactionID, cardAPILog.CardType, cardAPILog.CardSerial, cardAPILog.CardCode, amount.ToString("#,#").Replace(",", ".")), "Card_" + cardAPILog.TransactionID, null, null);
                                /// _CardAPILog.Status = (int) ResponseCode.TransactionSuccessful;
                                /// 
                                //if (!string.IsNullOrEmpty(_Partner.SMSCommand))
                                //{
                                //    var rw = GetRW(_Partner.PartnerCode, cardAPILog.CardType);
                                //    if (rw > 0)
                                //    {
                                //        var TotalR = Convert.ToInt64(amount * rw);
                                //        var Balancedesc2 = String.Format("Cộng tiền hoa hồng nạp thẻ đối tác {4} số tiền: {3} mgd: {0}-{1}-{2}", cardAPILog.TransactionID, cardAPILog.CardType, cardAPILog.CardSerial + "-" + cardAPILog.CardCode, Convert.ToInt64(amount).ToString("#,#").Replace(",", "."), _Partner.PartnerCode);
                                //        UpdatePartnerBalanceReward(_Partner.SMSCommand, TotalR, Balancedesc2, "RCardIn_" + cardAPILog.TransactionID.ToString());
                                //    }
                                //}

                            }

                            var privateKey = _Partner.PrivateKey;
                            var datacb = new DataCallback()
                            {
                                Amount = Convert.ToInt32(amount),
                                RefCode = cardAPILog.RequestNo,
                                Status = responseCode,
                                Signature = Libs.Utils.Encrypts.MD5(cardAPILog.RequestNo + responseCode + amount + privateKey)
                            };

                            if (!string.IsNullOrEmpty(cardAPILog.CallbackUrl))
                                Task.Run(async () => await CallbackJsonV2(cardAPILog.CallbackUrl, serializer.Serialize(datacb), cardAPILog.TransactionID, cardAPILog.PartnerCode + " " + messageDb.Id).ConfigureAwait(false));
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
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "APIVinaPays", "Callback", "Error", exp.Message });
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
        private void UpdatePartnerBalanceReward(string PartnerCode, long realAmount, string TranId, string RefCode)
        {
            try
            {
                NLogLogger.Info(new string[] { "Update Balance Reward", PartnerCode, realAmount.ToString(), TranId.ToString(), RefCode });

                if (realAmount == 0)
                {
                    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return;
                }

                //long realAmount = Amount - fee;
                // NLogLogger.Info(new string[] { "Bank Topup", realAmount.ToString(), ck.ToString() });
                new Users().Topup(realAmount, PartnerCode, PartnerCode, TranId, RefCode);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
            }


        }
        private void UpdatePartnerBalance(string PartnerCode, long Amount, string CardType, string Note, string RefCode)
        {
            try
            {
                NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), CardType, Note });
                var partner = new Partners().GetCache(PartnerCode);
                if (string.IsNullOrEmpty(partner.Hotline))
                {
                    //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                    return;
                }
                var user = new Users().GetByUserName(partner.Hotline.Trim());
                if (user == null)
                {
                    //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                    return;
                }
                var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, 2030, 1);
                if (listpartnerDiscount == null)
                {
                    //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return;
                }

                if (!listpartnerDiscount.Exists(x => x.Date.Day == 1))
                    return;

                var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == 1);
                decimal ck = 0;
                switch (CardType)
                {
                    case "vms":
                        ck = _partnerDiscount.DiscountVMS;
                        break;
                    case "vnp":
                        ck = _partnerDiscount.DiscountVNP;
                        break;
                    case "viettel":
                        ck = _partnerDiscount.DiscountVTT;
                        break;
                    case "zing":
                        ck = _partnerDiscount.DiscountZING;
                        break;
                    case "vcoin":
                        ck = _partnerDiscount.DiscountGATE;
                        break;
                }
                if (ck == 0)
                    return;

                long realAmount = Amount - Convert.ToInt64(Amount * ck);
                //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
                new Users().Topup(realAmount, user.UserName, PartnerCode, Note, RefCode);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
            }
        }
        private decimal GetCK(string PartnerCode, string CardType)
        {
            try
            {

                var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, 2030, 1);
                if (listpartnerDiscount == null)
                {
                    //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return 0;
                }

                if (!listpartnerDiscount.Exists(x => x.Date.Day == 1))
                    return 0;

                var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == 1);
                decimal ck = 0;
                switch (CardType)
                {
                    case "vms":
                        ck = _partnerDiscount.DiscountVMS;
                        break;
                    case "vnp":
                        ck = _partnerDiscount.DiscountVNP;
                        break;
                    case "viettel":
                        ck = _partnerDiscount.DiscountVTT;
                        break;
                    case "zing":
                        ck = _partnerDiscount.DiscountZING;
                        break;
                    case "vcoin":
                        ck = _partnerDiscount.DiscountGATE;
                        break;
                }
                return ck;


            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        private decimal GetRW(string PartnerCode, string CardType)
        {
            try
            {

                var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, 2030, 1);
                if (listpartnerDiscount == null)
                {
                    //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return 0;
                }

                if (!listpartnerDiscount.Exists(x => x.Date.Day == 1))
                    return 0;

                var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == 1);
                decimal ck = 0;
                switch (CardType)
                {
                    case "vms":
                        ck = _partnerDiscount.RewardVMS;
                        break;
                    case "vnp":
                        ck = _partnerDiscount.RewardVNP;
                        break;
                    case "viettel":
                        ck = _partnerDiscount.RewardVTT;
                        break;
                    case "zing":
                        ck = _partnerDiscount.RewardZING;
                        break;
                    case "vcoin":
                        ck = _partnerDiscount.RewardGATE;
                        break;
                }
                return ck;


            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        private decimal getfeeProvider(string Type)
        {
            decimal ck = 0;
            switch (Type)
            {
                case "vms":
                    ck = 17 / 100;
                    break;
                case "vnp":
                    ck = 17 / 100;
                    break;
                case "viettel":
                    ck = 17 / 100;
                    break;
                case "zing":
                    ck = 17 / 100;
                    break;
                case "vcoin":
                    ck = 17 / 100;
                    break;

            }
            return ck;
        }
        public async Task<string> CallbackJson(string url, string postData, string code)
        {
            NLogLogger.Info(new string[] { "APIVinaPay", "Callback Partner", "Request", code, url, postData });

            try
            {
                var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.PostAsync(url, httpContent).ConfigureAwait(false);

                    if (response.Content != null)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        NLogLogger.Info(new string[] { "APIVinaPay", "Callback Partner", "Response", code, url, postData, responseContent });
                        return responseContent;
                    }
                }

            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "APIVinaPay", "Callback Partner", "Error", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }
        public static async Task<string> CallbackJsonV2(string url, string postData, long Id = 0, string refcode = "", int maxRetry = 3)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            HttpClient client = null;

            // Retry delays: retry #1=30s, retry #2=5m, retry #3=10m
            var retryDelays = new[]
            {
                TimeSpan.FromSeconds(60),
                TimeSpan.FromMinutes(5),
                TimeSpan.FromMinutes(10)
            };

            try
            {
                client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
                client.Timeout = TimeSpan.FromSeconds(90);

                // Tổng số lần gọi = 1 (lần đầu) + maxRetry (số lần retry)
                for (int attempt = 0; attempt <= maxRetry; attempt++)
                {
                    try
                    {
                        var attemptNo = (attempt + 1).ToString(); // để log dễ đọc (1..)

                        NLogLogger.Info(new[] { "MDrum", "Callback", "Attempt", attemptNo, "Transid", Id.ToString(), "Refcode", refcode, "Url", url });

                        using (var httpContent = new StringContent(postData ?? "", Encoding.UTF8, "application/json"))
                        {
                            var response = await client.PostAsync(url, httpContent).ConfigureAwait(false);

                            var responseContent = response.Content != null
                                ? await response.Content.ReadAsStringAsync().ConfigureAwait(false)
                                : string.Empty;

                            LogCache.LogCard(new LogInfo
                            {
                                LogTime = DateTime.Now,
                                Url = url,
                                TransactionID = Id,
                                Request = postData,
                                Respone = "HTTP " + ((int)response.StatusCode) + " " + response.ReasonPhrase + " | " + responseContent
                            });

                            if ((int)response.StatusCode == 200)
                                return responseContent;

                            NLogLogger.Info(new[] { "MDrum", "Callback", "StatusNot200", "Attempt", attemptNo, "Status", ((int)response.StatusCode).ToString(), responseContent });
                        }
                    }
                    catch (TaskCanceledException ex)
                    {
                        var attemptNo = (attempt + 1).ToString();

                        NLogLogger.Info(new[] { "MDrum", "Callback", "Timeout", "Attempt", attemptNo, "Transid", Id.ToString(), "Refcode", refcode, ex.Message });

                        LogCache.LogCard(new LogInfo
                        {
                            LogTime = DateTime.Now,
                            Url = url,
                            TransactionID = Id,
                            Request = postData,
                            Respone = "Timeout: " + ex.ToString()
                        });
                    }
                    catch (HttpRequestException ex)
                    {
                        var attemptNo = (attempt + 1).ToString();

                        NLogLogger.Info(new[] { "MDrum", "Callback", "HttpRequestException", "Attempt", attemptNo, "Transid", Id.ToString(), "Refcode", refcode, ex.Message });

                        LogCache.LogCard(new LogInfo
                        {
                            LogTime = DateTime.Now,
                            Url = url,
                            TransactionID = Id,
                            Request = postData,
                            Respone = "HttpRequestException: " + ex.ToString()
                        });
                    }
                    catch (Exception ex)
                    {
                        var attemptNo = (attempt + 1).ToString();

                        NLogLogger.Info(new[] { "MDrum", "Callback", "Exception", "Attempt", attemptNo, "Transid", Id.ToString(), "Refcode", refcode, ex.Message });

                        LogCache.LogCard(new LogInfo
                        {
                            LogTime = DateTime.Now,
                            Url = url,
                            TransactionID = Id,
                            Request = postData,
                            Respone = "Exception: " + ex.ToString()
                        });

                        return string.Empty; // lỗi không retry tiếp (theo logic cũ của bạn)
                    }

                    // Nếu đã hết lượt (lần cuối) thì dừng
                    if (attempt == maxRetry)
                        break;

                    // Delay theo lịch: retry #1=30s, #2=5m, #3=10m
                    var delayIndex = attempt; // attempt=0 -> delay[0] (30s), attempt=1 -> delay[1] (5m), attempt=2 -> delay[2] (10m)
                    var delay = retryDelays[Math.Min(delayIndex, retryDelays.Length - 1)];

                    NLogLogger.Info(new[] { "MDrum", "Callback", "DelayBeforeRetry", delay.ToString(), "AttemptNext", (attempt + 2).ToString(), "Transid", Id.ToString(), "Refcode", refcode });

                    await Task.Delay(delay).ConfigureAwait(false);
                }

                return string.Empty;
            }
            finally
            {
                if (client != null)
                    client.Dispose();
            }
        }
    }



}