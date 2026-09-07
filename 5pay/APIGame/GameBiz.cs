using System;
using System.Collections.Generic;
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
using APIGame.Entity;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APIGame
{
    public class GameBiz
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public string SendWebTopupCard(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, string sim, string simTarget, string clientId, int slot, int amount)
        {
            //Lấy ra để Process 
            TopupMobileLog topupProcess;
            switch (telco.ToUpper())
            {
                case "ZING":
                    topupProcess = new TopupMobileLog().GetProcess("zing", amount, partnerCode, providerCode);
                    break;
                case "GARENA":
                    topupProcess = new TopupMobileLog().GetProcess("garena", amount, partnerCode, providerCode);
                    break;
                case "VCOIN":
                    topupProcess = new TopupMobileLog().GetProcess("vtc", amount, partnerCode, providerCode);
                    break;
                case "GATE":
                    //topupProcess = new TopupMobileLog().GetProcess("dzo", amount, partnerCode, providerCode);
                    topupProcess = new TopupMobileLog().GetProcess("gate", amount, partnerCode, providerCode);
                    break;
                default:
                    topupProcess = new TopupMobileLog().GetProcess("gosu", amount, partnerCode, providerCode);
                    break;
            }


            if (topupProcess == null)
            {
                //TelegramNotify.SendNotify(providerCode, string.Empty, string.Empty, telco, amount, 1);
                NLogLogger.Info(new string[] { "TopupGame", "Order Not Found 1", telco, transactionId, amount.ToString(), partnerCode, providerCode });
                return string.Format("{0}|{1}", -320, 0); //Không tìm thấy transaction
            }

            if (topupProcess.TransactionID == 0)
            {
                NLogLogger.Info(new string[] { "TopupGame", "Order Not Found 2", telco, transactionId, amount.ToString(), partnerCode, providerCode });
                return string.Format("{0}|{1}", -320, 0); //Không tìm thấy transaction
            }

            NLogLogger.Info(new string[] { "TopupGame", topupProcess.TransactionID.ToString(), topupProcess.FullName, topupProcess.Mobile, topupProcess.TopupType.ToString() });


            var cardType = String.Empty;
            switch (telco)
            {
                case "gosu":
                    cardType = "gcard";
                    break;
                case "gate":
                    cardType = "fpt";
                    break;
                case "vcoin":
                    cardType = "vtc";
                    break;
                case "bit":
                    cardType = "bit";
                    break;
                case "zing":
                    cardType = "zing";
                    break;
                case "garena":
                    cardType = "garena";
                    break;
                default:
                    cardType = "";
                    break;
            }

            if (!string.IsNullOrEmpty(cardType))
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
                topup.BidRate = topupProcess.BidRate;
                topup.CreateTime = DateTime.Now;
                topup.Core = "Recharge";
                topup.Add();
                topup.Id = topup.ReturnValue;

                try
                {


                    APIResponse res = null;

                    if (topupProcess.Telco.ToLower() == "gosu") // Nap qua hệ thống GOSU
                    {
                        NLogLogger.Info(new string[] { "TopupGosu", transactionId, "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                        var tryAgain = 0;
                        res = GosuService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), cardType, topupProcess.AccountName.ToLower(), topupProcess.Password);
                        while ((res.ResponseCode == (int)ResponseCode.ParameterInvalid
                                || res.ResponseCode == (int)ResponseCode.TransactionTimeout
                                || res.ResponseCode == (int)ResponseCode.TransactionRejected
                                || res.ResponseCode == (int)ResponseCode.SystemBusy)
                               && tryAgain < 3)
                        {
                            Thread.Sleep(16000);
                            res = GosuService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), cardType, topupProcess.AccountName.ToLower(), topupProcess.Password);
                            tryAgain++;
                        }
                        NLogLogger.Info(new string[] { "TopupGosu", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                    }

                    if (topupProcess.Telco.ToLower() == "dzo") // Nap qua hệ thống DZO
                    {
                        NLogLogger.Info(new string[] { "TopupDzo", transactionId, "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                        var tryAgain = 0;
                        res = DzoService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), transactionId.ToString(), topup.Id.ToString(), topupProcess.AccountName.ToLower(), topupProcess.Password);
                        while ((res.ResponseCode == (int)ResponseCode.ParameterInvalid
                                || res.ResponseCode == (int)ResponseCode.TransactionTimeout
                                || res.ResponseCode == (int)ResponseCode.TransactionRejected
                                || res.ResponseCode == (int)ResponseCode.SystemBusy)
                               && tryAgain < 3)
                        {
                            res = DzoService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), transactionId.ToString(), topup.Id.ToString(), topupProcess.AccountName.ToLower(), topupProcess.Password);
                            tryAgain++;
                        }
                        NLogLogger.Info(new string[] { "TopupDzo", transactionId, "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                    }

                    if (topupProcess.Telco.ToLower() == "gate") // Nap qua hệ thống GATE
                    {
                        NLogLogger.Info(new string[] { "TopupGATE", transactionId, topup.Id.ToString(), "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                        var tryAgain = 0;
                        res = GateService.TopupCard(topup.Id, serial, pin, topupProcess.Mobile.ToLower(), topupProcess.AccountName, topupProcess.Password);
                        while ((res.ResponseCode == (int)ResponseCode.TransactionTimeout
                                || res.ResponseCode == (int)ResponseCode.AccountLocked
                                || res.ResponseCode == (int)ResponseCode.ParameterInvalid)
                               && tryAgain < 5)
                        {
                            res = GateService.TopupCard(topup.Id, serial, pin, topupProcess.Mobile.ToLower(), topupProcess.AccountName, topupProcess.Password);
                            tryAgain++;
                        }
                        NLogLogger.Info(new string[] { "TopupGATE", transactionId, topup.Id.ToString(), "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                    }

                    if (topupProcess.Telco.ToLower() == "zing") // Nap qua hệ thống Zing
                    {

                        var gameType = 0;
                        var gameName = string.Empty;
                        switch (topupProcess.TopupType)
                        {
                            case 9:
                                gameType = 1; //Võ Lâm Truyền Kỳ Miễn Phí
                                gameName = "Võ Lâm Truyền Kỳ Miễn Phí";
                                break;
                            case 11:
                                gameType = 2; //VLTK - Công Thành Chiến
                                gameName = "VLTK - Công Thành Chiến";
                                break;
                            case 12:
                                gameType = 3; //Võ Lâm Truyền Kỳ 1
                                gameName = "Võ Lâm Truyền Kỳ 1";
                                break;
                            case 13:
                                gameType = 4; //Kiếm Thế
                                gameName = "Kiếm Thế";
                                break;
                            case 14:
                                gameType = 5; //Tân Thiên Long 3D
                                gameName = "Tân Thiên Long 3D";
                                break;
                            case 15:
                                gameType = 6; //Võ Lâm Truyền Kỳ 2
                                gameName = "Võ Lâm Truyền Kỳ 2";
                                break;
                            case 16:
                                gameType = 7; //Võ Lâm Truyền Kỳ 2
                                gameName = "Võ Lâm Truyền Kỳ - Mobile";
                                break;
                            case 17:
                                gameType = 8; //Danh Tướng 3Q
                                gameName = "Danh Tướng 3Q - Mobile";
                                break;
                        }

                        NLogLogger.Info(new string[] { "TopupZing", transactionId, topup.Id.ToString(), "Request", gameName, serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString(), gameType.ToString() });
                        var tryAgain = 0;

                        res = ZingService.TopupCard(serial, pin, cardType, topupProcess.AccountName.ToLower(), topupProcess.Password, transactionId, topup.Id.ToString(), gameType, topupProcess.ExtData);
                        while ((res.ResponseCode == (int)ResponseCode.TransactionTimeout) && tryAgain < 3)
                        {
                            //Thread.Sleep(16000);
                            res = ZingService.TopupCard(serial, pin, cardType, topupProcess.AccountName.ToLower(), topupProcess.Password, transactionId, topup.Id.ToString(), gameType, topupProcess.ExtData);
                            tryAgain++;
                        }
                        NLogLogger.Info(new string[] { "TopupZing", transactionId, topup.Id.ToString(), "Response", gameName, serial, pin, topupProcess.Mobile, serializer.Serialize(res) });

                    }

                    if (topupProcess.Telco.ToLower() == "garena") // Nap qua hệ thống garena
                    {
                        NLogLogger.Info(new string[] { "TopupGarena", transactionId, topup.Id.ToString(), "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                        var tryAgain = 0;
                        res = GarenaService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), transactionId, topup.Id.ToString(), topupProcess.AccountName, topupProcess.Password, (int)topupProcess.TopupType);
                        while ((res.ResponseCode == (int)ResponseCode.TransactionTimeout
                                || res.ResponseCode == (int)ResponseCode.AccountLocked
                                || res.ResponseCode == (int)ResponseCode.ParameterInvalid)
                               && tryAgain < 3)
                        {
                            res = GarenaService.TopupCard(serial, pin, topupProcess.Mobile.ToLower(), transactionId, topup.Id.ToString(), topupProcess.AccountName, topupProcess.Password, (int)topupProcess.TopupType);
                            tryAgain++;
                        }
                        NLogLogger.Info(new string[] { "TopupGarena", transactionId, topup.Id.ToString(), "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                    }

                    if (topupProcess.Telco.ToLower() == "vtc") // Nap qua hệ thống VTC
                    {
                        NLogLogger.Info(new string[] { "TopupVTC", transactionId, topup.Id.ToString(), "Request", serial, pin, topupProcess.Mobile, topupProcess.TopupType.ToString() });
                        var tryAgain = 0;
                        res = VTCService.TopupCard(topup.Id, serial, pin, topupProcess.Mobile.ToLower(), topupProcess.AccountName, topupProcess.Password);
                        while ((res.ResponseCode == (int)ResponseCode.TransactionTimeout
                                || res.ResponseCode == (int)ResponseCode.AccountLocked
                                || res.ResponseCode == (int)ResponseCode.ParameterInvalid)
                               && tryAgain < 3)
                        {
                            res = VTCService.TopupCard(topup.Id, serial, pin, topupProcess.Mobile.ToLower(), topupProcess.AccountName, topupProcess.Password);
                            tryAgain++;
                        }
                        NLogLogger.Info(new string[] { "TopupVTC", transactionId, topup.Id.ToString(), "Response", serial, pin, topupProcess.Mobile, serializer.Serialize(res) });
                    }

                    if (res != null)
                    {
                        var response = res;
                        topup.LogContent = serializer.Serialize(res);

                        switch (response.ResponseCode)
                        {
                            case (int)ResponseCode.TransactionSuccessful:
                                int amountresponse = Convert.ToInt32(response.ResponseContent);
                                if (amountresponse > 0)
                                {
                                    topup.Status = (int)ResponseCode.TransactionSuccessful;
                                    topup.Amount = amountresponse;
                                    topup.Update();
                                    topupProcess.Topup(1, amountresponse, topup.BidRate); // Thanh công update Amount
                                    //topupProcess.Topup(0, 0); // mở lại cho chạy

                                    //Callback to Provider
                                    if (!string.IsNullOrEmpty(topupProcess.CallbackUrl))
                                    {
                                        var callbackData = new DataCallbackOrder()
                                        {
                                            OrderId = topupProcess.TransactionID,
                                            Amount = amountresponse,
                                            Status = (int)ResponseCode.TransactionSuccessful,
                                            CardSerial = topup.CardSerial,
                                            CardCode = topup.CardCode,
                                            UpdateTime = DateTime.Now,
                                            CreatTime = topup.CreateTime,
                                            Signature = string.Empty,
                                            BidRate = topup.BidRate,
                                            Description = response.Description
                                        };

                                        //Callback Provider
                                        Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, topup.Id, 1)).ConfigureAwait(false);
                                    }

                                    //Check Timeout
                                    var cardApiLog = new CardAPILog().Get(Convert.ToInt32(topup.TransactionId));
                                    if (cardApiLog != null)
                                    {
                                        if (cardApiLog.Status == (int)ResponseCode.TransactionTimeout)
                                        {
                                            NLogLogger.Info(new string[] { "GameBiz", "Transaction Timeout Process", cardApiLog.TransactionID.ToString(), cardApiLog.CardSerial });

                                            cardApiLog.Amount = Convert.ToInt32(topup.Amount);
                                            cardApiLog.Description = "Callback timeout " + cardApiLog.Amount;
                                            cardApiLog.Status = Convert.ToInt32(topup.Status);
                                            cardApiLog.Update();

                                            //Begin Callback for partner timeout
                                            if (!string.IsNullOrEmpty(cardApiLog.CallbackUrl))
                                            {
                                                var responseCode = cardApiLog.Status;
                                                if (responseCode == (int)ResponseCode.TransactionSuccessful)
                                                {
                                                    if (cardApiLog.CardType == "gate") //The Gate bo qua check sai menh gia
                                                    {
                                                        responseCode = (int)ResponseCode.TransactionSuccessful;
                                                    }
                                                    else
                                                    {
                                                        responseCode = cardApiLog.AmountUser != cardApiLog.Amount ? (int)ResponseCode.CardAmountInvalid : (int)ResponseCode.TransactionSuccessful;
                                                    }
                                                }

                                                var privateKey = new Partners().Get(topup.PartnerCode).PrivateKey;
                                                var datacb = new DataCallback()
                                                {
                                                    Amount = Convert.ToInt32(cardApiLog.Amount),
                                                    RefCode = cardApiLog.RequestNo,
                                                    Status = responseCode,
                                                    Signature = Libs.Utils.Encrypts.MD5(cardApiLog.RequestNo + responseCode + topup.Amount + privateKey)
                                                };
                                                Task.Run(async () => await CallbackJson(cardApiLog.CallbackUrl, serializer.Serialize(datacb), cardApiLog.PartnerCode + " " + topup.TransactionId).ConfigureAwait(false));
                                            }
                                        }
                                    }

                                    return string.Format("{0}|{1}", (int)ResponseCode.TransactionSuccessful, amountresponse);
                                }
                                else
                                {
                                    topup.Status = (int)ResponseCode.ServiceIsLocked;
                                    topup.Amount = 0;
                                    topup.Update();
                                    topupProcess.Topup(-1, amountresponse); // Lock
                                    topupProcess.Topup(0, 0); // mở lại cho chạy
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
                                            UpdateTime = DateTime.Now,
                                            CreatTime = topup.CreateTime,
                                            Signature = string.Empty,
                                            BidRate = topup.BidRate,
                                            Description = response.Description
                                        };
                                        Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, topup.Id, 1)).ConfigureAwait(false);
                                    }
                                    //TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, 2);
                                    return string.Format("{0}|{1}", (int)ResponseCode.ServiceIsLocked, 0);
                                }

                            case (int)ResponseCode.TransactionSuspicious:
                                topup.Status = (int)ResponseCode.TransactionSuspicious;
                                topup.Amount = 0;
                                topup.Update();
                                topupProcess.Topup(1, amount);
                                topupProcess.Topup(-6, 0); // Stop đơn để review
                                if (!string.IsNullOrEmpty(topupProcess.CallbackUrl))
                                {
                                    var callbackData = new DataCallbackOrder()
                                    {
                                        OrderId = topupProcess.TransactionID,
                                        Amount = 0,
                                        Status = (int)ResponseCode.TransactionReview,
                                        CardSerial = topup.CardSerial,
                                        CardCode = topup.CardCode,
                                        UpdateTime = DateTime.Now,
                                        CreatTime = topup.CreateTime,
                                        Signature = string.Empty,
                                        BidRate = topup.BidRate,
                                        Description = response.Description
                                    };
                                    Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, topup.Id, 1)).ConfigureAwait(false);
                                }
                                return string.Format("{0}|{1}", (int)ResponseCode.TransactionSuspicious, 0);

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
                                        UpdateTime = DateTime.Now,
                                        CreatTime = topup.CreateTime,
                                        Signature = string.Empty,
                                        BidRate = topup.BidRate,
                                        Description = response.Description
                                    };
                                    Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, topup.Id, 1)).ConfigureAwait(false);
                                }
                                //TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, 2, response.Description);
                                return string.Format("{0}|{1}", (int)ResponseCode.ServiceIsLocked, 0);

                            case (int)ResponseCode.TransactionLimit:
                                topupProcess.Topup(-4, 0);
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
                                        Status = (int)ResponseCode.TransactionLimit,
                                        CardSerial = topup.CardSerial,
                                        CardCode = topup.CardCode,
                                        UpdateTime = DateTime.Now,
                                        CreatTime = topup.CreateTime,
                                        Signature = string.Empty,
                                        BidRate = topup.BidRate,
                                        Description = response.Description
                                    };
                                    Task.Run(async () => await CallbackJson(topupProcess.CallbackUrl, serializer.Serialize(callbackData), topupProcess.AccountName, topup.Id, 1)).ConfigureAwait(false);
                                }
                                //TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, 2, response.Description);
                                return string.Format("{0}|{1}", (int)ResponseCode.TransactionLimit, 0);

                            case (int)ResponseCode.TransactionIgnore:
                                topupProcess.Topup(0, 0); //Ignore
                                topup.Status = response.ResponseCode;
                                topup.Amount = 0;
                                topup.Update();
                                TelegramNotify.SendNotify(providerCode, topupProcess.UserName, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, -2, serial);
                                return string.Format("{0}|{1}", (int)ResponseCode.TransactionIgnore, 0);

                            case (int)ResponseCode.SystemBusy:
                                topupProcess.Topup(0, 0); //Ignore
                                topup.Status = response.ResponseCode;
                                topup.Amount = 0;
                                topup.Update();
                                return string.Format("{0}|{1}", (int)ResponseCode.SystemBusy, 0);

                            case (int)ResponseCode.CardHasExpired:
                                topupProcess.Topup(0, 0); //Ignore
                                topup.Status = response.ResponseCode;
                                topup.Amount = 0;
                                topup.Update();
                                return string.Format("{0}|{1}", (int)ResponseCode.CardHasExpired, 0);

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
                    NLogLogger.Info(new string[] { "TopupGame", "Error", ex.Message, ex.StackTrace });
                    return string.Format("{0}|{1}", -1, 0);
                }
            }

            topupProcess.Topup(-1, 0);
            return string.Format("{0}|{1}", (int)ResponseCode.ServiceIsLocked, 0);

        }
        public async Task<string> CallbackJson(string url, string postData, string code, long tranId = 0, int type = 0)
        {
            NLogLogger.Info(new string[] { "GameBiz", "Callback Type", type.ToString(), "Request", code, tranId.ToString(), url, postData });

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
                        NLogLogger.Info(new string[] { "GameBiz", "Callback Type", type.ToString(), "Response", code, tranId.ToString(), url, postData, responseContent });
                        try
                        {
                            if (responseContent.Contains("1|"))
                            {
                                NLogLogger.Info(new string[] { "GameBiz", "Callback Type", type.ToString(), "Process TRUE", responseContent });
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
                                NLogLogger.Info(new string[] { "GameBiz", "Callback Type", type.ToString(), "Process FAIL", responseContent });
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
                            NLogLogger.Info(new string[] { "GameBiz", "Callback Type", type.ToString(), "Error", e.Message });
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
                NLogLogger.Info(new string[] { "GameBiz", "Callback Type", type.ToString(), "Error", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }

        public string SendWebBuyCard(string transactionId, string telco, int amount, int quanity, string partnerCode, string providerCode, string clientId)
        {
            //var userName = "pp_dl1"; //Fix cứng lấy đơn từ TK này
            //Lấy ra để Process 
            TopupMobileLog topupProcess;
            switch (telco.ToUpper())
            {
                case "VMS":
                    topupProcess = new TopupMobileLog().GetProcess("gate", amount, providerCode, quanity);
                    break;
                case "VNP":
                    topupProcess = new TopupMobileLog().GetProcess("gate", amount, providerCode, quanity);
                    break;
                case "VTT":
                    topupProcess = new TopupMobileLog().GetProcess("gate", amount, providerCode, quanity);
                    break;
                default:
                    topupProcess = new TopupMobileLog().GetProcess("", amount, providerCode, quanity);
                    break;
            }

            if (topupProcess == null)
            {
                //TelegramNotify.SendNotify(providerCode, string.Empty, string.Empty, telco, amount, 1);
                NLogLogger.Info(new string[] { "GameBiz", "SendWebBuyCard", "Order Not Found", partnerCode, providerCode, telco, transactionId, amount.ToString() });
                return serializer.Serialize(new DataResponse()
                {
                    code = (int)ResponseCode.TransactionNotExists,
                    message = "Không tìm thấy transaction"
                });

            }

            APIResponse res = null;

            if (topupProcess.Telco.ToLower() == "gate") // Lấy qua hệ thống Gate
            {
                NLogLogger.Info(new string[] { "GameBiz", "SendWebBuyCard", "BuyCard GATE", transactionId, "Request", telco, amount.ToString() });

                //Add Log
                var topup = new TopupMobile3rdLog();
                topup.RequestNo = topupProcess.TransactionID;
                topup.TransactionId = Convert.ToInt64(transactionId);
                topup.PartnerCode = partnerCode;
                topup.ProviderCode = providerCode;
                topup.Telco = topupProcess.Telco;
                topup.ClientId = clientId;
                topup.Sim = topupProcess.Mobile;
                topup.SimTarget = topupProcess.Mobile;
                topup.Amount = amount;
                topup.CreateTime = DateTime.Now;
                topup.CardSerial = string.Empty;
                topup.CardCode = string.Empty;
                topup.Slot = quanity;
                topup.Core = "ByCard";
                topup.Add();
                topup.Id = topup.ReturnValue;
                //

                var tryAgain = 0;
                res = GateService.BuyCardMaster(topup.Id, telco, amount, quanity, topupProcess.AccountName.ToLower(), topupProcess.Password);
                while ((res.ResponseCode == (int)ResponseCode.ParameterInvalid
                        || res.ResponseCode == (int)ResponseCode.TransactionTimeout
                        || res.ResponseCode == (int)ResponseCode.TransactionRejected
                        || res.ResponseCode == (int)ResponseCode.SystemBusy)
                       && tryAgain < 3)
                {
                    Thread.Sleep(1000);

                    NLogLogger.Info(new string[] { "GameBiz", "SendWebBuyCard", "BuyCard GATE ResCode", res.ResponseCode.ToString(), res.Description, "Try", tryAgain.ToString(), transactionId, "Request", telco, amount.ToString() });
                    res = GateService.BuyCardMaster(topup.Id, telco, amount, quanity, topupProcess.AccountName.ToLower(), topupProcess.Password);
                    tryAgain++;
                }

                if (res != null)
                {
                    var response = res;
                    topup.LogContent = serializer.Serialize(res);

                    switch (response.ResponseCode)
                    {
                        case (int)ResponseCode.TransactionSuccessful:
                            topup.Status = (int)ResponseCode.TransactionSuccessful;
                            topup.Update();
                            topupProcess.Cash(1, amount * quanity, topup.BidRate);

                            return serializer.Serialize(new DataResponse()
                            {
                                code = (int)ResponseCode.TransactionSuccessful,
                                message = response.Description,
                                content = res.ResponseContent
                            });


                        case (int)ResponseCode.ServiceIsLocked:
                            topupProcess.Cash(-1, 0);
                            topup.Status = response.ResponseCode;
                            topup.Amount = 0;
                            topup.Update();

                            return serializer.Serialize(new DataResponse()
                            {
                                code = (int)ResponseCode.ServiceIsLocked,
                                message = response.Description
                            });


                        case (int)ResponseCode.TransactionLimit:
                            topupProcess.Cash(-4, 0);
                            topup.Status = response.ResponseCode;
                            topup.Amount = 0;
                            topup.Update();

                            return serializer.Serialize(new DataResponse()
                            {
                                code = (int)ResponseCode.TransactionLimit,
                                message = response.Description
                            });

                        default:
                            topup.Status = response.ResponseCode;
                            topup.Amount = 0;
                            topup.Update();
                            topupProcess.Cash(0, 0);

                            return serializer.Serialize(new DataResponse()
                            {
                                code = response.ResponseCode,
                                message = response.Description
                            });
                    }

                }

            }

            return serializer.Serialize(new DataResponse()
            {
                code = (int)ResponseCode.TransactionFailed,
                message = "Transaction Failed"
            });
        }

        public string SendWebTranferBalance(string transactionId, string telco, int amount, string simTarget, string partnerCode, string providerCode, string clientId)
        {
            //var userName = "pp_dl1"; //Fix cứng lấy đơn từ TK này
            //Lấy ra để Process 
            TopupMobileLog topupProcess;
            switch (telco.ToUpper())
            {
                case "VMS":
                    topupProcess = new TopupMobileLog().GetProcess("gate", amount, providerCode, 1);
                    break;
                case "VNP":
                    topupProcess = new TopupMobileLog().GetProcess("gate", amount, providerCode, 1);
                    break;
                case "VTT":
                    topupProcess = new TopupMobileLog().GetProcess("gate", amount, providerCode, 1);
                    break;
                default:
                    topupProcess = new TopupMobileLog().GetProcess("", amount, providerCode, 1);
                    break;
            }

            if (topupProcess == null)
            {
                //TelegramNotify.SendNotify(providerCode, string.Empty, string.Empty, telco, amount, 1);
                NLogLogger.Info(new string[] { "GameBiz", "SendWebTranferBalance", "Order Not Found", partnerCode, providerCode, telco, transactionId, amount.ToString() });
                
            }

            APIResponse res = null;

            if (topupProcess.Telco.ToLower() == "gate") // Lấy qua hệ thống Gate
            {
                NLogLogger.Info(new string[] { "GameBiz", "SendWebTranferBalance", "Tranfer GATE", transactionId, "Request", telco, amount.ToString() });

                //Add Log
                var topup = new TopupMobile3rdLog();
                topup.RequestNo = topupProcess.TransactionID;
                topup.TransactionId = Convert.ToInt64(transactionId);
                topup.PartnerCode = partnerCode;
                topup.ProviderCode = providerCode;
                topup.Telco = topupProcess.Telco;
                topup.ClientId = clientId;
                topup.Sim = topupProcess.Mobile;
                topup.SimTarget = simTarget;
                topup.Amount = amount;
                topup.CreateTime = DateTime.Now;
                topup.CardSerial = string.Empty;
                topup.CardCode = string.Empty;
                topup.Slot = 1;
                topup.Core = "Tranfer";
                topup.Add();
                topup.Id = topup.ReturnValue;
                //

                var tryAgain = 0;
                res = GateService.TranferMaster(topup.Id, telco, amount, simTarget, topupProcess.AccountName.ToLower(), topupProcess.Password);
                while ((res.ResponseCode == (int)ResponseCode.ParameterInvalid
                        || res.ResponseCode == (int)ResponseCode.TransactionTimeout
                        || res.ResponseCode == (int)ResponseCode.TransactionRejected
                        //|| res.ResponseCode == (int)ResponseCode.SystemBusy
                        )
                       && tryAgain < 3)
                {
                    Thread.Sleep(1000);

                    NLogLogger.Info(new string[] { "GameBiz", "SendWebTranferBalance", "Tranfer GATE ResCode", res.ResponseCode.ToString(), res.Description, "Try", tryAgain.ToString(), transactionId, "Request", telco, amount.ToString() });
                    res = GateService.TranferMaster(topup.Id, telco, amount, simTarget, topupProcess.AccountName.ToLower(), topupProcess.Password);
                    tryAgain++;
                }

                if (res != null)
                {
                    var response = res;
                    topup.LogContent = serializer.Serialize(res);

                    switch (response.ResponseCode)
                    {
                        case (int)ResponseCode.TransactionSuccessful:
                            topup.Status = (int)ResponseCode.TransactionSuccessful;
                            topup.Update();
                            topupProcess.Cash(1, amount, topup.BidRate);

                            return serializer.Serialize(new DataResponse()
                            {
                                code = (int)ResponseCode.TransactionSuccessful,
                                message = response.Description,
                                content = res.ResponseContent
                            });


                        case (int)ResponseCode.ServiceIsLocked:
                            topupProcess.Cash(-1, 0);
                            topup.Status = response.ResponseCode;
                            topup.Amount = 0;
                            topup.Update();

                            return serializer.Serialize(new DataResponse()
                            {
                                code = (int)ResponseCode.ServiceIsLocked,
                                message = response.Description
                            });


                        case (int)ResponseCode.TransactionLimit:
                            topupProcess.Cash(-4, 0);
                            topup.Status = response.ResponseCode;
                            topup.Amount = 0;
                            topup.Update();

                            return serializer.Serialize(new DataResponse()
                            {
                                code = (int)ResponseCode.TransactionLimit,
                                message = response.Description
                            });

                        case (int)ResponseCode.SystemBusy:
                            topup.Status = response.ResponseCode;
                            topup.Amount = 0;
                            topup.Update();
                            topupProcess.Cash(0, 0);

                            return serializer.Serialize(new DataResponse()
                            {
                                code = (int)ResponseCode.SystemBusy,
                                message = response.Description
                            });

                        default:
                            topup.Status = response.ResponseCode;
                            topup.Amount = 0;
                            topup.Update();
                            topupProcess.Cash(0, 0);

                            return serializer.Serialize(new DataResponse()
                            {
                                code = (int)ResponseCode.TransactionFailed,
                                message = response.Description
                            });
                    }

                }

            }

            return serializer.Serialize(new DataResponse()
            {
                code = (int)ResponseCode.TransactionFailed,
                message = "Transaction Failed"
            });
        }

        public string SendWebTranferAccountBalance(string transactionId, string telco, int type, string clientId)
        {
            var userName = "b2b_dl"; //HardCode chi lay Account nay
            TopupMobileLog topupProcess;
            string mobileDes = string.Empty;
            Int64 idDes = 0;
            var amountShare = 0;

            NLogLogger.Info(new string[] { "GameBiz", "SendWebTranferAccountBalance", "Request GetProcess", transactionId, userName, telco, type.ToString(), clientId });
            switch (telco.ToUpper())
            {
                case "GATE":
                    topupProcess = new TopupMobileLog().GetProcess("gate", userName, type, ref mobileDes, ref idDes);
                    break;
                default:
                    topupProcess = new TopupMobileLog().GetProcess("", userName, type, ref mobileDes, ref idDes);
                    break;
            }

            if (topupProcess == null || idDes == 0)
            {
                //TelegramNotify.SendNotify(providerCode, string.Empty, string.Empty, telco, amount, 1);
                NLogLogger.Info(new string[] { "GameBiz", "SendWebTranferAccountBalance", type.ToString(), "Order Not Found", telco, transactionId, "Result", serializer.Serialize(topupProcess), idDes.ToString() });
                return serializer.Serialize(new DataResponse()
                {
                    code = (int)ResponseCode.TransactionNotExists,
                    message = "Không tìm thấy transaction"
                });
            }

            switch (type)
            {
                case 1:
                    amountShare = (int)topupProcess.AmountTopupSuccess;
                    break;
                case 2:
                    amountShare = 262500;
                    //amountShare = 525000;
                    break;
            }

            APIResponse res = null;

            if (topupProcess.Telco.ToLower() == "gate") // Lấy qua hệ thống Gate
            {
                NLogLogger.Info(new string[] { "GameBiz", "SendWebTranferAccountBalance", "Tranfer GATE", transactionId, "Request", topupProcess.Mobile, mobileDes, amountShare.ToString() });
                var tryAgain = 0;
                res = GateService.TranferAccountMaster(transactionId, topupProcess.Mobile, mobileDes, amountShare, topupProcess.AccountName, topupProcess.Password);
                while ((res.ResponseCode == (int)ResponseCode.ParameterInvalid
                        || res.ResponseCode == (int)ResponseCode.TransactionTimeout
                        || res.ResponseCode == (int)ResponseCode.TransactionRejected
                        || res.ResponseCode == (int)ResponseCode.SystemBusy)
                       && tryAgain < 3)
                {
                    Thread.Sleep(1000);

                    NLogLogger.Info(new string[] { "GameBiz", "SendWebTranferAccountBalance", "Tranfer GATE ResCode", res.ResponseCode.ToString(), res.Description, "Try", tryAgain.ToString(), transactionId, "Request", topupProcess.Mobile, mobileDes, amountShare.ToString() });
                    res = GateService.TranferAccountMaster(transactionId, topupProcess.Mobile, mobileDes, amountShare, topupProcess.AccountName, topupProcess.Password);
                    tryAgain++;
                }
                NLogLogger.Info(new string[] { "GameBiz", "SendWebTranferAccountBalance", "Tranfer GATE", transactionId, "Response", transactionId, topupProcess.Mobile, mobileDes, amountShare.ToString(), serializer.Serialize(res) });

                if (res != null)
                {
                    var response = res;
                    switch (response.ResponseCode)
                    {
                        case (int)ResponseCode.TransactionSuccessful:
                            topupProcess.TranferAccountConfirm(topupProcess.TransactionID, idDes, amountShare, 1, (int)topupProcess.Status, (int)topupProcess.StatusCash);
                            return serializer.Serialize(new DataResponse()
                            {
                                code = (int)ResponseCode.TransactionSuccessful,
                                message = response.Description,
                                content = res.ResponseContent
                            });
                        case (int)ResponseCode.BalanceNotEnough:
                            topupProcess.TranferAccountConfirm(topupProcess.TransactionID, idDes, Convert.ToInt32(response.ResponseContent), -51, (int)topupProcess.Status, (int)topupProcess.StatusCash);
                            return serializer.Serialize(new DataResponse()
                            {
                                code = (int)ResponseCode.BalanceNotEnough,
                                message = response.Description,
                                content = res.ResponseContent
                            });

                        default:
                            topupProcess.TranferAccountConfirm(topupProcess.TransactionID, idDes, 0, -1, (int)topupProcess.Status, (int)topupProcess.StatusCash);
                            return serializer.Serialize(new DataResponse()
                            {
                                code = (int)ResponseCode.TransactionFailed,
                                message = response.Description
                            });
                    }

                }


            }

            return serializer.Serialize(new DataResponse()
            {
                code = (int)ResponseCode.TransactionFailed,
                message = "Transaction Failed"
            });
        }


    }
}