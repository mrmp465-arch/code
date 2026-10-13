using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using System.Globalization;
using Libs.API;

public partial class Pages_Monitor_SMS_Monitor : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.CardAPIMonitor);

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
        drpPartner.DataValueField = "PartnerCode";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Đối tác:", ""));
        if (AppUtils.IsAdmin)
        {
            var _Provider = new Providers();
            drpProvider.DataSource = _Provider.GetList(4);
            drpProvider.DataBind();
            drpProvider.DataTextField = "Name";
            drpProvider.DataValueField = "ProviderCode";
            drpProvider.DataBind();
            drpProvider.Items.Insert(0, new ListItem("Nhà cung cấp:", ""));
        }
    }

    private void GetList()
    {
        string PartnerCodes = drpPartner.SelectedValue;
        if (string.IsNullOrEmpty(PartnerCodes) && !AppUtils.IsAdmin)
        {
            List<Partners> lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
            if (lstPartner != null && lstPartner.Count > 0)
                PartnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode.ToString()).ToArray());
            else
                PartnerCodes = "-1";
        }  
        int top = Convert.ToInt32(drpTop.SelectedValue);
        DateTime creatTime = AppUtils.ToDateTime(txtCreatTime.Text);

        bool erro = false;
        int? status = null;
        if (txtStatus.Text.ToLower() != "all")
            status = AppUtils.ToInt32(txtStatus.Text, out erro);
        if (erro)
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfos').html('Chưa nhập Status'); $('#AlertInfo').modal()}); ", true);
            return;
        }
        string provider = drpProvider.SelectedValue;

        MessageIn _MessageIn = new MessageIn();
        rptList.DataSource = _MessageIn.GetTable(top, PartnerCodes, creatTime, status, "", provider);
        rptList.DataBind();
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }

    protected string DetailUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.SMSMonitorDetail + "?id=" + id;
    }
}