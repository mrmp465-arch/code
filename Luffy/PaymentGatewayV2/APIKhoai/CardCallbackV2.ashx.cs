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
using Khoai.Entity;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace Khoai
{
    /// <summary>
    /// Summary description for VPGJsonService
    /// </summary>
    public class CardCallbackV2: IHttpHandler
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string key = "e07166726f03b5bc51f15410efed00b8";
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
            NLogLogger.Info(new string[] { "APIKhoai", "Callback", serializer.Serialize(resObj) });

            if (GlobalHelper.IsAnyNullOrEmpty(resObj))
            {
                context.Response.Write("-99|Data empty");
                return;
            }


            var signature = Encrypts.MD5(key + resObj.transaction_id);

            if (resObj.sign != signature)
            {
                context.Response.Write("-313|Signature invalid");
            }

            var result = string.Empty;

            try
            {

                var messageDb = DataRequest.GetTopupCardLog(resObj.refcode);
                if (messageDb != null)
                {
                    var responseCode = (int)ResponseCode.UndefinedError;
                    switch (resObj.status)
                    {
                        case "1":
                            responseCode = (int)ResponseCode.TransactionSuccessful;
                            //if (messageDb.AmountUser != Convert.ToInt32(resObj.real_value))
                            //{
                            //    responseCode = (int)ResponseCode.CardAmountInvalid;
                            //}    
                            break;
                        case "-1":
                            responseCode = (int)ResponseCode.CardUsed;
                            break;
                        default:
                            responseCode = (int)ResponseCode.TransactionFailed;
                            break;
                    }


                    var resultUpdate = DataRequest.UpdateTopupCard(Convert.ToInt64(resObj.refcode), Convert.ToInt32(resObj.real_value), responseCode, serializer.Serialize(resObj), string.Empty);

                    if (resultUpdate == 0)
                    {

                        if (messageDb.Status != (int)ResponseCode.TransactionSuccessful && (DateTime.Now - messageDb.CreateTime).TotalSeconds >= 90)
                        {
                            var cardAPILog = new CardAPILog().Get(Convert.ToInt32(messageDb.TransactionId));
                            cardAPILog.Amount = Convert.ToInt32(resObj.real_value);
                            cardAPILog.Description = "Callback " + responseCode + " " + resObj.real_value;
                            cardAPILog.Status = responseCode;
                            //cardAPILog.Update();
                            if (responseCode == (int)ResponseCode.TransactionSuccessful)
                            {

                                if (cardAPILog.AmountUser != Convert.ToInt32(resObj.real_value))
                                {
                                    //responseCode = (int)ResponseCode.CardAmountInvalid;
                                    //cardAPILog.Status = (int)ResponseCode.CardAmountInvalid;
                                    //if(cardAPILog.PartnerCode=="huv")
                                    //{
                                    //    if (cardAPILog.AmountUser < Convert.ToInt32(resObj.real_value))
                                    //    {
                                    //        TelegramNotify.SendWarning("583426534", $"CẢNH BÁO sai mệnh giá: {cardAPILog.CardType} TranId: {cardAPILog.TransactionID} , RefCode: {cardAPILog.RequestNo} , CardSerial: {cardAPILog.CardSerial},  Mệnh giá {cardAPILog.AmountUser}/{Convert.ToInt32(resObj.real_value)}, Partner: {cardAPILog.PartnerCode}");
                                    //        TelegramNotify.SendWarning("1497473671", $"CẢNH BÁO sai mệnh giá: {cardAPILog.CardType} TranId: {cardAPILog.TransactionID} , RefCode: {cardAPILog.RequestNo} , CardSerial: {cardAPILog.CardSerial},  Mệnh giá {cardAPILog.AmountUser}/{Convert.ToInt32(resObj.real_value)}, Partner: {cardAPILog.PartnerCode}");
                                    //    }
                                    //}    

                                }
                                //NLogLogger.Info(new string[] { "CardTelco Topup", transaction.PartnerCode, result.ResponseContent, request.CardType.ToLower() });
                                if (cardAPILog.PartnerCode == "huv")
                                {
                                    var amount = Math.Min(cardAPILog.Amount, cardAPILog.AmountUser);
                                    Action<string, long, string> send = UpdatePartnerBalance;
                                    var asynSend = send.BeginInvoke(cardAPILog.PartnerCode, amount, cardAPILog.CardType.ToLower(), null, null);

                                }
                                //tính tiền ở đây
                            }
                            //Begin Callback for partner
                            if (responseCode == (int)ResponseCode.TransactionSuccessful)
                            {
                                if (cardAPILog.AmountUser != Convert.ToInt32(resObj.real_value))
                                {
                                    responseCode = (int)ResponseCode.CardAmountInvalid;
                                    cardAPILog.Status = (int)ResponseCode.TransactionSuccessful;
                                }
                            }
                            //else
                            //{
                            //    responseCode = (int)ResponseCode.TransactionFailed;
                            //}

                            cardAPILog.Update();

                            var privateKey = new Partners().Get(messageDb.PartnerCode).PrivateKey;
                            var datacb = new DataCallback()
                            {
                                Amount = Convert.ToInt32(resObj.real_value),
                                RefCode = cardAPILog.RequestNo,
                                Status = responseCode,
                                Signature = Libs.Utils.Encrypts.MD5(cardAPILog.RequestNo + responseCode + resObj.real_value + privateKey)
                            };

                            if (!string.IsNullOrEmpty(cardAPILog.CallbackUrl))
                                Task.Run(async () => await CallbackJson(cardAPILog.CallbackUrl, serializer.Serialize(datacb), cardAPILog.PartnerCode + " " + messageDb.Id).ConfigureAwait(false));
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
                NLogLogger.Info(new string[] { "APIKhoai", "Callback", "Error", exp.Message });
                context.Response.Write(ResponseUtils.Response((int)ResponseCode.ParameterInvalid));
            }

            context.Response.Write(result);
        }
        private void UpdatePartnerBalance(string PartnerCode, long Amount, string CardType)
        {
            NLogLogger.Info(new string[] { "CardTelco Topup", PartnerCode, Amount.ToString(), CardType });
            var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, DateTime.Now.Year, DateTime.Now.Month);
            if (listpartnerDiscount == null)
                return;
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
            NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
            new Partners().Topup(realAmount, PartnerCode);

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
            

            try
            {
                var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(120);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.PostAsync(url, httpContent).ConfigureAwait(false);

                    if (response.Content != null)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        NLogLogger.Info(new string[] { "APIKhoai", "Callback Partner", "Response", code, url, postData, responseContent });
                        return responseContent;
                    }
                }

            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "APIKhoai", "Callback Partner", "Request", code, url, postData });
                NLogLogger.Info(new string[] { "APIKhoai", "Callback Partner", "Error", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }
    }



}