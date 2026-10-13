using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;

public partial class Pages_Security_Roles_Edit : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.RolesEdit);
        if (!IsPostBack)
        {
            init();
        }
    }
    private void loadRoles()
    {
        var lst = new Roles().GetList();
        if (lst != null)
            lst = lst.Where(x => x.ParentId == 0 && x.Group == Convert.ToInt32(txtGoup.SelectedValue)).ToList();
        if (lst != null&& lst.Any())
        {
            txtParentId.DataSource = lst;
            
            txtParentId.DataTextField = "Name";
            txtParentId.DataValueField = "Id";
            txtParentId.DataBind();

        }
       
        txtParentId.Items.Insert(0, new ListItem("Trang chính:", "0"));

    }
    private void init()
    {
        var _Roles = new Roles() { Id = Convert.ToInt32(AppUtils.Request("id")) };
        _Roles = _Roles.Get();

        if (_Roles == null)
        {
            Response.Redirect(Resources.Url.RolesList);
        }
        txtName.Text=_Roles.Name ;
        txtDescription.Text=_Roles.Description;
        txtUrl.Text=_Roles.Url;
        txtGoup.SelectedValue = _Roles.Group.ToString();
       
        txtIsMenu.Checked = _Roles.IsMenu;
        txtOrderNo.Text= _Roles.OrderNo.ToString(); 
        txtStatus.Text= _Roles.Status.ToString();
        loadRoles();
        txtParentId.Text = _Roles.ParentId.ToString();
    }

    protected void btUpdate_Click(object sender, EventArgs e)
    {
        var _Roles = new Roles() { Id = Convert.ToInt32(AppUtils.Request("id")) };
        _Roles = _Roles.Get();
        if (_Roles != null)
        {
            _Roles.Name = txtName.Text;
            _Roles.Code = "";
            _Roles.Description = txtDescription.Text;
            _Roles.Url = txtUrl.Text;
            _Roles.Group = Convert.ToInt32(txtGoup.SelectedValue);
            _Roles.GroupName = txtGoup.SelectedItem.Text;
            _Roles.IsMenu = txtIsMenu.Checked;
            _Roles.ParentId = Convert.ToInt32(txtParentId.SelectedValue);
            try
            {
                if (!string.IsNullOrEmpty(txtOrderNo.Text))
                    _Roles.OrderNo = Convert.ToInt32(txtOrderNo.Text);
                else
                    _Roles.OrderNo = 0;
            }
            catch (Exception) { }
            _Roles.Status = Convert.ToInt32(txtStatus.SelectedValue);
            _Roles.Update();
        }
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.RolesList);
    } 
    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.RolesList);
    }
    protected void txtGoup_SelectedIndexChanged(object sender, EventArgs e)
    {
        loadRoles();
    }
}