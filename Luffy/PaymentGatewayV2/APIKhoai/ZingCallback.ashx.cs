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
    public class ZingCallback : IHttpHandler
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string key = "b0e08528827db52f07eabddc088000d3";
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


            var resObj = new PostGetHelper().GetFromQueryString<ZCallback>();
            NLogLogger.Info(new string[] { "APIKhoai", "Callback", serializer.Serialize(resObj) });

            //if (GlobalHelper.IsAnyNullOrEmpty(resObj))
            //{
            //    context.Response.Write("-99|Data empty");
            //    return;
            //}


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
                    if (resObj.status == "1")
                    {
                        responseCode = (int)ResponseCode.TransactionSuccessful;
                    }
                    else
                    {
                        switch (resObj.status_code)
                        {
                            case "card_used":
                                responseCode = (int)ResponseCode.CardUsed;
                                break;
                            case "card_code_error":
                                responseCode = (int)ResponseCode.CardCodeInvalid;
                                break;
                            case "card_serial_error":
                                responseCode = (int)ResponseCode.CardSerialInvalid;
                                break;
                            default:
                                responseCode = (int)ResponseCode.TransactionFailed;
                                break;
                        }
                    }


                    var resultUpdate = DataRequest.UpdateTopupCard(Convert.ToInt64(resObj.refcode), Convert.ToInt32(resObj.real_value), responseCode, serializer.Serialize(resObj), string.Empty);
                    NLogLogger.Info(new string[] { "UpdateTopupCard", resultUpdate.ToString(), resObj.refcode, responseCode.ToString() });
                    if (resultUpdate == 0)
                    {

                        if ( (DateTime.Now - messageDb.CreateTime).TotalSeconds >= 90 || (!ConfigurationManager.AppSettings["PartnerDirect"].ToString().Contains("," + messageDb.PartnerCode + ",")))
                        {
                            var cardAPILog = new CardAPILog().Get(Convert.ToInt32(messageDb.TransactionId));
                            cardAPILog.Amount = Convert.ToInt32(resObj.real_value);
                            cardAPILog.Description = "Callback " + responseCode + " " + resObj.real_value;
                            cardAPILog.Status = responseCode;
                            
                            //Begin Callback for partner
                            if (responseCode == (int)ResponseCode.TransactionSuccessful)
                            {
                                if (cardAPILog.AmountUser != Convert.ToInt32(resObj.real_value))
                                {
                                    responseCode = (int)ResponseCode.TransactionSuccessful;
                                    cardAPILog.Status = (int)ResponseCode.TransactionSuccessful;
                                }
                            }
                            //else
                            //{
                            //    responseCode = (int)ResponseCode.TransactionFailed;
                            //}
                            NLogLogger.Info(new string[] { "cardAPILog.Update", cardAPILog.TransactionID.ToString(), cardAPILog.CardSerial });
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
                            result = "00|Callback Success";
                        }
                        else
                        {
                            result = "01|Callback Success";
                        }

                        

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
            NLogLogger.Info(new string[] { "APIKhoai", "Callback Partner", "Request", code, url, postData });

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
                        NLogLogger.Info(new string[] { "APIKhoai", "Callback Partner", "Response", code, url, postData, responseContent });
                        return responseContent;
                    }
                }

            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "APIKhoai", "Callback Partner", "Error", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }
    }



}