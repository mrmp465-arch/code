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

namespace Libs.CardTelco.VinaPay
{
    public class VinaPayCard : ICardTelcoHandler
    {

        protected string providerName = string.Empty;
        string serviceUrl = "http://127.0.0.1:1596/";

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

                int[] listValue = { 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000 };
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
                        NLogLogger.Info(new string[] { "Push247Card", transaction.TransactionID.ToString(), transaction.PartnerCode, "TopupRequest", request.CardSerial, request.CardCode, "Error Insert Data" });
                        return new APIResponse((int)ResponseCode.TransactionIgnore);
                    }
                    return new APIResponse(_CardAPILog.ReturnValue);
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
                    case "garena":
                        telcoCode = "garena";
                        break;
                    case "vcoin":
                        telcoCode = "vcoin";
                        break;
                    case "zing":
                        telcoCode = "zing";
                        break;

                }

                TopupRequest topupRequest = new TopupRequest();
                topupRequest.sim = "sim";
                topupRequest.simTarget = string.Empty;
                topupRequest.cardSerial = request.CardSerial;
                topupRequest.cardCode = request.CardCode;
                topupRequest.telco = telcoCode;
                topupRequest.amount = request.AmountUser;
                topupRequest.transactionId = transaction.TransactionID;
                topupRequest.partnerCode = transaction.PartnerCode;
                topupRequest.providerCode = providerName;

                NLogLogger.Info(new string[] { "Push247Card", transaction.TransactionID.ToString(), "TopupRequest", serializer.Serialize(topupRequest) });
                TopupResponse cardResult = ProcessCard(topupRequest);
                NLogLogger.Info(new string[] { "Push247Card", transaction.TransactionID.ToString(), "TopupResponse", serializer.Serialize(cardResult) });

                step = 4;
                // Nếu thành công
                if (cardResult.code == 1)
                {
                    if (_CardAPILog.AmountUser == Convert.ToInt64(cardResult.amount))
                    {
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                        _CardAPILog.Status = (int)ResponseCode.TransactionSuccessful;
                    }
                    else
                    {
                        _APIResponse = new APIResponse((int)ResponseCode.CardAmountInvalid);
                        //_CardAPILog.Status = cardResult.amount > 50000 ? (int) ResponseCode.TransactionSuccessful : (int) ResponseCode.CardAmountInvalid;
                    }
                    _CardAPILog.Amount = Convert.ToInt64(cardResult.amount);
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = "Amount: " + _CardAPILog.Amount;
                    _CardAPILog.Status = (int) ResponseCode.TransactionSuccessful;


                }
                else
                {
                    switch (cardResult.code)
                    {
                        case 0:
                            _APIResponse = new APIResponse(0);
                            _CardAPILog.Status = 0;
                            break;
                        case (int)ResponseCode.CardUsed:
                            _APIResponse = new APIResponse((int)ResponseCode.CardUsed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;

                        case (int)ResponseCode.SystemBusy:
                            _APIResponse = new APIResponse((int)ResponseCode.SystemBusy);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;

                        case (int)ResponseCode.TransactionRejected:
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionRejected);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;

                        case (int)ResponseCode.AccessDenied:
                            _APIResponse = new APIResponse((int)ResponseCode.AccessDenied);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case (int)ResponseCode.TransactionTimeout:
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionTimeout);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            _APIResponse.ResponseContent = "p17";
                            break;
                        case (int)ResponseCode.CardCodeInvalid:
                            _APIResponse = new APIResponse((int)ResponseCode.CardCodeInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case (int)ResponseCode.CardSerialInvalid:
                            _APIResponse = new APIResponse((int)ResponseCode.CardSerialInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case (int)ResponseCode.CardAmountInvalid:
                            _APIResponse = new APIResponse((int)ResponseCode.CardAmountInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case (int)ResponseCode.ParameterInvalid:
                            _APIResponse = new APIResponse((int)ResponseCode.ParameterInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case (int)ResponseCode.TransactionSuspicious:
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
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
                        NLogLogger.Info(new string[] { "Push247Card", transaction.TransactionID.ToString(), "Error", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "Push247Card", transaction.TransactionID.ToString(), "Error", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "Push247Card", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - UseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "Push247Card", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "Push247Card", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }
            }

            if (_CardAPILog.TransactionID > 0 && _CardAPILog.Status!=0)
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
            //NLogLogger.Info(new string[] { "Push247Card", "TopupRequest", serializer.Serialize(topupRequest) });
            TopupMobile _VPGService = new TopupMobile(serviceUrl + "TopupCard.asmx");
            _VPGService.Timeout = 120000;
            string serviceResponse = string.Empty;
            //serviceResponse = _VPGService.RequestTopup(topupRequest.transactionId.ToString(), topupRequest.telco, topupRequest.partnerCode, topupRequest.providerCode, topupRequest.cardSerial, topupRequest.cardCode, topupRequest.simTarget, topupRequest.amount);
            //NLogLogger.Info(new string[] { "Push247Card", "TopupResponse", serviceResponse });
            //return serializer.Deserialize<TopupResponse>(serviceResponse);

            try
            {
                serviceResponse = _VPGService.RequestTopup(topupRequest.transactionId.ToString(), topupRequest.telco, topupRequest.partnerCode, topupRequest.providerCode, topupRequest.cardSerial, topupRequest.cardCode, topupRequest.simTarget, topupRequest.amount);
            }

            catch (TimeoutException e)
            {
                NLogLogger.Info(new string[] { "APIPush247", "Request", "TimeoutException", topupRequest.transactionId.ToString(), e.Message });
                Thread.ResetAbort();
                return new TopupResponse()
                {
                    code = (int)ResponseCode.TransactionTimeout,
                    message = "The Timeout",
                    amount = 0

                };
            }

            catch (ThreadAbortException e)
            {
                NLogLogger.Info(new string[] { "APIPush247", "Request", "ThreadAbortException", topupRequest.transactionId.ToString(), e.Message });
                Thread.ResetAbort();
                return new TopupResponse()
                {
                    code = (int)ResponseCode.TransactionTimeout,
                    message = "The Thread Abort",
                    amount = 0

                };
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "APIPush247", "Request", "Exception", topupRequest.transactionId.ToString(), e.Message });
                return new TopupResponse()
                {
                    code = (int)ResponseCode.TransactionTimeout,
                    message = "The Exception",
                    amount = 0

                };
            }

            try
            {
                NLogLogger.Info(new string[] { "APIPush247", "Request", topupRequest.transactionId.ToString(), serviceResponse });
                return serializer.Deserialize<TopupResponse>(serviceResponse);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "APIPush247", "Request", topupRequest.transactionId.ToString(), e.Message });
                return new TopupResponse()
                {
                    code = (int)ResponseCode.TransactionTimeout,
                    message = "The Exception Deserialize",
                    amount = 0

                };
            }

        }



    }

}
