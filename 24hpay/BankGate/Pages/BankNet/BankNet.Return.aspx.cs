using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Libs.Utils;
using Libs.BankGate;
using Libs.BankGate.Banknet;
using Libs.API;


public partial class Pages_BankNet_BankNet_Return : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {


        if (IsPostBack) return;
        int step = 0;
        
        BankGateAPI _BankGateAPI = new BankGateAPI();
        Partners _Partner = new Partners();
        string url = "";
        try
        {
            NLogLogger.Info(new string[] { "BankGate", "BankNet", "Response", Request.Url.ToString() });

            // step = 0, kiểm tra dữ liệu
            long transactionID = 0;
            int status = 0;
            long transactionTime = 0;
            string sign = "";

            transactionID = Convert.ToInt64(Request["transactionid"]);
            status = Convert.ToInt32(Request["status"]);
            transactionTime = Convert.ToInt64(Request["transactiontime"]);
            sign = Request["sign"];

            // step = 1, kiểm tra chữ ký, thời gian
            step = 1;
            BankNet _BankNet = new BankNet();
            
            if (!_BankNet.CheckUrlReturn(transactionID, status, transactionTime, sign))
            {
                NLogLogger.Info(new string[] { "BankGate", "BankNet", "Response", "Error", "Du lieu khong hop le" });
                Response.Redirect("/bankgate/message.aspx?error=-1", true);
            }

            // step = 2, kiểm tra thông tin giao dịch
            step = 2;
            _BankGateAPI = _BankGateAPI.Get(transactionID);

            // Kiểm tra thời gian
            if (_BankGateAPI.CreatedTime.AddDays(10) < DateTime.Now)
            {
                NLogLogger.Info(new string[] { "BankGate", "BankNet", _BankGateAPI.TransactionID.ToString(), "Error", "Du lieu qua han" });
                Response.Redirect("/bankgate/message.aspx?error=-1");
            }

            // Giao dịch không tồn tại, hoặc bị lặp
            if (_BankGateAPI == null || _BankGateAPI.Status != 0 && _BankGateAPI.Status != (int)ResponseCode.TransactionSuspicious)
            {
                NLogLogger.Info(new string[] { "BankGate", "BankNet", "Response", "Error", "TransactionID khong hop le" });
                Response.Redirect("/bankgate/message.aspx?error=-1", true);
            }

            // Lấy thông tin đối tác
            _Partner = _Partner.Get(_BankGateAPI.PartnerID);

            if (status == -1)   // step = 3, giao dịch không thành công 
            {
                step = 3;
                _BankGateAPI.Status = (int)ResponseCode.TransactionFailed;
            }
            else    // Giao dịch thành công
            {
                step = 4;

                // Xác nhận
                if (_BankNet.ConfirmTransaction(transactionID, true))
                {
                    _BankGateAPI.Status = 1;
                }
                else
                {
                    _BankGateAPI.Status = (int)ResponseCode.TransactionFailed;
                }
            }

            // Cập nhật trạng thái giao dịch
            step = 5;
            _BankGateAPI.LogContent = Request.Url.ToString();
            _BankGateAPI.UpdateStatus();
        }
        catch (Exception ex)
        {
            NLogLogger.Info(new string[] { "BankGate", "BankNet", "Response", "Error", step.ToString(), ex.Message.Replace("\n", " ") });

            switch (step)
            {
                case 4:
                    _BankGateAPI.Status = (int)ResponseCode.TransactionSuspicious;
                    _BankGateAPI.LogContent = Request.Url.ToString() + " - " + ex.Message;
                    break;

                case 0: // dữ liệu không hợp lệ
                case 1: // chữ ký, thời gian không hợp lệ
                case 2: // kiểm tra thông tin giao dịch
                case 3: // giao dịch không thành công
                default:
                    Response.Redirect("/bankgate/message.aspx?error=-1", false);
                    break;
            }
        }
        _BankGateAPI.UpdateStatus();
        url = BankGateUtils.UrlResponse(_BankGateAPI.ReturnUrl, _BankGateAPI.OrderNo, _BankGateAPI.Status.ToString(), _BankGateAPI.TransactionID.ToString(), "banknet", _BankGateAPI.TotalAmount.ToString(), _BankGateAPI.FullName, _BankGateAPI.Mobile, _Partner.SignatureType, _Partner.PrivateKey);
        NLogLogger.Info(new string[] { "BankGate", "BankNet", "Response", url });
        Response.Redirect(url);
    }
}