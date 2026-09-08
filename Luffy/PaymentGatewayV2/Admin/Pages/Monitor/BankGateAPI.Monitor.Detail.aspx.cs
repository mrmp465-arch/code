using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;
public partial class Pages_Monitor_BankGateAPI_Monitor_Detail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankGateAPIMonitorDetail);
        if (!IsPostBack)
        {
            BindData();
        }
    }

    private void BindData()
    {
        Libs.Report.BankGateAPI _BankGateAPI = new Libs.Report.BankGateAPI();
        _BankGateAPI.TransactionID = Convert.ToInt64(Request["id"]);

        _BankGateAPI = _BankGateAPI.Get();

        lblTransactionID.Text = _BankGateAPI.TransactionID.ToString();
        lblAccountName.Text = _BankGateAPI.FullName;

        lblAmount.Text = Convert.ToInt64(_BankGateAPI.Amount).ToString("N0").Replace(",", ".");
        lblTotalAmount.Text = Convert.ToInt64(_BankGateAPI.TotalAmount).ToString("N0").Replace(",", ".");

        lblProvider.Text = _BankGateAPI.ProviderCode;
        lblCreatTime.Text = _BankGateAPI.CreatedTime.ToString();
        lblLastTime.Text = _BankGateAPI.LastTime.ToString();
        lblRefCode.Text = _BankGateAPI.RefCode;
        lblBankCode.Text = _BankGateAPI.BankCode;
        lblOrderNo.Text = _BankGateAPI.OrderNo;
        lblOrderInfo.Text = _BankGateAPI.OrderInfo;
        lblMobile.Text = _BankGateAPI.Mobile;
        lblEmail.Text = _BankGateAPI.Email;
        lblBankAccountName.Text = _BankGateAPI.BankAccountName;
        lblBankAccountNumber.Text = _BankGateAPI.BankAccountNumber;
        lblCallbackUrl.Text = _BankGateAPI.ReturnUrl;
        lblStatus.Text = _BankGateAPI.Status + " (" + ResponseUtils.Description(_BankGateAPI.Status) + ")";
        txtLog.Text = _BankGateAPI.LogContent;

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        var apiResponse = new DataCallback()
        {
            TransId = _BankGateAPI.RefCode,
            Content = _BankGateAPI.OrderNo,
            Amount = Convert.ToInt32(_BankGateAPI.Amount),
            Mobile="",
            BankCode= _BankGateAPI.BankCode,
        };
        var partner = new Partners().Get(_BankGateAPI.PartnerCode);
        apiResponse.Signature = PaymentUtils.Signature(apiResponse.TransId + apiResponse.Amount + apiResponse.Content, partner.PrivateKey, partner.SignatureType);
        // Task.Run(async () => await M32VTPBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(apiResponse)).ConfigureAwait(false));
        lblCallbackdata.Text = serializer.Serialize(apiResponse);
    }
    protected void txtRecheck_Click(object sender, EventArgs e)
    {

        //var handler = BankGateV2Factory.GetHandler(lblProvider.Text);
        //var result = handler.ReCheck(lblTransactionID.Text);
        //lblRecheck.Text = result.Description;

    }
    public class DataCallback
    {
        public string TransId { get; set; }
        public int Amount { get; set; }
        public string Content { get; set; }
        public string Signature { get; set; }
        public string BankCode { get; set; }
        public string Mobile { get; set; }
    }
}