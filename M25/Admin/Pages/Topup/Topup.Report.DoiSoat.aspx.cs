using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using Libs.Utils;

public partial class Pages_Topup_Report_DoiSoat : System.Web.UI.Page
{
    public List<TopupMobileLog> lstReportDoiSoat1 { get; set; }
    public string Type { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        Type = Request["Type"];
        AppUtils.CheckRoles(Resources.Url.TopupDoiSoat);

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
       
        txtUsers.DataSource = new Users().GetListTopupBySort(AppUtils.IsAdmin, AppUtils.IsTopup, AppUtils.UserID);
        txtUsers.DataBind();
        txtUsers.DataTextField = "UserName";
        txtUsers.DataValueField = "UserId";
        txtUsers.DataBind();
        txtUsers.Items.Insert(0, new ListItem("Tài khoản:", ""));


    }

    private void GetList1()
    {
        string UserIds = txtUsers.SelectedValue;

        if (string.IsNullOrEmpty(UserIds))
        {
            var lst = new Users().GetListTopupBySort(AppUtils.IsAdmin, AppUtils.IsTopup, AppUtils.UserID);
            if (lst != null && lst.Count > 0)
                UserIds = string.Join(",", lst.Select(e => e.UserID.ToString()).ToArray());
            else
                UserIds = "0";
        }
        string Provider = string.Empty;
        DateTime beginTime = AppUtils.DateTimeParseExact(txtBeginTime1.Text);
        DateTime endTime = AppUtils.DateTimeParseExact(txtEndTime1.Text).AddDays(1);
        lstReportDoiSoat1 = new TopupMobileTransactionLog().ReportDoiSoat(UserIds, txtTelco.SelectedValue, beginTime, endTime);
    }

    protected void btView_Click1(object sender, EventArgs e)
    {
        GetList1();
    }

    protected void ExportTran_Click(object sender, EventArgs e)
    {
        var userIds = string.Empty;

        if (txtUsers.SelectedValue == "")
        {
            userIds = txtUsers.SelectedValue;
            if (string.IsNullOrEmpty(userIds) && !AppUtils.IsAdmin)
            {
                var lstUsers = new Users().GetList();
                if (lstUsers != null)
                    if (AppUtils.IsTopup)
                        lstUsers = lstUsers.Where(u => u.ParentId == AppUtils.UserID || u.UserID == AppUtils.UserID && u.IsTopup == 1).ToList();
                    else if (AppUtils.IsAdmin)
                        lstUsers = lstUsers.Where(u => u.IsTopup == 1 || u.IsAdmin == 1).ToList();
                if (lstUsers != null && lstUsers.Count > 0)
                    userIds = string.Join(",", lstUsers.Select(u => u.UserID.ToString()).ToArray());
                else
                    userIds = "-1";
            }
        }
        else
        {
            userIds = txtUsers.SelectedValue;
        }

        if (AppUtils.IsAdmin)
        {
            userIds = string.Empty;
        }

        
        DateTime beginTime = AppUtils.DateTimeParseExact(txtBeginTime1.Text);
        DateTime endTime = AppUtils.DateTimeParseExact(txtEndTime1.Text).AddDays(1);
        var orderNo = beginTime.ToString("Mdyyyy") + "_" + endTime.ToString("Mdyyyy");
        var orderList = new TopupMobileLog().GetTableTransactionExport(userIds, null, null, beginTime, endTime);
        orderList.TableName = orderNo == string.Empty ? "NoName" : orderNo;
        //NLogLogger.Info(new string[] { "Topup.Report.DoiSoat", userIds, null, null, beginTime.ToString(), endTime.ToString() });
        new Utils().ExportedExcel(orderList, orderNo, this.Response);
    }
}