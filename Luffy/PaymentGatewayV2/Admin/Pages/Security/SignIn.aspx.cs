using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Utils;

public partial class Pages_Security_SignIn : System.Web.UI.Page
{
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) return;
    }

    protected void btSignIn_Click(object sender, EventArgs e)
    {
        Session.RemoveAll();
        Users _User = new Users();

        txtUserName.Text = txtUserName.Text.Trim().ToLower();
        _User.UserName = txtUserName.Text;
        _User.Password = Encrypts.MD5(txtPassword.Text);

        if (!_User.Authentication(_User.UserName, _User.Password))
        {
            PanelMessage.Visible = true;
            lblMessage.Text = "Mật khẩu hoặc tên đăng nhập không hợp lệ";
            txtUserName.Focus();
            return;
        }
        _User= _User.Get();

        Session["IsAdmin"]= Convert.ToBoolean(_User.IsAdmin);
        Session["IsPartner"] = Convert.ToBoolean(_User.IsPartner);
        Session["IsProvider"] = Convert.ToBoolean(_User.IsProvider);
        Session["IsTopup"] = Convert.ToBoolean(_User.IsTopup); 
        Session.Timeout = 120;
        Session["UserID"] = _User.UserID;        
        Session["UserName"] = _User.UserName;
        Session["AccessKey"] = _User.AccessKey;
        Session["FullName"] = _User.FullName;
        Session["LastestTime"] = _User.LastestTime.ToString();

        var partners = new Partners();
        var lstPartners = partners.GetListByUserId(_User.UserID);
        AppUtils.PartnerUser = lstPartners;

        var providers = new Providers();
        var lstProviders = providers.GetListByUserId(_User.UserID);
        AppUtils.ProviderUser = lstProviders;

       // NLogLogger.Info(new string[] { "SignIn", AppUtils.UserName, "Providers", serializer.Serialize(lstProviders), "IsProvider: " + _User.IsProvider });

        string url = Request["u"];
        url = url == null ? Constant.ADMIN_PATH + "" : url;

        //NLogLogger.Info(new string[] { "SignIn", AppUtils.UserName, DateTime.Now.ToString() });

        Response.Redirect(url);
    }
}