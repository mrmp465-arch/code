using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Utils;

public partial class Pages_Security_ChangePassword : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckLogin();
       
        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
        btSubmit.Text= Resources.Pay.Change;
        Title = Title + " - Đổi mật khẩu";
        if (!IsPostBack)
        {
            //AppUtils.AddLog("open");
        }
    }

    protected void btSubmit_Click(object sender, EventArgs e)
    {
        //AppUtils.AddLog("update");
        Users _User = new Users() { UserID = AppUtils.UserID };
        _User = _User.Get();
        if (Encrypts.MD5(txtPasswordOld.Text.Trim()) != _User.Password)
        { 
            AlertBans.Text = Resources.Pay.OldPasswordWrong;
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertBan').modal()}); ", true);
            return;
        }

        _User.Password = Encrypts.MD5(txtPassword.Text.Trim());
        _User.Update();
        //Log User
        var _userLog = new UserLog
        {
            UserName = AppUtils.UserName,
            Action = "changepass",
            ActionName = "Đổi mật khẩu",
            Description = "Đổi mật khẩu"
        };

        _userLog.Add();
        AlertSuccesss.Text = Resources.Pay.PasswordSuccess;
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertSuccess').modal()}); ", true); 
    }

}