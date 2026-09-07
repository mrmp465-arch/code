using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;

namespace Libs.CardTelco.NganLuongV2
{
    public class NganLuongCardV2
    {
        protected string webserviceUrl = "http://exu.vn/mobile_card_api_v30.php";
        protected string merchant_id = "31656";
        protected string version = "3.0";
        protected string merchant_pass = "Kjka793H3Kml6";
        protected string providerName = "nganluong";
        protected string merchant_account = "thanhtoan@vgg.vn";
        protected int timeOut = 150000;

        protected JavaScriptSerializer serializer = new JavaScriptSerializer();

        public NganLuongCardV2()
        {

        }

        public APIResponse UseCard(APITransaction transaction)
        {
            APIResponse _APIResponse = new APIResponse();
            CardAPILog _CardAPILog = new CardAPILog();

            int step = 0;
            try
            {
                // Bước 1: Phân tích yêu cầu thành đối tượng
                step = 1;
                UseCardRequest request = new UseCardRequest();
                request = serializer.Deserialize<UseCardRequest>(transaction.RequestContent);

                // Bước 2: Ghi log giao dịch
                step = 2;

                _CardAPILog.TransactionID = transaction.TransactionID;
                _CardAPILog.PartnerID = transaction.PartnerID;
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

                // Bước 3: khởi tạo dữ liệu
                step = 3;

                NLRequest _NLRequest = new NLRequest();
                _NLRequest.pin_card = request.CardCode;
                _NLRequest.type_card = request.CardType.ToUpper();
                _NLRequest.ref_code = transaction.TransactionID.ToString();
                _NLRequest.merchant_account = merchant_account;
                _NLRequest.client_fullname = request.AccountName;;
                _NLRequest.client_email = "";
                _NLRequest.client_mobile = "";
                _NLRequest.card_serial = request.CardSerial;

                string data = serializer.Serialize(_NLRequest);
                data = Encrypts.Encrypt(merchant_pass, data);


                // Bước 4: gọi sang Ngân Lượng
                step = 4;

                EXU_API _EXU_API = new EXU_API(webserviceUrl);
                NLogLogger.Info(new string[] { "NganLuongCard", transaction.TransactionID.ToString(), "NganLuongCardRequest", merchant_id, version, data });
                string webserviceResponse = _EXU_API.CardCharge(merchant_id, version, data);
                NLogLogger.Info(new string[] { "NganLuongCard", transaction.TransactionID.ToString(), "NganLuongCardResponse", webserviceResponse });

                // Bước 5: phân tích kết quả
                step = 5;

                NLResponse _NLResponse = new NLResponse();
                _NLResponse = serializer.Deserialize<NLResponse>(webserviceResponse);

                if (_NLResponse.error_code == "00")
                {
                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    _CardAPILog.Amount = Convert.ToInt64(_NLResponse.card_amount);
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Status = 1;
                }
                else
                {
                    _APIResponse = new APIResponse(ConvertResponCode(_NLResponse.error_code));
                    _CardAPILog.Status = _APIResponse.ResponseCode;
                }
                _CardAPILog.Description = webserviceResponse;
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "NganLuongCard", transaction.TransactionID.ToString(), "Error", "UseCard", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "NganLuongCard", transaction.TransactionID.ToString(), "Error", "UseCard", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "NganLuongCard", transaction.TransactionID.ToString(), "Error", "UseCard", "Step3", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Encrypts.Encrypt " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "NganLuongCard", transaction.TransactionID.ToString(), "Error", "UseCard", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 5:
                        NLogLogger.Info(new string[] { "NganLuongCard", transaction.TransactionID.ToString(), "Error", "UseCard", "Step5", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step5 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "NganLuongCard", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error " + ex.Message.Replace("\n", " ");
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

        private int ConvertResponCode(string error_code)
        {
            switch (error_code)
            {
                case "00":
                    //00 Đã xử lý thành công (thẻ đã bị gạch, tiền đã được nạp vào tài khoản của merchant)
                    return (int)ResponseCode.TransactionSuccessful;
                case "99":
                    //01 Lỗi, tuy nhiên lỗi chưa được định nghĩa/không xác định
                    return (int)ResponseCode.TransactionFailed;
                case "01":
                    //01 Lỗi, địa chỉ IP truy cập API của NgânLượng.vn bị từ chối
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "02":
                    //02 Lỗi, Mã website/merchant không tồn tại hoặc website/merchant đang bị khóa
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "03":
                    //03 Lỗi, Địa chỉ IP truy cập API của NgânLượng.vn bị từ chối
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "04":
                    //04 Lỗi, Mã checksum không chính xác
                    return (int)ResponseCode.TransactionFailed;
                case "05":
                    //05 Tài khoản nhận tiền nạp của merchant không tồn tại
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "06":
                    //06 Tài khoản nhận tiền nạp của merchant đang bị khóa hoặc bị phong tỏa, không thể thực hiện được giao dịch nạp tiền
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "07":
                    //07 Thẻ đã được sử dụng, hoặc thẻ sai
                    return (int)ResponseCode.CardUsed;
                case "09":
                    //09 Thẻ hết hạn sử dụng
                    return (int)ResponseCode.CardHasExpired;
                case "10":
                    //10 Thẻ chưa được kích hoạt hoặc không tồn tại
                    return (int)ResponseCode.CardNotActivated;
                case "11":
                    //11 Mã thẻ sai định dạng
                    return (int)ResponseCode.CardCodeInvalid;
                case "12":
                    //12 Sai số serial của thẻ
                    return (int)ResponseCode.CardSerialInvalid;
                case "13":
                    //13 Mã thẻ và số serial không khớp
                    return (int)ResponseCode.CardCodeInvalid;
                case "14":
                    //14 Thẻ không tồn tại
                    return (int)ResponseCode.CardCodeInvalid;
                case "15":
                    //15 Thẻ không sử dụng được
                    return (int)ResponseCode.CardIsLocked;
                case "16":
                    //16 Số lần thử (nhập sai liên tiếp) của thẻ vượt quá giới hạn cho phép
                    return (int)ResponseCode.CardIsLocked;
                case "17":
                    //17 Hệ thống Telco bị lỗi hoặc quá tải, thẻ chưa bị trừ
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "18":
                    //18 Hệ thống Telco bị lỗi hoặc quá tải, thẻ có thể bị trừ, cần phối hợp với NgânLượng.vn để tra soát
                    return (int)ResponseCode.TransactionSuspicious;
                //19		Kết nối từ NgânLượng.vn tới hệ thống Telco bị lỗi, thẻ chưa bị trừ (thường do lỗi kết nối giữa NgânLượng.vn với Telco, ví dụ sai tham số kết nối, mà không liên quan đến merchant)
                //20	Kết nối tới telco thành công, thẻ bị trừ nhưng chưa cộng tiền trên NgânLượng.vn
                default:
                    return (int)ResponseCode.TransactionFailed;
            }
        }
    }

    public class NLRequest
    {
        public string pin_card { get; set; }               // mã thẻ
        public string type_card { get; set; }           // loại thẻ: VMS, VNP, VIETTEL, VCOIN
        public string ref_code { get; set; }            // mã giao dịch
        public string merchant_account { get; set; }    // email của VGG trên hệ thống Ngân Lượng
        public string client_fullname { get; set; }     // họ tên khách hàng
        public string client_email { get; set; }        // email của khách hàng
        public string client_mobile { get; set; }       // số điện thoại của khách hàng
        public string card_serial { get; set; }         // số serial của thẻ

        public NLRequest()
        {

        }

    }

    public class NLResponse
    {
        public string error_code { get; set; }  
        public string merchant_id { get; set; } 
        public string merchant_account { get; set; }
        public string type_card { get; set; }  
        public string pin_card { get; set; }   
        public string card_serial { get; set; }
        public string ref_code { get; set; }   
        public string client_fullname { get; set; }
        public string client_email { get; set; }
        public string client_mobile { get; set; }
        public string card_amount { get; set; }
        public string transaction_amount { get; set; }
        public string transaction_id { get; set; }

        public NLResponse()
        {

        }

    }
}
