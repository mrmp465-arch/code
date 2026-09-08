using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;
using Libs.Utils;

public partial class Default : System.Web.UI.Page
{
    protected string graphLineData = String.Empty;
    protected string graphPieData = String.Empty;
    protected long totalAmount = 0;
    protected int totalTransaction = 0;
    protected string totalMonthAmount;
    protected float monthRate = 0;
    protected int providerCount = 0;
    protected long balance = 0;

    protected int partnerCount = 0;
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckLogin();

        if (AppUtils.CheckRolesPermission(Resources.Url.Dashboard))
        {
            if (!IsPostBack)
            {
                Init();
                GetList(string.Empty, string.Empty, string.Empty);
            }
        }
        else
        {
            PanelContent.Visible = false;
        }

    }

    private void Init()
    {
        var lst = new List<Partners>();
        if (AppUtils.IsAdmin)
            lst = new Partners().GetList().Where(e => e.Status == 1).ToList();
        else
        {
            drpPartner.Visible = false;
            drpCardType.Visible = false;
            lst = new Partners().GetListByUserId(AppUtils.UserID);

            if (AppUtils.IsPartner)
            {
                foreach (var item in lst)
                {
                    balance += item.Balance;
                }
            }
        }

        // Month
        drpMonth.Items.Add(new ListItem("Tháng:", "0"));
        for (int i = 1; i <= 12; i++)
        {
            drpMonth.Items.Add(new ListItem(i.ToString()));
        }

        drpMonth.SelectedValue = DateTime.Now.Month.ToString();

        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "PartnerID";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Đối tác:", ""));

        var _CardType = new Products();
        drpCardType.DataSource = _CardType.GetList(7, 1);
        drpCardType.DataTextField = "Name";
        drpCardType.DataValueField = "Code";
        drpCardType.DataBind();
        drpCardType.Items.Insert(0, new ListItem("Loại thẻ:", ""));
    }

    private void GetList(string partnerIDs, string provider, string cardType)
    {
        //var partnerIDs = ""; 
        //var provider = "";
        //var cardType = "";


        if (!AppUtils.IsAdmin && !AppUtils.IsTopup && AppUtils.IsPartner)
        {
            var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
            if (lstPartner != null && lstPartner.Count > 0)
                partnerIDs = string.Join(",", lstPartner.Select(e => e.PartnerID.ToString()).ToArray());
            else
                partnerIDs = "-1";
        }

        string providerCodes = string.Empty;
        if (AppUtils.IsProvider && !AppUtils.IsAdmin && !AppUtils.IsTopup)
        {
            if (string.IsNullOrEmpty(providerCodes))
            {
                var lstProvider = new Providers().GetListByUserId(AppUtils.UserID);
                if (lstProvider != null && lstProvider.Count > 0)
                {
                    providerCodes = string.Join(",", lstProvider.Select(e => e.ProviderCode).ToArray());
                }
            }

        }

        if (!AppUtils.IsTopup)
        {
            Dashboard dsDashboard = new Dashboard();
            var lineData = dsDashboard.Report3DayLineChart(partnerIDs, providerCodes, cardType);
            graphLineData = serializer.Serialize(lineData);
            var pieData = dsDashboard.ReportPieChart(partnerIDs, cardType, providerCodes);
            graphPieData = serializer.Serialize(pieData);
            rptListProduct.DataSource = pieData;
            rptListProduct.DataBind();
            rptListProductTrans.DataSource = pieData;
            rptListProductTrans.DataBind();

            long totalThisMonth = 0;
            long totalLastMonth = 0;
            int totalProvider = 0;
            int totalPartner = 0;

            NLogLogger.Info(new string[] { "Statis", partnerIDs, providerCodes });
            dsDashboard.ReportDashboardStatic(int.Parse(drpMonth.SelectedValue),partnerIDs, providerCodes, ref totalThisMonth, ref totalLastMonth, ref totalProvider, ref totalPartner);

            //totalMonthAmount = totalThisMonth;
            totalMonthAmount = Libs.Utils.AbbrevationUtility.AbbreviateNumber(totalThisMonth);
            monthRate = (((float)totalThisMonth / (float)totalLastMonth)) * 100;
            providerCount = totalProvider;
            partnerCount = totalPartner;
        }


    }

    protected void drpMonth_SelectedIndexChanged(object sender, EventArgs e)
    {
        var partnerIDs = drpPartner.SelectedValue;
        string providerCodes = string.Empty;
        var cardType = drpCardType.SelectedValue;
        GetList(partnerIDs, providerCodes, cardType);


    }

    protected void drpPartner_SelectedIndexChanged(object sender, EventArgs e)
    {
        var partnerIDs = drpPartner.SelectedValue;
        string providerCodes = string.Empty;
        var cardType = drpCardType.SelectedValue;
        GetList(partnerIDs, providerCodes, cardType);

        //NLogLogger.Info(new string[] { "Default", "SelectedIndexChanged", partnerIDs, providerCodes, cardType });

        //Dashboard dsDashboard = new Dashboard();
        //var lineData = dsDashboard.Report3DayLineChart(partnerIDs, providerCodes, cardType);
        //graphLineData = serializer.Serialize(lineData);
        //var pieData = dsDashboard.ReportPieChart(partnerIDs, cardType, providerCodes);
        //graphPieData = serializer.Serialize(pieData);
        //rptListProduct.DataSource = pieData;
        //rptListProduct.DataBind();
        //rptListProductTrans.DataSource = pieData;
        //rptListProductTrans.DataBind();

        //long totalThisMonth = 0;
        //long totalLastMonth = 0;
        //int totalProvider = 0;
        //int totalPartner = 0;

        //NLogLogger.Info(new string[] { "Statis", partnerIDs, providerCodes });
        //dsDashboard.ReportDashboardStatic(partnerIDs, providerCodes, ref totalThisMonth, ref totalLastMonth, ref totalProvider, ref totalPartner);

        //totalMonthAmount = totalThisMonth;
        //monthRate = (((float)totalThisMonth / (float)totalLastMonth)) * 100;
        //providerCount = totalProvider;
        //partnerCount = totalPartner;
    }

    protected void drpCardType_SelectedIndexChanged(object sender, EventArgs e)
    {
        var partnerIDs = drpPartner.SelectedValue;
        string providerCodes = string.Empty;
        var cardType = drpCardType.SelectedValue;
        GetList(partnerIDs, providerCodes, cardType);
    }
}