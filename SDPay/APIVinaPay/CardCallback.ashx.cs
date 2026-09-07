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
using APIVinaPay.Entity;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APIVinaPay
{
    /// <summary>
    /// Summary description for VPGJsonService
    /// </summary>
    public class CardCallback : IHttpHandler
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const int ClientID = 20;

        private const string SecretKey = "AUcajG1Z1L2Q5Y43foh51VmaFvygKZaVS8PmmggEyVNPWKhghQPYWgOPyL5NtdQQ";
        public void ProcessRequest(HttpContext context)
        {

            context.Response.ContentType = "text/plain";
            //string transaction_id = context.Request.QueryString["transaction_id"];
            //string status = context.Request.QueryString["status"];
            //string value = context.Request.QueryString["value"];
            //string real_value = context.Request.QueryString["real_value"];
            //string received_value = context.Request.QueryString["received_value"];
            //string card_seri = context.Request.QueryString["card_seri"];
            //string card_code = context.Request.QueryString["card_code"];
            //string refcode = context.Request.QueryString["refcode"];
            //string sign = context.Request.QueryString["sign"];


            var resObj = new PostGetHelper().GetFromQueryString<Callback>();
            NLogLogger.Info(new string[] { "APIVinaPays", "Callback", serializer.Serialize(resObj) });

            if (GlobalHelper.IsAnyNullOrEmpty(resObj))
            {
                context.Response.Write("-99|Data empty");
                return;
            }


            var signature = Encrypts.MD5(String.Format("{0}|{1}|{2}", SecretKey, resObj.CardCode, resObj.CardSeri));

            if (resObj.Signature != signature)
            {
                NLogLogger.Info(new string[] { "APIVinaPays", resObj.Signature, signature });
                context.Response.Write("-313|Signature invalid");

            }

            var result = string.Empty;

            try
            {

                var messageDb = DataRequest.GetTopupCardLog(resObj.TransID);
                //NLogLogger.Info(new string[] { "APIVinaPays", "messageDb", serializer.Serialize(messageDb) });
                if (messageDb != null)
                {
                    var responseCode = (int)ResponseCode.UndefinedError;
                    switch (resObj.Status)
                    {
                        case 0:
                        case -100:
                            responseCode = (int)ResponseCode.TransactionSuccessful;
                            break;

                        default:
                            responseCode = (int)ResponseCode.TransactionFailed;


                            break;
                    }
                    var amount = Math.Min(resObj.Amount, resObj.ReadAmount);
                    var resultUpdate = DataRequest.UpdateTopupCard(Convert.ToInt64(resObj.TransID), Convert.ToInt32(amount), responseCode, serializer.Serialize(resObj), string.Empty);

                    if (resultUpdate == 0)
                    {

                        if (messageDb.Status != (int)ResponseCode.TransactionSuccessful && (DateTime.Now - messageDb.CreateTime).TotalSeconds >= 90)
                        {
                            var cardAPILog = new CardAPILog().Get(Convert.ToInt32(messageDb.TransactionId));
                            cardAPILog.Amount = Convert.ToInt32(amount);
                            cardAPILog.Description = "Callback " + responseCode + " " + amount;
                            cardAPILog.Status = responseCode;
                            //cardAPILog.Update();
                            cardAPILog.AccountID = 0;
                            if (responseCode > 0)
                            {
                                var ck = GetCK(cardAPILog.PartnerCode, cardAPILog.CardType);
                                cardAPILog.AccountID = Convert.ToInt64(amount * ck);
                            }



                            //Begin Callback for partner
                            if (responseCode == (int)ResponseCode.TransactionSuccessful)
                            {
                                if (cardAPILog.AmountUser != Convert.ToInt32(amount))
                                {
                                    responseCode = (int)ResponseCode.CardAmountInvalid;
                                    cardAPILog.Status = (int)ResponseCode.TransactionSuccessful;
                                }
                              
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
                                Amount = Convert.ToInt32(amount),
                                RefCode = cardAPILog.RequestNo,
                                Status = responseCode,
                                Signature = Libs.Utils.Encrypts.MD5(cardAPILog.RequestNo + responseCode +amount + privateKey)
                            };

                            if (!string.IsNullOrEmpty(cardAPILog.CallbackUrl))
                                Task.Run(async () => await CallbackJson(cardAPILog.CallbackUrl, serializer.Serialize(datacb), cardAPILog.PartnerCode + " " + messageDb.Id).ConfigureAwait(false));
                            //End Callback for partner
                            if (cardAPILog.Status == 1)
                            {
                                
                                Action<string, long, string, string, string> send = UpdatePartnerBalance;
                                var asynSend = send.BeginInvoke(_Partner.PartnerCode, amount, cardAPILog.CardType.ToLower(), String.Format("Cộng tiền nạp thẻ {4} mgd: {0}-{1}-{2}-{3}", cardAPILog.TransactionID, cardAPILog.CardType, cardAPILog.CardSerial, cardAPILog.CardCode, amount.ToString("#,#").Replace(",", ".")), "Card_" + cardAPILog.TransactionID, null, null);
                                /// _CardAPILog.Status = (int) ResponseCode.TransactionSuccessful;

                            }

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
        private decimal GetCK(string PartnerCode, string CardType)
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
    }



}