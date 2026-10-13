using System;
using System.Linq;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;

public partial class Pages_topup_topup_Monitor : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.TopupMonitor);

        if (!IsPostBack)
        {
            init();
            BindData();
        }

    }

    private void init()
    {
        //var _Products = new Products();
        //txtTelco.DataSource = _Products.GetList(1, null);
        //txtTelco.DataBind();
        //txtTelco.DataTextField = "Name";
        //txtTelco.DataValueField = "Code";
        //txtTelco.DataBind();
        //txtTelco.Items.Insert(0, new ListItem("Telco:", ""));
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
        bool erro = false;
        int? status = null;
        var requestNo = Request["r"];
        var qOrderNo = Request["o"];
        

        if (!string.IsNullOrEmpty(qOrderNo))
        {
            txtOrderNo.Text = qOrderNo;
        }
        var qStatus = Request["s"];
        if (!string.IsNullOrEmpty(qStatus))
        {
            txtStatus.Text = qStatus;
        }

        if (txtStatus.Text.ToLower() != "all")
            status = AppUtils.ToInt32(txtStatus.Text, out erro);
        if (erro)
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfos').html('Chưa nhập Status'); $('#AlertInfo').modal()}); ", true);
            return;
        }

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
       
        string Mobile = "";

        DateTime creatTime = AppUtils.ToDateTime(txtCreatTime.Text);
        rptList.DataSource = new TopupMobileTransactionLog().GetTable(top, UserIDs, txtTelco.SelectedValue, requestNo, Mobile, creatTime, status, string.Empty, string.Empty,  txtOrderNo.Text, drpEngine.SelectedValue);
        rptList.DataBind();
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
    }

    protected string DetailUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.TopupMonitorDetail + "?id=" + id;
    }

    protected string SearchUrl(string id, string rNo)
    {
        return Constant.ADMIN_PATH + Resources.Url.TopupSearch + "?m=" + id + "&r=" + rNo;
    }

}