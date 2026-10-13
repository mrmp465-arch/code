using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using System.Data;


public partial class Pages_Monitor_CardAPI_Report_Partner2 : System.Web.UI.Page
{
    public string UrlDetail { get; set; }

    public List<string> Partner { get; set; }

    public List<CardAPILogReport> Data { get; set; }
    public List<int> Time { get; set; }
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

        

    }

    private void GetList()
    {
        string cardType = drpCardType.SelectedValue;

       
        

        int year = Convert.ToInt32(drpYear.SelectedValue);
        int month = Convert.ToInt32(drpMonth.SelectedValue);
        int day = Convert.ToInt32(drpDay.SelectedValue);
        

        if (year == 0)
        {
            month = 0;
            day = 0;
        }

        if (month == 0) day = 0;

        CardAPILog _CardAPILog = new CardAPILog();

        Data = _CardAPILog.ReportPartner2("", cardType, year, month, day);
       
       
       
        Partner= Data.GroupBy(x => x.PartnerCode).Select(a => a.Key).OrderBy(x=>x).ToList();
        //Partner = Partner.Where(x => !x.Contains("zab")  && !x.Contains("hyn") && !x.Contains("imd") && !x.Contains("ken")).ToList();
        Time = Data.GroupBy(x => x.Time).Select(a => a.Key).ToList();
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