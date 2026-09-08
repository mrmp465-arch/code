using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using APIMobiNext.Entity;
using APIMobiNext.GSM;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APIMobiNext
{
    public class MobiNextBiz
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public string SendWebTopupCard(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, string sim, string simTarget, string clientId, int slot, int amount)
        {
            //Lấy ra để Process

            //var provider = new Providers().Get(providerCode);

            //if (provider.SignatureType == 1) //Có chạy USSD
            //{
            //    var getListSimReady = GSMServiceBiz.GetListSimGSMStatus();
            //    var strSimReady = string.Join(",", getListSimReady);
            //    NLogLogger.Info(new string[] { "GSMService", "Sim Ready", strSimReady });
            //    topupProcess = new TopupMobileLog().GetProcess(telco, amount, partnerCode, providerCode, strSimReady);
            //}
            //else
            //{
            //    topupProcess = new TopupMobileLog().GetProcess(telco, amount, partnerCode, providerCode);
            //}

            TopupMobileLog topupProcess = new TopupMobileLog();

            if (slot == 1) //Có chạy USSD
            {
                var getListSimReady = GSMServiceBiz.GetListSimGSMStatus();
                var strSimReady = string.Join(",", getListSimReady);
                NLogLogger.Info(new string[] { "GSMService", "Sim Ready", strSimReady });
                topupProcess = new TopupMobileLog().GetProcess(telco, amount, partnerCode, providerCode, strSimReady);
            }
            else
            {
                topupProcess = new TopupMobileLog().GetProcess(telco, amount, partnerCode, providerCode);
            }

            if (topupProcess == null)
            {
                TelegramNotify.SendNotify(providerCode, string.Empty, telco, amount, 1);
                NLogLogger.Info(new string[] { "TopupMobiNext", "Order Not Found 1", transactionId, amount.ToString(), partnerCode, providerCode });
                return string.Format("{0}|{1}", -320, 0); //Không tìm thấy transaction
            }

            if (topupProcess.TransactionID == 0)
            {
                NLogLogger.Info(new string[] { "TopupMobiNext", "Order Not Found 2", transactionId, amount.ToString(), partnerCode, providerCode });
                return string.Format("{0}|{1}", -320, 0); //Không tìm thấy transaction
            }

            NLogLogger.Info(new string[] { "TopupMobiNext", topupProcess.TransactionID.ToString(), topupProcess.FullName, topupProcess.Mobile });

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
                topup.Add();
                topup.Id = topup.ReturnValue;

                if ((topupProcess.CountCharge >= 5 && topupProcess.Ussd == 1) || (topupProcess.Ussd == 2))
                {
                    //Send Request to GSMService
                    try
                    {
                        NLogLogger.Info(new string[] { "GSMService", transactionId, "Request", topup.Id.ToString(), telco, topupProcess.Mobile, String.Empty, serial, pin, topupProcess.TopupType.ToString() });
                        var res = GSMServiceBiz.SendChardRequest(topup.Id.ToString(), telco, topupProcess.Mobile, String.Empty, serial, pin, Convert.ToInt32(topupProcess.TopupType));
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
                                DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.SimNotActive, string.Empty);
                                topupProcess.Topup(0, 0); //Mở chạy lại
                                return "-401|0";

                            default:
                                DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.TransactionRejected, string.Empty);
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
                    //Send Request to MobiNext
                    try
                    {

                        var parameters = new TopupRequest
                        {
                            pin = pin,
                            phoneNumber = topupProcess.Mobile,
                            accountName = topupProcess.AccountName,
                            token = topupProcess.Password,
                            serial = string.Empty
                        };
                        APIResponse response = null;
                        NLogLogger.Info(new string[] { "TopupMobiNext", transactionId, "Request", serializer.Serialize(parameters) });

                        var tryAgain = 0;
                        response = MobiNextService.TopupCard(parameters.serial, parameters.pin, parameters.phoneNumber, Convert.ToInt32(topupProcess.TopupType), parameters.accountName, parameters.token, string.Empty);
                        //while ((response.ResponseCode == (int)ResponseCode.TransactionTimeout || response.ResponseCode == (int)ResponseCode.ParameterInvalid) && tryAgain < 3)  
                        while ((response.ResponseCode == (int)ResponseCode.ParameterInvalid || response.ResponseCode == (int)ResponseCode.SystemBusy) && tryAgain < 3)
                        {
                            if (response.ResponseCode == (int)ResponseCode.ParameterInvalid)
                            {
                                var captcha = Utils.DeCaptcha(response.ResponseContent, parameters.accountName);
                                response = MobiNextService.TopupCard(parameters.serial, parameters.pin, parameters.phoneNumber, Convert.ToInt32(topupProcess.TopupType), parameters.accountName, parameters.token, captcha.Value);
                            }
                            else
                            {
                                response = MobiNextService.TopupCard(parameters.serial, parameters.pin, parameters.phoneNumber, Convert.ToInt32(topupProcess.TopupType), parameters.accountName, parameters.token, string.Empty);
                            }

                            tryAgain++;
                            Thread.Sleep(1000);
                        }
                        NLogLogger.Info(new string[] { "TopupMobiNext", transactionId, "Response", serializer.Serialize(response) });
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
                                    topupProcess.Topup(0, 0); // mở lại cho chạy
                                    return string.Format("{0}|{1}", (int)ResponseCode.TransactionSuccessful, response.ResponseContent);

                                case (int)ResponseCode.ServiceIsLocked:
                                    topup.Status = response.ResponseCode;
                                    topup.Amount = 0;
                                    topup.Update();
                                    topupProcess.Topup(-1, 0);
                                    TelegramNotify.SendNotify(providerCode, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, 2);
                                    return string.Format("{0}|{1}", (int)ResponseCode.ServiceIsLocked, 0);

                                case (int)ResponseCode.TransactionLimit:
                                    topup.Status = response.ResponseCode;
                                    topup.Amount = 0;
                                    topup.Update();
                                    topupProcess.Topup(-6, 0); //Update CountCharge for VMS

                                    if (topupProcess.Ussd == 1)
                                    {
                                        //Send Request to GSMService
                                        try
                                        {

                                            NLogLogger.Info(new string[] { "GSMService", transactionId, "Request", topup.Id.ToString(), telco, topupProcess.Mobile, String.Empty, serial, pin, topupProcess.TopupType.ToString() });
                                            var res = GSMServiceBiz.SendChardRequest(topup.Id.ToString(), telco, topupProcess.Mobile, String.Empty, serial, pin, Convert.ToInt32(topupProcess.TopupType));
                                            NLogLogger.Info(new string[] { "GSMService", transactionId, "Response", serializer.Serialize(res), serializer.Serialize(parameters) });

                                            topupProcess.Topup(0, 0); //Mở chạy lại

                                            switch (res.ResponseCode)
                                            {
                                                case (int)ResponseCode.TransactionSuccessful:
                                                    return Task.Run(async () => await CheckStatusAsync(topup.Id)).Result;

                                                case (int)ResponseCode.SimNotActive:
                                                    DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.SimNotActive, string.Empty);
                                                    topupProcess.Topup(0, 0); //Mở chạy lại
                                                    return "-401|0";

                                                default:
                                                    DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.TransactionRejected, string.Empty);
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
                                        topup.Status = response.ResponseCode;
                                        topup.Amount = 0;
                                        topup.Update();
                                        topupProcess.Topup(-2, 0);
                                        TelegramNotify.SendNotify(providerCode, topupProcess.Mobile, topupProcess.Telco, topupProcess.Amount, 2);
                                        return string.Format("{0}|{1}", (int)ResponseCode.ServiceIsLocked, 0);
                                    }

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
                        NLogLogger.Info(new string[] { "TopupMobiNext", "Error", ex.Message, ex.StackTrace });
                        return string.Format("{0}|{1}", (int)ResponseCode.TransactionFailed, 0);
                    }
                }
            }

            return string.Format("{0}|{1}", (int)ResponseCode.CardProviderInvalid, 0); //Không tìm thấy transaction

        }

        public string SendWebTopupCardAPI(Int64 RequestNo, string partnerCode, string serial, string pin, string sim, int amount, string token)
        {

            var topup = new TopupMobile3rdLog();
            topup.RequestNo = RequestNo;
            topup.TransactionId = 0;
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = "ApiNext";
            topup.Telco = "vms";
            topup.Sim = sim;
            topup.SimTarget = sim;
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.AmountUser = amount;
            topup.Add();
            topup.Id = topup.ReturnValue;


            //Send Request to MobiNext
            try
            {

                var parameters = new TopupRequest
                {
                    pin = pin,
                    phoneNumber = sim,
                    accountName = sim,
                    token = token,
                    serial = string.Empty
                };
                APIResponse response = null;
                NLogLogger.Info(new string[] { "TopupMobiNext API", RequestNo.ToString(), "Request", serializer.Serialize(parameters) });

                var tryAgain = 0;
                response = MobiNextService.TopupCard(parameters.serial, parameters.pin, parameters.phoneNumber, 0, parameters.accountName, parameters.token, string.Empty);
                while ((response.ResponseCode == (int)ResponseCode.ParameterInvalid || response.ResponseCode == (int)ResponseCode.SystemBusy) && tryAgain < 3)
                {
                    if (response.ResponseCode == (int)ResponseCode.ParameterInvalid)
                    {
                        var captcha = Utils.DeCaptcha(response.ResponseContent, parameters.accountName);
                        response = MobiNextService.TopupCard(parameters.serial, parameters.pin, parameters.phoneNumber, 0, parameters.accountName, parameters.token, captcha.Value);
                    }
                    else
                    {
                        response = MobiNextService.TopupCard(parameters.serial, parameters.pin, parameters.phoneNumber, 0, parameters.accountName, parameters.token, string.Empty);
                    }

                    tryAgain++;
                    Thread.Sleep(1000);
                }
                NLogLogger.Info(new string[] { "TopupMobiNext API", RequestNo.ToString(), "Response", serializer.Serialize(response) });
                if (response != null)
                {
                    topup.LogContent = serializer.Serialize(response);
                    switch (response.ResponseCode)
                    {
                        case (int)ResponseCode.TransactionSuccessful:
                            topup.Status = topup.Status = (int)ResponseCode.TransactionSuccessful;
                            topup.Amount = Convert.ToInt32(response.ResponseContent);
                            topup.Update();
                            return serializer.Serialize(new APIResponse((int)ResponseCode.TransactionSuccessful) { ResponseContent = response.ResponseContent });

                        case (int)ResponseCode.ServiceIsLocked:
                            topup.Status = response.ResponseCode;
                            topup.Amount = 0;
                            topup.Update();
                            return serializer.Serialize(new APIResponse((int)ResponseCode.ServiceIsLocked) { Description = response.Description });

                        case (int)ResponseCode.CardUsed:
                            topup.Status = response.ResponseCode;
                            topup.Amount = 0;
                            topup.Update();
                            return serializer.Serialize(new APIResponse((int)ResponseCode.CardUsed) { Description = response.Description });

                        case (int)ResponseCode.CardCodeInvalid:
                            topup.Status = response.ResponseCode;
                            topup.Amount = 0;
                            topup.Update();
                            return serializer.Serialize(new APIResponse((int)ResponseCode.CardCodeInvalid) { Description = response.Description });

                        case (int)ResponseCode.ParameterInvalid:
                            topup.Status = response.ResponseCode;
                            topup.Amount = 0;
                            topup.Update();
                            return serializer.Serialize(new APIResponse((int)ResponseCode.ParameterInvalid) { Description = response.Description });

                        case (int)ResponseCode.SystemBusy:
                            topup.Status = response.ResponseCode;
                            topup.Amount = 0;
                            topup.Update();
                            return serializer.Serialize(new APIResponse((int)ResponseCode.SystemBusy) { Description = response.Description });

                        case (int)ResponseCode.TransactionLimit:
                            topup.Status = response.ResponseCode;
                            topup.Amount = 0;
                            topup.Update();
                            return serializer.Serialize(new APIResponse((int)ResponseCode.TransactionLimit) { Description = response.Description });

                        default:
                            topup.Status = response.ResponseCode;
                            topup.Amount = 0;
                            topup.Update();
                            return serializer.Serialize(new APIResponse((int)ResponseCode.TransactionFailed) { Description = response.Description });
                    }

                }

            }
            catch (Exception ex)
            {
                topup.Status = (int)ResponseCode.TransactionFailed;
                topup.Amount = 0;
                topup.Update();
                NLogLogger.Info(new string[] { "TopupMobiNext API", "Error", ex.Message, ex.StackTrace });

            }
            return serializer.Serialize(new APIResponse((int)ResponseCode.TransactionFailed));
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
    }

}
