using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Math;
using System.Web.Script.Serialization;
using Libs.Utils;



public partial class Pages_Monitor_BankGateAPI_Report_Partner2 : System.Web.UI.Page
{
    public List<string> Partner { get; set; }

    public List<BankGateAPILogReportPartner> Data { get; set; }

    public List<BankGateAPILogReportPartner> DataTotal { get; set; }
    public List<int> Time { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankGateAPIReportPartner);
        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
        if (!IsPostBack)
        {
            init();
            GetList();
        }
    }

    private void init()
    {


        var lstBankCode = new List<BankCashAPI>();
        lstBankCode.Add(new BankCashAPI { BankCode = "MOMO" });
        lstBankCode.Add(new BankCashAPI { BankCode = "BANK" });
        drpBankCode.DataSource = lstBankCode;
        drpBankCode.DataTextField = "BankCode";
        drpBankCode.DataValueField = "BankCode";
        drpBankCode.DataBind();
        drpBankCode.Items.Insert(0, new ListItem("BankCode:", ""));


        // Year
        drpYear.Items.Add(new ListItem(Resources.Pay.Year, "0"));
        for (int i = 2013; i <= DateTime.Now.Year; i++)
        {
            drpYear.Items.Add(new ListItem(i.ToString()));
        }

        // Month
        drpMonth.Items.Add(new ListItem(Resources.Pay.Month, "0"));
        for (int i = 1; i <= 12; i++)
        {
            drpMonth.Items.Add(new ListItem(i.ToString()));
        }

        // Day
        drpDay.Items.Add(new ListItem(Resources.Pay.Day, "0"));
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
        string bankcode = drpBankCode.SelectedValue;


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
        BankGateAPI _BankGateAPI = new BankGateAPI();
        Data = _BankGateAPI.ReportPartner2(bankcode, string.Empty, year, month, day, ref totalTransaction, ref totalAmount);
        //JavaScriptSerializer serializer = new JavaScriptSerializer();
        //NLogLogger.Info(new string[] { "reprt", serializer.Serialize(Data) });

        var lstPartner = Data.OrderBy(x => x.TotalAmount).GroupBy(x => x.PartnerCode).Select(a => a.Key).OrderBy(x => x).ToList();

        //Partner = Partner.Where(x => !x.Contains("zab")  && !x.Contains("hyn") && !x.Contains("imd") && !x.Contains("ken")).ToList();
        Time = Data.GroupBy(x => x.Time).Select(a => a.Key).ToList();
        DataTotal = new List<BankGateAPILogReportPartner>();
        foreach (var item in lstPartner)
        {
            var pdata = new BankGateAPILogReportPartner { PartnerCode = item };
            pdata.TotalAmount = Data.Where(x => x.PartnerCode == item).Sum(a => a.TotalAmount);
            DataTotal.Add(pdata);

        }
        Partner = new List<String>();
        DataTotal = DataTotal.OrderByDescending(x => x.TotalAmount).ToList();
        foreach (var item in DataTotal)
        {
            Partner.Add(item.PartnerCode);
        }
        //Partner = Partner.Take(15).ToList();
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
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.BankGateAPIReportPartner);
    }

    protected void btService_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.BankGateAPIReportService);
    }
}