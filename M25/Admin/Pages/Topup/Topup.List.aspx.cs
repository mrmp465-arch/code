using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;

public partial class Pages_Topup_Topup_List : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.TopupList);

        if (!IsPostBack)
        {
            var orderNo = Request["id"];
            if (!string.IsNullOrEmpty(orderNo))
            {

                txtOrderNo.Text = orderNo;

                var status = Request["s"];
                if (!string.IsNullOrEmpty(status))
                    txtStatus.SelectedValue = status;
                else
                    txtStatus.SelectedValue = "";
                BindData();
            }

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

        var orderNo = Request["id"];
        var status = Request["s"];
        if (!string.IsNullOrEmpty(orderNo) || !string.IsNullOrEmpty(status))
        {
            txtConfirm.SelectedValue = "";
        }

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
        string RequestNo = "";
        string Mobile = txtMobile.Text;
        int Amount = 0;
        DateTime creatTime = AppUtils.ToDateTime(txtCreatTime.Text);
        string strPriority = txtPriority.SelectedValue;
        int? priority;
        if (string.IsNullOrEmpty(strPriority))
            priority = null;
        else priority = Convert.ToInt32(strPriority);
        string orderNo = txtOrderNo.Text;
        string strTopuptype = txtTopupType.SelectedValue;
        int? topuptype;
        if (string.IsNullOrEmpty(strTopuptype))
            topuptype = null;
        else topuptype = Convert.ToInt32(strTopuptype);

        string strIsConfirm = txtConfirm.SelectedValue;
        int? isConfirm;
        if (string.IsNullOrEmpty(strIsConfirm))
            isConfirm = null;
        else isConfirm = Convert.ToInt32(strIsConfirm);

        string strUssd = txtUssd.SelectedValue;
        int? ussd;
        if (string.IsNullOrEmpty(strUssd))
            ussd = null;
        else ussd = Convert.ToInt32(strUssd);

        string fullName = txtFullName.Text;

        rptList.DataSource = new TopupMobileLog().GetTable(top, UserIDs, txtTelco.SelectedValue, RequestNo, Mobile, Amount, creatTime, status, priority, orderNo, topuptype, isConfirm, fullName, ussd, string.Empty);
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

    protected string SearchUrl(string rNo)
    {
        return Constant.ADMIN_PATH + Resources.Url.TopupSearch + "?r=" + rNo;
    }

    protected string DellUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.TopupDelete + "?id=" + id;
    }

    protected bool VisableDel(int Amoumt, int AmountUser)
    {
        return Amoumt == AmountUser;
    }

    protected string TopupTypeDetail(int topupType)
    {
        switch (topupType)
        {
            case 1:
                return "Trả trước";
            case 2:
                return "Trả sau";
            case 3:
                return "Cước internet";
            case 4:
                return "Smas";
            case 5:
                return "Điện thoại cố định";
            case 6:
                return "Nhà thuốc";
            case 7:
                return "Tiêm Chủng";
            case 8:
                return "ShopOne";
            case 9:
                return "Nạp Game I";
            case 11:
                return "Nạp Game II";
            case 10:
                return "Metro Wan/Leased line";
            case 12:
                return "Nạp Game III";
            case 13:
                return "Nạp Game IV";
            case 14:
                return "Nạp Game V";
            case 15:
                return "Nạp Game VI";
            case 16:
                return "Nạp Game M VII";
            case 17:
                return "Nạp Game M VIII";
            default:
                return "Không xác định";

        }
    }

    protected string StatusDetail(int status)
    {
        switch (status)
        {
            case 1:
                return "Đợi xử lý";
            case 2:
                return "Đang xử lý";
            case 3:
                return "Đã hoàn thành";
            case 0:
                return "Không sử dụng";
            case -1:
                return "Bỏ qua";
            case -2:
                return "Telco khóa";
            case -3:
                return "Đợi nạp";
            case -4:
                return "Telco hết lượt";
            case -6:
                return "Dừng để Review";
            default:
                return "Không xác định";

        }
    }

    protected string StateDetail(int state)
    {
        switch (state)
        {
            case 0:
                return "Chưa chốt";
            case 1:
                return "Đã chốt";
            default:
                return "Không xác định";

        }
    }

    protected string PriorityDetail(int status)
    {
        if (0 <= status && status <= 50)
        {
            return "P" + status;
        }
        else
        {
            return "Không xác định";
        }
    }

    protected void rptList_ItemCommand(object source, RepeaterCommandEventArgs e)
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

            int transactionId = Convert.ToInt32(e.CommandArgument.ToString());
            var transactioOrderExport = new TopupMobileLog().GetTableTransactionExport(userIds, string.Empty, transactionId, null, null);
            transactioOrderExport.TableName = "TranHistory";
            if (transactioOrderExport != null)
            {
                new Utils().ExportedExcel(transactioOrderExport, transactionId.ToString(), this.Response);
            }

        }
    }
}