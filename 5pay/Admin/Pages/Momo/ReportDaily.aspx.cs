using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;

public partial class Pages_Momo_ReportDaily : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.MomoReportDaily);
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
        txtEndTime1.Text =DateTime.Now.ToString("MM/dd/yyyy");



    }
    private void GetList()
    {


        DateTime beginTime = AppUtils.DateTimeParseExact(txtBeginTime1.Text);
        DateTime endTime = AppUtils.DateTimeParseExact(txtEndTime1.Text).AddDays(1);

        MomoTransaction _BankGateAPI = new MomoTransaction();
        var lstdata = _BankGateAPI.ReportDaily(beginTime, endTime);

        if (lstdata != null)
        {
            lblTotalIn.Text = string.Format("{0} ({1})", lstdata.Sum(x => x.TotalAmountIn).ToString("#,#").Replace(".", ","), lstdata.Sum(x => x.TotalIn).ToString());
            lblTotalOut.Text = string.Format("{0} ({1})", lstdata.Sum(x => x.TotalAmountOut).ToString("#,#").Replace(".", ","), lstdata.Sum(x => x.TotalOut).ToString());
            lblTotalCash.Text = string.Format("{0} ({1})", lstdata.Sum(x => x.TotalAmountCash).ToString("#,#").Replace(".", ","), lstdata.Sum(x => x.TotalCash).ToString());
            lblTotalTranferInternal.Text = string.Format("{0} ({1})", lstdata.Sum(x => x.TotalAmountTranferInternal).ToString("#,#").Replace(".", ","), lstdata.Sum(x => x.TotalTranferInternal).ToString());
            lblTotalTranferOutside.Text = string.Format("{0} ({1})", lstdata.Sum(x => x.TotalAmountTranferOutside).ToString("#,#").Replace(".", ","), lstdata.Sum(x => x.TotalTranferOutside).ToString());
        }
        rptList.DataSource = lstdata;


        rptList.DataBind();
    }

    protected void btView_Click1(object sender, EventArgs e)
    {
        GetList();
    }
}