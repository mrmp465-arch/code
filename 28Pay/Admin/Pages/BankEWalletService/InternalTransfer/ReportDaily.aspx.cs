using Libs.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_BankEWalletService_InternalTransfer_ReportDaily : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankReportDaily);
        if (!IsPostBack)
        {
            init();
            GetList();
        }
    }
    private void init()
    {
        DateTime time = DateTime.Now;
        time = new DateTime(time.Year, time.Month, 1);
        txtBeginTime1.Text = time.ToString("MM/dd/yyyy");
        txtEndTime1.Text = DateTime.Now.ToString("MM/dd/yyyy");



    }
    private void GetList()
    {


        DateTime beginTime = AppUtils.DateTimeParseExact(txtBeginTime1.Text);
        DateTime endTime = AppUtils.DateTimeParseExact(txtEndTime1.Text).AddDays(1);

        ITTransaction _BankGateAPI = new ITTransaction();
        var lstdata = _BankGateAPI.ReportDaily(beginTime, endTime, txtBankId.Text);
       

        if (lstdata != null)
        {
            lblTotalIn.Text = lstdata.Sum(x => x.TotalIn).ToString();
            lblTotalAmoutIn.Text = lstdata.Sum(x => x.TotalAmountOut).ToString("#,#").Replace(".", ",");

           
        }
        rptList.DataSource = lstdata;


        rptList.DataBind();
    }

    protected void btView_Click1(object sender, EventArgs e)
    {
        GetList();
    }
}