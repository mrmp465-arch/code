using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using System.ComponentModel;

public partial class Pages_Security_Users_List : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = Title + " - Danh sách người dùng"; 
        AppUtils.CheckRoles(Resources.Url.UsersList); 
        if (!IsPostBack)
        {
            BindData();
        }
    }
    
    private void BindData()
    {
        var lst = new Users().GetList(); 
        if (lst != null) 
            lst = lst.Where(e => e.ParentId == AppUtils.UserID || e.UserID == AppUtils.UserID).ToList(); 
        rptList.DataSource = AppUtils.ToDataTable(lst);
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
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.UsersAdd);
    }
    
}
