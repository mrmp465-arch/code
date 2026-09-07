using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using Libs.Utils;

public partial class Pages_Monitor_BankGateAPI_Report_Profit : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.CardAPIReport);

        if (!IsPostBack)
        {
            init();
            //GetList();
        }
    }

    private void init()
    {



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

        //drpBankCode.DataSource = new BankGateAPI().GetBankCode();
        //drpBankCode.DataTextField = "BankCode";
        //drpBankCode.DataValueField = "BankCode";
        //drpBankCode.DataBind();
        //drpBankCode.Items.Insert(1, new ListItem("Momo", "momo"));
        //drpBankCode.Items.Insert(2, new ListItem("BankTranfer", "banktranfer"));


        var lst = new List<Partners>();
        if (AppUtils.IsAdmin)
            lst = new Partners().GetList();//.Where(e => e.Status == 1).ToList();
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
            lstProvider = new Providers().GetList(13);//.Where(e => e.Status == 1).ToList();
        else
            lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 13).ToList();

        drpProvider.DataSource = lstProvider;
        drpProvider.DataTextField = "Name";
        drpProvider.DataValueField = "ProviderCode";
        drpProvider.DataBind();
        drpProvider.Items.Insert(0, new ListItem("Nhà cung cấp:", ""));

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
                var lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 7).ToList();
                if (lstProvider != null && lstProvider.Count > 0)
                {
                    providerCodes = string.Join(",", lstProvider.Select(e => e.ProviderCode).ToArray());
                }
            }

        }

        string bankCode = string.Empty; //drpBankCode.SelectedValue;
        int year = Convert.ToInt32(drpYear.SelectedValue);
        int month = Convert.ToInt32(drpMonth.SelectedValue);
        int day = Convert.ToInt32(drpDay.SelectedValue);
        int totalTransaction = 0;
        long totalAmount = 0;
        long totalProfit = 0;

        if (year == 0)
        {
            month = 0;
            day = 0;
        }

        if (month == 0) day = 0;

        BankGateAPI _bankGateApiAPILog = new BankGateAPI();
        NLogLogger.Info(new string[] { partnerCodes, bankCode, providerCodes });
        rptList.DataSource = _bankGateApiAPILog.ReportProfit(partnerCodes, bankCode, providerCodes, year, month, day, ref totalTransaction, ref totalAmount, ref totalProfit);
        lblTotalTransaction.Text = totalTransaction.ToString();
        lblTotalAmount.Text = totalAmount.ToString("#,#").Replace(",", ".");
        lblTotalProfit.Text = totalProfit.ToString("#,#").Replace(",", ".");
        txtTotalFit.Text = AppUtils.AmountToPercentFit(totalAmount.ToString(), totalProfit.ToString());
        rptList.DataBind();
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }

    //protected void btCardType_Click(object sender, EventArgs e)
    //{
    //    Response.Redirect(Constant.ADMIN_PATH + Resources.Url.CardAPIReportCardType);
    //}

    //protected void btProvider_Click(object sender, EventArgs e)
    //{
    //    Response.Redirect(Constant.ADMIN_PATH + Resources.Url.CardAPIReportProvider);
    //}
}