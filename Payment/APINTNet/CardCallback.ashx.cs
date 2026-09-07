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
using APINTNet.Entity;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APINTNet
{
    /// <summary>
    /// Summary description for VPGJsonService
    /// </summary>
    public class CardCallback : IHttpHandler
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {

            context.Request.ContentType = "application/json";
            context.Response.ContentType = "application/json";
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            var jsonString = String.Empty;
            var result = string.Empty;
            context.Request.InputStream.Position = 0;

            string[] providers = { "ntnetvms", "ntnetvnp", "ntnetvtt" };

            try
            {
                using (var inputStream = new StreamReader(context.Request.InputStream))
                {
                    jsonString = inputStream.ReadToEnd();
                }

                NLogLogger.Info(new string[] { "NTNet", "Callback", jsonString });
                if (string.IsNullOrEmpty(jsonString))
                {
                    context.Response.Write("{\"status\":\"0\",\"message\":\"Thất bại\"}");
                    return;
                }
                var resObj = javaScriptSerializer.Deserialize<CallbackResponse>(jsonString);

                var messageDb = DataRequest.GetTopupCardLog(resObj.refcode);
                int amountReal = 0;
                if (messageDb != null)
                {

                    if (!providers.Contains(messageDb.ProviderCode))
                    {
                        context.Response.Write("{\"status\":\"0\",\"message\":\"Thất bại\"}");
                        return;
                    }

                    var responseCode = (int)ResponseCode.UndefinedError;
                    switch (resObj.status)
                    {
                        case "2":
                        case "3":
                            responseCode = (int)ResponseCode.TransactionSuccessful;
                            amountReal = resObj.amount;
                            //if (messageDb.AmountUser != Convert.ToInt32(resObj.amount))
                            //{
                            //    responseCode = (int)ResponseCode.CardAmountInvalid;
                            //}
                            break;
                        case "4":
                            responseCode = (int)ResponseCode.CardUsed;
                            break;
                        case "96":
                            responseCode = (int)ResponseCode.CardSerialInvalid;
                            break;
                        default:
                            responseCode = (int)ResponseCode.TransactionFailed;
                            break;
                    }

                    var resultUpdate = DataRequest.UpdateTopupCard(Convert.ToInt64(resObj.refcode), amountReal, responseCode, jsonString, string.Empty);

                    if (resultUpdate == 0)
                    {

                        if (messageDb.Status != (int)ResponseCode.TransactionSuccessful && (DateTime.Now - messageDb.CreateTime).TotalSeconds >= 90)
                        {
                            var cardAPILog = new CardAPILog().Get(Convert.ToInt32(messageDb.TransactionId));
                            cardAPILog.Amount = amountReal;
                            cardAPILog.Description = "Callback " + responseCode + " " + amountReal;
                            cardAPILog.Status = responseCode;
                            //cardAPILog.Update();
                            cardAPILog.AccountID = 0;
                            if (responseCode > 0)
                            {
                                var ck = GetCK(cardAPILog.PartnerCode, cardAPILog.CardType);
                                cardAPILog.AccountID = Convert.ToInt64(amountReal * ck);
                            }

                            //Begin Callback for partner
                            if (responseCode == (int)ResponseCode.TransactionSuccessful)
                            {
                                if (cardAPILog.AmountUser != amountReal)
                                {
                                    responseCode = (int)ResponseCode.CardAmountInvalid;
                                    cardAPILog.Status = (int)ResponseCode.TransactionSuccessful;
                                    //if (cardAPILog.PartnerCode == "huv")
                                    //{
                                    //    if (cardAPILog.AmountUser < amountReal)
                                    //    {
                                    //        TelegramNotify.SendWarning("583426534", $"CẢNH BÁO sai mệnh giá: {cardAPILog.CardType} TranId: {cardAPILog.TransactionID} , RefCode: {cardAPILog.RequestNo} , CardSerial: {cardAPILog.CardSerial},  Mệnh giá {cardAPILog.AmountUser}/amountReal, Partner: {cardAPILog.PartnerCode}");
                                    //        TelegramNotify.SendWarning("1497473671", $"CẢNH BÁO sai mệnh giá: {cardAPILog.CardType} TranId: {cardAPILog.TransactionID} , RefCode: {cardAPILog.RequestNo} , CardSerial: {cardAPILog.CardSerial},  Mệnh giá {cardAPILog.AmountUser}/amountReal, Partner: {cardAPILog.PartnerCode}");
                                    //    }
                                    //}
                                }
                                //NLogLogger.Info(new string[] { "CardTelco Topup", transaction.PartnerCode, result.ResponseContent, request.CardType.ToLower() });

                              
                                //tính tiền ở đây

                            }
                            else
                            {
                                responseCode = (int)ResponseCode.TransactionFailed;
                            }

                            cardAPILog.Update();


                            Partners _Partner = new Partners().GetCache(messageDb.PartnerCode);
                            var privateKey = _Partner.PrivateKey;
                            var datacb = new DataCallback()
                            {
                                Amount = amountReal,
                                RefCode = cardAPILog.RequestNo,
                                Status = responseCode,
                                Signature = Libs.Utils.Encrypts.MD5(cardAPILog.RequestNo + responseCode + amountReal + privateKey)
                            };
                            if (cardAPILog.Status == 1)
                            {
                                var Amount = Math.Min(cardAPILog.AmountUser, Convert.ToInt64(amountReal));
                                Action<string, long, string, string, string> send = UpdatePartnerBalance;
                                var asynSend = send.BeginInvoke(_Partner.PartnerCode, Amount, cardAPILog.CardType.ToLower(), String.Format("Cộng tiền nạp thẻ {4} mgd: {0}-{1}-{2}-{3}", cardAPILog.TransactionID, cardAPILog.CardType, cardAPILog.CardSerial, cardAPILog.CardCode, Amount.ToString("#,#").Replace(",", ".")), "Card_" + cardAPILog.TransactionID, null, null);
                                /// _CardAPILog.Status = (int) ResponseCode.TransactionSuccessful;

                            }
                            if (!string.IsNullOrEmpty(cardAPILog.CallbackUrl))
                            {
                                Task.Run(async () => await CallbackJson(cardAPILog.CallbackUrl, serializer.Serialize(datacb), cardAPILog.PartnerCode + " " + messageDb.Id).ConfigureAwait(false));
                                NLogLogger.Info(new string[] { "NTNet", "Callback Partner", messageDb.TransactionId.ToString() });
                            }


                            //End Callback for partner
                        }

                        result = "{\"status\":\"1\",\"message\":\"Đã nhận\"}";

                    }
                    else
                    {
                        result = "{\"status\":\"0\",\"message\":\"Thất bại\"}";
                    }
                }



            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "NTNet", "Callback", "Error", exp.Message });
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
        private decimal  GetCK(string PartnerCode, string CardType )
        {
            try
            {
               
                var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, DateTime.Now.Year, DateTime.Now.Month);
                if (listpartnerDiscount == null)
                {
                    //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return 0;
                }

                if (!listpartnerDiscount.Exists(x => x.Date.Day == DateTime.Now.Day))
                    return 0;

                var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == DateTime.Now.Day);
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
                    case "gate":
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
                var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, DateTime.Now.Year, DateTime.Now.Month);
                if (listpartnerDiscount == null)
                {
                    //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return;
                }

                if (!listpartnerDiscount.Exists(x => x.Date.Day == DateTime.Now.Day))
                    return;

                var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == DateTime.Now.Day);
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
                    case "gate":
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
        public async Task<string> CallbackJson(string url, string postData, string code)
        {
            NLogLogger.Info(new string[] { "NTNet", "Callback Partner", "Request", code, url, postData });

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
                        NLogLogger.Info(new string[] { "NTNet", "Callback Partner", "Response", code, url, postData, responseContent });
                        return responseContent;
                    }
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "NTNet", "Callback Partner", "Error", e.Message });
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