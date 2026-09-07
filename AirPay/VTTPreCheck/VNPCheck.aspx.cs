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
using APIMyVNTP;
using APIMyVNTP.Entity;
using Libs.Utils;
using Account = APIMyVNTP.Account;

namespace VTTPreCheck
{
    public partial class VNPCheck : System.Web.UI.Page
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
                var account = new Account().GetAccountCheck(0);
                if (account == null)
                {
                    isStar = false;
                    NLogLogger.Info(new string[] { "Hết tài khoàn My VNP" });
                    //txtResult.Text = txtResult.Text = "Hết tài khoàn My VTT \r\n";
                }

                if (!account.AccountName.StartsWith("0") && account.AccountName.Length < 10)
                {
                    account.AccountName = "0" + account.AccountName;
                }

                if (account.AccountName.Length == 10)
                {
                    account.AccountName = Regex.Replace(account.AccountName, "^0", "84");
                }
                // Login
                try
                {
                    //var loginAppObjResponse = new MyVNTPAppLoginResponse { error_code = "-99" };
                    //while (loginAppObjResponse.error_code != "0")
                    //{
                        var loginRequest = new MyVNTPAppLoginResquest()
                        {
                            device_info = "SM-G532G",
                            fcm_registration_token = "dUGtz-1rH1E:APA91bFxAcOps6MHiwc6chGn-RTydfW2NsHpcV6pJ1kLKliVW9XD3SF6OHRq4lyojpzsKeH6z8aHjWOExZr_wiogh4lrB1lutS7nyQB5GsExHNo4Pe5BF2GQPZTqw5DfYZQGPNZ1BzN6",
                            mode = "password",
                            msisdn = account.AccountName,
                            password = Encrypts.MD5(account.Password).ToUpper()
                        };
                        var loginAppResponse = Task.Run(() => UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/authen_msisdn", serializer.Serialize(loginRequest))).Result;
                        NLogLogger.Info(new string[] { "VNTP Check", "Login response", account.AccountName, account.Password, loginAppResponse });
                        if (!string.IsNullOrEmpty(loginAppResponse))
                        {
                            var loginAppObjResponse = serializer.Deserialize<MyVNTPAppLoginResponse>(loginAppResponse);
                            //Kiểm tra xem có login được không
                            switch (loginAppObjResponse.error_code)
                            {
                                case "0":
                                    account.Status = 1;
                                    account.Update();
                                    NLogLogger.Info(new string[] { account.AccountName, "==> Login Ok" });
                                    break;
                                case "1":
                                case "2":
                                    account.Status = -1; // Khóa không login được có thể là sai pass
                                    account.Update();
                                    NLogLogger.Info(new string[] { account.AccountName, "==> Login Fail (Sai mật khẩu)" });
                                    break;
                                default:
                                    account.Status = -1; // Khóa không login được
                                    account.Update();
                                    NLogLogger.Info(new string[] { account.AccountName, "==> Login Fail (Un handler)" });
                                    break;

                            }
                        }


                    //}
                }
                catch (Exception exp)
                {
                    NLogLogger.Info(new string[] { "==> exp", exp.Message });
                }
                Thread.Sleep(1000);

            }
        }
    }
}