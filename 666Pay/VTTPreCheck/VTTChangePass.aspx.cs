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
    public partial class VTTChangePass : System.Web.UI.Page
    {
        private static bool isStar = true;
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        private static string ProxyServer = ConfigurationManager.AppSettings["Proxy_Server"] ?? "zproxy.lum-superproxy.io:22225";
        private static string ProxyUserName = ConfigurationManager.AppSettings["Proxy_User_Name"] ?? "lum-customer-hl_37347aa4-zone-datacenter-country-vn";
        private static string ProxyPass = ConfigurationManager.AppSettings["Proxy_Pass"] ?? "wud8xp4slx75";

        private static string PassPrefix = ConfigurationManager.AppSettings["Pass_Prefix"] ?? "a";
        private static string PassSuffix = ConfigurationManager.AppSettings["Pass_Suffix"] ?? "@b";
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

                var deviceId = Utils.GenDeviceId();
                string url = string.Format("https://apivtp.vietteltelecom.vn:6768/myviettel.php/loginV2?device_name={0}&version_app=3.11&build_code=158&os_type=android", deviceId);

                var parameters = new Dictionary<string, string>();
                parameters.Add("username", account.AccountName);
                parameters.Add("password", account.Password);
                parameters.Add("actionForm", "mob");
                parameters.Add("device_name", deviceId);
                parameters.Add("device_id", deviceId);
                parameters.Add("os_type", "0");
                parameters.Add("os_version", "26");
                parameters.Add("app_version", "157");
                parameters.Add("imei", deviceId);
                parameters.Add("model", deviceId);
                parameters.Add("app_id", "com.vttm.vietteldiscovery");

                int tryAgain = 0;
                while (tryAgain < 5)
                {

                    NLogLogger.Info(new string[] { account.AccountName, account.Password });

                    try
                    {

                        var res = Task.Run(async () => await Utils.PostTask(url, parameters)).Result;
                        NLogLogger.Info(new string[] { account.AccountName, "res: ", Regex.Unescape(res) });
                        var response = serializer.Deserialize<LoginResponse>(res);
                        if (response != null)
                        {
                            switch (response.errorCode)
                            {

                                case "0":
                                    //txtResult.Text = txtResult.Text + " ==> Login Ok \r\n";
                                    var newPass = PassPrefix + account.AccountName.Trim() + PassSuffix;
                                    //var newPass = "Queanh999";
                                    NLogLogger.Info(new string[] { account.AccountName, "==> Login Ok" });


                                    var change = MyViettelService.ChangePass(account.AccountName, account.Password, newPass, response.data.data.token);
                                    if (change)
                                    {
                                        account.Password = newPass;
                                        account.LastChangePass = DateTime.Now;
                                    }

                                    //Check 136
                                    //var change = false;
                                    //if (response.data.data.productCode == "LTEASY")
                                    //{
                                    //    account.Type = 2;
                                    //    change = MyViettelService.ChangePass(account.AccountName, account.Password, newPass, response.data.data.token);
                                    //    if (change) { account.Password = newPass; }
                                    //}

                                    account.ProductCode = response.data.data.productCode;
                                    account.LastToken = response.data.data.token;
                                    account.Status = 1;
                                    account.Update();
                                    NLogLogger.Info(new string[] { account.AccountName, "==> Change Pass to: " + newPass + " is " + change });
                                    tryAgain = 5;
                                    break;
                                case "2": //sai MK
                                    account.Status = 0;
                                    account.Update();
                                    //txtResult.Text = txtResult.Text + " ==> Login Fail (Sai mật khẩu) \r\n";
                                    NLogLogger.Info(new string[] { account.AccountName, "==> Login Fail (Sai mật khẩu)" });
                                    tryAgain = 5;
                                    break;
                                case "105": //Tài khoản của bạn tạm thời bị khóa
                                case "5":
                                    account.Status = -4;
                                    account.Update();
                                    NLogLogger.Info(new string[] { account.AccountName, "==> Login Fail (Tài khoản của bạn tạm thời bị khóa)" });
                                    //txtResult.Text = txtResult.Text + " ==> Login Fail (Tài khoản của bạn tạm thời bị khóa) \r\n";
                                    tryAgain = 5;
                                    break;
                                case "-4":
                                    account.Status = -6;
                                    account.Update();
                                    NLogLogger.Info(new string[] { account.AccountName, "==> Login Fail (Cần thay đổi mật khẩu để đăng nhập)" });
                                    //txtResult.Text = txtResult.Text + " ==> Login Fail (Tài khoản của bạn tạm thời bị khóa) \r\n";
                                    tryAgain = 5;
                                    break;

                                case "1": //Thông tin loại tài khoản không hợp lệ - Tài khoản hoặc mật khẩu không đúng
                                    account.Status = -2;
                                    account.Update();
                                    //txtResult.Text = txtResult.Text + " ==> Login Fail (Thông tin loại tài khoản không hợp lệ - Tài khoản hoặc mật khẩu không đúng) \r\n";
                                    NLogLogger.Info(new string[] { account.AccountName, "==> Login Fail (Thông tin loại tài khoản không hợp lệ - Tài khoản hoặc mật khẩu không đúng)" });
                                    tryAgain = 5;
                                    break;

                                default:
                                    //txtResult.Text = txtResult.Text + " ==> Login Fail (Không dõ nguyên nhân) \r\n";
                                    NLogLogger.Info(new string[] { account.AccountName, "==> Login Fail (Không dõ nguyên nhân)" });
                                    tryAgain = 5;
                                    break;

                            }


                        }
                    }
                    catch (Exception exp)
                    {
                        tryAgain++;
                        NLogLogger.Info(new string[] { "==> exp", exp.Message });

                    }
                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            var account = new Account()
            {
                AccountName = "347577284",
                Password = "a010890",
                LastToken = "0194C6A3-0FC8-6CBF-241C-ED6668856F51"
            };

            var logout = MyViettelService.Logout(account.AccountName, account.LastToken);
        }
    }
}