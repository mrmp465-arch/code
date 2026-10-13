using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;

public partial class Pages_Monitor_BuyCard_FixStatus : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BuyCardFixStatus);
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        BuyCard _BuyCard = new BuyCard();
        _BuyCard.TransactionID = AppUtils.ToInt64(txtTransactionID.Text);

        _BuyCard = _BuyCard.Get();

        if (_BuyCard == null)
        {
            txtCardSerial.Text = "";
            txtCardCode.Text = "";
            txtStatus.Text = "";
            txtAmount.Text = "";
            AlertInfos.Text = "Không tồn tại giao dịch";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
        }
        else
        {
            //txtCardSerial.Text = _BuyCard.CardSerial;
            //txtCardCode.Text = _BuyCard.CardCode;
            txtStatus.Text = _BuyCard.Status.ToString();
            txtAmount.Text = _BuyCard.Amount.ToString();
        }
    }

    protected void btUpdate_Click(object sender, EventArgs e)
    {
        BuyCard _BuyCard = new BuyCard();
        _BuyCard.TransactionID = AppUtils.ToInt64(txtTransactionID.Text);

        _BuyCard = _BuyCard.Get();
        if (_BuyCard == null)
        {
            txtCardSerial.Text = "";
            txtCardCode.Text = "";
            txtStatus.Text = "";
            txtAmount.Text = "";
            AlertInfos.Text = "Không tồn tại giao dịch";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
        }
        else
        {
            _BuyCard.Status = AppUtils.ToInt32(txtStatus.Text);
            _BuyCard.Amount = AppUtils.ToInt64(txtAmount.Text);
            _BuyCard.Update();
            AlertSuccesss.Text = "Cập nhập thành công";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertSuccess').modal()}); ", true);
        }

    }
}