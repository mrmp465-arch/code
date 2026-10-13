using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;
using System.Security.Cryptography;
using Libs.Report;
using System.Configuration;

namespace Libs.CardTelco.PayPlusUSSD
{
    public class PayPlusCard : ICardTelcoHandler
    {

        protected string providerName = string.Empty;
        protected string ListMailNotify = string.Empty;
        string serviceUrl = "http://149.28.130.246:1583/";
        //string serviceUrl = "http://callback.bb2d.com/";

        public APIResponse UseCard(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            APIResponse _APIResponse = new APIResponse();
            CardAPILog _CardAPILog = new CardAPILog();

            providerName = transaction.ProviderCode;
            switch (providerName)
            {
                case "ppussd":
                    ListMailNotify = "anhty0005@gmail.com, bb2dpay@gmail.com";
                    break;
                case "bigussd":
                    ListMailNotify = "bb2dpay@gmail.com";
                    break;
                case "glbussd":
                    ListMailNotify = "napthedidong68@gmail.com, bb2dpay@gmail.com";
                    break;
            }

            int step = 0;
            try
            {
                // Bước: Phân tích yêu cầu thành đối tượng
                step = 1;
                UseCardRequest request = new UseCardRequest();
                request = serializer.Deserialize<UseCardRequest>(transaction.RequestContent);

                // Bước: Ghi log giao dịch
                step = 2;
                _CardAPILog.TransactionID = transaction.TransactionID;
                _CardAPILog.PartnerID = transaction.PartnerID;
                _CardAPILog.PartnerCode = transaction.PartnerCode;
                _CardAPILog.AccountName = request.AccountName;
                _CardAPILog.AccountID = 0;
                _CardAPILog.CardSerial = request.CardSerial;
                _CardAPILog.CardCode = request.CardCode;
                _CardAPILog.CardType = request.CardType;
                _CardAPILog.Amount = 0;
                _CardAPILog.AmountUser = request.AmountUser;
                _CardAPILog.Provider = providerName;
                _CardAPILog.Status = 0;
                _CardAPILog.Description = "_CardAPILog.Add";
                _CardAPILog.RequestNo = request.RefCode;
                _CardAPILog.Add();

                // Nếu thêm giao dịch không hợp lệ
                if (_CardAPILog.ReturnValue < 0)
                {
                    return new APIResponse(_CardAPILog.ReturnValue); ;
                }

                // Bước: gọi hàm sang API
                step = 3;
                string telcoCode = "";
                switch (_CardAPILog.CardType.ToLower())
                {
                    case "vms":
                        telcoCode = "vms";
                        break;
                    case "vnp":
                        telcoCode = "vnp";
                        break;
                    case "viettel":
                        telcoCode = "vtt";
                        break;

                }

                var deviceSim = new DeviceSimUSSD().GetSimUSSDCondition(providerName, telcoCode);
                if (deviceSim == null)
                {
                    _APIResponse = new APIResponse((int)ResponseCode.ProviderNotFound);
                    _CardAPILog.Status = _APIResponse.ResponseCode;
                    if (_CardAPILog.TransactionID > 0)
                    {
                        _CardAPILog.Update();
                    }

                    return _APIResponse;
                }

                TopupRequest topupRequest = new TopupRequest();
                //Get SIM thực hiện USSD (LayDB các thông số này theo điều kiện)
                topupRequest.sim = "sim";
                topupRequest.simTarget = string.Empty;
                topupRequest.slot = deviceSim.Slot;
                topupRequest.clientId = deviceSim.ClientId;
                //Samsung
                //topupRequest.slot = 0;
                //topupRequest.clientId = "dzgJcGarka4:APA91bHbfhoBoF61Jt2eeWzmRKy85McdJbR4SKLJMKwxyWZ5AzQ0VpR34I4H4p5soRV36JENf78aHg399BGCCUgOQ-yn8rSmbduuqR2nEES_X_Fa_aVOzcP19VzmXQKHHbkd5NutE1z_yESu7gSRQMfU1FNu1pQK5w";
                //Mi5
                //topupRequest.clientId = "dZunpEVBDos:APA91bGLxXL9z2_YqrBzklJAdAVQYY7AoAl9sluism4ZRMY87-YYSoRl7nEJLFw71pwRejEApdyPWh9FXkGkf5QK4iwVNyz290ss_j8roEWAUR5HeJ068ofYLUGgd0vug0Vaz8NrVnSq7oStlMdVYZ2JNZHN-WpviQ";
                //End
                topupRequest.cardSerial = request.CardSerial;
                topupRequest.cardCode = request.CardCode;
                topupRequest.telco = telcoCode;
                topupRequest.amount = request.AmountUser;
                topupRequest.transactionId = transaction.TransactionID;
                topupRequest.partnerCode = transaction.PartnerCode;
                topupRequest.providerCode = providerName;

                NLogLogger.Info(new string[] { "PayPlusCardTopup", transaction.TransactionID.ToString(), "PayPlusCardTopupRequest", serializer.Serialize(topupRequest) });
                TopupResponse cardResult = ProcessCard(topupRequest);
                NLogLogger.Info(new string[] { "PayPlusCardTopup", transaction.TransactionID.ToString(), "PayPlusCardTopupResponse", serializer.Serialize(cardResult) });

                step = 4;
                // Nếu thành công
                if (cardResult.code == 1)
                {
                    int[] listValue = { 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000 };
                    if (listValue.Contains(cardResult.amount))
                    {
                        //Update SIM Amount
                        if (deviceSim.Quota > 0)
                        {
                            var result = new SimUSSD().UpdateAmout(topupRequest.clientId, topupRequest.slot, cardResult.amount);
                            //Nếu Amount > Quota sẽ bắn notify
                            if (result == 1)
                            {
                                var logMobile3Rd = new TopupMobile3rdLog().GetByTransactionId(transaction.TransactionID);
                                Action<string, string, int> send = (string sim, string deviceName, int slot) =>
                                {
                                    PushNotifyOverQuota(sim, deviceName, slot);
                                };
                                send.BeginInvoke(logMobile3Rd.Sim, deviceSim.Name, logMobile3Rd.Slot, null, null);
                            }
                        }

                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                        _CardAPILog.Amount = Convert.ToInt64(cardResult.amount);
                        _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                        _CardAPILog.Description = "Amount: " + _CardAPILog.Amount + " | Slot: " + topupRequest.slot;
                        _CardAPILog.Status = 1;
                    }
                    else
                    {
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        _CardAPILog.Amount = Convert.ToInt64(cardResult.amount);
                        _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                        _CardAPILog.Description = "Amount: " + _CardAPILog.Amount + " | Slot: " + topupRequest.slot;
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                    }
                }
                else
                {
                    switch (cardResult.code)
                    {
                            //int RESULT_SUCCESS = 1;
                            //int RESULT_INIT = 0;
                            //int RESULT_FAIL = -1;
                            //int RESULT_BLOCK_CARD = -2;
                            //int RESULT_INVALID_BALANCE = -3;
                            //int RESULT_INVALID_AMOUNT = -4;
                            //int RESULT_CARD_USED = -5;
                            //int RESULT_CARD_CODE_INVALID = -6;
                            //int RESULT_SIM_UNAVAILABLE = -7;
                            //int RESULT_NUMBER_NOT_FOUND = -8;
                            //int RESULT_OVER_QUOTA = -9;
                            //int RESULT_QUEUE_FULL = -10;

                        case -1:
                            //Thẻ đã được sử dụng
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -2:
                            //Sim bị khóa đầu nạp
                            _APIResponse = new APIResponse((int)ResponseCode.ServiceIsLocked);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            var logMobile3Rd = new TopupMobile3rdLog().GetByTransactionId(transaction.TransactionID);

                            //Update Sim bi khóa
                            var simUpdate = new SimUSSD().UpdateStatus(topupRequest.clientId, topupRequest.slot, string.Empty, -2);

                            Action<string, string, int, long> send = (string sim, string deviceName, int slot, long id) =>
                              {
                                  PushNotify(sim, deviceName, slot, id);
                              };
                            send.BeginInvoke(logMobile3Rd.Sim, deviceSim.Name, logMobile3Rd.Slot, logMobile3Rd.Id, null, null);

                            break;
                        case -3:
                            //Không lấy được Balance
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -4:
                            //Không lấy được Amount
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -5:
                            //Card used
                            _APIResponse = new APIResponse((int)ResponseCode.CardUsed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -6:
                            //Card code không đúng
                            _APIResponse = new APIResponse((int)ResponseCode.CardCodeInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -9:
                        case -10:
                            //TransactionRejected
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionRejected);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -326:
                            //Timeout
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionTimeout);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -373:
                            //Timeout
                            _APIResponse = new APIResponse((int)ResponseCode.CardProviderInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;

                        default:
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                    }
                    _CardAPILog.Description = serializer.Serialize(cardResult) + " | Slot: " + topupRequest.slot;
                }
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "PayPlusCardTopup", transaction.TransactionID.ToString(), "Error", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "PayPlusCardTopup", transaction.TransactionID.ToString(), "Error", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "PayPlusCardTopup", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - UseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "PayPlusCardTopup", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "PayPlusCardTopup", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }
            }

            if (_CardAPILog.TransactionID > 0)
            {
                _CardAPILog.Update();
            }
            return _APIResponse;
        }

