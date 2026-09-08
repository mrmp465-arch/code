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

namespace APIMyViettel
{
    public class SmasService
    {
        static readonly string baseUrl = "https://smas.edu.vn";
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        static bool CheckSerial_MyVTT = bool.Parse(ConfigurationManager.AppSettings["CheckSerialFirst_MyVTT"] ?? "true");
        static bool CheckSerial_Service = bool.Parse(ConfigurationManager.AppSettings["CheckSerialFirst_Service"] ?? "true");

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

            var getTopup = new SmasCookie();
            var decaptcha = new Captcha().GetCaptcha(3, "smas:" + accountName);
            if (decaptcha == null)
            {
                var sid = UtilsSmas.GenSid(accountName);
                getTopup = GetTopup(accountName, passWord);
                if (getTopup.IsTopup)
                {
                    decaptcha = UtilsSmas.DeCaptcha(baseUrl + getTopup.CaptChaLink, sid, getTopup);
                    getTopup.HtmlContent = string.Empty;
                    getTopup.SessionId = sid;
                    UtilsSmas.SetCookieCache(sid, getTopup);
                }
                else
                {
                    return new APIResponse((int) ResponseCode.ServiceIsLocked)
                    {
                        Description = getTopup.HtmlContent
                    };
                }

            }
            else
            {
                getTopup = UtilsSmas.GetCookieCache(decaptcha.SessionId);
            }

            if (getTopup != null)
            {
                var res = Topup(cardSerial, cardCode, decaptcha.Value, getTopup);
                NLogLogger.Info(new string[] { "SmasService", "TopupCard", "Response", accountName, cardSerial, cardCode, res });

                for (int i = 0; i < 1; i++)
                {
                    Action<string, string, SmasCookie> send = PreGenCaptCha;
                    send.BeginInvoke(accountName, passWord, getTopup, null, null);
                }

                if (res.Contains("đã nạp thành công"))
                {
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        Description = res,
                        ResponseContent = Regex.Match(res, @"(\d+.)+").Value.Trim().Replace(".", "")
                    };
                }
                if (res.Contains("Post Topup Ignore Timeout"))
                {
                    //new Captcha().DeleteCaptcha(3, accountName);
                    return new APIResponse((int)ResponseCode.TransactionIgnore)
                    {
                        Description = res
                    };
                }
                if (res.Contains("Post Topup Ignore Null") 
                    || res.Contains("Post Topup Ignore Exeption"))
                {
                    return new APIResponse((int)ResponseCode.ParameterInvalid);
                }

