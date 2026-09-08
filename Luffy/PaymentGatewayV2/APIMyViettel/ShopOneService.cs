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
using APIMyViettel.Entity;
using APIMyViettel.Service;
using Lib.Captcha;
using Libs.API;
using Libs.Utils;
using Newtonsoft.Json.Linq;

namespace APIMyViettel
{
    public class ShopOneService
    {
        static readonly string baseUrl = "https://shopone.com.vn";
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        static bool CheckSerial_MyVTT = bool.Parse(ConfigurationManager.AppSettings["ShopOne_CheckSerialFirst_MyVTT"] ?? "false");
        static bool CheckSerial_Service = bool.Parse(ConfigurationManager.AppSettings["ShopOne_CheckSerialFirst_Service"] ?? "false");

        public static APIResponse TopupCard(string cardSerial, string cardCode, string mobile, int type, string accountName, string passWord)
        {

            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(passWord))
            {
                return new APIResponse((int)ResponseCode.ServiceIsLocked)
                {
                    Description = "Mobile or AccountName or Password is Empty"
                };
            }

            //Check Service

            if (CheckSerial_Service)
            {
                var checkReponse = CheckSerial.CheckCard(cardSerial);
                NLogLogger.Info(new string[] { "MyViettelApp", "CheckSerial", serializer.Serialize(checkReponse) });
                if (checkReponse.ResponseCode == (int)ResponseCode.CardUsed ||
                    checkReponse.ResponseCode == (int)ResponseCode.CardSerialInvalid)
                {
                    return checkReponse;
                }

                if (checkReponse.ResponseCode == (int)ResponseCode.TransactionFailed)
                {

                    //Check by MyVTT
                    if (CheckSerial_MyVTT)
                    {
                        int tryAgain = 0;
                        checkReponse = MyViettelService.CheckCard(cardSerial);
                        while (tryAgain < 5 && (checkReponse.ResponseCode == (int)ResponseCode.LoginFail
                                                || checkReponse.ResponseCode == (int)ResponseCode.TransactionLimit
                                                || checkReponse.ResponseCode == (int)ResponseCode.AccessDenied
                                                || checkReponse.ResponseCode == (int)ResponseCode.ParameterInvalid))
                        {
                            Thread.Sleep(1000);
                            checkReponse = MyViettelService.CheckCard(cardSerial);
                            if (checkReponse.ResponseCode == (int)ResponseCode.AccountNotExists) tryAgain = 5;
                            tryAgain++;
                        }

                        if (checkReponse.ResponseCode == (int)ResponseCode.CardUsed ||
                            checkReponse.ResponseCode == (int)ResponseCode.CardSerialInvalid)
                        {
                            return checkReponse;
                        }
                    }
                }
            }

            if (CheckSerial_MyVTT)
            {
                int tryAgain = 0;
                var checkReponse = MyViettelService.CheckCard(cardSerial);
                while (tryAgain < 5 && (checkReponse.ResponseCode == (int)ResponseCode.LoginFail
                                        || checkReponse.ResponseCode == (int)ResponseCode.TransactionLimit
                                        || checkReponse.ResponseCode == (int)ResponseCode.AccessDenied
                                        || checkReponse.ResponseCode == (int)ResponseCode.ParameterInvalid))
                {
                    Thread.Sleep(1000);
                    checkReponse = MyViettelService.CheckCard(cardSerial);
                    if (checkReponse.ResponseCode == (int)ResponseCode.AccountNotExists) tryAgain = 5;
                    tryAgain++;
                }
                if (checkReponse.ResponseCode == (int)ResponseCode.CardUsed || checkReponse.ResponseCode == (int)ResponseCode.CardSerialInvalid)
                {
                    return checkReponse;
                }
            }

            var getTopup = new ShopOneCookie();
            var decaptcha = new Captcha().GetCaptcha(6, "shop:" + accountName);
            if (decaptcha == null)
            {
                var sid = UtilsShopOne.GenSid(accountName);
                getTopup = GetTopup(accountName, passWord);
                if (getTopup.IsTopup)
                {
                    decaptcha = UtilsShopOne.DeCaptcha(baseUrl + getTopup.CaptChaLink, sid, getTopup);
                    getTopup.HtmlContent = string.Empty;
                    getTopup.SessionId = sid;
                    UtilsShopOne.SetCookieCache(sid, getTopup);
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
                getTopup = UtilsShopOne.GetCookieCache(decaptcha.SessionId);
            }

            if (getTopup != null)
            {
                var res = Topup(cardSerial, cardCode, decaptcha.Value, getTopup);

                for (int i = 0; i < 1; i++)
                {
                    Action<string, string, ShopOneCookie> send = PreGenCaptCha;
                    send.BeginInvoke(accountName, passWord, getTopup, null, null);
                }

                if (res.Contains("Success"))
                {
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        Description = res,
                        ResponseContent = Regex.Match(res, @"(\d+.)+").Value.Trim()
                    };
                }

                if (res.Contains("lòng thử lại sau"))
                {
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = res
                    };
                }


