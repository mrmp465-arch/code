using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using System.Globalization;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.Expressions;
using DocumentFormat.OpenXml.Drawing;
using Libs.API;
using Libs.Utils;
using System.Data;
using ServiceStack.Common.Extensions;

public partial class Pages_Monitor_CardCheck_Report_DoiSoat : System.Web.UI.Page
{
    public List<APICheckCardLog> lstReportDoiSoat1 { get; set; }
    public MyVTTAccountReport AccountReport { get; set; }
    public int totalSerialValid { get; set; }
    public int totalSerialInvalid { get; set; }

    public int CountOfPartnerCode { get; set; }
    public bool IsFirstRowWithPartnerCode { get; set; }
    public int CountOfCommandCode { get; set; }
    public bool IsFirstRowWithCommandCode { get; set; }


    protected void Page_Load(object sender, EventArgs e)
    {

        AppUtils.CheckRoles(Resources.Url.CardCheckDoiSoat);

        if (!IsPostBack)
        {
            init();
        }
    }

    private void init()
    {
        DateTime time = DateTime.Now;
        time = new DateTime(time.Year, time.Month, 1);
        txtBeginTime1.Text = time.ToString("MM/dd/yyyy");
        txtEndTime1.Text = time.AddMonths(1).AddDays(-1).ToString("MM/dd/yyyy");

        drpPartner1.Items.Insert(0, new ListItem("Đối tác:", ""));
        drpPartner1.Items.Insert(1, new ListItem("payplus", "pp"));
        drpPartner1.Items.Insert(2, new ListItem("zota", "zota"));

        drpCardType1.Items.Insert(0, new ListItem("Loại Thẻ:", ""));
        drpCardType1.Items.Insert(1, new ListItem("VTT", "vtt"));
        drpCardType1.Items.Insert(2, new ListItem("VNP", "vnp"));
        drpCardType1.Items.Insert(3, new ListItem("VMS", "vms"));

        string source = "";
        AccountReport = new MyVTTAccountReport().Report("", source);
    }

    private void GetList1()
    {
        string source = "";
        AccountReport = new MyVTTAccountReport().Report("", source);

        string partnerCode = drpPartner1.SelectedValue;
        string CardType = drpCardType1.SelectedValue;
        DateTime beginTime = AppUtils.DateTimeParseExact(txtBeginTime1.Text);
        DateTime endTime = AppUtils.DateTimeParseExact(txtEndTime1.Text).AddDays(1);
        APICheckCardLog _CardAPILog = new APICheckCardLog();
        lstReportDoiSoat1 = _CardAPILog.Report(partnerCode, CardType, beginTime, endTime);
        totalSerialValid = lstReportDoiSoat1.Select(p => p.SerialValid).Sum();
        totalSerialInvalid = lstReportDoiSoat1.Select(p => p.SerialInvalid).Sum();

        rptList.DataSource = this.GetMergedData(lstReportDoiSoat1);
        //rptList.DataSource = lstReportDoiSoat1;
        rptList.DataBind();
    }

    protected void btView_Click1(object sender, EventArgs e)
    {
        GetList1();
    }


    private List<APICheckCardLog> GetMergedData(List<APICheckCardLog> allProducts)
    {
        List<APICheckCardLog> mergedProducts = new List<APICheckCardLog>();

        var groupingsByPartnerCode = allProducts.GroupBy(grb => grb.PartnerCode);

        foreach (var grbPartnerCode in groupingsByPartnerCode)
        {
            APICheckCardLog firstPartnerCode = grbPartnerCode.First();
            firstPartnerCode.CountOfPartnerCode = grbPartnerCode.Count();
            firstPartnerCode.IsFirstRowWithPartnerCode = true;

            var groupingsByCommandCode = grbPartnerCode.GroupBy(grb => grb.CommandCode);
            foreach (var grbCommandCode in groupingsByCommandCode)
            {
                APICheckCardLog firstCommandCode = grbCommandCode.First();
                firstCommandCode.CountOfCommandCode = grbCommandCode.Count();
                firstCommandCode.IsFirstRowWithCommandCode = true;
            }

            mergedProducts.Add(firstPartnerCode);
            mergedProducts.AddRange(grbPartnerCode.Skip(1));

        }

        return mergedProducts;
    }
}