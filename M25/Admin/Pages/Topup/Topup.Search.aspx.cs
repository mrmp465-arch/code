using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;

public partial class Pages_topup_search : System.Web.UI.Page
{
    public List<Roles> lstRole;
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.TopupSearch);
        if (!IsPostBack)
        {
            txtCreatTime.Text = DateTime.Now.AddDays(1).ToString();
            var mobile = Request["m"];
            if (!string.IsNullOrEmpty(mobile))
            {

                txtMobile.Text = mobile;
                BindData();
            }

            var requestNo = Request["r"];
            if (!string.IsNullOrEmpty(requestNo))
            {
                txtRequestNo.Text = requestNo;
                BindData();
            }
        }
    }
    protected void BindData()
    {

        int Top = Convert.ToInt32(drpTop.SelectedValue);
        DateTime creatTime = AppUtils.ToDateTime(txtCreatTime.Text);

        string UserIDs = string.Empty;
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

        string mobile = txtMobile.Text;
        var cardSerial = txtCardSerial.Text;
        var cardCode = txtCardCode.Text;


        rptList.DataSource = new TopupMobileTransactionLog().GetTable(Top, UserIDs, "", txtRequestNo.Text, mobile, creatTime, null, cardSerial, cardCode, null, null);
        rptList.DataBind();
    }

    public string viewMenu(bool obj, string txt)
    {
        if (obj)
            return txt;
        else
            return "";
    }
    public string viewStatus(int obj, string txt)
    {
        if (obj == 1)
            return txt;
        else
            return "";
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