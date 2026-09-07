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
    public class DzoService
    {

        static JavaScriptSerializer serializer = new JavaScriptSerializer();

        public static APIResponse TopupCard(string cardSerial, string cardCode, string mobile, string tranId, string tranId3rd, string accountName, string passWord)
        {

            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(passWord))
            {
                return new APIResponse((int)ResponseCode.ServiceIsLocked)
                {
                    Description = "Mobile or AccountName or Password is Empty"
                };
            }

            var getTopup = new GameCookie();
            var decaptcha = new Captcha().GetCaptcha(18, "dzo:" + accountName);
            var sid = UtilsDzo.GenSid(accountName);

            if (decaptcha == null)
            {
                getTopup = GetTopup(accountName, passWord, tranId, tranId3rd);

                if (getTopup.IsTopup)
                {
                    decaptcha = UtilsDzo.DeCaptcha("https://vi.dzogame.vn/apis/Captcha.ashx", sid, getTopup.CookieContainer, tranId, tranId3rd);
                    decaptcha.Type = 18;
                    decaptcha.Add();
                    getTopup.SessionId = sid;
                    getTopup.HtmlContent = String.Empty;
                    UtilsDzo.SetCookieCache(sid, getTopup);
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
                getTopup = UtilsDzo.GetCookieCache(decaptcha.SessionId);

            }

            if (getTopup != null)
            {
                var res = Topup(cardSerial, cardCode, mobile, decaptcha.Value, getTopup, tranId, tranId3rd);
                NLogLogger.Info(new string[] { "DzoService", tranId, tranId3rd, "TopupCard", "Response", accountName, cardSerial, cardCode, res });

                //for (int i = 0; i < 1; i++)
                //{
                //    Action<string, string, GameCookie> send = PreGenCaptCha;
                //    send.BeginInvoke(accountName, passWord, getTopup, null, null);
                //}

                if (res.Contains("success"))
                {

                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        Description = res,
                        ResponseContent = Regex.Match(res, @"(\d+.)+").Value.Trim().Replace(",", "")
                    };
                }

                if (res.Contains("The da duoc su dung") || res.Contains("Thẻ đã được nạp thành công trước đó"))
                {

                    return new APIResponse((int)ResponseCode.CardUsed)
                    {
                        Description = res
                    };
                }

                if (res.Contains("ERROR_VALIDATE_CARD")) //Lỗi: Mã số thẻ không hợp lệ!
                {

                    return new APIResponse((int)ResponseCode.CardCodeInvalid)
                    {
                        Description = res
                    };
                }

                
                if (res.Contains("REDIRECT_TOPUP") || res.Contains("ERROR_CAPTCHA") || res.Contains("ERROR_EXCEPTION")) // Phế bài làm lại 
                {

                    new Captcha().DeleteCaptcha(18, "dzo:" + accountName);

                    return new APIResponse((int)ResponseCode.ParameterInvalid)
                    {
                        Description = res
                    };
                }

                if (res.Contains("tam khoa 30 phut"))
                {

                    
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = "User nhap sai thong tin the, tam khoa 30 phut."
                    };
                }

                

            }

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public static GameCookie GetTopup(string accountName, string passWord, string tranId, string tranId3rd)
        {
            try
            {
                NLogLogger.Info(new string[] { "DzoService", tranId, tranId3rd, "GetTopup", "Request", accountName, passWord });
                var urlpreLogin = "https://vi.dzogame.vn/dang-nhap";
                var urlCaptcha = "https://vi.dzogame.vn/apis/Captcha.ashx";
                var urlLogin = "https://vi.dzogame.vn/apis/auth.ashx";
                var preLoginResult = Task.Run(() => UtilsDzo.GetTask(urlpreLogin, new CookieContainer())).Result;
                var loginMessage = string.Empty;
                var tryAgain = 0;
                DzoGame.Login loginObj = new DzoGame.Login()
                {
                    status = -99 //start
                };
                while ((loginObj.status == -99 || loginObj.status == 3) && tryAgain < 3)
                {
                    var captcha = UtilsDzo.DeCaptcha(urlCaptcha, "n/a", preLoginResult.CookieContainer, tranId, tranId3rd);
                    var param = new Dictionary<string, string>();
                    param.Add("t", "28");
                    param.Add("email", accountName);
                    param.Add("password", passWord);
                    param.Add("captcha", captcha.Value);
                    var loginResult = Task.Run(() => UtilsDzo.PostTask(urlLogin, param, preLoginResult.CookieContainer)).Result;
                    NLogLogger.Info(new string[] { "DzoService", tranId, tranId3rd, "GetTopup", "login", loginResult.HtmlContent });
                    loginObj = serializer.Deserialize<DzoGame.Login>(loginResult.HtmlContent);
                    if (loginObj.status == 0)
                    {
                        loginResult.IsTopup = true;
                        return loginResult;
                    }
                    else if (loginObj.status == 2)
                    {
                        return new GameCookie()
                        {
                            IsTopup = false,
                            HtmlContent = loginObj.msg
                        };
                    }
                    else
                    {
                        loginMessage = loginObj.msg;
                    }

                    tryAgain++;
                }

                return new GameCookie()
                {
                    IsTopup = false,
                    HtmlContent = loginMessage
                };

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "DzoService", tranId, tranId3rd, "Error", accountName, passWord, e.Message });
                return new GameCookie()
                {
                    IsTopup = false,
                    HtmlContent = HttpUtility.HtmlDecode("Error:" + e.Message)
                };
            }
        }

        public static string Topup(string cardSerial, string cardCode, string mobile, string answer, GameCookie gameCookie, string tranId, string tranId3rd)
        {
            var result = string.Empty;

            //Charge
            //var cardCode = "2822001815634124";
            string friend_username = null;

            var cardRequest = new DzoGame.CardRequest()
            {
                serialCode = cardCode,
                serialNumber = cardSerial,
                partner = "VN-GTE",
                rate = string.Empty,
                capchar = answer
            };


            var param = new Dictionary<string, string>();
            param.Add("t", "2");
            param.Add("data", serializer.Serialize(cardRequest));
           
            try
            {
                var urlPay = "https://vi.dzogame.vn/apis/payment.ashx";

                NLogLogger.Info(new string[] { "DzoService", tranId, tranId3rd, "Topup", "Request", cardSerial, cardCode, mobile, serializer.Serialize(param), urlPay });
                var payResult = Task.Run(() => UtilsDzo.PostTask(urlPay, param, gameCookie.CookieContainer)).Result;
                NLogLogger.Info(new string[] { "DzoService", tranId, tranId3rd, "Topup", "Response", payResult.HtmlContent });
                var payResponse = serializer.Deserialize<DzoGame.TopupResponse>(payResult.HtmlContent);
                if (payResponse.status == "100")
                {
                    //CheckCharge
                    var vndAdd = payResponse.data.balanceAdd / 0.96;
                    result = "success " + vndAdd;
                }
                else if (payResponse.status == "3")
                {
                    result = "ERROR_CAPTCHA";
                }
                else
                {
                    result = payResponse.msg +  " | " + payResponse.errorMsg;
                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "DzoService", tranId, tranId3rd, "Topup", "Error", cardSerial, cardCode, mobile, e.Message });
                return "ERROR_EXCEPTION";
            }

            return result;
        }

        private static void PreGenCaptCha(string accountName, string passWord, GameCookie gameCookie)
        {

            var sid = UtilsDzo.GenSid(accountName);
            var getTopup = Task.Run(() => UtilsDzo.GetTask("https://vi.dzogame.vn/nap-tien", gameCookie.CookieContainer)).Result;

            if (!string.IsNullOrEmpty(getTopup.CaptChaLink))
            {
                var sessionCaptcha = UtilsDzo.DeCaptcha("https://vi.dzogame.vn/apis/Captcha.ashx", sid, getTopup.CookieContainer, "", "");
                sessionCaptcha.Type = 18;
                sessionCaptcha.Add();
                getTopup.SessionId = sid;
                getTopup.HtmlContent = String.Empty;
                UtilsDzo.SetCookieCache(sid, getTopup);
            }


        }
    }
}