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
    public partial class VTTCheck : System.Web.UI.Page
    {
        private static bool isStar = true;
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
     
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnStart_Click(object sender, EventArgs e)
        {
            if (!isStar) isStar = true;
            while (isStar)
            {
                //Doing

                var account = new Account();
                account = new Account().GetAccountCheck(0);

                //var account = new Account()
                //{
                //    AccountName = "0342787745",
                //    Password = "1234567a"
                //};


                if (account == null)
                {
                    isStar = false;
                    NLogLogger.Info(new string[] { "Hết tài khoàn My VTT" });
                    //txtResult.Text = txtResult.Text = "Hết tài khoàn My VTT \r\n";
                }
              

                //txtResult.Text = txtResult.Text + "Start check: " + account.AccountName;
                

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
                    //var proxy = Utils.GenProxy();
                    //var Client = new HttpClient(new HttpClientHandler() { UseCookies = false, UseProxy = true, Proxy = new WebProxy(proxy) });

                    //var session_id = new Random().Next().ToString();
                    //var credentials = new NetworkCredential(ProxyUserName + "-session-" + session_id, ProxyPass);
                    //var handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials) };
                    //var client = new HttpClient(handler);
                    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                    //client.DefaultRequestHeaders.Clear();
                    ////client.DefaultRequestHeaders.Connection.Add("Keep-Alive");
                    //client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");
                    //client.Timeout = TimeSpan.FromSeconds(15);

                    NLogLogger.Info(new string[] { account.AccountName, account.Password});

                    try
                    {
                        //var res = Task.Run(async () => await Utils.PostTask(url, parameters, Client)).Result;
                        var res = Task.Run(async () => await Utils.PostTask(url, parameters)).Result;
                        NLogLogger.Info(new string[] { account.AccountName, "res: ", Regex.Unescape(res) });
                        var response = serializer.Deserialize<LoginResponse>(res);
                        if (response != null)
                        {
                            switch (response.errorCode)
                            {
                                case "0":
                                    //txtResult.Text = txtResult.Text + " ==> Login Ok \r\n";
                                    NLogLogger.Info(new string[] { account.AccountName, "==> Login Ok"});
                                    account.Status = 1;
                                    account.Update();
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

                                    if (!response.message.Contains("Có lỗi trong quá trình thực hiện"))
                                    {
                                        account.Status = -2;
                                        account.Update();
                                        //txtResult.Text = txtResult.Text + " ==> Login Fail (Thông tin loại tài khoản không hợp lệ - Tài khoản hoặc mật khẩu không đúng) \r\n";
                                        NLogLogger.Info(new string[] {account.AccountName, "==> Login Fail (Thông tin loại tài khoản không hợp lệ - Tài khoản hoặc mật khẩu không đúng)"});
                                        tryAgain = 5;
                                    }

                                    break;

                                default:
                                    //txtResult.Text = txtResult.Text + " ==> Login Fail (Không dõ nguyên nhân) \r\n";
                                    NLogLogger.Info(new string[] { account.AccountName, "==> Login Fail (Không dõ nguyên nhân)" });
                                    tryAgain = 5;
                                    break;

                            }

                            //txtResult.SelectionStart = txtResult.Text.Length;
                            //txtResult.ScrollToCaret();

                            // scroll down!
                            //string scriptKey = "TextBoxScrollDownScript";
                            //string javaScript = string.Format("scrollTextBoxDown('{0}');", txtResult.ClientID);
                            //ScriptManager.RegisterStartupScript(Page, this.GetType(), scriptKey, javaScript, true);
                         
                        }
                    }
                    catch (Exception exp)
                    {
                        tryAgain++;
                        //txtResult.Text = txtResult.Text + " ==> exp " + exp.Message + "\r\n"; // + exp.StackTrace + "\r\n";
                        //NLogLogger.Info(new string[] { "==> exp", exp.Message, proxy });
                        NLogLogger.Info(new string[] { "==> exp", exp.Message});
                        //isStar = false;
                    }
                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
            }
        }
    }
}