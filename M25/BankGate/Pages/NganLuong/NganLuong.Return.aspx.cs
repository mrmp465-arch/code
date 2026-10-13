using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using Libs.BankGate;
using Libs.BankGate.NganLuong;
using Libs.API;
using Libs.Utils;

public partial class Pages_NganLuong_NganLuong_Return : System.Web.UI.Page
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
        NLogLogger.Info(new string[] { "BankGate", "NganLuong", "Response", Request.Url.ToString() });

        BankGateAPI _BankGateAPI = new BankGateAPI();
        NganLuong _NganLuong = new NganLuong();

        string token = Request["token"];

        ResponseCheckOrder response = new ResponseCheckOrder();
        response = _NganLuong.GetTransactionDetail(token);

        // Nếu không lấy được thông tin giao dịch
        if (response.errorCode != "00")
        {
            Response.Redirect(Constant.HOME_ROOT + Resources.Url.Message + "?error=-1");
        }

        _BankGateAPI.TransactionID = Convert.ToInt32(response.order_code);
        _BankGateAPI = _BankGateAPI.Get();

        Partners _Partner = new Partners() { PartnerID = _BankGateAPI.PartnerID };
        _Partner = _Partner.Get();

        // Giao dịch không tồn tại
        if (_BankGateAPI == null || _Partner == null)
        {
            Response.Redirect(Constant.HOME_ROOT + Resources.Url.Message + "?error=-1");
        }

        // Kiểm tra thời gian
        if (_BankGateAPI.CreatedTime.AddDays(10) < DateTime.Now)
        {
            NLogLogger.Info(new string[] { "BankGate", "NganLuong", _BankGateAPI.TransactionID.ToString(), "Error", "Du lieu qua han" });
            Response.Redirect("/bankgate/message.aspx?error=-1");
        }

        // Giao dịch đã thực hiện
        if (_BankGateAPI.Status != 0 && _BankGateAPI.Status != (int)ResponseCode.TransactionSuspicious)
        {
            Response.Redirect(Constant.HOME_ROOT + Resources.Url.Message + "?error=-3");
        }
        else
        {
            _NganLuong = _NganLuong.Get(_BankGateAPI.TransactionID);

            // Giao dịch chưa được thực hiện
            if (response.transactionStatus != "00")
            {
                Response.Redirect(Constant.HOME_ROOT + Resources.Url.Message + "?m=" + HttpUtility.UrlEncode("Giao dịch chưa được thanh toán!"));
            }

            _BankGateAPI.Status = 1;
            _BankGateAPI.LogContent = Request.Url.ToString();
            _BankGateAPI.UpdateStatus();

            string url;
            url = BankGateUtils.UrlResponse(_BankGateAPI.ReturnUrl, _BankGateAPI.OrderNo, _BankGateAPI.Status.ToString(), _BankGateAPI.TransactionID.ToString(), "nganluong", _BankGateAPI.TotalAmount.ToString(), _BankGateAPI.FullName, _BankGateAPI.Mobile, _Partner.SignatureType, _Partner.PrivateKey);

            NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "NganLuong", "Response", url });
            Response.Redirect(url);
        }
    }
}