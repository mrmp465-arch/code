using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;
using Libs.Utils;
using ServiceStack.Text;
public partial class Pages_BankEWalletService_Bank_ReportBalance : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankBalanceReport);
        if (!IsPostBack)
        {


            Init();
            BindData();
        }
    }
    protected void Init()
    {
        // DateTime time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

        //txtBeginTime.Text = time.AddDays(-7).ToString();
        //txtEndTime.Text = time.AddDays(1).ToString();
        //btView.Text = Resources.Pay.View;
        txtEndTime.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");



    }
    protected void BindData()
    {
        //JavaScriptSerializer serializer = new JavaScriptSerializer();
        DateTime endtime = AppUtils.DateTimeParseExact(txtEndTime.Text);
        long time = long.Parse(endtime.ToString("yyyyMMdd"));
        var lstData = new BankDaily().GetList(txtMobile.Text.Trim(), drpBankType.SelectedValue, int.Parse(drpType.SelectedValue), int.Parse(ddlStatus.SelectedValue), time);
        //NLogLogger.Info(new string[] { "datarepot", txtMobile.Text.Trim(), drpBankType.SelectedValue, drpType.SelectedValue, ddlStatus.SelectedValue, time.ToString(),serializer.Serialize(lstData) });
        lblTotal.Text = String.Format("Tổng số dư : {0}", lstData.Sum(x => x.Balance).ToString("N0"));
        rptList.DataSource = lstData;
        rptList.DataBind();
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
    }
    public string formatDay(string day)
    {
        var newDate = DateTime.ParseExact(day,
                                   "yyyyMMdd",
                                    CultureInfo.InvariantCulture);
        return newDate.ToString("dd/MM/yyyy");
        //return String.Format("{0}/{1}/{2}", day[6] + day[7], day[4] + day[5], day[0] + day[1] + day[2] + day[3]);
    }
    public string getType(string type)

    {
        if (type == "1")
            return "BANK";
        return "MOMO";
    }

}