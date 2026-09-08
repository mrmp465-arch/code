using APIBB2D.Entity;
using Libs.API;
using Libs.Report;
using Libs.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.UI;
namespace APIBB2D
{
    public partial class Cardtt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string str = string.Empty;
            string str2 = string.Empty;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            using (StreamReader reader = new StreamReader(base.Request.InputStream))
            {
                str = reader.ReadToEnd();
            }
            if (!string.IsNullOrEmpty(str))
            {
                string[] list = new string[] { "Lion2", "Callback", str };
                NLogLogger.Info(list);
                DataCallback resObj = new JavaScriptSerializer().Deserialize<DataCallback>(str);
                string[] textArray2 = new string[] { "APIBB2D", "Callback", serializer.Serialize(resObj) };
                NLogLogger.Info(textArray2);
                if (!GlobalHelper.IsAnyNullOrEmpty(resObj))
                {
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

                                if ((DateTime.Now - messageDb.CreateTime).TotalSeconds >= 90 || messageDb.PartnerCode == "hyn7" || messageDb.PartnerCode == "hyn6")
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
                                            responseCode = (int)ResponseCode.CardAmountInvalid;
                                            cardAPILog.Status = (int)ResponseCode.TransactionSuccessful;
                                        }
                                    }
                                    else
                                    {
                                        responseCode = (int)ResponseCode.TransactionFailed;
                                    }

                                    cardAPILog.Update();

                                    var privateKey = new Partners().Get(messageDb.PartnerCode).PrivateKey;
                                    var datacb = new DataCallback()
                                    {
                                        Amount = Convert.ToInt32(resObj.Amount),
                                        RefCode = cardAPILog.RequestNo,
                                        Status = responseCode,
                                        Signature = Libs.Utils.Encrypts.MD5(cardAPILog.RequestNo + responseCode + resObj.Amount + privateKey)
                                    };

                                    if (!string.IsNullOrEmpty(cardAPILog.CallbackUrl))
                                        Task.Run(async () => await CallbackJson(cardAPILog.CallbackUrl, serializer.Serialize(datacb), cardAPILog.PartnerCode + " " + messageDb.Id).ConfigureAwait(false));
                                    //End Callback for partner

                                }

                                //result = "00|Callback Success";

                            }
                            else
                            {
                                //result = "01|Callback Failed";
                            }
                        }



                    }
                    catch (Exception exception2)
                    {
                        string[] textArray4 = new string[] { "APIBB2D", "Callback", "Error", exception2.Message };
                        NLogLogger.Info(textArray4);
                    }
                }
               
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