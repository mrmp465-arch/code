using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using APIMomo.Entity;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APIMomo
{
    public class MomoBiz
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public string SendWebTopupCard(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, string sim, string simTarget, string clientId, int slot, int amount)
        {
            //Lấy ra để Process
            var coreEngine = string.Empty;
            TopupMobileLog topupProcess = new TopupMobileLog();

            if (topupProcess == null)
            {
                TelegramNotify.SendNotify(providerCode, string.Empty, string.Empty, telco, amount, 1);
                NLogLogger.Info(new string[] { "TopupMyMobi", "Order Not Found 1", transactionId, amount.ToString(), partnerCode, providerCode });
                return string.Format("{0}|{1}", -320, 0); //Không tìm thấy transaction
            }

            if (topupProcess.Status == -6) //Tự động khóa vì sai 5 lần
            {
                TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, 2);
                NLogLogger.Info(new string[] { "TopupMyMobi", "Order Auto Lock Review", transactionId, topupProcess.Mobile, topupProcess.Telco, partnerCode, providerCode, topupProcess.UserName });
                return string.Format("{0}|{1}", -314, 0);
            }
            //if (topupProcess.Status == -303 )
            //{
            //    TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, topupProcess.Status.GetValueOrDefault());

            //}
            if (topupProcess.Ussd == 2)
            {
                coreEngine = "ussd";
            }

            NLogLogger.Info(new string[] { "TopupMyMobi", topupProcess.TransactionID.ToString(), topupProcess.FullName, topupProcess.Mobile });

            if (topupProcess.TopupType == 1 || topupProcess.TopupType == 2)
            {
                var topup = new TopupMobile3rdLog();
                topup.RequestNo = topupProcess.TransactionID;
                topup.TransactionId = Convert.ToInt64(transactionId);
                topup.PartnerCode = partnerCode;
                topup.ProviderCode = providerCode;
                topup.Telco = telco;
                topup.ClientId = clientId;
                topup.Sim = topupProcess.Mobile;
                topup.SimTarget = topupProcess.Mobile;
                topup.CardSerial = serial;
                topup.CardCode = pin;
                topup.Amount = amount;
                topup.AmountUser = amount;
                topup.Core = coreEngine;
                topup.Add();
                topup.Id = topup.ReturnValue;

              
                    //Send Request to MyMobile
                    //if (topupProcess.CountCharge >= 5)
                    //{
                    //    topupProcess.Topup(-4, 0);
                    //    return string.Format("{0}|{1}", (int)ResponseCode.TransactionLimit, 0);
                    //}

                    try
                    {

                        APIResponse response = null;
                        NLogLogger.Info(new string[] { "TopupMyMobi", transactionId, "Request", topupProcess.Mobile, serial, pin });
                        var tryAgain = 0;
                        response = MomoService.TopupCard(serial, pin, topupProcess.Mobile, Convert.ToInt32(topupProcess.TopupType), topupProcess.AccountName, topupProcess.Password, string.Empty);
                        //while ((response.ResponseCode == (int)ResponseCode.TransactionTimeout || response.ResponseCode == (int)ResponseCode.ParameterInvalid) && tryAgain < 3)  
                        while ((response.ResponseCode == (int)ResponseCode.ParameterInvalid || response.ResponseCode == (int)ResponseCode.SystemBusy) && tryAgain < 3)
                        {
                            response = MomoService.TopupCard(serial, pin, topupProcess.Mobile, Convert.ToInt32(topupProcess.TopupType), topupProcess.AccountName, topupProcess.Password, string.Empty);
                            tryAgain++;
                            Thread.Sleep(1000);
                        }
                        NLogLogger.Info(new string[] { "TopupMyMobi", transactionId, "Response", serializer.Serialize(response) });
                        if (response != null)
                        {
                            topup.LogContent = serializer.Serialize(response);
                            switch (response.ResponseCode)
                            {
                                case (int)ResponseCode.TransactionSuccessful:
                                    topup.Status = topup.Status = (int)ResponseCode.TransactionSuccessful;
                                    topup.Amount = Convert.ToInt32(response.ResponseContent);
                                    topup.Update();
                                    topupProcess.Topup(1, Convert.ToInt32(response.ResponseContent)); // Thanh công update Amount
                                                                                                      //topupProcess.Topup(0, 0); // mở lại cho chạy nằm SQL

                                    //Callback Provider
                                    if (!string.IsNullOrEmpty(topupProcess.CallbackUrl))
                                    {
                                        var callbackData = new DataCallbackOrder()
                                        {
                                            OrderId = topupProcess.TransactionID,
                                            Amount = Convert.ToInt32(response.ResponseContent),
                                            Status = 1,
                                            CardSerial = topup.CardSerial,
                                            CardCode = topup.CardCode,
                                            UpdateTime = DateTime.Now,
                                            Signature = string.Empty
                                        };

                                        //Callback Provider
                                        Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName + " " + topup.Id)).ConfigureAwait(false);
                                    }

                                    return string.Format("{0}|{1}", (int)ResponseCode.TransactionSuccessful, response.ResponseContent);

                                case (int)ResponseCode.ServiceIsLocked:
                                    topup.Status = response.ResponseCode;
                                    topup.Amount = 0;
                                    topup.Update();
                                    topupProcess.Topup(-1, 0);
                                    TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, 2);
                                    return string.Format("{0}|{1}", (int)ResponseCode.ServiceIsLocked, 0);


                                case (int)ResponseCode.TransactionSuspicious:
                                    topup.Status = (int)ResponseCode.TransactionSuspicious;
                                    topup.Amount = 0;
                                    topup.Update();
                                    topupProcess.Topup(1, amount); // Cộng đơn
                                    //topupProcess.Topup(0, 0); // mở lại cho chạy
                                    TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, -2, serial);
                                    return string.Format("{0}|{1}", (int)ResponseCode.TransactionSuspicious, 0);

                                case (int)ResponseCode.TransactionLimit:
                                    //topup.Status = (int)ResponseCode.TransactionSuspicious;
                                    topup.Status = (int)ResponseCode.TransactionLimit;
                                    topup.Amount = 0;
                                    topup.Update();
                                    //topupProcess.Topup(1, amount); // Cộng đơn
                                    //topupProcess.Topup(-6, 0); //Update CountCharge for VMS
                                    topupProcess.Topup(-4, 0); //Chuyển trạng thái về Limit Charge
                                    //TelegramNotify.SendNotify(providerCode, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, 3);
                                    //return string.Format("{0}|{1}", (int)ResponseCode.TransactionSuspicious, 0); // Bị ăn thẻ cuối nên là trả luôn nghi vấn
                                    return string.Format("{0}|{1}", (int)ResponseCode.SystemBusy, 0); //

                                case (int)ResponseCode.SystemBusy:
                                    topup.Status = response.ResponseCode;
                                    topup.Amount = 0;
                                    topup.Update();
                                    topupProcess.Topup(0, 0);
                                    TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, -99, "", response.ResponseCode);
                                    return string.Format("{0}|{1}", response.ResponseCode, 0);

                                default:
                                    topup.Status = response.ResponseCode;
                                    topup.Amount = 0;
                                    topup.Update();
                                    topupProcess.Topup(0, 0);
                                    if (response.ResponseCode != -330 && response.ResponseCode != -335 && response.ResponseCode != -324)
                                        TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, -99, "", response.ResponseCode);
                                    return string.Format("{0}|{1}", response.ResponseCode, 0);
                            }

                        }

                        topup.Status = (int)ResponseCode.TransactionFailed;
                        topup.Amount = 0;
                        topup.Update();
                        topupProcess.Topup(0, 0);
                        return string.Format("{0}|{1}", (int)ResponseCode.TransactionFailed, 0);


                    }
                    catch (Exception ex)
                    {
                        topup.Status = (int)ResponseCode.TransactionFailed;
                        topup.Amount = 0;
                        topup.Update();
                        topupProcess.Topup(0, 0);
                        NLogLogger.Info(new string[] { "TopupMyMobi", "Error", ex.Message, ex.StackTrace });
                        return string.Format("{0}|{1}", (int)ResponseCode.TransactionFailed, 0);
                    }
                
            }

            return string.Format("{0}|{1}", (int)ResponseCode.CardProviderInvalid, 0); //Không tìm thấy transaction

        }

        private async Task<string> CheckStatusAsync(long id)
        {
            NLogLogger.Info(new string[] { "GSMService", "CheckStatusAsync", id.ToString() });
            for (int i = 0; i < 100; i++)
            {
                //Check DB 30 lan tuong ung 30s
                var topup = new TopupMobile3rdLog();
                topup.Id = id;
                var cardRequest = topup.Get();
                if (cardRequest != null)
                    if (cardRequest.Status != 0)
                        return string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount);
                System.Threading.Thread.Sleep(1000);
                //NLogLogger.Info(new string[] { "GSMService", "CheckStatusAsync", "i=", i.ToString() });
            }

            //Update Timeout
            DataRequest.UpdateTopupCard(id, 0, -326, string.Empty); // Update Timeout
            NLogLogger.Info(new string[] { "GSMService", "CheckStatusAsync", id.ToString(), "Timeout" });
            return "-326|0";
        }

        public async Task<string> CallbackJson(string url, string postData, string code)
        {
            NLogLogger.Info(new string[] { "TopupMyMobi", "Callback JSON", "Request", code, url, postData });

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
                        NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Provider", "Response", code, url, postData, responseContent });
                        return responseContent;
                    }
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "TopupMyMobi", "Callback JSON", "Error", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }

    }
}