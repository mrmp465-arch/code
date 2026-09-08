using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Utils;
using System.Data;

public partial class Pages_Topup_Users_Edit : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    { 
        AppUtils.CheckRoles(Resources.Url.TopupUsersEdit);
        if (IsPostBack) return;
        init();
    }

    private void init()
    {
        var _User = new Users();
        _User = _User.Get(Convert.ToInt32(AppUtils.Request("id")));
        if (_User == null) Response.Redirect(Constant.ADMIN_PATH + Resources.Url.TopupUsersList);

        txtUserName.Text = _User.UserName;
        txtFullName.Text = _User.FullName;
        chkIsActive.Checked = Convert.ToBoolean(_User.Status);  
       
        BindRoles(_User);
    }
    private void BindRoles(Users _User)
    {

        //var LstPartners = new Partners().GetList();
        //DataTable tablepn = new DataTable();
        //tablepn.Columns.Add("PartnerId", typeof(int));
        //tablepn.Columns.Add("PartnerCode", typeof(string));
        //tablepn.Columns.Add("Name", typeof(string));
        //tablepn.Columns.Add("IsCheck", typeof(bool));
        //if (LstPartners != null)
        //{
        //    LstPartners = LstPartners.Where(e => e.Status == 1).ToList();
        //    var LstUserPartner = new UserPartner().GetList();
        //    if (LstUserPartner == null)
        //        LstUserPartner = new List<UserPartner>();
        //    foreach (var item in LstPartners)
        //    {
        //        var temp = LstUserPartner.FirstOrDefault(e => e.PartnerId == item.PartnerID && e.UserId == _User.UserID);
        //        if (temp != null)
        //        {
        //            tablepn.Rows.Add(item.PartnerID, item.PartnerCode, item.Name, true);

        //        }
        //        else //if (LstUserPartner.Count(e => e.PartnerId == item.PartnerID) == 0)
        //            tablepn.Rows.Add(item.PartnerID, item.PartnerCode, item.Name, false);
        //    }
        //}

        //rptListPartner.DataSource = tablepn;
        //rptListPartner.DataBind();

        var LstRoles = new Roles().GetList().Where(e=>e.Group==1);
        DataTable table = new DataTable();
        table.Columns.Add("RoleId", typeof(int));
        table.Columns.Add("Url", typeof(string));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Group", typeof(int));
        table.Columns.Add("GroupName", typeof(string));
        table.Columns.Add("IsCheck", typeof(bool));
    
        if (LstRoles != null)
        {
            if (!AppUtils.IsAdmin )
                LstRoles=LstRoles.Where(e => !e.Url.ToLower().Contains(Resources.Url.TopupUsersAdd) && !e.Url.ToLower().Contains(Resources.Url.TopupUsersEdit) && !e.Url.ToLower().Contains(Resources.Url.TopupUsersList) && !e.Url.ToLower().Contains(Resources.Url.TopupUsersDelete)).ToList();
            
            var LstUserRoles = new UsersRole().GetList();
            if (LstUserRoles == null)
                LstUserRoles = new List<UsersRole>();

            if (!AppUtils.IsAdmin)
            {
                var RolebyUser = new UsersRole().GetListByUser(AppUtils.UserID);
                if (RolebyUser != null && RolebyUser.Count > 0)
                {
                    var Ids = new List<int>();
                    Ids = RolebyUser.Select(x => x.RoleId).ToList();
                    LstRoles = LstRoles.Where(x => Ids.Contains(x.Id)).ToList();
                }
                else
                    LstRoles = new List<Roles>();
            }
            foreach (var item in LstRoles)
            {
                var temp = LstUserRoles.FirstOrDefault(e => e.RoleId == item.Id && e.UserId == _User.UserID);
                if (temp != null)
                    table.Rows.Add(item.Id, item.Url, item.Name, item.Group, item.GroupName, true);
                else
                    table.Rows.Add(item.Id, item.Url, item.Name, item.Group, item.GroupName, false);
            }
        }
        rptListRoles.DataSource = table;
        rptListRoles.DataBind();
    }

    protected void btUpdate_Click(object sender, EventArgs e)
    {
        var _User = new Users() { UserID = Convert.ToInt32(AppUtils.Request("id")) };
        _User = _User.Get();

        if (txtPassword.Text.Length > 0)
        {
            _User.Password = Encrypts.MD5(txtPassword.Text);
        }
        _User.UserName = txtUserName.Text.Trim();
        _User.FullName = txtFullName.Text;
        _User.Status = Convert.ToInt32(chkIsActive.Checked); 
        _User.IsTopup = 1;        
        _User.Update(); 
         
        updateRoles(_User);
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.TopupUsersList);
    }
    private void updateRoles(Users _User)
    {  

        var LstRoles = new Roles().GetList();
        if (LstRoles != null)
        {
            LstRoles = LstRoles.Where(e => !e.Url.ToLower().Contains(Resources.Url.TopupUsersAdd) && !e.Url.ToLower().Contains(Resources.Url.TopupUsersEdit) && !e.Url.ToLower().Contains(Resources.Url.TopupUsersList) && !e.Url.ToLower().Contains(Resources.Url.TopupUsersDelete)).ToList();
            LstRoles = LstRoles.Where(a => a.Status == 1).ToList();

            var LstUsersRole = new UsersRole().GetList();
            if (LstUsersRole == null)
                LstUsersRole = new List<UsersRole>();
            for (int i = 0; i < rptListRoles.Items.Count; i++)
            {

                CheckBox IsCheck = (CheckBox)rptListRoles.Items[i].FindControl("cbxIsCheckRole");
                HiddenField txtRoleId = (HiddenField)rptListRoles.Items[i].FindControl("txtRoleId");
                HiddenField txtUrl = (HiddenField)rptListRoles.Items[i].FindControl("txtUrl");
                UsersRole item = LstUsersRole.FirstOrDefault(a => a.UserId == _User.UserID && a.RoleId == Convert.ToInt32(txtRoleId.Value));
                if (IsCheck.Checked)
                {
                    if (item == null)
                    {
                        item = new UsersRole()
                        {
                            UserId = _User.UserID,
                            RoleId = Convert.ToInt32(txtRoleId.Value),
                            UserName = _User.UserName,
                            Url= txtUrl.Value
                        };
                        item.Add();
                    }
                }
                else
                    if (item != null)
                    item.Delete();
            }
        }
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.TopupUsersList);
    }
    public string Group = "";
    public int Index = 0;
    public string Checked = ".attr('Checked','Checked');";
    public string GetHtmlGroup(object group, object GroupName, object check)
    {
        if (!(bool)check )
            Checked = ".removeAttr('Checked');";  
        if (string.IsNullOrEmpty(Group))
        {
            Group = group.ToString();
            return "<h4><input id='CheckAll"+ group + "' data-value='"+group+"' class='CheckAllBlock' type='checkbox' >" + GroupName + " </h4><div>"+ "<script type='text/javascript'>$('#CheckAll" + group + "')" + Checked + "</script>";
        }
        else if (group.ToString() != Group ) {
            if ((bool)check) 
                Checked = ".attr('Checked','Checked');";
           
            Group = group.ToString(); 
            if (Index++ ==3)
                return "</div></div><div style='clear:both;'></div><div class='col-md-3 col-sm-6 col-xs-12' style='padding-bottom: 10px;display:table-cell'>" + "<h4> <input id='CheckAll" + group + "'  data-value='" + group + "' class='CheckAllBlock'  type='checkbox'   >" + GroupName + "</h4><div>"+ "<script type='text/javascript'>$('#CheckAll" + group + "')" + Checked + "</script>";
            else
                return "</div></div><div class='col-md-3 col-sm-12 col-xs-12' style='padding-bottom: 10px;display:table-cell'>" + "<h4><input id='CheckAll" + group + "'  data-value='" + group + "' class='CheckAllBlock'  type='checkbox'   >" + GroupName + " </h4><div>" + "<script type='text/javascript'>$('#CheckAll" + group + "')" + Checked + "</script>";  
        }
        else 
            return "<script type='text/javascript'>$('#CheckAll"+ group + "')"+ Checked + "</script>";  
    }
}