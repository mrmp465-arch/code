using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Utils;
using Google.Authenticator;
using System.Threading.Tasks;

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
            lblMessage.Text = "Invalid password or login name";
            txtUserName.Focus();
            return;
        }
        _User = _User.Get();

        if (!string.IsNullOrEmpty(_User.F2a))
        {
            if (string.IsNullOrEmpty(txtOTP.Text))
            {
                PanelMessage.Visible = true;
                lblMessage.Text = "Invalid 2FA authentication code";
                txtUserName.Focus();
                return;
            }
            TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
            bool isValid = TwoFacAuth.ValidateTwoFactorPIN(_User.F2a, txtOTP.Text, false);
            if (!isValid)
            {
                PanelMessage.Visible = true;
                lblMessage.Text = "Invalid 2FA authentication code";
                txtUserName.Focus();
                return;
            }
        }
        if (!string.IsNullOrEmpty(_User.Ip))
        {
            if (!_User.Ip.Contains(GetIP()))
            {
                NLogLogger.Info(new string[] { "SignIn Fail", _User.UserName, DateTime.Now.ToString(), GetIP() });
                PanelMessage.Visible = true;
                lblMessage.Text = "IP đăng nhập không hợp lệ " + GetIP();
                txtUserName.Focus();
                return;
            }
        }
        Session["IsAdmin"] = Convert.ToBoolean(_User.IsAdmin);
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

        if(_User.IsAdmin!=1)
        {
            if(lstPartners.Count()==0)
            {
                NLogLogger.Info(new string[] { "SignIn Fail", AppUtils.UserName, DateTime.Now.ToString(), GetIP() });
                PanelMessage.Visible = true;
                lblMessage.Text = "Tài khoản chưa set cho đối tác nào";
                txtUserName.Focus();
                return;
            }    
        }    

        var providers = new Providers();
        var lstProviders = providers.GetListByUserId(_User.UserID);
        AppUtils.ProviderUser = lstProviders;

        NLogLogger.Info(new string[] { "SignIn", AppUtils.UserName, "Providers", serializer.Serialize(lstProviders), "IsProvider: " + _User.IsProvider });

        string url = Request["u"];
        url = url == null ? Constant.ADMIN_PATH + "" : url;

        NLogLogger.Info(new string[] { "SignIn", AppUtils.UserName, DateTime.Now.ToString() });

        if(_User.IsAdmin==1)
        {
            //Log User
            var _userLog = new UserLog
            {
                UserName = _User.UserName,
                Action = "login",
                ActionName = "Đăng nhập",
                Description = "Đăng nhập"
            };

            _userLog.Add();
        }    
       
        Response.Redirect(url);
    }
    public static string GetIP()
    {
        string IP = "";
        //return IP;
        if (HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"] != null)
        {
            IP = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];
            if (IP.Contains(","))
                return IP.Split(',')[0].Trim();
            return IP;
        }

        if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
        {
            IP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (IP.Contains(","))
                return IP.Split(',')[0].Trim();
            return IP;
        }

        if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED"] != null)
        {
            IP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED"];
            if (IP.Contains(","))
                return IP.Split(',')[0].Trim();
            return IP;
        }

        if (HttpContext.Current.Request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"] != null)
        {
            IP = HttpContext.Current.Request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"];
            if (IP.Contains(","))
                return IP.Split(',')[0].Trim();
            return IP;
        }

        if (HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED_FOR"] != null)
        {
            IP = HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED_FOR"];
            if (IP.Contains(","))
                return IP.Split(',')[0].Trim();
            return IP;
        }

        if (HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED"] != null)
        {
            IP = HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED"];
            if (IP.Contains(","))
                return IP.Split(',')[0].Trim();
            return IP;
        }

        if (IP == "")
        {
            IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }
        if (IP.Contains(","))
            return IP.Split(',')[0].Trim();


        return IP;
    }
}