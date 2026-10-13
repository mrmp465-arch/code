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

public partial class Pages_Momo_Partners : System.Web.UI.Page
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
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.MomoPartnerAdd);
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
    }
    private void BindData()
    {
        var data = new PartnerMomo().GetListPartner();
        if (!string.IsNullOrEmpty(txtPartnerCode.Text))
            data = data.Where(x => x.Code == txtPartnerCode.Text).ToList();

        if (!string.IsNullOrEmpty(txtName.Text))
            data = data.Where(x => x.Name.Contains(txtName.Text)).ToList();

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
            TextBox tbx = (TextBox)rptList.Items[i].FindControl("txtCallback");
            Label lbPId = (Label)rptList.Items[i].FindControl("lblId");



            //NLogLogger.Info(new string[] { "Data", "lbPId", lbPId.Text });
            var _Partner = new MPartner();
            _Partner.Id = Convert.ToInt32(lbPId.Text);
            _Partner.Status = cbx.Checked ? 1 : 0;
            _Partner.InCallbackUrl = tbx.Text;

            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //NLogLogger.Info(new string[] { "Data", "lbPId", serializer.Serialize(_PartnerMomo) });
            new PartnerMomo().UpdatePartner(_Partner);
        }
        BindData();
    }
  
    protected void Delete_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        int Id = Convert.ToInt32(e.CommandArgument.ToString());

        //var order = new PartnerMomo { Id = Id };
        new PartnerMomo().DeletePartner(Id);
        BindData();
    }
    protected void drpPartner_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindData();
    }
}