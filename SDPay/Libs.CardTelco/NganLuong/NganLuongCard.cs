using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;
using Libs.Utils;
using System.Web.Script.Serialization;
using Libs.CardTelco.Vcoin;

namespace Libs.CardTelco.NganLuong
{
    public class NganLuongCard
    {
        // Intecom
        protected string webserviceUrl = "https://nganluong.vn:443/mobile_card_api.php";
        string merchant_id = "31656";
        string merchantPassword = "fB95016582Afa0b78C";
        string merchant_account = "thanhtoan@vgg.vn";
        string providerName = "nganluong";
        
        protected JavaScriptSerializer serializer = new JavaScriptSerializer();

        public NganLuongCard()
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
                string pin_card = request.CardCode;
                string type_card = request.CardType.ToUpper();
                string ref_code = request.AccountName;
                string client_fullname = request.AccountName;
                string client_email = transaction.TransactionID.ToString() + "@vgg.vn";
                string client_mobile = "";
                string card_serial = request.CardSerial;

                string pars = pin_card
                    + "|" + type_card
                    + "|" + ref_code
                    + "|" + merchant_account
                    + "|" + client_fullname
                    + "|" + client_email
                    + "|" + client_mobile 
                    + "|" + card_serial;

                string checksum = Encrypts.MD5(pars + "|" + merchantPassword);

                // Bước 4: gọi sang Ngân Lượng
                NGANLUONG_API _NGANLUONG_API = new NGANLUONG_API(webserviceUrl);
                NLogLogger.Info(new string[] { "NganLuongCard", transaction.TransactionID.ToString(), "NganLuongCardRequest", merchant_id, checksum, pars });
                string webserviceResponse = _NGANLUONG_API.CardCharge(merchant_id, checksum, pars);
                NLogLogger.Info(new string[] { "NganLuongCard", transaction.TransactionID.ToString(), "NganLuongCardResponse", webserviceResponse });

                // Bước 5: phân tích kết quả
                string[] list = webserviceResponse.Split('|');

                if (list[0] == "00" || list[0] == "10")
                {
                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    int amount = Convert.ToInt32(list[3]);
                    _CardAPILog.Amount = ConvertAmount(amount);
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Status = 1;
                }
                else
                {
                    _APIResponse = new APIResponse(ConvertResponCode(list[0]));
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
                case "01":
                    //01 Lỗi, tuy nhiên lỗi chưa được định nghĩa/không xác định
                    return (int)ResponseCode.TransactionFailed;
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
                    //05 Lỗi, Mã thẻ cào không chính xác hoặc đã được sử dụng
                    return (int)ResponseCode.CardCodeInvalid;
                case "06":
                    //06 Lỗi, Không kết nối tới hệ thống xác thực thẻ của Telco
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "07":
                    //07 Lỗi, Tài khoản NgânLượng.vn của merchant nhận tiền nạp không tồn tại
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "08":
                    //08 Lỗi, Tài khoản NgânLượng.vn của merchant nhận tiền nạp đang bị khóa hoặc bị phong tỏa
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "09":
                    //09 Lỗi, khách hàng tương ứng với tham số ref_code bị khóa (do nhập sai mã thẻ liên tiếp nhiều lần)
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "10":
                    //10 Lỗi, thẻ bị trừ, nhưng nạp được tiền vào tài khoản NgânLượng.vn của người bán
                    return (int)ResponseCode.TransactionSuccessful;
                default:
                    return (int)ResponseCode.TransactionFailed;
            }
        }

        private int ConvertAmount(int amount)
        {
            if (amount <= 10000) return 10000;

            if (amount <= 20000) return 20000;

            if (amount <= 30000) return 30000;

            if (amount <= 50000) return 50000;

            if (amount <= 100000) return 100000;

            if (amount <= 200000) return 200000;

            if (amount <= 300000) return 300000;

            if (amount <= 500000) return 500000;

            return amount;
        }
    }
}
