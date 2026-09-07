using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libs.SMS.SMSHandler
{
    public class SmsResponseConstant
    {
        public const string LOCK_ACCOUNT_SUCCESS = "Tai khoan: {0}. Da duoc khoa thanh cong";
        public const string UNLOCK_ACCOUNT_SUCCESS = "Tai khoan: {0}. Da duoc mo khoa thanh cong";
        public const string UNLOCK_ACCOUNT_NOTSUCCESS_LOCKEDBYADMIN = "Mo khoa khong thanh cong. Tai khoan: {0} da bi khoa boi admin";
        public const string MOBILE_NOT_EXISTS = "So dien thoai {0} cua ban chua duoc xac nhan dang ky OTP trong he thong. Vui long active so dien thoai hoac lien he hotline de duoc ho tro";
        public const string ACCOUNT_NOT_EXISTS = "Tai khoan {0} cua ban khong ton tai trong he thong. Vui long lien he hotline de duoc ho tro";
        public const string FORGOT_PASSWORD_SENT_OTP_SUCCESS = "Doi mat khau cua DESGAME ID. Username={0}, Ma xac thuc={1}. Tong dai ho tro: hotline";
        public const string SENT_OTP_SUCCESS = "{0} la mat khau OTP cua ban tren COBY";
        public const string SENT_VERIFY_SUCCESS = "{0} la ma xac thuc cua ban tren COBY";
        public const string SENT_VERIFY_FAIL = "Qua trinh khoi tao ma xa thuc loi. Vui long lien he hotline de duoc giai dap. Xin cam on!";
        public const string SMS_SYNTAX_ERROR = "Cu phap chua dung. Vui long lien he hotline de duoc giai dap. Xin cam on!";
        public const string SMS_SYSTEM_ERROR = "Loi he thong. Vui long lien he hotline de duoc giai dap. Xin cam on!";
        public const string PARTNER_NOT_FOUND = "Khong tim thay Game hoac da tam dung. Vui long kiem tra cu phap hoac lien he hotline de duoc giai dap. Xin cam on!";
        public const string PARTNER_FAIL_FOUND = "Khong thanh cong tu Game. Vui long lien he hotline de duoc giai dap. Xin cam on!";
        public const string SMS_TOPUP_SUCCESS = "Nap tien thanh cong. {0} COB da duoc nap vao tai khoan ma ID {1} cua ban. TK hien nay cua ban la {2} COB";
        public const string SMS_SYSTEM_NOT_HANDLER_ERROR = "Loi he thong. Vui long lien he hotline de duoc ho tro";
        public const string SMS_MOBILE_SERVICE_NOT_EXISTS = "Menh gia nap tien khong ho tro. Vui long lien he hotline de duoc ho tro.";

       
    }
}
