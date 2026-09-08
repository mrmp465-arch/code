using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;

public partial class Pages_Monitor_BankCash_Search : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankCashSearch);

        if (!IsPostBack)
        {
            init();
            //GetList();
        }
    }

    private void init()
    {
        var lst = new List<Partners>();
        if (AppUtils.IsAdmin)
            lst = new Partners().GetList();
        else
            lst = new Partners().GetListByUserId(AppUtils.UserID);
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "PartnerCode";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Đối tác:", ""));

        var lstProvider = new List<Providers>();
        if (AppUtils.IsAdmin)
            lstProvider = new Providers().GetList(18);
        else
            lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 18).ToList();

        drpProvider.DataSource = lstProvider;
        drpProvider.DataTextField = "Name";
        drpProvider.DataValueField = "ProviderCode";
        drpProvider.DataBind();
        drpProvider.Items.Insert(0, new ListItem("Nhà cung cấp:", ""));

        txtCreatTime.Text = DateTime.Now.ToString("MM/dd/yyyy");


        drpBankCode.DataSource = new BankCashAPI().GetBankCode();
        drpBankCode.DataTextField = "BankCode";
        drpBankCode.DataValueField = "BankCode";
        drpBankCode.DataBind();
        drpBankCode.Items.Insert(0, new ListItem("BankCode:", ""));
    }

    private void GetList()
    {
        BankCashAPI _BankCash = new BankCashAPI();
        //bool erro = false;
        //rptList.DataSource = _BankCash.Search(AppUtils.ToInt64(txtTransactionID.Text));
        //if (erro) {
        //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfos').html('Nhập TransactionID sai'); $('#AlertInfo').modal()}); ", true);
        //    return;
        //}
        //else
        //{
        //    rptList.DataBind();
        //    return;
        //} 
        long transId = AppUtils.ToInt64(txtTransactionID.Text);
        int top = Convert.ToInt32(drpTop.SelectedValue);
        string refCode = txtRefCode.Text.Trim();
       
        string orderNo = txtOrderNo.Text.Trim();
        string mobile = txtMobile.Text.Trim();
       
        string bankCode = drpBankCode.SelectedValue;
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
                var lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 18).ToList();
                if (lstProvider != null && lstProvider.Count > 0)
                {
                    providerCodes = string.Join(",", lstProvider.Select(e => e.ProviderCode).ToArray());
                }
            }

        }

        if (transId > 0)
        {
            rptList.DataSource = _BankCash.Search(transId);
        }
        else
        {
            if (txtCreatTime.Text.Trim() == "")
            {
                rptList.DataSource = _BankCash.Search(top, refCode, orderNo, mobile, bankCode, partnerCodes, providerCodes);
            }
            else
            {
                DateTime requestTime = AppUtils.ToDateTime(txtCreatTime.Text).AddDays(1);
                rptList.DataSource = _BankCash.Search(top, refCode, orderNo, mobile, bankCode, partnerCodes, providerCodes, requestTime);
            }
        }

        rptList.DataBind();
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }
    public string GetStatus(int status)
    {
        return status.ToString();
    }
}