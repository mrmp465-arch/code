using DocumentFormat.OpenXml.Wordprocessing;
using Libs.API;
using Libs.Utils;
using ServiceStack.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Telegram.Bot.Types;

public partial class Pages_Security_User_Topup : System.Web.UI.Page
{
    public string UserName { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.UsersHistory);
        if (!IsPostBack)
        {
            init();
        }
    }
    private void init()
    {
        UserName = Request["name"];
    }
    protected void btAdd_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtDescription.Text) || string.IsNullOrEmpty(txtAmount.Text))
        {
            AlertInfoss.Text = "Bạn chưa nhập đủ thông tin";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }
        var name = Request["name"];
        string desc = txtDescription.Text + " thực hiện bời " + AppUtils.UserName;
        long amount = long.Parse(txtAmount.Text);
        if(amount>0)
        {



            TelegramClient.SendTeleV2("-4762440012", "Cộng tiền  " + amount.ToString("#,#").Replace(",", ".") + " cho user " + name + " Từ tài khoản " + AppUtils.UserName);

            new Users().Topup(amount, name, name, desc, AppUtils.UserName);
            AlertSuccesss.Text = "Cộng tiền thành công";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);

            //Log User
            var _userLog = new UserLog
            {
                UserName = AppUtils.UserName,
                Action = "usertopup",
                ActionName = "Cộng tiền người dùng",
                Description = "Cộng tiền  " + amount.ToString("#,#").Replace(",", ".") + " cho người dùng " + name 
            };
            _userLog.Add();
            System.Threading.Thread.Sleep(2000);
            txtAmount.Text = "";
            txtDescription.Text = "";
            Response.Redirect(Constant.ADMIN_PATH + Resources.Url.UsersList);
        }
        else
        {
            amount = amount * -1;

            TelegramClient.SendTeleV2("-4762440012", "Trừ tiền  " + amount.ToString("#,#").Replace(",", ".") + " cho user " + name + " Từ tài khoản " + AppUtils.UserName);

            var result =new Users().Deduct(amount, name, name, desc, AppUtils.UserName);
            if(result > 0)
            {
                AlertSuccesss.Text = "Trừ tiền thành công";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);

                //Log User
                var _userLog = new UserLog
                {
                    UserName = AppUtils.UserName,
                    Action = "userdeduct",
                    ActionName = "Trừ tiền người dùng",
                    Description = "Trừ tiền  " + amount.ToString("#,#").Replace(",", ".") + " cho người dùng " + name
                };
                _userLog.Add();
                System.Threading.Thread.Sleep(2000);
                txtAmount.Text = "";
                txtDescription.Text = "";
                Response.Redirect(Constant.ADMIN_PATH + Resources.Url.UsersList);
            }    
           
        }    

       
    }
}