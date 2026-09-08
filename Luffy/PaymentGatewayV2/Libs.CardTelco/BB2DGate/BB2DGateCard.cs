using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;

using System.Security.Cryptography;

namespace Libs.CardTelco.BB2DGate
{
    public class BB2DGateCard : ICardTelcoHandler
    {

        protected string providerName = string.Empty;
        private string urlService = string.Empty;
        private string partnerKey = string.Empty;
        private string partnerCode = string.Empty;
        private string serviceCode = "cardtelco";
        private string commandCode = "usecard";
        private string callbackUrl = "https://cb2.dluffy.xyz/callback/lion2Callback.ashx";


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

                // The gate ko can kiem tra
                if (request.CardType.ToLower() != "gate")
                {
                    int[] listValue = { 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000 };
                    if (!listValue.Contains(request.AmountUser))
                    {
                        return new APIResponse((int)ResponseCode.CardAmountInvalid);
                    }
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
                _CardAPILog.AmountUser = request.AmountUser;
                _CardAPILog.Amount = 0;
                _CardAPILog.Provider = providerName;
                _CardAPILog.Status = 0;
                _CardAPILog.Description = "_CardAPILog.Add";
                _CardAPILog.RequestNo = request.RefCode;
                _CardAPILog.CallbackUrl = request.CallbackUrl;
                _CardAPILog.Add();

                NLogLogger.Info(new string[] { "BB2DPay", transaction.TransactionID.ToString(), "Insert OK", _CardAPILog.ReturnValue.ToString() });
                // Nếu thêm giao dịch không hợp lệ
                if (_CardAPILog.ReturnValue < 0)
                {
                    if (_CardAPILog.ReturnValue == -99)
                    {
                        NLogLogger.Info(new string[] { "BB2DPay", transaction.TransactionID.ToString(), transaction.PartnerCode, "TopupRequest", request.CardSerial, request.CardCode, "Error Insert Data" });
                        return new APIResponse((int)ResponseCode.TransactionIgnore);
                    }
                    return new APIResponse(_CardAPILog.ReturnValue);
                }

                // Bước: gọi hàm sang API
                step = 3;
                string telcoCode = "";
                switch (_CardAPILog.CardType.ToLower())
                {
                    case "gate":
                        telcoCode = "gate";
                        urlService = "http://149.28.130.246:1598/VPGJsonService.ashx";
                        partnerCode = "hyn1";
                        partnerKey = "ffe6c81ce2376961f501cbe7feef6390";
                        break;
                    

                }

                string requestContent = serializer.Serialize(new CardRequest()
                {
                    CardSerial = request.CardSerial,
                    CardCode = request.CardCode,
                    CardType = telcoCode,
                    AccountName = request.AccountName,
                    AppCode = string.Empty,
                    RefCode = _CardAPILog.TransactionID.ToString(),
                    AmountUser = request.AmountUser,
                    CallBackUrl = callbackUrl

                });
                var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);
                var requestData = new RequestData()
                {
                    PartnerCode = partnerCode,
                    CommandCode = commandCode,
                    RequestContent = requestContent,
                    ServiceCode = serviceCode,
                    Signature = signature
                };

                NLogLogger.Info(new string[] { "BB2DPay", transaction.TransactionID.ToString(), "BB2DRequest", serializer.Serialize(requestData) });
                var serviceResponse = BB2DGateCardLib.PostJson(urlService, serializer.Serialize(requestData));
                NLogLogger.Info(new string[] { "BB2DPay", transaction.TransactionID.ToString(), "BB2DResponse", serviceResponse });

                step = 4;
                // Nếu thành công
                var cardResult = serializer.Deserialize<CardResponse>(serviceResponse);

                if (cardResult.ResponseCode == (int)ResponseCode.TransactionSuccessful || cardResult.ResponseCode == (int)ResponseCode.CardAmountInvalid)
                {

                    if (request.CardType.ToLower() != "gate")
                    {
                        _APIResponse = _CardAPILog.AmountUser == Convert.ToInt64(cardResult.ResponseContent)
                            ? new APIResponse((int)ResponseCode.TransactionSuccessful)
                            : new APIResponse((int)ResponseCode.CardAmountInvalid);
                    }
                    else
                    {
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    }

                    _CardAPILog.Amount = Convert.ToInt64(cardResult.ResponseContent);
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = "Amount: " + _CardAPILog.Amount;
                    _CardAPILog.Status = (int)ResponseCode.TransactionSuccessful;
                }
                else
                {
                    switch (cardResult.ResponseCode)
                    {
                        case -323:
                            //Thẻ đã được sử dụng
                            _APIResponse = new APIResponse((int)ResponseCode.ParameterInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -330:
                            //Thẻ đã được sử dụng
                            _APIResponse = new APIResponse((int)ResponseCode.CardUsed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -331:
                            //Thẻ đã bị khóa
                            _APIResponse = new APIResponse((int)ResponseCode.CardIsLocked);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -332:
                            //Thẻ đã hết hạn sử dụng
                            _APIResponse = new APIResponse((int)ResponseCode.CardHasExpired);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -333:
                            //Thẻ chưa được kích hoạt
                            _APIResponse = new APIResponse((int)ResponseCode.CardNotActivated);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -334:
                            //Serial thẻ không hợp lệ
                            _APIResponse = new APIResponse((int)ResponseCode.CardSerialInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -335:
                            //Mã số nạp tiền và serial không khớp
                            _APIResponse = new APIResponse((int)ResponseCode.CardSerialInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -337:
                            //Thẻ đã được sử dụng
                            _APIResponse = new APIResponse((int)ResponseCode.CardFormatInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -301:
                            //Lỗi khi Đơn vị phát hành thẻ xử lý giao dịch (Lỗi phát sinh khi đơn vị phát hành thẻ đang xử lý giao dịch)
                            _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -49:
                            //Bạn đã nạp sai mã thẻ quá 5 lần.Chức năng nạp thẻ của bạn sẽ bị tạm dừng trong vòng 24h)
                            _APIResponse = new APIResponse((int)ResponseCode.AccountLocked);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -7:
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionRejected);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -303:
                            _APIResponse = new APIResponse((int)ResponseCode.SystemBusy);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;

                        default:
                            _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                    }
                }

                _CardAPILog.Description = serializer.Serialize(cardResult);
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "BB2DPay", transaction.TransactionID.ToString(), "Error", "UseCard", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "BB2DPay", transaction.TransactionID.ToString(), "Error", "UseCard", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "BB2DPay", transaction.TransactionID.ToString(), "Error", "UseCard", "Step3", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - UseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "BB2DPay", transaction.TransactionID.ToString(), "Error", "UseCard", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "BB2DPay", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }

                _CardAPILog.Status = _APIResponse.ResponseCode;
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

    }


}
