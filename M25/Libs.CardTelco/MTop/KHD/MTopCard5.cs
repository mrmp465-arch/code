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
using System.Security.Policy;
using System.Security.Principal;
using System.Web;
using Libs.CardTelco.MTop;

namespace Libs.CardTelco.MTop.KHD
{
    public class MTopCard5 : ICardTelcoHandler
    {

        protected static string providerName = "mtop5";

        // Sandbox
        //protected string ServiceUrl = "https://sandbox.paydirect.vn/voucher/rest/useCard";
        //protected string PartnerCode = "sandbox";
        //protected string Password = "123456";
        //protected string SecretKey = "sandbox_sk";

        // Production : Không hóa đơn
        protected static string BaseUrl = "https://api.mtop.vn/";
        protected static string username = "‎0986909640";
        protected static string password = "@Meoicondoi2018";
        protected static string accountNo = "0829804988";
        protected static string authenCode = "161205";
        protected static string token = string.Empty;

        private string login()
        {
            var token = (string)HttpContext.Current.Application["MTopToken"];
            if (string.IsNullOrEmpty(token))
            {
                NLogLogger.Info(new string[] { "MTop",  "Login" });
                var urlLogin = string.Format("{0}paydee/login?username={1}&password={2}", BaseUrl, username, password);
                token = MTopCardLib.login(urlLogin);
                HttpContext.Current.Application["MTopToken"] = token;
            }
            return token;
        }

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
                        telcoCode = "VTT";
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
                    case "zing":
                        telcoCode = "ZING";
                        break;
                    case "mtop":
                        telcoCode = "MTOP";
                        break;

                }
                CardRequest cardRequest = new CardRequest();
                cardRequest.issuer = telcoCode;
                cardRequest.cardSerial = request.CardSerial;
                cardRequest.cardCode = request.CardCode;
                cardRequest.accountNo = accountNo;
                cardRequest.walletType = "MTOP";
                cardRequest.serviceId = "1";

                token = login();
                var urlUseCard = string.Format("{0}statement/topup-wallet-by-card?token-id={1}", BaseUrl, token);
                NLogLogger.Info(new string[] { "MTop", transaction.TransactionID.ToString(), "MTopRequest", serializer.Serialize(cardRequest) });
                CardResponse cardResult = MTopCardLib.ProcessCard(cardRequest, urlUseCard);
                NLogLogger.Info(new string[] { "MTop", transaction.TransactionID.ToString(), "MTopResponse", serializer.Serialize(cardResult) });

                step = 4;
                // Nếu thành công
                if (cardResult.code == "01")
                {

                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    _CardAPILog.Amount = Convert.ToInt64(cardResult.data.price);
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = "Amount: " + _CardAPILog.Amount.ToString();
                    _CardAPILog.Status = 1;
                }
                else
                {
                    switch (cardResult.code)
                    {
                       
                        case "100":
                            //Mã số nạp tiền không tồn tại hoặc đã được sử dụng
                            _APIResponse = new APIResponse((int)ResponseCode.CardHasExpired);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "103":
                            //Thẻ đã được sử dụng
                            _APIResponse = new APIResponse((int)ResponseCode.CardUsed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "104":
                            //Thẻ đã bị khóa
                            _APIResponse = new APIResponse((int)ResponseCode.CardIsLocked);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "105":
                            //Thẻ đã hết hạn sử dụng
                            _APIResponse = new APIResponse((int)ResponseCode.CardHasExpired);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "106":
                            //Thẻ chưa được kích hoạt
                            _APIResponse = new APIResponse((int)ResponseCode.CardNotActivated);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "107":
                            //Thực hiện sai quá số lần cho phép
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionLimit);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "108":
                            //Giao dịch nghi vấn (Timeout từ Đơn vị phát hành thẻ, chưa xử lý xong)
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "109":
                            //Lỗi nhà cung cấp
                            _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "120":
                            //Sai định dạng thông tin truyền vào
                            _APIResponse = new APIResponse((int)ResponseCode.ParameterInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "123":
                            //Serial thẻ không hợp lệ
                            _APIResponse = new APIResponse((int)ResponseCode.CardSerialInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "124":
                            //Mã số nạp tiền và serial không khớp
                            _APIResponse = new APIResponse((int)ResponseCode.CardCodeInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "128":
                            //Mã số nạp tiền không đúng định dạng
                            _APIResponse = new APIResponse((int)ResponseCode.ParameterInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "141":
                            //Lỗi khi đơn vị phát hành thẻ xử lý giao dịch
                            _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "199":
                            //Lỗi không xác định
                            _APIResponse = new APIResponse((int)ResponseCode.UndefinedError);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "-5001":
                            //không kết nối được nhà cung cấp
                            _APIResponse = new APIResponse((int)ResponseCode.PaymentConnectionFailed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "-5002":
                            //Ngắt kết nối đến nhà cung cấp. Timeout exception
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionTimeout);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        default:
                            _APIResponse = new APIResponse((int)ResponseCode.UndefinedError);
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
                        NLogLogger.Info(new string[] { "MTop", transaction.TransactionID.ToString(), "Error", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "MTop", transaction.TransactionID.ToString(), "Error", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "MTop", transaction.TransactionID.ToString(), "Error", "Step3", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - UseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "MTop", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "MTop", transaction.TransactionID.ToString(), "Error", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = ex.Message.Replace("\n", " ");
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

    }

}
