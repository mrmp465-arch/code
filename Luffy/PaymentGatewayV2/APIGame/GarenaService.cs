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
using APIGame.Entity;
using Jurassic.Library;
using Lib.Captcha;
using Libs.API;
using Libs.Report;
using Libs.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace APIGame
{
    public class GarenaService
    {

        static JavaScriptSerializer serializer = new JavaScriptSerializer();

        public static APIResponse TopupCard(string cardSerial, string cardCode, string mobile, string tranId, string tranId3rd, string accountName, string passWord, int topupType)
        {

            if (string.IsNullOrEmpty(mobile))
            {
                return new APIResponse((int)ResponseCode.ServiceIsLocked)
                {
                    Description = "Mobile or AccountName or Password is Empty"
                };
            }

            var account = new GarenaAccount();

            if (topupType == 9)
            {
                if (!string.IsNullOrEmpty(accountName) && !string.IsNullOrEmpty(passWord))
                {
                    account = new GarenaAccount() { Id = 0, AccountName = accountName, Password = passWord, CountCheck = 99 }; // 99 la khoi tao cho minh
                }
                else
                {
                    account = new GarenaAccount().GetAccount();
                }
            }
            else if (topupType == 11)
            {
                account = new GarenaAccount() { Id = 0, AccountName = accountName, Password = passWord }; // 99 la khoi tao cho minh
            }
            else
            {
                return new APIResponse((int)ResponseCode.ServiceIsLocked)
                {
                    Description = "Sai loại Game"
                };
            }


            if (account == null)
            {
                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Get Account", "NULL" });
                return new APIResponse((int)ResponseCode.TransactionFailed);
            }

            var getTopup = new GameCookie();
            var decaptcha = new Captcha().GetCaptcha(11, "garena:" + account.AccountName);
            var sid = UtilsGarena.GenSid(account.AccountName);

            if (decaptcha == null)
            {
                if (topupType == 9)
                {
                    getTopup = GetTopup(account.AccountName, account.Password, tranId, tranId3rd);
                }
                else if (topupType == 11)
                {
                    getTopup = GetTopupOpenId(account.AccountName, tranId, tranId3rd);
                }

                if (getTopup.IsTopup)
                {
                    var sessionCaptcha = new Captcha();
                    sessionCaptcha.SessionId = sid;
                    sessionCaptcha.Type = 11;
                    sessionCaptcha.Value = string.Empty;
                    sessionCaptcha.TaskId = 0;
                    sessionCaptcha.ImgBase64 = string.Empty;
                    sessionCaptcha.Add();

                    getTopup.HtmlContent = string.Empty;
                    getTopup.SessionId = sid;
                    UtilsGarena.SetCookieCache(sid, getTopup);
                }
                else
                {
                    if (getTopup.HtmlContent.Contains("captcha"))
                        account.Status = -1;
                    else
                        account.Status = 0;
                    account.Update();

                    if (account.CountCheck == 99)
                    {
                        return new APIResponse((int)ResponseCode.ServiceIsLocked)
                        {
                            Description = getTopup.HtmlContent
                        };
                    }

                    return new APIResponse((int)ResponseCode.AccountLocked)
                    {
                        Description = getTopup.HtmlContent
                    };
                }

            }
            else
            {
                getTopup = UtilsGarena.GetCookieCache(decaptcha.SessionId);
                if (getTopup == null)
                {
                    if (topupType == 9)
                    {
                        getTopup = GetTopup(account.AccountName, account.Password, tranId, tranId3rd);
                    }
                    else if (topupType == 11)
                    {
                        getTopup = GetTopupOpenId(account.AccountName, tranId, tranId3rd);
                    }

                    if (getTopup.IsTopup)
                    {
                        var sessionCaptcha = new Captcha();
                        sessionCaptcha.SessionId = sid;
                        sessionCaptcha.Type = 11;
                        sessionCaptcha.Value = string.Empty;
                        sessionCaptcha.TaskId = 0;
                        sessionCaptcha.ImgBase64 = string.Empty;
                        sessionCaptcha.Add();

                        getTopup.HtmlContent = string.Empty;
                        getTopup.SessionId = sid;
                        UtilsGarena.SetCookieCache(sid, getTopup);
                    }
                    else
                    {
                        if (getTopup.HtmlContent.Contains("captcha"))
                            account.Status = -1;
                        else
                            account.Status = 0;

                        account.Update();

                        if (account.CountCheck == 99)
                        {
                            return new APIResponse((int)ResponseCode.ServiceIsLocked)
                            {
                                Description = getTopup.HtmlContent
                            };
                        }

                        return new APIResponse((int)ResponseCode.AccountLocked)
                        {
                            Description = getTopup.HtmlContent
                        };
                    }
                }
            }

            if (getTopup != null)
            {

                var res = string.Empty;
                if (topupType == 9)
                {
                    res = Topup(cardSerial, cardCode, mobile, getTopup, tranId, tranId3rd, account.CountCheck == 99 ? true : false);
                }
                else if (topupType == 11)
                {
                    res = TopupOpenId(cardSerial, cardCode, mobile, getTopup, tranId, tranId3rd);
                }

                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "TopupCard", "Response", account.AccountName, cardSerial, cardCode, res });

                if (res.Contains("Success"))
                {
                    account.Status = 1;
                    account.Update();
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        Description = res,
                        ResponseContent = Regex.Match(res, @"(\d+.)+").Value.Trim().Replace(",", "")
                    };
                }

                if (res.Contains("error_used_card"))
                {
                    account.Status = 1;
                    account.Update();
                    return new APIResponse((int)ResponseCode.CardUsed)
                    {
                        Description = res
                    };
                }

                if (res.Contains("error_invalid_card"))
                {
                    account.Status = 1;
                    account.Update();
                    return new APIResponse((int)ResponseCode.CardCodeInvalid)
                    {
                        Description = res
                    };
                }

                if (res.Contains("error_decaptcha_failed"))
                {
                    account.Status = 1;
                    account.Update();

                    return new APIResponse((int)ResponseCode.ParameterInvalid)
                    {
                        Description = res
                    };
                }

                if (res.Contains("error_require_login"))
                {
                    account.Status = 1;
                    account.Update();

                    new Captcha().DeleteCaptcha(11, "garena:" + account.AccountName);

                    return new APIResponse((int)ResponseCode.ParameterInvalid)
                    {
                        Description = res
                    };
                }

                if (res.Contains("error_friend_username"))
                {
                    account.Status = 1;
                    account.Update();
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = res
                    };
                }

                if (res.Contains("error_pay_empty_or_timeout"))
                {
                    account.Status = 1;
                    account.Update();
                    return new APIResponse((int)ResponseCode.TransactionTimeout)
                    {
                        Description = res
                    };
                }

            }

            account.Status = 1;
            account.Update();

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public static GameCookie GetTopup(string accountName, string passWord, string tranId, string tranId3rd)
        {
            try
            {
                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "GetTopup", "Request", accountName, passWord });
                var urlpreLogin = string.Format("https://auth.garena.com/api/prelogin?account={0}&format=json&id={1}&app_id=10017", accountName, (long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
                var preLoginResult = Task.Run(() => UtilsGarena.GetTask(urlpreLogin, new CookieContainer())).Result;
                var tryAgain = 0;
                while ((preLoginResult.HtmlContent.Contains("error_require_captcha") || preLoginResult.HtmlContent.Contains("error_captcha")) && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "GetTopup", "preLogin", "Try", tryAgain.ToString(), preLoginResult.HtmlContent });
                    var captcha_key = Sercurity.Encrypts.MD5(((long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds).ToString());
                    var urlCaptcha = string.Format("https://gop.captcha.garena.com/image?key={0}", captcha_key);
                    var captcha = UtilsGarena.DeCaptcha(urlCaptcha, preLoginResult.CookieContainer, tranId, tranId3rd);
                    urlpreLogin = string.Format("https://auth.garena.com/api/prelogin?account={0}&captcha_key={1}&captcha={2}&format=json&id={3}&app_id=10017", accountName, captcha_key, captcha, DateTime.Now.Subtract(new DateTime(1970, 1, 9, 0, 0, 00)).TotalSeconds);
                    preLoginResult = Task.Run(() => UtilsGarena.GetTask(urlpreLogin, new CookieContainer())).Result;
                    tryAgain++;
                }
                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "GetTopup", "preLogin", preLoginResult.HtmlContent });

                if (preLoginResult.HtmlContent.Contains("error"))
                {

                    return new GameCookie()
                    {
                        IsTopup = false,
                        HtmlContent = preLoginResult.HtmlContent
                    };
                }

                var preLogin = serializer.Deserialize<PreLogin>(preLoginResult.HtmlContent);

                var s = ChromeDriverService.CreateDefaultService();
                s.HideCommandPromptWindow = true;
                ChromeDriver d = new ChromeDriver(s);

                //Ecrypt
                var urlEncrypt = "http://149.28.130.246:9091/index.html?v1={0}&v2={1}&password={2}";
                //var urlEncrypt = "http://127.0.0.1:9091/index.html?v1={0}&v2={1}&password={2}";
                d.Navigate().GoToUrl(string.Format(urlEncrypt, preLogin.v1, preLogin.v2, HttpUtility.UrlEncode(passWord)));

                IJavaScriptExecutor e = (IJavaScriptExecutor)d;
                string encryptedPassword = (string)e.ExecuteScript("return document.title");
                s.Dispose();
                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "GetTopup", "encryptedPassword", encryptedPassword, string.Format(urlEncrypt, preLogin.v1, preLogin.v2, UtilsGarena.Base64Encode(passWord)) });

                //Login
                var urlLogin = string.Format("https://auth.garena.com/api/login?account={0}&password={1}&format=json&id={2}&app_id=10017", accountName, encryptedPassword, (long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
                var loginResult = Task.Run(() => UtilsGarena.GetTask(urlLogin, preLoginResult.CookieContainer)).Result;
                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "GetTopup", "login", loginResult.HtmlContent });


                if (loginResult.HtmlContent.Contains("error_auth"))
                {

                    return new GameCookie()
                    {
                        IsTopup = false,
                        HtmlContent = "Đăng nhập thất bại: sai tên tài khoản hoặc mật khẩu"
                    };
                }
                if (loginResult.HtmlContent.Contains("error_user_ban"))
                {

                    return new GameCookie()
                    {
                        IsTopup = false,
                        HtmlContent = "Đăng nhập thất bại: Tài khoản bị khóa"
                    };
                }

                //Grant Token
                var urlGrantToken = "https://auth.garena.com/oauth/token/grant";
                var param = new Dictionary<string, string>();
                param.Add("client_id", "10017");
                param.Add("redirect_uri", "https://napthe.vn/app/10090");
                param.Add("response_type", "token");
                param.Add("platform", "1");
                param.Add("locale", "vi-VN");
                param.Add("theme", "white");
                param.Add("format", "json");
                param.Add("id", ((long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds).ToString());
                param.Add("app_id", "10017");

                var tokenReult = Task.Run(() => UtilsGarena.PostTask(urlGrantToken, param, loginResult.CookieContainer)).Result;

                //if (!string.IsNullOrEmpty(tokenReult.HtmlContent))
                //{
                //    if (tokenReult.HtmlContent.Contains("error_no_session"))
                //    {

                //    }
                //}


                var token = serializer.Deserialize<GrantToken>(tokenReult.HtmlContent);
                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "GetTopup", "Token", tokenReult.HtmlContent });

                //InspectToken --> SAVA Cookie đoạn này
                var urlInspectToken = "https://napthe.vn/api/auth/inspect_token";
                var inspectTokenRequest = new InspectToken()
                {
                    token = token.access_token
                };
                var inspectTokenResult = Task.Run(() => UtilsGarena.PostTask(urlInspectToken, serializer.Serialize(inspectTokenRequest), tokenReult.CookieContainer)).Result;
                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "GetTopup", "inspectToken", inspectTokenResult.HtmlContent });
                inspectTokenResult.IsTopup = true;
                return inspectTokenResult;

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Error", accountName, passWord, e.Message });
                return new GameCookie()
                {
                    IsTopup = false,
                    HtmlContent = HttpUtility.HtmlDecode("Error:" + e.Message)
                };
            }
        }

        public static GameCookie GetTopupOpenId(string accountName, string tranId, string tranId3rd)
        {
            try
            {
                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "GetTopup OpenId", "Request", accountName });
                //var preLogin = Task.Run(() => UtilsGarena.GetTask("https://napthe.vn/app/100067/idlogin", new CookieContainer())).Result;
                //Grant Token
                var urlGrantToken = "https://napthe.vn/api/auth/player_id_login";
                var param = new LoginOpenIdRequest
                {
                    app_id = 100067,
                    login_id = accountName
                };
                var tokenReult = Task.Run(() => UtilsGarena.PostTask(urlGrantToken, serializer.Serialize(param), new CookieContainer())).Result;

                var token = serializer.Deserialize<LoginOpenId>(tokenReult.HtmlContent);
                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "GetTopup OpenId", "Token", tokenReult.HtmlContent });
                if (!string.IsNullOrEmpty(token.error))
                    return new GameCookie
                    {
                        IsTimeout = false,
                        HtmlContent = token.error
                    };

                tokenReult.IsTopup = true;
                tokenReult.RequestVerificationToken = token.open_id;
                return tokenReult;

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Error OpenId", accountName, e.Message });
                return new GameCookie()
                {
                    IsTopup = false,
                    HtmlContent = HttpUtility.HtmlDecode("Error:" + e.Message)
                };
            }
        }

        public static string Topup(string cardSerial, string cardCode, string mobile, GameCookie garenaCookie, string tranId, string tranId3rd, bool self)
        {
            var result = string.Empty;

            //Charge
            //var cardCode = "2822001815634124";
            string friend_username = null;

            if (!self)
            {
                friend_username = mobile;
            }

            var payRequest = new PayRequest()
            {
                app_id = 10090,
                service = "pc",
                packed_role_id = 0,
                channel_id = 205000,
                channel_data = new ChannelData()
                {
                    card_password = cardCode,
                    friend_username = friend_username
                }
            };

            try
            {
                var urlPreflight = "https://napthe.vn/api/preflight";
                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Topup", "Request", cardSerial, cardCode, mobile, "{}", urlPreflight });
                var preflightResult = Task.Run(() => UtilsGarena.PostTask(urlPreflight, "{}", garenaCookie.CookieContainer)).Result;
                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Topup", "Response", cardSerial, cardCode, mobile, "{}", urlPreflight, preflightResult.HtmlContent });

                var csrf = string.Empty;
                foreach (System.Net.Cookie cookie in preflightResult.CookieContainer.GetCookies(new Uri(urlPreflight)))
                {
                    if (cookie.Name == "__csrf__")
                    {
                        csrf = cookie.Value;
                        break;
                    }
                }

                var header = new Dictionary<string, string>();
                header.Add("x-csrf-token", csrf);

                var urlPay = "https://napthe.vn/api/shop/pay/init?language=vi&region=VN";
                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Topup", "Request", cardSerial, cardCode, mobile, serializer.Serialize(payRequest), urlPay });
                var payResult = Task.Run(() => UtilsGarena.PostTask(urlPay, serializer.Serialize(payRequest), preflightResult.CookieContainer, header)).Result;

                if (payResult.HtmlContent.Contains("error_require_login"))
                {
                    return "error_require_login";
                }

                var tryAgain = 0;
                while ((payResult.HtmlContent.Contains("error_require_captcha") || payResult.HtmlContent.Contains("error_captcha")) && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Topup", "Try", tryAgain.ToString(), cardSerial, cardCode, mobile, payResult.HtmlContent });
                    var captcha_key = Sercurity.Encrypts.MD5(((long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds).ToString());
                    var urlCaptcha = string.Format("https://gop.captcha.garena.com/image?key={0}", captcha_key);
                    var captcha = UtilsGarena.DeCaptcha(urlCaptcha, payResult.CookieContainer, tranId, tranId3rd);

                    if (!string.IsNullOrEmpty(captcha))
                    {
                        var payRequestCaptcha = new PayRequestCaptcha()
                        {
                            app_id = 10090,
                            service = "pc",
                            packed_role_id = 0,
                            channel_id = 205000,
                            channel_data = new ChannelDataCaptcha()
                            {
                                card_password = cardCode,
                                captchaKey = captcha_key,
                                captcha = captcha,
                                friend_username = mobile
                            },
                            captcha_key = captcha_key,
                            captcha = captcha

                        };
                        payResult = Task.Run(() => UtilsGarena.PostTask(urlPay, serializer.Serialize(payRequestCaptcha), garenaCookie.CookieContainer)).Result;
                    }
                    else
                    {
                        NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Topup", "error_require_captcha", "Captcha Service Failed", cardSerial, cardCode, mobile, payResult.HtmlContent });
                        return "error_decaptcha_failed";
                    }

                    tryAgain++;
                }

                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Topup", "Response", cardSerial, cardCode, mobile, payResult.HtmlContent });

                if (string.IsNullOrEmpty(payResult.HtmlContent))
                {
                    return "error_pay_empty_or_timeout";
                }

                var payResponse = serializer.Deserialize<PayResponse>(payResult.HtmlContent);
                if (payResponse.result == "success")
                {
                    //CheckCharge
                    var urlCheckPay = "https://napthe.vn/api/shop/pay/poll";
                    var checkPayRequest = new PayCheckRequest()
                    {
                        display_id = payResponse.display_id
                    };
                    var checkPayResult = Task.Run(() => UtilsGarena.PostTask(urlCheckPay, serializer.Serialize(checkPayRequest), payResult.CookieContainer)).Result;
                    NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Topup", "Response checkpay", payResponse.display_id, checkPayResult.HtmlContent });
                    var payCheckResponse = serializer.Deserialize<PayCheckResponse>(checkPayResult.HtmlContent);
                    result = "Success: " + payCheckResponse.point_amount / 2 * 1000;
                }
                else
                {
                    //{"display_id":"15807125233508078763","result":"error_invalid_card","exec":{"display_id":"15807125233508078763"}} --> CardCode Invalid
                    //{"display_id":"15807125233508078763","result":"error_used_card","exec":{"display_id":"15807125233508078763"}} --> CardUsed or Invalid
                    //{ "display_id":"13511475890774449061","result":"error_friend_username","exec":{ "display_id":"13511475890774449061"} } --> User ko Tồn Tại
                    //{"error":"error_require_captcha"}
                    //NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Topup", "Error", cardSerial, cardCode, mobile, payResponse.result });
                    result = payResponse.result;
                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Topup", "Error", cardSerial, cardCode, mobile, e.Message });
                return "error_exception";
            }

            return result;
        }

        public static string TopupOpenId(string cardSerial, string cardCode, string accountName, GameCookie garenaCookie, string tranId, string tranId3rd)
        {
            var result = string.Empty;

            var payRequest = new PayRequestOpenId()
            {
                service = "pc",
                app_id = 100067,
                packed_role_id = 0,
                channel_id = 205000,
                channel_data = new ChannelData()
                {
                    card_password = cardCode,
                    friend_username = null
                },
                open_id = garenaCookie.RequestVerificationToken

            };

            try
            {
                var urlPay = "https://napthe.vn/api/shop/pay/init?language=vi&region=VN";

                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Topup OpenId", "Request", cardSerial, cardCode, accountName, serializer.Serialize(payRequest), urlPay });
                var payResult = Task.Run(() => UtilsGarena.PostTask(urlPay, serializer.Serialize(payRequest), garenaCookie.CookieContainer)).Result;

                if (payResult.HtmlContent.Contains("error_require_login"))
                {
                    return "error_require_login";
                }

                var tryAgain = 0;
                while ((payResult.HtmlContent.Contains("error_require_captcha") || payResult.HtmlContent.Contains("error_captcha")) && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Topup OpenId", "Try", tryAgain.ToString(), cardSerial, cardCode, accountName, payResult.HtmlContent });
                    var captcha_key = Sercurity.Encrypts.MD5(DateTime.Now.Subtract(new DateTime(1970, 1, 9, 0, 0, 00)).TotalSeconds.ToString());
                    var urlCaptcha = string.Format("https://gop.captcha.garena.com/image?key={0}", captcha_key);
                    var captcha = UtilsGarena.DeCaptcha(urlCaptcha, payResult.CookieContainer, tranId, tranId3rd);

                    if (!string.IsNullOrEmpty(captcha))
                    {
                        var payRequestCaptcha = new PayRequestCaptchaOpenId()
                        {
                            app_id = 100067,
                            service = "pc",
                            packed_role_id = 0,
                            channel_id = 205000,
                            channel_data = new ChannelDataCaptcha()
                            {
                                card_password = cardCode,
                                captchaKey = captcha_key,
                                captcha = captcha,
                                friend_username = null
                            },
                            captcha_key = captcha_key,
                            captcha = captcha,
                            open_id = garenaCookie.RequestVerificationToken

                        };
                        payResult = Task.Run(() => UtilsGarena.PostTask(urlPay, serializer.Serialize(payRequestCaptcha), garenaCookie.CookieContainer)).Result;
                    }
                    else
                    {
                        NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Topup OpenId", "error_require_captcha", "Captcha Service Failed", cardSerial, cardCode, accountName, payResult.HtmlContent });
                        return "error_decaptcha_failed";
                    }

                    tryAgain++;
                }

                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Topup OpenId", "Response", cardSerial, cardCode, accountName, payResult.HtmlContent });

                if (string.IsNullOrEmpty(payResult.HtmlContent))
                {
                    return "error_pay_empty_or_timeout";
                }

                var payResponse = serializer.Deserialize<PayResponse>(payResult.HtmlContent);
                if (payResponse.result == "success")
                {
                    //CheckCharge
                    var urlCheckPay = "https://napthe.vn/api/shop/pay/poll";
                    var checkPayRequest = new PayCheckRequest()
                    {
                        display_id = payResponse.display_id
                    };
                    var checkPayResult = Task.Run(() => UtilsGarena.PostTask(urlCheckPay, serializer.Serialize(checkPayRequest), payResult.CookieContainer)).Result;
                    var payCheckResponse = serializer.Deserialize<PayCheckResponse>(checkPayResult.HtmlContent);
                    result = "Success: " + payCheckResponse.currency_amount / 2 * 1000;
                }
                else
                {
                    //{"display_id":"15807125233508078763","result":"error_invalid_card","exec":{"display_id":"15807125233508078763"}} --> CardCode Invalid
                    //{"display_id":"15807125233508078763","result":"error_used_card","exec":{"display_id":"15807125233508078763"}} --> CardUsed or Invalid
                    //{ "display_id":"13511475890774449061","result":"error_friend_username","exec":{ "display_id":"13511475890774449061"} } --> User ko Tồn Tại
                    //{"error":"error_require_captcha"}
                    //NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Topup", "Error", cardSerial, cardCode, mobile, payResponse.result });
                    result = payResponse.result;
                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GarenaService", tranId, tranId3rd, "Topup OpenId", "Error", cardSerial, cardCode, accountName, e.Message });
                return "error_exception";
            }

            return result;
        }
    }
}