using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
public partial class Pages_BankEWalletService_Bank_Report : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankReport);

        if (!IsPostBack)
        {
            init();
            GetList();
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

    }

    private void GetList()
    {


        int year = Convert.ToInt32(drpYear.SelectedValue);
        int month = Convert.ToInt32(drpMonth.SelectedValue);
        int day = Convert.ToInt32(drpDay.SelectedValue);


        if (year == 0)
        {
            month = 0;
            day = 0;
        }

        if (month == 0) day = 0;

        BankTransaction _BankGateAPI = new BankTransaction();
        var lstdata = _BankGateAPI.Report(year, month, day,txtBankId.Text);

        if (lstdata != null)
        {
            lblTotalIn.Text = string.Format("{0} ({1})", lstdata.Sum(x => x.TotalAmountIn).ToString("#,#").Replace(".", ","), lstdata.Sum(x => x.TotalIn).ToString());
            lblTotalOut.Text = string.Format("{0} ({1})", lstdata.Sum(x => x.TotalAmountOut).ToString("#,#").Replace(".", ","), lstdata.Sum(x => x.TotalOut).ToString());
           
            lblTotalTranfer.Text = string.Format("{0} ({1})", lstdata.Sum(x => x.TotalAmountTranfer).ToString("#,#").Replace(".", ","), lstdata.Sum(x => x.TotalTranfer).ToString());
            
        }
        rptList.DataSource = lstdata;


        rptList.DataBind();
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
}