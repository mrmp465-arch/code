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

public partial class Pages_VTCPay_VTCPay_Confirm : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        long transactionID = 1303149;
        int status = 1;

        // Lấy dữ liệu
        BankGateAPI _BankGateAPI = new BankGateAPI() { TransactionID = transactionID };
        _BankGateAPI = _BankGateAPI.Get();

        if (_BankGateAPI == null)
        {
            {
                NLogLogger.Info(new string[] { "BankGate", "VTCPay", "Response", "Error", "Du lieu khong hop le" });
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

            string data = "";
            data = orderNo + status + responseNo + responseTime;
            string signature = PaymentUtils.Signature(data, privateKey, signatureType);

            int r;
            r = _VGGBilling.ProcessBankGateRequest(partnerKey, orderNo, status, responseNo, responseTime, signature, "visa", _BankGateAPI.TotalAmount, _BankGateAPI.FullName, _BankGateAPI.Mobile);
            NLogLogger.Info(new string[] { "BankGate", "VTCPay", "Response", "Error", _BankGateAPI.TransactionID.ToString(), r.ToString() });
        }
    }
}