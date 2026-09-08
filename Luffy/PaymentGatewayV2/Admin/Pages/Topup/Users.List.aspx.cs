using System;
using System.Web.UI.WebControls;
using Libs.API;

public partial class Pages_Topup_Users_List : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = Title + " - Danh sách người dùng"; 
        AppUtils.CheckRoles(Resources.Url.TopupUsersList); 
        if (!IsPostBack)
        {
            BindData();
        }
    }

    private void BindData()
    {
        var Lstusers = new Users().GetListTopupBySort(AppUtils.IsAdmin, AppUtils.IsTopup, AppUtils.UserID);
        rptList.DataSource = AppUtils.ToDataTable(Lstusers);
        rptList.DataBind();
    }
   
    protected void btApply_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            var _User = new Users();
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
            Label labelUserID = (Label)rptList.Items[i].FindControl("lblUserID");
            _User.UserID = Convert.ToInt32(labelUserID.Text);
            _User = _User.Get();
            _User.Status = cbx.Checked ? 1 : 0;
            _User.Update();
        }
        BindData();
    } 

    protected void btAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.TopupUsersAdd);
    }

    protected string DelUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.TopupUsersDelete + "?id=" + id;
    }
}