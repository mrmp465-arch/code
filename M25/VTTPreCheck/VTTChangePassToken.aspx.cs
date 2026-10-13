using APIMyViettel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using APIMyViettel.Entity;
using Libs.Utils;

namespace VTTPreCheck
{
    public partial class VTTChangePassToken : System.Web.UI.Page
    {
        private static bool isStar = true;
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        private static string ProxyServer = ConfigurationManager.AppSettings["Proxy_Server"] ?? "zproxy.lum-superproxy.io:22225";
        private static string ProxyUserName = ConfigurationManager.AppSettings["Proxy_User_Name"] ?? "lum-customer-hl_37347aa4-zone-datacenter-country-vn";
        private static string ProxyPass = ConfigurationManager.AppSettings["Proxy_Pass"] ?? "wud8xp4slx75";
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnStart_Click(object sender, EventArgs e)
        {
            if (!isStar) isStar = true;
            while (isStar)
            {
                //Doing

                //var account = new Account()
                //{
                //    AccountName = "84377095076",
                //    Password = "Zozo68!@sale"
                //};

                var account = new Account();
                account = new Account().GetAccountCheck(0);
                if (account == null)
                {
                    isStar = false;
                    NLogLogger.Info(new string[] { "Hết tài khoàn My VTT" });
                    txtResult.Text = txtResult.Text = "Hết tài khoàn My VTT \r\n";
                    break;
                }

                var change = MyViettelService.ChangePass(account.AccountName, account.Password, "123456a", account.LastToken);

                Thread.Sleep(1000);
            }
        }
    }
}