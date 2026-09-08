using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;

public partial class Pages_Monitor_SMS_Report_CardType : System.Web.UI.Page
{ 

    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.SMSDoiSoat);

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
        if (AppUtils.IsAdmin)
        {  
            var _Provider = new Providers();
            drpProvider.DataSource = _Provider.GetList(4);
            drpProvider.DataBind();
            drpProvider.DataTextField = "Name";
            drpProvider.DataValueField = "ProviderCode";
            drpProvider.DataBind();
            drpProvider.Items.Insert(0, new ListItem("Nhà cung cấp:", ""));
        }
    }

    private void GetList()
    {
        string partnerCodes = drpPartner.SelectedValue;
        if (string.IsNullOrEmpty(partnerCodes) && !AppUtils.IsAdmin)
        {
            List<Partners> lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
            if (lstPartner != null && lstPartner.Count > 0)
                partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode.ToString()).ToArray());
            else
                partnerCodes = "-1";
        }
        var Provider = drpProvider.SelectedValue;

        if (!AppUtils.IsAdmin) 
            Provider = ""; 

       
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
        MessageIn _MessageIn = new MessageIn();
        rptList.DataSource = _MessageIn.ReportReceiverNumber(partnerCodes, Provider, cardType, year, month, day, ref totalTransaction, ref totalAmount);
        lblTotalTransaction.Text = totalTransaction.ToString("#,#").Replace(",", ".");
        lblTotalAmount.Text = totalAmount.ToString("#,#").Replace(",", ".");
        rptList.DataBind(); 
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }
    public string GetPartnerCode(string Id)
    {
        if (drpPartner.SelectedValue != "")
        {
            return drpPartner.SelectedItem.Text;
        }
        return "";
    } 
}