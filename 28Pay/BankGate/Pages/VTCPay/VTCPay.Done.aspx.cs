using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Libs.Utils;
using Libs.BankGate;
using Libs.API;

public partial class Pages_VTCPay_VTCPay_Done : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        if (Request.HttpMethod.ToUpper() == "GET")
        {
            AGET();
        }
        else if (Request.HttpMethod.ToUpper() == "POST")
        {
            APOST();
        }
    }

    private void AGET()
    {
        NLogLogger.Info(new string[] { "BankGate", "VTCPay GET", "Response", Request.Url.ToString() });

        int status = 0;
        int website_id = 0;
        string order_code = "";
        long transactionID = 0;
        int amount = 0;
        string sign = "";

        try
        {
            status = Convert.ToInt32(Request["status"]);
            website_id = Convert.ToInt32(Request["website_id"]);
            amount = Convert.ToInt32(Request["amount"]);
            order_code = Request["order_code"];
            transactionID = Convert.ToInt64(order_code);
            sign = Request["sign"];
        }
        catch
        {
            NLogLogger.Info(new string[] { "BankGate", "VTCPay GET", "Response", "Error", "Du lieu khong hop le" });
            Response.Redirect("/bankgate/message.aspx?error=-1");
        }

        // Kiểm tra chữ ký: status + “-“ + website_id + “-“ + order_code + “-“ + amount + “-“ + secret_key
        string secret_key = ConfigurationManager.AppSettings["VTCPay_SecretKey"];
        string data = status.ToString() + "-" + website_id + "-" + order_code + "-" + amount + "-" + secret_key;
        if (Encrypts.SHA256(data) != sign)
        {
            NLogLogger.Info(new string[] { "BankGate", "VTCPay GET", "Response", "Error", "Chu ky khong hop le" });
            Response.Redirect("/bankgate/message.aspx?error=-1");
        }

        BankGateAPI _BankGateAPI = new BankGateAPI() { TransactionID = transactionID };
        _BankGateAPI = _BankGateAPI.Get();

        if (_BankGateAPI == null)
        {
            {
                NLogLogger.Info(new string[] { "BankGate", "VTCPay GET", "Response", "Error", "Du lieu khong hop le" });
                Response.Redirect("/bankgate/message.aspx?error=-1");
            }
        }

        Libs.BankGate.VTCPay.VTCPay _VTCPay = new Libs.BankGate.VTCPay.VTCPay() { TransactionID = transactionID };
        _VTCPay = _VTCPay.Get();

        Partners _Partner = new Partners() { PartnerID = _BankGateAPI.PartnerID };
        _Partner = _Partner.Get();

        // Kiểm tra thời gian
        if (_BankGateAPI.CreatedTime.AddDays(10) < DateTime.Now)
        {
            NLogLogger.Info(new string[] { "BankGate", "VTCPay GET", _BankGateAPI.TransactionID.ToString(), "Error", "Du lieu qua han" });
            Response.Redirect("/bankgate/message.aspx?error=-1");
        }

        // Giao dịch đang thực hiện
        if (_BankGateAPI.Status == 0)
        {
            _VTCPay.ResponseAmount = amount;
            _VTCPay.ResponCode = status;
            _VTCPay.Update();

            switch (status)
            {
                case 1:
                    //1: Giao dịch thành công (hình thức thanh toán ngay)
                    _BankGateAPI.Status = (int)ResponseCode.TransactionSuccessful;
                    break;
                case 2:
                    //2: Giao dịch thành công (hình thức tạm  giữ)
                    _BankGateAPI.Status = (int)ResponseCode.TransactionSuccessful;
                    break;
                case 0:
                    //0: Giao dịch đang xử lý 
                    //_BankGateAPI.Status = (int)ResponseCode.TransactionReview;
                    break;
                case -1:
                    //-1: Giao dịch thất  bại
                    _BankGateAPI.Status = (int)ResponseCode.TransactionFailed;
                    break;
                case -5:
                    //-5: Mã đơn hàng không hợp lệ
                    _BankGateAPI.Status = (int)ResponseCode.ParameterInvalid;
                    break;
                case -6:
                    //-6: Số dư không đủ thanh toán
                    _BankGateAPI.Status = (int)ResponseCode.BalanceNotEnough;
                    break;
                case 7:
                    //7: Giao dịch cần duyệt
                    _BankGateAPI.Status = (int)ResponseCode.TransactionReview;
                    break;
                default:
                    _BankGateAPI.Status = (int)ResponseCode.TransactionFailed;
                    break;

            }

            _BankGateAPI.LogContent = Request.Url.ToString();
            _BankGateAPI.UpdateStatus();
        }
        else
        {
            Response.Redirect("/bankgate/message.aspx?error=-3");
        }

        if (status == 7 || status == 0)
        {
            _BankGateAPI.Status = (int)ResponseCode.TransactionReview;
        }

        string url;
        string serviceName = "vtcpay";
        if (_BankGateAPI.ServiceID == 14)
        {
            serviceName = "creditcard";
        }
        url = BankGateUtils.UrlResponse(_BankGateAPI.ReturnUrl, _BankGateAPI.OrderNo, _BankGateAPI.Status.ToString(), _BankGateAPI.TransactionID.ToString(), serviceName, _BankGateAPI.TotalAmount.ToString(), _BankGateAPI.FullName, _BankGateAPI.Mobile, _Partner.SignatureType, _Partner.PrivateKey);

        NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "VTCPay GET", "Response", url });
        Response.Redirect(url);
    }

    private void APOST()
    {
        NLogLogger.Info(new string[] { "BankGate", "VTCPay POST", "Response", Request.Url.ToString() });

        string data = Request.Form.Get("data") ?? "";
        string sign = Request.Form.Get("sign") ?? "";

        NLogLogger.Info(new string[] { "BankGate", "VTCPay POST", "Response", data, sign });

        // Kiểm tra chữ ký
        string secret_key = ConfigurationManager.AppSettings["VTCPay_SecretKey"];
        if (Encrypts.SHA256(data + secret_key) != sign)
        {
            NLogLogger.Info(new string[] { "BankGate", "VTCPay POST", "Response", "Error", "Chu ky khong hop le" });
            return;
        }

        int status = 0;
        int website_id = 0;
        string order_code = "";
        long transactionID = 0;

        try
        {
            status = Convert.ToInt32(data.Split('|')[0]);
            order_code = data.Split('|')[1];
            website_id = Convert.ToInt32(data.Split('|')[2]);
            transactionID = Convert.ToInt64(order_code);
        }
        catch
        {
            NLogLogger.Info(new string[] { "BankGate", "VTCPay POST", "Response", "Error", "Du lieu khong hop le" });
            return;
        }

        // Lấy dữ liệu
        BankGateAPI _BankGateAPI = new BankGateAPI() { TransactionID = transactionID };
        _BankGateAPI = _BankGateAPI.Get();

        if (_BankGateAPI == null)
        {
            {
                NLogLogger.Info(new string[] { "BankGate", "VTCPay POST", "Response", "Error", "Du lieu khong hop le" });
                return;
            }
        }

        Libs.BankGate.VTCPay.VTCPay _VTCPay = new Libs.BankGate.VTCPay.VTCPay() { TransactionID = transactionID };
        _VTCPay = _VTCPay.Get();

        Partners _Partner = new Partners() { PartnerID = _BankGateAPI.PartnerID };
        _Partner = _Partner.Get();

        // Giao dịch đang thực hiện
        if (_BankGateAPI.Status == 0 || _BankGateAPI.Status == (int)ResponseCode.TransactionReview)
        {
            _VTCPay.ResponCode = status;
            _VTCPay.Update();

            switch (status)
            {
                case 1:
                    //1: Giao dịch thành công (hình thức thanh toán ngay)
                    _BankGateAPI.Status = (int)ResponseCode.TransactionSuccessful;
                    break;
                case 2:
                    //2: Giao dịch thành công (hình thức tạm  giữ)
                    _BankGateAPI.Status = (int)ResponseCode.TransactionSuccessful;
                    break;
                case 0:
                    //0: Giao dịch đang xử lý 
                    break;
                case -1:
                    //-1: Giao dịch thất  bại
                    _BankGateAPI.Status = (int)ResponseCode.TransactionFailed;
                    break;
                case -5:
                    //-5: Mã đơn hàng không hợp lệ
                    _BankGateAPI.Status = (int)ResponseCode.ParameterInvalid;
                    break;
                case -6:
                    //-6: Số dư không đủ thanh toán
                    _BankGateAPI.Status = (int)ResponseCode.BalanceNotEnough;
                    break;
                case 7:
                    //7: Giao dịch cần duyệt
                    _BankGateAPI.Status = (int)ResponseCode.TransactionReview;
                    break;
                default:
                    _BankGateAPI.Status = (int)ResponseCode.TransactionFailed;
                    break;

            }

            _BankGateAPI.LogContent = Request.Url.ToString();
            _BankGateAPI.UpdateStatus();

            // Gọi VGG Billing để xác nhận
            VGGBilling _VGGBilling = new VGGBilling(ConfigurationManager.AppSettings["VTCPay_VGGBilling_Url"]);
            string partnerKey = ConfigurationManager.AppSettings["VTCPay_VGGBilling_Key"];
            string responseTime = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string orderNo = _BankGateAPI.OrderNo;
            status = _BankGateAPI.Status;
            string responseNo = _BankGateAPI.TransactionID.ToString();
            string privateKey = _Partner.PrivateKey;
            int signatureType = _Partner.SignatureType;

            data = orderNo + status + responseNo + responseTime;
            string signature = PaymentUtils.Signature(data, privateKey, signatureType);

            int r;
            r = _VGGBilling.ProcessBankGateRequest(partnerKey, orderNo, status, responseNo, responseTime, signature, "creditcard", _BankGateAPI.TotalAmount, _BankGateAPI.FullName, _BankGateAPI.Mobile);
            NLogLogger.Info(new string[] { "BankGate", "VTCPay POST", "Response", "Error", _BankGateAPI.TransactionID.ToString(), r.ToString() });
        }
        else
        {
            NLogLogger.Info(new string[] { "BankGate", "VTCPay POST", "Response", "Error", _BankGateAPI.TransactionID.ToString(), "giao dịch đã xử lý" });
            return;
        }


    }
}