using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using Libs.Utils;
public partial class Pages_Monitor_BankGateAPI_Report_Profit2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankGateAPIReportProfit);
        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();

        if (!IsPostBack)
        {
            init();
            //GetList();
        }
    }

    private void init()
    {



        DateTime time = DateTime.Now;
        DateTime time2 = new DateTime(time.Year, time.Month, 1);
        //txtBeginTime1.Text = txtBeginTime2.Text = time.ToString("MM/dd/yyyy");
        //txtEndTime1.Text = txtEndTime2.Text = time.AddMonths(1).AddDays(-1).ToString("MM/dd/yyyy");

        //time = DateTime.Now.AddDays(-1);
        txtBeginTime1.Text = time2.ToString("MM/dd/yyyy");
        txtEndTime1.Text = time.ToString("MM/dd/yyyy");


        //drpBankCode.DataSource = new BankGateAPI().GetBankCode();
        //drpBankCode.DataTextField = "BankCode";
        //drpBankCode.DataValueField = "BankCode";
        //drpBankCode.DataBind();
        //drpBankCode.Items.Insert(1, new ListItem("Momo", "momo"));
        //drpBankCode.Items.Insert(2, new ListItem("BankTranfer", "banktranfer"));


        //var lst = new List<Partners>();
        //if (AppUtils.IsAdmin)
        //    lst = new Partners().GetList();//.Where(e => e.Status == 1).ToList();
        //else
        //    lst = new Partners().GetListByUserId(AppUtils.UserID);
        //lst = lst.OrderBy(x => x.PartnerCode).ToList();
        //drpPartner.DataSource = lst;
        //drpPartner.DataTextField = "Name";
        //drpPartner.DataValueField = "PartnerCode";
        //drpPartner.DataBind();
        //drpPartner.Items.Insert(0, new ListItem("Đối tác:", ""));



    }
    private void GetList2()
    {




        string bankCode = string.Empty; //drpBankCode.SelectedValue;
        DateTime beginTime = AppUtils.DateTimeParseExact(txtBeginTime1.Text);
        DateTime endTime = AppUtils.DateTimeParseExact(txtEndTime1.Text).AddDays(1);



        BankGateAPI _bankGateApiAPILog = new BankGateAPI();
        var lstdata = _bankGateApiAPILog.ReportProfitV3("mr", string.Empty, beginTime, endTime); ;
        if (lstdata != null)
        {
            lblTotalAmount2.Text = lstdata.Sum(x => x.TotalAmount).ToString("#,#").Replace(".", ",");
            lblFee2.Text = lstdata.Sum(x => x.Fee).ToString("#,#").Replace(".", ",");
            lblReward2.Text = lstdata.Sum(x => x.Reward).ToString("#,#").Replace(".", ",");
            lblProfit2.Text = lstdata.Sum(x => x.Profit).ToString("#,#").Replace(".", ",");
            lblFeeProvider2.Text = lstdata.Sum(x => x.FeeProvider).ToString("#,#").Replace(".", ",");
            lblTotalTransaction2.Text = lstdata.Sum(x => x.TotalTransaction).ToString();
            lblFit2.Text = AppUtils.AmountToPercentFit(lstdata.Sum(x => x.TotalAmount).ToString(), lstdata.Sum(x => x.Profit).ToString());


        }
        BankCashAPI _bankGateApiAPILog2 = new BankCashAPI();

        var lstTotal = new List<BankReportProfitTotal>();
        foreach (var item in lstdata)
        {
            var itemadd = new BankReportProfitTotal();
            itemadd.PartnerCode = item.PartnerCode;
            itemadd.TotalTransaction = item.TotalTransaction;
            itemadd.TotalAmount = item.TotalAmount;
            itemadd.Fee = item.Fee;
            itemadd.Reward = item.Reward;
            itemadd.Profit = item.Profit;
            itemadd.FeeProvider = item.FeeProvider;
            itemadd.TotalProfit = itemadd.Profit + itemadd.Profit2;
            lstTotal.Add(itemadd);
        }

        rptList2.DataSource = lstTotal;

        rptList2.DataBind();
    }
    private void GetList()
    {




        string bankCode = string.Empty; //drpBankCode.SelectedValue;
        DateTime beginTime = AppUtils.DateTimeParseExact(txtBeginTime1.Text);
        DateTime endTime = AppUtils.DateTimeParseExact(txtEndTime1.Text).AddDays(1);



        BankGateAPI _bankGateApiAPILog = new BankGateAPI();
        var lstdata = _bankGateApiAPILog.ReportProfitV3("hh", string.Empty, beginTime, endTime); ;
        if (lstdata != null)
        {
            lblTotalAmount.Text = lstdata.Sum(x => x.TotalAmount).ToString("#,#").Replace(".", ",");
            lblFee.Text = lstdata.Sum(x => x.Fee).ToString("#,#").Replace(".", ",");
            lblReward.Text = lstdata.Sum(x => x.Reward).ToString("#,#").Replace(".", ",");
            lblProfit.Text = lstdata.Sum(x => x.Profit).ToString("#,#").Replace(".", ",");
            lblFeeProvider.Text = lstdata.Sum(x => x.FeeProvider).ToString("#,#").Replace(".", ",");
            lblTotalTransaction.Text = lstdata.Sum(x => x.TotalTransaction).ToString();
            lblFit.Text = AppUtils.AmountToPercentFit(lstdata.Sum(x => x.TotalAmount).ToString(), lstdata.Sum(x => x.Profit).ToString());


        }
        BankCashAPI _bankGateApiAPILog2 = new BankCashAPI();

        var lstTotal = new List<BankReportProfitTotal>();
        foreach (var item in lstdata)
        {
            var itemadd = new BankReportProfitTotal();
            itemadd.PartnerCode = item.PartnerCode;
            itemadd.TotalTransaction = item.TotalTransaction;
            itemadd.TotalAmount = item.TotalAmount;
            itemadd.Fee = item.Fee;
            itemadd.Reward = item.Reward;
            itemadd.Profit = item.Profit;
            itemadd.FeeProvider = item.FeeProvider;
            itemadd.TotalProfit = itemadd.Profit + itemadd.Profit2;
            lstTotal.Add(itemadd);
        }

        rptList.DataSource = lstTotal;

        rptList.DataBind();
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
        GetList2();
    }
    public class BankReportProfitTotal
    {
        public string PartnerCode { get; set; }
        public long TotalTransaction { get; set; }
        public long TotalAmount { get; set; }
        public long Fee { get; set; }
        public long FeeProvider { get; set; }
        public long Reward { get; set; }

        public long Profit { get; set; }

        public long TotalTransaction2 { get; set; }
        public long TotalAmount2 { get; set; }
        public long Fee2 { get; set; }
        public long Reward2 { get; set; }

        public long Profit2 { get; set; }
        public long TotalProfit { get; set; }

    }
}