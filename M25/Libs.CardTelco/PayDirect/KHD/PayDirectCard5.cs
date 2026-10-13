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
using Libs.CardTelco.PayDirect;

namespace Libs.CardTelco.PayDirectCard.KHD
{
    public class PayDirectCard5 : ICardTelcoHandler
    {
        
        protected string providerName = "paydirect5";

        // Sandbox
        //protected string ServiceUrl = "https://sandbox.paydirect.vn/voucher/rest/useCard";
        //protected string PartnerCode = "sandbox";
        //protected string Password = "123456";
        //protected string SecretKey = "sandbox_sk";
   
        // Production : Không hóa đơn
        protected string ServiceUrl = "http://45.122.220.249:8284/voucher/rest/useCard";
        protected string PartnerCode = "XBOM51";
        protected string Password = "xs#@!2bom";
        protected string SecretKey = "sx!@#asd";

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
                        telcoCode = "MOBI";
                        break;
                    case "vnp":
                        telcoCode = "VINA";
                        break;
                    case "viettel":
                        telcoCode = "VT";
                        break;
                    case "vcoin":
                        telcoCode = "VCOIN";
                        break;
                    case "gate":
                        telcoCode = "GATE";
                        break;
                    case "bit":
                        telcoCode = "BIT";
                        break;
                    case "gmobile":
                        telcoCode = "GM";
                        break;
                    case "zing":
                        telcoCode = "ZING";
                        break;

                }
                CardRequest cardRequest = new CardRequest();
                cardRequest.issuer = telcoCode;
                cardRequest.cardSerial = request.CardSerial;
                cardRequest.cardCode = request.CardCode;
                cardRequest.amount = string.Empty;
                cardRequest.transRef = transaction.TransactionID.ToString();
                cardRequest.partnerCode = PartnerCode;
                cardRequest.password = Password;
                cardRequest.accountId = request.AccountName;
                cardRequest.serviceCode = string.Empty;
                cardRequest.signature = Encrypts.MD5(string.Format("{0}{1}{2}{3}{4}{5}", cardRequest.issuer, cardRequest.cardCode, cardRequest.transRef, cardRequest.partnerCode, cardRequest.password, SecretKey));

                NLogLogger.Info(new string[] { "PayDirectCard", transaction.TransactionID.ToString(), "PayDirectCardRequest", serializer.Serialize(cardRequest) });
                CardResult cardResult = ProcessCard(cardRequest);
                NLogLogger.Info(new string[] { "PayDirectCard", transaction.TransactionID.ToString(), "PayDirectCardResponse", serializer.Serialize(cardResult) });

