using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using System.Data;

public partial class Pages_Monitor_SMS_Report_Partner : System.Web.UI.Page
{
    public string UrlDetail { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.SMSReportPartner);

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
        
        var lst = new Partners().GetList().Where(e => e.Status == 1).ToList();
        if (!AppUtils.IsAdmin)
            lst = new Partners().GetListByUserId(AppUtils.UserID);
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "partnerCode";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Đối tác:", ""));
    }

    private void GetList()
    {
        string ReceiverNumber = drpReceiverNumber.SelectedValue;
        var partnerCode = drpPartner.SelectedValue.ToString();
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


        MessageIn _MessageIn = new MessageIn();
        var lst = _MessageIn.ReportPartner(partnerCode, ReceiverNumber, year, month, day, ref totalTransaction, ref totalAmount);
        lblTotalTransaction.Text = totalTransaction.ToString("#,#").Replace(",", ".");
        lblTotalAmount.Text = totalAmount.ToString("#,#").Replace(",", ".");

        DataTable table = new DataTable();
        table.Columns.Add("TotalTransaction", typeof(string));
        table.Columns.Add("TotalAmount", typeof(string));
        table.Columns.Add("ReceiverNumber", typeof(string));
        table.Columns.Add("PartnerCode", typeof(string));
        table.Columns.Add("Partner", typeof(string));
        table.Columns.Add("Row", typeof(string));
        table.Columns.Add("STT", typeof(string));
        table.Columns.Add("TrClass", typeof(string));
        int i = 0;
        foreach (DataRow item in lst.Rows)
        {
            i++;
            string TrClass = "old2";
            if (i % 2 == 0)
                TrClass = "even2"; 
            totalTransaction = 0;
            totalAmount = 0;
            table.Rows.Add(item["TotalTransaction"], item["TotalAmount"], item["ReceiverNumber"], item["PartnerCode"], item["PartnerCode"], 0, i, TrClass);
            var lst2 = _MessageIn.ReportReceiverNumber(item["PartnerCode"].ToString(), "", ReceiverNumber, year, month, day, ref totalTransaction, ref totalAmount);
            foreach (DataRow item2 in lst2.Rows)
            {
                table.Rows.Add(item2["TotalTransaction"], item2["TotalAmount"], item2["ReceiverNumber"], item2["PartnerCode"], "", 1, "", "");
            }
        }
        rptList.DataSource = table;
        rptList.DataBind(); 
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }
}