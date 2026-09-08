using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using System.Globalization;
using Libs.API;
using Libs.Utils;


public partial class Pages_Monitor_BankCash_Report_DoiSoat : System.Web.UI.Page
{
    public List<BankCashAPI> lstReportDoiSoat1 { get; set; }
    public List<BankCashAPI> lstReportDoiSoat2 { get; set; }
    public string Type { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        Type = Request["Type"];



        AppUtils.CheckRoles(Resources.Url.CardAPIDoiSoat);

        if (!IsPostBack)
        {
            if (AppUtils.IsProvider)
            {
                Type = 1.ToString();
            }

            if (AppUtils.IsProvider)
            {
                Type = 2.ToString();
            }

            init();
            //GetList1();
            //GetList2();
        }
    }
    public string GetBankType(int status)
    {
        switch (status)
        {
            case 1:
                return "MOMO";
            case 2:
                return "Bank";
            case 3:
                return "ViettelPay";
            case 4:
                return "USDT";
            case 5:
                return "ZALO";
        }
        return "";
    }
    public string getBank(string provider)
    {
        if (provider == "coba"|| provider == "bankvnpaycash")
            return "bank";
        return "momo";
    }
    private void init()
    {
        DateTime time = DateTime.Now;
        time = new DateTime(time.Year, time.Month, 1);
        txtBeginTime1.Text = txtBeginTime2.Text = time.ToString("MM/dd/yyyy");
        txtEndTime1.Text = txtEndTime2.Text = time.AddMonths(1).AddDays(-1).ToString("MM/dd/yyyy");

        var lst = new List<Partners>();
        if (AppUtils.IsAdmin)
            lst = new Partners().GetList();//.Where(e => e.Status == 1).ToList();
        else
            lst = new Partners().GetListByUserId(AppUtils.UserID);
        lst = lst.OrderBy(x => x.PartnerCode).ToList();
        drpPartner1.DataSource = lst;
        drpPartner1.DataTextField = "Name";
        drpPartner1.DataValueField = "PartnerCode";
        drpPartner1.DataBind();
        drpPartner1.Items.Insert(0, new ListItem("Đối tác:", ""));


        var lstBankcode = new BankCashAPI().GetBankCode();
        drpBankCode1.DataSource = lstBankcode;
        drpBankCode1.DataTextField = "BankCode";
        drpBankCode1.DataValueField = "BankCode";
        drpBankCode1.DataBind();
        drpBankCode1.Items.Insert(0, new ListItem("BankCode:", ""));


        drpBankCode2.DataSource = lstBankcode;
        drpBankCode2.DataTextField = "BankCode";
        drpBankCode2.DataValueField = "BankCode";
        drpBankCode2.DataBind();
        drpBankCode2.Items.Insert(0, new ListItem("BankCode:", ""));

        var lstProvider = new List<Providers>();
        if (AppUtils.IsAdmin)
            lstProvider = new Providers().GetList(18);//.Where(e => e.Status == 1).ToList();
        else
            lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 18).ToList();

        drpProvider2.DataSource = lstProvider;
        drpProvider2.DataTextField = "Name";
        drpProvider2.DataValueField = "ProviderCode";
        drpProvider2.DataBind();
        drpProvider2.Items.Insert(0, new ListItem("Nhà cung cấp:", ""));
    }

    private void GetList1()
    {
        string partnerCodes = drpPartner1.SelectedValue;
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

        string bankCode = drpBankCode1.SelectedValue;
        string providerCodes = string.Empty;
        DateTime beginTime = AppUtils.DateTimeParseExact(txtBeginTime1.Text);
        DateTime endTime = AppUtils.DateTimeParseExact(txtEndTime1.Text).AddDays(1);
        BankCashAPI _BankCash = new BankCashAPI();
        lstReportDoiSoat1 = _BankCash.ReportDoiSoat(partnerCodes, providerCodes, bankCode, beginTime, endTime, 1);
    }
    private void GetList2()
    {
        string providerCodes = drpProvider2.SelectedValue;

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

        string bankCode = drpBankCode2.SelectedValue;
        DateTime beginTime = AppUtils.DateTimeParseExact(txtBeginTime2.Text);
        DateTime endTime = AppUtils.DateTimeParseExact(txtEndTime2.Text).AddDays(1);
        BankCashAPI _BankCash = new BankCashAPI();
        lstReportDoiSoat2 = _BankCash.ReportDoiSoat("", providerCodes, bankCode, beginTime, endTime, 2);
    }

    protected void btView_Click1(object sender, EventArgs e)
    {
        Type = "1";
        GetList1();
    }
    protected void btView_Click2(object sender, EventArgs e)
    {
        Type = "2";
        GetList2();
    }
}