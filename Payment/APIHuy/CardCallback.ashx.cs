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
using Huy.Entity;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace Huy
{
    /// <summary>
    /// Summary description for VPGJsonService
    /// </summary>
    public class CardCallback : IHttpHandler
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string SecretKey = "d2d90872cf914df466210af20d3242da";
        public void ProcessRequest(HttpContext context)
        {

            context.Response.ContentType = "text/plain";
            string data = context.Request.QueryString["data"];
            //string status = context.Request.QueryString["status"];
            //string value = context.Request.QueryString["value"];
            //string real_value = context.Request.QueryString["real_value"];
            //string received_value = context.Request.QueryString["received_value"];
            //string card_seri = context.Request.QueryString["card_seri"];
            //string card_code = context.Request.QueryString["card_code"];
            //string refcode = context.Request.QueryString["refcode"];
            //string sign = context.Request.QueryString["sign"];
            if (GlobalHelper.IsAnyNullOrEmpty(data))
            {
                context.Response.Write("-99|Data empty");
                return;
            }
            var dataArr = Base64Decode(data).Split('|');
            if (dataArr.Length<1)
            {
                context.Response.Write("-99|Data Error");
                return;
            }
            NLogLogger.Info(new string[] { "APIHuy", "Callback", Base64Decode(data) });

            var resObj = new Callback();
            resObj.status = dataArr[0];
            resObj.message = dataArr[1];
            resObj.real_value = dataArr[2];
            resObj.refcode = dataArr[3];
            resObj.tran = dataArr[4];
            //resObj.status = 1;
            resObj.sign = dataArr[7];
           



            var signature = HmacSha256Digest($"{resObj.status}|{resObj.message}|{resObj.real_value}|{resObj.refcode}|{resObj.tran}|{dataArr[5]}|{dataArr[6]}",SecretKey);

            if (resObj.sign != signature)
            {
                NLogLogger.Info(new string[] { "APIHuy", "Signature invalid", $"{resObj.status}|{resObj.message}|{resObj.real_value}|{resObj.refcode}|{resObj.tran}|{dataArr[5]}|{dataArr[6]}" });

                context.Response.Write("-313|Signature invalid");
            }


            var result = string.Empty;

            try
            {
                //resObj.refcode = (long.Parse(resObj.refcode) - 20000000).ToString();
                var messageDb = DataRequest.GetTopupCardLog(resObj.refcode);
                if (messageDb != null)
                {
                    var responseCode = (int)ResponseCode.UndefinedError;
                    switch (resObj.status)
                    {
                        case "0":
                            responseCode = (int)ResponseCode.TransactionSuccessful;
                            break;
                        case "44":
                            responseCode = (int)ResponseCode.CardUsed;
                            break;
                        case "47":
                            responseCode = (int)ResponseCode.CardAmountInvalid;
                            break;
                        default:
                            responseCode = (int)ResponseCode.TransactionFailed;
                            break;
                    }

                    var resultUpdate = DataRequest.UpdateTopupCard(Convert.ToInt64(resObj.refcode), Convert.ToInt32(resObj.real_value), responseCode, serializer.Serialize(resObj), string.Empty);

                    if (resultUpdate == 0)
                    {
                        Partners _Partner = new Partners().GetCache(messageDb.PartnerCode);

                        if (messageDb.Status != (int)ResponseCode.TransactionSuccessful && (DateTime.Now - messageDb.CreateTime).TotalSeconds >= 90 || (_Partner.RequestType == 1))
                        {
                            var cardAPILog = new CardAPILog().Get(Convert.ToInt32(messageDb.TransactionId));

                            var Amount = Math.Min(cardAPILog.AmountUser, Convert.ToInt64(resObj.real_value));
                            cardAPILog.Amount = Convert.ToInt64(resObj.real_value);
                            cardAPILog.Description = "Callback " + responseCode + " " + resObj.real_value;
                            cardAPILog.Status = responseCode;
                            //cardAPILog.Update();

                            //Begin Callback for partner
                            //if (responseCode == (int)ResponseCode.TransactionSuccessful)
                            //{
                            //    if (cardAPILog.AmountUser != Convert.ToInt32(resObj.real_value))
                            //    {
                            //        responseCode = (int)ResponseCode.CardAmountInvalid;
                            //        cardAPILog.Status = (int)ResponseCode.CardAmountInvalid;
                            //    }
                            //}
                            //else
                            //{
                            //    responseCode = (int)ResponseCode.TransactionFailed;
                            //}

                            cardAPILog.Update();

                            if(cardAPILog.Status==1)
                            {
                                Action<string, long, string, string, string> send = UpdatePartnerBalance;
                                var asynSend = send.BeginInvoke(_Partner.PartnerCode, Amount, cardAPILog.CardType.ToLower(), String.Format("Cộng tiền nạp thẻ {4} mgd: {0}-{1}-{2}-{3}", cardAPILog.TransactionID, cardAPILog.CardType, cardAPILog.CardSerial, cardAPILog.CardCode, Amount.ToString("#,#").Replace(",", ".")), "Card_" + cardAPILog.TransactionID, null, null);
                                /// _CardAPILog.Status = (int) ResponseCode.TransactionSuccessful;

                            }

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
                NLogLogger.Info(new string[] { "APIHuy", "Callback", "Error", exp.Message });
                context.Response.Write(ResponseUtils.Response((int)ResponseCode.ParameterInvalid));
            }

            context.Response.Write(result);
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
                var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, 2024, 1);
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
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static string HmacSha256Digest(string message, string secretKey)
        {
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            System.Security.Cryptography.HMACSHA256 cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);
            byte[] bytes = cryptographer.ComputeHash(messageBytes);
            string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
            return base64String;
        }

        public async Task<string> CallbackJson(string url, string postData, string code)
        {
            NLogLogger.Info(new string[] { "APIHuy", "Callback Partner", "Request", code, url, postData });

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
                        NLogLogger.Info(new string[] { "APIHuy", "Callback Partner", "Response", code, url, postData, responseContent });
                        return responseContent;
                    }
                }

            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "APIHuy", "Callback Partner", "Error", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }
    }



}