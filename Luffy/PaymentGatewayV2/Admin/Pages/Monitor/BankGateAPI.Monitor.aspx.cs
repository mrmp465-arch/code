using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using System.Globalization;
using Libs.API;

public partial class Pages_Monitor_BankGateAPI_Monitor : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankGateAPIMonitor);


        if (!IsPostBack)
        {
            init();
            GetList();
        }
    }

    private void init()
    {

        var lst = new List<Partners>();
        if (AppUtils.IsAdmin)
            lst = new Partners().GetList();
        else
            lst = new Partners().GetListByUserId(AppUtils.UserID);

        lst = lst.OrderBy(x => x.PartnerCode).ToList();
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "PartnerCode";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Đối tác:", ""));

        var lstProvider = new List<Providers>();
        if (AppUtils.IsAdmin)
            lstProvider = new Providers().GetList(13);
        else
            lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 13).ToList();

        drpProvider.DataSource = lstProvider;
        drpProvider.DataTextField = "Name";
        drpProvider.DataValueField = "ProviderCode";
        drpProvider.DataBind();
        drpProvider.Items.Insert(0, new ListItem("Nhà cung cấp:", ""));

        drpBankCode.DataSource = new BankGateAPI().GetBankCode();
        drpBankCode.DataTextField = "BankCode";
        drpBankCode.DataValueField = "BankCode";
        drpBankCode.DataBind();
        drpBankCode.Items.Insert(0, new ListItem("BankCode:", ""));

        txtCreatTime.Text = DateTime.Now.ToString("MM/dd/yyyy");
    }

    private void GetList()
    {
        int top = Convert.ToInt32(drpTop.SelectedValue);
       
        DateTime requestTime = AppUtils.DateTimeParseExact(txtCreatTime.Text);
        requestTime = requestTime.AddDays(1);

        bool erro = false;
        int? status = null;
        if (txtStatus.Text.ToLower() != "all")
            status = AppUtils.ToInt32(txtStatus.Text, out erro);
        if (erro)
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfos').html('Chưa nhập Status'); $('#AlertInfo').modal()}); ", true);
            return;
        }

        string partnerCodes = drpPartner.SelectedValue;
        string providerCodes = drpProvider.SelectedValue;


        if (AppUtils.IsPartner && !AppUtils.IsAdmin)
        {

            if (string.IsNullOrEmpty(partnerCodes))
            {
                var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
                if (lstPartner != null && lstPartner.Count > 0)
                {
                    partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode).ToArray());
                }
            }


        }

        if (AppUtils.IsProvider && !AppUtils.IsAdmin)
        {
            if (string.IsNullOrEmpty(providerCodes))
            {
                var lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 13).ToList();
                if (lstProvider != null && lstProvider.Count > 0)
                {
                    providerCodes = string.Join(",", lstProvider.Select(e => e.ProviderCode).ToArray());
                }
            }

        }
        string bankCode = drpBankCode.SelectedValue;
        BankGateAPI _BankGateAPI = new BankGateAPI();
        rptList.DataSource = _BankGateAPI.GetTable(top, partnerCodes, providerCodes, requestTime, status.GetValueOrDefault(), bankCode);
        rptList.DataBind();
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }
   
}