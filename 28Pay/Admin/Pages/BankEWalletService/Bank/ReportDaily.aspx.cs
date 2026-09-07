using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
public partial class Pages_BankEWalletService_Bank_ReportDaily : System.Web.UI.Page
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

        BankTransaction _BankGateAPI = new BankTransaction();
        var lstdata = _BankGateAPI.ReportDaily(beginTime, endTime);

        if (lstdata != null)
        {
            lblTotalIn.Text = string.Format("{0} ({1})", lstdata.Sum(x => x.TotalAmountIn).ToString("#,#").Replace(".", ","), lstdata.Sum(x => x.TotalIn).ToString());
            lblTotalOut.Text = string.Format("{0} ({1})", lstdata.Sum(x => x.TotalAmountOut).ToString("#,#").Replace(".", ","), lstdata.Sum(x => x.TotalOut).ToString());

            lblTotalTranfer.Text = string.Format("{0} ({1})", lstdata.Sum(x => x.TotalAmountTranfer).ToString("#,#").Replace(".", ","), lstdata.Sum(x => x.TotalTranfer).ToString());

        }
        rptList.DataSource = lstdata;


        rptList.DataBind();
    }

    protected void btView_Click1(object sender, EventArgs e)
    {
        GetList();
    }
}