using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;
using Libs.Utils;

namespace Libs.BankDirect.Banknet
{
    public class Banknetvn
    {
         
        private string country_code = "vn";

        // Cấu hình
        private string merchant_code = "010078";
        private string merchant_trans_key = "ofuodj#$AHB54@&pdhmn";
        protected string webserviceUrl = "https://direct.banknetvn.com.vn/pg/services/DirectPayment.DirectPaymentHttpSoap11" + "Endpoint";

        // Sandbox
        //private string merchant_code = "011013";
        //private string merchant_trans_key = "555677oxt42d04dc44446dxx11134253";
        //protected string webserviceUrl = "http://sandbox.bndebit.vn/pg2705/services/DirectPayment.DirectPaymentHttpSoap11En" + "dpoint";

        //Trạng thái step: 1 - tạo giao dịch, 2 - xác thực thông tin thẻ hợp lệ, 3 - xác thực otp thành công, 4 - xác nhận với banknet thành công.
        public Banknetvn()
        {

        }

        // Thêm mới giao dịch
        public APIResponse AddTransaction(BankGateAPI bankGateAPI, string bankCode)
        {
            DirectPayment _DirectPayment = new DirectPayment(webserviceUrl);
            BanknetDirect _BanknetDirect = new BanknetDirect();

            _BanknetDirect.TransactionID = bankGateAPI.TransactionID;
            _BanknetDirect.Selected_bank = bankCode;
            _BanknetDirect.Add();

            if (_BanknetDirect.ReturnValue < 0)
            {
                return new APIResponse((int)ResponseCode.SystemError);
            }

            string merchant_trans_id = (bankGateAPI.TransactionID % 1000000).ToString("D6");
            string good_code = bankGateAPI.TransactionID.ToString();
            string xml_description = bankGateAPI.OrderInfo;
            string net_cost = bankGateAPI.TotalAmount.ToString();
            string ship_fee = "0";
            string tax = "0";
            string trans_datetime = bankGateAPI.CreatedTime.ToString("yyyyMMddHHmmss");

            string trans_secure_code = Encrypts.MD5(merchant_trans_id + merchant_code + good_code + net_cost + ship_fee + tax + merchant_trans_key);

            NLogLogger.Info(new string[] { "BankDirectService", "Banknetvn", bankGateAPI.TransactionID.ToString(), "AddTransaction", "Request", merchant_trans_id, merchant_code, country_code, good_code, xml_description, net_cost, ship_fee, tax, trans_datetime, trans_secure_code, bankCode });
            string s = _DirectPayment.SendGoodInfo_Ext(merchant_trans_id, merchant_code, country_code, good_code, xml_description, net_cost, ship_fee, tax, trans_datetime, trans_secure_code, bankCode);
            NLogLogger.Info(new string[] { "BankDirectService", "Banknetvn", bankGateAPI.TransactionID.ToString(), "AddTransaction", "Response", s });

            string[] list = s.Split('|');

            switch (list[0])
            {
                case "00": // Thành công
                    _BanknetDirect.Trans_id = list[2];
                    _BanknetDirect.Update();
                    return new APIResponse((int)ResponseCode.TransactionSuccessful);
                default:
                    return new APIResponse((int)ResponseCode.TransactionFailed);
            }
        }

        // Xác thực thông tin thẻ
        public APIResponse VerifyCard(VerifyCardRequest verifyCardRequest)
        {
            DirectPayment _DirectPayment = new DirectPayment(webserviceUrl);
            BanknetDirect _BanknetDirect = new BanknetDirect();

            _BanknetDirect = _BanknetDirect.Get(verifyCardRequest.TransactionID);

            if (_BanknetDirect.Step > 1)
            {
                return new APIResponse((int)ResponseCode.TransactionDuplicate);
            }

            if (verifyCardRequest.CardYear > 100)
            {
                verifyCardRequest.CardYear = verifyCardRequest.CardYear % 100;
            }

            _BanknetDirect.CardNumber = verifyCardRequest.CardNumber;
            _BanknetDirect.FullName = verifyCardRequest.FullName;
            _BanknetDirect.CardMonth = verifyCardRequest.CardMonth;
            _BanknetDirect.CardYear = verifyCardRequest.CardYear;
            _BanknetDirect.OTPType = verifyCardRequest.OTPType;


            _BanknetDirect.Update();

            if (_BanknetDirect.ReturnValue < 0)
            {
                return new APIResponse((int)ResponseCode.SystemError);
            }

            string merchant_trans_id = (_BanknetDirect.TransactionID % 1000000).ToString("D6");
            string trans_id = _BanknetDirect.Trans_id;
            string card_holder_number = _BanknetDirect.CardNumber;
            string card_holder_name = _BanknetDirect.FullName;
            string card_holder_month = _BanknetDirect.CardMonth.ToString();
            string card_holder_year = _BanknetDirect.CardYear.ToString();
            string otpGetType = _BanknetDirect.OTPType;

            string trans_secure_code = Encrypts.MD5(merchant_trans_id + merchant_code + merchant_trans_key + trans_id + card_holder_number + card_holder_year);

            NLogLogger.Info(new string[] { "BankDirectService", "Banknetvn", merchant_trans_id, "VerifyCard", "Request", trans_id, card_holder_number, card_holder_name, card_holder_month, card_holder_year, otpGetType, trans_secure_code });
            string s = _DirectPayment.checkCardHolder(merchant_trans_id, merchant_code, trans_id, card_holder_number, card_holder_name, card_holder_month, card_holder_year, otpGetType, trans_secure_code);
            NLogLogger.Info(new string[] { "BankDirectService", "Banknetvn", merchant_trans_id, "VerifyCard", "Response", s });

            string[] list = s.Split('|');

            switch (list[0])
            {
                case "00": // Thành công
                    _BanknetDirect.Step = 2;
                    _BanknetDirect.Update();
                    return new APIResponse((int)ResponseCode.TransactionSuccessful);
                case "08": // Lỗi timeout trong quá trình xử lý
                    return new APIResponse((int)ResponseCode.TransactionExpired);
                case "25": // Nhập sai thông tin chủ thẻ - Còn 2 lần nhập
                case "26": // Nhập sai thông tin chủ thẻ - Còn 1 lần nhập
                case "27": // Nhập sai thông tin chủ thẻ - Thanh toán không thành công
                    return new APIResponse((int)ResponseCode.BankCardInfoInvalid);
                case "41": // Thẻ nghi vấn (thẻ đánh mất, hot card)
                case "62": // Thẻ bị khóa
                    return new APIResponse((int)ResponseCode.CardIsLocked);
                case "54": // Thẻ hết hạn
                    return new APIResponse((int)ResponseCode.CardHasExpired);
                case "57": // Chưa đăng ký dịch vụ thanh toán trực tuyến
                    return new APIResponse((int)ResponseCode.BankServiceNotActive);
                default:
                    return new APIResponse((int)ResponseCode.TransactionFailed);
            }
        }

