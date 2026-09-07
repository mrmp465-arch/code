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
    public int IsUserAdmin;
    public int IsUserPartner;
    public int IsUserToup;
    public decimal ckBank;
    public decimal ckBankOut;
    public decimal rwBank = 0;
    public decimal rwBankout = 0;
    public bool RoleUpadte { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.UsersEdit);
        RoleUpadte = AppUtils.CheckRolesPermission("pages/security/users.editrole.aspx");
        if (IsPostBack) return;
        init();
    }

    private void init()
    {
        var lstGroup = new PartnerGroup().GetList();
        lstGroup = lstGroup.OrderBy(x => x.Name).ToList();
        drpOrder.DataSource = lstGroup;
        drpOrder.DataTextField = "Name";
        drpOrder.DataValueField = "Name";
        drpOrder.DataBind();
        //drpOrder.Items.Insert(1, new ListItem("inhouse:", "inhouse"));
        drpOrder.Items.Insert(0, new ListItem("other", "other"));
        // drpOrder.Items.Insert(0, new ListItem("Nhóm:", ""));

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
        drpOrder.SelectedValue = _User.Source;
        IsUserAdmin = _User.IsAdmin;
        IsUserPartner = _User.IsPartner;
        IsUserToup = _User.IsTopup;
        chkWithdraw.Checked = Convert.ToBoolean(_User.Withdraw);
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
        if (IsUserPartner == 1)
        {
            var listpartnerDiscount = new PartnersDiscount().GetList(_User.UserName, 2030, 1);
            if (listpartnerDiscount != null)
            {
                if (listpartnerDiscount.Exists(x => x.Date.Day == 1))
                {
                    var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == 1);

                    txtDiscountBANKOUTTRANFER.Text = _partnerDiscount.DiscountBANKOUTTRANFER.ToString();
                    txtDiscountBANKTRANFER.Text = _partnerDiscount.DiscountBANKTRANFER.ToString();
                    hdDiscountBANKOUTTRANFER.Value = _partnerDiscount.DiscountBANKOUTTRANFER.ToString();
                    hdDiscountBANKTRANFER.Value = _partnerDiscount.DiscountBANKTRANFER.ToString();

                    txtRewardBANKOUTTRANFER.Text = _partnerDiscount.RewardBANKOUTTRANFER.ToString();
                    txtRewardBANKTRANFER.Text = _partnerDiscount.RewardBANKTRANFER.ToString();
                    hdRewardBANKOUTTRANFER.Value = _partnerDiscount.RewardBANKOUTTRANFER.ToString();
                    hdRewardBANKTRANFER.Value = _partnerDiscount.RewardBANKTRANFER.ToString();


                    txtDiscountMOMOOUT.Text = _partnerDiscount.DiscountMOMOOUT.ToString();
                    txtDiscountMOMO.Text = _partnerDiscount.DiscountMOMO.ToString();
                    hdDiscountMOMOOUT.Value = _partnerDiscount.DiscountMOMOOUT.ToString();
                    hdDiscountMOMO.Value = _partnerDiscount.DiscountMOMO.ToString();

                    txtRewardMOMOOUT.Text = _partnerDiscount.RewardMOMOOUT.ToString();
                    txtRewardMOMO.Text = _partnerDiscount.RewardMOMO.ToString();


                    txtDiscountVTT.Text = ((float)_partnerDiscount.DiscountVTT * 100).ToString();
                    hdDiscountVTT.Value = ((float)_partnerDiscount.DiscountVTT * 100).ToString();

                }
                else
                {
                    txtDiscountBANKOUTTRANFER.Text = "0.0000";
                    txtDiscountBANKTRANFER.Text = "0.0000";
                    txtDiscountMOMOOUT.Text = "0.0000";
                    txtDiscountMOMO.Text = "0.0000";

                    txtRewardBANKOUTTRANFER.Text = "0.0000";
                    txtRewardBANKTRANFER.Text = "0.0000";
                    hdRewardBANKOUTTRANFER.Value = "0.0000";
                    hdRewardBANKTRANFER.Value = "0.0000";


                    hdDiscountBANKOUTTRANFER.Value = "0.0000";
                    hdDiscountBANKTRANFER.Value = "0.0000";
                    hdDiscountMOMOOUT.Value = "0.0000";
                    hdDiscountMOMO.Value = "0.0000";

                    txtDiscountVTT.Text = "0.0000";
                    hdDiscountVTT.Value = "0.0000";

                    txtRewardMOMOOUT.Text = "0.0000";
                    txtRewardMOMO.Text = "0.0000";
                }
            }
            else
            {
                txtDiscountBANKOUTTRANFER.Text = "0.0000";
                txtDiscountBANKTRANFER.Text = "0.0000";
                txtDiscountMOMOOUT.Text = "0.0000";
                txtDiscountMOMO.Text = "0.0000";

                txtRewardBANKOUTTRANFER.Text = "0.0000";
                txtRewardBANKTRANFER.Text = "0.0000";
                hdRewardBANKOUTTRANFER.Value = "0.0000";
                hdRewardBANKTRANFER.Value = "0.0000";
                txtRewardMOMOOUT.Text = "0.0000";
                txtRewardMOMO.Text = "0.0000";

                hdDiscountBANKOUTTRANFER.Value = "0.0000";
                hdDiscountBANKTRANFER.Value = "0.0000";
                hdDiscountMOMOOUT.Value = "0.0000";
                hdDiscountMOMO.Value = "0.0000";

                txtDiscountVTT.Text = "0.0000";
                hdDiscountVTT.Value = "0.0000";

            }

            var _Partner = new Partners().Get(_User.UserName);
            txtSMSCommand.Text = _Partner.SMSCommand;
            txtSMSPlusUrl.Text = _Partner.SMSPlusUrl;
            txtPartnerCode.Text = _Partner.PartnerCode;
            txtPartnerKey.Text = _Partner.PublicKey;
            txtUserConfirm.Text = _Partner.SMSPlusCheckUrl;
            txtRequestType.Text = _Partner.RequestType.ToString();

            var _PartnerService = new PartnerService();
            List<PartnerService> list = _PartnerService.GetList(_Partner.PartnerID, 0);
            var bankService = list.FirstOrDefault(x => x.ServiceID == 13);
            var bankoutService = list.FirstOrDefault(x => x.ServiceID == 18);

            if (bankService.CommandCode.Contains("bank"))
            {
                ddlBankInEnable.Checked = true;
            }
            if (bankoutService.CommandCode.Contains("bank"))
            {
                ddlBankCashEnable.Checked = true;
            }
            txtAPIIP.Text = bankoutService.IPAddress;
        }
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
        if (_User.IsAdmin == 1)
        {
            _User.Source = "inhouse";
        }
        else
        {
            _User.Source = drpOrder.SelectedValue;
        }
        _User.Withdraw = Convert.ToInt32(chkWithdraw.Checked);
        //_User.F2a = txtDeposit.Text;
        _User.Update();
        if (_User.IsPartner == 1)
        {
            //cập nhật kết nối
            var _PartnerService = new PartnerService();

            var _Partner = new Partners().Get(_User.UserName);
            //if (_Partner.Status != _User.Status)
            //{
            _Partner.Status = _User.Status;
            _Partner.SMSUrl = drpOrder.SelectedValue;
            _Partner.SMSPlusUrl = txtSMSPlusUrl.Text.Trim();
            _Partner.SMSCommand = txtSMSCommand.Text.Trim();
            _Partner.SMSPlusCheckUrl = txtUserConfirm.Text.Trim();
            _Partner.RequestType = int.Parse(txtRequestType.Text.Trim());
            _Partner.Update();
            //}
            List<PartnerService> list = _PartnerService.GetList(_Partner.PartnerID, 0);
            var bankService = list.FirstOrDefault(x => x.ServiceID == 13);
            var bankoutService = list.FirstOrDefault(x => x.ServiceID == 18);

            bankService.CommandCode = "";
            bankoutService.CommandCode = "";
            if (ddlBankInEnable.Checked)
            {
                bankService.CommandCode += ",bank";
            }
            if (ddlBankCashEnable.Checked)
            {
                bankoutService.CommandCode += ",bank";
            }
            bankoutService.IPAddress = txtAPIIP.Text;
            bankoutService.Update();
            bankService.Update();

            decimal DiscountBANKTRANFER;
            bool isDecimal = decimal.TryParse(txtDiscountBANKTRANFER.Text, out DiscountBANKTRANFER);

            decimal DiscountBANKOUTTRANFER;
            bool isDecimal2 = decimal.TryParse(txtDiscountBANKOUTTRANFER.Text, out DiscountBANKOUTTRANFER);

            decimal DiscountMOMO;
            bool isDecimal3 = decimal.TryParse(txtDiscountMOMO.Text, out DiscountMOMO);
            decimal DiscountMOMOOUT;
            bool isDecimal4 = decimal.TryParse(txtDiscountMOMOOUT.Text, out DiscountMOMOOUT);


            decimal DiscountVTT;
            bool isDecimal5 = decimal.TryParse(txtDiscountVTT.Text, out DiscountVTT);
            DiscountVTT = DiscountVTT / 100;

            decimal RWBANKTRANFER;
            bool isDecimal6 = decimal.TryParse(txtRewardBANKTRANFER.Text, out RWBANKTRANFER);

            decimal RWBANKOUTTRANFER;
            bool isDecimal7 = decimal.TryParse(txtRewardBANKOUTTRANFER.Text, out RWBANKOUTTRANFER);


            decimal RWMOMO;
            bool isDecimal8 = decimal.TryParse(txtRewardMOMO.Text, out RWMOMO);
            decimal RWMOMOOUT;
            bool isDecimal9 = decimal.TryParse(txtRewardMOMOOUT.Text, out RWMOMOOUT);


            if (!isDecimal || !isDecimal2 || !isDecimal3 || !isDecimal4 || !isDecimal5)
            {
                AlertBans.Text = "Chiết khấu không hợp lệ";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertBan').modal()}); ", true);
                return;
            }

            if (DiscountBANKTRANFER < 0 || DiscountBANKOUTTRANFER < 0 || DiscountMOMO < 0 || DiscountMOMOOUT < 0)
            {
                AlertBans.Text = "Chiết khấu phải >0";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertBan').modal()}); ", true);
                return;
            }

            var obj = new PartnersDiscount();
            obj.PartnerCode = _User.UserName;
            obj.Date = new DateTime(2030, 1, 1);
            obj.DiscountVTT = DiscountVTT;
            obj.RewardVTT = 0;
            obj.DiscountVNP = DiscountVTT;
            obj.RewardVNP = 0;
            obj.DiscountVMS = DiscountVTT;
            obj.RewardVMS = 0;

            obj.DiscountZING = 0;
            obj.RewardZING = 0;
            obj.DiscountBANKTRANFER = DiscountBANKTRANFER;
            obj.RewardBANKTRANFER = RWBANKTRANFER;
            obj.DiscountMOMO = DiscountMOMO;
            obj.RewardMOMO = RWMOMO;
            obj.DiscountVTTOUT = 0;
            obj.RewardVTTOUT = 0;
            obj.DiscountBANKOUTTRANFER = DiscountBANKOUTTRANFER;
            obj.RewardBANKOUTTRANFER = RWBANKOUTTRANFER;
            obj.DiscountMOMOOUT = DiscountMOMOOUT;
            obj.RewardMOMOOUT = RWMOMOOUT;

            obj.RewardVNPOUT = obj.RewardVTTOUT;
            obj.DiscountVNPOUT = obj.DiscountVTTOUT;
            obj.RewardVMSOUT = obj.RewardVTTOUT;
            obj.DiscountVMSOUT = obj.DiscountVTTOUT;

            obj.DiscountGATE = 0;
            obj.RewardGATE = 0;

            //check xem có update không
            if (rckUpdate.Checked)
            {
                obj.Add();
                obj.DeleteCache(_User.UserName);
                obj.DeleteCache("");
                var _userLog2 = new UserLog
                {
                    UserName = AppUtils.UserName,
                    Action = "partnereditck",
                    ActionName = "Cập nhật chiếu khấu tài khoản",
                    Description = "Cập nhật chiếu khấu tài khoản " + obj.PartnerCode + " " + String.Format("{0}|{1}|{2}|{3}", obj.DiscountBANKTRANFER, obj.DiscountMOMO, obj.DiscountBANKOUTTRANFER, obj.DiscountMOMOOUT)
                };
                _userLog2.Add();
                //NLogLogger.Info("update ck");
            }

        }
        if (_User.IsTopup == 1)
        {
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
        }




        if (RoleUpadte)
        {
            updateRoles(_User);
        }
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