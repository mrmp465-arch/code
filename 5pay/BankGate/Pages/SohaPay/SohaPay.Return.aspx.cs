using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Utils;
using Libs.BankGate;
using Libs.BankGate.SohaPay;
using Libs.API;

public partial class Pages_SohaPay_SohaPay_Return : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BankResponse();
        }
    }

    private void BankResponse()
    {
        NLogLogger.Info(new string[] { "BankGate", "SohaPay", "Response", Request.Url.ToString() });

        SohaPay _SohaPay = new SohaPay();

        // Sai chữ ký
        if (!_SohaPay.VerifyReturnUrl())
        {
            Response.Redirect("/bankgate/message.aspx?error=-1");
        }

        BankGateAPI _BankGateAPI = new BankGateAPI();
        try
        {
            _BankGateAPI.TransactionID = Convert.ToInt64(Request["order_code"]);
        }
        catch
        {
            Response.Redirect("/bankgate/message.aspx?error=-1");
        }

        _BankGateAPI = _BankGateAPI.Get();

        Partners _Partner = new Partners() { PartnerID = _BankGateAPI.PartnerID };
        _Partner = _Partner.Get();

        // Giao dịch không tồn tại
        if (_BankGateAPI == null || _Partner == null)
        {
            Response.Redirect("/bankgate/message.aspx?id=-1");
        }

        // Kiểm tra thời gian
        if (_BankGateAPI.CreatedTime.AddDays(10) < DateTime.Now)
        {
            NLogLogger.Info(new string[] { "BankGate", "SohaPay", _BankGateAPI.TransactionID.ToString(), "Error", "Du lieu qua han" });
            Response.Redirect("/bankgate/message.aspx?error=-1");
        }

        // Giao dịch đã thực hiện
        if (_BankGateAPI.Status == 1 && _BankGateAPI.Status == (int)ResponseCode.TransactionSuspicious)
        {
            Response.Redirect("/bankgate/message.aspx?id=-3");
        }

        // Giá trị giao dịch không khớp
        if (_BankGateAPI.TotalAmount != AppUtils.Request("price"))
        {
            Response.Redirect("/bankgate/message.aspx?id=-1");
        }

        try
        {
            _SohaPay.TransactionID = _BankGateAPI.TransactionID;
            _SohaPay = _SohaPay.Get(_BankGateAPI.TransactionID);

            _SohaPay.Order_product_title = HttpContext.Current.Request["order_product_title"] == null ? "" : HttpContext.Current.Request["order_product_title"];
            _SohaPay.Response_code = HttpContext.Current.Request["response_code"] == null ? "" : HttpContext.Current.Request["response_code"];
            _SohaPay.Response_message = HttpContext.Current.Request["response_message"] == null ? "" : HttpContext.Current.Request["response_message"];
            _SohaPay.Payment_time = HttpContext.Current.Request["payment_time"] == null ? "" : HttpContext.Current.Request["payment_time"];
            _SohaPay.Error_text = HttpContext.Current.Request["error_text"] == null ? "" : HttpContext.Current.Request["error_text"];
        }
        catch (Exception ex)
        {
            NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "BaoKim", "Error", ex.Message.Replace("\n", " ") });
            Response.Redirect("/bankgate/message.aspx?id=-1");
        }
        _SohaPay.Update();

        _BankGateAPI.Status = ConvertResponseCode(_SohaPay.Response_code);
        _BankGateAPI.LogContent = Request.Url.ToString();
        _BankGateAPI.UpdateStatus();

        string url;
        url = BankGateUtils.UrlResponse(_BankGateAPI.ReturnUrl, _BankGateAPI.OrderNo, _BankGateAPI.Status.ToString(), _BankGateAPI.TransactionID.ToString(), "sohapay", _BankGateAPI.TotalAmount.ToString(), _BankGateAPI.FullName, _BankGateAPI.Mobile, _Partner.SignatureType, _Partner.PrivateKey);
        NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "SohaPay", "Response", url });

        Response.Redirect(url);
    }

    private int ConvertResponseCode(string response_code)
    {
        switch (response_code)
        {
            case "0":   //  0: Giao dịch thành công
                return (int)ResponseCode.TransactionSuccessful;
            case "1":   //  1: Ngân hàng từ chối giao dịch
            case "5":   //  5: Số tiền không hợp lệ
            case "7":   //  7: Lỗi không xác định
            case "8":   //  8: Số thẻ không đúng
            case "9":   //  9: Tên chủ thẻ không đúng
            case "10":  //  10: Thẻ hết hạn/Thẻ bị khóa
            case "11":  //  11: Thẻ chưa đăng ký sử dụng dịch vụ thanh toán trực tuyến.
            case "12":  //  12: Ngày phát hành/Hết hạn không đúng
            case "13":  //  13: Vượt quá hạn mức thanh toán
            case "99":  //  99: Người sử dụng hủy giao dịch
            case "100":  //  100: Không nhập thông tin thẻ/ Hủy giao dịch thanh toán
                return (int)ResponseCode.TransactionFailed;
            case "3":  //  3: Mã đơn vị không tồn tại
            case "4":  //  4: Không đúng access code
            case "6":  //  6: Mã tiền tệ không tồn tại
                return (int)ResponseCode.PaymentConnectionFailed;
            case "21":  //  21: Số dư không đủ để thanh toán
                return (int)ResponseCode.BalanceNotEnough;
            case "PG":  //  PG: Không tồn tại giao dịch trên hệ thống
                return (int)ResponseCode.TransactionNotExists;
            default:
                return (int)ResponseCode.TransactionFailed;
        }
    }
}