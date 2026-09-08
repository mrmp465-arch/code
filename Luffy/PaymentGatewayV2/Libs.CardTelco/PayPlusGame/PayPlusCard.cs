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
using System.Threading;

namespace Libs.CardTelco.PayPlusGame
{
    public class PayPlusCard : ICardTelcoHandler
    {

        protected string providerName = string.Empty;
        string serviceUrl = "http://127.0.0.1:1588/";

        public APIResponse UseCard(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            APIResponse _APIResponse = new APIResponse();
            CardAPILog _CardAPILog = new CardAPILog();

            providerName = transaction.ProviderCode;
            int step = 0;
            try
            {
                // Bước: Phân tích yêu cầu thành đối tượng
                step = 1;
                UseCardRequest request = new UseCardRequest();
                request = serializer.Deserialize<UseCardRequest>(transaction.RequestContent);
                int[] listValue = { 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 5000000, 10000000 };
                //int[] listValue = { 50000, 100000, 200000, 300000, 500000, 1000000 };
                if (!listValue.Contains(request.AmountUser))
                {
                    return new APIResponse((int)ResponseCode.CardAmountInvalid);
                }

                if (!Utils.GlobalHelper.CheckCardCode(request.CardType, request.CardCode))
                {
                    return new APIResponse((int)ResponseCode.CardCodeInvalid);
                }

                if (!Utils.GlobalHelper.CheckCardSerial(request.CardType, request.CardSerial))

                {
                    return new APIResponse((int)ResponseCode.CardSerialInvalid);
                }

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
                _CardAPILog.CallbackUrl = request.CallbackUrl;
                _CardAPILog.Add();

                // Nếu thêm giao dịch không hợp lệ
                if (_CardAPILog.ReturnValue < 0)
                {
                    if (_CardAPILog.ReturnValue == -99)
                    {
                        NLogLogger.Info(new string[] { "PayPlusGameTopup", transaction.TransactionID.ToString(), transaction.PartnerCode, "TopupRequest", request.CardSerial, request.CardCode, "Error Insert Data" });
                        return new APIResponse((int)ResponseCode.TransactionIgnore);
                    }
                    return new APIResponse(_CardAPILog.ReturnValue);
                }

                // Bước: gọi hàm sang API
                step = 3;
                string telcoCode = "";
                switch (_CardAPILog.CardType.ToLower())
                {
                    case "bit":
                        telcoCode = "bit";
                        break;
                    case "gate":
                        telcoCode = "gate";
                        break;
                    case "vcoin":
                        telcoCode = "vcoin";
                        break;
                    case "gosu":
                        telcoCode = "gosu";
                        break;
                    case "zing":
                        telcoCode = "zing";
                        break;
                    case "garena":
                        telcoCode = "garena";
                        break;
                    default:
                        return new APIResponse((int)ResponseCode.CardProviderInvalid);
                }

                TopupRequest topupRequest = new TopupRequest();
                topupRequest.simTarget = "simtarget";
                topupRequest.cardSerial = request.CardSerial;
                topupRequest.cardCode = request.CardCode;
                topupRequest.telco = telcoCode;
                topupRequest.amount = request.AmountUser;
                topupRequest.transactionId = transaction.TransactionID;
                topupRequest.partnerCode = transaction.PartnerCode;
                topupRequest.providerCode = providerName;

                NLogLogger.Info(new string[] { "PayPlusGameTopup", transaction.TransactionID.ToString(), "PayPlusGameTopupRequest", serializer.Serialize(topupRequest) });
                var tryAgain = 0;
                TopupResponse cardResult = ProcessCard(topupRequest);
                while (cardResult.code == (int)ResponseCode.ServiceIsLocked && tryAgain < 1)
                {
                    cardResult = ProcessCard(topupRequest);
                    tryAgain++;
                    Thread.Sleep(1000);
                }
                NLogLogger.Info(new string[] { "PayPlusGameTopup", transaction.TransactionID.ToString(), "PayPlusGameTopupResponse", serializer.Serialize(cardResult) });

                step = 4;
                // Nếu thành công
                if (cardResult.code == 1)
                {
                    if (_CardAPILog.AmountUser == Convert.ToInt64(cardResult.amount))
                    {
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    }
                    else
                    {
                        _APIResponse = new APIResponse((int)ResponseCode.CardAmountInvalid);
                    }
                    _CardAPILog.Amount = Convert.ToInt64(cardResult.amount);
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = "Amount: " + _CardAPILog.Amount;
                    _CardAPILog.Status = 1;
                }
                else
                {
                    switch (cardResult.code)
                    {
                        case -1:
                            //Giao dịch failed
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -2:
                            //Nghi vấn
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -7:
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionRejected);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -300:
                            _APIResponse = new APIResponse((int)ResponseCode.SystemMaintain);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -320:
                            //Mã nạp thẻ không đúng
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionNotExists);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -323:
                            //Mã nạp thẻ không đúng
                            _APIResponse = new APIResponse((int)ResponseCode.ParameterInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -330:
                            //Mã nạp thẻ không đúng
                            _APIResponse = new APIResponse((int)ResponseCode.CardUsed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -335:
                            //Mã nạp thẻ không đúng
                            _APIResponse = new APIResponse((int)ResponseCode.CardCodeInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -337:
                            //Mã nạp thẻ không đúng
                            _APIResponse = new APIResponse((int)ResponseCode.CardFormatInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -328:
                            //Mã nạp thẻ không đúng
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionIgnore);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -326:
                            //Mã nạp thẻ không đúng
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionTimeout);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -317:
                            //Không gửi được Request xuống Client My Viettel
                            _APIResponse = new APIResponse((int)ResponseCode.ServiceNotExists);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -303:
                            _APIResponse = new APIResponse((int)ResponseCode.SystemBusy);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -334:
                            _APIResponse = new APIResponse((int)ResponseCode.CardSerialInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -332:
                            _APIResponse = new APIResponse((int)ResponseCode.CardHasExpired);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        default:
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                    }
                    _CardAPILog.Description = serializer.Serialize(cardResult);
                }
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "PayPlusGameTopup", transaction.TransactionID.ToString(), "Error", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "PayPlusGameTopup", transaction.TransactionID.ToString(), "Error", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "PayPlusGameTopup", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - UseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "PayPlusGameTopup", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "PayPlusGameTopup", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
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
            var log3Rd = new TopupMobile3rdLog().GetByTransactionId(Convert.ToInt64(transactionId));
            return null;
        }

        private TopupResponse ProcessCard(TopupRequest topupRequest)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            NLogLogger.Info(new string[] { "PayPlusGameTopup", "PayPlusGameTopupRequest", serializer.Serialize(topupRequest) });
            //TopupAppVTTService _VPGService = new TopupAppVTTService(serviceUrl + "TopupAppV2VTT.asmx");
            TopupAppVTTService _VPGService = new TopupAppVTTService(serviceUrl + "APIGame.asmx");
            _VPGService.Timeout = 120000;
            string serviceResponse = string.Empty;
            try
            {
                serviceResponse = _VPGService.RequestTopup(topupRequest.transactionId.ToString(), topupRequest.telco, topupRequest.partnerCode, topupRequest.providerCode, topupRequest.cardSerial, topupRequest.cardCode, topupRequest.simTarget, topupRequest.amount);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "PayPlusGameTopup", "PayPlusGameTopupResponse", e.Message });
                return new TopupResponse()
                {
                    code = (int)ResponseCode.TransactionTimeout,
                    message = "The operation has timed out",
                    amount = 0

                };
            }

            NLogLogger.Info(new string[] { "PayPlusGameTopup", "PayPlusGameTopupResponse", serviceResponse });
            return serializer.Deserialize<TopupResponse>(serviceResponse);
        }


    }

}
