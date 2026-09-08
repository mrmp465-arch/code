using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using APIMyVNTP.Entity;
using APIMyVNTP.GSM;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APIMyVNTP
{
    public class MyVNTPBiz
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        //private static string proxy = UtilsVNTPWeb.GenProxy();
        //private static HttpClient client = new HttpClient(new HttpClientHandler() { UseProxy = true, Proxy = new WebProxy(proxy) }) { Timeout = TimeSpan.FromSeconds(120) };
        //public string SendWebTopupCard(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, string sim, string simTarget, string clientId, int slot, int amount)
        //{
        //    //Lấy ra để Process 
        //    var topupProcess = new TopupMobileLog().GetProcess(telco, amount, partnerCode, providerCode);
        //    if (topupProcess == null)
        //    {
        //        TelegramNotify.SendNotify(providerCode, string.Empty, telco, amount, 1);
        //        NLogLogger.Info(new string[] { "TopupAppVNTP", "Order Not Found 1", transactionId, amount.ToString(), partnerCode, providerCode });
        //        return string.Format("{0}|{1}", -320, 0); //Không tìm thấy transaction
        //    }

        //    if (topupProcess.TransactionID == 0)
        //    {
        //        NLogLogger.Info(new string[] { "TopupAppVNTP", "Order Not Found 2", transactionId, amount.ToString(), partnerCode, providerCode });
        //        return string.Format("{0}|{1}", -320, 0); //Không tìm thấy transaction
        //    }

        //    NLogLogger.Info(new string[] { "TopupAppVNTP", topupProcess.TransactionID.ToString(), topupProcess.FullName, topupProcess.Mobile });

        //    if (topupProcess.TopupType == 1 || topupProcess.TopupType == 2)
        //    {
        //        var topup = new TopupMobile3rdLog();
        //        topup.RequestNo = topupProcess.TransactionID;
        //        topup.TransactionId = Convert.ToInt64(transactionId);
        //        topup.PartnerCode = partnerCode;
        //        topup.ProviderCode = providerCode;
        //        topup.Telco = telco;
        //        topup.ClientId = clientId;
        //        topup.Sim = sim;
        //        topup.SimTarget = topupProcess.Mobile;
        //        topup.CardSerial = serial;
        //        topup.CardCode = pin;
        //        topup.Amount = amount;
        //        topup.Add();
        //        topup.Id = topup.ReturnValue;

        //        try
        //        {
        //            var parameters = new DataRequest
        //            {
        //                card_id = pin,
        //                for_msisdn = topupProcess.Mobile
        //            };
        //            NLogLogger.Info(new string[] { "TopupAppVNTP", transactionId, "Request", serializer.Serialize(parameters) });
        //            var response = TopupAppVNP.TopupCard(pin, topupProcess.Mobile);
        //            NLogLogger.Info(new string[] { "TopupAppVNTP", transactionId, "Response", serializer.Serialize(response) });
        //            if (response != null)
        //            {
        //                topup.LogContent = serializer.Serialize(response);
        //                switch (response.ResponseCode)
        //                {
        //                    case (int)ResponseCode.TransactionSuccessful:
        //                        topup.Status = topup.Status = (int)ResponseCode.TransactionSuccessful;
        //                        topup.Amount = Convert.ToInt32(response.ResponseContent);
        //                        topup.Update();
        //                        topupProcess.Topup(1, Convert.ToInt32(response.ResponseContent)); // Thanh công update Amount
        //                        topupProcess.Topup(0, 0); // mở lại cho chạy
        //                        return string.Format("{0}|{1}", (int)ResponseCode.TransactionSuccessful, response.ResponseContent);

        //                    case (int)ResponseCode.ServiceIsLocked:
        //                        topup.Status = response.ResponseCode;
        //                        topup.Amount = 0;
        //                        topup.Update();
        //                        topupProcess.Topup(-1, 0);
        //                        TelegramNotify.SendNotify(providerCode, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, 2);
        //                        return string.Format("{0}|{1}", (int)ResponseCode.ServiceIsLocked, 0);

        //                    default:
        //                        topup.Status = response.ResponseCode;
        //                        topup.Amount = 0;
        //                        topup.Update();
        //                        topupProcess.Topup(0, 0);
        //                        return string.Format("{0}|{1}", response.ResponseCode, 0);

        //                }

        //            }

        //            topup.Status = (int)ResponseCode.TransactionFailed;
        //            topup.Amount = 0;
        //            topup.Update();
        //            topupProcess.Topup(0, 0);
        //            return string.Format("{0}|{1}", (int)ResponseCode.TransactionFailed, 0);


        //        }
        //        catch (Exception ex)
        //        {
        //            topup.Status = (int)ResponseCode.TransactionFailed;
        //            topup.Amount = 0;
        //            topup.Update();
        //            topupProcess.Topup(0, 0);
        //            NLogLogger.Info(new string[] { "TopupAppVNTP", "Error", ex.Message, ex.StackTrace });
        //            return string.Format("{0}|{1}", (int)ResponseCode.TransactionFailed, 0);
        //        }
        //    }

        //    return string.Format("{0}|{1}", (int)ResponseCode.CardProviderInvalid, 0); //Không tìm thấy transaction

        //}

        public string SendWebTopupCardV2(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, string sim, string simTarget, string clientId, int slot, int amount)
        {
            //Lấy ra để Process 
            //var topupProcess = new TopupMobileLog().GetProcess(telco, amount, partnerCode, providerCode);
            var coreEngine = string.Empty;
            TopupMobileLog topupProcess = new TopupMobileLog();

            if (slot == 1) //Có chạy USSD
            {
                var provider = new Providers().Get(providerCode);
                var getListSimReady = GSMServiceBiz.GetListSimGSMStatus(provider.GSMUrl);
                var strSimReady = string.Join(",", getListSimReady);
                NLogLogger.Info(new string[] { "GSMService", "Sim Ready", strSimReady });
                topupProcess = new TopupMobileLog().GetProcess(telco, amount, partnerCode, providerCode, strSimReady);
                coreEngine = "ussd";
            }
            else
            {
                topupProcess = new TopupMobileLog().GetProcess(telco, amount, partnerCode, providerCode);
                coreEngine = "my";
            }

            if (topupProcess == null)
            {
                TelegramNotify.SendNotify(providerCode, string.Empty, string.Empty, telco, amount,1);
                NLogLogger.Info(new string[] { "TopupAppVNTP", "Order Not Found 1", transactionId, serial, pin, amount.ToString(), partnerCode, providerCode });
                return string.Format("{0}|{1}", -320, 0); //Không tìm thấy transaction
            }

            if (topupProcess.Status == -6) //Tự động khóa vì sai 5 lần
            {
                TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, -314);
                NLogLogger.Info(new string[] { "TopupAppVNP", "Order Auto Lock Review", transactionId, topupProcess.Mobile, topupProcess.Telco, partnerCode, providerCode, topupProcess.UserName });
                return string.Format("{0}|{1}", -314, 0);
            }

            NLogLogger.Info(new string[] { "TopupAppVNTP", topupProcess.TransactionID.ToString(), serial, pin, topupProcess.FullName, topupProcess.Mobile });

            if (topupProcess.Ussd == 2)
            {
                coreEngine = "ussd";
            }

            if (topupProcess.TopupType == 1 || topupProcess.TopupType == 2)
            {
                var topup = new TopupMobile3rdLog();
                topup.RequestNo = topupProcess.TransactionID;
                topup.TransactionId = Convert.ToInt64(transactionId);
                topup.PartnerCode = partnerCode;
                topup.ProviderCode = providerCode;
                topup.Telco = telco;
                topup.ClientId = clientId;
                topup.Sim = sim;
                topup.SimTarget = topupProcess.Mobile;
                topup.CardSerial = serial;
                topup.CardCode = pin;
                topup.Amount = amount; //
                topup.AmountUser = amount;
                topup.Core = coreEngine;
                topup.BidRate = topupProcess.BidRate;
                topup.Add();
                topup.Id = topup.ReturnValue;

                if (topupProcess.Ussd == 2)
                {
                    var provider = new Providers().Get(providerCode);
                    //Send Request to GSMService
                    try
                    {
                        NLogLogger.Info(new string[] { "GSMService", transactionId, "Request", topup.Id.ToString(), telco, topupProcess.Mobile, String.Empty, serial, pin, topupProcess.TopupType.ToString() });
                        var res = GSMServiceBiz.SendChardRequest(topup.Id.ToString(), telco, topupProcess.Mobile, String.Empty, serial, pin, Convert.ToInt32(topupProcess.TopupType) * 0, provider.GSMUrl);
                        NLogLogger.Info(new string[] { "GSMService", transactionId, "Response", serializer.Serialize(res), topup.Id.ToString(), telco, topupProcess.Mobile, String.Empty, serial, pin, topupProcess.TopupType.ToString() });
                        switch (res.ResponseCode)
                        {
                            case (int)ResponseCode.TransactionSuccessful:
                                return Task.Run(async () => await CheckStatusAsync(topup.Id)).Result;

                            //case (int)ResponseCode.SystemBusy:
                            //    DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.SystemBusy, string.Empty);
                            //    topupProcess.Topup(0, 0); //Mở chạy lại
                            //    return "-303|0";

                            case (int)ResponseCode.SimNotActive:
                                Entity.DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.SimNotActive, string.Empty);
                                topupProcess.Topup(0, 0); //Mở chạy lại
                                return "-401|0";

                            default:
                                Entity.DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.TransactionRejected, string.Empty);
                                topupProcess.Topup(0, 0); //Mở chạy lại
                                return "-7|0";

                        }


                    }
                    catch (Exception ex)
                    {
                        topup.Status = (int)ResponseCode.TransactionFailed;
                        topup.Amount = 0;
                        topup.Update();
                        topupProcess.Topup(0, 0);
                        NLogLogger.Info(new string[] { "GSMService", "Error", ex.Message, ex.StackTrace });
                        return string.Format("{0}|{1}", (int)ResponseCode.TransactionFailed, 0);
                    }

                }
                else
                {

                    try
                    {

                        //var proxy = UtilsVNTPWeb.GenProxy();
                        //var client = new HttpClient(new HttpClientHandler() {UseProxy = true, Proxy = new WebProxy(proxy) });
                        APIResponse response = null;
                        var tryAgain = 0;


                        if (topupProcess.TopupType == 1 && !string.IsNullOrEmpty(topupProcess.AccountName) && !string.IsNullOrEmpty(topupProcess.Password)) // Trả trước có My ko giới hạn
                        {
                            //WEB NAPTIEN -------
                            NLogLogger.Info(new string[] { "TopupAppVNTP Naptien", transactionId, "Request TT Có My", serial, pin, topupProcess.Mobile, string.Empty, topupProcess.AccountName, topupProcess.Password });
                            tryAgain = 0;
                            response = VNTPWebService.TopupCard(topup.Id, serial, pin, topupProcess.Mobile, string.Empty, topupProcess.AccountName, topupProcess.Password, Convert.ToInt32(topupProcess.TopupType));
                            while ((response.ResponseCode == (int)ResponseCode.ParameterInvalid || response.ResponseCode == (int)ResponseCode.TransactionTimeout) && tryAgain < 3)
                            {
                                response = VNTPWebService.TopupCard(topup.Id, serial, pin, topupProcess.Mobile, string.Empty, topupProcess.AccountName, topupProcess.Password, Convert.ToInt32(topupProcess.TopupType));
                                tryAgain++;
                                Thread.Sleep(10000);
                            }
                        }


                        //------------

                        //if (topupProcess.TopupType == 2)
                        //{
                        //    ////MYVNTP WEB
                        //    NLogLogger.Info(new string[] { "TopupAppVNTP Web", transactionId, "Request", serial, pin, topupProcess.Mobile, string.Empty, topupProcess.AccountName, topupProcess.Password });
                        //    var tryAgain = 0;
                        //    response = MyVNTPWebService.TopupCard(serial, pin, topupProcess.Mobile, string.Empty, topupProcess.AccountName, topupProcess.Password);
                        //    while (response.ResponseCode == (int)ResponseCode.ParameterInvalid && tryAgain < 3)
                        //    {
                        //        response = MyVNTPWebService.TopupCard(serial, pin, topupProcess.Mobile, string.Empty, topupProcess.AccountName, topupProcess.Password);
                        //        tryAgain++;
                        //        Thread.Sleep(1000);
                        //    }
                        //}

                        else
                        {
                            //NAP App -------
                            if (string.IsNullOrEmpty(topupProcess.Password))
                            {
                                NLogLogger.Info(new string[] { "TopupAppVNTP", transactionId, "Request All Không My", serial, pin, topupProcess.Mobile });
                                response = TopupAppVNP.TopupCardV2(topup.Id, serial, pin, topupProcess.Mobile);
                                while ((response.ResponseCode == (int)ResponseCode.ParameterInvalid
                                        || response.ResponseCode == (int)ResponseCode.LoginFail
                                        || response.ResponseCode == (int)ResponseCode.TransactionLimit) && tryAgain < 3)
                                {
                                    response = TopupAppVNP.TopupCardV2(topup.Id, serial, pin, topupProcess.Mobile);
                                    tryAgain++;
                                    Thread.Sleep(10000);
                                }
                            }

                            else
                            {
                                if (topupProcess.TopupType == 2 && topupProcess.CountCharge >= 3) //Trả sau có My
                                {
                                    NLogLogger.Info(new string[] { "TopupAppVNTP", transactionId, "Request Nap hộ TS có My", serial, pin, topupProcess.Mobile, string.Empty, topupProcess.AccountName, topupProcess.Password });
                                    tryAgain = 0;
                                    response = TopupAppVNP.TopupCardV2(topup.Id, serial, pin, topupProcess.Mobile);
                                    while ((response.ResponseCode == (int)ResponseCode.ParameterInvalid) && tryAgain < 3)
                                    {
                                        tryAgain++;
                                        Thread.Sleep(10000);
                                    }
                                }
                                else
                                {
                                    NLogLogger.Info(new string[] { "TopupAppVNTP", transactionId, "Request Chính nó All Có My", serial, pin, topupProcess.Mobile, string.Empty, topupProcess.AccountName, topupProcess.Password });
                                    tryAgain = 0;
                                    response = TopupAppVNP.TopupCardV2(topup.Id, serial, pin, topupProcess.Mobile, topupProcess.AccountName, topupProcess.Password);
                                    while ((response.ResponseCode == (int)ResponseCode.ParameterInvalid) && tryAgain < 3)
                                    {
                                        response = TopupAppVNP.TopupCardV2(topup.Id, serial, pin, topupProcess.Mobile, topupProcess.AccountName, topupProcess.Password);
                                        tryAgain++;
                                        Thread.Sleep(10000);
                                    }
                                }



                            }
                        }

                        NLogLogger.Info(new string[] { "TopupAppVNTP", transactionId, "Response", serializer.Serialize(response) });

                        if (response != null)
                        {
                            topup.LogContent = serializer.Serialize(response);
                            switch (response.ResponseCode)
                            {
                                case (int)ResponseCode.TransactionSuccessful:
                                    topup.Status = topup.Status = (int)ResponseCode.TransactionSuccessful;
                                    topup.Amount = Convert.ToInt32(response.ResponseContent);
                                    topup.Update();
                                    topupProcess.Topup(1, Convert.ToInt32(response.ResponseContent), topup.BidRate); // Thanh công update Amount
                                    //topupProcess.Topup(0, 0); // mở lại cho chạy

                                    //Check Timeout
                                    //var cardApiLog = new CardAPILog().Get(Convert.ToInt32(topup.TransactionId));

                                    //if (cardApiLog != null)
                                    //{
                                    //    if (cardApiLog.Status == (int)ResponseCode.TransactionTimeout)
                                    //    {
                                    //        cardApiLog.Amount = Convert.ToInt64(topup.Amount);
                                    //        cardApiLog.Description = "Callback timeout " + cardApiLog.Amount;
                                    //        cardApiLog.Status = (int)ResponseCode.TransactionSuccessful;
                                    //        cardApiLog.Update();

                                    //        //Begin Callback for partner
                                    //        if (!string.IsNullOrEmpty(cardApiLog.CallbackUrl))
                                    //        {
                                    //            var responseCode = cardApiLog.AmountUser != cardApiLog.Amount ? (int)ResponseCode.CardAmountInvalid : (int)ResponseCode.TransactionSuccessful;
                                    //            var privateKey = new Partners().Get(topup.PartnerCode).PrivateKey;
                                    //            var datacb = new DataCallback()
                                    //            {
                                    //                Amount = Convert.ToInt32(topup.Amount),
                                    //                RefCode = cardApiLog.RequestNo,
                                    //                Status = responseCode,
                                    //                Signature = Libs.Utils.Encrypts.MD5(cardApiLog.RequestNo + responseCode + topup.Amount + privateKey)
                                    //            };
                                    //            Task.Run(async () => await CallbackJson(cardApiLog.CallbackUrl, serializer.Serialize(datacb), cardApiLog.PartnerCode + " " + topup.TransactionId).ConfigureAwait(false));
                                    //            //NLogLogger.Info(new string[] { "TopupAppVNTP", "Callback Partner", topup.TransactionId.ToString(), tcallback.Result });
                                    //        }
                                    //    }
                                    //}

                                    //Callback to Provider
                                    if (!string.IsNullOrEmpty(topupProcess.CallbackUrl))
                                    {
                                        var callbackData = new DataCallbackOrder()
                                        {
                                            OrderId = topupProcess.TransactionID,
                                            Amount = Convert.ToInt32(response.ResponseContent),
                                            Status = (int)ResponseCode.TransactionSuccessful,
                                            CardSerial = topup.CardSerial,
                                            CardCode = topup.CardCode,
                                            BidRate = topup.BidRate,
                                            UpdateTime = DateTime.Now,
                                            CreatTime = topup.CreateTime,
                                            Signature = string.Empty,
                                            Description = response.Description
                                        };
                                        Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, topup.Id, 1)).ConfigureAwait(false);
                                    }
                                    return string.Format("{0}|{1}", (int)ResponseCode.TransactionSuccessful, response.ResponseContent);

                                case (int)ResponseCode.ServiceIsLocked:
                                    topup.Status = response.ResponseCode;
                                    topup.Amount = 0;
                                    topup.Update();
                                    topupProcess.Topup(-1, 0);
                                    //Callback to Provider
                                    if (!string.IsNullOrEmpty(topupProcess.CallbackUrl))
                                    {
                                        var callbackData = new DataCallbackOrder()
                                        {
                                            OrderId = topupProcess.TransactionID,
                                            Amount = 0,
                                            Status = (int)ResponseCode.ServiceIsLocked,
                                            CardSerial = topup.CardSerial,
                                            CardCode = topup.CardCode,
                                            BidRate = topup.BidRate,
                                            UpdateTime = DateTime.Now,
                                            CreatTime = topup.CreateTime,
                                            Signature = string.Empty,
                                            Description = response.Description
                                        };
                                        Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, topup.Id, 1)).ConfigureAwait(false);
                                    }
                                    TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, 2);
                                    return string.Format("{0}|{1}", (int)ResponseCode.ServiceIsLocked, 0);

                                case (int)ResponseCode.TransactionSuspicious:
                                    topup.Status = response.ResponseCode;
                                    topup.Amount = 0;
                                    topup.Update();
                                    topupProcess.Topup(1, amount, topup.BidRate); // + Đơn tính sau
                                    topupProcess.Topup(-6, 0); // Dừng để Review
                                    //Callback to Provider
                                    if (!string.IsNullOrEmpty(topupProcess.CallbackUrl))
                                    {
                                        var callbackData = new DataCallbackOrder()
                                        {
                                            OrderId = topupProcess.TransactionID,
                                            Amount = 0,
                                            Status = (int)ResponseCode.TransactionReview,
                                            CardSerial = topup.CardSerial,
                                            CardCode = topup.CardCode,
                                            BidRate = topup.BidRate,
                                            UpdateTime = DateTime.Now,
                                            CreatTime = topup.CreateTime,
                                            Signature = string.Empty,
                                            Description = response.Description
                                        };
                                        Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, topup.Id, 1)).ConfigureAwait(false);
                                    }
                                    TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, -2);
                                    return string.Format("{0}|{1}", (int)ResponseCode.TransactionSuspicious, 0);

                                case (int)ResponseCode.SystemBusy:
                                    topup.Status = response.ResponseCode;
                                    topup.Amount = 0;
                                    topup.Update();
                                    topupProcess.Topup(0, 0);
                                    return string.Format("{0}|{1}", (int)ResponseCode.SystemBusy, 0);

                                case (int)ResponseCode.TransactionLimit:
                                    topup.Status = response.ResponseCode;
                                    topup.Amount = 0;
                                    topup.Update();
                                    topupProcess.Topup(-4, 0);
                                    //Callback to Provider
                                    if (!string.IsNullOrEmpty(topupProcess.CallbackUrl))
                                    {
                                        var callbackData = new DataCallbackOrder()
                                        {
                                            OrderId = topupProcess.TransactionID,
                                            Amount = 0,
                                            Status = (int)ResponseCode.TransactionLimit,
                                            CardSerial = topup.CardSerial,
                                            CardCode = topup.CardCode,
                                            BidRate = topup.BidRate,
                                            UpdateTime = DateTime.Now,
                                            CreatTime = topup.CreateTime,
                                            Signature = string.Empty,
                                            Description = response.Description
                                        };
                                        Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, topup.Id, 1)).ConfigureAwait(false);
                                    }
                                    return string.Format("{0}|{1}", (int)ResponseCode.TransactionLimit, 0);

                                default:
                                    topup.Status = response.ResponseCode;
                                    topup.Amount = 0;
                                    topup.Update();
                                    topupProcess.Topup(0, 0);
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
                        NLogLogger.Info(new string[] { "TopupAppVNTP", "Error", ex.Message, ex.StackTrace });
                        return string.Format("{0}|{1}", (int)ResponseCode.TransactionFailed, 0);
                    }
                }
            }
            else
            {
                topupProcess.Topup(-1, 0);
                //Callback to Provider
                if (!string.IsNullOrEmpty(topupProcess.CallbackUrl))
                {
                    var callbackData = new DataCallbackOrder()
                    {
                        OrderId = topupProcess.TransactionID,
                        Amount = 0,
                        Status = (int)ResponseCode.ServiceIsLocked,
                        CardSerial = serial,
                        CardCode = pin,
                        BidRate = topupProcess.BidRate,
                        UpdateTime = DateTime.Now,
                        CreatTime = DateTime.Now,
                        Signature = string.Empty,
                        Description = "Miss Topup Type"
                    };
                    Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, 0, 1)).ConfigureAwait(false);
                }

                TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, telco, amount, 2);
                NLogLogger.Info(new string[] { "TopupAppVNTP", "Order Not Found 3", "Miss Topup Type", transactionId, serial, pin, amount.ToString(), partnerCode, providerCode, topupProcess.Mobile });
                return string.Format("{0}|{1}", (int)ResponseCode.ServiceIsLocked, 0);
            }

            //return string.Format("{0}|{1}", (int)ResponseCode.CardProviderInvalid, 0); //Không tìm thấy transaction

        }

        private async Task<string> CheckStatusAsync(long id)
        {
            NLogLogger.Info(new string[] { "GSMService", "CheckStatusAsync", id.ToString() });
            for (int i = 0; i < 90; i++)
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
            Entity.DataRequest.UpdateTopupCard(id, 0, -326, string.Empty); // Update Timeout
            NLogLogger.Info(new string[] { "GSMService", "CheckStatusAsync", id.ToString(), "Timeout" });
            return "-326|0";
        }

        public async Task<string> CallbackJson(string url, string postData, string code, long tranId = 0, int type = 0)
        {
            NLogLogger.Info(new string[] { "TopupAppVNTP", "Callback Provider", "Request", code, tranId.ToString(), url, postData });

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
                        NLogLogger.Info(new string[] { "TopupAppVNTP", "Callback Provider", "Response", code, tranId.ToString(), url, postData, responseContent });

                        try
                        {
                            if (responseContent.Contains("1|"))
                            {
                                //NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Process TRUE", responseContent });
                                if (type == 1)
                                {
                                    new TopupMobile3rdLog().UpdateCallback(tranId, 1, null);
                                }
                                //else if (type == 2)
                                //{
                                //    new TopupMobile3rdLog().UpdateCallback(tranId, null, 1);
                                //}
                            }
                            else
                            {
                                //NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Process FAIL", responseContent });
                                if (type == 1)
                                {
                                    new TopupMobile3rdLog().UpdateCallback(tranId, -1, null);
                                }
                                //else if (type == 2)
                                //{
                                //    new TopupMobile3rdLog().UpdateCallback(tranId, null, -1);
                                //}
                            }
                        }
                        catch (Exception e)
                        {
                            if (type == 1)
                            {
                                new TopupMobile3rdLog().UpdateCallback(tranId, -1, null);
                            }
                            NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Error", e.Message });
                        }

                        return responseContent;
                    }
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "TopupAppVNTP", "Callback Partner", "Error", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }

    }
}