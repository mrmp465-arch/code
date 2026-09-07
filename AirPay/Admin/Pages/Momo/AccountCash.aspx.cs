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
public partial class Pages_Momo_AccountCash : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.MomoPartner);

        if (!IsPostBack)
        {
            //init();
            BindData();
        }
    }

    protected void btAdd_Click(object sender, EventArgs e)
    {
        var obj = new BankAccountCash();
        obj.BankCode = drpBankCode.SelectedValue;
        obj.AccountName = txtAccountName.Text;
        obj.AccountNumber = txtAccountNumber.Text;
        obj.Status = Convert.ToInt32(chkIsActive.Checked);
        new BankAccountCash().Add(obj);
        System.Threading.Thread.Sleep(200);
        Response.Redirect(Request.RawUrl);
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
    }
    private void BindData()
    {
        var data = new BankAccountCash().GetLis();

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
            var _Partner = new BankAccountCash();
            _Partner.Id = Convert.ToInt32(lbPId.Text);
            _Partner.Status = cbx.Checked ? 1 : 0;
            
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //NLogLogger.Info(new string[] { "Data", "lbPId", serializer.Serialize(_PartnerMomo) });
            new BankAccountCash().Update(_Partner);
        }
        BindData();
    }

    protected void Delete_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        int Id = Convert.ToInt32(e.CommandArgument.ToString());

        //var order = new PartnerMomo { Id = Id };
        new BankAccountCash().Delete(Id);
        BindData();
    }
    protected void drpPartner_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindData();
    }
}