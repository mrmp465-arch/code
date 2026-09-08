using APIMyVNTP.Entity;
using Lib.Captcha;
using Libs.API;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace APIMyVNTP
{
    public class VNTPWebService
    {
        static readonly string baseUrl = "http://naptien.vinaphone.com.vn";
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        static string CaptchaProvider = ConfigurationManager.AppSettings["Captcha_Provider"] ?? "anti-captcha.com";

        public static APIResponse TopupCard(long id, string cardSerial, string cardCode, string mobile, string cardType, string accountName, string passWord, int topupType)
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
            var decaptcha = new Captcha().GetCaptcha(9, "vnpweb:" + accountName);
            if (decaptcha == null)
            {
                var sid = UtilsVNTPWeb.GenSid(accountName);

                var tryAgain = 0;
                getTopup = GetTopup(id, accountName, passWord, topupType);
                while (getTopup.IsTopup == false && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "GetTopup", "Try", tryAgain.ToString(), accountName, passWord, topupType.ToString() });
                    getTopup = GetTopup(id, accountName, passWord, topupType);
                    tryAgain++;
                    Thread.Sleep(10000);
                }

                if (getTopup.IsTopup)
                {
                    decaptcha = UtilsVNTPWeb.DeCaptcha(getTopup.CaptChaLink, sid, getTopup);

                    tryAgain = 0;
                    while (decaptcha == null && tryAgain < 3)
                    {
                        NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "GetTopup", "Try Decaptcha null", tryAgain.ToString(), sid, getTopup.CaptChaLink });
                        decaptcha = UtilsVNTPWeb.DeCaptcha(getTopup.CaptChaLink, sid, getTopup);
                        tryAgain++;
                        Thread.Sleep(10000);
                    }

                    if (decaptcha == null)
                    {
                        return new APIResponse((int)ResponseCode.ParameterInvalid) { Description = "DeCaptcha Null" };
                    }

                    getTopup.HtmlContent = string.Empty;
                    getTopup.SessionId = sid;
                    UtilsVNTPWeb.SetCookieCache(sid, getTopup);
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
                getTopup = UtilsVNTPWeb.GetCookieCache(decaptcha.SessionId);
                if (getTopup != null) NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "TopupCard", "Get CACHED OK", getTopup.SessionId });
            }

            if (getTopup != null)
            {

                var tryAgain = 0;
                var res = Topup(id, cardSerial, cardCode, decaptcha.Value, cardType, getTopup, accountName, topupType);
                while (string.IsNullOrEmpty(res) && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "TopupCard", "Try", tryAgain.ToString(), cardSerial, cardCode, decaptcha.Value, cardType, accountName });
                    res = Topup(id, cardSerial, cardCode, decaptcha.Value, cardType, getTopup, accountName, topupType);
                    tryAgain++;
                }


                NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "TopupCard", "Response", accountName, cardSerial, cardCode, res });

                //Gen truoc x Captcha
                if (CaptchaProvider == "anti-captcha.com")
                {
                    for (int i = 0; i < 1; i++)
                    {
                        Action<string, MyVNTPWebCookie> send = PreGenCaptCha;
                        send.BeginInvoke(accountName, getTopup, null, null);
                        Thread.Sleep(1000);
                    }
                }

                if (res.Contains("success"))
                {

                    var strAmount = Regex.Match(res, @"\d+").Value;
                    var amount = !string.IsNullOrEmpty(strAmount) ? Convert.ToInt32(Regex.Match(res, @"\d+").Value) : 0;
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

                if (res.Contains("balanceBefore failed"))
                {
                    return new APIResponse((int)ResponseCode.SystemBusy)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Post Topup Ignore Timeout"))
                {
                    return new APIResponse((int)ResponseCode.TransactionTimeout)
                    {
                        Description = res
                    };
                }
                if (res.Contains("Post Topup Ignore Null")
                    || res.Contains("Post Topup Ignore Exeption"))
                {
                    return new APIResponse((int)ResponseCode.ParameterInvalid);
                }


                if (res.Contains("Captcha không đúng")
                    || res.Contains("Tính năng nạp tiền hộ tạm thời không được hỗ trợ"))
                {
                    return new APIResponse((int)ResponseCode.ParameterInvalid)
                    {
                        Description = res
                    };
                }


                if (topupType == 1)
                {
                    if (res.Contains("Mã số thẻ nạp không đúng"))
                    {
                        return new APIResponse((int)ResponseCode.CardUsed)
                        {
                            Description = res
                        };
                    }

                    if (res.Contains("Đã có lỗi xảy ra, nạp thẻ không thành công"))
                    {
                        return new APIResponse((int)ResponseCode.ServiceIsLocked)
                        {
                            Description = res
                        };
                    }
                }
                else if (topupType == 2)
                {
                    if (res.Contains("Đã có lỗi xảy ra, nạp thẻ không thành công")
                        || res.Contains("Mã số thẻ nạp không đúng"))
                    {
                        return new APIResponse((int)ResponseCode.CardUsed)
                        {
                            Description = res
                        };
                    }

                }




                if (res.Contains("Thuê bao không được phép nạp tiền")
                    || res.Contains("Bạn đã nạp thẻ sai quá nhiều")
                )
                {
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = res
                    };
                }

            }

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public static MyVNTPWebCookie GetTopup(long id, string accountName, string passWord, int topupType)
        {
            try
            {

                NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "GetTopup", "Request", accountName, passWord });

                var getLogin = Task.Run(async () => await UtilsVNTPWeb.GetTask("https://id.vinaphone.com.vn/auth/login?service=http%3a%2f%2fnaptien.vinaphone.com.vn%2fHome%2fLoginedAddCard", new CookieContainer())).Result;
                //var getLogin = Task.Run(() => UtilsVNTPWeb.GetTask("https://vinaphone.com.vn/auth/login?service=http%3a%2f%2fnaptien.vinaphone.com.vn%2fHome%2fLoginedAddCard", new CookieContainer())).Result;

                var tryAgain = 0;

                while (getLogin == null && tryAgain < 5)
                {
                    NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "GetTopup", "Try Null", tryAgain.ToString(), accountName, passWord });
                    getLogin = Task.Run(async () => await UtilsVNTPWeb.GetTask("https://id.vinaphone.com.vn/auth/login?service=http%3a%2f%2fnaptien.vinaphone.com.vn%2fHome%2fLoginedAddCard", new CookieContainer())).Result;
                    //getLogin = Task.Run(() => UtilsVNTPWeb.GetTask("https://vinaphone.com.vn/auth/login?service=http%3a%2f%2fnaptien.vinaphone.com.vn%2fHome%2fLoginedAddCard", new CookieContainer())).Result;

                    tryAgain++;
                    Thread.Sleep(1000);
                }

                //tryAgain = 0;
                //while (string.IsNullOrEmpty(getLogin.HtmlContent) && tryAgain < 3)
                //{
                //    NLogLogger.Info(new string[] { "VNTPWebService", "GetTopup", "Try Empty", tryAgain.ToString(), accountName, passWord });
                //    getLogin = Task.Run(async () => await UtilsVNTPWeb.GetTask("https://vinaphone.com.vn/auth/login?service=http%3a%2f%2fnaptien.vinaphone.com.vn%2fHome%2fLoginedAddCard", new CookieContainer())).Result;
                //    tryAgain++;
                //    Thread.Sleep(1000);
                //}

                if (getLogin == null || string.IsNullOrEmpty(getLogin.HtmlContent))
                {
                    NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "GetTopup", "Không Get được trang Login", accountName, passWord });
                    return new MyVNTPWebCookie()
                    {
                        IsTopup = false,
                        HtmlContent = "Không Get được trang Login"
                    };
                }

                //Get lt
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(getLogin.HtmlContent);

                var lt = doc.DocumentNode.SelectSingleNode("//input[@type='hidden' and @name='lt']").Attributes["value"] != null ? doc.DocumentNode.SelectSingleNode("//input[@type='hidden' and @name='lt']").Attributes["value"].Value : string.Empty;

                var parameters = new Dictionary<string, string>();
                parameters.Add("username", accountName);
                parameters.Add("password", passWord);
                parameters.Add("lt", lt);
                parameters.Add("_eventId", "submit");
                parameters.Add("dnstb", "Đăng nhập");

                NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "Login Request", serializer.Serialize(parameters) });

                tryAgain = 0;
                var loginResponse = Task.Run(async () => await UtilsVNTPWeb.PostTask("https://id.vinaphone.com.vn/auth/login?service=http%3a%2f%2fnaptien.vinaphone.com.vn%2fHome%2fLoginedAddCard", parameters, getLogin.CookieContainer)).Result;
                //var loginResponse = Task.Run(() => UtilsVNTPWeb.PostTask("https://vinaphone.com.vn/auth/login?service=http%3a%2f%2fnaptien.vinaphone.com.vn%2fHome%2fLoginedAddCard", parameters, getLogin.CookieContainer)).Result;

                while (loginResponse == null && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "GetTopup", "Try Login Null", tryAgain.ToString(), accountName, passWord });
                    loginResponse = Task.Run(async () => await UtilsVNTPWeb.PostTask("https://id.vinaphone.com.vn/auth/login?service=http%3a%2f%2fnaptien.vinaphone.com.vn%2fHome%2fLoginedAddCard", parameters, getLogin.CookieContainer)).Result;
                    //loginResponse = Task.Run(() => UtilsVNTPWeb.PostTask("https://vinaphone.com.vn/auth/login?service=http%3a%2f%2fnaptien.vinaphone.com.vn%2fHome%2fLoginedAddCard", parameters, getLogin.CookieContainer)).Result;
                    tryAgain++;
                    Thread.Sleep(10000);
                }


                if (loginResponse == null)
                {

                    NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "GetTopup", "Login Timeout", accountName, passWord });
                    return new MyVNTPWebCookie()
                    {
                        IsTopup = false,
                        IsTimeout = true,
                        HtmlContent = "Login Timeout"
                    };
                }

                doc.LoadHtml(loginResponse.HtmlContent);

                var detectFailed = loginResponse.HtmlContent.Contains("Đăng nhập Vinaportal");
                var detectSuccess = loginResponse.HtmlContent.Contains("log-out");
                if (!detectSuccess)
                {

                    if (detectFailed)
                    {
                        return new MyVNTPWebCookie()
                        {
                            IsTopup = false,
                            HtmlContent = "Tài khoản đăng nhập không đúng Web!"
                        };
                    }
                    else
                    {
                        NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "Topup", "Error Login Web", serializer.Serialize(parameters), loginResponse.HtmlContent });
                        return new MyVNTPWebCookie()
                        {
                            IsTopup = false,
                            HtmlContent = "Hệ thống VNP có vấn đề ko login được!"
                        };
                    }
                }
                else
                {
                    //Add Account MyVNP
                    Action<string, string, int> addAccount = AddAccountVNP;
                    addAccount.BeginInvoke(accountName, passWord, topupType, null, null);

                    NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "Topup", "Login Ok", serializer.Serialize(parameters) });
                }

                var getTopup = Task.Run(async () => await UtilsVNTPWeb.GetTask("http://naptien.vinaphone.com.vn/Home/AddCard", loginResponse.CookieContainer)).Result;
                //var getTopup = Task.Run(() => UtilsVNTPWeb.GetTask("http://naptien.vinaphone.com.vn/Home/AddCard", loginResponse.CookieContainer)).Result;

                tryAgain = 0;
                while (getTopup == null && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "GetTopup", "Try", tryAgain.ToString(), accountName, passWord });
                    getTopup = Task.Run(async () => await UtilsVNTPWeb.GetTask("http://naptien.vinaphone.com.vn/Home/AddCard", loginResponse.CookieContainer)).Result;
                    //getTopup = Task.Run(() => UtilsVNTPWeb.GetTask("http://naptien.vinaphone.com.vn/Home/AddCard", loginResponse.CookieContainer)).Result;

                    tryAgain++;
                    Thread.Sleep(10000);
                }

                // Trả sau
                if (topupType == 2)
                {

                    var token = UtilsMyVNTPApp.GetTokenCache(accountName);
                    if (token == null)
                    {
                        var loginRequest = new MyVNTPAppLoginResquest()
                        {
                            device_info = "SM-G532G",
                            fcm_registration_token = "dUGtz-1rH1E:APA91bFxAcOps6MHiwc6chGn-RTydfW2NsHpcV6pJ1kLKliVW9XD3SF6OHRq4lyojpzsKeH6z8aHjWOExZr_wiogh4lrB1lutS7nyQB5GsExHNo4Pe5BF2GQPZTqw5DfYZQGPNZ1BzN6",
                            mode = "password",
                            msisdn = accountName,
                            password = Encrypts.MD5(passWord).ToUpper()
                        };
                        var loginAppResponse = Task.Run(async () => await UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/authen_msisdn", serializer.Serialize(loginRequest))).Result;
                        //var loginAppResponse = Task.Run( () =>  UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/authen_msisdn", serializer.Serialize(loginRequest))).Result;

                        if (string.IsNullOrEmpty(loginAppResponse))
                        {
                            NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "Topup", "Error Login App", serializer.Serialize(parameters) });
                            return new MyVNTPWebCookie()
                            {
                                IsTopup = false,
                                HtmlContent = "Tài khoản đăng nhập không đúng App"
                            };
                        }
                        var loginAppObjResponse = serializer.Deserialize<MyVNTPAppLoginResponse>(loginAppResponse);
                        if (loginAppObjResponse.error_code == "1")
                        {
                            return new MyVNTPWebCookie()
                            {
                                IsTopup = false,
                                HtmlContent = "System Busy App"
                            };
                        }
                        getTopup.SessionApp = loginAppObjResponse.session;
                        UtilsMyVNTPApp.SetTokenCache(accountName, loginAppObjResponse.session);
                    }
                    else
                    {
                        getTopup.SessionApp = token;
                    }

                    //var loginRequest = new MyVNTPAppLoginResquest()
                    //{
                    //    device_info = "SM-G532G",
                    //    fcm_registration_token = "dUGtz-1rH1E:APA91bFxAcOps6MHiwc6chGn-RTydfW2NsHpcV6pJ1kLKliVW9XD3SF6OHRq4lyojpzsKeH6z8aHjWOExZr_wiogh4lrB1lutS7nyQB5GsExHNo4Pe5BF2GQPZTqw5DfYZQGPNZ1BzN6",
                    //    mode = "password",
                    //    msisdn = accountName,
                    //    password = Encrypts.MD5(passWord).ToUpper()
                    //};
                    //var loginAppResponse = Task.Run(async () => await UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/authen_msisdn", serializer.Serialize(loginRequest))).Result;
                    ////var loginAppResponse = Task.Run( () =>  UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/authen_msisdn", serializer.Serialize(loginRequest))).Result;

                    //if (string.IsNullOrEmpty(loginAppResponse))
                    //{
                    //    NLogLogger.Info(new string[] { "VNTPWebService", "Topup", "Error Login App", serializer.Serialize(parameters) });
                    //    return new MyVNTPWebCookie()
                    //    {
                    //        IsTopup = false,
                    //        HtmlContent = "Tài khoản đăng nhập không đúng App"
                    //    };
                    //}
                    //var loginAppObjResponse = serializer.Deserialize<MyVNTPAppLoginResponse>(loginAppResponse);
                    //if (loginAppObjResponse.error_code == "1")
                    //{
                    //    return new MyVNTPWebCookie()
                    //    {
                    //        IsTopup = false,
                    //        HtmlContent = "System Busy App"
                    //    };
                    //}
                    //getTopup.SessionApp = loginAppObjResponse.session;
                }
                // --------------------------

                if (getTopup == null)
                {
                    NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "GetTopup", "NULLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL", tryAgain.ToString(), accountName, passWord });
                    getTopup = new MyVNTPWebCookie
                    {
                        IsTopup = false
                    };
                }
                else
                {
                    getTopup.IsTopup = true;
                }

                return getTopup;
            }
            catch (ThreadAbortException e)
            {
                NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "GetTopup", "Exception", e.Message });
                Thread.ResetAbort();
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "GetTopup", "Exception", e.Message });
            }

            return new MyVNTPWebCookie()
            {
                IsTopup = false,
                HtmlContent = "Không Get được trang Login"
            };
        }

        public static string Topup(long id, string cardSerial, string cardCode, string answer, string cardType, MyVNTPWebCookie smasCookie, string accountName, int topupType)
        {
            var result = string.Empty;
            var parameters = new Dictionary<string, string>();
            MyVNTPWebCookie postTopup = null;

            // Trả sau
            int balanceBefore = 0;
            int balanceAfter = 0;
            var getBalanceRequest = new MyVNTPAppBalanceResquest()
            {
                msisdn = accountName,
                session = smasCookie.SessionApp
            };
            var tryAgain = 0;
            if (topupType == 2)
            {

                var getBalanceResponse = Task.Run(async () => await UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/mobile_IN_balances", serializer.Serialize(getBalanceRequest))).Result;
                NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "GetBalance Before", "Response", serializer.Serialize(getBalanceRequest), getBalanceResponse });
                while (string.IsNullOrEmpty(getBalanceResponse) && tryAgain < 3)
                {
                    getBalanceResponse = Task.Run(async () => await UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/mobile_IN_balances", serializer.Serialize(getBalanceRequest))).Result;
                    tryAgain++;
                    Thread.Sleep(1000);
                }

                if (!string.IsNullOrEmpty(getBalanceResponse))
                {
                    var balanceObj = serializer.Deserialize<MyVNTPAppBalanceResponse>(getBalanceResponse);
                    foreach (var balacne in balanceObj.result)
                    {
                        if (balacne.BALANCE_NAME == "Tài khoản chính")
                        {
                            balanceBefore = Convert.ToInt32(balacne.BALANCE);
                            break;
                        }
                    }
                }
                else
                {
                    NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "GetBalance Before", "Response Empty", serializer.Serialize(getBalanceRequest), getBalanceResponse });
                    return "balanceBefore failed";
                }
            }
            //


            try
            {
                parameters.Add("PhoneNum", accountName);
                parameters.Add("MaThe", cardCode);
                parameters.Add("Answer", answer);
                NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "Topup", "Request", serializer.Serialize(parameters) });

                postTopup = Task.Run(async () => await UtilsVNTPWeb.PostTask("http://naptien.vinaphone.com.vn/Home/AddPrepaid", parameters, smasCookie.CookieContainer)).Result;
                //postTopup = Task.Run(() => UtilsVNTPWeb.PostTask("http://naptien.vinaphone.com.vn/Home/AddPrepaid", parameters, smasCookie.CookieContainer)).Result;
                tryAgain = 0;
                while (postTopup == null && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "PostTask", "Try", tryAgain.ToString(), serializer.Serialize(parameters) });
                    postTopup = Task.Run(async () => await UtilsVNTPWeb.PostTask("http://naptien.vinaphone.com.vn/Home/AddPrepaid", parameters, smasCookie.CookieContainer)).Result;
                    //postTopup = Task.Run(() => UtilsVNTPWeb.PostTask("http://naptien.vinaphone.com.vn/Home/AddPrepaid", parameters, smasCookie.CookieContainer)).Result;
                    tryAgain++;
                    Thread.Sleep(1000);
                }

                if (postTopup != null)
                {
                    if (!string.IsNullOrEmpty(postTopup.HtmlContent))
                    {
                        var resultJson = serializer.Deserialize<TopupResponse>(postTopup.HtmlContent);
                        if (resultJson.success)
                        {
                            NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "Result Success", cardSerial, cardCode, accountName, postTopup.HtmlContent });
                            //Trả sau
                            if (topupType == 2)
                            {
                                tryAgain = 0;
                                var getBalanceResponse = Task.Run(async () => await UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/mobile_IN_balances", serializer.Serialize(getBalanceRequest))).Result;
                                NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "GetBalance After", "Response", tryAgain.ToString(), serializer.Serialize(getBalanceRequest), getBalanceResponse });

                                while (string.IsNullOrEmpty(getBalanceResponse) && tryAgain < 3)
                                {
                                    getBalanceResponse = Task.Run(async () => await UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/mobile_IN_balances", serializer.Serialize(getBalanceRequest))).Result;
                                    tryAgain++;
                                    Thread.Sleep(1000);
                                }


                                if (!string.IsNullOrEmpty(getBalanceResponse))
                                {
                                    var balanceObj = serializer.Deserialize<MyVNTPAppBalanceResponse>(getBalanceResponse);
                                    foreach (var balacne in balanceObj.result)
                                    {
                                        if (balacne.BALANCE_NAME == "Tài khoản chính")
                                        {
                                            balanceAfter = Convert.ToInt32(balacne.BALANCE);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "GetBalance After", "Response Empty", tryAgain.ToString(), serializer.Serialize(getBalanceRequest), getBalanceResponse });
                                    return "success 0";
                                }


                                var amount = balanceAfter - balanceBefore;
                                //tryAgain = 0;
                                //while (amount == 0 && tryAgain < 3)
                                //{
                                //    //NLogLogger.Info(new string[] { "VNTPWebService", "GetBalance After", "Try", tryAgain.ToString(), serializer.Serialize(getBalanceRequest) });
                                //    getBalanceResponse = Task.Run(async () => await UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/mobile_IN_balances", serializer.Serialize(getBalanceRequest))).Result;
                                //    NLogLogger.Info(new string[] { "VNTPWebService", "GetBalance", "Try", tryAgain.ToString(), serializer.Serialize(getBalanceRequest), getBalanceResponse });
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
                                //    amount = balanceAfter - balanceBefore;
                                //    tryAgain++;
                                //    Thread.Sleep(1000);
                                //}

                                result = "success " + amount;
                            }
                            //---------------------
                            else
                            {

                                result = "success " + resultJson.ErrMsg;
                            }

                        }
                        else
                        {
                            NLogLogger.Info(new string[]
                            {
                                "VNTPWebService",id.ToString(),  "Result non Success", cardSerial, cardCode, accountName, postTopup.HtmlContent
                            });
                            result = resultJson.ErrMsg;
                        }

                    }
                    else
                    {
                        NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "Result HtmlContent Null", cardSerial, cardCode, answer, result, smasCookie.SessionId });
                    }
                }
                else
                {
                    NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "Result Object postTopup Null", cardSerial, cardCode, answer, result, smasCookie.SessionId });
                    return result;
                }


            }

            catch (ThreadAbortException e)
            {
                NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "Topup", "Error", serializer.Serialize(parameters), e.Message, smasCookie.SessionId });
                Thread.ResetAbort();
                return "Post Topup Ignore Timeout";
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "VNTPWebService", id.ToString(), "Topup", "Error", serializer.Serialize(parameters), e.Message, smasCookie.SessionId, postTopup.HtmlContent });
                return "Post Topup Ignore Exeption";
            }

            return result;
        }



        private static void PreGenCaptCha(string accountName, MyVNTPWebCookie smasCookie)
        {
            var sid = UtilsVNTPWeb.GenSid(accountName);
            var sessionCaptcha = UtilsVNTPWeb.DeCaptcha("http://naptien.vinaphone.com.vn/Home/GenerateCaptcha", sid, smasCookie);
            sessionCaptcha.Type = 9;
            sessionCaptcha.Add();
            smasCookie.SessionId = sid;
            UtilsVNTPWeb.SetCookieCache(sid, smasCookie);

        }

        private static void ReportCaptcha(int TaskId)
        {
            var result = new Lib.Captcha.Anticaptcha.AnticaptchaService().ReportIncorrectImageCaptcha(TaskId);
            NLogLogger.Info(new string[] { "VNTPWebService", "ReportCaptcha", TaskId.ToString(), result.ToString() });
        }

        private static void AddAccountVNP(string accountName, string passWord, int type)
        {
            var account = new Account()
            {
                AccountName = accountName,
                Password = passWord,
                Type = type, //1 : Trả trước; 2: Trả sau
                Source = "Auto"
            };
            var res = account.Insert();
            switch (res)
            {
                case 1:
                    NLogLogger.Info(new string[] { "VNTPWebService", "Insert Account Success", res.ToString(), accountName, passWord });
                    break;
                case 2:
                    NLogLogger.Info(new string[] { "VNTPWebService", "Update Account Success", res.ToString(), accountName, passWord });
                    break;
                case -99:
                    NLogLogger.Info(new string[] { "VNTPWebService", "Insert or Update Account Failed", res.ToString(), accountName, passWord });
                    break;
                default:
                    NLogLogger.Info(new string[] { "VNTPWebService", "Insert or Update Account Failed", res.ToString(), accountName, passWord });
                    break;
            }

        }
    }
}