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
using APIMyVNTP.Entity;
using Lib.Captcha;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APIMyVNTP
{
    public class MyVNTPWebService
    {
        //static readonly string baseUrl = "https://my.vnpt.com.vn/";
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        static bool ReportIncorrectCaptcha = bool.Parse(ConfigurationManager.AppSettings["Report_Incorrect_Captcha"] ?? "true");
        public static APIResponse TopupCard(string cardSerial, string cardCode, string mobile, string cardType, string accountName, string passWord)
        {

            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(passWord))
            {
                return new APIResponse((int)ResponseCode.ServiceIsLocked)
                {
                    Description = "Mobile or AccountName or Password is Empty"
                };
            }

            if (!accountName.StartsWith("0") && accountName.Length < 10)
            {
                accountName = "84" + accountName;

            }
            else
            {
                accountName = Regex.Replace(accountName, "^0", "84");
            }

            var getTopup = new MyVNTPWebCookie();
            var decaptcha = new Captcha().GetCaptcha(8, "vnpw:" + accountName);
            if (decaptcha == null)
            {
                var sid = UtilsMyVNTPWeb.GenSid(accountName);
                getTopup = GetTopup(accountName, passWord);
                if (getTopup.IsTopup)
                {
                    decaptcha = UtilsMyVNTPWeb.DeCaptcha(getTopup.CaptChaLink, sid, getTopup);
                    if (decaptcha == null)
                    {
                        return new APIResponse((int)ResponseCode.ParameterInvalid) { Description = "DeCaptcha Null" };
                    }
                    getTopup.HtmlContent = string.Empty;
                    getTopup.SessionId = sid;
                    UtilsMyVNTPWeb.SetCookieCache(sid, getTopup);
                }
                else
                {
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = getTopup.HtmlContent
                    };
                }

            }
            else
            {
                getTopup = UtilsMyVNTPWeb.GetCookieCache(decaptcha.SessionId);
            }

            if (getTopup != null)
            {

                var res = Topup(cardSerial, cardCode, decaptcha.Value, cardType, getTopup, accountName);
                NLogLogger.Info(new string[] { "MyVNTPWebService", "TopupCard", "Response", accountName, cardSerial, cardCode, res });

                for (int i = 0; i < 1; i++)
                {
                    Action<string, string, MyVNTPWebCookie> send = PreGenCaptCha;
                    send.BeginInvoke(accountName, passWord, getTopup, null, null);
                }

                if (res.Contains("success"))
                {

                    //if (res.Trim() == "success 0")
                    //{
                    //    return new APIResponse((int)ResponseCode.TransactionSuspicious)
                    //    {
                    //        Description = res
                    //    };
                    //}

                    var amount = Convert.ToInt32(Regex.Match(res, @"\d+$").Value.Trim());
                    int[] listValue = { 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000 };

                    if (!listValue.Contains(amount))
                    {
                        return new APIResponse((int)ResponseCode.TransactionSuspicious)
                        {
                            Description = res
                        };
                    }

                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        Description = res,
                        ResponseContent = amount.ToString()
                    };
                }

                if (res.Contains("Post Topup Ignore Null")
                    || res.Contains("Post Topup Ignore Exeption"))
                {
                    return new APIResponse((int)ResponseCode.ParameterInvalid);
                }


                if (res.Contains("Thẻ đã sử dụng hoặc không tồn tại"))
                {
                    return new APIResponse((int)ResponseCode.CardUsed)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Mã Captcha gõ không đúng"))
                {

                    //Cap Comvn
                    if (ReportIncorrectCaptcha)
                    {
                        Action<string, int, string> send = ReportCaptcha;
                        send.BeginInvoke(decaptcha.ImgBase64, 1, decaptcha.Value, null, null);
                    }
                    return new APIResponse((int)ResponseCode.ParameterInvalid)
                    {
                        Description = res
                    };
                }

            }

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public static MyVNTPWebCookie GetTopup(string accountName, string passWord)
        {
            NLogLogger.Info(new string[] { "MyVNTPWebService", "GetTopup", "Request", accountName, passWord });

            //string baseUrl = "https://smas.edu.vn";

            var getLogin = Task.Run(() => UtilsMyVNTPWeb.GetTask("https://id.vinaphone.com.vn/auth/login?service=https%3a%2f%2fmy.vnpt.com.vn%2fAccount%2fLogin", new CookieContainer())).Result;

            //Get lt
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(getLogin.HtmlContent);

            var lt = doc.DocumentNode.SelectSingleNode("//input[@type='hidden' and @name='lt']").Attributes["value"].Value;

            var parameters = new Dictionary<string, string>();
            parameters.Add("username", accountName);
            parameters.Add("password", passWord);
            parameters.Add("lt", lt);
            parameters.Add("_eventId", "submit");
            parameters.Add("dnstb", "Đăng nhập");

            var loginResponse = Task.Run(() => UtilsMyVNTPWeb.PostTask("https://id.vinaphone.com.vn/auth/login?service=https%3a%2f%2fmy.vnpt.com.vn%2fAccount%2fLogOff%3furl%3d%2f", parameters, getLogin.CookieContainer)).Result;

            if (loginResponse.IsTimeout)
            {
                return new MyVNTPWebCookie()
                {
                    IsTopup = false,
                    IsTimeout = true,
                    HtmlContent = HttpUtility.HtmlDecode("Login Timeout")
                };
            }


            doc.LoadHtml(loginResponse.HtmlContent);

            var detectSuccess = loginResponse.HtmlContent.Contains("Th&#244;ng tin t&#224;i khoản");
            if (!detectSuccess)
            {
                NLogLogger.Info(new string[] { "MyVNTPWebService", "Topup", "Error Login", serializer.Serialize(parameters) });
                return new MyVNTPWebCookie()
                {
                    IsTopup = false,
                    HtmlContent = "Tài khoản đăng nhập không đúng"
                };
            }

            var getTopup = Task.Run(() => UtilsMyVNTPWeb.GetTask("https://my.vnpt.com.vn/Payment/AddCard", loginResponse.CookieContainer)).Result;

            if (getTopup == null)
            {
                NLogLogger.Info(new string[] { "MyVNTPWebService", "Topup", "Error Locked 30 min", serializer.Serialize(parameters) });
                return new MyVNTPWebCookie() { IsTopup = false, HtmlContent = "Locked 30 min" }; //Bi khoa 15P
            }

            //doc.LoadHtml(getTopup.HtmlContent);
            //var balaceString = doc.DocumentNode.SelectSingleNode("//span[@class='tk_number']") != null ? doc.DocumentNode.SelectSingleNode("//span[@class='tk_number']").InnerText.Trim() : string.Empty;
            //getTopup.BalanceBefore = Convert.ToDecimal(Regex.Match(balaceString, @"(\d+.)+").Value.Trim());


            var loginRequest = new MyVNTPAppLoginResquest()
            {
                device_info = "SM-G532G",
                fcm_registration_token = "dUGtz-1rH1E:APA91bFxAcOps6MHiwc6chGn-RTydfW2NsHpcV6pJ1kLKliVW9XD3SF6OHRq4lyojpzsKeH6z8aHjWOExZr_wiogh4lrB1lutS7nyQB5GsExHNo4Pe5BF2GQPZTqw5DfYZQGPNZ1BzN6",
                mode = "password",
                msisdn = accountName,
                password = Encrypts.MD5(passWord).ToUpper()
            };
            var loginAppResponse = Task.Run(() => UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/authen_msisdn", serializer.Serialize(loginRequest))).Result;
            if (!string.IsNullOrEmpty(loginAppResponse))
            {
                var loginAppObjResponse = serializer.Deserialize<MyVNTPAppLoginResponse>(loginAppResponse);
                getTopup.SessionApp = loginAppObjResponse.session;

                //var getBalanceRequest = new MyVNTPAppBalanceResquest()
                //{
                //    msisdn = accountName,
                //    session = loginAppObjResponse.session
                //};
                //var getBalanceResponse = Task.Run(() => UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/mobile_IN_balances", serializer.Serialize(getBalanceRequest))).Result;
                //if (!string.IsNullOrEmpty(getBalanceResponse))
                //{
                //    var balanceObj = serializer.Deserialize<MyVNTPAppBalanceResponse>(getBalanceResponse);
                //    foreach (var balacne in balanceObj.result)
                //    {
                //        if (balacne.BALANCE_NAME == "Tài khoản chính")
                //        {
                //            getTopup.BalanceBefore = Convert.ToInt32(balacne.BALANCE);
                //            break;
                //        }
                //    }
                //}
            }
            getTopup.IsTopup = true;

            return getTopup;

        }

        public static string Topup(string cardSerial, string cardCode, string answer, string cardType, MyVNTPWebCookie smasCookie, string accountName)
        {
            var result = string.Empty;
            var parameters = new Dictionary<string, string>();
            MyVNTPWebCookie postTopup = null;
            try
            {
                //int balanceBefore = 0;
                //int balanceAfter = 0;

                var balaceCharge = new BalaceCharge();

                var getBalanceRequest = new MyVNTPAppBalanceResquest()
                {
                    msisdn = accountName,
                    session = smasCookie.SessionApp
                };
                
                
                //var getBalanceResponse = Task.Run(() => UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/mobile_IN_balances", serializer.Serialize(getBalanceRequest))).Result;
                //if (!string.IsNullOrEmpty(getBalanceResponse))
                //{
                //    var balanceObj = serializer.Deserialize<MyVNTPAppBalanceResponse>(getBalanceResponse);
                //    foreach (var balacne in balanceObj.result)
                //    {
                //        if (balacne.BALANCE_NAME == "Tài khoản chính")
                //        {
                //            balanceBefore = Convert.ToInt32(balacne.BALANCE);
                //            break;
                //        }
                //    }
                //}


                var getBalanceResponse = Task.Run(() => UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/mobile_balance_info", serializer.Serialize(getBalanceRequest))).Result;
                if (!string.IsNullOrEmpty(getBalanceResponse))
                {
                    var balanceObj = serializer.Deserialize<BalanceInfoResponse>(getBalanceResponse);
                    foreach (var balacne in balanceObj.balance)
                    {
                        if (balacne.origin_acc_name == "EZPAY_CORE")
                        {
                            balaceCharge.Ezpay_Core_Before = Convert.ToInt32(balacne.value);
                        }

                        if (balacne.origin_acc_name == "Hot Charge")
                        {
                            balaceCharge.Hot_Charge_Before = Convert.ToInt32(balacne.value);
                        }
                    }
                }


                parameters.Add("MaThe", cardCode);
                parameters.Add("Answer", answer);
                NLogLogger.Info(new string[] { "MyVNTPWebService", "Topup", "Request", serializer.Serialize(parameters) });
                postTopup = Task.Run(() => UtilsMyVNTPWeb.PostTask("https://my.vnpt.com.vn/Payment/PpsPaymentIn", parameters, smasCookie.CookieContainer)).Result;
                if (postTopup.IsTimeout)
                {
                    NLogLogger.Info(new string[] { "MyVNTPWebService", "Topup", "Error TimeOut", serializer.Serialize(parameters) });
                    return "Post Topup Ignore Timeout";
                }
                var resultJson = serializer.Deserialize<TopupResponse>(postTopup.HtmlContent);


                //var getFcm = new MyVNTPAppFcmResquest()
                //{
                //    fcm_token ="dUGtz-1rH1E:APA91bFxAcOps6MHiwc6chGn-RTydfW2NsHpcV6pJ1kLKliVW9XD3SF6OHRq4lyojpzsKeH6z8aHjWOExZr_wiogh4lrB1lutS7nyQB5GsExHNo4Pe5BF2GQPZTqw5DfYZQGPNZ1BzN6",
                //    otp_service = "mobile_payment_recharge",
                //    session = smasCookie.SessionApp
                //};

                //var getFcmResponse = Task.Run(() => UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/otp_fcm", serializer.Serialize(getFcm))).Result;

                //var rechargeRequest = new MyVNTPAppRechargeResquest()
                //{
                //    card_id = cardCode,
                //    fcm_otp = answer,
                //    fcm_token = "dUGtz-1rH1E:APA91bFxAcOps6MHiwc6chGn-RTydfW2NsHpcV6pJ1kLKliVW9XD3SF6OHRq4lyojpzsKeH6z8aHjWOExZr_wiogh4lrB1lutS7nyQB5GsExHNo4Pe5BF2GQPZTqw5DfYZQGPNZ1BzN6",
                //    for_msisdn = accountName,
                //    session = smasCookie.SessionApp
                //};

                //var rechargeResponse = Task.Run(() => UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/mobile_payment_recharge", serializer.Serialize(rechargeRequest))).Result;
                //var resultJSON = serializer.Deserialize<MyVNTPAppRechargeResponse>(rechargeResponse);

                //if (resultJSON.error_code == "0")
                if (resultJson.success)
                {
                    //HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    //var getTopup = Task.Run(() => UtilsMyVNTPWeb.GetTask("https://my.vnpt.com.vn/Payment/AddCard", postTopup.CookieContainer)).Result;
                    //getTopup.BalanceBefore = smasCookie.BalanceBefore;
                    //doc.LoadHtml(getTopup.HtmlContent);
                    //var balaceString = doc.DocumentNode.SelectSingleNode("//span[@class='tk_number']") != null ? doc.DocumentNode.SelectSingleNode("//span[@class='tk_number']").InnerText.Trim() : string.Empty;
                    //getTopup.BalanceAfter = Convert.ToDecimal(Regex.Match(balaceString, @"(\d+.)+").Value.Trim());
                    //while ((getTopup.BalanceAfter - getTopup.BalanceBefore) == 0)
                    //{
                    //    getTopup = Task.Run(() => UtilsMyVNTPWeb.GetTask("https://my.vnpt.com.vn/Payment/AddCard", postTopup.CookieContainer)).Result;
                    //    doc.LoadHtml(getTopup.HtmlContent);
                    //    balaceString = doc.DocumentNode.SelectSingleNode("//span[@class='tk_number']") != null ? doc.DocumentNode.SelectSingleNode("//span[@class='tk_number']").InnerText.Trim() : string.Empty;
                    //    getTopup.BalanceBefore = smasCookie.BalanceBefore;
                    //    getTopup.BalanceAfter = Convert.ToDecimal(Regex.Match(balaceString, @"(\d+.)+").Value.Trim());
                    //}
                    //result = "Amount: " + (getTopup.BalanceAfter - getTopup.BalanceBefore);


                    //Ver cu
                    //getBalanceResponse = Task.Run(() => UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/mobile_IN_balances", serializer.Serialize(getBalanceRequest))).Result;
                    //if (!string.IsNullOrEmpty(getBalanceResponse))
                    //{
                    //    var balanceObj = serializer.Deserialize<MyVNTPAppBalanceResponse>(getBalanceResponse);
                    //    foreach (var balacne in balanceObj.result)
                    //    {
                    //        if (balacne.BALANCE_NAME == "Tài khoản chính")
                    //        {
                    //            balanceAfter = Convert.ToInt32(balacne.BALANCE);
                    //            break;
                    //        }
                    //    }
                    //}


                    getBalanceResponse = Task.Run(() => UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/mobile_balance_info", serializer.Serialize(getBalanceRequest))).Result;
                    if (!string.IsNullOrEmpty(getBalanceResponse))
                    {
                        var balanceObj = serializer.Deserialize<BalanceInfoResponse>(getBalanceResponse);
                        foreach (var balacne in balanceObj.balance)
                        {
                            if (balacne.origin_acc_name == "EZPAY_CORE")
                            {
                                balaceCharge.Ezpay_Core_After = Convert.ToInt32(balacne.value);
                            }

                            if (balacne.origin_acc_name == "Hot Charge")
                            {
                                balaceCharge.Hot_Charge_After = Convert.ToInt32(balacne.value);
                            }
                        }
                    }


                    //var amount = Math.Abs(balanceAfter - balanceBefore);


                    var amount = Math.Abs(balaceCharge.Ezpay_Core_After - balaceCharge.Ezpay_Core_Before);

                    //if (amount > 0 && amount <= 10000)
                    //{
                    //    amount = 10000;
                    //}
                    //else if (amount > 10000 && amount <= 20000)
                    //{
                    //    amount = 20000;
                    //}
                    //else if (amount > 20000 && amount <= 30000)
                    //{
                    //    amount = 30000;
                    //}
                    //else if (amount > 30000 && amount <= 50000)
                    //{
                    //    amount = 50000;
                    //}
                    //else if (amount > 50000 && amount <= 100000)
                    //{
                    //    amount = 100000;
                    //}
                    //else if (amount > 100000 && amount <= 200000)
                    //{
                    //    amount = 200000;
                    //}
                    //else if (amount > 200000 && amount <= 300000)
                    //{
                    //    amount = 300000;
                    //}
                    //else if (amount > 300000 && amount <= 500000)
                    //{
                    //    amount = 500000;
                    //}

                    NLogLogger.Info(new string[] { "MyVNTPWebService", "GetBalance", "Success", serializer.Serialize(getBalanceRequest), balaceCharge.Hot_Charge_After.ToString(), balaceCharge.Hot_Charge_Before.ToString(), amount.ToString() });

                    
                    //Process deving
                    //var tryAgain = 0;
                    //int[] listValue = { 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000 };

                    //if (!listValue.Contains(amount))
                    //{
                    //    var charge = balaceCharge.Hot_Charge_After - balaceCharge.Hot_Charge_Before;
                    //    var beforeCharge = balaceCharge.Ezpay_Core_After + charge;
                    //    amount = beforeCharge - balaceCharge.Ezpay_Core_Before;
                    //}
                    //-----

                    //while (!listValue.Contains(amount) && tryAgain < 3)
                    //{
                    //    getBalanceResponse = Task.Run(() => UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/mobile_IN_balances", serializer.Serialize(getBalanceRequest))).Result;

                    //    if (!string.IsNullOrEmpty(getBalanceResponse))
                    //    {
                    //        var balanceObj = serializer.Deserialize<MyVNTPAppBalanceResponse>(getBalanceResponse);
                    //        foreach (var balacne in balanceObj.result)
                    //        {
                    //            if (balacne.BALANCE_NAME == "Tài khoản chính")
                    //            {
                    //                balanceAfter = Convert.ToInt32(balacne.BALANCE);
                    //                break;
                    //            }
                    //        }
                    //    }

                    //    amount = Math.Abs(balanceAfter - balanceBefore);

                    //    NLogLogger.Info(new string[] { "MyVNTPWebService", "GetBalance", "Try", tryAgain.ToString(), serializer.Serialize(getBalanceRequest), balanceAfter.ToString(), balanceBefore.ToString(), amount.ToString() });

                    //    //if (amount > 0 && amount <= 10000)
                    //    //{
                    //    //    amount = 10000;
                    //    //}
                    //    //else if (amount > 10000 && amount <= 20000)
                    //    //{
                    //    //    amount = 20000;
                    //    //}
                    //    //else if (amount > 20000 && amount <= 30000)
                    //    //{
                    //    //    amount = 30000;
                    //    //}
                    //    //else if (amount > 30000 && amount <= 50000)
                    //    //{
                    //    //    amount = 50000;
                    //    //}
                    //    //else if (amount > 50000 && amount <= 100000)
                    //    //{
                    //    //    amount = 100000;
                    //    //}
                    //    //else if (amount > 100000 && amount <= 200000)
                    //    //{
                    //    //    amount = 200000;
                    //    //}
                    //    //else if (amount > 200000 && amount <= 300000)
                    //    //{
                    //    //    amount = 300000;
                    //    //}
                    //    //else if (amount > 300000 && amount <= 500000)
                    //    //{
                    //    //    amount = 500000;
                    //    //}

                    //    NLogLogger.Info(new string[] { "MyVNTPWebService", "GetBalance", "Try", tryAgain.ToString(), serializer.Serialize(getBalanceRequest), balanceAfter.ToString(), balanceBefore.ToString(), amount.ToString() });
                    //    tryAgain++;
                    //    Thread.Sleep(1000);
                    //}

                    result = "success " + amount;
                }
                else
                {
                    //result = resultJSON.message;
                    result = resultJson.ErrMsg;

                }

                NLogLogger.Info(new string[] { "MyVNTPWebService", "Result", cardSerial, cardCode, answer, result });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWebService", "Topup", "Error", serializer.Serialize(parameters), e.Message, smasCookie.SessionId });
                return "Post Topup Ignore Exeption";
            }

            return result;
        }



        private static void PreGenCaptCha(string accountName, string passWord, MyVNTPWebCookie smasCookie)
        {
            var sid = UtilsMyVNTPWeb.GenSid(accountName);
            var sessionCaptcha = UtilsMyVNTPWeb.DeCaptcha("https://my.vnpt.com.vn/Payment/GenerateCaptcha", sid, smasCookie);
            sessionCaptcha.Type = 8;
            sessionCaptcha.Add();
            smasCookie.SessionId = sid;
            UtilsMyVNTPWeb.SetCookieCache(sid, smasCookie);
        }

        private static void ReportCaptcha(int TaskId)
        {
            var result = new Lib.Captcha.Anticaptcha.AnticaptchaService().ReportIncorrectImageCaptcha(TaskId);
            NLogLogger.Info(new string[] { "MyVNTPWebService", "ReportCaptcha", TaskId.ToString(), result.ToString() });
        }

        private static void ReportCaptcha(string imageBase64, int type, string resultCap)
        {
            var result = new Lib.Captcha.CaptchaComVn.CaptchaComVnService().ReportIncorrectImageCaptcha(imageBase64, type, resultCap);
            NLogLogger.Info(new string[] { "MyViettelApp", "ReportCaptcha", resultCap, result.ToString() });
        }
    }
}