        public APIResponse ReCheck(string transactionId)
        {
            throw new NotImplementedException();
        }

        private TopupResponse ProcessCard(TopupRequest topupRequest)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            NLogLogger.Info(new string[] { "PayPlusCardTopup", "PayPlusCardTopupRequest", serializer.Serialize(topupRequest) });
            TopupMobile _VPGService = new TopupMobile(serviceUrl + "Topup.asmx");
            string serviceResponse = string.Empty;
            serviceResponse = _VPGService.RequestTopup(topupRequest.transactionId.ToString(), topupRequest.telco, topupRequest.partnerCode, topupRequest.providerCode, topupRequest.sim, topupRequest.simTarget, topupRequest.clientId, topupRequest.cardSerial, topupRequest.cardCode, topupRequest.slot, topupRequest.amount);
            NLogLogger.Info(new string[] { "PayPlusCardTopup", "PayPlusCardTopupResponse", serviceResponse });
            return serializer.Deserialize<TopupResponse>(serviceResponse);
        }

        private void PushNotify(string sim, string deviceName, int slot, long id)
        {
            var send = false;
            //while (send == false)
            //{
            //    var data = Encrypts.Encrypt("pay", id.ToString());
            //    var urlUnlock = string.Format("{0}UnlockSim.ashx?data={1}", serviceUrl, Encrypts.Base64Encode(data));
            //    var subject = string.Format("Số SIM {0} đang bị khóa nạp thẻ !", sim);
            //    var body = string.Format("Số SIM <b>{0}</b> trên thiết bị <b>{1}</b> được cắm tại Slot <b>{2}</b> đang bị khóa nạp thẻ, hệ thống đã tự động ngừng phân bổ nạp tiền vào SIM này cho đến khi mở khóa lại.<br/><br/><b>Các bước mở khóa:</b><br/> Bước 1: Thực hiện thao tác mở khóa với nhà mạng.<br/>Bước 2: Ấn vào <a href=\"{3}\"><b>URL UNLOCK</b></a> này hoặc thao tác trên <b>CLIENT MOBILE</b> để báo với hệ thống Payment", sim, deviceName, slot, urlUnlock);
            //    send = EmailService.SendMail(subject, body, ListMailNotify);
            //    NLogLogger.Info(new string[] { "SendMail Lock", send.ToString() });
            //}
        }

        private void PushNotifyOverQuota(string sim, string deviceName, int slot)
        {
            var send = false;
            //while (send == false)
            //{
            //for (int i = 0; i < 5; i++)
            //{
            //    var subject = string.Format("Số SIM {0} đang bị vượt quá hạn mức !", sim);
            //    var body = string.Format("Số SIM <b>{0}</b> trên thiết bị <b>{1}</b> được cắm tại Slot <b>{2}</b> đang bị vượt quá hạn mức, hệ thống đã tự động ngừng phân bổ nạp tiền vào SIM này cho đến khi Reset lại Amount của Slot.<br/><br/><b>Các bước Reset Amount:</b><br/> Bước 1: Đăng nhập vào hệ thống CMS Chọn menu <b>USSD --> Quản lý Device & Sim </b> sau đó Click <b>Reset Amount</b>.<br/>Bước 2: Thay đổi trạng thái của Slot về <b>Actived</b>", sim, deviceName, slot);
            //    send = EmailService.SendMail(subject, body, ListMailNotify);
            //    NLogLogger.Info(new string[] { "SendMail Over Quota", send.ToString() });
            //    if (send) break;
            //}

        }

    }

}
