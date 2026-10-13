using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using Libs.Utils;

public partial class Pages_Topup_Topup_List_Order : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.TopupListOrder);

        if (!IsPostBack)
        {
            init();
            BindData();
        }

    }

    private void init()
    {
        txtCreatTime.Text = DateTime.Now.AddDays(1).ToString();

        txtUsers.DataSource = new Users().GetListTopupBySort(AppUtils.IsAdmin, AppUtils.IsTopup, AppUtils.UserID);
        txtUsers.DataBind();
        txtUsers.DataTextField = "UserName";
        txtUsers.DataValueField = "UserId";
        txtUsers.DataBind();
        txtUsers.Items.Insert(0, new ListItem("Tài khoản:", ""));
    }

    private void BindData()
    {

        string UserIDs = txtUsers.SelectedValue;
        if (string.IsNullOrEmpty(UserIDs) && !AppUtils.IsAdmin)
        {
            var lstUsers = new Users().GetList();
            if (lstUsers != null)
                if (AppUtils.IsTopup)
                    lstUsers = lstUsers.Where(e => e.ParentId == AppUtils.UserID || e.UserID == AppUtils.UserID && e.IsTopup == 1).ToList();
                else if (AppUtils.IsAdmin)
                    lstUsers = lstUsers.Where(e => e.IsTopup == 1 || e.IsAdmin == 1).ToList();
            if (lstUsers != null && lstUsers.Count > 0)
                UserIDs = string.Join(",", lstUsers.Select(e => e.UserID.ToString()).ToArray());
            else
                UserIDs = "-1";
        }

        int top = Convert.ToInt32(drpTop.SelectedValue);
        int? status = null;
        if (txtStatus.SelectedValue != "") status = Convert.ToInt32(txtStatus.SelectedValue);
        int Amount = 0;
        DateTime creatTime = AppUtils.ToDateTime(txtCreatTime.Text);
        string orderNo = txtOrderNo.Text;
        long totalSuccess = 0;
        long totalRequest = 0;
        long totalWaiting = 0;
        rptList.DataSource = new TopupMobileLog().GetTableOrder(top, UserIDs, txtTelco.SelectedValue, orderNo, creatTime, status, string.Empty,null,  ref totalSuccess, ref totalRequest, ref totalWaiting);
        lblTotalSuccess.Text = totalSuccess.ToString("#,#").Replace(",", ".");
        lblTotalRequest.Text = totalRequest.ToString("#,#").Replace(",", ".");
        lblTotalMiss.Text = (totalRequest - totalSuccess).ToString("#,#").Replace(",", ".");
        lblTotalWaiting.Text = totalWaiting.ToString("#,#").Replace(",", ".");
        rptList.DataBind();
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
    }

    protected string EditlUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.TopupEdit + "?id=" + id;
    }

    protected string SearchUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.TopupSearch + "?m=" + id;
    }

    protected string DellUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.TopupDelete + "?id=" + id;
    }

    protected bool VisableDel(int Amoumt, int AmountUser)
    {
        return Amoumt == AmountUser;
    }

    protected void rptList_OnItemCommand(object source, RepeaterCommandEventArgs e)
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

        if (e.CommandName == "Export")
        {
            var orderNo = e.CommandArgument.ToString();
            var orderList = new TopupMobileLog().GetTableOrderExport(userIds, orderNo);
            orderList.TableName = orderNo == string.Empty ? "NoName" : Regex.Replace(orderNo.Length > 30 ? orderNo.Substring(0, 30) : orderNo, @"&quot;|['"",&?%\.*:#/\\-]", "").Trim();
            new Utils().ExportedExcel(orderList, orderNo, this.Response);


        }

        if (e.CommandName == "ExportTrans")
        {
            var orderNo = e.CommandArgument.ToString();
            var orderList = new TopupMobileLog().GetTableTransactionExport(userIds, orderNo, null, null, null);
            orderList.TableName = orderNo == string.Empty ? "NoName" : Regex.Replace(orderNo.Length > 30 ? orderNo.Substring(0, 30) : orderNo, @"&quot;|['"",&?%\.*:#/\\-]", "").Trim();
            NLogLogger.Info(new string[] { "Topup.Report.DoiSoat", userIds, orderNo, null, null, null });
            new Utils().ExportedExcel(orderList, orderNo, this.Response);

        }
    }

}