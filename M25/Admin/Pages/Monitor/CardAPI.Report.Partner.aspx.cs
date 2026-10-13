using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using System.Data;

public partial class Pages_Monitor_CardAPI_Report_Partner : System.Web.UI.Page
{
    public string UrlDetail { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.CardAPIReportPartner);

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

        var _CardType = new Products();
        drpCardType.DataSource = _CardType.GetList(7, 1);
        drpCardType.DataTextField = "Name";
        drpCardType.DataValueField = "Code";
        drpCardType.DataBind();
        drpCardType.Items.Insert(0, new ListItem("Loại thẻ:", ""));

        var lst = new List<Partners>();
        if (AppUtils.IsAdmin)
            lst = new Partners().GetList();//.Where(e => e.Status == 1).ToList();
        else
            lst = new Partners().GetListByUserId(AppUtils.UserID);

        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "PartnerCode";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Đối tác:", ""));

    }

    private void GetList()
    {
        string cardType = drpCardType.SelectedValue;

        string partnerCodes = drpPartner.SelectedValue;
        if (string.IsNullOrEmpty(partnerCodes) && !AppUtils.IsAdmin)
        {
            var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
            if (lstPartner != null && lstPartner.Count > 0)
                partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode).ToArray());
            else
                partnerCodes = "Empty"; //Set Empty for not search
        }

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


        CardAPILog _CardAPILog = new CardAPILog();
        var lst = _CardAPILog.ReportPartner(partnerCodes, cardType, year, month, day, ref totalTransaction, ref totalAmount);
        lblTotalTransaction.Text = totalTransaction.ToString("#,#").Replace(",", ".");
        lblTotalAmount.Text = totalAmount.ToString("#,#").Replace(",", ".");
        DataTable table = new DataTable();
        table.Columns.Add("TotalTransaction", typeof(string));
        table.Columns.Add("TotalAmount", typeof(string));
        table.Columns.Add("CardType", typeof(string));
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
            //totalTransaction = 0;
            //totalAmount = 0;
            table.Rows.Add(item["TotalTransaction"], item["TotalAmount"], item["CardType"], item["PartnerID"], item["PartnerCode"], 0, i, TrClass);
            //var lst2 = _CardAPILog.ReportCardType(item["PartnerCode"].ToString(), "", cardType, year, month, day, ref totalTransaction, ref totalAmount);
            //foreach (DataRow item2 in lst2.Rows)
            //{
            //    table.Rows.Add(item2["TotalTransaction"], item2["TotalAmount"], item2["CardType"], item2["PartnerCode"], "", 1, "", "");
            //}
        }
        rptList.DataSource = table;
        rptList.DataBind();
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }

    protected string PartnerDiscountUrl(string id, string code)
    {
        return Constant.ADMIN_PATH + Resources.Url.PartnerDiscount + "?id=" + id + "&code=" + code;
    }

}