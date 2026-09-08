using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;

public partial class Pages_Monitor_BankCash_Report : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankCashReport);

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

        drpBankCode.DataSource = new BankCashAPI().GetBankCode();
        drpBankCode.DataTextField = "BankCode";
        drpBankCode.DataValueField = "BankCode";
        drpBankCode.DataBind();
        drpBankCode.Items.Insert(0, new ListItem("BankCode:", ""));


        // Year
        drpYear.Items.Add(new ListItem("Năm:", "0"));
        for (int i = 2013; i <= DateTime.Now.Year; i++)
        {
            drpYear.Items.Add(new ListItem(i.ToString()));
        }

        // Month
        drpMonth.Items.Add(new ListItem("Tháng:", "0"));
        for (int i = 1; i <= 12; i++)
        {
            drpMonth.Items.Add(new ListItem(i.ToString()));
        }

        // Day
        drpDay.Items.Add(new ListItem("Ngày", "0"));
        for (int i = 1; i <= 31; i++)
        {
            drpDay.Items.Add(new ListItem(i.ToString()));
        }

        drpYear.SelectedValue = DateTime.Now.Year.ToString();
        drpMonth.SelectedValue = DateTime.Now.Month.ToString();
        drpDay.SelectedValue = DateTime.Now.Day.ToString();

    }

    private void GetList()
    {
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

        int year = Convert.ToInt32(drpYear.SelectedValue);
        int month = Convert.ToInt32(drpMonth.SelectedValue);
        int day = Convert.ToInt32(drpDay.SelectedValue);
        int totalTransaction = 0;
        long totalAmount = 0;

        if (year == 0)
        {
            month = 0;
            day = 0;
        }

        if (month == 0) day = 0;
        string bankCode = drpBankCode.SelectedValue;
        BankCashAPI _BankCash = new BankCashAPI();

        rptList.DataSource = _BankCash.Report(bankCode, partnerCodes, providerCodes, year, month, day, ref totalTransaction, ref totalAmount);
        lblTotalTransaction.Text = totalTransaction.ToString("#,#").Replace(",", ".");
        lblTotalAmount.Text = totalAmount.ToString("#,#").Replace(",", ".");
        rptList.DataBind();
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }

    protected void drpYear_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (drpYear.SelectedValue == "0")
        {
            drpMonth.SelectedValue = "0";
            drpDay.SelectedValue = "0";
        }
    }

    protected void drpMonth_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (drpYear.SelectedValue == "0")
        {
            drpDay.SelectedValue = "0";
        }
    }

    protected void btPartner_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.BankCashReportPartner);
    }

    protected void btService_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.BankCashReportService);
    }
}