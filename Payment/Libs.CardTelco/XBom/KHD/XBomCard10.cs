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
using Libs.CardTelco.XBom;
using Libs.CardTelco.XBomCardLib;

namespace Libs.CardTelco.XBomCard.KHD
{
    public class XBomCard10 : ICardTelcoHandler
    {
        
        protected string providerName = "xbom10";

        // Production : Không hóa đơn
        string serviceUrl = "http://27.118.16.46:1581/VPGService.asmx";
        string partnerKey = "339d92699e013e9240f9ea7ff592f308";
        string partnerCode = "a6810";
        string serviceCode = "cardtelco";
        string commandCode = "usecard";

        public APIResponse UseCard(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            APIResponse _APIResponse = new APIResponse();
            CardAPILog _CardAPILog = new CardAPILog();

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
                _CardAPILog.Provider = providerName;
                _CardAPILog.Status = 0;
                _CardAPILog.Description = "_CardAPILog.Add";
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
                        telcoCode = "viettel";
                        break;
                    //case "vcoin":
                    //    telcoCode = "vcoin";
                    //    break;
                    //case "fpt":
                    //    telcoCode = "fpt";
                    //    break;
                    //case "bit":
                    //    telcoCode = "bit";
                    //    break;
                    //case "gmobile":
                    //    telcoCode = "gmobile";
                    //    break;

                }
                CardRequest cardRequest = new CardRequest();
                cardRequest.CardType = telcoCode;
                cardRequest.CardSerial = request.CardSerial;
                cardRequest.CardCode = request.CardCode;
                cardRequest.AccountName = request.AccountName;
                cardRequest.AppCode = request.AppCode;
                cardRequest.RefCode = transaction.TransactionID.ToString();

                var requestContent = serializer.Serialize(cardRequest);
                var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);

                var usecardRequest = new CardDataRequest()
                {
                    commandCode = commandCode,
                    partnerCode = partnerCode,
                    requestContent = requestContent,
                    serviceCode = serviceCode,
                    signature = signature
                };

                NLogLogger.Info(new string[] { "XBomCard", transaction.TransactionID.ToString(), "XBomCardRequest", serializer.Serialize(cardRequest) });
                CardResponse cardResult = ProcessCard(usecardRequest);
                NLogLogger.Info(new string[] { "XBomCard", transaction.TransactionID.ToString(), "XBomCardResponse", serializer.Serialize(cardResult) });

                step = 4;
                // Nếu thành công
                if (cardResult.ResponseCode == 1)
                {

                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    _CardAPILog.Amount = Convert.ToInt64(cardResult.ResponseContent);
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = "Amount: " + _CardAPILog.Amount.ToString();
                    _CardAPILog.Status = 1;
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

                        default:
                            _APIResponse = new APIResponse((int)ResponseCode.SystemError);
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
                        NLogLogger.Info(new string[] { "XBomCard", transaction.TransactionID.ToString(), "Error", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "XBomCard", transaction.TransactionID.ToString(), "Error", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "XBomCard", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - UseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "XBomCard", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "XBomCard", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
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

        private CardResponse ProcessCard(CardDataRequest usecardRequest)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            NLogLogger.Info(new string[] { "XBomCard", "XBomCardRequest", serializer.Serialize(usecardRequest) });
            VPGService _VPGService = new VPGService(serviceUrl);
            string serviceResponse = string.Empty;
            serviceResponse = _VPGService.Request(partnerCode, serviceCode, commandCode, usecardRequest.requestContent, usecardRequest.signature);
            NLogLogger.Info(new string[] { "XBomCard", "XBomCardResponse", serviceResponse });
            return serializer.Deserialize<CardResponse>(serviceResponse);
        }

    }

}
