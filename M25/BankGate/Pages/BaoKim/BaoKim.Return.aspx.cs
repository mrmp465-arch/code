using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Utils;
using Libs.BankGate;
using Libs.BankGate.BaoKim;
using Libs.API;

public partial class Pages_BaoKim_BaoKim_Return : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        BankResponse();
    }

    private void BankResponse()
    {
        string created_on = Request["created_on"] == null ? "" : Request["created_on"];
        string customer_address = Request["customer_address"] == null ? "" : Request["customer_address"];
        string customer_email = Request["customer_email"] == null ? "" : Request["customer_email"];
        string customer_name = Request["customer_name"] == null ? "" : Request["customer_name"];
        string customer_phone = Request["customer_phone"] == null ? "" : Request["customer_phone"];
        string fee_amount = Request["fee_amount"] == null ? "" : Request["fee_amount"];
        string merchant_id = Request["merchant_id"] == null ? "" : Request["merchant_id"];
        string merchant_name = Request["merchant_name"] == null ? "" : Request["merchant_name"];
        string merchant_phone = Request["merchant_phone"] == null ? "" : Request["merchant_phone"];
        string net_amount = Request["net_amount"] == null ? "" : Request["net_amount"];
        string order_id = Request["order_id"] == null ? "" : Request["order_id"];
        string payment_type = Request["payment_type"] == null ? "" : Request["payment_type"];
        string total_amount = Request["total_amount"] == null ? "" : Request["total_amount"];
        string transaction_id = Request["transaction_id"] == null ? "" : Request["transaction_id"];
        string transaction_status = Request["transaction_status"] == null ? "" : Request["transaction_status"];
        string checksum = Request["checksum"] == null ? "" : Request["checksum"];

        NLogLogger.Info(new string[] { "BankGate", order_id, "BaoKim", "Response", Request.Url.ToString() });

        BaoKim _BaoKim = new BaoKim();

        if (!_BaoKim.VerifyResponseUrl(Request.QueryString))
        {
            Response.Redirect("/bankgate/message.aspx?error=-1");
        }

        BankGateAPI _BankGateAPI = new BankGateAPI() { TransactionID = Convert.ToInt64(order_id) };
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
            NLogLogger.Info(new string[] { "BankGate", "BaoKim", _BankGateAPI.TransactionID.ToString(), "Error", "Du lieu qua han" });
            Response.Redirect("/bankgate/message.aspx?error=-1");
        }

        // Giao dịch đã thực hiện
        if (_BankGateAPI.Status == 1 && _BankGateAPI.Status == (int)ResponseCode.TransactionSuspicious)
        {
            Response.Redirect("/bankgate/message.aspx?id=-3");
        }
        else
        {
            try
            {
                _BaoKim.TransactionID = _BankGateAPI.TransactionID;
                _BaoKim.Merchant_id = Convert.ToInt64(merchant_id);
                _BaoKim.ResponseNo = transaction_id;
                _BaoKim.Created_on = Convert.ToInt64(created_on);
                _BaoKim.Payment_type = Convert.ToInt32(payment_type);
                _BaoKim.Transaction_status = Convert.ToInt32(transaction_status);
                _BaoKim.Total_amount = Convert.ToDouble(total_amount);
                _BaoKim.Net_amount = Convert.ToDouble(net_amount);
                _BaoKim.Fee_amount = Convert.ToDouble(fee_amount);
                _BaoKim.Customer_name = customer_name;
                _BaoKim.Customer_email = customer_email;
                _BaoKim.Customer_phone = customer_phone;
                _BaoKim.Customer_address = customer_address;
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "BaoKim", "Error", ex.Message.Replace("\n", " ") });
                Response.Redirect("/bankgate/message.aspx?id=-1");
            }
            _BaoKim.Update();

            switch (_BaoKim.Transaction_status)
            {
                case 1: //1: giao dịch chưa xác minh OTP
                case 2: //2: giao dịch đã xác minh OTP
                case 5: //5: giao dịch bị hủy
                case 6: //6: giao dịch bị từ chối nhận tiền
                case 7: //7: giao dịch hết hạn
                case 8: //8: giao dịch thất bại
                default: //X: các trạng thái giao dịch khác
                    _BankGateAPI.Status = (int)ResponseCode.TransactionFailed;
                    break;
                case 4: //5: giao dịch thành công
                    _BankGateAPI.Status = 1;
                    break;
                case 12: //12: giao dịch bị đóng băng
                    _BankGateAPI.Status = (int)ResponseCode.TransactionSuspicious;
                    break;
                case 13: //13: Giao dịch bị tạm giữ (thanh toán an toàn)
                    _BankGateAPI.Status = 1;
                    break;
            }

            _BankGateAPI.LogContent = Request.Url.ToString();
            _BankGateAPI.UpdateStatus();

            string url;
            url = BankGateUtils.UrlResponse(_BankGateAPI.ReturnUrl, _BankGateAPI.OrderNo, _BankGateAPI.Status.ToString(), _BankGateAPI.TransactionID.ToString(), "baokim", _BankGateAPI.TotalAmount.ToString(), _BankGateAPI.FullName, _BankGateAPI.Mobile, _Partner.SignatureType, _Partner.PrivateKey);

            NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "BaoKim", "Response", url });
            Response.Redirect(url);
        }

    }
}