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
        private const int ClientID = 13;

        private const string SecretKey = "cNzMT1RPhYtdxjC2kCkVVq0Yi1pv6qvx9WIoDAWI";
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


            var signature = Encrypts.MD5(String.Format("{0}|{1}|{2}|{3}", ClientID, SecretKey, resObj.transaction_id, resObj.status));

            if (resObj.signature != signature)
            {
                NLogLogger.Info(new string[] { "APIVinaPays", resObj.signature, signature });
                context.Response.Write("-313|Signature invalid");

            }

            var result = string.Empty;

            try
            {

                var messageDb = DataRequest.GetTopupCardLog(resObj.transaction_id);
                //NLogLogger.Info(new string[] { "APIVinaPays", "messageDb", serializer.Serialize(messageDb) });
                if (messageDb != null)
                {
                    var responseCode = (int)ResponseCode.UndefinedError;
                    switch (resObj.status)
                    {
                        case 1:
                            responseCode = (int)ResponseCode.TransactionSuccessful;
                            break;
                        case 3:
                        case 4:
                            responseCode = (int)ResponseCode.CardUsed;
                            break;
                        case 12:
                            responseCode = (int)ResponseCode.CardSerialInvalid;
                            break;
                        default:
                            if(resObj.message.Equals("Thẻ cào không hợp lệ hoặc đã được sử dụng"))
                            {
                                responseCode = (int)ResponseCode.CardUsed;
                            }
                            else
                            {
                                responseCode = (int)ResponseCode.TransactionFailed;
                            }
                               
                           
                            break;
                    }

                    var resultUpdate = DataRequest.UpdateTopupCard(Convert.ToInt64(resObj.transaction_id), Convert.ToInt32(resObj.value), responseCode, serializer.Serialize(resObj), string.Empty);

                    if (resultUpdate == 0)
                    {

                        if (messageDb.Status != (int)ResponseCode.TransactionSuccessful && (DateTime.Now - messageDb.CreateTime).TotalSeconds >= 90)
                        {
                            var cardAPILog = new CardAPILog().Get(Convert.ToInt32(messageDb.TransactionId));
                            cardAPILog.Amount = Convert.ToInt32(resObj.value);
                            cardAPILog.Description = "Callback " + responseCode + " " + resObj.value;
                            cardAPILog.Status = responseCode;
                            //cardAPILog.Update();



                            //Begin Callback for partner
                            if (responseCode == (int)ResponseCode.TransactionSuccessful)
                            {
                                if (cardAPILog.AmountUser != Convert.ToInt32(resObj.value))
                                {
                                    responseCode = (int)ResponseCode.CardAmountInvalid;
                                    cardAPILog.Status = (int)ResponseCode.TransactionSuccessful;
                                }
                                //NLogLogger.Info(new string[] { "CardTelco Topup", transaction.PartnerCode, result.ResponseContent, request.CardType.ToLower() });

                                //Action<string, long, string> send = UpdatePartnerBalance;
                                //var asynSend = send.BeginInvoke(cardAPILog.PartnerCode, Math.Min(cardAPILog.Amount, cardAPILog.AmountUser), cardAPILog.CardType.ToLower(), null, null);

                            }
                            else
                            {
                                responseCode = (int)ResponseCode.TransactionFailed;
                            }

                            cardAPILog.Update();

                            var privateKey = new Partners().Get(messageDb.PartnerCode).PrivateKey;
                            var datacb = new DataCallback()
                            {
                                Amount = Convert.ToInt32(resObj.value),
                                RefCode = cardAPILog.RequestNo,
                                Status = responseCode,
                                Signature = Libs.Utils.Encrypts.MD5(cardAPILog.RequestNo + responseCode + resObj.value + privateKey)
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
                case "gate":
                    ck = _partnerDiscount.RewardGATE;
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