using Libs.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_Topup_Topup_DeviceV2 : System.Web.UI.Page
{
    public string Url { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtUsers.DataSource = new Users().GetListTopupBySort(AppUtils.IsAdmin, AppUtils.IsTopup, AppUtils.UserID);
            txtUsers.DataBind();
            txtUsers.DataTextField = "UserName";
            txtUsers.DataValueField = "UserName";
            txtUsers.DataBind();
            txtUsers.Items.Insert(0, new ListItem("Tài khoản:", ""));
            getUrl(AppUtils.UserName);
        }
    }
    private void getUrl(string UserName)
    {
        switch (UserName)
        {
            case "chu_dl1":
                Url = "http://45.77.36.46:8083/";
                break;
            default:
                Url = "http://14.232.245.188:1584/";
                break;
        }
    }

    protected void txtUsers_SelectedIndexChanged(object sender, EventArgs e)
    {
        getUrl(txtUsers.SelectedValue);
    }
}