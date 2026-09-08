using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
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
    public class TcVncdcService
    {
        static readonly string baseUrl = "https://tiemchung.vncdc.gov.vn";
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
            //if (CheckSerial_Service)
            //{
            //    var checkReponse = CheckSerial.CheckCard(cardSerial);
            //    NLogLogger.Info(new string[] { "MyViettelApp", "CheckSerial", serializer.Serialize(checkReponse) });
            //    if (checkReponse.ResponseCode == (int)ResponseCode.CardUsed ||
            //        checkReponse.ResponseCode == (int)ResponseCode.CardSerialInvalid)
            //    {
            //        return checkReponse;
            //    }

            //    if (checkReponse.ResponseCode == (int)ResponseCode.TransactionFailed)
            //    {

            //        //Check by MyVTT
            //        if (CheckSerial_MyVTT)
            //        {
            //            int tryAgain = 0;
            //            checkReponse = MyViettelService.CheckCard(cardSerial);
            //            while (tryAgain < 5 && (checkReponse.ResponseCode == (int)ResponseCode.LoginFail
            //                                    || checkReponse.ResponseCode == (int)ResponseCode.TransactionLimit
            //                                    || checkReponse.ResponseCode == (int)ResponseCode.AccessDenied
            //                                    || checkReponse.ResponseCode == (int)ResponseCode.ParameterInvalid))
            //            {
            //                Thread.Sleep(1000);
            //                checkReponse = MyViettelService.CheckCard(cardSerial);
            //                if (checkReponse.ResponseCode == (int)ResponseCode.AccountNotExists) tryAgain = 5;
            //                tryAgain++;
            //            }

            //            if (checkReponse.ResponseCode == (int)ResponseCode.CardUsed ||
            //                checkReponse.ResponseCode == (int)ResponseCode.CardSerialInvalid)
            //            {
            //                return checkReponse;
            //            }
            //        }
            //    }
            //}

            //if (CheckSerial_MyVTT)
            //{
            //    int tryAgain = 0;
            //    var checkReponse = MyViettelService.CheckCard(cardSerial);
            //    while (tryAgain < 5 && (checkReponse.ResponseCode == (int)ResponseCode.LoginFail
            //                            || checkReponse.ResponseCode == (int)ResponseCode.TransactionLimit
            //                            || checkReponse.ResponseCode == (int)ResponseCode.AccessDenied
            //                            || checkReponse.ResponseCode == (int)ResponseCode.ParameterInvalid))
            //    {
            //        Thread.Sleep(1000);
            //        checkReponse = MyViettelService.CheckCard(cardSerial);
            //        if (checkReponse.ResponseCode == (int)ResponseCode.AccountNotExists) tryAgain = 5;
            //        tryAgain++;
            //    }
            //    if (checkReponse.ResponseCode == (int)ResponseCode.CardUsed || checkReponse.ResponseCode == (int)ResponseCode.CardSerialInvalid)
            //    {
            //        return checkReponse;
            //    }
            //}

            var getTopup = new TcVncdcCookie();
            var decaptcha = new Captcha().GetCaptcha(5, "tcvncdc:" + accountName);
            if (decaptcha == null)
            {
                var sid = UtilsTcVncdc.GenSid(accountName);
                getTopup = GetTopup(accountName, passWord);
                if (getTopup.IsTopup)
                {
                    decaptcha = UtilsTcVncdc.DeCaptcha(baseUrl + getTopup.CaptChaLink, sid, getTopup);
                    getTopup.HtmlContent = string.Empty;
                    getTopup.SessionId = sid;
                    UtilsTcVncdc.SetCookieCache(sid, getTopup);
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
                getTopup = UtilsTcVncdc.GetCookieCache(decaptcha.SessionId);
                NLogLogger.Info(new string[] { "TcVncdcService", "Topup", "Login CACHED", accountName, passWord, cardSerial, cardCode });
            }

            if (getTopup != null)
            {
                var res = Topup(cardSerial, cardCode, decaptcha.Value, getTopup);

                for (int i = 0; i < 1; i++)
                {
                    Action<string, string, TcVncdcCookie> send = PreGenCaptCha;
                    send.BeginInvoke(accountName, passWord, getTopup, null, null);
                }

                if (res.Contains("Sequence contains no elements"))
                {
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        Description = res,
                        ResponseContent = Regex.Match(res, @"(\d+.)+").Value.Trim()
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

                if (res.Contains("The khong ton tai"))
                {
                    return new APIResponse((int)ResponseCode.CardSerialInvalid)
                    {
                        Description = res
                    };
                }

                if (res.Contains("The da duoc su dung"))
                {
                    return new APIResponse((int)ResponseCode.CardUsed)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Mã xác nhận không hợp lệ"))
                {
                    return new APIResponse((int)ResponseCode.ParameterInvalid)
                    {
                        Description = res
                    };
                }

            }

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public static TcVncdcCookie GetTopup(string accountName, string passWord)
        {
            NLogLogger.Info(new string[] { "TcVncdcService", "GetTopup", "Request", accountName, passWord });

            var getLogin = Task.Run(() => UtilsTcVncdc.GetTask("https://tiemchung.vncdc.gov.vn/Account/Login", new CookieContainer())).Result;

            var parameters = new Dictionary<string, string>();
            parameters.Add("__RequestVerificationToken", getLogin.RequestVerificationToken);
            parameters.Add("UserName", accountName);
            parameters.Add("password", passWord);
            parameters.Add("remember_me", "True");
            var loginResponse = Task.Run(() => UtilsTcVncdc.PostTask("https://tiemchung.vncdc.gov.vn/Account/Login", parameters, getLogin.CookieContainer)).Result;

            var detectSuccess = loginResponse.HtmlContent.Contains("Sai t&#234;n đăng nhập hoặc mật khẩu");
            if (detectSuccess)
            {
                NLogLogger.Info(new string[] { "TcVncdcService", "Topup", "Error Login", serializer.Serialize(parameters) });
                return new TcVncdcCookie()
                {
                    IsTopup = false,
                    HtmlContent = HttpUtility.HtmlDecode("Sai t&#234;n đăng nhập hoặc mật khẩu")
                };
            }

            var getTopup = Task.Run(() => UtilsTcVncdc.GetTask("https://tiemchung.vncdc.gov.vn/DichVuVAS/NapTien", loginResponse.CookieContainer)).Result;

            if (getTopup == null)
            {
                NLogLogger.Info(new string[] { "TcVncdcService", "Topup", "Error Locked 30 min", serializer.Serialize(parameters) });
                return new TcVncdcCookie() { IsTopup = false, HtmlContent = "Locked 30 min" }; //Bi khoa 15P
            }
            getTopup.IsTopup = true;

            return getTopup;

        }

        public static string Topup(string cardSerial, string cardCode, string answer, TcVncdcCookie smasCookie)
        {
            var result = string.Empty;
            var parameters = new Dictionary<string, string>();
            TcVncdcCookie postTopup = null;
            try
            {
                parameters.Add("MaTheCao", cardCode);
                parameters.Add("SoSeri", cardSerial);
                parameters.Add("MaXacNhan", answer);

                postTopup = Task.Run(() => UtilsTcVncdc.PostTask("https://tiemchung.vncdc.gov.vn/DichVuVas/NapTien/NapTienDaiLy", parameters, smasCookie.CookieContainer)).Result;
                if (postTopup.IsTimeout)
                {
                    NLogLogger.Info(new string[] { "TcVncdcService", "Topup", "Error TimeOut", serializer.Serialize(parameters) });
                    return "Post Topup Ignore Timeout";
                }

                //NLogLogger.Info(new string[] { "TcVncdcService", "Result", cardSerial, cardCode, answer, postTopup.HtmlContent });

                var res = (string)JObject.Parse(postTopup.HtmlContent)["Message"];
                if (!string.IsNullOrEmpty(res))
                {
                    if (res.Equals("Sequence contains no elements"))
                    {
                        //Do check menh gia'
                        int value = 0;
                        var parametersHistory = new Dictionary<string, string>();
                        parametersHistory.Add("start", "0");
                        parametersHistory.Add("length", "2");
                        parametersHistory.Add("search[value]", "");
                        parametersHistory.Add("search[regex]", "false");
                        parametersHistory.Add("TuNgay", "");
                        parametersHistory.Add("ToiNgay", "");
                        parametersHistory.Add("TrangThai", "");

                        var topupHistory = Task.Run(() => UtilsTcVncdc.PostTask("https://tiemchung.vncdc.gov.vn/DichVuVAS/NapTien/LichSuNapTien", parametersHistory, smasCookie.CookieContainer)).Result;
                        if (!string.IsNullOrEmpty(topupHistory.HtmlContent))
                        {
                            var cSerrial = (string)JObject.Parse(topupHistory.HtmlContent)["data"][0]["SO_SERI"];
                            var mMessage = (string)JObject.Parse(topupHistory.HtmlContent)["data"][0]["MESSAGE"];

                            if (cSerrial == cardSerial && mMessage == "Giao dich thanh cong")
                            {
                                value = (int)JObject.Parse(topupHistory.HtmlContent)["data"][0]["SO_TIEN"];
                            }
                        }

                        result = res + " menh gia " + value;
                    }
                    else
                    {
                        result = postTopup.HtmlContent;

                    }

                }
                return result;

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "TcVncdcService", "Topup", "Error", serializer.Serialize(parameters), e.Message, smasCookie.SessionId });
                return "Post Topup Ignore Exeption";
            }
        }

        private static void PreGenCaptCha(string accountName, string passWord, TcVncdcCookie tcVncdcCookie)
        {

            var sid = UtilsTcVncdc.GenSid(accountName);
            //var getTopup = GetTopup(accountName, passWord);
            var getTopup = Task.Run(() => UtilsTcVncdc.GetTask("https://tiemchung.vncdc.gov.vn/DichVuVAS/NapTien", tcVncdcCookie.CookieContainer)).Result;

            if (!string.IsNullOrEmpty(getTopup.CaptChaLink))
            {
                var sessionCaptcha = UtilsTcVncdc.DeCaptcha(baseUrl + getTopup.CaptChaLink, sid, getTopup);
                sessionCaptcha.Type = 5;
                sessionCaptcha.Add();
                getTopup.SessionId = sid;
                getTopup.HtmlContent = String.Empty;
                UtilsTcVncdc.SetCookieCache(sid, getTopup);
            }


        }

        private static void ReportCaptcha(int TaskId)
        {
            var result = new Lib.Captcha.Anticaptcha.AnticaptchaService().ReportIncorrectImageCaptcha(TaskId);
            NLogLogger.Info(new string[] { "TcVncdcService", "ReportCaptcha", TaskId.ToString(), result.ToString() });
        }
    }
}