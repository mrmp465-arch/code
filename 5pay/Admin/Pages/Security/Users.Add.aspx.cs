using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Utils;
using System.Data;
using Libs.Report;

public partial class Pages_Security_Users_Add : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.UsersAdd);
        if (IsPostBack) return;
        Unit();
    }
    //private void BindRoles(Users _User)
    //{
    //    var LstRoles = new Roles().GetList();
    //    DataTable table = new DataTable();
    //    table.Columns.Add("RoleId", typeof(int));
    //    table.Columns.Add("Url", typeof(string));
    //    table.Columns.Add("Name", typeof(string));
    //    table.Columns.Add("Group", typeof(int));
    //    table.Columns.Add("GroupName", typeof(string));
    //    table.Columns.Add("IsCheck", typeof(bool));

    //    if (LstRoles != null)
    //    {
    //        LstRoles = LstRoles.OrderBy(e => e.Group).ToList();
    //        var LstUserRoles = new UsersRole().GetList();
    //        if (LstUserRoles == null)
    //            LstUserRoles = new List<UsersRole>();

    //        if (!AppUtils.IsAdmin)
    //        {
    //            var RolebyUser = new UsersRole().GetListByUser(AppUtils.UserID);
    //            if (RolebyUser != null && RolebyUser.Count > 0)
    //            {
    //                var Ids = new List<int>();
    //                Ids = RolebyUser.Select(x => x.RoleId).ToList();
    //                LstRoles = LstRoles.Where(x => Ids.Contains(x.Id)).ToList();
    //            }
    //            else
    //                LstRoles = new List<Roles>();
    //        }
    //        foreach (var item in LstRoles)
    //        {
    //            var temp = LstUserRoles.FirstOrDefault(e => e.RoleId == item.Id && e.UserId == _User.UserID);
    //            if (temp != null)
    //                table.Rows.Add(item.Id, item.Url, item.Name, item.Group, item.GroupName, true);
    //            else
    //                table.Rows.Add(item.Id, item.Url, item.Name, item.Group, item.GroupName, false);
    //        }
    //    }
    //    rptListRoles.DataSource = table;
    //    rptListRoles.DataBind();
    //}
    protected void Unit()
    {

        Title = Title + " - Thêm mới người dùng";
        var _User = new Users();

        var lstGroup = new PartnerGroup().GetList();
        lstGroup = lstGroup.OrderBy(x => x.Name).ToList();
        drpOrder.DataSource = lstGroup;
        drpOrder.DataTextField = "Name";
        drpOrder.DataValueField = "Name";
        drpOrder.DataBind();
        //drpOrder.Items.Insert(1, new ListItem("inhouse:", "inhouse"));
        //drpOrder.Items.Insert(0, new ListItem("other", "other"));
        // drpOrder.Items.Insert(0, new ListItem("Nhóm:", ""));

        //var LstPartners = new Partners().GetList();
        //DataTable table = new DataTable();
        //table.Columns.Add("PartnerId", typeof(int));
        //table.Columns.Add("PartnerCode", typeof(string));
        //table.Columns.Add("Name", typeof(string));
        //table.Columns.Add("IsCheck", typeof(bool));
        //if (LstPartners != null)
        //{
        //    LstPartners = LstPartners.Where(e => e.Status == 1).ToList();
        //    var LstUserPartner = new UserPartner().GetList();
        //    if (LstUserPartner == null)
        //        LstUserPartner = new List<UserPartner>();
        //    foreach (var item in LstPartners)
        //    {  
        //        //if (LstUserPartner.Count(e => e.PartnerId == item.PartnerID) == 0)
        //            table.Rows.Add(item.PartnerID, item.PartnerCode, item.Name, false);
        //    }
        //}

        //rptList.DataSource = table;
        //rptList.DataBind();
        //BindRoles(_User);
    }

    protected void btAdd_Click(object sender, EventArgs e)
    {
        var _User = new Users();
        _User.UserName = txtUserName.Text.Trim().ToLower();
        _User.FullName = txtFullName.Text.Trim();
        _User.Password = Encrypts.MD5(txtPassword.Text);
        _User.Status = Convert.ToInt32(chkIsActive.Checked);
        _User.IsAdmin = Convert.ToInt32(cbxIsAdmin.Checked);
        _User.IsPartner = Convert.ToInt32(cbxIsPartner.Checked);
        _User.IsProvider = Convert.ToInt32(cbxIsProvider.Checked);
        _User.IsTopup = Convert.ToInt32(cbxIsTopup.Checked);
        _User.ParentId = AppUtils.UserID;
        if (_User.IsAdmin == 1)
        {
            _User.Source = "inhouse";
        }
        else
        {
            _User.Source = drpOrder.SelectedValue;
        }
        _User.Add();


        if (_User.IsPartner == 1)
        {
            //tao partner
            var key = Libs.Utils.Encrypts.MD5(DateTime.Now.ToString("dd/MM/yyy hh:mm:ss"));
            var _Partner = new Partners();
            _Partner.Name = _User.UserName;
            _Partner.PartnerCode = _User.UserName;
            _Partner.Status = 1;
            _Partner.SignatureType = 1;
            _Partner.PrivateKey = key;
            _Partner.PublicKey = key;
            _Partner.SMSCommand = "";
            _Partner.SMSUrl = "";
            _Partner.SMSPlusCommand = "";
            _Partner.SMSPlusUrl = "";
            _Partner.SMSPlusCheckUrl = "";
            _Partner.Hotline = _User.UserName;
            _Partner.Add();

            var item = new UserPartner()
            {
                UserId = _User.UserID,
                PartnerId = _Partner.PartnerID,
                PartnerCode = _User.UserName,
            };

            item.Add();

            var _PartnerService = new PartnerService();
            _PartnerService.IPAddress = "";
            _PartnerService.CommandCode = ",bank,momo";
            _PartnerService.Status = 1;
            _PartnerService.Quota = 0;
            _PartnerService.Occurs = 4;
            _PartnerService.PartnerID = _Partner.PartnerID;
            _PartnerService.ServiceID = 13;
            _PartnerService.Add();

            var _PartnerService2 = new PartnerService();
            _PartnerService2.IPAddress = "";
            _PartnerService2.CommandCode = ",bank,momo";
            _PartnerService2.Status = 1;
            _PartnerService2.Quota = 0;
            _PartnerService2.Occurs = 4;
            _PartnerService2.PartnerID = _Partner.PartnerID;
            _PartnerService2.ServiceID = 18;
            _PartnerService2.Add();

            var _PartnerService3 = new PartnerService();
            _PartnerService3.IPAddress = "";
            _PartnerService3.CommandCode = "";
            _PartnerService3.Status = 1;
            _PartnerService3.Quota = 0;
            _PartnerService3.Occurs = 4;
            _PartnerService3.PartnerID = _Partner.PartnerID;
            _PartnerService3.ServiceID = 7;
            _PartnerService3.Add();
        }
        if (_User.IsPartner == 1 || _User.IsTopup == 1)
        {
            var LstRoles = new Roles().GetList();
            var listRoleId = new List<int> { 84, 20, 21, 23,89, 92, 93, 95,102, 147,138,139,1,2,4 };
            
            //tk mc
            if (_User.IsTopup == 1)
            {
                listRoleId = new List<int> { 84, 20, 21, 23, 89,92, 93, 95,139,1,2,4, 102, 147, 138 };
            }
            foreach (var role in listRoleId)
            {
                var itemRole = new UsersRole
                {
                    UserId = _User.UserID,
                    RoleId = role,
                    Url = LstRoles.FirstOrDefault(x => x.Id == role).Url,
                    UserName = _User.UserName,
                };
                itemRole.Add();

            }
        }

        //Log User
        var _userLog = new UserLog
        {
            UserName = AppUtils.UserName,
            Action = "useradd",
            ActionName = "Thêm người dùng",
            Description = "Thêm người dùng " + _User.UserName
        };

        _userLog.Add();
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.UsersEdit + "?id=" + _User.UserID.ToString());
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.UsersList);
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