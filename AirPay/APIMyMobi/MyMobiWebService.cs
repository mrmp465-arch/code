
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
using APIMyMobi.Entity;
using HtmlAgilityPack;

namespace APIMyMobi
{
    public class MyMobiWebService
    {
        static readonly string baseUrl = "https://www.mobifone.vn/tai-khoan/dang-nhap-nhanh?referal=http%3A%2F%2Fwww.mobifone.vn";
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        static string CaptchaProvider = ConfigurationManager.AppSettings["Captcha_Provider"] ?? "anti-captcha.com";

        public static APIResponse TopupCard(long id, string cardSerial, string cardCode, string mobile, string accountName, string passWord, int type)
        {

            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(passWord))
            {
                return new APIResponse((int)ResponseCode.ServiceIsLocked)
                {
                    Description = "Mobile or AccountName or Password is Empty"
                };
            }

            var getTopup = new MobiCookie();
            var cookieStore = new Captcha().GetCaptcha(19, "mymobi:" + accountName);
            var sid = UtilsWeb.GenSid(accountName);
            if (cookieStore == null)
            {
                var tryAgain = 0;
                getTopup = GetTopup(id, sid, accountName, passWord);
                while (getTopup.IsTopup == false && getTopup.IsLogin == false && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "GetTopup", "Try", tryAgain.ToString(), accountName, passWord });
                    getTopup = GetTopup(id, sid, accountName, passWord);
                    tryAgain++;
                    Thread.Sleep(10000);
                }

