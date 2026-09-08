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
using APIMon.Entity;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APIMon
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
            context.Response.ContentType = "application/json";
            string status = context.Request.Form["status"];
            string amount = context.Request.Form["amount"];
            string real_amount = context.Request.Form["real_amount"];
            string tran_id = context.Request.Form["tran_id"];
            string card_seri = context.Request.Form["card_seri"];
            string card_code = context.Request.Form["card_code"];
            string request_id = context.Request.Form["request_id"];
            string message = context.Request.Form["message"];

            var result = string.Empty;
            string[] providers = { "monvms", "monvnp", "monvtt" };

            try
            {

                NLogLogger.Info(new string[] { "MON", "Callback", status, amount, real_amount, tran_id, card_seri, card_code, request_id, message });


                if (string.IsNullOrEmpty(status) || string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(real_amount) || string.IsNullOrEmpty(tran_id)
                    || string.IsNullOrEmpty(request_id) || string.IsNullOrEmpty(message) || string.IsNullOrEmpty(card_seri) || string.IsNullOrEmpty(card_code))
                {
                    context.Response.Write("{\"status\":\"0\",\"message\":\"Thất bại\"}");
                    return;
                }

                var messageDb = DataRequest.GetTopupCardLog(request_id);
                int amountReal = 0;
                if (messageDb != null)
                {

                    if (!providers.Contains(messageDb.ProviderCode))
                    {
                        context.Response.Write("{\"status\":\"0\",\"message\":\"Thất bại\"}");
                        return;
                    }

                    var responseCode = (int)ResponseCode.UndefinedError;
                    switch (status)
                    {
                        case "0":
                        case "100":
                            responseCode = (int)ResponseCode.TransactionSuccessful;
                            amountReal = Convert.ToInt32(real_amount);
                            break;
                        case "2":
                        case "1":
                            responseCode = (int)ResponseCode.CardUsed;
                            break;
                        case "101":
                            responseCode = (int)ResponseCode.TransactionSuspicious;
                            break;
                        default:
                            responseCode = (int)ResponseCode.TransactionFailed;
                            break;
                    }

                    var resultUpdate = DataRequest.UpdateTopupCard(Convert.ToInt64(request_id), amountReal, responseCode, String.Empty, Convert.ToInt64(tran_id));
                    if (resultUpdate == 0)
                    {

                        if (messageDb.Status != (int)ResponseCode.TransactionSuccessful && (DateTime.Now - messageDb.CreateTime).TotalSeconds >= 90)
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
                                //NLogLogger.Info(new string[] { "CardTelco Topup", transaction.PartnerCode, result.ResponseContent, request.CardType.ToLower() });

                                Action<string, long, string> send = UpdatePartnerBalance;
                                var asynSend = send.BeginInvoke(cardAPILog.PartnerCode, Math.Min(cardAPILog.Amount, cardAPILog.AmountUser), cardAPILog.CardType.ToLower(), null, null);

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

                            if (!string.IsNullOrEmpty(cardAPILog.CallbackUrl))
                            {
                                Task.Run(async () => await CallbackJson(cardAPILog.CallbackUrl, serializer.Serialize(datacb), cardAPILog.PartnerCode + " " + messageDb.Id).ConfigureAwait(false));
                                //NLogLogger.Info(new string[] {  "MON", "Callback Partner", messageDb.TransactionId.ToString(), tcallback.Result });
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
                NLogLogger.Info(new string[] { "MON", "Callback", "Error", exp.Message });
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
        public async Task<string> CallbackJson(string url, string postData, string code)
        {
            NLogLogger.Info(new string[] { "MON", "Callback Partner", "Request", code, url, postData });

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
                        NLogLogger.Info(new string[] { "MON", "Callback Partner", "Response", code, url, postData, responseContent });
                        return responseContent;
                    }
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MON", "Callback Partner", "Error", e.Message });
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