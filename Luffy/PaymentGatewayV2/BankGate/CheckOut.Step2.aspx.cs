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

public partial class CheckOut_Step2 : System.Web.UI.Page
{
    protected int amount;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["BankGateAPI"] == null) Response.Redirect(Constant.HOME_ROOT + Resources.Url.Message + "?error=-2");
        if (!IsPostBack)
        {
            BankGateAPI _BankGateAPI = (BankGateAPI)Session["BankGateAPI"];
            lblAmount.Text = _BankGateAPI.Amount.ToString("#,#").Replace(",", ".") + " VNĐ";
            lblOrderInfo.Text = _BankGateAPI.OrderInfo;

            //if (_BankGateAPI.Mobile == "0906237148")
            //{
            //    panelVisa.Visible = true;
            //}
            //else
            //{
            //    panelVisa.Visible = false;
            //}
        }
    }

    protected void lbNext_Click(object sender, EventArgs e)
    {
        string url;
        BankGateAPI _BankGateAPI = (BankGateAPI)Session["BankGateAPI"];
        Partners _Partner = (Partners)Session["Partner"];

        if (Request.Params["rdoBank"] == null)
        {
            lblErrorMessage.Text = "Bạn chưa chọn hình thức thanh toán!";
            lblErrorMessage.Visible = true;
            lblErrorMessage.Focus();
            return;
        }

        string bankName = Request.Params["rdoBank"].ToLower();

        // 2014-03-26: kiểm tra giá trị giao dịch
        if (_BankGateAPI.Amount < 90000 && (bankName == "visa" || bankName == "master" || bankName=="jcb"))
        {
            lblErrorMessage.Text = "Giá trị thanh toán qua thẻ tín dụng phải đạt tối thiểu 90.000 VNĐ!";
            lblErrorMessage.Visible = true;
            lblErrorMessage.Focus();
            return;
        }

        // Xóa session
        Session.RemoveAll();
        // Thêm vào csdl
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        _BankGateAPI.TotalAmount = TotalAmount(_BankGateAPI.Amount, bankName);
        _BankGateAPI.LogContent = serializer.Serialize(_BankGateAPI);
        _BankGateAPI = _BankGateAPI.Add();

        // Nếu csdl lỗi
        if (_BankGateAPI.ReturnValue < 0)
        {
            url = BankGateUtils.UrlResponse(_BankGateAPI.ReturnUrl, _BankGateAPI.OrderNo, _BankGateAPI.ReturnValue.ToString(), "0", _Partner.SignatureType, _Partner.PrivateKey);
            NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "Response", url });
            Response.Redirect(url);
        }

        Libs.BankGate.VTCPay.VTCPay _VTCPay = new Libs.BankGate.VTCPay.VTCPay();
        switch (bankName)
        {
            case "visa": // Visa
            case "master": // Visa
            case "jcb":
                _VTCPay.TransactionID = _BankGateAPI.TransactionID;

                // Thêm giao dịch
                _VTCPay.Add();

                // Lỗi csdl
                if (_VTCPay.ReturnValue < 0)
                {
                    url = BankGateUtils.UrlResponse(_BankGateAPI.ReturnUrl, _BankGateAPI.OrderNo, Convert.ToString((int)ResponseCode.SystemError), "0", _Partner.SignatureType, _Partner.PrivateKey);
                    NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "VTCPay", "Response", url });
                    Response.Redirect(url);
                }

                // Cập nhật loại giao dịch, phân biệt VTC Pay
                _BankGateAPI.ServiceID = 14; // Thẻ visa
                _BankGateAPI.UpdateService(_BankGateAPI.TransactionID, _BankGateAPI.ServiceID);

                url = _VTCPay.GetUrlCheckOut(_BankGateAPI, "PaymentType:Visa");
                NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "VTCPay", "Request", url });
                Response.Redirect(url);
                break;

            case "vtcpay": // VTC Pay
                _VTCPay.TransactionID = _BankGateAPI.TransactionID;

                // Thêm giao dịch
                _VTCPay.Add();

                // Lỗi csdl
                if (_VTCPay.ReturnValue < 0)
                {
                    url = BankGateUtils.UrlResponse(_BankGateAPI.ReturnUrl, _BankGateAPI.OrderNo, Convert.ToString((int)ResponseCode.SystemError), "0", _Partner.SignatureType, _Partner.PrivateKey);
                    NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "VTCPay", "Response", url });
                    Response.Redirect(url);
                }
                
                url = _VTCPay.GetUrlCheckOut(_BankGateAPI);
                NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "VTCPay", "Request", url });
                Response.Redirect(url);
                break;

            case "baokim":
                Libs.BankGate.BaoKim.BaoKim _BaoKim = new Libs.BankGate.BaoKim.BaoKim();
                _BaoKim.TransactionID = _BankGateAPI.TransactionID;

                // Thêm giao dịch
                _BaoKim.Add();

                // Lỗi csdl
                if (_BaoKim.ReturnValue < 0)
                {
                    url = BankGateUtils.UrlResponse(_BankGateAPI.ReturnUrl, _BankGateAPI.OrderNo, Convert.ToString((int)ResponseCode.SystemError), "0", _Partner.SignatureType, _Partner.PrivateKey);
                    NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "BaoKim", "Response", url });
                    Response.Redirect(url);
                }
                
                url = _BaoKim.GetUrlCheckOut(_BankGateAPI);
                NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "BaoKim", "Request", url });
                Response.Redirect(url);
                break;

            case "sohapay":
                //return;
                Libs.BankGate.SohaPay.SohaPay _SohaPay = new Libs.BankGate.SohaPay.SohaPay();
                _SohaPay.TransactionID = _BankGateAPI.TransactionID;
                _SohaPay.Order_email = _SohaPay.TransactionID.ToString() + "@vgg.vn";
                _SohaPay.Order_email = "thanhtoan@vgg.vn";

                // Thêm giao dịch
                _SohaPay.Add();

                // Lỗi csdl
                if (_SohaPay.ReturnValue < 0)
                {
                    url = BankGateUtils.UrlResponse(_BankGateAPI.ReturnUrl, _BankGateAPI.OrderNo, Convert.ToString((int)ResponseCode.SystemError), "0", _Partner.SignatureType, _Partner.PrivateKey);
                    NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "SohaPay", "Response", url });
                    
                    Response.Redirect(url);
                }
                
                url = _SohaPay.GetUrlCheckOut(_BankGateAPI);
                NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "SohaPay", "Request", url });
                Response.Redirect(url);
                break;
            case "nganluong":
                Libs.BankGate.NganLuong.NganLuong _NganLuong = new Libs.BankGate.NganLuong.NganLuong(_BankGateAPI.TransactionID);

                // Thêm giao dịch
                _NganLuong.Add();

                // Lỗi csdl
                if (_NganLuong.ReturnValue < 0)
                {
                    url = BankGateUtils.UrlResponse(_BankGateAPI.ReturnUrl, _BankGateAPI.OrderNo, Convert.ToString((int)ResponseCode.SystemError), "0", _Partner.SignatureType, _Partner.PrivateKey);
                    NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "NganLuong", "Response", url });
                    Response.Redirect(url);
                }

                _NganLuong = _NganLuong.GetUrlCheckout(_NganLuong, _BankGateAPI);
                _NganLuong.Update();

                if (_NganLuong.Error_code == "00")
                {
                    url = _NganLuong.Checkout_url;
                    NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "NganLuong", "Request", url });
                    Response.Redirect(url);
                }
                else
                {
                    _BankGateAPI.Status = (int)ResponseCode.TransactionFailed;
                    _BankGateAPI.UpdateStatus();

                    url = BankGateUtils.UrlResponse(_BankGateAPI.ReturnUrl, _BankGateAPI.OrderNo, Convert.ToString((int)ResponseCode.TransactionFailed), "0", _Partner.SignatureType, _Partner.PrivateKey);
                    NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "NganLuong", "Response", url });
                    Response.Redirect(url);
                }
                break;

            case "vcb": //vietcombank
            case "tcb": //techcombank
            case "vib": //vib bank
            case "abb": //abbank
            case "stb": //sacombank
            case "msb": //maritime bank
            case "nvb": //navibank
            case "ctg": //vietinbank
            case "dab": //dongabank
            case "hdb": //hdbank
            case "vab": //vietabank
            case "vpb": //vpbank
            case "acb": //acb
            case "mb": //mbbank 
            case "gpb": //gpbank
            case "eib": //eximbank
            case "ojb": //oceanbank
            case "nasb": //bacabank
            case "ocb": //oricombank
            case "tpb": //tpbank
            case "lpb": //lienvietpostbank
            case "seab": //seabank
            case "bidv": //bidv
            case "varb": //agribank
            case "bvb": //baovietbank
            case "shb": //shb
            case "klb": //kienlongbank
            case "scb": //scb

                // Smartlink
                _BankGateAPI.ServiceID = 11;
                _BankGateAPI.UpdateService(_BankGateAPI.TransactionID, _BankGateAPI.ServiceID);

                Libs.BankGate.Smartlink.Smartlink _Smartlink = new Libs.BankGate.Smartlink.Smartlink();
                url = _Smartlink.GetUrlCheckOut(_BankGateAPI);
                NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "Smartlink", "Resquest", url });
                Response.Redirect(url);

                //// Ngân Lượng
                //_NganLuong = new Libs.BankGate.NganLuong.NganLuong(_BankGateAPI.TransactionID);
                //_NganLuong.Bank_code = _NganLuong.NganLuongBankCode(bankName);
                //_NganLuong.Payment_method = "ATM_ONLINE";

                //// Thêm giao dịch
                //_NganLuong.Add();

                //// Lỗi csdl
                //if (_NganLuong.ReturnValue < 0)
                //{
                //    url = BankGateUtils.UrlResponse(_BankGateAPI.ReturnUrl, _BankGateAPI.OrderNo, Convert.ToString((int)ResponseCode.SystemError), "0", _Partner.SignatureType, _Partner.PrivateKey);
                //    NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "NganLuong", "Response", url });
                //    Response.Redirect(url);
                //}

                //_NganLuong = _NganLuong.GetUrlCheckout(_NganLuong, _BankGateAPI);
                //_NganLuong.Update();

                //if (_NganLuong.Error_code == "00")
                //{
                //    Response.Redirect(_NganLuong.Checkout_url);
                //}
                //else
                //{
                //    _BankGateAPI.Status = (int)ResponseCode.TransactionFailed;
                //    _BankGateAPI.UpdateStatus();

                //    url = BankGateUtils.UrlResponse(_BankGateAPI.ReturnUrl, _BankGateAPI.OrderNo, Convert.ToString((int)ResponseCode.TransactionFailed), "0", _Partner.SignatureType, _Partner.PrivateKey);
                //    NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "NganLuong", "Response", url });
                //    Response.Redirect(url);
                //}

                break;
            case "970488":  // BIDV
            case "161087":  // Sài Gòn Công thương
            case "970428":  // Nam á
            case "970489":  // Vietinbank
            case "970499":  // Agribank
            case "970459":  // Anbinh Bank
            case "970430":  // Xăng dầu Petrolimex
            case "970408":  // Dầu khí Toàn cầu
            case "970468":  // SeaBank
            case "970443":  // SHB
            // 2014-07-22
            case "970403":  // SacomBank
            case "970414":  // OceanBank
            case "970436":  // Vietcombank
            case "970441":  // VIB - Ngân Hàng Quốc Tế
            case "970422":  // MB - Ngân hàng TMCP Quân Đội
            case "970407":  // TechComBank
            case "970426":  // MaritimeBank
            case "970432":  // VPBank
            case "970423":  // TienPhongBank
            case "970409":  // BacA Bank
            case "970427":  // VietABank
                _BankGateAPI.ServiceID = 13;
                _BankGateAPI.UpdateService(_BankGateAPI.TransactionID, _BankGateAPI.ServiceID);

                Libs.BankGate.Banknet.BankNet _BankNet = new Libs.BankGate.Banknet.BankNet();
                url = _BankNet.GetUrlCheckOut(_BankGateAPI, bankName);
                NLogLogger.Info(new string[] { "BankGate", _BankGateAPI.TransactionID.ToString(), "BankNet", "Resquest", url });

                if (url.Length > 0)
                {
                    Response.Redirect(url);
                }
                else
                {
                    Response.Redirect(Constant.HOME_ROOT + Resources.Url.Message + "?m=Có lỗi trong quá trình thanh toán, bạn vui lòng thực hiện lại!");
                }
                break;
            default:
                AppUtils.Alert(Page, "Bạn chưa chọn hình thức thanh toán!");
                return;
        }
    }

    protected int TotalAmount(int amount, string bank)
    {
        long bigAmount = amount;
        switch (bank.ToLower())
        {
            case "visa":
                //return Convert.ToInt32(bigAmount * 1028 / 1000 + 5500);
                return amount;
            case "master":
                //return Convert.ToInt32(bigAmount * 1028 / 1000 + 5500);
                return amount;
            case "jcb":
                //return Convert.ToInt32(bigAmount * 1028 / 1000 + 5500);
                return amount;
            case "vtcpay":
                //return amount;
                return Convert.ToInt32(bigAmount * 101 / 100);
            case "vtcpay1":
                //return amount;
                return Convert.ToInt32(bigAmount * 101 / 100);
            case "baokim":
                return Convert.ToInt32(bigAmount * 101 / 100);
            case "nganluong":
                return Convert.ToInt32(bigAmount * 101 / 100 + 500);
            case "sohapay":
                return Convert.ToInt32(bigAmount * 103 / 100);
            case "smartlink":
                return Convert.ToInt32(bigAmount * 1011 / 1000 + 1760);
            default:
                return Convert.ToInt32(bigAmount * 1011 / 1000 + 1760);
        }
    }

    protected string GetTotalAmount(string bank)
    {
        BankGateAPI _BankGateAPI = (BankGateAPI)Session["BankGateAPI"];

        return TotalAmount(_BankGateAPI.Amount, bank).ToString("#,#").Replace(",", ".");
    }

}