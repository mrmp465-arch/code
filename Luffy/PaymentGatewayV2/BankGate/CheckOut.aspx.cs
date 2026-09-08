using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using Libs.BankGate;
using Libs.API;
using Libs.Utils;
using System.Text.RegularExpressions;

public partial class CheckOut : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Response.Redirect(Constant.HOME_ROOT + Resources.Url.Message + "?m=Hệ thống đang bảo trì!");

        if (IsPostBack) return;

        try
        {
            Session.RemoveAll();

            BankGateAPI _BankGateAPI = new BankGateAPI();
            string logID = DateTime.Now.ToString("MMddHHmmss") + DateTime.Now.Millisecond.ToString("D3");

            // Ghi log
            NLogLogger.Info(new string[] { "BankGate", logID, "Request", Request.Url.ToString() });

            // Kiểm tra dữ liệu
            if (!VerifyRequest(ref _BankGateAPI))
            {
                NLogLogger.Info(new string[] { "BankGate", logID, "Return", "Du lieu khong hop le" });
                Response.Redirect(Constant.HOME_ROOT + Resources.Url.Message + "?error=-1", false);
                return;
            }

            // Kiểm tra chữ ký
            Partners _Partner = new Partners().Get(_BankGateAPI.PartnerID);
            if (!CheckSignature(_BankGateAPI, _Partner))
            {
                NLogLogger.Info(new string[] { "BankGate", logID, "Return", "Chu ky khong hop le" });
                Response.Redirect(Constant.HOME_ROOT + Resources.Url.Message + "?error=-1", false);
                return;
            }

            lblOrderNo.Text = _BankGateAPI.OrderNo;
            lblOrderInfo.Text = _BankGateAPI.OrderInfo;
            lblAmount.Text = _BankGateAPI.Amount.ToString("#,#").Replace(",", ".") + " VNĐ";

            Session["BankGateAPI"] = _BankGateAPI;
            Session["Partner"] = _Partner;
        }
        catch (Exception ex)
        {
            NLogLogger.Info(new string[] { "BankGate", "Error", "CheckOut", ex.Message });
            Response.Redirect(Constant.HOME_ROOT + Resources.Url.Message + "?m=Dữ liệu không hợp lệ, bạn vui lòng thực hiện lại!");
        }
    }

    private BankGateAPI VerifyRequest()
    {
        BankGateAPI _BankGateAPI = new BankGateAPI();

        _BankGateAPI.PartnerID = AppUtils.Request("partnerid");
        if (_BankGateAPI.PartnerID <= 0) return new BankGateAPI();

        _BankGateAPI.OrderNo = Request["orderno"].Trim();
        // Chỉ gồm các ký tự a-z, A-Z và 0-9
        if (!new Regex(@"^[a-zA-Z0-9]{4,30}$").Match(_BankGateAPI.OrderNo).Success) return new BankGateAPI();

        _BankGateAPI.OrderInfo = HttpUtility.UrlDecode(Request["orderinfo"]);
        if (_BankGateAPI.OrderInfo.Length > 500) return new BankGateAPI();

        _BankGateAPI.Amount = AppUtils.Request("amount");
        if (_BankGateAPI.Amount <= 10000 || _BankGateAPI.Amount > 100000000) return new BankGateAPI();

        try
        {
            _BankGateAPI.RequestTime = Convert.ToInt64(Request["requesttime"]);
            long minTime = Convert.ToInt64(DateTime.UtcNow.AddMinutes(-10).ToString("yyyyMMddHHmmss"));
            long maxTime = Convert.ToInt64(DateTime.UtcNow.AddMinutes(10).ToString("yyyyMMddHHmmss"));
            if (_BankGateAPI.RequestTime < minTime || _BankGateAPI.RequestTime > maxTime) return new BankGateAPI();
        }
        catch
        {
            return new BankGateAPI();
        }

        _BankGateAPI.ReturnUrl = HttpUtility.UrlDecode(Request["returnurl"]).ToLower().Trim();
        if (!_BankGateAPI.ReturnUrl.StartsWith("http")) return new BankGateAPI();

        _BankGateAPI.Signature = Request["signature"];

        return _BankGateAPI;
    }

    private bool VerifyRequest(ref BankGateAPI _BankGateAPI)
    {
        _BankGateAPI.PartnerID = AppUtils.Request("partnerid");
        if (_BankGateAPI.PartnerID <= 0) return false;

        _BankGateAPI.OrderNo = Request["orderno"].Trim();
        // Chỉ gồm các ký tự a-z, A-Z và 0-9
        if (!new Regex(@"^[a-zA-Z0-9]{4,30}$").Match(_BankGateAPI.OrderNo).Success) return false;

        _BankGateAPI.OrderInfo = HttpUtility.UrlDecode(Request["orderinfo"]);
        if (_BankGateAPI.OrderInfo.Length > 500) return false;

        _BankGateAPI.Amount = AppUtils.Request("amount");
        if (_BankGateAPI.Amount <= 10000 || _BankGateAPI.Amount > 100000000) return false;

        try
        {
            _BankGateAPI.RequestTime = Convert.ToInt64(Request["requesttime"]);
            long minTime = Convert.ToInt64(DateTime.UtcNow.AddMinutes(-10).ToString("yyyyMMddHHmmss"));
            long maxTime = Convert.ToInt64(DateTime.UtcNow.AddMinutes(10).ToString("yyyyMMddHHmmss"));
            if (_BankGateAPI.RequestTime < minTime || _BankGateAPI.RequestTime > maxTime) return false;
        }
        catch
        {
            return false;
        }

        _BankGateAPI.ReturnUrl = HttpUtility.UrlDecode(Request["returnurl"]).ToLower().Trim();
        //if (!_BankGateAPI.ReturnUrl.StartsWith("http")) return false;

        _BankGateAPI.Signature = Request["signature"];

        return true;
    }

    private bool CheckSignature(BankGateAPI bankGateAPI, Partners partner)
    {
        if (partner == null) return false;

        string publicKey = partner.PublicKey;
        int signatureType = partner.SignatureType;

        string data = bankGateAPI.PartnerID.ToString() + bankGateAPI.OrderNo + bankGateAPI.OrderInfo + bankGateAPI.Amount.ToString() + bankGateAPI.ReturnUrl + bankGateAPI.RequestTime.ToString();

        // Kiểm tra chữ ký
        return PaymentUtils.CheckSignature(data, bankGateAPI.Signature, publicKey, signatureType);
    }

    protected void lbNext_Click(object sender, EventArgs e)
    {
        if (Session["BankGateAPI"] == null)
        {
            lblErrorMessage.Text = "Dữ liệu không hợp lệ hoặc phiên làm việc đã hết, bạn vui lòng thực hiện lại";
            lblErrorMessage.Visible = true;
            lblErrorMessage.Focus();
            return;
        }

        if (txtFullName.Text.Trim().Length < 1)
        {
            lblErrorMessage.Text = "Bạn chưa nhập họ tên!";
            lblErrorMessage.Visible = true;
            lblErrorMessage.Focus();
            return;
        }

        if (!CheckMobile(txtMobile.Text.Trim()))
        {
            lblErrorMessage.Text = "Bạn cần nhập chính xác số điện thoại!";
            lblErrorMessage.Visible = true;
            lblErrorMessage.Focus();
            return;
        }


        BankGateAPI _BankGateAPI = new BankGateAPI();
        _BankGateAPI = (BankGateAPI)Session["BankGateAPI"];

        _BankGateAPI.FullName = txtFullName.Text.Trim();
        _BankGateAPI.Mobile = txtMobile.Text.Trim();

        Response.Redirect(Constant.HOME_ROOT + Resources.Url.CheckOutStep2);
    }

    protected void lbCancel_Click(object sender, EventArgs e)
    {
        if (Session["BankGateAPI"] == null)
        {
            lblErrorMessage.Text = "Dữ liệu không hợp lệ hoặc phiên làm việc đã hết, bạn vui lòng thực hiện lại";
            lblErrorMessage.Visible = true;
            lblErrorMessage.Focus();
            return;
        }

        BankGateAPI _BankGateAPI = (BankGateAPI)Session["BankGateAPI"];
        Partners _Partner = (Partners)Session["Partner"];

        string url = BankGateUtils.UrlResponse(_BankGateAPI.ReturnUrl, _BankGateAPI.OrderNo, ((int)ResponseCode.TransactionCancel).ToString(), "0", _Partner.SignatureType, _Partner.PrivateKey);
        NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "Response", url });
        Response.Redirect(url);
    }

    private bool CheckMobile(string mobile)
    {
        if (mobile.StartsWith("84") && mobile.Length > 3) mobile = "0" + mobile.Substring(2);

        if (!new Regex(@"^[0-9]{10,11}$").Match(mobile).Success) return false;

        if (mobile.StartsWith("09") && mobile.Length == 10) return true;

        if (mobile.StartsWith("01") && mobile.Length == 11) return true;

        return false;
    }
}