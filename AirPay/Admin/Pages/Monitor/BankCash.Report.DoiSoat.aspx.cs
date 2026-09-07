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
using System.IO;

public partial class Pages_Monitor_BankCash_Report_DoiSoat : System.Web.UI.Page
{
    public List<BankCashAPI> lstReportDoiSoat1 { get; set; }
    public List<BankCashAPI> lstReportDoiSoat2 { get; set; }
    public string Type { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        Type = Request["Type"];


        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
        AppUtils.CheckRoles(Resources.Url.BankCashDoiSoat);

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

    private void init()
    {
        btView1.Text = Resources.Pay.View;
        lblTtitle.Text = Resources.Pay.Reconcilation;
        DateTime time = DateTime.Now;
        //time = new DateTime(time.Year, time.Month, 1);
        //txtBeginTime1.Text = txtBeginTime2.Text = time.ToString("MM/dd/yyyy");
        //txtEndTime1.Text = txtEndTime2.Text = time.AddMonths(1).AddDays(-1).ToString("MM/dd/yyyy");
        time = DateTime.Now.AddDays(-1);
        txtBeginTime1.Text = txtBeginTime2.Text = time.ToString("MM/dd/yyyy");
        txtEndTime1.Text = txtEndTime2.Text = time.ToString("MM/dd/yyyy");
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
        //lstReportDoiSoat1 = _BankCash.ReportDoiSoat(partnerCodes, providerCodes, bankCode, beginTime, endTime, 1);
        var lstdata = _BankCash.ReportDoiSoat(partnerCodes, providerCodes, bankCode, beginTime, endTime, 1);
        lstReportDoiSoat1 = new List<BankCashAPI>();
        var listPartnerCode = lstdata.GroupBy(x => x.PartnerCode).Select(x => x.Key);
        foreach (var pacode in listPartnerCode)
        {
            if (lstdata.Exists(x => x.PartnerCode == pacode && x.BankCode == "MOMO"))
            {
                lstReportDoiSoat1.Add(lstdata.FirstOrDefault(x => x.PartnerCode == pacode && x.BankCode == "MOMO"));

            }
            if (lstdata.Exists(x => x.PartnerCode == pacode && x.BankCode != "MOMO"))
            {
                var item = new BankCashAPI { BankCode = "BANK", PartnerCode = pacode };
                item.ReturnValue = lstdata.Where(x => x.PartnerCode == pacode && x.BankCode != "MOMO").Sum(x => x.ReturnValue);
                item.ReturnTotalValue = lstdata.Where(x => x.PartnerCode == pacode && x.BankCode != "MOMO").Sum(x => x.ReturnTotalValue);
                lstReportDoiSoat1.Add(item);
            }
        }
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
        //lstReportDoiSoat2 = _BankCash.ReportDoiSoat("", providerCodes, bankCode, beginTime, endTime, 2);
        var lstdata = _BankCash.ReportDoiSoat("", providerCodes, bankCode, beginTime, endTime, 2);
        lstReportDoiSoat2 = new List<BankCashAPI>();
        var listPartnerCode = lstdata.GroupBy(x => x.ProviderCode).Select(x => x.Key);
        foreach (var pacode in listPartnerCode)
        {
            if (lstdata.Exists(x => x.ProviderCode == pacode && x.BankCode == "MOMO"))
            {
                lstReportDoiSoat2.Add(lstdata.FirstOrDefault(x => x.ProviderCode == pacode && x.BankCode == "MOMO"));

            }
            if (lstdata.Exists(x => x.ProviderCode == pacode && x.BankCode != "MOMO"))
            {
                var item = new BankCashAPI { BankCode = "BANK", ProviderCode = pacode };
                item.ReturnValue = lstdata.Where(x => x.ProviderCode == pacode && x.BankCode != "MOMO").Sum(x => x.ReturnValue);
                item.ReturnTotalValue = lstdata.Where(x => x.ProviderCode == pacode && x.BankCode != "MOMO").Sum(x => x.ReturnTotalValue);
                lstReportDoiSoat2.Add(item);
            }
        }
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
    protected void ExportTran_Click(object sender, EventArgs e)
    {
        string partnerCodes = drpPartner1.SelectedValue;
        if (AppUtils.IsPartner && !AppUtils.IsAdmin)
        {

            if (string.IsNullOrEmpty(partnerCodes))
            {
                var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
                if (lstPartner != null && lstPartner.Count > 0)
                {
                    partnerCodes = string.Join(",", lstPartner.Select(x => x.PartnerCode).ToArray());
                }
            }


        }

        string bankCode = drpBankCode1.SelectedValue;
        string providerCodes = string.Empty;
        DateTime fromdate = AppUtils.DateTimeParseExact(txtBeginTime1.Text);
        DateTime todate = AppUtils.DateTimeParseExact(txtEndTime1.Text).AddDays(1);
        BankGateAPI _BankGateAPI = new BankGateAPI();
        var orderList = new BankCashAPI().ListReportDoiSoat(partnerCodes, "", fromdate, todate, 1);
        var lstData = new List<BanCashAPIExcel>();
        foreach (var bank in orderList)
        {
            var item = new BanCashAPIExcel();
            item.TransactionID = bank.TransactionID;

            //item.OrderNo = bank.OrderNo;
            item.BankCode = bank.BankCode;
            item.BankAccountNumber = "'" + bank.BankAccountNumber;
            item.BankAccountName = bank.BankAccountName;
            item.RefCode = bank.RefCode;
            item.Amount = Convert.ToInt32(bank.TotalAmount);
            item.LastTime = bank.LastTime.ToString("dd/MM/yyy HH:mm");
            lstData.Add(item);
        }

        ExportToExcel(lstData, "cash-" + fromdate.ToString("dd-MM-yyy"));
    }
    protected void ExportTran2_Click(object sender, EventArgs e)
    {
        string providerCodes = drpProvider2.SelectedValue;

        if (AppUtils.IsProvider && !AppUtils.IsAdmin)
        {
            if (string.IsNullOrEmpty(providerCodes))
            {
                var lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 13).ToList();
                if (lstProvider != null && lstProvider.Count > 0)
                {
                    providerCodes = string.Join(",", lstProvider.Select(x => x.ProviderCode).ToArray());
                }
            }

        }

        string bankCode = drpBankCode1.SelectedValue;

        DateTime fromdate = AppUtils.DateTimeParseExact(txtBeginTime1.Text);
        DateTime todate = AppUtils.DateTimeParseExact(txtEndTime1.Text).AddDays(1);
        BankGateAPI _BankGateAPI = new BankGateAPI();
        var orderList = new BankCashAPI().ListReportDoiSoat("", providerCodes, fromdate, todate, 1);
        var lstData = new List<BanCashAPIExcel>();
        foreach (var bank in orderList)
        {
            var item = new BanCashAPIExcel();
            item.TransactionID = bank.TransactionID;
            item.LastTime = bank.LastTime.ToString("dd/MM/yyy HH:mm");
            //item.OrderNo = bank.OrderNo;
            item.BankCode = bank.BankCode;
            item.BankAccountNumber = bank.BankAccountNumber;
            item.BankAccountName = bank.BankAccountName;
            item.RefCode = bank.RefCode;
            item.Amount = Convert.ToInt32(bank.TotalAmount);
            lstData.Add(item);
        }

        ExportToExcel(lstData, "cash-" + fromdate.ToString("dd-MM-yyy"));
    }
    protected void ExportToExcel(List<BanCashAPIExcel> data, string name)
    {
        Response.Clear();
        Response.Buffer = true;
        //Response.Charset = "UTF-8"; 
        Response.AppendHeader("Content-Disposition", "attachment;filename=" + name + ".xls");
        Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
        Response.ContentType = "application/ms-excel";
        EnableViewState = false;
        var myCItrad = new CultureInfo("VI-VN", true);
        var oStringWriter = new StringWriter(myCItrad);
        var oHtmlTextWriter = new HtmlTextWriter(oStringWriter);


        var grid = new DataGrid { DataSource = data };
        grid.DataBind();
        grid.RenderControl(oHtmlTextWriter);

        Response.Write(oStringWriter.ToString());
        Response.Flush();
        Response.End();
    }
    public class BanCashAPIExcel
    {
        public long TransactionID { get; set; }

        public string BankCode { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }
        public string RefCode { get; set; }
        public int Amount { get; set; }
        public string LastTime { get; set; }

    }
}