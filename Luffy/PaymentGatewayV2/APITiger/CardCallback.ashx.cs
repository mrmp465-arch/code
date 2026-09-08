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
using Tiger.Entity;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace Tiger
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
            try
            {
                using (var inputStream = new StreamReader(context.Request.InputStream))
                {
                    jsonString = inputStream.ReadToEnd();
                }

                NLogLogger.Info(new string[] { "APITiger", "Callback", jsonString });
                if (string.IsNullOrEmpty(jsonString))
                {
                    context.Response.Write("99|Data empty");
                    return;
                }
                var resObj = javaScriptSerializer.Deserialize<Callback>(jsonString);
                if (resObj.message_code == null)
                    resObj.message_code = String.Empty;
                //.Info(new string[] { "APITiger", "Callback", resObj.card_transaction_id });
                //NLogLogger.Info(new string[] { "APITiger", "Callback", resObj.message_code.ToString() });
                var messageDb = DataRequest.GetTopupCardLog(resObj.card_transaction_id);
                if (messageDb != null)
                {
                    var responseCode = (int)ResponseCode.UndefinedError;
                    switch (resObj.card_status)
                    {
                        case 2:
                            responseCode = (int)ResponseCode.TransactionSuccessful;
                            break;
                        case 3:
                            if (resObj.message_code.ToString().Equals("CARD_CODE_NOT_EXIST")|| resObj.message_code.ToString().Equals("CARD_CODE_NOT_ACTIVED"))
                                responseCode = (int)ResponseCode.CardCodeInvalid;
                            else
                                responseCode = (int)ResponseCode.CardUsed;
                            break;
                        default:
                            responseCode = (int)ResponseCode.TransactionFailed;
                            break;
                    }
                    //switch (resObj.data.statuscode)
                    //{
                    //    case "00":
                    //        responseCode = (int)ResponseCode.TransactionSuccessful;
                    //        break;
                    //    case "24":
                    //        responseCode = (int)ResponseCode.CardUsed;
                    //        break;
                    //    case "07":
                    //    case "99":
                    //    case "08":
                    //        responseCode = (int)ResponseCode.CardUsed;
                    //        break;
                    //    case "33":
                    //        responseCode = (int)ResponseCode.TransactionSuspicious;
                    //        break;
                    //    default:
                    //        responseCode = (int)ResponseCode.TransactionFailed;
                    //        break;
                    //}

                    var resultUpdate = DataRequest.UpdateTopupCard(Convert.ToInt64(resObj.card_transaction_id), resObj.card_real_amount, responseCode, jsonString, string.Empty);

                    if (resultUpdate == 0)
                    {

                        if ( (DateTime.Now - messageDb.CreateTime).TotalSeconds >= 90 ||(!ConfigurationManager.AppSettings["PartnerDirect"].ToString().Contains("," + messageDb.PartnerCode + ",")) )
                        {
                            var cardAPILog = new CardAPILog().Get(Convert.ToInt32(messageDb.TransactionId));
                            cardAPILog.Amount = resObj.card_real_amount;
                            cardAPILog.Description = "Callback " + responseCode + " " + resObj.card_real_amount;
                            cardAPILog.Status = responseCode;
                            //cardAPILog.Update();

                            //Begin Callback for partner
                            if (responseCode == (int)ResponseCode.TransactionSuccessful)
                            {
                                //if (cardAPILog.AmountUser != resObj.card_real_amount)
                                //{
                                //    responseCode = (int)ResponseCode.CardAmountInvalid;
                                //    cardAPILog.Status = (int)ResponseCode.CardAmountInvalid;
                                //}
                                if (cardAPILog.PartnerCode == "azt")
                                {
                                    var amount = Math.Min(cardAPILog.Amount, cardAPILog.AmountUser);
                                    Action<string, long, string,string> send = UpdatePartnerBalance;
                                    var asynSend = send.BeginInvoke(cardAPILog.PartnerCode, amount, cardAPILog.CardType.ToLower(), cardAPILog.CardSerial, null, null);

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
                                Amount = (int)Math.Min(cardAPILog.Amount, cardAPILog.AmountUser),
                                RefCode = cardAPILog.RequestNo,
                                Status = responseCode,
                                Signature = Libs.Utils.Encrypts.MD5(cardAPILog.RequestNo + responseCode + (int)Math.Min(cardAPILog.Amount, cardAPILog.AmountUser) + privateKey)
                            };

                            Task.Run(async () => await CallbackJson(cardAPILog.CallbackUrl, serializer.Serialize(datacb), cardAPILog.PartnerCode + " " + messageDb.Id).ConfigureAwait(false));
                            //NLogLogger.Info(new string[] { "APITiger", "Callback Partner", messageDb.TransactionId.ToString(), tcallback.Result });
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
                NLogLogger.Info(new string[] { "APITiger", "Callback", "Error", exp.Message });
                context.Response.Write(ResponseUtils.Response((int)ResponseCode.ParameterInvalid));
            }

            context.Response.Write(result);
        }
        private void UpdatePartnerBalance(string PartnerCode, long Amount, string CardType,string CardSeri)
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
                case "vnm":
                    ck = _partnerDiscount.DiscountGATE;
                    break;
            }
            if (ck == 0)
                return;

            long realAmount = Amount - Convert.ToInt64(Amount * ck);
            NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
            new Partners().Topup(realAmount, PartnerCode,$"Cộng tiền nạp thẻ {CardSeri}");

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
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    client.Timeout = TimeSpan.FromSeconds(60);
                    var response = await client.PostAsync(url, httpContent).ConfigureAwait(false);

                    if (response.Content != null)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        NLogLogger.Info(new string[] { "APITiger", "Callback Partner", "Response", code, url, postData, responseContent });
                        return responseContent;
                    }
                }

            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "APITiger", "Callback Partner", "Request", code, url, postData });
                NLogLogger.Info(new string[] { "APITiger", "Callback Partner", "Error", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }
    }




}