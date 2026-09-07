using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Utils;
using Libs.BankGate;
using Libs.BankGate.Smartlink;
using Libs.API;

public partial class Pages_Smartlink_Smartlink_Return : System.Web.UI.Page
{
    private string SecretKey = ConfigurationManager.AppSettings["Smartlink_SecretKey"];

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        BankReponse();
    }

    private void BankReponse()
    {
        string vpc_Version = Request["vpc_Version"] == null ? "" : HttpUtility.UrlDecode(Request["vpc_Version"]);
        string vpc_Locale = Request["vpc_Locale"] == null ? "" : HttpUtility.UrlDecode(Request["vpc_Locale"]);
        string vpc_Command = Request["vpc_Command"] == null ? "" : HttpUtility.UrlDecode(Request["vpc_Command"]);
        string vpc_Merchant = Request["vpc_Merchant"] == null ? "" : HttpUtility.UrlDecode(Request["vpc_Merchant"]);
        string vpc_MerchTxnRef = Request["vpc_MerchTxnRef"] == null ? "" : HttpUtility.UrlDecode(Request["vpc_MerchTxnRef"]);
        string vpc_Amount = Request["vpc_Amount"] == null ? "" : HttpUtility.UrlDecode(Request["vpc_Amount"]);
        string vpc_CurrencyCode = Request["vpc_CurrencyCode"] == null ? "" : HttpUtility.UrlDecode(Request["vpc_CurrencyCode"]);
        string vpc_OrderInfo = Request["vpc_OrderInfo"] == null ? "" : HttpUtility.UrlDecode(Request["vpc_OrderInfo"]);
        string vpc_ResponseCode = Request["vpc_ResponseCode"] == null ? "" : HttpUtility.UrlDecode(Request["vpc_ResponseCode"]);
        string vpc_TransactionNo = Request["vpc_TransactionNo"] == null ? "" : HttpUtility.UrlDecode(Request["vpc_TransactionNo"]);
        string vpc_CardType = Request["vpc_CardType"] == null ? "" : HttpUtility.UrlDecode(Request["vpc_CardType"]);
        string vpc_BatchNo = Request["vpc_BatchNo"] == null ? "" : HttpUtility.UrlDecode(Request["vpc_BatchNo"]);
        string vpc_AcqResponseCode = Request["vpc_AcqResponseCode"] == null ? "" : HttpUtility.UrlDecode(Request["vpc_AcqResponseCode"]);
        string vpc_Message = Request["vpc_Message"] == null ? "" : HttpUtility.UrlDecode(Request["vpc_Message"]);
        string vpc_AdditionalData = Request["vpc_AdditionalData"] == null ? "" : HttpUtility.UrlDecode(Request["vpc_AdditionalData"]);
        string vpc_SecureHash = Request["vpc_SecureHash"] == null ? "" : HttpUtility.UrlDecode(Request["vpc_SecureHash"]);

        NLogLogger.Info(new string[] { "BankGate", vpc_MerchTxnRef, "Smartlink", "Response", Request.Url.ToString() });

        string sign = SecretKey
            + vpc_AcqResponseCode
            + vpc_AdditionalData
            + vpc_Amount
            + vpc_BatchNo
            + vpc_CardType
            + vpc_Command
            + vpc_CurrencyCode
            + vpc_Locale
            + vpc_MerchTxnRef
            + vpc_Merchant
            + vpc_Message
            + vpc_OrderInfo
            + vpc_ResponseCode
            + vpc_TransactionNo
            + vpc_Version;

        sign = Encrypts.MD5(sign).ToUpper();

        if (sign != vpc_SecureHash)
        {
            Response.Redirect("/bankgate/message.aspx?error=-1");
        }

        BankGateAPI _BankGateAPI = new BankGateAPI() { TransactionID = Convert.ToInt64(vpc_MerchTxnRef)};
        _BankGateAPI = _BankGateAPI.Get();

        Partners _Partner = new Partners() { PartnerID = _BankGateAPI.PartnerID };
        _Partner = _Partner.Get();

        // Giao dịch không tồn tại
        if (_BankGateAPI == null || _BankGateAPI.TotalAmount * 100 != Convert.ToInt32(vpc_Amount))
        {
            Response.Redirect("/bankgate/message.aspx?error=-1");
        }

        // Giao dịch đã thực hiện
        if (_BankGateAPI.Status != 0 && _BankGateAPI.Status != (int)ResponseCode.TransactionSuspicious)
        {
            Response.Redirect("/bankgate/message.aspx?error=-3");
        }
        else
        {
            Smartlink _Smartlink = new Smartlink();
            _Smartlink.TransactionID = _BankGateAPI.TransactionID;
            _Smartlink.ResponseCode = Convert.ToInt32(vpc_ResponseCode);
            _Smartlink.TransactionNo = vpc_TransactionNo;
            _Smartlink.CardType = vpc_CardType;
            _Smartlink.BatchNo = vpc_BatchNo;
            _Smartlink.AcqResponseCode = vpc_AcqResponseCode;
            _Smartlink.Message = vpc_Message;
            _Smartlink.AdditionalData = vpc_AdditionalData;
            _Smartlink.Add();

            switch (Convert.ToInt32(vpc_ResponseCode))
            {
                case 0: // Giao dịch thành công
                    _BankGateAPI.Status = 1;
                    break;
                case 1: // Ngân hàng từ chối thanh toán: thẻ/tài khoản bị khóa
                case 3: // Thẻ hết hạn
                case 4: // Lỗi người mua hàng: Quá số lần cho phép. (Sai OTP, quá hạn mức trong ngày)
                case 5: // Không có trả lời của Ngân hàng
                case 6: // Lỗi giao tiếp với Ngân hàng
                case 9: // Kiểu giao dịch không được hỗ trợ
                case 10: // Lỗi Khác
                case 11: // Giao dịch chưa được xác thực OTP
                case 12: // Giao dịch không thành công, thẻ vượt quá hạn mức trong ngày 
                case 13: // Thẻ chưa đăng kýdịch vụ giao dịch qua internet 
                case 14: // Sai OTP  
                case 15: // Sai mật khẩu 
                case 16: // Sai tên chủ thẻ  
                case 17: // Sai số thẻ
                case 18: // Sai ngày hiệu lực thẻ  (sai ngày phát hành)
                case 19: // Sai ngày hiệu lực thẻ (sai ngày hết hạn) 
                case 20: // OTP Timeout 
                case 22: // Chưa xác thực thông tin thẻ 
                case 23: // Không đủ điều kiện thanh toán (thẻ/tài khoản không hợp lệ hoặc TK không đủ số dư). 
                case 24: // Giao dịch không thành công, số tiền giao dịch vượt quá hạn mức 1 lần thanh toán. 
                case 25: // Giao dịch không thành công, số tiền giao dịch vượt hạn mức thanh toán. 
                    _BankGateAPI.Status = (int)ResponseCode.TransactionFailed;
                    break;
                case 8: // Lỗi checksum dữ liệu
                    _BankGateAPI.Status = (int)ResponseCode.SystemError;
                    break;
                case 7: // Tài khoản không đủ tiền
                    _BankGateAPI.Status = (int)ResponseCode.BalanceNotEnough;
                    break;
                case 26: // Giao dịch chờ xác nhận từ Ngân hàng
                    _BankGateAPI.Status = (int)ResponseCode.TransactionSuspicious;
                    break;
                default:
                    _BankGateAPI.Status = (int)ResponseCode.TransactionFailed;
                    break;

            }
        }

        _BankGateAPI.LogContent = Request.Url.ToString();
        _BankGateAPI.UpdateStatus();

        string url;
        url = BankGateUtils.UrlResponse(_BankGateAPI.ReturnUrl, _BankGateAPI.OrderNo, _BankGateAPI.Status.ToString(), _BankGateAPI.TransactionID.ToString(), "smartlink", _BankGateAPI.TotalAmount.ToString(), _BankGateAPI.FullName, _BankGateAPI.Mobile, _Partner.SignatureType, _Partner.PrivateKey);
        NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "Smartlink", "Response", url });

        Response.Redirect(url);
    }

}