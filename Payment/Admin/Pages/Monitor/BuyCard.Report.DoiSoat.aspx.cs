using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using System.Globalization;
using Libs.API;

public partial class Pages_Monitor_BuyCard_Report_DoiSoat : System.Web.UI.Page
{
    public List<BuyCard> lstReportDoiSoat1 { get; set; }
    public List<BuyCard> lstReportDoiSoat2 { get; set; }
    public string Type { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        Type = Request["Type"];
        AppUtils.CheckRoles(Resources.Url.BuyCardDoiSoat);
        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
        if (!IsPostBack)
        {
            init();
        }
    }

    private void init()
    {
        DateTime time = DateTime.Now;
        time = new DateTime(time.Year, time.Month, 1);
        txtBeginTime1.Text = txtBeginTime2.Text = time.ToString("MM/dd/yyyy");
        txtEndTime1.Text = txtEndTime2.Text = time.AddMonths(1).AddDays(-1).ToString("MM/dd/yyyy");

        var lst = new Partners().GetList().Where(e => e.Status == 1).ToList();
        if (!AppUtils.IsAdmin)
            lst = new Partners().GetListByUserId(AppUtils.UserID);

        lst = lst.OrderBy(x => x.PartnerCode).ToList();
        drpPartner1.DataSource = lst;
        drpPartner1.DataTextField = "Name";
        drpPartner1.DataValueField = "PartnerID";
        drpPartner1.DataBind();
        drpPartner1.Items.Insert(0, new ListItem("Đối tác:", ""));


        var _BuyCard1 = new BuyCard();
        drpCardType1.DataSource = _BuyCard1.GetCardType();
        drpCardType1.DataBind();
        drpCardType1.DataTextField = "Provider";
        drpCardType1.DataValueField = "Provider";
        drpCardType1.DataBind();
        drpCardType1.Items.Insert(0, new ListItem("Loại Thẻ:", ""));

        var _BuyCard2 = new BuyCard();
        drpCardType2.DataSource = _BuyCard2.GetCardType();
        drpCardType2.DataBind();
        drpCardType2.DataTextField = "Provider";
        drpCardType2.DataValueField = "Provider";
        drpCardType2.DataBind();
        drpCardType2.Items.Insert(0, new ListItem("Loại Thẻ:", ""));

        var _Provider1 = new Providers().GetList(15);
        var _Provider2 = new Providers().GetList(5);
        var allProvider = _Provider1.Concat(_Provider2).ToList();
        drpProvider2.DataSource = allProvider;
        drpProvider2.DataBind();
        drpProvider2.DataTextField = "Name";
        drpProvider2.DataValueField = "ProviderCode";
        drpProvider2.DataBind();
        drpProvider2.Items.Insert(0, new ListItem("Nhà cung cấp:", ""));
    }

    private void GetList1()
    {
        string partnerIDs = drpPartner1.SelectedValue;
        if (string.IsNullOrEmpty(partnerIDs) && !AppUtils.IsAdmin)
        {
            List<Partners> lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
            if (lstPartner != null && lstPartner.Count > 0)
                partnerIDs = string.Join(",", lstPartner.Select(e => e.PartnerID.ToString()).ToArray());
            else
                partnerIDs = "-1";
        }

        string Provider = drpCardType1.SelectedValue;

        DateTime beginTime = AppUtils.DateTimeParseExact(txtBeginTime1.Text);
        DateTime endTime = AppUtils.DateTimeParseExact(txtEndTime1.Text).AddDays(1);
        BuyCard _BuyCard = new BuyCard();
        lstReportDoiSoat1 = _BuyCard.ReportDoiSoat(partnerIDs, Provider, "", beginTime, endTime, 1);
    }
    private void GetList2()
    {
        string ProviderCode = drpProvider2.SelectedValue;
        string Provider = drpCardType2.SelectedValue;
        if (!AppUtils.IsAdmin)
            ProviderCode = "";
        DateTime beginTime = AppUtils.DateTimeParseExact(txtBeginTime2.Text);
        DateTime endTime = AppUtils.DateTimeParseExact(txtEndTime2.Text).AddDays(1);
        BuyCard _BuyCard = new BuyCard();
        lstReportDoiSoat2 = _BuyCard.ReportDoiSoat("", Provider, ProviderCode, beginTime, endTime, 2);
    }

    protected void btView_Click1(object sender, EventArgs e)
    {
        Type = "1";
        GetList1();
    }
    protected void btView_Click2(object sender, EventArgs e)
    {
        Type = "2";
        GetList2();
    }
}