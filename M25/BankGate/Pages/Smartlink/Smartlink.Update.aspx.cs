using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Utils;
using Libs.BankGate;
using Libs.BankGate.Smartlink;
using Libs.API;

public partial class Pages_Smartlink_Smartlink_Update : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        BankReponse();
    }

    private void BankReponse()
    {
        int status = 0;
        BankGateAPI _BankGateAPI = new BankGateAPI() { TransactionID = 1301264 };
        _BankGateAPI = _BankGateAPI.Get();

        Partners _Partner = new Partners() { PartnerID = _BankGateAPI.PartnerID };
        _Partner = _Partner.Get();

        // Giao dịch không tồn tại
        if (_BankGateAPI == null)
        {
            Response.Redirect("/bankgate/message.aspx?id=-1");
        }

        // Giao dịch đã thực hiện
        if (_BankGateAPI.Status != 0 && _BankGateAPI.Status != (int)ResponseCode.TransactionSuspicious)
        {
            Response.Redirect("/bankgate/message.aspx?id=-3");
        }
        else
        {

            switch (status)
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