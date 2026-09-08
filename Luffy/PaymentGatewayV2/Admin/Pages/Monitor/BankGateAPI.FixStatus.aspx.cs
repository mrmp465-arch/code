using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;

public partial class Pages_Monitor_BankGateAPI_FixStatus : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankGateAPIFixStatus);

        if (!IsPostBack)
        {
            var lst = new List<Partners>();
            if (AppUtils.IsAdmin)
                lst = new Partners().GetList();
            else
                lst = new Partners().GetListByUserId(AppUtils.UserID);
            drpPartner.DataSource = lst;
            drpPartner.DataTextField = "Name";
            drpPartner.DataValueField = "PartnerID";
            drpPartner.DataBind();
            drpPartner.Items.Insert(0, new ListItem("Đối tác:", "-1"));
        }
        
    }
    protected void GetOrder()
    {
        BankGateAPI _BankGateAPI = new BankGateAPI();
        _BankGateAPI.TransactionID = AppUtils.ToInt64(txtTransactionID.Text);

        _BankGateAPI = _BankGateAPI.Get();
        if (_BankGateAPI == null)
        {
            txtStatus.Text = "";
            txtAmount.Text = "";
            txtLastTime.Text = "";
            txtRefCode.Text = "";
            txtBankAccountName.Text = "";
            txtBankAccountNumber.Text = "";
            AlertInfos.Text = "Không tồn tại giao dịch";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
        }
        else
        {
            txtStatus.Text = _BankGateAPI.Status.ToString();
            txtAmount.Text = _BankGateAPI.TotalAmount.ToString();
            txtLastTime.Text = _BankGateAPI.LastTime.ToString();
            txtOrderInfo.Text = _BankGateAPI.OrderInfo.ToString();
            txtRefCode.Text = _BankGateAPI.RefCode.ToString();
            txtBankAccountName.Text = _BankGateAPI.BankAccountName.ToString();
            txtBankAccountNumber.Text = _BankGateAPI.BankAccountNumber.ToString();
            drpPartner.SelectedValue = _BankGateAPI.PartnerID.ToString();
        }
    }
    protected void getOrderNo(string OrderNo)
    {
        //BankGateAPI _BankGateAPI = new BankGateAPI();
        //_BankGateAPI.TransactionID = AppUtils.ToInt64(txtTransactionID.Text);

        //_BankGateAPI = _BankGateAPI.Get();

      

        //if (_BankGateAPI == null)
        //{
        //    txtStatus.Text = "";
        //    txtAmount.Text = "";
        //    txtLastTime.Text = "";
        //    txtRefCode.Text ="";
        //    txtBankAccountName.Text = "";
        //    txtBankAccountNumber.Text = "";
        //    AlertInfos.Text = "Không tồn tại giao dịch";
        //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
        //}
        //else
        //{
        //    txtStatus.Text = _BankGateAPI.Status.ToString();
        //    txtAmount.Text = _BankGateAPI.TotalAmount.ToString();
        //    txtLastTime.Text = _BankGateAPI.LastTime.ToString();
        //    txtOrderInfo.Text = _BankGateAPI.OrderInfo.ToString();
        //    txtRefCode.Text = _BankGateAPI.RefCode.ToString();
        //    txtBankAccountName.Text = _BankGateAPI.BankAccountName.ToString();
        //    txtBankAccountNumber.Text = _BankGateAPI.BankAccountNumber.ToString();
        //    drpPartner.SelectedValue = _BankGateAPI.PartnerID.ToString();
        //}
    }

    protected void btUpdate_Click(object sender, EventArgs e)
    {
        BankGateAPI _BankGateAPI = new BankGateAPI();
        _BankGateAPI.TransactionID = AppUtils.ToInt64(txtTransactionID.Text);

        _BankGateAPI = _BankGateAPI.Get();
        if (_BankGateAPI == null)
        {
            txtStatus.Text = "";
            txtAmount.Text = "";
            txtOrderInfo.Text = "";
            txtRefCode.Text = "";
            txtRefCode.Text = "";
            txtBankAccountName.Text = "";
            txtBankAccountNumber.Text = "";
            AlertInfos.Text = "Không tồn tại giao dịch";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
        }
        else
        {
            _BankGateAPI.Status = AppUtils.ToInt32(txtStatus.Text);
            _BankGateAPI.TotalAmount = Convert.ToDecimal(txtAmount.Text);
            _BankGateAPI.LastTime = AppUtils.ToDateTime(txtLastTime.Text);
            _BankGateAPI.LogContent = txtLogContent.Text;
            _BankGateAPI.BankAccountName = txtBankAccountName.Text;
             _BankGateAPI.BankAccountNumber = txtBankAccountNumber.Text;
            _BankGateAPI.PartnerID = AppUtils.ToInt32(drpPartner.SelectedValue);
            if(_BankGateAPI.PartnerID>0)
            {
                _BankGateAPI.PartnerCode = new Partners().Get(_BankGateAPI.PartnerID).PartnerCode;
            }
            _BankGateAPI.UpdateStatusCheckStatus();
            AlertSuccesss.Text = "Cập nhập thành công";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertSuccess').modal()}); ", true);
        }

    }
}