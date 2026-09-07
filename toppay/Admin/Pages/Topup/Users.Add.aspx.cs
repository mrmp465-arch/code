using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Utils;
using System.Data;

public partial class Pages_Topup_Users_Add : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.TopupUsersAdd);
        if (IsPostBack) return;
        Unit();
    }
    private void BindRoles(Users _User)
    {
        var LstRoles = new Roles().GetList().Where(e => e.Group == 1);
        DataTable table = new DataTable();
        table.Columns.Add("RoleId", typeof(int));
        table.Columns.Add("Url", typeof(string));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Group", typeof(int));
        table.Columns.Add("GroupName", typeof(string));
        table.Columns.Add("IsCheck", typeof(bool));

        if (LstRoles != null)
        {
            if (!AppUtils.IsAdmin)
                LstRoles = LstRoles.Where(e => !e.Url.ToLower().Contains(Resources.Url.TopupUsersAdd) && !e.Url.ToLower().Contains(Resources.Url.TopupUsersEdit) && !e.Url.ToLower().Contains(Resources.Url.TopupUsersList) && !e.Url.ToLower().Contains(Resources.Url.TopupUsersDelete)).ToList();
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
    protected void Unit()
    {
       
        Title = Title + " - Thêm mới người dùng";
        var _User = new Users();
        _User = _User.Get(AppUtils.UserID);

        var LstProviders = new Providers().GetList(7); // Chỉ lấy gạch thẻ
        DataTable tablepn = new DataTable();
        tablepn.Columns.Add("ProviderId", typeof(int));
        tablepn.Columns.Add("ProviderCode", typeof(string));
        tablepn.Columns.Add("Name", typeof(string));
        tablepn.Columns.Add("IsCheck", typeof(bool));
        if (LstProviders != null)
        {
            LstProviders = LstProviders.Where(e => e.Status == 1).ToList();
            var LstUserProvider = new UsersProvider().GetList();
            if (LstUserProvider == null)
                LstUserProvider = new List<UsersProvider>();
            foreach (var item in LstProviders)
            {
                var temp = LstUserProvider.FirstOrDefault(e => e.ProviderId == item.ProviderId && e.UserId == _User.UserID);
                if (temp != null)
                {
                    tablepn.Rows.Add(item.ProviderId, item.ProviderCode, item.Name, true);

                }
                else //if (LstUserPartner.Count(e => e.PartnerId == item.PartnerID) == 0)
                    tablepn.Rows.Add(item.ProviderId, item.ProviderCode, item.Name, false);
            }
        }

        rptList.DataSource = tablepn;
        rptList.DataBind();


        var _UserRole = new Users();
        BindRoles(_UserRole);
    }

    protected void btAdd_Click(object sender, EventArgs e)
    {
        var _User = new Users();
        _User.UserName = txtUserName.Text.Trim().ToLower();
        _User.FullName = txtFullName.Text.Trim();
        _User.Password = Encrypts.MD5(txtPassword.Text);
        _User.Status = Convert.ToInt32(chkIsActive.Checked);  
        _User.IsTopup = 1;
        _User.IsAdmin= 0;
        _User.IsPartner= 0;
        _User.ParentId = AppUtils.UserID;
        _User.Add();   

        var LstProviders = new Providers().GetList(7);
        if (LstProviders != null)
        {
            //LstPartners = LstPartners.Where(a => a.Status == 1).ToList();
            var LstUserProvider = new UsersProvider().GetList();
            if (LstUserProvider == null)
                LstUserProvider = new List<UsersProvider>();
            for (int i = 0; i < rptList.Items.Count; i++)
            {

                CheckBox IsCheck = (CheckBox)rptList.Items[i].FindControl("cbxIsCheck");
                HiddenField txtProviderId = (HiddenField)rptList.Items[i].FindControl("txtProviderId");
                HiddenField txtProviderCode = (HiddenField)rptList.Items[i].FindControl("txtProviderCode");
                UsersProvider item = LstUserProvider.FirstOrDefault(a => a.UserId == _User.UserID && a.ProviderId == Convert.ToInt32(txtProviderId.Value));
                if (IsCheck.Checked)
                {
                    if (item == null)
                    {
                        item = new UsersProvider()
                        {
                            UserId = _User.UserID,
                            ProviderId = Convert.ToInt32(txtProviderId.Value),
                            ProviderCode = txtProviderCode.Value,
                        };
                        item.Add();
                    }
                }
                else
                if (item != null)
                    item.Delete();
            }
        }
        updateRoles(_User);
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.TopupUsersEdit);
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.TopupUsersList);
    }
    private void updateRoles(Users _User)
    {

        var LstRoles = new Roles().GetList();
        if (LstRoles != null)
        {
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
                            Url = txtUrl.Value
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
    public string Group = "";
    public int Index = 0;
    public string Checked = ".attr('Checked','Checked');";
    public string GetHtmlGroup(object group, object obj1, object check)
    {
        if (!(bool)check)
            Checked = ".removeAttr('Checked');";
        if (string.IsNullOrEmpty(Group))
        {
            Group = obj1.ToString();
            return "<h4><input id='CheckAll" + group + "' data-value='" + group + "' class='CheckAllBlock' type='checkbox' >" + Group + " </h4><div>" + "<script type='text/javascript'>$('#CheckAll" + group + "')" + Checked + "</script>";
        }
        else if (obj1.ToString() != Group)
        {
            if ((bool)check)
                Checked = ".attr('Checked','Checked');";

            Group = obj1.ToString();
            if (Index++ == 3)
                return "</div></div><div style='clear:both;'></div><div class='col-md-3 col-sm-6 col-xs-12' style='padding-bottom: 10px;display:table-cell'>" + "<h4> <input id='CheckAll" + group + "'  data-value='" + group + "' class='CheckAllBlock'  type='checkbox'   >" + Group + "</h4><div>" + "<script type='text/javascript'>$('#CheckAll" + group + "')" + Checked + "</script>";
            else
                return "</div></div><div class='col-md-3 col-sm-12 col-xs-12' style='padding-bottom: 10px;display:table-cell'>" + "<h4><input id='CheckAll" + group + "'  data-value='" + group + "' class='CheckAllBlock'  type='checkbox'   >" + Group + " </h4><div>" + "<script type='text/javascript'>$('#CheckAll" + group + "')" + Checked + "</script>";
        }
        else
            return "<script type='text/javascript'>$('#CheckAll" + group + "')" + Checked + "</script>";
    }
}