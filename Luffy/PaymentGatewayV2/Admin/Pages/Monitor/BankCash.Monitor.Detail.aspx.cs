using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;
public partial class Pages_Monitor_BankCash_Monitor_Detail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankCashMonitorDetail);
        if (!IsPostBack)
        {
            BindData();
        }
    }

    private void BindData()
    {
        Libs.Report.BankCashAPI _BankCash = new Libs.Report.BankCashAPI();
        _BankCash.TransactionID = Convert.ToInt64(Request["id"]);

        _BankCash = _BankCash.Get();

        lblTransactionID.Text = _BankCash.TransactionID.ToString();
        lblAccountName.Text = _BankCash.FullName;

        //lblAmount.Text = _BankCash.Amount.ToString();
        lblTotalAmount.Text = _BankCash.Amount.ToString();

        lblProvider.Text = _BankCash.ProviderCode;
        lblCreatTime.Text = _BankCash.CreatedTime.ToString();
        lblLastTime.Text = _BankCash.LastTime.ToString();
        lblRefCode.Text = _BankCash.RefCode;
        lblBankCode.Text = _BankCash.BankCode;
        lblOrderNo.Text = _BankCash.OrderNo;
        //lblOrderInfo.Text = _BankCash.OrderInfo;
        //lblMobile.Text = _BankCash.Mobile;
        //lblEmail.Text = _BankCash.Email;
        lblBankAccountName.Text = _BankCash.BankAccountName;
        lblBankAccountNumber.Text = _BankCash.BankAccountNumber;
        lblCallbackUrl.Text = _BankCash.ReturnUrl;
        lblStatus.Text = _BankCash.Status + " (" + ResponseUtils.Description(_BankCash.Status) + ")";
        txtLog.Text = _BankCash.LogContent;
    }
    protected void txtRecheck_Click(object sender, EventArgs e)
    {

        //var handler = BankGateV2Factory.GetHandler(lblProvider.Text);
        //var result = handler.ReCheck(lblTransactionID.Text);
        //lblRecheck.Text = result.Description;

    }
}