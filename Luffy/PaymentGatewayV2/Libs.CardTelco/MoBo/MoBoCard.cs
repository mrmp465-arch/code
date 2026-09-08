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
using Libs.CardTelco.MoBo;


namespace Libs.CardTelco.MoBo
{
    public class MoBoCard : ICardTelcoHandler
    {

        protected string providerName = string.Empty;

        // Production MoBo
        //protected string ServiceUrl = "https://btcvn.me/v3.0/recharge";
        //protected string client_id = "3ybcucmcna9tdct5vwgycuqg3ujdddkm";
        //protected string key = "P.Huy@3ybcucmcna9t";

        protected string ServiceUrl = "http://localhost:1599/TopupCard.asmx";
        //protected string client_id = "13689e9a427dfb59166f5fab96edd7fe";
        protected string partnerCode = "88a8faa6796453737a82b046267d0a48";

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
                        NLogLogger.Info(new string[] { "MoBo", transaction.TransactionID.ToString(), transaction.PartnerCode, "TopupRequest", request.CardSerial, request.CardCode, "Error Insert Data" });
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
                        telcoCode = "GATE";
                        break;
                    //case "vnp":
                    //    telcoCode = "VINAPHONE";
                    //    break;
                    //case "viettel":
                    //    telcoCode = "VIETTEL";
                    //    break;
                }

                CardRequest cardRequest = new CardRequest();
                
                cardRequest.serial = request.CardSerial;
                cardRequest.pinCode = request.CardCode;
                cardRequest.clientDateTime = string.Empty;
                cardRequest.partnerTransId = transaction.TransactionID.ToString();
                NLogLogger.Info(new string[] { "MoBo", transaction.TransactionID.ToString(), "MoBoRequest", serializer.Serialize(cardRequest) });
                TopupMobile _VPGService = new TopupMobile(ServiceUrl);
                _VPGService.Timeout = 120000;
                string serviceResponse = string.Empty;
                serviceResponse = _VPGService.RequestTopup(cardRequest.partnerTransId.ToString(), cardRequest.serial, cardRequest.pinCode);
                //CardResult cardResult = ProcessCard(cardRequest);
                CardResult cardResult = serializer.Deserialize<CardResult>(serviceResponse);
                step = 4;
                // Nếu thành công
                if (cardResult.resCode =="00")
                {
                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    _CardAPILog.Amount = Convert.ToInt64(cardResult.cardValue);
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = "Amount: " + _CardAPILog.Amount.ToString();
                    _CardAPILog.Status = 1;
                }
                else
                {
                    _APIResponse = new APIResponse(MoboCardLib.ConvertResponCode(cardResult.resCode));
                    _CardAPILog.Status = _APIResponse.ResponseCode;
                    cardResult.sign = "";
                    cardResult.pinCode = "";
                    cardResult.serial = "";
                    _CardAPILog.Description = serializer.Serialize(cardResult);
                }
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "MoBo", transaction.TransactionID.ToString(), "Error", "UseCard", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "MoBo", transaction.TransactionID.ToString(), "Error", "UseCard", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "MoBo", transaction.TransactionID.ToString(), "Error", "UseCard", "Step3", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - UseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "MoBo", transaction.TransactionID.ToString(), "Error", "UseCard", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "MoBo", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
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

        //private CardResult ProcessCard(CardRequest cardRequest)
        //{
        //    //string parameters = "clientid={0}&serial={1}&pin={2}&callback={3}";
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    string responseData = MoboCardLib.PostJson(ServiceUrl, serializer.Serialize(cardRequest));
        //    NLogLogger.Info(new string[] { "MoBo", cardRequest.trans_id.ToString(), "MoBoResponseRaw", responseData });
        //    return serializer.Deserialize<CardResult>(responseData);
        //}


        public APIResponse ReCheck(string transactionId)
        {
            throw new NotImplementedException();
        }

    }


}
