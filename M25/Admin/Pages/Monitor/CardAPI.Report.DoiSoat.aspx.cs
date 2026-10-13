using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using System.Globalization;
using Libs.API;
using Libs.Utils;

public partial class Pages_Monitor_CardAPI_Report_DoiSoat : System.Web.UI.Page
{
    public List<CardAPILog> lstReportDoiSoat1 { get; set; }
    public List<CardAPILog> lstReportDoiSoat2 { get; set; }
    public string Type { get; set; }
    public int ShowAmountResidual { get; set; }
    public int Amount { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        Type = Request["Type"];

        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();

        AppUtils.CheckRoles(Resources.Url.CardAPIDoiSoat);

        if (!IsPostBack)
        {
            Amount = 0;
            if (AppUtils.IsProvider)
            {
                Type = 1.ToString();
            }

            if (AppUtils.IsProvider)
            {
                Type = 2.ToString();
            }

            init();
            //GetList1();
            //GetList2();
        }
    }

    private void init()
    {
        DateTime time = DateTime.Now;
        time = new DateTime(time.Year, time.Month, 1);
        txtBeginTime1.Text = txtBeginTime2.Text = time.ToString("MM/dd/yyyy");
        txtEndTime1.Text = txtEndTime2.Text = time.AddMonths(1).AddDays(-1).ToString("MM/dd/yyyy");

        var lst = new List<Partners>();
        if (AppUtils.IsAdmin)
            lst = new Partners().GetList();//.Where(e => e.Status == 1).ToList();
        else
            lst = new Partners().GetListByUserId(AppUtils.UserID);
        lst = lst.OrderBy(x => x.PartnerCode).ToList();
        drpPartner1.DataSource = lst;
        drpPartner1.DataTextField = "Name";
        drpPartner1.DataValueField = "PartnerCode";
        drpPartner1.DataBind();
        drpPartner1.Items.Insert(0, new ListItem("Đối tác:", ""));


        var _CardAPILog1 = new CardAPILog();
        drpCardType1.DataSource = _CardAPILog1.GetCardType();
        drpCardType1.DataTextField = "CardType";
        drpCardType1.DataValueField = "CardType";
        drpCardType1.DataBind();
        drpCardType1.Items.Insert(0, new ListItem("Loại Thẻ:", ""));

        var _CardAPILog2 = new CardAPILog();
        drpCardType2.DataSource = _CardAPILog2.GetCardType();
        drpCardType2.DataTextField = "CardType";
        drpCardType2.DataValueField = "CardType";
        drpCardType2.DataBind();
        drpCardType2.Items.Insert(0, new ListItem("Loại Thẻ:", ""));

        var lstProvider = new List<Providers>();
        if (AppUtils.IsAdmin)
            lstProvider = new Providers().GetList(7);//.Where(e => e.Status == 1).ToList();
        else
            lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 7).ToList();
        lstProvider = lstProvider.OrderBy(x => x.ProviderCode).ToList();

       
        drpProvider2.DataSource = lstProvider;
        drpProvider2.DataTextField = "Name";
        drpProvider2.DataValueField = "ProviderCode";
        drpProvider2.DataBind();
        drpProvider2.Items.Insert(0, new ListItem("Nhà cung cấp:", ""));
    }

    private void GetList1()
    {
        string partnerCodes = drpPartner1.SelectedValue;
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
        ShowAmountResidual = 0;
        if (AppUtils.UserName.ToLower() == "huv" || drpPartner1.SelectedValue.ToLower() == "huv")
            ShowAmountResidual = 1;
        int Amount = 0;
        string CardType = drpCardType1.SelectedValue;
        string providerCodes = string.Empty;
        DateTime beginTime = AppUtils.DateTimeParseExact(txtBeginTime1.Text);
        DateTime endTime = AppUtils.DateTimeParseExact(txtEndTime1.Text).AddDays(1);
        CardAPILog _CardAPILog = new CardAPILog();
        Amount = 0;
        lstReportDoiSoat1 = _CardAPILog.ReportDoiSoat(partnerCodes, providerCodes, CardType, beginTime, endTime, 1,ref Amount);
        if(ShowAmountResidual==1)
        {
            lblAmount.Text = Amount.ToString("#,#").Replace(",", ".");
        }    
        

    }
    private void GetList2()
    {
        string providerCodes = drpProvider2.SelectedValue;

        if (AppUtils.IsProvider && !AppUtils.IsAdmin)
        {
            if (string.IsNullOrEmpty(providerCodes))
            {
                var lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 7).ToList();
                if (lstProvider != null && lstProvider.Count > 0)
                {
                    providerCodes = string.Join(",", lstProvider.Select(e => e.ProviderCode).ToArray());
                }
            }

        }
        ShowAmountResidual = 0;
        if (AppUtils.UserName.ToLower() == "huv" || drpPartner1.SelectedValue.ToLower() == "huv")
            ShowAmountResidual = 1;
        int  Amount = 0;
        string CardType = drpCardType2.SelectedValue;
        DateTime beginTime = AppUtils.DateTimeParseExact(txtBeginTime2.Text);
        DateTime endTime = AppUtils.DateTimeParseExact(txtEndTime2.Text).AddDays(1);
        CardAPILog _CardAPILog = new CardAPILog();
        lstReportDoiSoat2 = _CardAPILog.ReportDoiSoat("", providerCodes, CardType, beginTime, endTime, 2,ref Amount);
        if (ShowAmountResidual == 1)
        {
            lblAmount.Text = Amount.ToString("#,#").Replace(",", ".");
        }
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