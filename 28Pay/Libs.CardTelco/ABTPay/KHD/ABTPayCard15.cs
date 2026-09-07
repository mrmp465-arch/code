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
using Libs.CardTelco.ABTPay;

namespace Libs.CardTelco.ABTPayCard.KHD
{
    public class ABTPayCard15 : ICardTelcoHandler
    {

        protected string providerName = "abtpay15";
        // Sandbox
        //protected string ServiceUrl = "https://newcharging.abtpay.vn/apis/card/charge";
        //protected string SecretKey = "hO8inv3D1lUtzuXv";
        //protected int MerchantId = 1000188;
        //protected string Acc = "xbom01";
        //protected string Password = "X130@2017";

        // Production XBOM
        protected string ServiceUrl = "https://newcharging.abtpay.vn/apis/card/charge";
        protected string ServiceRecheckUrl = "https://newcharging.abtpay.vn/apis/card/recheck";
        protected string SecretKey = "hO8inv3D1lUtzuXv";
        protected int MerchantId = 1000188;
        protected string Acc = "xbom01";
        protected string Password = "X130@2017";

        // Production A68
        //protected string ServiceUrl = "https://newcharging.abtpay.vn/apis/card/charge";
        //protected string SecretKey = "TOE785WOYCV2J51588UUJQ2DYHS042";
        //protected int MerchantId = 10000350;
        //protected string Acc = "chinhpoor";
        //protected string Password = "123456";


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
                        telcoCode = "VMS";
                        break;
                    case "vnp":
                        telcoCode = "VNP";
                        break;
                    case "viettel":
                        telcoCode = "VTE";
                        break;
                }

                ArrayList args = new ArrayList();
                args.Add(telcoCode);
                args.Add(MerchantId.ToString());
                args.Add(transaction.TransactionID.ToString());
                args.Add(request.CardCode);
                args.Add(request.CardSerial);
                string checksum = ABTCardLib.createChecksum(args, SecretKey);

                CardRequest cardRequest = new CardRequest();
                cardRequest.merchant_id = MerchantId;
                cardRequest.merchant_txn_id = transaction.TransactionID.ToString();
                cardRequest.pin = request.CardCode;
                cardRequest.seri = request.CardSerial;
                cardRequest.card_type = telcoCode;
                cardRequest.checksum = checksum;

                NLogLogger.Info(new string[] { "ABTPayCard", transaction.TransactionID.ToString(), "ABTPayCardRequest", serializer.Serialize(cardRequest) });
                CardResult cardResult = ProcessCard(cardRequest);
                NLogLogger.Info(new string[] { "ABTPayCard", transaction.TransactionID.ToString(), "ABTPayCardResponse", serializer.Serialize(cardResult) });

                step = 4;
                // Nếu thành công
                if (cardResult.code == "1")
                {
                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    _CardAPILog.Amount = Convert.ToInt64(cardResult.card_amount);
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = "Amount: " + _CardAPILog.Amount.ToString();
                    _CardAPILog.Status = 1;
                }
                else
                {
                    _APIResponse = new APIResponse(ABTCardLib.ConvertResponCode(cardResult.code));
                    _CardAPILog.Status = _APIResponse.ResponseCode;
                    _CardAPILog.Description = serializer.Serialize(cardResult);
                }
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "ABTPayCard", transaction.TransactionID.ToString(), "Error", "UseCard", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "ABTPayCard", transaction.TransactionID.ToString(), "Error", "UseCard", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "ABTPayCard", transaction.TransactionID.ToString(), "Error", "UseCard", "Step3", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - UseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "ABTPayCard", transaction.TransactionID.ToString(), "Error", "UseCard", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "ABTPayCard", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
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

        private CardResult ProcessCard(CardRequest cardRequest)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string responseData = ABTCardLib.PostData(ServiceUrl, serializer.Serialize(cardRequest));
            return serializer.Deserialize<CardResult>(responseData);
        }
       

        public APIResponse ReCheck(string transactionId)
        {
            APIResponse _APIResponse = new APIResponse();
            CardAPILog _CardAPILog = new CardAPILog();
            var merchant_txn_id = transactionId;
            var merchant_id = MerchantId;
            ArrayList args = new ArrayList();
            args.Add(merchant_txn_id);
            args.Add(merchant_id.ToString());
            string checksum = ABTCardLib.createChecksum(args, SecretKey);

            var recheckRequest = new RecheckRequest()
            {
                merchant_txn_id = merchant_txn_id,
                merchant_id = merchant_id,
                checksum = checksum
            };

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string cardResult = ABTCardLib.PostData(ServiceRecheckUrl, serializer.Serialize(recheckRequest));
            var responseData = serializer.Deserialize<CardResult>(cardResult);

            if (responseData.code == "1")
            {
                _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                _APIResponse.Description = serializer.Serialize(cardResult);
            }
            else
            {
                _APIResponse = new APIResponse(ABTCardLib.ConvertResponCode(responseData.code));
                _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                _APIResponse.Description = serializer.Serialize(cardResult);
            }
            return _APIResponse;

        }

    }


}
