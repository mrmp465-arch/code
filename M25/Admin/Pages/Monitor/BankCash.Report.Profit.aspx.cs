using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using Libs.Utils;

public partial class Pages_Monitor_BankCash_Report_Profit : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.CardAPIReport);
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

        time = DateTime.Now;
        txtBeginTime1.Text = time2.ToString("MM/dd/yyyy");
        txtEndTime1.Text = time.ToString("MM/dd/yyyy");


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




    }

    private void GetList()
    {
        string partnerCodes = drpPartner.SelectedValue;


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



        string bankCode = string.Empty; //drpBankCode.SelectedValue;
        DateTime beginTime = AppUtils.DateTimeParseExact(txtBeginTime1.Text);
        DateTime endTime = AppUtils.DateTimeParseExact(txtEndTime1.Text).AddDays(1);

        //int totalTransaction = 0;
        //long totalAmount = 0;
        //long totalProfit = 0;



        BankCashAPI _bankGateApiAPILog = new BankCashAPI();
        var lstdata = _bankGateApiAPILog.ReportProfit("",partnerCodes, beginTime, endTime); ;
        if (lstdata != null)
        {
            lblTotalAmount.Text = lstdata.Sum(x => x.TotalAmount).ToString("#,#").Replace(".", ",");
            lblFee.Text = lstdata.Sum(x => x.Fee).ToString("#,#").Replace(".", ",");
            lblReward.Text = lstdata.Sum(x => x.Reward).ToString("#,#").Replace(".", ",");
            lblProfit.Text = lstdata.Sum(x => x.Profit).ToString("#,#").Replace(".", ",");
            lblTotalTransaction.Text = lstdata.Sum(x => x.TotalTransaction).ToString();

            lblFit.Text = AppUtils.AmountToPercentFit(lstdata.Sum(x => x.TotalAmount).ToString(), lstdata.Sum(x => x.Profit).ToString());

        }
        rptList.DataSource = lstdata;
        //lblTotalTransaction.Text = totalTransaction.ToString();
        //lblTotalAmount.Text = totalAmount.ToString("#,#").Replace(",", ".");
        //lblTotalProfit.Text = totalProfit.ToString("#,#").Replace(",", ".");
        //txtTotalFit.Text = AppUtils.AmountToPercentFit(totalAmount.ToString(), totalProfit.ToString());
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