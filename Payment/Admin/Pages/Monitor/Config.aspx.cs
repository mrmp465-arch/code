using Libs.API;
using Libs.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_Monitor_Config : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.SystemConfig);
        if (!IsPostBack)
        {
            init();
        }
    }
    private void init()
    {
        var lstconfig = new SystemDataConfig().GetListCache();
        txtMaxBankCashAmount.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("MaxBankCashAmount")).DataValue;
        txtMaxMomoCashAmount.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("MaxMomoCashAmount")).DataValue;
        txtMinBankCashAmount.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("MinBankCashAmount")).DataValue;
        txtMinMomoCashAmount.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("MinMomoCashAmount")).DataValue;
        txtMomoLimitAmoutCash.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("MomoLimitAmoutCash")).DataValue;
        txtBlockAccount.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("BlockAccount")).DataValue;


        txtMinBankAproveAmount.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("MinBankAproveAmount")).DataValue;
        txtBankCodeInMaintain.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("BankCodeInMaintain")).DataValue;
        txtBankPrefix.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("BankPrefix")).DataValue;

        txtBankCodeVS.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("BankCodeVS8")).DataValue;

        ddlBankCashEnable.Checked = Convert.ToBoolean(int.Parse(lstconfig.FirstOrDefault(x => x.DataKey.Equals("BankCashEnable")).DataValue));
        ddlMomoCashEnable.Checked = Convert.ToBoolean(int.Parse(lstconfig.FirstOrDefault(x => x.DataKey.Equals("MomoCashEnable")).DataValue));
        ddlBankInEnable.Checked = Convert.ToBoolean(int.Parse(lstconfig.FirstOrDefault(x => x.DataKey.Equals("BankInEnable")).DataValue));
        ddlMomoInEnable.Checked = Convert.ToBoolean(int.Parse(lstconfig.FirstOrDefault(x => x.DataKey.Equals("MomoInEnable")).DataValue));
        ddlBankApp.Checked = Convert.ToBoolean(int.Parse(lstconfig.FirstOrDefault(x => x.DataKey.Equals("BankOutApp")).DataValue));
        rptList.DataSource = new BankCodeTranfer().GetListCache().ToList();

        rptList.DataBind();
    }
    protected void btAdd_Click(object sender, EventArgs e)
    {

        var systemDataConfig = new SystemDataConfig();

        systemDataConfig.Update("BankCashEnable", Convert.ToInt32(ddlBankCashEnable.Checked).ToString());
        systemDataConfig.Update("BankInEnable", Convert.ToInt32(ddlBankInEnable.Checked).ToString());
        systemDataConfig.Update("MomoCashEnable", Convert.ToInt32(ddlMomoCashEnable.Checked).ToString());
        systemDataConfig.Update("MomoInEnable", Convert.ToInt32(ddlMomoInEnable.Checked).ToString());
        systemDataConfig.Update("BankOutApp", Convert.ToInt32(ddlBankApp.Checked).ToString());
        systemDataConfig.Update("BankCodeInMaintain", txtBankCodeInMaintain.Text);
        systemDataConfig.Update("BankCodeVS8", txtBankCodeVS.Text);
        systemDataConfig.Update("BankPrefix", txtBankPrefix.Text);

        systemDataConfig.Update("BlockAccount", txtBlockAccount.Text);
        systemDataConfig.DeleteCache();
        AlertSuccesss.Text = "Cập nhật thành công";
        var _userLog = new UserLog
        {
            UserName = AppUtils.UserName,
            Action = "systemconfig",
            ActionName = "Cập nhật cấu hình",
            Description = "Cập nhật cấu hình"
        };
        _userLog.Add();

        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);

    }
    protected void btAdd_Click2(object sender, EventArgs e)
    {
        var systemDataConfig = new SystemDataConfig();

        systemDataConfig.Update("MaxBankCashAmount", txtMaxBankCashAmount.Text);
        systemDataConfig.Update("MaxMomoCashAmount", txtMaxMomoCashAmount.Text);
        systemDataConfig.Update("MinBankAproveAmount", txtMinBankAproveAmount.Text);
        systemDataConfig.Update("MaxBankCashAmount", txtMaxBankCashAmount.Text);
        systemDataConfig.Update("MinMomoCashAmount", txtMinMomoCashAmount.Text);
        systemDataConfig.Update("MinBankCashAmount", txtMinBankCashAmount.Text);
        systemDataConfig.Update("MomoLimitAmoutCash", txtMomoLimitAmoutCash.Text);
        systemDataConfig.DeleteCache();

        var _userLog = new UserLog
        {
            UserName = AppUtils.UserName,
            Action = "systemconfig",
            ActionName = "Cập nhật cấu hình",
            Description = "Cập nhật cấu hình"
        };
        _userLog.Add();
        AlertSuccesss.Text = "Cập nhật thành công";

        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);

    }
    protected void btAdd_Click3(object sender, EventArgs e)
    {
        var bankCodeTranfer = new BankCodeTranfer();
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
            Label labelUserID = (Label)rptList.Items[i].FindControl("lblId");

            bankCodeTranfer.Update(Convert.ToInt32(labelUserID.Text), cbx.Checked ? 1 : 0);
           
        }
        var _userLog = new UserLog
        {
            UserName = AppUtils.UserName,
            Action = "systemconfig",
            ActionName = "Cập nhật cấu hình",
            Description = "Cập nhật cấu hình"
        };
        _userLog.Add();
        bankCodeTranfer.DeleteCache();
        AlertSuccesss.Text = "Cập nhật thành công";

        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);


    }
}