                if (res.Contains("Post Topup Ignore Timeout"))
                {
                    return new APIResponse((int)ResponseCode.TransactionIgnore)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Mã thẻ hoặc số serial thẻ không đúng"))
                {
                    return new APIResponse((int)ResponseCode.CardCodeInvalid)
                    {
                        Description = res
                    };
                }

                //if (res.Contains("The da duoc su dung"))
                //{
                //    return new APIResponse((int)ResponseCode.CardUsed)
                //    {
                //        Description = res
                //    };
                //}

                if (res.Contains("Mã bảo vệ không đúng"))
                {
                    return new APIResponse((int)ResponseCode.ParameterInvalid)
                    {
                        Description = res
                    };
                }

            }

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public static ShopOneCookie GetTopup(string accountName, string passWord)
        {
            NLogLogger.Info(new string[] { "ShopOneService", "GetTopup", "Request", accountName, passWord });

            var getLogin = Task.Run(() => UtilsShopOne.GetTask("https://shopone.com.vn/login", new CookieContainer())).Result;

            var parameters = new Dictionary<string, string>();
            parameters.Add("token", getLogin.RequestVerificationToken);
            parameters.Add("-1", "-1");
            parameters.Add("userName", accountName);
            parameters.Add("password", passWord);
            parameters.Add("typeLogin", "on");
            parameters.Add("shortName", "");
            parameters.Add("rememberMe", "1");
            parameters.Add("hd_cskh", "1");
            parameters.Add("hd_sale", "0");
            var loginResponse = Task.Run(() => UtilsShopOne.PostTask("https://shopone.com.vn/submitlogin", parameters, getLogin.CookieContainer)).Result;

            var detectSuccess = loginResponse.HtmlContent.Contains("Tên đăng nhập hoặc mật khẩu không hợp lệ");
            if (detectSuccess)
            {
                NLogLogger.Info(new string[] { "ShopOneService", "Topup", "Error Login", serializer.Serialize(parameters) });
                return new ShopOneCookie()
                {
                    IsTopup = false,
                    HtmlContent = "Tên đăng nhập hoặc mật khẩu không hợp lệ" //HttpUtility.HtmlDecode("Sai t&#234;n đăng nhập hoặc mật khẩu")
                };
            }

            var getTopup = Task.Run(() => UtilsShopOne.GetTask("https://shopone.com.vn/prepaid", loginResponse.CookieContainer)).Result;

            if (getTopup == null)
            {
                NLogLogger.Info(new string[] { "ShopOneService", "Topup", "Error Locked 30 min", serializer.Serialize(parameters) });
                return new ShopOneCookie() { IsTopup = false, HtmlContent = "Locked 30 min" }; //Bi khoa 15P
            }
            getTopup.IsTopup = true;

            return getTopup;

        }

        public static string Topup(string cardSerial, string cardCode, string answer, ShopOneCookie shopCookie)
        {
            var result = string.Empty;
            var parameters = new Dictionary<string, string>();
            ShopOneCookie postTopup = null;
            try
            {
                parameters.Add("serial", cardSerial);
                parameters.Add("viettelCardCode", cardCode);
                parameters.Add("captcha", answer);
                parameters.Add("token", shopCookie.RequestVerificationToken);

                postTopup = Task.Run(() => UtilsShopOne.PostTask("https://shopone.com.vn/prepaid/submitPrepaidProcess", parameters, shopCookie.CookieContainer)).Result;
                if (postTopup.IsTimeout)
                {
                    NLogLogger.Info(new string[] { "ShopOneService", "Topup", "Error TimeOut", serializer.Serialize(parameters) });
                    return "Post Topup Ignore Timeout";
                }

                NLogLogger.Info(new string[] { "ShopOneService", "Result", cardSerial, cardCode, answer, postTopup.HtmlContent });
                var res = (string)JObject.Parse(postTopup.HtmlContent)["money"];

                if (!string.IsNullOrEmpty(res))
                {
                    result = "Success " + res;
                }
                else
                {
                    result = (string)JObject.Parse(postTopup.HtmlContent)["errorMsg"];
                }


                return result;

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "ShopOneService", "Topup", "Error", serializer.Serialize(parameters), e.Message, shopCookie.SessionId });
                return "Post Topup Ignore Exeption";
            }
        }

        private static void PreGenCaptCha(string accountName, string passWord, ShopOneCookie shopCookie)
        {

            var sid = UtilsShopOne.GenSid(accountName);
            //var getTopup = GetTopup(accountName, passWord);
            var getTopup = Task.Run(() => UtilsShopOne.GetTask("https://shopone.com.vn/prepaid", shopCookie.CookieContainer)).Result;

            if (!string.IsNullOrEmpty(getTopup.CaptChaLink))
            {
                var sessionCaptcha = UtilsShopOne.DeCaptcha(baseUrl + getTopup.CaptChaLink, sid, getTopup);
                sessionCaptcha.Type = 6;
                sessionCaptcha.Add();
                getTopup.SessionId = sid;
                getTopup.HtmlContent = String.Empty;
                UtilsShopOne.SetCookieCache(sid, getTopup);
            }


        }

        private static void ReportCaptcha(int TaskId)
        {
            var result = new Lib.Captcha.Anticaptcha.AnticaptchaService().ReportIncorrectImageCaptcha(TaskId);
            NLogLogger.Info(new string[] { "ShopOneService", "ReportCaptcha", TaskId.ToString(), result.ToString() });
        }
    }
}