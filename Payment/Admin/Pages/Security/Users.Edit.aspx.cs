using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Utils;
using System.Data;
using Libs.Report;

public partial class Pages_Security_Users_Edit : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.UsersEdit);
        if (IsPostBack) return;
        init();
    }

    private void init()
    {
        var _User = new Users();
        _User = _User.Get(Convert.ToInt32(AppUtils.Request("id")));
        if (_User == null) Response.Redirect(Constant.ADMIN_PATH + Resources.Url.UsersList);

        txtUserName.Text = _User.UserName;
        txtFullName.Text = _User.FullName;
        chkIsActive.Checked = Convert.ToBoolean(_User.Status);
        cbxIsAdmin.Checked = Convert.ToBoolean(_User.IsAdmin);
        cbxIsPartner.Checked = Convert.ToBoolean(_User.IsPartner);
        cbxIsTopup.Checked = Convert.ToBoolean(_User.IsTopup);
        cbxIsProvider.Checked = Convert.ToBoolean(_User.IsProvider);
        txtIp.Text = _User.Ip;
        //txtDeposit.Text = _User.F2a.ToString();

        var LstPartners = new Partners().GetList();
        DataTable table = new DataTable();
        table.Columns.Add("PartnerId", typeof(int));
        table.Columns.Add("PartnerCode", typeof(string));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("IsCheck", typeof(bool));
        if (LstPartners != null)
        {
            LstPartners = LstPartners.Where(e => e.Status == 1).ToList();
            var LstUserPartner = new UserPartner().GetList();
            if (LstUserPartner == null)
                LstUserPartner = new List<UserPartner>();
            foreach (var item in LstPartners)
            {
                var temp = LstUserPartner.FirstOrDefault(e => e.PartnerId == item.PartnerID && e.UserId == _User.UserID);
                if (temp != null)
                {
                    table.Rows.Add(item.PartnerID, item.PartnerCode, item.Name, true);

                }
                else //if (LstUserPartner.Count(e => e.PartnerId == item.PartnerID) == 0)
                    table.Rows.Add(item.PartnerID, item.PartnerCode, item.Name, false);
            }
        }

        rptListPartner.DataSource = table;
        rptListPartner.DataBind();


        var LstProviders = new Providers().GetList(7);
        DataTable tablep = new DataTable();
        tablep.Columns.Add("ProviderId", typeof(int));
        tablep.Columns.Add("ProviderCode", typeof(string));
        tablep.Columns.Add("Name", typeof(string));
        tablep.Columns.Add("IsCheck", typeof(bool));

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
                    tablep.Rows.Add(item.ProviderId, item.ProviderCode, item.Name, true);

                }
                else //if (LstUserPartner.Count(e => e.PartnerId == item.PartnerID) == 0)
                    tablep.Rows.Add(item.ProviderId, item.ProviderCode, item.Name, false);
            }

        }

        rptListProvider.DataSource = tablep;
        rptListProvider.DataBind();

        var LstProviders3 = new Providers().GetList(15);
        DataTable tablep3 = new DataTable();
        tablep3.Columns.Add("ProviderId", typeof(int));
        tablep3.Columns.Add("ProviderCode", typeof(string));
        tablep3.Columns.Add("Name", typeof(string));
        tablep3.Columns.Add("IsCheck", typeof(bool));

        if (LstProviders3 != null)
        {
            LstProviders3 = LstProviders3.Where(e => e.Status == 1).ToList();
            var LstUserProvider3 = new UsersProvider().GetList();
            if (LstUserProvider3 == null)
                LstUserProvider3 = new List<UsersProvider>();
            foreach (var item in LstProviders3)
            {
                var temp = LstUserProvider3.FirstOrDefault(e => e.ProviderId == item.ProviderId && e.UserId == _User.UserID);
                if (temp != null)
                {
                    tablep3.Rows.Add(item.ProviderId, item.ProviderCode, item.Name, true);

                }
                else //if (LstUserPartner.Count(e => e.PartnerId == item.PartnerID) == 0)
                    tablep3.Rows.Add(item.ProviderId, item.ProviderCode, item.Name, false);
            }

        }
        rptListProvider3.DataSource = tablep3;
        rptListProvider3.DataBind();

        var LstProviders2 = new Providers().GetList(13);
        DataTable tablep2 = new DataTable();
        tablep2.Columns.Add("ProviderId", typeof(int));
        tablep2.Columns.Add("ProviderCode", typeof(string));
        tablep2.Columns.Add("Name", typeof(string));
        tablep2.Columns.Add("IsCheck", typeof(bool));

        if (LstProviders2 != null)
        {
            LstProviders2 = LstProviders2.Where(e => e.Status == 1).ToList();
            var LstUserProvider2 = new UsersProvider().GetList();
            if (LstUserProvider2 == null)
                LstUserProvider2 = new List<UsersProvider>();
            foreach (var item in LstProviders2)
            {
                var temp = LstUserProvider2.FirstOrDefault(e => e.ProviderId == item.ProviderId && e.UserId == _User.UserID);
                if (temp != null)
                {
                    tablep2.Rows.Add(item.ProviderId, item.ProviderCode, item.Name, true);

                }
                else //if (LstUserPartner.Count(e => e.PartnerId == item.PartnerID) == 0)
                    tablep2.Rows.Add(item.ProviderId, item.ProviderCode, item.Name, false);
            }

        }

        rptListProvider2.DataSource = tablep2;
        rptListProvider2.DataBind();

        BindRoles(_User);
    }
    private void BindRoles(Users _User)
    {
        var LstRoles = new Roles().GetList();
        DataTable table = new DataTable();
        table.Columns.Add("RoleId", typeof(int));
        table.Columns.Add("Url", typeof(string));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Group", typeof(int));
        table.Columns.Add("GroupName", typeof(string));
        table.Columns.Add("IsCheck", typeof(bool));

        if (LstRoles != null)
        {
            LstRoles = LstRoles.OrderBy(e => e.Group).ToList();
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
        _User.IsAdmin = Convert.ToInt32(cbxIsAdmin.Checked);
        _User.IsPartner = Convert.ToInt32(cbxIsPartner.Checked);
        _User.IsTopup = Convert.ToInt32(cbxIsTopup.Checked);
        _User.IsProvider = Convert.ToInt32(cbxIsProvider.Checked);
        _User.Ip = txtIp.Text.Trim();
        //_User.F2a = txtDeposit.Text;
        _User.Update();

        var LstPartners = new Partners().GetList();
        if (LstPartners != null)
        {
            //LstPartners = LstPartners.Where(a => a.Status == 1).ToList();
            var LstUserPartner = new UserPartner().GetList();
            if (LstUserPartner == null)
                LstUserPartner = new List<UserPartner>();
            for (int i = 0; i < rptListPartner.Items.Count; i++)
            {

                CheckBox IsCheck = (CheckBox)rptListPartner.Items[i].FindControl("cbxIsCheckPartner");
                HiddenField txtPartnerId = (HiddenField)rptListPartner.Items[i].FindControl("txtPartnerId");
                HiddenField txtPartnerCode = (HiddenField)rptListPartner.Items[i].FindControl("txtPartnerCode");
                UserPartner item = LstUserPartner.FirstOrDefault(a => a.UserId == _User.UserID && a.PartnerId == Convert.ToInt32(txtPartnerId.Value));
                if (IsCheck.Checked)
                {
                    if (item == null)
                    {
                        item = new UserPartner()
                        {
                            UserId = _User.UserID,
                            PartnerId = Convert.ToInt32(txtPartnerId.Value),
                            PartnerCode = txtPartnerCode.Value,
                        };
                        item.Add();
                    }
                }
                else
                    if (item != null)
                    item.Delete();
            }
        }


        var LstProviders = new Providers().GetList(7);
        var LstProviders2 = new Providers().GetList(12);
        if (LstProviders != null)
        {
            //LstPartners = LstPartners.Where(a => a.Status == 1).ToList();
            var LstUserProvider = new UsersProvider().GetList();
            if (LstUserProvider == null)
                LstUserProvider = new List<UsersProvider>();
            for (int i = 0; i < rptListProvider.Items.Count; i++)
            {

                CheckBox IsCheck = (CheckBox)rptListProvider.Items[i].FindControl("cbxIsCheckProvider");
                HiddenField txtProviderId = (HiddenField)rptListProvider.Items[i].FindControl("txtProviderId");
                HiddenField txtProviderCode = (HiddenField)rptListProvider.Items[i].FindControl("txtProviderCode");
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
                {
                    item.Delete();
                }
                   
            }

            for (int i = 0; i < rptListProvider3.Items.Count; i++)
            {

                CheckBox IsCheck = (CheckBox)rptListProvider3.Items[i].FindControl("cbxIsCheckProvider");
                HiddenField txtProviderId = (HiddenField)rptListProvider3.Items[i].FindControl("txtProviderId");
                HiddenField txtProviderCode = (HiddenField)rptListProvider3.Items[i].FindControl("txtProviderCode");
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
                {
                    item.Delete();
                }

            }


            for (int i = 0; i < rptListProvider2.Items.Count; i++)
            {

                CheckBox IsCheck = (CheckBox)rptListProvider2.Items[i].FindControl("cbxIsCheckProvider");
                HiddenField txtProviderId = (HiddenField)rptListProvider2.Items[i].FindControl("txtProviderId");
                HiddenField txtProviderCode = (HiddenField)rptListProvider2.Items[i].FindControl("txtProviderCode");
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
                {
                    item.Delete();
                }

            }

            //Update cho Account Parnter
            if (_User.IsTopup == 1)
            {

                var providerCodes = string.Empty;

                var lstProvider = new Providers().GetListByUserId(_User.UserID);
                if (lstProvider != null && lstProvider.Count > 0)
                {
                    providerCodes = string.Join(",", lstProvider.Select(i => i.ProviderCode).ToArray());
                }
                //new TopupMobileLog().UpdateProviders(providerCodes, _User.UserID);

                var Lstusers = new Users().GetListTopupBySort(false, true, _User.UserID); // Lấy toàn bộ ID DL_2 & DL_1
                foreach (var userDl2 in Lstusers)
                {
                    new TopupMobileLog().UpdateProviders(providerCodes, userDl2.UserID);
                }
            }
        }

        updateRoles(_User);
        //Log User
        var _userLog = new UserLog
        {
            UserName = AppUtils.UserName,
            Action = "userupdate",
            ActionName = "Cập nhật người dùng",
            Description = "Cập nhật người dùng " + _User.UserName
        };
        _userLog.Add();
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.UsersList);
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

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.UsersList);
    }
    public string Group = "";
    public int Index = 0;
    public string Checked = ".attr('Checked','Checked');";
    public string GetHtmlGroup(object group, object GroupName, object check)
    {
        if (!(bool)check)
            Checked = ".removeAttr('Checked');";
        if (string.IsNullOrEmpty(Group))
        {
            Group = group.ToString();
            return "<h4><input id='CheckAll" + group + "' data-value='" + group + "' class='CheckAllBlock' type='checkbox' >" + GroupName + " </h4><div>" + "<script type='text/javascript'>$('#CheckAll" + group + "')" + Checked + "</script>";
        }
        else if (group.ToString() != Group)
        {
            if ((bool)check)
                Checked = ".attr('Checked','Checked');";

            Group = group.ToString();
            if (Index++ == 3)
                return "</div></div><div style='clear:both;'></div><div class='col-md-3 col-sm-6 col-xs-12' style='padding-bottom: 10px;display:table-cell'>" + "<h4> <input id='CheckAll" + group + "'  data-value='" + group + "' class='CheckAllBlock'  type='checkbox'   >" + GroupName + "</h4><div>" + "<script type='text/javascript'>$('#CheckAll" + group + "')" + Checked + "</script>";
            else
                return "</div></div><div class='col-md-3 col-sm-12 col-xs-12' style='padding-bottom: 10px;display:table-cell'>" + "<h4><input id='CheckAll" + group + "'  data-value='" + group + "' class='CheckAllBlock'  type='checkbox'   >" + GroupName + " </h4><div>" + "<script type='text/javascript'>$('#CheckAll" + group + "')" + Checked + "</script>";
        }
        else
            return "<script type='text/javascript'>$('#CheckAll" + group + "')" + Checked + "</script>";
    }
}