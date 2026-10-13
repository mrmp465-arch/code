using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using System.Data;

public partial class Pages_Monitor_BuyCard_Report_Partner : System.Web.UI.Page
{
    public string UrlDetail { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BuyCardReportPartner);

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
        var _BuyCard = new BuyCard();
        drpCardType.DataSource = _BuyCard.GetCardType();
        drpCardType.DataBind();
        drpCardType.DataTextField = "Provider";
        drpCardType.DataValueField = "Provider";
        drpCardType.DataBind();
        drpCardType.Items.Insert(0, new ListItem("Loại Thẻ:", ""));
        var lst = new Partners().GetList().Where(e => e.Status == 1).ToList();
        if (!AppUtils.IsAdmin)
            lst = new Partners().GetListByUserId(AppUtils.UserID);
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "PartnerCode";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Đối tác:", "")); 
    }

    private void GetList()
    {
        
        string partnerCodes = drpPartner.SelectedValue;
        if (string.IsNullOrEmpty(partnerCodes) && !AppUtils.IsAdmin)
        {
            var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
            if (lstPartner != null && lstPartner.Count > 0)
                partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode).ToArray());
            else
                partnerCodes = "Empty"; //Set Empty for not search
        }


        string cardType = drpCardType.SelectedValue;

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


        BuyCard _BuyCard = new BuyCard();
        var lst = _BuyCard.ReportPartner(partnerCodes, cardType, year, month, day, ref totalTransaction, ref totalAmount);
        lblTotalTransaction.Text = totalTransaction.ToString("#,#").Replace(",", ".");
        lblTotalAmount.Text = totalAmount.ToString("#,#").Replace(",", ".");
        DataTable table = new DataTable();
        table.Columns.Add("TotalTransaction", typeof(string));
        table.Columns.Add("TotalAmount", typeof(string));
        table.Columns.Add("Provider", typeof(string));
        table.Columns.Add("PartnerID", typeof(string));
        table.Columns.Add("PartnerCode", typeof(string));
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
            var temp = Convert.ToInt32(item["PartnerID"].ToString());
            totalTransaction = 0;
            totalAmount = 0;
            table.Rows.Add(item["TotalTransaction"], item["TotalAmount"], item["Provider"], item["PartnerCode"] ,( item["PartnerCode"] + "").Replace(" ", ""), 0, i, TrClass);
            var lst2 = _BuyCard.ReportCardType(item["PartnerCode"].ToString(), "", cardType, year, month, day, ref totalTransaction, ref totalAmount);
            foreach (DataRow item2 in lst2.Rows)
            {
                table.Rows.Add(item2["TotalTransaction"], item2["TotalAmount"], item2["Provider"], "", (item2["PartnerCode"] + "").Replace(" ", ""), 1,"","");
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