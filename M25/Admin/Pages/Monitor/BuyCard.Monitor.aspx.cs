using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;

public partial class Pages_Monitor_BuyCard_Monitor : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    { 
        AppUtils.CheckRoles(Resources.Url.BuyCardMonitor);

        if (!IsPostBack)
        { 
            init();
            GetList();
        }

    }

    private void init()
    {
        txtCreatTime.Text = DateTime.Now.AddDays(1).ToString(); 
        var lst = new Partners().GetList().Where(e => e.Status == 1).ToList();
        if (!AppUtils.IsAdmin)
            lst = new Partners().GetListByUserId(AppUtils.UserID);
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "PartnerID";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Đối tác:", ""));
        if (AppUtils.IsAdmin)
        {
            var _Provider1 = new Providers().GetList(15);
            var _Provider2 = new Providers().GetList(5);
            var allProvider = _Provider1.Concat(_Provider2).ToList();
            drpProvider.DataSource = allProvider;
            drpProvider.DataBind();
            drpProvider.DataTextField = "Name";
            drpProvider.DataValueField = "ProviderCode";
            drpProvider.DataBind();
            drpProvider.Items.Insert(0,new ListItem("Nhà cung cấp:", ""));  
        }
    }

    private void GetList()
    {
        string partnerIDs = drpPartner.SelectedValue;
        if (string.IsNullOrEmpty(partnerIDs) && !AppUtils.IsAdmin)
        {
            List<Partners> lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
            if (lstPartner != null && lstPartner.Count > 0)
                partnerIDs = string.Join(",", lstPartner.Select(e => e.PartnerID.ToString()).ToArray());
            else
                partnerIDs = "-1";
        }

        string provider = drpProvider.SelectedValue;

        int top = Convert.ToInt32(drpTop.SelectedValue);
        DateTime creatTime = AppUtils.ToDateTime(txtCreatTime.Text);

        bool erro = false;
        int? status = null;
        if (txtStatus.Text.ToLower() != "all")
            status = AppUtils.ToInt32(txtStatus.Text, out erro);
        if (erro) {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfos').html('Chưa nhập Status'); $('#AlertInfo').modal()}); ", true);
            return;
        } 

        BuyCard _BuyCard = new BuyCard();
        rptList.DataSource = _BuyCard.GetTable(top, partnerIDs, creatTime, status, "", provider);
        rptList.DataBind();
    }
    
    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }

    protected string DetailUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.BuyCardMonitorDetail + "?id=" + id;
    }
}