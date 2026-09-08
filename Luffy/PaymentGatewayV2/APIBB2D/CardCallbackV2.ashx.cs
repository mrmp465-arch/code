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
using APIBB2D.Entity;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APIBB2D
{
    /// <summary>
    /// Summary description for VPGJsonService
    /// </summary>
    public class CardCallbackV2 : IHttpHandler
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string key = "20832ac84654d7c219912c7976263164";
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


            //var signature = Encrypts.MD5(key + resObj.transaction_id);

            //if (resObj.sign != signature)
            //{
            //    context.Response.Write("-313|Signature invalid");
            //}

            // var result = string.Empty;

            try
            {

                var messageDb = DataRequest.GetTopupCardLog(resObj.RefCode);
                if (messageDb != null)
                {
                    var responseCode = (int)ResponseCode.UndefinedError;
                    switch (resObj.Status)
                    {
                        case 1:
                        case 2:
                        case -372:
                            responseCode = (int)ResponseCode.TransactionSuccessful;

                            break;
                        case -330:
                            responseCode = (int)ResponseCode.CardUsed;
                            break;
                        default:
                            responseCode = (int)ResponseCode.TransactionFailed;
                            break;
                    }


                    var resultUpdate = DataRequest.UpdateTopupCard(Convert.ToInt64(resObj.RefCode), Convert.ToInt32(resObj.Amount), responseCode, serializer.Serialize(resObj), string.Empty);

                    if (resultUpdate == 0)
                    {

                        if (messageDb.Status != (int)ResponseCode.TransactionSuccessful )
                        {
                            var cardAPILog = new CardAPILog().Get(Convert.ToInt32(messageDb.TransactionId));
                            cardAPILog.Amount = Convert.ToInt32(resObj.Amount);
                            cardAPILog.Description = "Callback " + responseCode + " " + resObj.Amount;
                            cardAPILog.Status = responseCode;
                            //cardAPILog.Update();
                            if (responseCode == (int)ResponseCode.TransactionSuccessful)
                            {

                                if (cardAPILog.AmountUser != Convert.ToInt32(resObj.Amount))
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
                                //var amount = Math.Min(cardAPILog.Amount, cardAPILog.AmountUser);
                                //Action<string, long, string> send = UpdatePartnerBalance;
                                //var asynSend = send.BeginInvoke(cardAPILog.PartnerCode, amount, cardAPILog.CardType.ToLower(), null, null);

                                //tính tiền ở đây
                            }
                            //Begin Callback for partner
                            if (responseCode == (int)ResponseCode.TransactionSuccessful)
                            {
                                if (cardAPILog.AmountUser != Convert.ToInt32(resObj.Amount))
                                {
                                    responseCode = (int)ResponseCode.TransactionSuccessful;
                                    cardAPILog.Status = (int)ResponseCode.TransactionSuccessful;
                                }
                                if (cardAPILog.PartnerCode == "huv")
                                {
                                    var amount = Math.Min(cardAPILog.Amount, cardAPILog.AmountUser);
                                    Action<string, long, string> send = UpdatePartnerBalance;
                                    var asynSend = send.BeginInvoke(cardAPILog.PartnerCode, amount, cardAPILog.CardType.ToLower(), null, null);

                                }
                            }
                            else
                            {
                                responseCode = (int)resObj.Status;
                            }

                            cardAPILog.Update();

                            var privateKey = new Partners().Get(messageDb.PartnerCode).PrivateKey;
                            var datacb = new DataCallback()
                            {
                                Amount = (int)Math.Min(cardAPILog.Amount, cardAPILog.AmountUser),
                                RefCode = cardAPILog.RequestNo,
                                Status = responseCode,
                                Signature = Libs.Utils.Encrypts.MD5(cardAPILog.RequestNo + responseCode + resObj.Amount + privateKey)
                            };
                            try
                            {

                            
                            if (!string.IsNullOrEmpty(cardAPILog.CallbackUrl))
                                Task.Run(async () => await CallbackJson(cardAPILog.CallbackUrl, serializer.Serialize(datacb), cardAPILog.PartnerCode + " " + messageDb.Id).ConfigureAwait(false));
                            }
                            catch (Exception exp)
                            {
                                NLogLogger.Info(new string[] { "APIBB2D", "Callback", "Error", exp.Message });
                            }
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
                NLogLogger.Info(new string[] { "APIBB2D", "Callback", "Error", exp.Message });
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
            NLogLogger.Info(new string[] { "APIBB2D", "Callback Partner", "Request", code, url, postData });

            try
            {
                var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.PostAsync(url, httpContent).ConfigureAwait(false);

                    if (response.Content != null)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        NLogLogger.Info(new string[] { "APIBB2D", "Callback Partner", "Response", code, url, postData, responseContent });
                        return responseContent;
                    }
                }

            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "APIBB2D", "Callback Partner", "Error", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }
    }



}