using Libs.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BankGateV2.Pages
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btSignIn_Click(object sender, EventArgs e)
        {
            Session.RemoveAll();
            Users _User = new Users();

            txtUserName.Text = txtUserName.Text.Trim().ToLower();
            _User.UserName = txtUserName.Text;
            _User.Password = txtPassword.Text;

            if (_User.UserName=="hdvtest01"&& _User.Password == "hdvtest01")
            {
                Session["UserName"] = _User.UserName;
                HttpContext.Current.Response.Redirect("/pages/Nap.aspx");
                return;
            }
            else
            {
                PanelMessage.Visible = true;
                lblMessage.Text = "Thôgn tin đăng nhập không hợp lệ ";
            }
           
        }
    }
}