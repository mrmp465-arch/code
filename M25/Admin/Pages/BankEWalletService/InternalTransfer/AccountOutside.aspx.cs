using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using System.Data;
using Libs.Utils;
using System.Web.Script.Serialization;
public partial class Pages_BankEWalletService_InternalTransfer_AccountOutside : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.ITAccountOutside);

        if (!IsPostBack)
        {
            //init();
            BindData();
        }
    }

    protected void btAdd_Click(object sender, EventArgs e)
    {
        var obj = new ITBank();
        obj.BankCode = drpBankCode.SelectedValue;
        obj.BankName = txtAccountName.Text;
        obj.BankId = txtAccountNumber.Text;
        obj.Status = Convert.ToInt32(chkIsActive.Checked);
        obj.Type = "OUTSIDE";
        obj.Add();
        System.Threading.Thread.Sleep(200);
        Response.Redirect(Request.RawUrl);
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
    }
    private void BindData()
    {
        var data = new ITBank().GetList().Where(x => x.Type == "OUTSIDE");

        // JavaScriptSerializer serializer = new JavaScriptSerializer();
        //NLogLogger.Info(new string[] { "Data", "PartnerMomoo", serializer.Serialize(data) });
        rptList.DataSource = data;
        rptList.DataBind();


    }

    protected void btApply_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");

            Label lbPId = (Label)rptList.Items[i].FindControl("lblId");



            //NLogLogger.Info(new string[] { "Data", "lbPId", lbPId.Text });
            var _Partner = new ITBank();
            _Partner.Id = Convert.ToInt32(lbPId.Text);
            _Partner.Status = cbx.Checked ? 1 : 0;

            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //NLogLogger.Info(new string[] { "Data", "lbPId", serializer.Serialize(_PartnerMomo) });
            new ITBank().UpdateStatus(_Partner.Id, _Partner.Status);
        }
        BindData();
    }

    protected void Delete_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        int Id = Convert.ToInt32(e.CommandArgument.ToString());

        //var order = new PartnerMomo { Id = Id };
        new ITBank().Delete(Id);
        BindData();
    }
}