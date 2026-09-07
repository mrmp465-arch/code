using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using System.Data;


public partial class Pages_Monitor_CardAPI_Report_CardType2 : System.Web.UI.Page
{
    public string UrlDetail { get; set; }

    public List<string> Partner { get; set; }

    public List<CardAPILogReport> Data { get; set; }
    public List<int> Time { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        //AppUtils.CheckRoles(Resources.Url.CardAPIReportCardType2);

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
        drpDay.SelectedValue = "0";

        //var _CardType = new Products();

        var lst = new List<Partners>();
        if (AppUtils.IsAdmin)
            lst = new Partners().GetList();//.Where(e => e.Status == 1).ToList();
        else
            lst = new Partners().GetListByUserId(AppUtils.UserID);
        lst = lst.OrderBy(x => x.PartnerCode).ToList();
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "PartnerCode";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Đối tác:", ""));


    }

    private void GetList()
    {


        string partnerCodes = drpPartner.SelectedValue;

        if (AppUtils.IsPartner && !AppUtils.IsAdmin)
        {

            if (string.IsNullOrEmpty(partnerCodes))
            {
                var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
                if (lstPartner != null && lstPartner.Count > 0)
                {
                    partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode).ToArray());
                }
            }


        }

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

        Data = _CardAPILog.ReportCardType2(partnerCodes, "", year, month, day);



        var lstCardType = Data.GroupBy(x => x.CardType).Select(a => a.Key).OrderBy(x => x).ToList();
        Partner = new List<String> { "viettel", "vnp", "vms","zing","gate" };
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