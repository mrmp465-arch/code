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
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Drawing;


public partial class Pages_Monitor_BankGateAPI_Report_DoiSoat : System.Web.UI.Page
{
    public List<BankGateAPI> lstReportDoiSoat1 { get; set; }
    public List<BankGateAPI> lstReportDoiSoat2 { get; set; }
    public string Type { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        Type = Request["Type"];

        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();

        AppUtils.CheckRoles(Resources.Url.BankGateDoiSoat);

        if (!IsPostBack)
        {
            if (AppUtils.IsPartner)
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
        btView1.Text = Resources.Pay.View;
        lblTtitle.Text = Resources.Pay.Reconcilation;
        drpPartner1.DataSource = lst;
        drpPartner1.DataTextField = "Name";
        drpPartner1.DataValueField = "PartnerCode";
        drpPartner1.DataBind();
        drpPartner1.Items.Insert(0, new ListItem(Resources.Pay.Partner, ""));


        var lstBankcode = new BankGateAPI().GetBankCode();
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
            lstProvider = new Providers().GetList(13);//.Where(e => e.Status == 1).ToList();
        else
            lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 13).ToList();

        drpProvider2.DataSource = lstProvider;
        drpProvider2.DataTextField = "Name";
        drpProvider2.DataValueField = "ProviderCode";
        drpProvider2.DataBind();
        drpProvider2.Items.Insert(0, new ListItem("Nhà cung cấp:", ""));
    }

