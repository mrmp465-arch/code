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

namespace Libs.CardTelco.Global
{
    public class GlobalCard : ICardTelcoHandler
    {

        protected string providerName = "global";
        protected string cpcode = "21";
        protected string privateKey = "9caa01e229b0e56c190fd226fbb188c3";
        string serviceUrl = "http://api.napthedidong.com/add-card.php";

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
                int[] listValue = { 50000, 100000, 200000, 300000, 500000 };
                if (!listValue.Contains(request.AmountUser))
                {
                    return new APIResponse((int)ResponseCode.CardAmountInvalid);
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
                _CardAPILog.Provider = providerName;
                _CardAPILog.Status = 0;
                _CardAPILog.Description = "_CardAPILog.Add";
                _CardAPILog.RequestNo = request.RefCode;
                _CardAPILog.CallbackUrl = request.CallbackUrl;
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
                cardRequest.req = transaction.TransactionID.ToString();
                cardRequest.telco = telcoCode;
                cardRequest.cardcode = request.CardCode;
                cardRequest.serial = request.CardSerial;
                cardRequest.amount = request.AmountUser.ToString();
                cardRequest.cpcode = cpcode;

                var cardResult = ProcessCard(cardRequest);

                NLogLogger.Info(new string[] { "GlobleCard", transaction.TransactionID.ToString(), "CardResult", serializer.Serialize(cardResult) });

                step = 4;
                // Nếu thành công
                if (cardResult.code == 0)
                {
                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                }
                else
                {
                    switch (cardResult.code)
                    {
                        case 1:
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                            break;
                        case 2:
                            _APIResponse = new APIResponse((int)ResponseCode.CardAmountInvalid);
                            break;
                        case 3:
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionDuplicate);
                            break;
                        default:
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
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
                        NLogLogger.Info(new string[] { "GlobleCard", transaction.TransactionID.ToString(), "Error", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "GlobleCard", transaction.TransactionID.ToString(), "Error", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "GlobleCard", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - UseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "GlobleCard", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "GlobleCard", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
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

        private CardRequestResponse ProcessCard(CardRequest cardRequest)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var dictionary = new Dictionary<string, string>
                {
                    {"req", cardRequest.req},
                    {"telco", cardRequest.telco},
                    {"cardcode", cardRequest.cardcode},
                    {"serial", cardRequest.serial},
                    {"amount", cardRequest.amount},
                    {"cpcode", cardRequest.cpcode},
                };

                //string[] arr = new string[]
                //{
                //    cardRequest.req,
                //    cardRequest.telco,
                //    cardRequest.cardcode,
                //    cardRequest.serial,
                //    cardRequest.amount,
                //    cardRequest.cpcode
                //};

                var dataPost = Encrypts.Base64Encode(serializer.Serialize(dictionary));
                NLogLogger.Info(new string[] { "GlobleCard", "CardRequest", serializer.Serialize(dictionary) });
                NLogLogger.Info(new string[] { "GlobleCard", "CardRequest", dataPost });
                var result = GlobalCardLib.HttpPostRawData(serviceUrl, dataPost);
                NLogLogger.Info(new string[] { "GlobleCard", "CardResponse", result });
                if (!string.IsNullOrEmpty(result))
                {
                    var data = result.Split('|');
                    return new CardRequestResponse()
                    {
                        code = Convert.ToInt32(data[0]),
                        message = data[1]
                    };
                }
                return new CardRequestResponse()
                {
                    code = 1,
                    message = "Unsuccess"
                };
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GlobleCard", "CardRequest", "Error", ex.Message });
                return new CardRequestResponse()
                {
                    code = 1,
                    message = "Unsuccess"
                };

            }

        }


    }

}
