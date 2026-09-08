using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
using APIMyViettel.Entity;
using APIMyViettel.GSM;
using Libs.API;
using Libs.Report;
using Libs.Utils;
using Microsoft.Web.Administration;

namespace APIMyViettel
{
    public class MyViettelBiz
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        static bool CheckSerial_MyVTT = bool.Parse(ConfigurationManager.AppSettings["CheckSerialFirst_MyVTT"] ?? "true");
        static bool ussdOnly = bool.Parse(ConfigurationManager.AppSettings["USSD_Only"] ?? "false"); // Config cưỡng chế phải chạy USSD

        public string SendAppTopupCard(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, string sim, string simTarget, string clientId, int slot, int amount)
        {
            //Lấy ra để Process 
            var ussdOnlyIn = ussdOnly ? 0 : -1;
            var topupProcess = new TopupMobileLog().GetProcess(telco, amount, partnerCode, providerCode, string.Empty, ussdOnlyIn);
            var coreEngine = string.Empty;

            if (topupProcess == null)
            {
                TelegramNotify.SendNotify(providerCode, string.Empty, string.Empty, telco, amount, 1);
                NLogLogger.Info(new string[] { "TopupAppVTT", "Order Not Found 1", transactionId, amount.ToString(), partnerCode, providerCode });
                return string.Format("{0}|{1}", -320, 0); //Không tìm thấy transaction
            }

            if (topupProcess.Status == -6) //Tự động khóa vì sai 5 lần
            {
                //Callback to Provider
                if (!string.IsNullOrEmpty(topupProcess.CallbackUrl))
                {
                    var callbackData = new DataCallbackOrder()
                    {
                        OrderId = topupProcess.TransactionID,
                        Amount = 0,
                        Status = (int)ResponseCode.TransactionReview,
                        CardSerial = serial,
                        CardCode = pin,
                        BidRate = 0,
                        UpdateTime = DateTime.Now,
                        CreatTime = DateTime.Now,
                        Signature = string.Empty,
                        Description = "sai liên tiếp quá 5 lần"
                    };
                    Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, 0, 1)).ConfigureAwait(false);
                }
                TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, -314);
                NLogLogger.Info(new string[] { "TopupAppVTT", "Order Auto Lock Review", transactionId, topupProcess.Mobile, topupProcess.Telco, partnerCode, providerCode, topupProcess.UserName });
                return string.Format("{0}|{1}", -314, 0);
            }

            //NLogLogger.Info(new string[] { "TopupAppVTT", topupProcess.TransactionID.ToString(), topupProcess.FullName, topupProcess.Mobile, topupProcess.TopupType.ToString() });
            NLogLogger.Info(new string[] { "TopupAppVTT", "GET ORDER DETAIL", serializer.Serialize(topupProcess) });

            var type = Convert.ToInt32(topupProcess.TopupType);
            switch (type)
            {
                case 1:
                    type = 1; //Trả Trước
                    break;
                case 2:
                    type = 1; //Trả sau
                    break;
                case 3:
                    type = 4; //INTERNET
                    break;
                case 4:
                    type = 5; //SMAS
                    break;
                case 5:
                    type = 3; //PSTN HomePhone
                    break;
                case 6:
                    type = 6; //Yte Nha Thuoc
                    break;
                case 7:
                    type = 7; //Yte Tiem Trung
                    break;
                case 8:
                    type = 8; //ShopOne
                    break;
                case 10:
                    type = 10; //Metro Wan/Leased line
                    break;
                default:
                    type = 0;
                    break;
            }

            var ussdConfig = Utils.GetConfigCache("ussd" + topupProcess.TopupType); // Kiểm tra Config đã hết MyKo
            if (topupProcess.Ussd == 2 || (topupProcess.Ussd == 1 && ussdConfig == "true") || ussdOnly == true)
            {
                coreEngine = "ussd";
            }
            else
            {
                coreEngine = "my";
                if (topupProcess.TopupType == 1 && string.IsNullOrEmpty(topupProcess.AccountName) && string.IsNullOrEmpty(topupProcess.Password)) // Nap 136 ko My
                {
                    coreEngine = "ussd";
                }
            }


            if (type != 0)
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
                topup.Amount = amount;
                topup.AmountUser = amount;
                topup.Core = coreEngine;
                topup.BidRate = topupProcess.BidRate;
                topup.CreateTime = DateTime.Now;
                topup.Add();
                topup.Id = topup.ReturnValue;

                try
                {


                    APIResponse res = null;
                    if (topupProcess.Ussd == 2 || (topupProcess.Ussd == 1 && ussdConfig == "true") || ussdOnly == true) // Chỉ chạy USSD
                    {
                        var provider = new Providers().Get(providerCode);
                        if (string.IsNullOrEmpty(provider.GSMUrl))
                        {
                            NLogLogger.Info(new string[] { "TopupGSMVTT", transactionId, "Request GSM Link Service EMPTY", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                            DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.SystemBusy, string.Empty);
                            topupProcess.Topup(0, 0); //Mở chạy lại
                            return "-303|0";
                        }


                        if (CheckSerial_MyVTT)
                        {

                            if (Utils.GetCardSerialCache(serial) == null)
                            {
                                int tryAgain = 0;
                                var checkReponse = MyViettelService.CheckCard(serial);
                                while (tryAgain < 5 && (checkReponse.ResponseCode == (int)ResponseCode.LoginFail
                                                        || checkReponse.ResponseCode == (int)ResponseCode.TransactionLimit
                                                        || checkReponse.ResponseCode == (int)ResponseCode.AccessDenied
                                                        || checkReponse.ResponseCode == (int)ResponseCode.SystemBusy
                                                        || checkReponse.ResponseCode == (int)ResponseCode.ParameterInvalid))
                                {
                                    NLogLogger.Info(new string[] { "MyViettelApp", "CheckCard", "Try", tryAgain.ToString(), checkReponse.ResponseCode.ToString(), checkReponse.Description });
                                    checkReponse = MyViettelService.CheckCard(serial);
                                    if (checkReponse.ResponseCode == (int)ResponseCode.AccountNotExists) tryAgain = 5;
                                    tryAgain++;
                                }
                                if (checkReponse.ResponseCode == (int)ResponseCode.CardUsed
                                    || checkReponse.ResponseCode == (int)ResponseCode.CardSerialInvalid
                                    || checkReponse.ResponseCode == (int)ResponseCode.CardNotActivated)
                                {
                                    topup.Status = checkReponse.ResponseCode;
                                    topup.Amount = 0;
                                    topup.Update();
                                    topupProcess.Topup(0, 0);
                                    return string.Format("{0}|{1}", (int)checkReponse.ResponseCode, 0);
                                }
                            }


                        }

                        NLogLogger.Info(new string[] { "TopupGSMVTT", transactionId, "Request GSM", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                        //if (topupProcess.TopupType == 1 || topupProcess.TopupType == 2) // nếu là trả sau, trả trước lấy Mobile
                        //{
                        res = GSMServiceBiz.SendChardRequest(topup.Id.ToString(), telco, topupProcess.Mobile, topupProcess.Mobile, serial, pin, Convert.ToInt32(topupProcess.TopupType) * 10, provider.GSMUrl, topupProcess.AccountName, topupProcess.UserName);
                        //}
                        //else // Các loại khác lấy mã gạch nợ
                        //{
                        //    res = GSMServiceBiz.SendChardRequest(topup.Id.ToString(), telco, topupProcess.Mobile, topupProcess.Mobile, serial, pin, Convert.ToInt32(topupProcess.TopupType) * 10, provider.GSMUrl, topupProcess.AccountName, topupProcess.UserName);
                        //}
                        NLogLogger.Info(new string[] { "TopupGSMVTT", transactionId, "Response GSM", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });


                        switch (res.ResponseCode)
                        {
                            case (int)ResponseCode.TransactionSuccessful:
                                {
                                    return Task.Run(async () => await CheckStatusAsync(topup.Id)).Result;
                                }
                            case (int)ResponseCode.SystemBusy: //GSM Slot Busy
                                DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.SystemBusy, "Request GSM: " + serializer.Serialize(res));
                                topupProcess.Topup(0, 0); //Mở chạy lại
                                return "-303|0";

                            case (int)ResponseCode.SimNotActive:
                                DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.SimNotActive, "Request GSM: " + serializer.Serialize(res));
                                topupProcess.Topup(0, 0); //Mở chạy lại
                                return "-401|0";

                            case (int)ResponseCode.TransactionRejected:
                                DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.TransactionRejected, "Request GSM: " + serializer.Serialize(res));
                                topupProcess.Topup(0, 0); //Mở chạy lại
                                return "-7|0";

                            default:
                                DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.TransactionRejected, "Request GSM: " + serializer.Serialize(res));
                                topupProcess.Topup(0, 0); //Mở chạy lại
                                return "-7|0";
                        }

                    }
                    else
                    {
                        if (topupProcess.TopupType == 1) // Nap 136
                        {

                            if (!string.IsNullOrEmpty(topupProcess.AccountName) && !string.IsNullOrEmpty(topupProcess.Password))
                            {

                                NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request 136 My", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                var tryAgain = 0;
                                res = MyViettelService.TopupCard(serial, pin, topupProcess.Mobile, type, topupProcess.AccountName, topupProcess.Password, true);
                                while ((res.ResponseCode == (int)ResponseCode.ParameterInvalid
                                        || res.ResponseCode == (int)ResponseCode.LoginFail
                                        || res.ResponseCode == (int)ResponseCode.AccessDenied
                                        || res.ResponseCode == (int)ResponseCode.TransactionRejected
                                        || res.ResponseCode == (int)ResponseCode.TransactionIgnore
                                        || res.ResponseCode == (int)ResponseCode.SystemBusy) && tryAgain < 3)
                                {
                                    NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request 136 My", "Try", tryAgain.ToString(), new APIResponse(res.ResponseCode).Description, serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                    res = MyViettelService.TopupCard(serial, pin, topupProcess.Mobile, type, topupProcess.AccountName, topupProcess.Password, false);
                                    tryAgain++;
                                    //Thread.Sleep(1000);
                                }
                            }
                            else // 136 không cần my
                            {
                                //NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request 136 No My", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                var tryAgain = 0;
                                res = MyViettelService.TopupCard(serial, pin, topupProcess.Mobile, type, Convert.ToInt32(topupProcess.TopupType), amount, true);
                                while ((res.ResponseCode == (int)ResponseCode.ParameterInvalid
                                        || res.ResponseCode == (int)ResponseCode.LoginFail
                                        || res.ResponseCode == (int)ResponseCode.TransactionLimit
                                        || res.ResponseCode == (int)ResponseCode.AccessDenied
                                        || res.ResponseCode == (int)ResponseCode.AccountLocked
                                        || res.ResponseCode == (int)ResponseCode.TransactionRejected
                                        || res.ResponseCode == (int)ResponseCode.TransactionIgnore
                                        || res.ResponseCode == (int)ResponseCode.SystemBusy
                                        ) && tryAgain < 3)
                                {
                                    NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request 136 No My", "Try", tryAgain.ToString(), new APIResponse(res.ResponseCode).Description, serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                    res = MyViettelService.TopupCard(serial, pin, topupProcess.Mobile, type, Convert.ToInt32(topupProcess.TopupType), amount, false);
                                    tryAgain++;
                                    //Thread.Sleep(1000);
                                }

                                //res = new APIResponse((int)ResponseCode.ServiceIsLocked)
                                //{
                                //    Description = "Tài khoản không đăng nhập thành công, Cần cung cấp mật khẩu My Viettel"
                                //};
                            }
                            NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                        }
                        if (topupProcess.TopupType == 2) // Nap TS
                        {
                            // TS chính nó
                            if (!string.IsNullOrEmpty(topupProcess.AccountName) && !string.IsNullOrEmpty(topupProcess.Password))
                            {
                                NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                var tryAgain = 0;
                                res = MyViettelService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), type, topupProcess.AccountName, topupProcess.Password, true);
                                while ((res.ResponseCode == (int)ResponseCode.ParameterInvalid
                                        || res.ResponseCode == (int)ResponseCode.LoginFail
                                        || res.ResponseCode == (int)ResponseCode.TransactionLimit
                                        || res.ResponseCode == (int)ResponseCode.AccessDenied
                                        || res.ResponseCode == (int)ResponseCode.AccountLocked
                                        || res.ResponseCode == (int)ResponseCode.TransactionRejected
                                        || res.ResponseCode == (int)ResponseCode.TransactionIgnore
                                        || res.ResponseCode == (int)ResponseCode.SystemBusy
                                        ) && tryAgain < 3)
                                {

                                    if (res.ResponseCode == (int)ResponseCode.SystemBusy) Thread.Sleep(10000);
                                    NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request", "Try", tryAgain.ToString(), new APIResponse(res.ResponseCode).Description, serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                    res = MyViettelService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), type, topupProcess.AccountName, topupProcess.Password, false);
                                    tryAgain++;
                                    //Thread.Sleep(1000);
                                }
                                NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                            }
                            else //TS hộ
                            {
                                {
                                    NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                    var tryAgain = 0;
                                    res = MyViettelService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), type, Convert.ToInt32(topupProcess.TopupType), amount, true);
                                    while ((res.ResponseCode == (int)ResponseCode.ParameterInvalid
                                            || res.ResponseCode == (int)ResponseCode.LoginFail
                                            || res.ResponseCode == (int)ResponseCode.TransactionLimit
                                            || res.ResponseCode == (int)ResponseCode.AccessDenied
                                            || res.ResponseCode == (int)ResponseCode.AccountLocked
                                            || res.ResponseCode == (int)ResponseCode.TransactionRejected
                                            || res.ResponseCode == (int)ResponseCode.TransactionIgnore
                                            || res.ResponseCode == (int)ResponseCode.SystemBusy
                                            ) && tryAgain < 3)
                                    {

                                        if (res.ResponseCode == (int)ResponseCode.SystemBusy) Thread.Sleep(10000);
                                        NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request", "Try", tryAgain.ToString(), new APIResponse(res.ResponseCode).Description, serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                        res = MyViettelService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), type, Convert.ToInt32(topupProcess.TopupType), amount, false);
                                        tryAgain++;
                                        //Thread.Sleep(1000);
                                    }
                                    NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                                }
                            }
                        }
                        if (topupProcess.TopupType == 3 || topupProcess.TopupType == 5) //Internet or PSTN
                        {
                            NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                            var tryAgain = 0;
                            res = MyViettelService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), type, Convert.ToInt32(topupProcess.TopupType), amount, true);
                            while ((res.ResponseCode == (int)ResponseCode.ParameterInvalid
                                    || res.ResponseCode == (int)ResponseCode.LoginFail
                                    || res.ResponseCode == (int)ResponseCode.TransactionLimit
                                    || res.ResponseCode == (int)ResponseCode.AccessDenied
                                    || res.ResponseCode == (int)ResponseCode.AccountLocked
                                    || res.ResponseCode == (int)ResponseCode.TransactionRejected
                                    || res.ResponseCode == (int)ResponseCode.TransactionIgnore
                                    || res.ResponseCode == (int)ResponseCode.SystemBusy
                                    ) && tryAgain < 3)
                            {

                                if (res.ResponseCode == (int)ResponseCode.SystemBusy) Thread.Sleep(10000);
                                NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request", "Try", tryAgain.ToString(), new APIResponse(res.ResponseCode).Description, serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                res = MyViettelService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), type, Convert.ToInt32(topupProcess.TopupType), amount, false);
                                tryAgain++;
                                //Thread.Sleep(1000);
                            }
                            NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                        }


                        if (topupProcess.TopupType == 4) // Nap Smas
                        {
                            NLogLogger.Info(new string[] { "TopupSmas", transactionId, "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                            var tryAgain = 0;
                            res = SmasService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), type, topupProcess.AccountName.ToLower(), topupProcess.Password);
                            while ((res.ResponseCode == (int)ResponseCode.ParameterInvalid || res.ResponseCode == (int)ResponseCode.SystemBusy) && tryAgain < 3)
                            {
                                res = SmasService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), type, topupProcess.AccountName.ToLower(), topupProcess.Password);
                                tryAgain++;
                                //Thread.Sleep(1000);
                            }
                            NLogLogger.Info(new string[] { "TopupSmas", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                        }
                        if (topupProcess.TopupType == 6) // Nap YTe Nha Thuoc
                        {
                            NLogLogger.Info(new string[] { "TopupGpp", transactionId, "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                            var tryAgain = 0;
                            res = GppService.TopupCard(serial, pin, topupProcess.Mobile, type, topupProcess.AccountName, topupProcess.Password);
                            while (res.ResponseCode == (int)ResponseCode.ParameterInvalid && tryAgain < 3)
                            {
                                res = GppService.TopupCard(serial, pin, topupProcess.Mobile, type, topupProcess.AccountName, topupProcess.Password);
                                tryAgain++;
                                //Thread.Sleep(1000);
                            }
                            NLogLogger.Info(new string[] { "TopupGpp", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                        }
                        if (topupProcess.TopupType == 7) // Nap YTe Tiem Trung
                        {
                            NLogLogger.Info(new string[] { "TopupTcVncdc", transactionId, "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                            var tryAgain = 0;
                            res = TcVncdcService.TopupCard(serial, pin, topupProcess.Mobile, type, topupProcess.AccountName, topupProcess.Password);
                            while (res.ResponseCode == (int)ResponseCode.ParameterInvalid && tryAgain < 3)
                            {
                                res = TcVncdcService.TopupCard(serial, pin, topupProcess.Mobile, type, topupProcess.AccountName, topupProcess.Password);
                                tryAgain++;
                                //Thread.Sleep(1000);
                            }
                            NLogLogger.Info(new string[] { "TopupTcVncdc", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                        }
                        if (topupProcess.TopupType == 8) // ShopOne
                        {
                            NLogLogger.Info(new string[] { "TopupShopOne", transactionId, "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                            var tryAgain = 0;
                            res = ShopOneService.TopupCard(serial, pin, topupProcess.Mobile, type, topupProcess.AccountName, topupProcess.Password);
                            while (res.ResponseCode == (int)ResponseCode.ParameterInvalid && tryAgain < 3)
                            {
                                res = ShopOneService.TopupCard(serial, pin, topupProcess.Mobile, type, topupProcess.AccountName, topupProcess.Password);
                                tryAgain++;
                                //Thread.Sleep(1000);
                            }
                            NLogLogger.Info(new string[] { "TopupShopOne", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                        }
                    }



                    //Client.Dispose();

                    if (res != null)
                    {
                        var response = res;
                        var providerStatus = 0;
                        topup.LogContent = serializer.Serialize(res);

                        switch (response.ResponseCode)
                        {
                            case (int)ResponseCode.TransactionSuccessful:
                                var result = string.Empty;
                                int amountresponse;
                                int.TryParse(response.ResponseContent, out amountresponse);
                                int[] listValue = { 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000 };

                                if (listValue.Contains(amountresponse))
                                {
                                    topup.Status = (int)ResponseCode.TransactionSuccessful;
                                    topup.Amount = amountresponse;
                                    topup.Update();
                                    topupProcess.Topup(1, amountresponse, topup.BidRate); // Thanh công update Amount
                                    result = string.Format("{0}|{1}", (int)ResponseCode.TransactionSuccessful, amountresponse);
                                    providerStatus = (int)ResponseCode.TransactionSuccessful;
                                }
                                else
                                {
                                    topup.Status = (int)ResponseCode.TransactionSuspicious;
                                    topup.Amount = 0;
                                    topup.Update();
                                    topupProcess.Topup(-6, amount); // Lock
                                    result = string.Format("{0}|{1}", (int)ResponseCode.TransactionSuspicious, 0);
                                    providerStatus = (int)ResponseCode.TransactionReview;
                                }

                                //Callback to Provider
                                if (!string.IsNullOrEmpty(topupProcess.CallbackUrl))
                                {
                                    var callbackData = new DataCallbackOrder()
                                    {
                                        OrderId = topupProcess.TransactionID,
                                        Amount = amountresponse,
                                        Status = providerStatus,
                                        CardSerial = topup.CardSerial,
                                        CardCode = topup.CardCode,
                                        BidRate = topup.BidRate,
                                        UpdateTime = DateTime.Now,
                                        CreatTime = topup.CreateTime,
                                        Signature = string.Empty,
                                        Description = res.Description
                                    };
                                    Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, topup.Id, 1)).ConfigureAwait(false);
                                }
                                //Check Timeout
                                //var cardApiLog = new CardAPILog().Get(Convert.ToInt32(topup.TransactionId));

                                //if (cardApiLog != null)
                                //{
                                //    if (cardApiLog.Status == (int)ResponseCode.TransactionTimeout)
                                //    {
                                //        cardApiLog.Amount = Convert.ToInt32(topup.Amount);
                                //        cardApiLog.Description = "Callback timeout " + cardApiLog.Amount;
                                //        cardApiLog.Status = Convert.ToInt32(topup.Status);
                                //        cardApiLog.Update();

                                //        //Begin Callback for partner timeout
                                //        if (!string.IsNullOrEmpty(cardApiLog.CallbackUrl))
                                //        {
                                //            var responseCode = cardApiLog.Status;
                                //            if (responseCode == (int)ResponseCode.TransactionSuccessful)
                                //            {
                                //                responseCode = cardApiLog.AmountUser != cardApiLog.Amount ? (int)ResponseCode.CardAmountInvalid : (int)ResponseCode.TransactionSuccessful;
                                //            }

                                //            var privateKey = new Partners().Get(topup.PartnerCode).PrivateKey;
                                //            var datacb = new DataCallback()
                                //            {
                                //                Amount = Convert.ToInt32(cardApiLog.Amount),
                                //                RefCode = cardApiLog.RequestNo,
                                //                Status = responseCode,
                                //                Signature = Libs.Utils.Encrypts.MD5(cardApiLog.RequestNo + responseCode + topup.Amount + privateKey)
                                //            };
                                //            Task.Run(async () => await CallbackJson(cardApiLog.CallbackUrl, serializer.Serialize(datacb), cardApiLog.PartnerCode + " " + topup.TransactionId).ConfigureAwait(false));
                                //        }
                                //    }
                                //}

                                return result;


                            //case (int)ResponseCode.LoginFail:
                            //    if (topupProcess.TopupType == 4) //SMAS
                            //    {
                            //        topupProcess.Topup(-1, 0); //Lock
                            //    }
                            //    topup.Status = response.ResponseCode;
                            //    topup.Amount = 0;
                            //    topup.Update();
                            //    return string.Format("{0}|{1}", (int)ResponseCode.LoginFail, 0);

                            case (int)ResponseCode.ServiceIsLocked:
                                topupProcess.Topup(-1, 0);
                                topup.Status = response.ResponseCode;
                                topup.Amount = 0;
                                topup.Update();
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
                                        Description = res.Description
                                    };
                                    Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, topup.Id, 1)).ConfigureAwait(false);
                                }
                                TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, 2);
                                return string.Format("{0}|{1}", (int)ResponseCode.ServiceIsLocked, 0);

                            case (int)ResponseCode.TransactionIgnore:
                                topupProcess.Topup(-2, 0); //Ignore
                                topup.Status = response.ResponseCode;
                                topup.Amount = 0;
                                topup.Update();
                                return string.Format("{0}|{1}", (int)ResponseCode.SystemBusy, 0);

                            case (int)ResponseCode.TransactionSuspicious:
                                topup.Status = response.ResponseCode;
                                topup.Amount = 0;
                                topup.Update();
                                topupProcess.Topup(1, amount, topup.BidRate); // + Đơn tính sau
                                topupProcess.Topup(-6, 0); // Stop đơn để review
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
                                        Description = res.Description
                                    };
                                    Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, topup.Id, 1)).ConfigureAwait(false);
                                }
                                TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, -2, serial);
                                return string.Format("{0}|{1}", (int)ResponseCode.TransactionSuspicious, 0);

                            default:
                                topup.Status = response.ResponseCode;
                                topup.Amount = 0;
                                topup.Update();
                                topupProcess.Topup(0, 0);
                                switch (response.ResponseCode)
                                {
                                    case (int)ResponseCode.LoginFail:
                                        TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, -55);
                                        break;
                                    case (int)ResponseCode.TransactionFailed:
                                        TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, -99, serial, -1);
                                        break;
                                }
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
                    NLogLogger.Info(new string[] { "TopupAppVTT", "Error", ex.Message, ex.StackTrace });
                    return string.Format("{0}|{1}", -1, 0);
                }
            }

            topupProcess.Topup(-1, 0);
            return string.Format("{0}|{1}", (int)ResponseCode.ServiceIsLocked, 0);

        }

        public string SendWebTopupCard(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, string sim, string simTarget, string clientId, int slot, int amount)
        {
            //Lấy ra để Process 
            var ussdOnlyIn = ussdOnly ? 0 : -1;
            var topupProcess = new TopupMobileLog().GetProcess(telco, amount, partnerCode, providerCode, string.Empty, ussdOnlyIn);
            var coreEngine = string.Empty;

            if (topupProcess == null)
            {
                TelegramNotify.SendNotify(providerCode, string.Empty, string.Empty, telco, amount, 1);
                NLogLogger.Info(new string[] { "TopupAppVTT", "Order Not Found 1", transactionId, amount.ToString(), partnerCode, providerCode });
                return string.Format("{0}|{1}", -320, 0); //Không tìm thấy transaction
            }

            if (topupProcess.Status == -6) //Tự động khóa vì sai 5 lần
            {
                //Callback to Provider
                if (!string.IsNullOrEmpty(topupProcess.CallbackUrl))
                {
                    var callbackData = new DataCallbackOrder()
                    {
                        OrderId = topupProcess.TransactionID,
                        Amount = 0,
                        Status = (int)ResponseCode.TransactionReview,
                        CardSerial = serial,
                        CardCode = pin,
                        BidRate = 0,
                        UpdateTime = DateTime.Now,
                        CreatTime = DateTime.Now,
                        Signature = string.Empty,
                        Description = "sai liên tiếp quá 5 lần"
                    };
                    Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, 0, 1)).ConfigureAwait(false);
                }
                TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, -314);
                NLogLogger.Info(new string[] { "TopupAppVTT", "Order Auto Lock Review", transactionId, topupProcess.Mobile, topupProcess.Telco, partnerCode, providerCode, topupProcess.UserName });
                return string.Format("{0}|{1}", -314, 0);
            }

            //NLogLogger.Info(new string[] { "TopupAppVTT", topupProcess.TransactionID.ToString(), topupProcess.FullName, topupProcess.Mobile, topupProcess.TopupType.ToString() });
            NLogLogger.Info(new string[] { "TopupAppVTT", "GET ORDER DETAIL", serializer.Serialize(topupProcess) });

            var type = Convert.ToInt32(topupProcess.TopupType);
            switch (type)
            {
                case 1:
                    type = 1; //Trả Trước
                    break;
                case 2:
                    type = 1; //Trả sau
                    break;
                case 3:
                    type = 4; //INTERNET
                    break;
                case 4:
                    type = 5; //SMAS
                    break;
                case 5:
                    type = 3; //PSTN HomePhone
                    break;
                case 6:
                    type = 6; //Yte Nha Thuoc
                    break;
                case 7:
                    type = 7; //Yte Tiem Trung
                    break;
                case 8:
                    type = 8; //ShopOne
                    break;
                case 10:
                    type = 10; //Metro Wan/Leased line
                    break;
                default:
                    type = 0;
                    break;
            }

            var ussdConfig = Utils.GetConfigCache("ussd" + topupProcess.TopupType); // Kiểm tra Config đã hết MyKo
            if (topupProcess.Ussd == 2 || (topupProcess.Ussd == 1 && ussdConfig == "true") || ussdOnly == true)
            {
                coreEngine = "ussd";
            }
            else
            {
                coreEngine = "my";
                if (topupProcess.TopupType == 1 && string.IsNullOrEmpty(topupProcess.AccountName) && string.IsNullOrEmpty(topupProcess.Password)) // Nap 136 ko My
                {
                    coreEngine = "ussd";
                }
            }


            if (type != 0)
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
                topup.Amount = amount;
                topup.AmountUser = amount;
                topup.Core = coreEngine;
                topup.BidRate = topupProcess.BidRate;
                topup.CreateTime = DateTime.Now;
                topup.Add();
                topup.Id = topup.ReturnValue;

                try
                {


                    APIResponse res = null;
                    if (topupProcess.Ussd == 2 || (topupProcess.Ussd == 1 && ussdConfig == "true") || ussdOnly == true) // Chỉ chạy USSD
                    {
                        var provider = new Providers().Get(providerCode);
                        if (string.IsNullOrEmpty(provider.GSMUrl))
                        {
                            NLogLogger.Info(new string[] { "TopupGSMVTT", transactionId, "Request GSM Link Service EMPTY", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                            DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.SystemBusy, string.Empty);
                            topupProcess.Topup(0, 0); //Mở chạy lại
                            return "-303|0";
                        }


                        if (CheckSerial_MyVTT)
                        {

                            if (Utils.GetCardSerialCache(serial) == null)
                            {
                                int tryAgain = 0;
                                var checkReponse = WebService.CheckCard(serial);
                                while (tryAgain < 5 && (checkReponse.ResponseCode == (int)ResponseCode.LoginFail
                                                        || checkReponse.ResponseCode == (int)ResponseCode.TransactionLimit
                                                        || checkReponse.ResponseCode == (int)ResponseCode.AccessDenied
                                                        || checkReponse.ResponseCode == (int)ResponseCode.SystemBusy
                                                        || checkReponse.ResponseCode == (int)ResponseCode.ParameterInvalid))
                                {
                                    NLogLogger.Info(new string[] { "MyViettelApp", "CheckCard", "Try", tryAgain.ToString(), checkReponse.ResponseCode.ToString(), checkReponse.Description });
                                    checkReponse = WebService.CheckCard(serial);
                                    if (checkReponse.ResponseCode == (int)ResponseCode.AccountNotExists) tryAgain = 5;
                                    tryAgain++;
                                }
                                if (checkReponse.ResponseCode == (int)ResponseCode.CardUsed
                                    || checkReponse.ResponseCode == (int)ResponseCode.CardSerialInvalid
                                    || checkReponse.ResponseCode == (int)ResponseCode.CardNotActivated)
                                {
                                    topup.Status = checkReponse.ResponseCode;
                                    topup.Amount = 0;
                                    topup.Update();
                                    topupProcess.Topup(0, 0);
                                    return string.Format("{0}|{1}", (int)checkReponse.ResponseCode, 0);
                                }
                            }


                        }

                        NLogLogger.Info(new string[] { "TopupGSMVTT", transactionId, "Request GSM", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                        //if (topupProcess.TopupType == 1 || topupProcess.TopupType == 2) // nếu là trả sau, trả trước lấy Mobile
                        //{
                        res = GSMServiceBiz.SendChardRequest(topup.Id.ToString(), telco, topupProcess.Mobile, topupProcess.Mobile, serial, pin, Convert.ToInt32(topupProcess.TopupType) * 10, provider.GSMUrl, topupProcess.AccountName, topupProcess.UserName);
                        //}
                        //else // Các loại khác lấy mã gạch nợ
                        //{
                        //    res = GSMServiceBiz.SendChardRequest(topup.Id.ToString(), telco, topupProcess.Mobile, topupProcess.Mobile, serial, pin, Convert.ToInt32(topupProcess.TopupType) * 10, provider.GSMUrl, topupProcess.AccountName, topupProcess.UserName);
                        //}
                        NLogLogger.Info(new string[] { "TopupGSMVTT", transactionId, "Response GSM", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });


                        switch (res.ResponseCode)
                        {
                            case (int)ResponseCode.TransactionSuccessful:
                                {
                                    return Task.Run(async () => await CheckStatusAsync(topup.Id)).Result;
                                }
                            case (int)ResponseCode.SystemBusy: //GSM Slot Busy
                                DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.SystemBusy, "Request GSM: " + serializer.Serialize(res));
                                topupProcess.Topup(0, 0); //Mở chạy lại
                                return "-303|0";

                            case (int)ResponseCode.SimNotActive:
                                DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.SimNotActive, "Request GSM: " + serializer.Serialize(res));
                                topupProcess.Topup(0, 0); //Mở chạy lại
                                return "-401|0";

                            case (int)ResponseCode.TransactionRejected:
                                DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.TransactionRejected, "Request GSM: " + serializer.Serialize(res));
                                topupProcess.Topup(0, 0); //Mở chạy lại
                                return "-7|0";

                            default:
                                DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.TransactionRejected, "Request GSM: " + serializer.Serialize(res));
                                topupProcess.Topup(0, 0); //Mở chạy lại
                                return "-7|0";
                        }

                    }
                    else
                    {
                        if (topupProcess.TopupType == 1) // Nap 136
                        {

                            if (!string.IsNullOrEmpty(topupProcess.AccountName) && !string.IsNullOrEmpty(topupProcess.Password))
                            {

                                NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request 136 My", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                var tryAgain = 0;
                                res = WebService.TopupCard(serial, pin, topupProcess.Mobile, type, topupProcess.AccountName, topupProcess.Password, true);
                                while ((res.ResponseCode == (int)ResponseCode.ParameterInvalid
                                        || res.ResponseCode == (int)ResponseCode.LoginFail
                                        || res.ResponseCode == (int)ResponseCode.AccessDenied
                                        || res.ResponseCode == (int)ResponseCode.TransactionRejected
                                        || res.ResponseCode == (int)ResponseCode.TransactionIgnore
                                        || res.ResponseCode == (int)ResponseCode.SystemBusy) && tryAgain < 3)
                                {
                                    NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request 136 My", "Try", tryAgain.ToString(), new APIResponse(res.ResponseCode).Description, serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                    res = WebService.TopupCard(serial, pin, topupProcess.Mobile, type, topupProcess.AccountName, topupProcess.Password, false);
                                    tryAgain++;
                                    //Thread.Sleep(1000);
                                }
                            }
                            else // 136 không cần my
                            {
                                //NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request 136 No My", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                var tryAgain = 0;
                                res = WebService.TopupCard(serial, pin, topupProcess.Mobile, type, Convert.ToInt32(topupProcess.TopupType), amount, true);
                                while ((res.ResponseCode == (int)ResponseCode.ParameterInvalid
                                        || res.ResponseCode == (int)ResponseCode.LoginFail
                                        || res.ResponseCode == (int)ResponseCode.TransactionLimit
                                        || res.ResponseCode == (int)ResponseCode.AccessDenied
                                        || res.ResponseCode == (int)ResponseCode.AccountLocked
                                        || res.ResponseCode == (int)ResponseCode.TransactionRejected
                                        || res.ResponseCode == (int)ResponseCode.TransactionIgnore
                                        || res.ResponseCode == (int)ResponseCode.SystemBusy
                                        ) && tryAgain < 3)
                                {
                                    NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request 136 No My", "Try", tryAgain.ToString(), new APIResponse(res.ResponseCode).Description, serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                    res = WebService.TopupCard(serial, pin, topupProcess.Mobile, type, Convert.ToInt32(topupProcess.TopupType), amount, false);
                                    tryAgain++;
                                    //Thread.Sleep(1000);
                                }

                                //res = new APIResponse((int)ResponseCode.ServiceIsLocked)
                                //{
                                //    Description = "Tài khoản không đăng nhập thành công, Cần cung cấp mật khẩu My Viettel"
                                //};
                            }
                            NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                        }
                        if (topupProcess.TopupType == 2) // Nap TS
                        {
                            // TS chính nó
                            if (!string.IsNullOrEmpty(topupProcess.AccountName) && !string.IsNullOrEmpty(topupProcess.Password))
                            {
                                NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                var tryAgain = 0;
                                res = WebService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), type, topupProcess.AccountName, topupProcess.Password, true);
                                while ((res.ResponseCode == (int)ResponseCode.ParameterInvalid
                                        || res.ResponseCode == (int)ResponseCode.LoginFail
                                        || res.ResponseCode == (int)ResponseCode.TransactionLimit
                                        || res.ResponseCode == (int)ResponseCode.AccessDenied
                                        || res.ResponseCode == (int)ResponseCode.AccountLocked
                                        || res.ResponseCode == (int)ResponseCode.TransactionRejected
                                        || res.ResponseCode == (int)ResponseCode.TransactionIgnore
                                        || res.ResponseCode == (int)ResponseCode.SystemBusy
                                        ) && tryAgain < 3)
                                {

                                    if (res.ResponseCode == (int)ResponseCode.SystemBusy) Thread.Sleep(10000);
                                    NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request", "Try", tryAgain.ToString(), new APIResponse(res.ResponseCode).Description, serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                    res = WebService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), type, topupProcess.AccountName, topupProcess.Password, false);
                                    tryAgain++;
                                    //Thread.Sleep(1000);
                                }
                                NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                            }
                            else //TS hộ
                            {
                                {
                                    NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                    var tryAgain = 0;
                                    res = WebService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), type, Convert.ToInt32(topupProcess.TopupType), amount, true);
                                    while ((res.ResponseCode == (int)ResponseCode.ParameterInvalid
                                            || res.ResponseCode == (int)ResponseCode.LoginFail
                                            || res.ResponseCode == (int)ResponseCode.TransactionLimit
                                            || res.ResponseCode == (int)ResponseCode.AccessDenied
                                            || res.ResponseCode == (int)ResponseCode.AccountLocked
                                            || res.ResponseCode == (int)ResponseCode.TransactionRejected
                                            || res.ResponseCode == (int)ResponseCode.TransactionIgnore
                                            || res.ResponseCode == (int)ResponseCode.SystemBusy
                                            ) && tryAgain < 3)
                                    {

                                        if (res.ResponseCode == (int)ResponseCode.SystemBusy) Thread.Sleep(10000);
                                        NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request", "Try", tryAgain.ToString(), new APIResponse(res.ResponseCode).Description, serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                        res = WebService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), type, Convert.ToInt32(topupProcess.TopupType), amount, false);
                                        tryAgain++;
                                        //Thread.Sleep(1000);
                                    }
                                    NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                                }
                            }
                        }
                        if (topupProcess.TopupType == 3 || topupProcess.TopupType == 5) //Internet or PSTN
                        {
                            NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                            var tryAgain = 0;
                            res = WebService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), type, Convert.ToInt32(topupProcess.TopupType), amount, true);
                            while ((res.ResponseCode == (int)ResponseCode.ParameterInvalid
                                    || res.ResponseCode == (int)ResponseCode.LoginFail
                                    || res.ResponseCode == (int)ResponseCode.TransactionLimit
                                    || res.ResponseCode == (int)ResponseCode.AccessDenied
                                    || res.ResponseCode == (int)ResponseCode.AccountLocked
                                    || res.ResponseCode == (int)ResponseCode.TransactionRejected
                                    || res.ResponseCode == (int)ResponseCode.TransactionIgnore
                                    || res.ResponseCode == (int)ResponseCode.SystemBusy
                                    ) && tryAgain < 3)
                            {

                                if (res.ResponseCode == (int)ResponseCode.SystemBusy) Thread.Sleep(10000);
                                NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Request", "Try", tryAgain.ToString(), new APIResponse(res.ResponseCode).Description, serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                                res = WebService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), type, Convert.ToInt32(topupProcess.TopupType), amount, false);
                                tryAgain++;
                                //Thread.Sleep(1000);
                            }
                            NLogLogger.Info(new string[] { "TopupAppVTT", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                        }


                        if (topupProcess.TopupType == 4) // Nap Smas
                        {
                            NLogLogger.Info(new string[] { "TopupSmas", transactionId, "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                            var tryAgain = 0;
                            res = SmasService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), type, topupProcess.AccountName.ToLower(), topupProcess.Password);
                            while ((res.ResponseCode == (int)ResponseCode.ParameterInvalid || res.ResponseCode == (int)ResponseCode.SystemBusy) && tryAgain < 3)
                            {
                                res = SmasService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), type, topupProcess.AccountName.ToLower(), topupProcess.Password);
                                tryAgain++;
                                //Thread.Sleep(1000);
                            }
                            NLogLogger.Info(new string[] { "TopupSmas", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                        }
                        if (topupProcess.TopupType == 6) // Nap YTe Nha Thuoc
                        {
                            NLogLogger.Info(new string[] { "TopupGpp", transactionId, "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                            var tryAgain = 0;
                            res = GppService.TopupCard(serial, pin, topupProcess.Mobile, type, topupProcess.AccountName, topupProcess.Password);
                            while (res.ResponseCode == (int)ResponseCode.ParameterInvalid && tryAgain < 3)
                            {
                                res = GppService.TopupCard(serial, pin, topupProcess.Mobile, type, topupProcess.AccountName, topupProcess.Password);
                                tryAgain++;
                                //Thread.Sleep(1000);
                            }
                            NLogLogger.Info(new string[] { "TopupGpp", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                        }
                        if (topupProcess.TopupType == 7) // Nap YTe Tiem Trung
                        {
                            NLogLogger.Info(new string[] { "TopupTcVncdc", transactionId, "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                            var tryAgain = 0;
                            res = TcVncdcService.TopupCard(serial, pin, topupProcess.Mobile, type, topupProcess.AccountName, topupProcess.Password);
                            while (res.ResponseCode == (int)ResponseCode.ParameterInvalid && tryAgain < 3)
                            {
                                res = TcVncdcService.TopupCard(serial, pin, topupProcess.Mobile, type, topupProcess.AccountName, topupProcess.Password);
                                tryAgain++;
                                //Thread.Sleep(1000);
                            }
                            NLogLogger.Info(new string[] { "TopupTcVncdc", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                        }
                        if (topupProcess.TopupType == 8) // ShopOne
                        {
                            NLogLogger.Info(new string[] { "TopupShopOne", transactionId, "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                            var tryAgain = 0;
                            res = ShopOneService.TopupCard(serial, pin, topupProcess.Mobile, type, topupProcess.AccountName, topupProcess.Password);
                            while (res.ResponseCode == (int)ResponseCode.ParameterInvalid && tryAgain < 3)
                            {
                                res = ShopOneService.TopupCard(serial, pin, topupProcess.Mobile, type, topupProcess.AccountName, topupProcess.Password);
                                tryAgain++;
                                //Thread.Sleep(1000);
                            }
                            NLogLogger.Info(new string[] { "TopupShopOne", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                        }
                    }



                    //Client.Dispose();

                    if (res != null)
                    {
                        var response = res;
                        var providerStatus = 0;
                        topup.LogContent = serializer.Serialize(res);

                        switch (response.ResponseCode)
                        {
                            case (int)ResponseCode.TransactionSuccessful:
                                var result = string.Empty;
                                int amountresponse;
                                int.TryParse(response.ResponseContent, out amountresponse);
                                int[] listValue = { 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000 };

                                if (listValue.Contains(amountresponse))
                                {
                                    topup.Status = (int)ResponseCode.TransactionSuccessful;
                                    topup.Amount = amountresponse;
                                    topup.Update();
                                    topupProcess.Topup(1, amountresponse, topup.BidRate); // Thanh công update Amount
                                    result = string.Format("{0}|{1}", (int)ResponseCode.TransactionSuccessful, amountresponse);
                                    providerStatus = (int)ResponseCode.TransactionSuccessful;
                                }
                                else
                                {
                                    topup.Status = (int)ResponseCode.TransactionSuspicious;
                                    topup.Amount = 0;
                                    topup.Update();
                                    topupProcess.Topup(-6, amount); // Lock
                                    result = string.Format("{0}|{1}", (int)ResponseCode.TransactionSuspicious, 0);
                                    providerStatus = (int)ResponseCode.TransactionReview;
                                }

                                //Callback to Provider
                                if (!string.IsNullOrEmpty(topupProcess.CallbackUrl))
                                {
                                    var callbackData = new DataCallbackOrder()
                                    {
                                        OrderId = topupProcess.TransactionID,
                                        Amount = amountresponse,
                                        Status = providerStatus,
                                        CardSerial = topup.CardSerial,
                                        CardCode = topup.CardCode,
                                        BidRate = topup.BidRate,
                                        UpdateTime = DateTime.Now,
                                        CreatTime = topup.CreateTime,
                                        Signature = string.Empty,
                                        Description = res.Description
                                    };
                                    Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, topup.Id, 1)).ConfigureAwait(false);
                                }
                                //Check Timeout
                                //var cardApiLog = new CardAPILog().Get(Convert.ToInt32(topup.TransactionId));

                                //if (cardApiLog != null)
                                //{
                                //    if (cardApiLog.Status == (int)ResponseCode.TransactionTimeout)
                                //    {
                                //        cardApiLog.Amount = Convert.ToInt32(topup.Amount);
                                //        cardApiLog.Description = "Callback timeout " + cardApiLog.Amount;
                                //        cardApiLog.Status = Convert.ToInt32(topup.Status);
                                //        cardApiLog.Update();

                                //        //Begin Callback for partner timeout
                                //        if (!string.IsNullOrEmpty(cardApiLog.CallbackUrl))
                                //        {
                                //            var responseCode = cardApiLog.Status;
                                //            if (responseCode == (int)ResponseCode.TransactionSuccessful)
                                //            {
                                //                responseCode = cardApiLog.AmountUser != cardApiLog.Amount ? (int)ResponseCode.CardAmountInvalid : (int)ResponseCode.TransactionSuccessful;
                                //            }

                                //            var privateKey = new Partners().Get(topup.PartnerCode).PrivateKey;
                                //            var datacb = new DataCallback()
                                //            {
                                //                Amount = Convert.ToInt32(cardApiLog.Amount),
                                //                RefCode = cardApiLog.RequestNo,
                                //                Status = responseCode,
                                //                Signature = Libs.Utils.Encrypts.MD5(cardApiLog.RequestNo + responseCode + topup.Amount + privateKey)
                                //            };
                                //            Task.Run(async () => await CallbackJson(cardApiLog.CallbackUrl, serializer.Serialize(datacb), cardApiLog.PartnerCode + " " + topup.TransactionId).ConfigureAwait(false));
                                //        }
                                //    }
                                //}

                                return result;


                            //case (int)ResponseCode.LoginFail:
                            //    if (topupProcess.TopupType == 4) //SMAS
                            //    {
                            //        topupProcess.Topup(-1, 0); //Lock
                            //    }
                            //    topup.Status = response.ResponseCode;
                            //    topup.Amount = 0;
                            //    topup.Update();
                            //    return string.Format("{0}|{1}", (int)ResponseCode.LoginFail, 0);

                            case (int)ResponseCode.ServiceIsLocked:
                                topupProcess.Topup(-1, 0);
                                topup.Status = response.ResponseCode;
                                topup.Amount = 0;
                                topup.Update();
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
                                        Description = res.Description
                                    };
                                    Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, topup.Id, 1)).ConfigureAwait(false);
                                }
                                TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, 2);
                                return string.Format("{0}|{1}", (int)ResponseCode.ServiceIsLocked, 0);

                            case (int)ResponseCode.TransactionIgnore:
                                topupProcess.Topup(-2, 0); //Ignore
                                topup.Status = response.ResponseCode;
                                topup.Amount = 0;
                                topup.Update();
                                return string.Format("{0}|{1}", (int)ResponseCode.SystemBusy, 0);

                            case (int)ResponseCode.TransactionSuspicious:
                                topup.Status = response.ResponseCode;
                                topup.Amount = 0;
                                topup.Update();
                                topupProcess.Topup(1, amount, topup.BidRate); // + Đơn tính sau
                                topupProcess.Topup(-6, 0); // Stop đơn để review
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
                                        Description = res.Description
                                    };
                                    Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, topup.Id, 1)).ConfigureAwait(false);
                                }
                                TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, -2, serial);
                                return string.Format("{0}|{1}", (int)ResponseCode.TransactionSuspicious, 0);

                            default:
                                topup.Status = response.ResponseCode;
                                topup.Amount = 0;
                                topup.Update();
                                topupProcess.Topup(0, 0);
                                switch (response.ResponseCode)
                                {
                                    case (int)ResponseCode.LoginFail:
                                        TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, -55);
                                        break;
                                    case (int)ResponseCode.TransactionFailed:
                                        TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, -99, serial, -1);
                                        break;
                                }
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
                    NLogLogger.Info(new string[] { "TopupAppVTT", "Error", ex.Message, ex.StackTrace });
                    return string.Format("{0}|{1}", -1, 0);
                }
            }

            topupProcess.Topup(-1, 0);
            return string.Format("{0}|{1}", (int)ResponseCode.ServiceIsLocked, 0);

        }

        private async Task<string> CheckStatusAsync(long id)
        {
            //NLogLogger.Info(new string[] { "GSMService", "CheckStatusAsync", id.ToString() });
            for (int i = 0; i < 90; i++)
            {
                //Check DB 30 lan tuong ung 30s
                //NLogLogger.Info(new string[] { "GSMService", "CheckStatusAsync", "Request", "i=", i.ToString(), id.ToString() });
                var topup = new TopupMobile3rdLog();
                topup.Id = id;
                var cardRequest = topup.Get();
                if (cardRequest != null)
                {
                    if (cardRequest.Status != 0)
                    {
                        NLogLogger.Info(new string[] { "GSMService", "CheckStatusAsync", "Response", "i=", i.ToString(), id.ToString(), "Status: " + cardRequest.Status, "Amount: " + cardRequest.Amount });
                        return string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount);
                    }
                }
                System.Threading.Thread.Sleep(1000);
            }

            //Update Timeout
            DataRequest.UpdateTopupCard(id, 0, -326, "Timeout"); // Update Timeout
            NLogLogger.Info(new string[] { "GSMService", "CheckStatusAsync", id.ToString(), "Timeout" });
            return "-326|0";
        }

        public async Task<string> CallbackJson(string url, string postData, string code, long tranId = 0, int type = 0)
        {
            NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Request", code, tranId.ToString(), url, postData });

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
                        NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Response", code, tranId.ToString(), url, postData, responseContent });
                        try
                        {
                            if (responseContent.Contains("1|"))
                            {
                                NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Process TRUE", responseContent });
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
                                NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Process FAIL", responseContent });
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
                if (type == 1)
                {
                    new TopupMobile3rdLog().UpdateCallback(tranId, -1, null);
                }
                NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Error", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }

    }
}