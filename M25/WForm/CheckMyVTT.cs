using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using APIMyViettel;
using APIMyViettel.Entity;

namespace WForm
{
    public partial class CheckMyVTT : Form
    {
        private static bool isStar = true;
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        static HttpClient Client = new HttpClient(new HttpClientHandler() { UseCookies = false });
        public CheckMyVTT()
        {
            
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

            if (!isStar) isStar = true;
            while (isStar)
            {
                //Doing
                var account = new Account();
                account = new Account().GetAccountCheck(0);
                if (account == null)
                {
                    isStar = false;
                    txtResult.Text = txtResult.Text = "Hết tài khoàn My VTT \r\n";
                }

                Client.DefaultRequestHeaders.Clear();
                Client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");

                txtResult.Text = txtResult.Text + "Start check: " + account.AccountName;

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
                    try
                    {
                        var res = Task.Run(async () => await Utils.PostTask(url, parameters)).Result;
                        var response = serializer.Deserialize<LoginResponse>(res);
                        if (response != null)
                        {
                            switch (response.errorCode)
                            {
                                case "0":
                                    txtResult.Text = txtResult.Text + " ==> Login Ok \r\n";
                                    tryAgain = 5;
                                    break;
                                case "2": //sai MK
                                    account.Status = 0;
                                    account.Update();
                                    txtResult.Text = txtResult.Text + " ==> Login Fail (Sai mật khẩu) \r\n";
                                    tryAgain = 5;
                                    break;
                                case "105": //Tài khoản của bạn tạm thời bị khóa
                                case "5":
                                    account.Status = -4;
                                    account.Update();
                                    txtResult.Text = txtResult.Text + " ==> Login Fail (Tài khoản của bạn tạm thời bị khóa) \r\n";
                                    tryAgain = 5;
                                    Thread.Sleep(31000);
                                    break;

                                case "1": //Thông tin loại tài khoản không hợp lệ - Tài khoản hoặc mật khẩu không đúng
                                    account.Status = -2;
                                    account.Update();
                                    txtResult.Text = txtResult.Text + " ==> Login Fail (Thông tin loại tài khoản không hợp lệ - Tài khoản hoặc mật khẩu không đúng) \r\n";
                                    tryAgain = 5;
                                    break;

                                default:
                                    txtResult.Text = txtResult.Text + " ==> Login Fail (Không dõ nguyên nhân) \r\n";
                                    tryAgain = 5;
                                    break;

                            }

                            txtResult.SelectionStart = txtResult.Text.Length;
                            txtResult.ScrollToCaret();

                        }
                    }
                    catch (Exception exp)
                    {
                        tryAgain++;
                        txtResult.Text = txtResult.Text + " ==> exp " + exp.Message + "\r\n"; // + exp.StackTrace + "\r\n";
                        //isStar = false;
                    }
                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            isStar = false;
        }
    }
}
