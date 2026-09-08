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
            AlertBans.Text = "Mật khẩu cũ không chính xác!";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertBan').modal()}); ", true);
            return;
        }

        _User.Password = Encrypts.MD5(txtPassword.Text.Trim());
        _User.Update();
        AlertSuccesss.Text = "Đổi mật khẩu thành công!";
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertSuccess').modal()}); ", true); 
    }

}