                if (res.Contains("Mã thẻ cào không hợp lệ"))
                {
                    return new APIResponse((int)ResponseCode.CardSerialInvalid)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Thẻ cào không hợp lệ hoặc đã được sử dụng"))
                {
                    return new APIResponse((int)ResponseCode.CardCodeInvalid)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Mã xác nhận không đúng"))
                {
                    return new APIResponse((int)ResponseCode.ParameterInvalid)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Nạp thẻ bị khóa do nhập sai thông tin quá số lần quy định") 
                    || res.Contains("Chức năng nạp thẻ đã bị khóa"))
                {
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = res
                    };
                }
                if (res.Contains("Không thể thực hiện giao dịch"))
                {
                    return new APIResponse((int)ResponseCode.SystemBusy)
                    {
                        Description = res
                    };
                }
                if (res.Contains("Hệ thống đang bảo trì xin vui lòng quay lại sau")
                    || res.Contains("Thầy cô không có quyền thực hiện nạp thẻ"))
                {
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = res
                    };
                } 
            }

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public static SmasCookie GetTopup(string accountName, string passWord)
        {
            NLogLogger.Info(new string[] { "SmasService", "GetTopup", "Request", accountName, passWord });

            string baseUrl = "https://smas.edu.vn";

            var getLogin = Task.Run(() => UtilsSmas.GetTask("https://smas.edu.vn/Home/LogOn", new CookieContainer())).Result;
            var parameters = new Dictionary<string, string>();
            parameters.Add("__RequestVerificationToken", getLogin.RequestVerificationToken);
            parameters.Add("UserName", accountName);
            parameters.Add("Password", passWord);
            parameters.Add("tokenAuthen", string.Empty);
            parameters.Add("LoginVnStudy", "False");

            var loginResponse = Task.Run(() => UtilsSmas.PostTask("https://smas.edu.vn/Home/LogOn", parameters, getLogin.CookieContainer)).Result;

            if (loginResponse.IsTimeout)
            {
                return new SmasCookie()
                {
                    IsTopup = false,
                    IsTimeout = true,
                    HtmlContent = HttpUtility.HtmlDecode("Login Timeout")
                };
            }

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(loginResponse.HtmlContent);

            var detectSuccess = loginResponse.HtmlContent.Contains("T&#234;n đăng nhập hoặc mật khẩu kh&#244;ng ch&#237;nh x&#225;c") || loginResponse.HtmlContent.Contains("đăng nhập kh&#244;ng th&#224;nh c&#244;ng");
            if (detectSuccess)
            {
                NLogLogger.Info(new string[] { "SmasService", "Topup", "Error Login", serializer.Serialize(parameters) });
                return new SmasCookie() {
                    IsTopup = false,
                    HtmlContent = HttpUtility.HtmlDecode("T&#234;n đăng nhập hoặc mật khẩu kh&#244;ng ch&#237;nh x&#225;c || đăng nhập kh&#244;ng th&#224;nh c&#244;ng")
                };
            }

            var rdbSelectLevel = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'level-box')]") != null ? doc.DocumentNode.SelectSingleNode("//div[contains(@class,'level-box')]").GetAttributeValue("value", "empty") : "empty";
            if (rdbSelectLevel != "empty")
            {
                parameters.Clear();
                parameters.Add("__RequestVerificationToken", loginResponse.RequestVerificationToken);
                parameters.Add("RdbSelectLevel", rdbSelectLevel);

                var loginAuthorize = Task.Run(() => UtilsSmas.PostTask("https://smas.edu.vn/Home/AuthorizeAdmin", parameters, loginResponse.CookieContainer)).Result;

                loginResponse = loginAuthorize;
            }

            var getTopup = Task.Run(() => UtilsSmas.GetTask("https://smas.edu.vn/SMSEDUArea/Topup", loginResponse.CookieContainer)).Result;

            if (getTopup == null)
            {
                NLogLogger.Info(new string[] { "SmasService", "Topup", "Error Locked 30 min", serializer.Serialize(parameters) });
                return new SmasCookie() { IsTopup = false, HtmlContent = "Locked 30 min" }; //Bi khoa 15P
            }
            getTopup.IsTopup = true;

            return getTopup;

        }

        public static string Topup(string cardSerial, string cardCode, string answer, SmasCookie smasCookie)
        {
            var result = string.Empty;
            var parameters = new Dictionary<string, string>();
            SmasCookie postTopup = null;
            try
            {
                parameters.Add("__RequestVerificationToken", smasCookie.RequestVerificationToken);
                parameters.Add("PinCard", cardCode);
                parameters.Add("CardSerial", cardSerial);
                parameters.Add("ConfirmationCode", answer);
                NLogLogger.Info(new string[] { "SmasService", "Topup", "Request", serializer.Serialize(parameters) });

                postTopup = Task.Run(() => UtilsSmas.PostTask("https://smas.edu.vn/SMSEDUArea/Topup/Topup", parameters, smasCookie.CookieContainer)).Result;

                if (postTopup.IsTimeout)
                {
                    NLogLogger.Info(new string[] { "SmasService", "Topup", "Error TimeOut", serializer.Serialize(parameters) });
                    return "Post Topup Ignore Timeout";
                }

                if (postTopup.HtmlContent.Contains("Chức năng nạp thẻ đã bị khóa do nhà trường nạp sai thẻ cào quá 3 lần"))
                {
                    return "Chức năng nạp thẻ đã bị khóa";
                }

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(postTopup.HtmlContent);
                var messageSuccess = string.Empty;
                var messageError = string.Empty;
                var messageCaptcha = string.Empty;
                try
                {
                    //messageSuccess = doc.DocumentNode.SelectSingleNode("//p[@class='message-of-success mg-t-5']") != null ? doc.DocumentNode.SelectSingleNode("//p[@class='message-of-success']").InnerText.Trim() : string.Empty;
                    //messageError = doc.DocumentNode.SelectSingleNode("//p[@class='message-of-error mg-t-5']") != null ? doc.DocumentNode.SelectSingleNode("//p[@class='message-of-error']").InnerText.Trim() : string.Empty;
                    //messageCaptcha = doc.DocumentNode.SelectSingleNode("//span[@class ='field-validation-error']") != null ? doc.DocumentNode.SelectSingleNode("//span[@class ='field-validation-error']").InnerText.Trim() : string.Empty;

                    messageSuccess = doc.DocumentNode.SelectSingleNode("//p[@class='message-of-success mg-t-5']") != null ? doc.DocumentNode.SelectSingleNode("//p[@class='message-of-success mg-t-5']").InnerText.Trim() : string.Empty;
                    messageError = doc.DocumentNode.SelectSingleNode("//p[@class='message-of-error mg-t-5']") != null ? doc.DocumentNode.SelectSingleNode("//p[@class='message-of-error mg-t-5']").InnerText.Trim() : string.Empty;
                    messageCaptcha = doc.DocumentNode.SelectSingleNode("//span[@class ='field-validation-error']") != null ? doc.DocumentNode.SelectSingleNode("//span[@class ='field-validation-error']").InnerText.Trim() : string.Empty;
                }
                catch (Exception e)
                {
                    NLogLogger.Info(new string[] { "SmasService", "Result", cardSerial, cardCode, answer, postTopup.HtmlContent, e.Message });
                    return "Post Topup Ignore Null";
                }


                if (!string.IsNullOrEmpty(messageSuccess))
                {
                    result = messageSuccess;
                }
                if (!string.IsNullOrEmpty(messageError))
                {
                    result = messageError;
                }
                if (!string.IsNullOrEmpty(messageCaptcha))
                {
                    result = HttpUtility.HtmlDecode(messageCaptcha.Trim());
                }

                NLogLogger.Info(new string[] { "SmasService", "Result", cardSerial, cardCode, answer, result });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "SmasService", "Topup", "Error", serializer.Serialize(parameters), e.Message, smasCookie.SessionId });
                return "Post Topup Ignore Exeption";
            }

            return result;
        }

        private static void PreGenCaptCha(string accountName, string passWord, SmasCookie smasCookie)
        {

            var sid = UtilsSmas.GenSid(accountName);
            //var getTopup = GetTopup(accountName, passWord);
            var getTopup = Task.Run(() => UtilsSmas.GetTask("https://smas.edu.vn/SMSEDUArea/Topup", smasCookie.CookieContainer)).Result;
            //if (getTopup.IsTopup)
            if (!string.IsNullOrEmpty(getTopup.CaptChaLink))
            {
                var sessionCaptcha = UtilsSmas.DeCaptcha(baseUrl + getTopup.CaptChaLink, sid, getTopup);
                sessionCaptcha.Type = 3;
                sessionCaptcha.Add();
                getTopup.SessionId = sid;
                getTopup.HtmlContent = String.Empty;
                UtilsSmas.SetCookieCache(sid, getTopup);
            }


        }

        private static void ReportCaptcha(int TaskId)
        {
            var result = new Lib.Captcha.Anticaptcha.AnticaptchaService().ReportIncorrectImageCaptcha(TaskId);
            NLogLogger.Info(new string[] { "SmasService", "ReportCaptcha", TaskId.ToString(), result.ToString() });
        }
    }
}