    private void GetList1()
    {
        string partnerCodes = drpPartner1.SelectedValue;
        //string partnerIDs = drpPartner1.SelectedValue;
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
        if (AppUtils.IsAdmin)
        {
            if (string.IsNullOrEmpty(partnerCodes))
            {
                if (!string.IsNullOrEmpty(drpGroup.SelectedValue))
                {
                    var lstPartner = new Partners().GetList().Where(x => x.SMSUrl == drpGroup.SelectedValue).ToList();
                    if (lstPartner != null && lstPartner.Count > 0)
                    {
                        partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode).ToArray());
                    }
                }
            }
        }

        string bankCode = drpBankCode1.SelectedValue;
        string providerCodes = string.Empty;
        DateTime beginTime = AppUtils.DateTimeParseExact(txtBeginTime1.Text);
        DateTime endTime = AppUtils.DateTimeParseExact(txtEndTime1.Text).AddDays(1);
        BankGateAPI _BankGateAPI = new BankGateAPI();
        //bank in
        var lstdata = _BankGateAPI.ReportDoiSoat(partnerCodes, providerCodes, bankCode, beginTime, endTime, 1);
        lstReportDoiSoat1 = new List<BankGateAPI>();
        var listPartnerCode = lstdata.GroupBy(x => x.PartnerCode).Select(x => x.Key);
        foreach (var pacode in listPartnerCode)
        {
            if (lstdata.Exists(x => x.PartnerCode == pacode && x.BankCode == "MOMO"))
            {
                lstReportDoiSoat1.Add(lstdata.FirstOrDefault(x => x.PartnerCode == pacode && x.BankCode == "MOMO"));

            }
            if (lstdata.Exists(x => x.PartnerCode == pacode && x.BankCode != "MOMO"))
            {
                var item = new BankGateAPI { BankCode = "BANK", PartnerCode = pacode };
                item.ReturnValue = lstdata.Where(x => x.PartnerCode == pacode && x.BankCode != "MOMO").Sum(x => x.ReturnValue);
                item.ReturnTotalValue = lstdata.Where(x => x.PartnerCode == pacode && x.BankCode != "MOMO").Sum(x => x.ReturnTotalValue);
                lstReportDoiSoat1.Add(item);
            }
        }
        //bank out
        BankCashAPI _BankCash = new BankCashAPI();
        var lstdata2 = _BankCash.ReportDoiSoat(partnerCodes, providerCodes, bankCode, beginTime, endTime, 1);
        var lstReportDoiSoat2 = new List<BankCashAPI>();
        var listPartnerCode2 = lstdata2.GroupBy(x => x.PartnerCode).Select(x => x.Key);
        foreach (var pacode in listPartnerCode2)
        {
            if (lstdata2.Exists(x => x.PartnerCode == pacode && x.BankCode == "MOMO"))
            {
                var item = new BankCashAPI { BankCode = "MOMOOUT", PartnerCode = pacode };
                item.ReturnValue = lstdata2.Where(x => x.PartnerCode == pacode && x.BankCode == "MOMO").Sum(x => x.ReturnValue);
                item.ReturnTotalValue = lstdata2.Where(x => x.PartnerCode == pacode && x.BankCode == "MOMO").Sum(x => x.ReturnTotalValue);
                lstReportDoiSoat2.Add(item);
            }
            if (lstdata2.Exists(x => x.PartnerCode == pacode && x.BankCode != "MOMO"))
            {
                var item = new BankCashAPI { BankCode = "BANKOUT", PartnerCode = pacode };
                item.ReturnValue = lstdata2.Where(x => x.PartnerCode == pacode && x.BankCode != "MOMO").Sum(x => x.ReturnValue);
                item.ReturnTotalValue = lstdata2.Where(x => x.PartnerCode == pacode && x.BankCode != "MOMO").Sum(x => x.ReturnTotalValue);
                lstReportDoiSoat2.Add(item);
            }
        }

      
        if (lstReportDoiSoat2.Count > 0)
        {
            foreach (var item in lstReportDoiSoat2)
            {
                if (item != null)
                {
                    var itemreport = new BankGateAPI { BankCode = item.BankCode, PartnerCode = item.PartnerCode, ReturnValue = item.ReturnValue, ReturnTotalValue = item.ReturnTotalValue };
                    lstReportDoiSoat1.Add(itemreport);
                }

            }
        }


        //thẻ
        CardAPILog _CardAPILog = new CardAPILog();
        int Amount = 0;
        var lstdata3 = _CardAPILog.ReportDoiSoat(partnerCodes, providerCodes, string.Empty, beginTime, endTime, 1, ref Amount);
        var lstReportDoiSoat3 = new List<CardAPILog>();
        var listPartnerCode3 = lstdata3.GroupBy(x => x.PartnerCode).Select(x => x.Key);
        foreach (var pacode in listPartnerCode3)
        {

            if (!cbCard.Checked)
            {
                if (lstdata3.Exists(x => x.PartnerCode == pacode && x.CardType == "viettel"))
                {
                    var item = new CardAPILog { CardType = "viettel", PartnerCode = pacode };
                    item.ReturnValue = lstdata3.Where(x => x.PartnerCode == pacode && x.CardType == "viettel").Sum(x => x.ReturnValue);
                    item.Amount = lstdata3.Where(x => x.PartnerCode == pacode && x.CardType == "viettel").Sum(x => x.AmountReal * x.ReturnValue);
                    lstReportDoiSoat3.Add(item);
                }
                if (lstdata3.Exists(x => x.PartnerCode == pacode && x.CardType == "vnp"))
                {
                    var item = new CardAPILog { CardType = "vnp", PartnerCode = pacode };
                    item.ReturnValue = lstdata3.Where(x => x.PartnerCode == pacode && x.CardType == "vnp").Sum(x => x.ReturnValue);
                    item.Amount = lstdata3.Where(x => x.PartnerCode == pacode && x.CardType == "vnp").Sum(x => x.AmountReal * x.ReturnValue);
                    lstReportDoiSoat3.Add(item);
                }
                if (lstdata3.Exists(x => x.PartnerCode == pacode && x.CardType == "vms"))
                {
                    var item = new CardAPILog { CardType = "vms", PartnerCode = pacode };
                    item.ReturnValue = lstdata3.Where(x => x.PartnerCode == pacode && x.CardType == "vms").Sum(x => x.ReturnValue);
                    item.Amount = lstdata3.Where(x => x.PartnerCode == pacode && x.CardType == "vms").Sum(x => x.AmountReal * x.ReturnValue);
                    lstReportDoiSoat3.Add(item);
                }
            }
            else
            {
                if (lstdata3.Exists(x => x.PartnerCode == pacode))
                {
                    var item = new CardAPILog { CardType = "Card", PartnerCode = pacode };
                    item.ReturnValue = lstdata3.Where(x => x.PartnerCode == pacode).Sum(x => x.ReturnValue);
                    item.Amount = lstdata3.Where(x => x.PartnerCode == pacode).Sum(x => x.AmountReal * x.ReturnValue);
                    lstReportDoiSoat3.Add(item);
                }
            }


        }
        foreach (var item in lstReportDoiSoat3)
        {
            if (item != null)
            {
                var itemreport = new BankGateAPI { BankCode = item.CardType, PartnerCode = item.PartnerCode, ReturnValue = item.ReturnValue, ReturnTotalValue = item.Amount };
                lstReportDoiSoat1.Add(itemreport);
            }
        }
        //mua the
        BuyCard _BuyCard = new BuyCard();
        var lstdata4 = _BuyCard.ReportDoiSoat2(partnerCodes, "", "", beginTime, endTime, 1);

        var lstReportDoiSoat4 = new List<BuyCard>();
        var listPartnerCode4 = lstdata4.GroupBy(x => x.PartnerCode).Select(x => x.Key);
        foreach (var pacode in listPartnerCode4)
        {


            if (lstdata4.Exists(x => x.PartnerCode == pacode))
            {
                var item = new BuyCard { ProviderCode = "BuyCard", PartnerCode = pacode };
                item.ReturnValue = int.Parse(lstdata4.Where(x => x.PartnerCode == pacode).Sum(x => x.Quantity).ToString());
                item.Amount = lstdata4.Where(x => x.PartnerCode == pacode).Sum(x => x.Amount * x.Quantity);
                lstReportDoiSoat4.Add(item);
            }


        }
        foreach (var item in lstReportDoiSoat4)
        {
            if (item != null)
            {
                var itemreport = new BankGateAPI { BankCode = item.ProviderCode, PartnerCode = item.PartnerCode, ReturnValue = item.ReturnValue, ReturnTotalValue = item.Amount };
                lstReportDoiSoat1.Add(itemreport);
            }
        }
        //sô dư
        var lstReportDoiSoat5 = new List<BankCashAPI>();
        foreach (var pacode in listPartnerCode)
        {
            var lisWithdraw = new UserWithdraw().GetList(50000, pacode, 1, beginTime, endTime);
            var lisWithdraw2 = new UserDeposit().GetList(50000, pacode, 1, beginTime, endTime);
            if (lisWithdraw != null)
            {
                var item = new BankCashAPI { BankCode = "Rút số dư", PartnerCode = pacode };
                item.ReturnValue = lisWithdraw.Count();
                item.ReturnTotalValue = lisWithdraw.Sum(x => x.Amount);
                lstReportDoiSoat5.Add(item);
            }
            if (lisWithdraw2 != null)
            {
                var item = new BankCashAPI { BankCode = "Nạp số dư", PartnerCode = pacode };
                item.ReturnValue = lisWithdraw2.Count();
                item.ReturnTotalValue = lisWithdraw2.Sum(x => x.Amount);
                lstReportDoiSoat5.Add(item);
            }
        }
        foreach (var item in lstReportDoiSoat5)
        {
            if (item != null)
            {
                var itemreport = new BankGateAPI { BankCode = item.BankCode, PartnerCode = item.PartnerCode, ReturnValue = item.ReturnValue, ReturnTotalValue = item.ReturnTotalValue };
                lstReportDoiSoat1.Add(itemreport);
            }
        }
        lstReportDoiSoat1 = lstReportDoiSoat1.OrderBy(x => x.PartnerCode).ToList();
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
        BankGateAPI _BankGateAPI = new BankGateAPI();
        // lstReportDoiSoat2 = _BankGateAPI.ReportDoiSoat("", providerCodes, bankCode, beginTime, endTime, 2);
        var lstdata = _BankGateAPI.ReportDoiSoat("", providerCodes, bankCode, beginTime, endTime, 2);
        lstReportDoiSoat2 = new List<BankGateAPI>();
        var listPartnerCode = lstdata.GroupBy(x => x.ProviderCode).Select(x => x.Key);
        foreach (var pacode in listPartnerCode)
        {
            if (lstdata.Exists(x => x.ProviderCode == pacode && x.BankCode == "MOMO"))
            {
                lstReportDoiSoat2.Add(lstdata.FirstOrDefault(x => x.ProviderCode == pacode && x.BankCode == "MOMO"));

            }
            if (lstdata.Exists(x => x.ProviderCode == pacode && x.BankCode != "MOMO"))
            {
                var item = new BankGateAPI { BankCode = "BANK", ProviderCode = pacode };
                item.ReturnValue = lstdata.Where(x => x.ProviderCode == pacode && x.BankCode != "MOMO").Sum(x => x.ReturnValue);
                item.ReturnTotalValue = lstdata.Where(x => x.ProviderCode == pacode && x.BankCode != "MOMO").Sum(x => x.ReturnTotalValue);
                lstReportDoiSoat2.Add(item);
            }
        }
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
        var orderList = new BankGateAPI().ListReportDoiSoat(partnerCodes, "", fromdate, todate, 1);
        var lstData = new List<BankGateAPIExcel>();
        foreach (var bank in orderList)
        {
            var item = new BankGateAPIExcel();
            item.TransactionID = bank.TransactionID;
            item.LastTime = bank.LastTime;
            item.OrderNo = bank.OrderNo;
            item.BankCode = bank.BankCode;
            item.PartnerCode = bank.PartnerCode;
            item.ProviderCode = bank.ProviderCode;
            item.Amount = Convert.ToInt32(bank.TotalAmount);
            lstData.Add(item);
        }

        ExportToExcel(lstData, "bank-" + fromdate.ToString("dd-MM-yyy"));
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
        var orderList = new BankGateAPI().ListReportDoiSoat("", providerCodes, fromdate, todate, 1);
        var lstData = new List<BankGateAPIExcel>();
        foreach (var bank in orderList)
        {
            var item = new BankGateAPIExcel();
            item.TransactionID = bank.TransactionID;
            item.LastTime = bank.LastTime;
            item.OrderNo = bank.OrderNo;
            item.BankCode = bank.BankCode;
            item.PartnerCode = bank.PartnerCode;
            item.ProviderCode = bank.ProviderCode;
            item.Amount = Convert.ToInt32(bank.TotalAmount);
            lstData.Add(item);
        }

        ExportToExcel(lstData, "bank-" + fromdate.ToString("dd-MM-yyy"));
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
    protected void ExportToExcel(List<BankGateAPIExcel> data, string name)
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
    public class BankGateAPIExcel
    {
        public long TransactionID { get; set; }
        public int Amount { get; set; }
        public DateTime LastTime { get; set; }
        public string BankCode { get; set; }
        public string OrderNo { get; set; }
        public string PartnerCode { get; set; }
        public string ProviderCode { get; set; }

    }
}