                if (getTopup.IsTopup)
                {
                    getTopup.HtmlContent = string.Empty;
                    getTopup.SessionId = sid;

                    //Save Cookie key in DB
                    var captchaSession = new Captcha()
                    {
                        SessionId = sid,
                        Value = string.Empty,
                        TaskId = 0,
                        Type = 19,
                        ImgBase64 = string.Empty
                    };
                    captchaSession.Add();

                    //Save Cookie Value in Cached
                    UtilsWeb.SetCookieCache(sid, getTopup);
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
                getTopup = UtilsWeb.GetCookieCache(cookieStore.SessionId);
                if (getTopup != null)
                {
                    NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "TopupCard", "Get CACHED OK", getTopup.SessionId });
                }

            }

            //Gen truoc x Captcha sau login để Topup
            if (CaptchaProvider == "anti-captcha.com")
            {
                for (int i = 0; i < 1; i++) //Gen X cái
                {
                    Action<string> send = PreGenCaptCha;
                    send.BeginInvoke(accountName, null, null);
                }
            }

            if (getTopup != null)
            {

                var tryAgain = 0;
                var res = Topup(id, sid, cardSerial, cardCode, getTopup, accountName, type);
                while (string.IsNullOrEmpty(res) && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "TopupCard", "Try", tryAgain.ToString(), cardSerial, cardCode, accountName });
                    res = Topup(id, sid, cardSerial, cardCode, getTopup, accountName, type);
                    tryAgain++;
                }


                NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "TopupCard", "Response", accountName, cardSerial, cardCode, res });

                //Gen truoc x Captcha để vòng sau
                if (CaptchaProvider == "anti-captcha.com")
                {
                    for (int i = 0; i < 1; i++) //Gen X cái
                    {
                        Action<string> send = PreGenCaptCha;
                        send.BeginInvoke(accountName, null, null);
                    }
                }

                if (res.Contains("success"))
                {

                    var strAmount = Regex.Match(res, @"\d+").Value;
                    var amount = !string.IsNullOrEmpty(strAmount) ? Convert.ToInt32(Regex.Match(res, @"\d+").Value) : 0;
                    int[] listValue = { 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 5000000, 10000000 };

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

                if (res.Contains("BEFOR_BALANCE_FAILED"))
                {
                    return new APIResponse((int)ResponseCode.SystemBusy)
                    {
                        Description = res
                    };
                }

                if (res.Contains("AFTER_BALANCE_FAILED"))
                {
                    return new APIResponse((int)ResponseCode.TransactionSuspicious)
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


                if (res.Contains("Sai mã xác nhận")
                    || res.Contains("Captcha không hợp lệ")
                    )
                {
                    return new APIResponse((int)ResponseCode.ParameterInvalid)
                    {
                        Description = res
                    };
                }


                if (res.Contains("Thẻ đã được sử dụng"))
                {
                    return new APIResponse((int)ResponseCode.CardUsed)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Bạn đã nạp quá số lần nạp thẻ được cho phép trong 1 ngày") 
                    || res.Contains("Bạn đã nạp sai quá số lần cho phép"))
                {
                    return new APIResponse((int)ResponseCode.TransactionLimit)
                    {
                        Description = res
                    };
                }

                if (res.Contains("invalid-input-response") || res.Contains("timeout-or-duplicate"))
                {
                    return new APIResponse((int)ResponseCode.ParameterInvalid)
                    {
                        Description = res
                    };
                }

            }

            return new APIResponse((int)ResponseCode.TransactionFailed);

        }

        public static MobiCookie GetTopup(long id, string sid, string accountName, string passWord)
        {
            try
            {

                NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "GetTopup", "Request", accountName, passWord });
                var getCommon = Task.Run(async () => await UtilsWeb.GetTask("https://www.mobifone.vn/tai-khoan/dang-nhap?referal=https%3A%2F%2Fwww.mobifone.vn%2Ftai-khoan%2Fthong-tin-tai-khoan", new CookieContainer())).Result;
                if (getCommon == null || string.IsNullOrEmpty(getCommon.HtmlContent))
                {
                    NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "GetTopup", "Không Get được trang Login", accountName, passWord });
                    return new MobiCookie()
                    {
                        IsTopup = false,
                        IsLogin = false,
                        HtmlContent = "Không Get được trang Login"
                    };
                }

                //Check Captcha đã có chưa
                var decaptcha = new Captcha().GetCaptcha(190, "mymobi");
                if (decaptcha == null) decaptcha = UtilsWeb.NoCaptcha(sid);
                //var decaptcha = UtilsWeb.NoCaptcha(sid, getCommon);

                //Login
                var parameters = new Dictionary<string, string>();
                parameters.Add("url_send", "https://www.mobifone.vn/tai-khoan/dang-nhap-nhanh");
                parameters.Add("referal", "https://www.mobifone.vn/tai-khoan/thong-tin-tai-khoan");
                parameters.Add("recaptcha_token", decaptcha.Value);
                parameters.Add("phone", accountName);
                parameters.Add("password", passWord);

                NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "Topup", "Request", serializer.Serialize(parameters) });

                var login = Task.Run(async () => await UtilsWeb.PostTask("https://www.mobifone.vn/tai-khoan/dang-nhap", parameters, getCommon.CookieContainer)).Result;

                if (login == null || string.IsNullOrEmpty(login.HtmlContent))
                {
                    NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "GetTopup", "Không Get được trang Login", accountName, passWord });
                    return new MobiCookie()
                    {
                        IsTopup = false,
                        IsLogin = false,
                        HtmlContent = "Không Post được service Login"
                    };
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(login.HtmlContent);
                var loginCheck = doc.DocumentNode.SelectNodes("//a[@class='hasLogin']");
                if (loginCheck != null)
                {
                    if (loginCheck[0].LastChild.InnerHtml == accountName)
                    {
                        login.IsTopup = true;
                        login.IsLogin = true;
                        return login;
                    }
                }
                else
                {
                    return new MobiCookie()
                    {
                        IsTopup = false,
                        IsLogin = true,
                        HtmlContent = "Mật khẩu không hợp lệ hoạc lý do khác"
                    };
                }

            }
            catch (ThreadAbortException e)
            {
                NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "GetTopup", "Exception", e.Message });
                Thread.ResetAbort();
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "GetTopup", "Exception", e.Message });
            }

            return new MobiCookie()
            {
                IsTopup = false,
                IsLogin = false,
                HtmlContent = "Không Get được trang Nạp Tiền"
            };
        }

        public static string Topup(long id, string sid, string cardSerial, string cardCode, MobiCookie smasCookie, string accountName, int type)
        {
            var result = string.Empty;
            MobiCookie postTopup = null;
            var tryAgain = 0;
            int balanceBefor = 0;
            int balanceAfter = 0;
            var doc = new HtmlDocument();
            var parameters = new Dictionary<string, string>();

            //Get balance Befor
            NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "Topup", "Get befor Balance", accountName });
            var getBeforBalance = Task.Run(async () => await UtilsWeb.GetTask("https://www.mobifone.vn/tai-khoan/thong-tin-tai-khoan", smasCookie.CookieContainer)).Result;
            if (getBeforBalance == null || string.IsNullOrEmpty(getBeforBalance.HtmlContent))
            {
                return "BEFOR_BALANCE_FAILED";
            }
            else
            {
                NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "Topup", "BEGIN Get befor Balance", accountName });
                doc.LoadHtml(getBeforBalance.HtmlContent);
                if (type == 1) // Trả trước
                {
                    var nodeBalance = doc.DocumentNode.SelectSingleNode("//li[@class='account-balance-item']").LastChild.InnerText.Replace(".", string.Empty).Replace("đ", string.Empty).Trim();
                    balanceBefor = Convert.ToInt32(nodeBalance);
                }
                else if (type == 2) // Trả sau
                {
                    var nodeBalance = doc.DocumentNode.SelectSingleNode("//li[@class='account-balance-item item-border-r item-border-t']").LastChild.InnerText.Replace(".", string.Empty).Replace("đ", string.Empty).Trim();
                    balanceBefor = Convert.ToInt32(nodeBalance);
                }
                NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "Topup", "END Get befor Balance", accountName });
            }

            try
            {
                //Check Captcha đã có chưa
                var decaptcha = new Captcha().GetCaptcha(190, "mymobi");
                if (decaptcha == null) decaptcha = UtilsWeb.NoCaptcha(sid);
                //var decaptcha = UtilsWeb.NoCaptcha(sid, smasCookie);

                parameters.Add("card_id", cardCode);
                parameters.Add("recaptcha_token", decaptcha.Value);
                NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "Topup", "Request", serializer.Serialize(parameters) });

                postTopup = Task.Run(async () => await UtilsWeb.PostTask("https://www.mobifone.vn/tien-ich/nap-the", parameters, smasCookie.CookieContainer)).Result;
                tryAgain = 0;
                while (postTopup == null && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "PostTask", "Try", tryAgain.ToString(), serializer.Serialize(parameters) });
                    postTopup = Task.Run(async () => await UtilsWeb.PostTask("https://www.mobifone.vn/tien-ich/nap-the", parameters, smasCookie.CookieContainer)).Result;
                    tryAgain++;
                    Thread.Sleep(1000);
                }

                if (postTopup != null)
                {
                    if (!string.IsNullOrEmpty(postTopup.HtmlContent))
                    {
                        var resultJson = serializer.Deserialize<TopupWebResponse>(postTopup.HtmlContent.Replace("[","").Replace("]",""));
                        if (resultJson.status == 1)
                        {
                            NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "Result Success", cardSerial, cardCode, accountName, postTopup.HtmlContent });

                            int amount = 0;
                            bool reloadData = false;
                            while (amount <= 0 && tryAgain < 5)
                            {
                                //Get balance After
                                var getBalanceAfter = Task.Run(async () => await UtilsWeb.GetTask("https://www.mobifone.vn/tai-khoan/thong-tin-tai-khoan", smasCookie.CookieContainer)).Result;
                                if (string.IsNullOrEmpty(getBalanceAfter.HtmlContent))
                                {
                                    return "AFTER_BALANCE_FAILED";
                                }

                                doc.LoadHtml(getBalanceAfter.HtmlContent);

                                if (type == 1) // Trả trước
                                {
                                    var nodeBalance = doc.DocumentNode.SelectSingleNode("//li[@class='account-balance-item']").LastChild.InnerText.Replace(".", string.Empty).Replace("đ", string.Empty).Trim();
                                    balanceAfter = Convert.ToInt32(nodeBalance);
                                }
                                else if (type == 2) // Trả sau
                                {
                                    var nodeBalance = doc.DocumentNode.SelectSingleNode("//li[@class='account-balance-item item-border-r item-border-t']").LastChild.InnerText.Replace(".", string.Empty).Replace("đ", string.Empty).Trim();
                                    balanceAfter = Convert.ToInt32(nodeBalance);
                                }
                                //Tinh mệnh giá thẻ
                                amount = balanceAfter - balanceBefor;

                                if (amount == 0)
                                {
                                    //Reload Data
                                    var getReload = Task.Run(async () => await UtilsWeb.GetTask("https://www.mobifone.vn/force-update?mode=profile,service,package,loyalty&_=" + (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds.ToString("##"), smasCookie.CookieContainer)).Result;

                                    tryAgain++;
                                    Thread.Sleep(2000);
                                }
                            }

                            result = "success: " + amount;

                        }
                        else
                        {
                            NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "Result non Success", cardSerial, cardCode, accountName, postTopup.HtmlContent });
                            result = resultJson.message;
                        }

                    }
                    else
                    {
                        NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "Result HtmlContent Null", cardSerial, cardCode, result, smasCookie.SessionId });
                    }
                }
                else
                {
                    NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "Result Object postTopup Null", cardSerial, cardCode, result, smasCookie.SessionId });
                    return result;
                }

            }

            catch (ThreadAbortException e)
            {
                NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "Topup", "Error", serializer.Serialize(parameters), e.Message, smasCookie.SessionId });
                Thread.ResetAbort();
                return "Post Topup Ignore Timeout";
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyMobiWebService", id.ToString(), "Topup", "Error", serializer.Serialize(parameters), e.Message, smasCookie.SessionId, postTopup.HtmlContent });
                return "Post Topup Ignore Exeption";
            }

            return result;
        }

        private static void PreGenCaptCha(string accountName)
        {
            var sid = "mymobi";
            var sessionCaptcha = UtilsWeb.NoCaptcha(sid);
            sessionCaptcha.Type = 190;
            sessionCaptcha.Add();
        }

        //private static void ReportCaptcha(int TaskId)
        //{
        //    var result = new Lib.Captcha.Anticaptcha.AnticaptchaService().ReportIncorrectImageCaptcha(TaskId);
        //    NLogLogger.Info(new string[] { "MyMobiWebService", "ReportCaptcha", TaskId.ToString(), result.ToString() });
        //}

        //public static string Base64Encode(string plainText)
        //{
        //    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        //    return System.Convert.ToBase64String(plainTextBytes);
        //}
    }
}