                step = 4;
                // Nếu thành công
                if (cardResult.status == "01")
                {

                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    _CardAPILog.Amount = Convert.ToInt64(cardResult.amount);
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = "Amount: " + _CardAPILog.Amount.ToString();
                    _CardAPILog.Status = 1;
                }
                else
                {
                    switch (cardResult.status)
                    {
                        case "00":
                            //Mã số nạp tiền không tồn tại hoặc đã được sử dụng
                            _APIResponse = new APIResponse((int)ResponseCode.CardHasExpired);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "03":
                            //Thẻ đã được sử dụng
                            _APIResponse = new APIResponse((int)ResponseCode.CardUsed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "04":
                            //Thẻ đã bị khóa
                            _APIResponse = new APIResponse((int)ResponseCode.CardIsLocked);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "05":
                            //Thẻ đã hết hạn sử dụng
                            _APIResponse = new APIResponse((int)ResponseCode.CardHasExpired);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "06":
                            //Thẻ chưa được kích hoạt
                            _APIResponse = new APIResponse((int)ResponseCode.CardNotActivated);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "07":
                            //Thực hiện sai quá số lần cho phép
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionLimit);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "08":
                            //Giao dịch nghi vấn (Timeout từ Đơn vị phát hành thẻ, chưa xử lý xong)
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionReview);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "09":
                            //Sai định dạng thông tin truyền vào
                            _APIResponse = new APIResponse((int)ResponseCode.ParameterInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "10":
                            //Partner không tồn tại
                            _APIResponse = new APIResponse((int)ResponseCode.ServiceNotExists);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "11":
                            //Partner bị khóa
                            _APIResponse = new APIResponse((int)ResponseCode.ServiceIsLocked);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "13":
                            //Hệ thống của Đơn vị phát hành thẻ đang bận
                            _APIResponse = new APIResponse((int)ResponseCode.PaymentConnectionFailed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "14":
                            //Sai password
                            _APIResponse = new APIResponse((int)ResponseCode.ParameterInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "15":
                            //Sai địa chỉ IP
                            _APIResponse = new APIResponse((int)ResponseCode.ParameterInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "16":
                            //Sai chữ ký
                            _APIResponse = new APIResponse((int)ResponseCode.SignatureInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "20":
                            //Sai độ dài mã số nạp tiền
                            _APIResponse = new APIResponse((int)ResponseCode.ParameterInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "21":
                            //Mã giao dịch không hợp lệ (> 0 và < 30 ký tự)
                            _APIResponse = new APIResponse((int)ResponseCode.ParameterInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "23":
                            //Serial thẻ không hợp lệ
                            _APIResponse = new APIResponse((int)ResponseCode.CardSerialInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "24":
                            //Mã số nạp tiền và serial không khớp
                            _APIResponse = new APIResponse((int)ResponseCode.CardSerialInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "25":
                            //Trùng mã giao dịch (transRef)
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionDuplicate);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "26":
                            //Mã giao dịch không tồn tại
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "28":
                            //Mã số nạp tiền không đúng định dạng (chỉ bao gồm ký tự số)
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "40":
                            //Lỗi kết nối tới Đơn vị phát hành (Voucher Services ko kết nối được đến hệ thống đơn vị phát hành thẻ, ví dụ: mất kết nối)
                            _APIResponse = new APIResponse((int)ResponseCode.PaymentConnectionFailed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "41":
                            //Lỗi khi Đơn vị phát hành thẻ xử lý giao dịch (Lỗi phát sinh khi đơn vị phát hành thẻ đang xử lý giao dịch)
                            _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "51":
                            //Đơn vị phát hành thẻ không tồn tại
                            _APIResponse = new APIResponse((int)ResponseCode.ServiceNotExists);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "52":
                            //Đơn vị phát hành thẻ không hỗ trợ nghiệp vụ này
                            _APIResponse = new APIResponse((int)ResponseCode.ServiceNotExists);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "99":
                            //Lỗi không xác định khi xử lý giao dịch
                            _APIResponse = new APIResponse((int)ResponseCode.SystemError);
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
                        NLogLogger.Info(new string[] { "PayDirectCard", transaction.TransactionID.ToString(), "Error", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "PayDirectCard", transaction.TransactionID.ToString(), "Error", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "PayDirectCard", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - UseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "PayDirectCard", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "PayDirectCard", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
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

        private CardResult ProcessCard(CardRequest cardRequest)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            NLogLogger.Info(new string[] { "PayDirectCard", "PayDirectCardRequest", serializer.Serialize(cardRequest) });
            string responseData = PostData(ServiceUrl, serializer.Serialize(cardRequest));
            NLogLogger.Info(new string[] { "PayDirectCard", "PayDirectCardResponse", responseData });

            return serializer.Deserialize<CardResult>(responseData);
        }

        private string PostData(string uri, string postData)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Method = "POST";//GET
            //request.Accept = "JSON";
            using (Stream requestStream = request.GetRequestStream())
            {
                byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                requestStream.Write(postDatabytes, 0, postDatabytes.Length);
            }
            var webResponse = request.GetResponse();
            if (webResponse == null)
            {
                return "Unable to connect to the remote server";
            }
            var sr = new StreamReader(webResponse.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

    }

}
