using System;
using System.Linq;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;

public partial class Pages_Topup_Report_Telco : System.Web.UI.Page
{ 

    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.TopupReportCardType);

        if (!IsPostBack)
        {
            init();
            GetList();
        }
    }

    private void init()
    {
        drpYear.Items.Add(new ListItem("Năm:", "0"));
        for (int i = 2013; i <= DateTime.Now.Year; i++)
        {
            drpYear.Items.Add(new ListItem(i.ToString()));
        }
        drpMonth.Items.Add(new ListItem("Tháng:", "0"));
        for (int i = 1; i <= 12; i++)
        {
            drpMonth.Items.Add(new ListItem(i.ToString()));
        }
        drpDay.Items.Add(new ListItem("Ngày", "0"));
        for (int i = 1; i <= 31; i++)
        {
            drpDay.Items.Add(new ListItem(i.ToString()));
        }
        drpYear.SelectedValue = DateTime.Now.Year.ToString();
        drpMonth.SelectedValue = DateTime.Now.Month.ToString();
        drpDay.SelectedValue = DateTime.Now.Day.ToString();

        txtUsers.DataSource = new Users().GetListTopupBySort(AppUtils.IsAdmin, AppUtils.IsTopup, AppUtils.UserID);
        txtUsers.DataBind();
        txtUsers.DataTextField = "UserName";
        txtUsers.DataValueField = "UserId";
        txtUsers.DataBind();
        txtUsers.Items.Insert(0, new ListItem("Tài khoản:", ""));
    }

    private void GetList()
    {
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
        string UserIds = txtUsers.SelectedValue;

        if (string.IsNullOrEmpty(UserIds))
        {
            var lst = new Users().GetListTopupBySort(AppUtils.IsAdmin, AppUtils.IsTopup, AppUtils.UserID);
            if (lst != null && lst.Count > 0)
                UserIds = string.Join(",", lst.Select(e => e.UserID.ToString()).ToArray());
            else
                UserIds = "0";
        }


        rptList.DataSource = new TopupMobileTransactionLog().ReportTelco(UserIds, txtTelco.SelectedValue, year, month, day, ref totalTransaction, ref totalAmount);
        lblTotalTransaction.Text = totalTransaction.ToString("#,#").Replace(",", ".");
        lblTotalAmount.Text = totalAmount.ToString("#,#").Replace(",", ".");
        rptList.DataBind(); 
    } 
    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    } 
}