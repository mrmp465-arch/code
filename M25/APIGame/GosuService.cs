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
using Lib.Captcha;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APIGame
{
    public class GosuService
    {
        static readonly string baseUrl = "https://pay.gosu.vn";
        static JavaScriptSerializer serializer = new JavaScriptSerializer();

        public static APIResponse TopupCard(string cardSerial, string cardCode, string mobile, string cardType, string accountName, string passWord)
        {

            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(passWord))
            {
                return new APIResponse((int)ResponseCode.ServiceIsLocked)
                {
                    Description = "Mobile or AccountName or Password is Empty"
                };
            }

            var getTopup = new GameCookie();
            var decaptcha = new Captcha().GetCaptcha(7, "gosu:" + accountName);
            if (decaptcha == null)
            {
                var sid = UtilsGosu.GenSid(accountName);
                getTopup = GetTopup(accountName, passWord);
                if (getTopup.IsTopup)
                {
                    decaptcha = UtilsGosu.DeCaptcha(baseUrl + getTopup.CaptChaLink, sid, getTopup);
                    getTopup.HtmlContent = string.Empty;
                    getTopup.SessionId = sid;
                    UtilsGosu.SetCookieCache(sid, getTopup);
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
                getTopup = UtilsGosu.GetCookieCache(decaptcha.SessionId);
            }

            if (getTopup != null)
            {
                var res = Topup(cardSerial, cardCode, decaptcha.Value, cardType, getTopup);
                NLogLogger.Info(new string[] { "GosuService", "TopupCard", "Response", accountName, cardSerial, cardCode, res });

                //for (int i = 0; i < 1; i++)
                //{
                //    Action<string, string, GameCookie> send = PreGenCaptCha;
                //    send.BeginInvoke(accountName, passWord, getTopup, null, null);
                //}

                if (res.Contains("Bạn đã nạp"))
                {
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        Description = res,
                        ResponseContent = Regex.Match(res, @"(\d+.)+").Value.Trim().Replace(",", "")
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

                if (res.Contains("Thẻ đã sử dụng thành công trên hệ thống")
                    || res.Contains("Thẻ đã sử dụng")
                    || res.Contains("Thẻ đã được sử dụng")
                    )
                {
                    return new APIResponse((int)ResponseCode.CardUsed)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Dữ liệu đầu vào không hợp lệ")
                    || res.Contains("Thẻ không tồn tại")
                    || res.Contains("Thẻ không hợp lệ")
                    || res.Contains("Thông tin thẻ không hợp lệ"))
                {
                    return new APIResponse((int)ResponseCode.CardFormatInvalid)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Serial không hợp lệ"))
                {
                    return new APIResponse((int)ResponseCode.CardSerialInvalid)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Thông tin mã thẻ không hợp lệ")
                    || res.Contains("Mã Pin không hợp lệ"))
                {
                    return new APIResponse((int)ResponseCode.CardCodeInvalid)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Mã bảo vệ không đúng"))
                {
                    return new APIResponse((int)ResponseCode.ParameterInvalid)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Bạn đã thao tác quá nhanh")) //Bạn đã thao tác quá nhanh, hãy thử lại sau 15s.
                {
                    //Thread.Sleep(16000);
                    return new APIResponse((int)ResponseCode.TransactionRejected)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Không thể thực hiện giao dịch") || res.Contains("Quá trình gạch thẻ không thành công"))
                {
                    //Thread.Sleep(16000);
                    return new APIResponse((int)ResponseCode.TransactionIgnore)
                    {
                        Description = res
                    };
                }


                //if (res.Contains("Hệ thống đang bảo trì xin vui lòng quay lại sau")
                //    || res.Contains("Thầy cô không có quyền thực hiện nạp thẻ"))
                //{
                //    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                //    {
                //        Description = res
                //    };
                //}
            }

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public static GameCookie GetTopup(string accountName, string passWord)
        {
            NLogLogger.Info(new string[] { "GosuService", "GetTopup", "Request", accountName, passWord });


            //string baseUrl = "https://smas.edu.vn";

            var getHome = Task.Run(() => UtilsGosu.GetTask("https://pay.gosu.vn/", new CookieContainer())).Result;

            var getLogin = Task.Run(() => UtilsGosu.GetTask("https://pay.gosu.vn/Account/LogOn?ReturnUrl=%2fPayment%2fRecharge%3fType%3dcard&Type=card", getHome.CookieContainer)).Result;
            var parameters = new Dictionary<string, string>();
            parameters.Add("ReturnURL", "/Payment/Recharge?Type=card");
            parameters.Add("client_id", string.Empty);
            parameters.Add("UserName", accountName);
            parameters.Add("Password", Encrypts.MD5(passWord).ToLower());


            var getTopup = Task.Run(() => UtilsGosu.PostTask("https://pay.gosu.vn/Account/Logon", parameters, getLogin.CookieContainer)).Result;

            if (getTopup.IsTimeout)
            {
                return new GameCookie()
                {
                    IsTopup = false,
                    IsTimeout = true,
                    HtmlContent = HttpUtility.HtmlDecode("Login Timeout")
                };
            }

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(getTopup.HtmlContent);

            var detectSuccess = getTopup.HtmlContent.Contains("T&#224;i khoản đăng nhập kh&#244;ng đ&#250;ng");
            if (detectSuccess)
            {
                NLogLogger.Info(new string[] { "GosuService", "Topup", "Error Login", serializer.Serialize(parameters) });
                return new GameCookie()
                {
                    IsTopup = false,
                    HtmlContent = HttpUtility.HtmlDecode("T&#224;i khoản đăng nhập kh&#244;ng đ&#250;ng")
                };
            }

            //var getTopupPre = Task.Run(() => UtilsGosu.GetTask("https://pay.gosu.vn/Payment/Prepay", loginResponse.CookieContainer)).Result;

            //var getTopup = Task.Run(() => UtilsGosu.GetTask("https://pay.gosu.vn/Payment/Recharge?Type=card", loginResponse.CookieContainer)).Result;
            //if (getTopup == null)
            //{
            //    NLogLogger.Info(new string[] { "GosuService", "Topup", "Error Locked 30 min", serializer.Serialize(parameters) });
            //    return new GameCookie()
            //    {
            //        IsTopup = false,
            //        HtmlContent = "Locked 30 min"
            //    }; //Bi khoa 15P
            //}

            getTopup.IsTopup = true;

            return getTopup;

        }

        public static string Topup(string cardSerial, string cardCode, string answer, string cardType, GameCookie smasCookie)
        {


           

            var result = string.Empty;
            var parameters = new Dictionary<string, string>();
            GameCookie postTopup = null;
            try
            {
                parameters.Add("__RequestVerificationToken", smasCookie.RequestVerificationToken);
                parameters.Add("rdType", cardType);
                parameters.Add("Serial", cardSerial);
                parameters.Add("PinNumber", cardCode);
                //parameters.Add("Captcha", answer);
                parameters.Add("Type", "card");
                parameters.Add("PromoRate", "1");
                parameters.Add("PromoText", "");
                parameters.Add("Amount", "100000");
                parameters.Add("PhoneNumber", "0392332171");
                parameters.Add("BankID", "ABC");

                NLogLogger.Info(new string[] { "GosuService", "Topup", "Request", serializer.Serialize(parameters) });

                postTopup = Task.Run(() => UtilsGosu.PostTask("https://pay.gosu.vn/Payment/Recharge", parameters, smasCookie.CookieContainer)).Result;

                if (postTopup.IsTimeout)
                {
                    NLogLogger.Info(new string[] { "GosuService", "Topup", "Error TimeOut", serializer.Serialize(parameters) });
                    return "Post Topup Ignore Timeout";
                }

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(postTopup.HtmlContent);
                var messageSuccess = string.Empty;
                var messageError = string.Empty;
                var messageCaptcha = string.Empty;
                try
                {
                    messageSuccess = doc.DocumentNode.SelectSingleNode("//div[@class='box_mid']//p") != null ? doc.DocumentNode.SelectSingleNode("//div[@class='box_mid']//p").InnerText.Trim() : string.Empty;
                    messageError = doc.DocumentNode.SelectSingleNode("//span[@class='field-validation-error field-validation-error Mgs']") != null ? doc.DocumentNode.SelectSingleNode("//span[@class='field-validation-error field-validation-error Mgs']").InnerText.Trim() : string.Empty;
                    messageCaptcha = doc.DocumentNode.SelectSingleNode("//span[@class ='field-validation-error error_msg']") != null ? doc.DocumentNode.SelectSingleNode("//span[@class ='field-validation-error error_msg']").InnerText.Trim() : string.Empty;
                }
                catch (Exception e)
                {
                    NLogLogger.Info(new string[] { "GosuService", "Result", cardSerial, cardCode, answer, postTopup.HtmlContent, e.Message });
                    return "Post Topup Ignore Null";
                }


                if (!string.IsNullOrEmpty(messageSuccess))
                {
                    result = messageSuccess;
                }
                if (!string.IsNullOrEmpty(messageError))
                {
                    result = HttpUtility.HtmlDecode(messageError);
                }
                if (!string.IsNullOrEmpty(messageCaptcha))
                {
                    result = HttpUtility.HtmlDecode(messageCaptcha.Trim());
                }

                NLogLogger.Info(new string[] { "GosuService", "Result", cardSerial, cardCode, answer, result });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GosuService", "Topup", "Error", serializer.Serialize(parameters), e.Message, smasCookie.SessionId });
                return "Post Topup Ignore Exeption";
            }

            return result;
        }

        //private static void PreGenCaptCha(string accountName, string passWord, GameCookie smasCookie)
        //{

        //    var sid = UtilsGosu.GenSid(accountName);
        //    var getTopup = Task.Run(() => GetTopup(accountName, passWord)).Result;

        //    //var getTopup = Task.Run(() => UtilsGosu.GetTask("https://pay.gosu.vn/Payment/Recharge?Type=card", new CookieContainer())).Result;
        //    //if (getTopup.IsTopup)

        //    if (!string.IsNullOrEmpty(getTopup.CaptChaLink))
        //    {
        //        var sessionCaptcha = UtilsGosu.DeCaptcha(baseUrl + getTopup.CaptChaLink, sid, getTopup);
        //        sessionCaptcha.Type = 7;
        //        sessionCaptcha.Add();
        //        getTopup.SessionId = sid;
        //        getTopup.HtmlContent = String.Empty;
        //        UtilsGosu.SetCookieCache(sid, getTopup);
        //    }


        //}

        //private static void ReportCaptcha(int TaskId)
        //{
        //    var result = new Lib.Captcha.Anticaptcha.AnticaptchaService().ReportIncorrectImageCaptcha(TaskId);
        //    NLogLogger.Info(new string[] { "GosuService", "ReportCaptcha", TaskId.ToString(), result.ToString() });
        //}
    }
}