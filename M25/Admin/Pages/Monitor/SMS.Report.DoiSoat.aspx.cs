using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using System.Globalization;
using Libs.API;

public partial class Pages_Monitor_SMS_Report_DoiSoat : System.Web.UI.Page
{

    public List<MessageIn> lstReportDoiSoat1 { get; set; }
    public List<MessageIn> lstReportDoiSoat2 { get; set; }
    public string Type { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        Type = Request["Type"];
        AppUtils.CheckRoles(Resources.Url.CardAPIDoiSoat);
        if (!IsPostBack)
        {
            init();
            //GetList1();
            //GetList2();
        }
    }

    private void init()
    {
        DateTime time = DateTime.Now.AddMonths(-1);
        time = new DateTime(time.Year, time.Month, 1);
        txtBeginTime1.Text = txtBeginTime2.Text = time.ToString("MM/dd/yyyy");
        txtEndTime1.Text = txtEndTime2.Text = time.AddMonths(1).AddDays(-1).ToString("MM/dd/yyyy");

        var lst = new List<Partners>();
        if (AppUtils.IsAdmin)
            lst = new Partners().GetList().Where(e => e.Status == 1).ToList();
        else
            lst = new Partners().GetListByUserId(AppUtils.UserID);

        drpPartner1.DataSource = lst;
        drpPartner1.DataTextField = "Name";
        drpPartner1.DataValueField = "PartnerID";
        drpPartner1.DataBind();
        drpPartner1.Items.Insert(0, new ListItem("Đối tác:", ""));

        var _Provider2 = new Providers();
        drpProvider2.DataSource = _Provider2.GetList(1);
        drpProvider2.DataTextField = "Name";
        drpProvider2.DataValueField = "ProviderCode";
        drpProvider2.DataBind();
        drpProvider2.Items.Insert(0, new ListItem("Nhà cung cấp:", ""));
    }

    private void GetList1()
    {
        //int partnerID = int.Parse(drpPartner1.SelectedValue);
        string partnerCodes = drpPartner1.SelectedValue;
        if (string.IsNullOrEmpty(partnerCodes) && !AppUtils.IsAdmin)
        {
            var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
            if (lstPartner != null && lstPartner.Count > 0)
                partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode).ToArray());
            else
                partnerCodes = "Empty"; //Set Empty for not search
        }

        string CardType = drpCardType1.SelectedValue;
        string Provider = string.Empty;
        DateTime beginTime = AppUtils.DateTimeParseExact(txtBeginTime1.Text);
        DateTime endTime = AppUtils.DateTimeParseExact(txtEndTime1.Text).AddDays(1);
        MessageIn _MessageInLog = new MessageIn();
        lstReportDoiSoat1 = _MessageInLog.ReportDoiSoat(partnerCodes, Provider, CardType, beginTime, endTime, 1);
    }
    private void GetList2()
    {
        string Provider = drpProvider2.SelectedValue;
        string CardType = drpCardType2.SelectedValue;
        if (!AppUtils.IsAdmin)
        {
            Provider = "";
        }
        DateTime beginTime = AppUtils.DateTimeParseExact(txtBeginTime2.Text);
        DateTime endTime = AppUtils.DateTimeParseExact(txtEndTime2.Text).AddDays(1);
        MessageIn _MessageInLog = new MessageIn();
        lstReportDoiSoat2 = _MessageInLog.ReportDoiSoat("", Provider, CardType, beginTime, endTime, 2);
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