        // Xác thực OTP
        public APIResponse VerifyOTP(VerifyOTPRequest verifyOTPRequest)
        {
            DirectPayment _DirectPayment = new DirectPayment(webserviceUrl);
            BanknetDirect _BanknetDirect = new BanknetDirect();

            _BanknetDirect = _BanknetDirect.Get(verifyOTPRequest.TransactionID);

            // Nếu trạng thái
            if (_BanknetDirect.Step != 2)
            {
                return new APIResponse((int)ResponseCode.TransactionDuplicate);
            }

            // Cập nhật OTP
            _BanknetDirect.OTP = verifyOTPRequest.OTP;
            _BanknetDirect.Update();

            // Nếu cập nhật thất bại
            if (_BanknetDirect.ReturnValue < 0)
            {
                return new APIResponse((int)ResponseCode.SystemError);
            }

            string merchant_trans_id = (_BanknetDirect.TransactionID % 1000000).ToString("D6");
            string trans_id = _BanknetDirect.Trans_id;
            string otp_code = _BanknetDirect.OTP;
            string otpGetType = _BanknetDirect.OTPType;

            string trans_secure_code = Encrypts.MD5(merchant_trans_id + merchant_code + merchant_trans_key + otp_code);

            NLogLogger.Info(new string[] { "BankDirectService", "Banknetvn", merchant_trans_id, "VerifyOTP", "Request", trans_id, otp_code, otpGetType, trans_secure_code });
            string s = _DirectPayment.verifyOTP(merchant_trans_id, merchant_code, trans_id, otp_code, trans_secure_code, otpGetType);
            NLogLogger.Info(new string[] { "BankDirectService", "Banknetvn", merchant_trans_id, "VerifyOTP", "Response", s });

            string[] list = s.Split('|');

            switch (list[0])
            {
                case "00": // Thành công
                    _BanknetDirect.Step = 3;
                    _BanknetDirect.Update();
                    // Xác nhận thành công
                    ConfirmTransaction(_BanknetDirect.TransactionID);
                    return new APIResponse((int)ResponseCode.TransactionSuccessful);
                case "08": // Lỗi timeout trong quá trình xử lý
                    return new APIResponse((int)ResponseCode.TransactionExpired);
                case "21": // Mật khẩu OTP không đúng
                    return new APIResponse((int)ResponseCode.BankOtpInvalid);
                case "65": // Quá hạn mức giao dịch trong ngày
                    return new APIResponse((int)ResponseCode.BankAmountLimit);
                default:
                    return new APIResponse((int)ResponseCode.TransactionFailed);
            }
        }

        // Kiểm tra trạng thái giao dịch
        public APIResponse CheckTransaction(APITransaction transaction)
        {
            return new APIResponse();
        }

        // Xác thực thông tin giao dịch với ngân hàng
        public APIResponse ConfirmTransaction(long transactionID)
        {
            DirectPayment _DirectPayment = new DirectPayment(webserviceUrl);
            BanknetDirect _BanknetDirect = new BanknetDirect();

            _BanknetDirect = _BanknetDirect.Get(transactionID);

            string merchant_trans_id = (_BanknetDirect.TransactionID % 1000000).ToString("D6");
            string trans_id = _BanknetDirect.Trans_id;
            string trans_result = "0";
            string trans_secure_code = Encrypts.MD5(merchant_trans_id + trans_id + merchant_code + trans_result + merchant_trans_key);

            NLogLogger.Info(new string[] { "BankDirectService", "Banknetvn", merchant_trans_id, "ConfirmTransaction", "Request", trans_id, trans_result, trans_secure_code });
            string s = _DirectPayment.ConfirmTransactionResult(merchant_trans_id, trans_id, merchant_code, trans_result, trans_secure_code);
            NLogLogger.Info(new string[] { "BankDirectService", "Banknetvn", merchant_trans_id, "ConfirmTransaction", "Response", s });

            return new APIResponse();
        }
    }
}
