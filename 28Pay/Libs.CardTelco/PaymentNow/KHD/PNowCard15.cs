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
using Libs.CardTelco.PaymentNow;

namespace Libs.CardTelco.PaymentNow.KHD
{
    public class PNowCard15 : ICardTelcoHandler
    {

        protected string providerName = "pnow15";

        // Production PaymentNow
        protected string ServiceUrl = "http://api.paymentnow.net:8989/card/recharge";
        protected int user_id = 8;
        protected int project_id = 11;
        protected string key = "qf8Qcxs5qUk6bkWMhkgbuZSgT37Xn6kwpN2V23vueqYzb8wJ2m";


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
                        telcoCode = "MOBIFONE";
                        break;
                    case "vnp":
                        telcoCode = "VINAPHONE";
                        break;
                    case "viettel":
                        telcoCode = "VIETTEL";
                        break;
                }

                CardRequest cardRequest = new CardRequest();
                cardRequest.user_id = this.user_id;
                cardRequest.project_id = this.project_id;
                cardRequest.pin_field = request.CardCode;
                cardRequest.seri_field = request.CardSerial;
                cardRequest.card_id = telcoCode;
                cardRequest.time = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                cardRequest.trans_id = transaction.TransactionID.ToString();
                cardRequest.sign = PNowCardLib.createChecksum(cardRequest, key);


                NLogLogger.Info(new string[] { "PaymentNow", transaction.TransactionID.ToString(), "PaymentNowRequest", serializer.Serialize(cardRequest) });

                CardResult cardResult = ProcessCard(cardRequest);


                step = 4;
                // Nếu thành công
                if (cardResult.status == 1)
                {
                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    _CardAPILog.Amount = Convert.ToInt64(cardResult.amount);
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = "Amount: " + _CardAPILog.Amount.ToString();
                    _CardAPILog.Status = 1;
                }
                else
                {
                    _APIResponse = new APIResponse(PNowCardLib.ConvertResponCode(cardResult.status));
                    _CardAPILog.Status = _APIResponse.ResponseCode;
                    _CardAPILog.Description = serializer.Serialize(cardResult);
                }
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "PaymentNow", transaction.TransactionID.ToString(), "Error", "UseCard", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "PaymentNow", transaction.TransactionID.ToString(), "Error", "UseCard", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "PaymentNow", transaction.TransactionID.ToString(), "Error", "UseCard", "Step3", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - UseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "PaymentNow", transaction.TransactionID.ToString(), "Error", "UseCard", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "PaymentNow", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
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
            string parameters = "project_id={0}&user_id={1}&trans_id={2}&card_id={3}&pin_field={4}&seri_field={5}&time={6}&sign={7}";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //string responseData = PNowCardLib.PostData(ServiceUrl, serializer.Serialize(cardRequest));
            string responseData = PNowCardLib.HttpPost(ServiceUrl, string.Format(parameters, cardRequest.project_id, cardRequest.user_id, cardRequest.trans_id, cardRequest.card_id, cardRequest.pin_field, cardRequest.seri_field, cardRequest.time, cardRequest.sign));
            NLogLogger.Info(new string[] { "PaymentNow", cardRequest.trans_id, "PaymentNowResponseRaw", responseData });
            return serializer.Deserialize<CardResult>(responseData);
        }


        public APIResponse ReCheck(string transactionId)
        {
            throw new NotImplementedException();
        }

    }


}
