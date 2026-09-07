
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
using APIGame.Entity;
using HtmlAgilityPack;

namespace APIGame
{
    public class VTCService
    {
        static readonly string baseUrl = "https://vtcgame.vn";
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        static string CaptchaProvider = ConfigurationManager.AppSettings["Captcha_Provider"] ?? "anti-captcha.com";

        public static APIResponse TopupCard(long id, string cardSerial, string cardCode, string mobile, string accountName, string passWord)
        {

            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(passWord))
            {
                return new APIResponse((int)ResponseCode.ServiceIsLocked)
                {
                    Description = "Mobile or AccountName or Password is Empty"
                };
            }

            var getTopup = new GameCookie();
            var decaptcha = new Captcha().GetCaptcha(17, "vtc:" + accountName);
            if (decaptcha == null)
            {
                var sid = UtilsVTC.GenSid(accountName);

                var tryAgain = 0;
                getTopup = GetTopup(id, accountName, passWord);
                while (getTopup.IsTopup == false && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "VTCService", id.ToString(), "GetTopup", "Try", tryAgain.ToString(), accountName, passWord });
                    getTopup = GetTopup(id, accountName, passWord);
                    tryAgain++;
                    Thread.Sleep(10000);
                }

                if (getTopup.IsTopup)
                {
                    //decaptcha = UtilsVTC.DeCaptcha(getTopup.CaptChaLink, sid, getTopup);
                    decaptcha = UtilsVTC.NoCaptcha(sid, getTopup);

                    tryAgain = 0;
                    while (decaptcha == null && tryAgain < 3)
                    {
                        NLogLogger.Info(new string[] { "VTCService", id.ToString(), "GetTopup", "Try Decaptcha null", tryAgain.ToString(), sid, getTopup.CaptChaLink });
                        //decaptcha = UtilsVTC.DeCaptcha(getTopup.CaptChaLink, sid, getTopup);
                        decaptcha = UtilsVTC.NoCaptcha(sid, getTopup);
                        tryAgain++;
                        Thread.Sleep(10000);
                    }

                    if (decaptcha == null)
                    {
                        return new APIResponse((int)ResponseCode.ParameterInvalid) { Description = "DeCaptcha Null" };
                    }

                    getTopup.HtmlContent = string.Empty;
                    getTopup.SessionId = sid;
                    UtilsVTC.SetCookieCache(sid, getTopup);
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
                getTopup = UtilsVTC.GetCookieCache(decaptcha.SessionId);
                if (getTopup != null) NLogLogger.Info(new string[] { "VTCService", id.ToString(), "TopupCard", "Get CACHED OK", getTopup.SessionId });
            }

            if (getTopup != null)
            {

                var tryAgain = 0;
                var res = Topup(id, cardSerial, cardCode, decaptcha.Value, getTopup, accountName);
                while (string.IsNullOrEmpty(res) && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "VTCService", id.ToString(), "TopupCard", "Try", tryAgain.ToString(), cardSerial, cardCode, decaptcha.Value, accountName });
                    res = Topup(id, cardSerial, cardCode, decaptcha.Value, getTopup, accountName);
                    tryAgain++;
                }


                NLogLogger.Info(new string[] { "VTCService", id.ToString(), "TopupCard", "Response", accountName, cardSerial, cardCode, res });

                //Gen truoc x Captcha
                if (CaptchaProvider == "anti-captcha.com")
                {
                    for (int i = 0; i < 1; i++)
                    {
                        Action<string, GameCookie> send = PreGenCaptCha;
                        send.BeginInvoke(accountName, getTopup, null, null);
                        //Thread.Sleep(1000);
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

                if (res.Contains("balanceBefore failed"))
                {
                    return new APIResponse((int)ResponseCode.SystemBusy)
                    {
                        Description = res
                    };
                }

                if (res.Contains("balanceAfter failed"))
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

            }

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public static GameCookie GetTopup(long id, string accountName, string passWord)
        {
            try
            {

                NLogLogger.Info(new string[] { "VTCService", id.ToString(), "GetTopup", "Request", accountName, passWord });

                //var getRecharegePage = Task.Run(async () => await UtilsVTC.GetTask("https://vtcgame.vn/nap-vcoin/qua-the-cao.html", new CookieContainer())).Result;

                //var tryAgain = 0;

                //while (getRecharegePage == null && tryAgain < 5)
                //{
                //    NLogLogger.Info(new string[] { "VTCService", id.ToString(), "GetTopup", "Try Null", tryAgain.ToString(), accountName, passWord });
                //    getRecharegePage = Task.Run(async () => await UtilsVTC.GetTask("https://vtcgame.vn/nap-vcoin/qua-the-cao.html", new CookieContainer())).Result;

                //    tryAgain++;
                //    Thread.Sleep(1000);
                //}

                //if (getRecharegePage == null || string.IsNullOrEmpty(getRecharegePage.HtmlContent))
                //{
                //    NLogLogger.Info(new string[] { "VTCService", id.ToString(), "GetTopup", "Không Get được trang Nạp tiền", accountName, passWord });
                //    return new GameCookie()
                //    {
                //        IsTopup = false,
                //        HtmlContent = "Không Get được trang Nạp tiền"
                //    };
                //}

                //GetLogin
                //var getCommon = Task.Run(async () => await UtilsVTC.GetTask("https://header.vtcgame.vn/home/commonv2", getRecharegePage.CookieContainer)).Result;
                var getCommon = Task.Run(async () => await UtilsVTC.GetTask("https://header.vtcgame.vn/home/commonv2", new CookieContainer())).Result;
                if (getCommon == null || string.IsNullOrEmpty(getCommon.HtmlContent))
                {
                    NLogLogger.Info(new string[] { "VTCService", id.ToString(), "GetTopup", "Không Get được trang Login", accountName, passWord });
                    return new GameCookie()
                    {
                        IsTopup = false,
                        HtmlContent = "Không Get được trang Login"
                    };
                }

                //Login
                var parameters = new Dictionary<string, string>();
                parameters.Add("conten", Base64Encode(accountName));
                parameters.Add("value", Base64Encode(passWord));
                parameters.Add("capt", string.Empty);
                parameters.Add("hidverify", string.Empty);
                parameters.Add("isRemember", "false");
                parameters.Add("key", getCommon.userID);
                parameters.Add("otp", string.Empty);
                parameters.Add("otpType", "1");
                parameters.Add("returnURL", string.Empty);

                NLogLogger.Info(new string[] { "VTCService", id.ToString(), "Topup", "Request", serializer.Serialize(parameters) });

                var login = Task.Run(async () => await UtilsVTC.PostTask("https://header.vtcgame.vn/Handler/Process.ashx?act=LoginByMobile", parameters, getCommon.CookieContainer)).Result;
                if (login == null || string.IsNullOrEmpty(login.HtmlContent))
                {
                    NLogLogger.Info(new string[] { "VTCService", id.ToString(), "GetTopup", "Không Get được trang Login", accountName, passWord });
                    return new GameCookie()
                    {
                        IsTopup = false,
                        HtmlContent = "Không Post được service Login"
                    };
                }

                var loginResult = serializer.Deserialize<LoginResponse>(login.HtmlContent);
                switch (loginResult.errorCode)
                {
                    case 999:
                        var getRecharegePage = Task.Run(async () => await UtilsVTC.GetTask("https://vtcgame.vn/nap-vcoin/qua-the-cao.html", login.CookieContainer)).Result;
                        getRecharegePage.IsTopup = true;
                        return getRecharegePage;
                    case -53:
                        return new GameCookie()
                        {
                            IsTopup = false,
                            HtmlContent = "Mật khẩu không hợp lệ"
                        };
                    default:
                        return new GameCookie()
                        {
                            IsTopup = false,
                            HtmlContent = "Lỗi đăng nhập"
                        };
                }

            }
            catch (ThreadAbortException e)
            {
                NLogLogger.Info(new string[] { "VTCService", id.ToString(), "GetTopup", "Exception", e.Message });
                Thread.ResetAbort();
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "VTCService", id.ToString(), "GetTopup", "Exception", e.Message });
            }

            return new GameCookie()
            {
                IsTopup = false,
                HtmlContent = "Không Get được trang Nạp Tiền"
            };
        }

        public static string Topup(long id, string cardSerial, string cardCode, string answer, GameCookie smasCookie, string accountName)
        {
            var result = string.Empty;
            var parameters = new Dictionary<string, string>();
            GameCookie postTopup = null;
            var tryAgain = 0;

            //Get balance Befor
            //var getBalanceBefor = Task.Run(async () => await UtilsVTC.GetTask("https://vtcgame.vn/thong-tin-tai-khoan", smasCookie.CookieContainer)).Result;
            //if (string.IsNullOrEmpty(getBalanceBefor.HtmlContent))
            //{
            //    return "balanceBefore failed";
            //}

            //var doc = new HtmlDocument();
            //doc.LoadHtml(getBalanceBefor.HtmlContent);
            //var balanceBefor = Convert.ToInt32(doc.DocumentNode.SelectSingleNode("//table[@class='cg_chitiet_all_thongtin_taikhoan']//tr//td[2]").InnerHtml.Replace(" Vcoin", ""));

            try
            {

                parameters.Add("__RequestVerificationToken", smasCookie.RequestVerificationToken);
                parameters.Add("typeCard", "VC");
                parameters.Add("seriCard", cardSerial);
                parameters.Add("codeCard", cardCode);
                parameters.Add("userNameReceive", accountName);
                //parameters.Add("captcha", answer);
                //parameters.Add("captchaVerify", string.Empty);
                parameters.Add("reCaptchaToken", answer); 

                NLogLogger.Info(new string[] { "VTCService", id.ToString(), "Topup", "Request", serializer.Serialize(parameters) });

                postTopup = Task.Run(async () => await UtilsVTC.PostTask("https://vtcgame.vn/Vcoin/TopupByCard", parameters, smasCookie.CookieContainer)).Result;
                tryAgain = 0;
                while (postTopup == null && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "VTCService", id.ToString(), "PostTask", "Try", tryAgain.ToString(), serializer.Serialize(parameters) });
                    postTopup = Task.Run(async () => await UtilsVTC.PostTask("https://vtcgame.vn/Vcoin/TopupByCard", parameters, smasCookie.CookieContainer)).Result;
                    tryAgain++;
                    Thread.Sleep(1000);
                }

                if (postTopup != null)
                {
                    if (!string.IsNullOrEmpty(postTopup.HtmlContent))
                    {
                        var resultJson = serializer.Deserialize<TopupResponse>(postTopup.HtmlContent);
                        if (resultJson.ResponseStatus == 1)
                        {
                            NLogLogger.Info(new string[] { "VTCService", id.ToString(), "Result Success", cardSerial, cardCode, accountName, postTopup.HtmlContent });

                            //var amount = 0;
                            //while (amount <= 0 && tryAgain < 3)
                            //{
                            //    //Get balance After
                            //    var getBalanceAfter = Task.Run(async () => await UtilsVTC.GetTask("https://vtcgame.vn/thong-tin-tai-khoan", smasCookie.CookieContainer)).Result;
                            //    if (string.IsNullOrEmpty(getBalanceAfter.HtmlContent))
                            //    {
                            //        return "balanceAfter failed";
                            //    }
                            //    var balanceAfter = Convert.ToInt32(doc.DocumentNode.SelectSingleNode("//table[@class='cg_chitiet_all_thongtin_taikhoan']//tr//td[2]").InnerHtml.Replace(" Vcoin", ""));

                            //    //Tinh mệnh giá thẻ
                            //    amount = (balanceAfter - balanceBefor) * 1000; //1 Vcoin = 1K VNĐ
                            //    tryAgain++;
                            //    Thread.Sleep(1000);
                            //}

                            var amount = 0;
                            tryAgain = 0;
                            while (amount <= 0 && tryAgain < 3)
                            {
                                var rechargeHistory = Task.Run(async () => await UtilsVTC.GetTask("https://vtcgame.vn/account/GetHistoryTransaction?currentPage=1&pageSize=1&_=" + ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds(), smasCookie.CookieContainer)).Result;
                                var doc = new HtmlDocument();
                                doc.LoadHtml(rechargeHistory.HtmlContent);
                                NLogLogger.Info(new string[] { "VTCService", id.ToString(), cardSerial, cardCode, "Get history", "Try", tryAgain.ToString(), rechargeHistory.HtmlContent.Replace("\r\n", string.Empty) });
                                var listHistory = doc.DocumentNode.SelectNodes("//table[@class='cg_bang_lichsu_giaodich']//tr[2]//td");
                                if (listHistory[4].InnerText.Contains(cardSerial.ToUpper()))
                                {
                                    amount = Convert.ToInt32(listHistory[3].InnerText) * 1000;
                                }

                                //foreach (var td in listHistory)
                                //{
                                //    if (td.InnerText.Contains(cardSerial))
                                //    {
                                //        amount = Convert.ToInt32(listHistory[3]);
                                //        break;
                                //    }

                                //}
                                tryAgain++;
                                Thread.Sleep(2000);
                            }

                            result = "success: " + amount;

                        }
                        else
                        {
                            NLogLogger.Info(new string[] { "VTCService", id.ToString(), "Result non Success", cardSerial, cardCode, accountName, postTopup.HtmlContent });
                            result = resultJson.ErorrMess;
                        }

                    }
                    else
                    {
                        NLogLogger.Info(new string[] { "VTCService", id.ToString(), "Result HtmlContent Null", cardSerial, cardCode, answer, result, smasCookie.SessionId });
                    }
                }
                else
                {
                    NLogLogger.Info(new string[] { "VTCService", id.ToString(), "Result Object postTopup Null", cardSerial, cardCode, answer, result, smasCookie.SessionId });
                    return result;
                }


            }

            catch (ThreadAbortException e)
            {
                NLogLogger.Info(new string[] { "VTCService", id.ToString(), "Topup", "Error", serializer.Serialize(parameters), e.Message, smasCookie.SessionId });
                Thread.ResetAbort();
                return "Post Topup Ignore Timeout";
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "VTCService", id.ToString(), "Topup", "Error", serializer.Serialize(parameters), e.Message, smasCookie.SessionId, postTopup.HtmlContent });
                return "Post Topup Ignore Exeption";
            }

            return result;
        }



        private static void PreGenCaptCha(string accountName, GameCookie smasCookie)
        {
            var sid = UtilsVTC.GenSid(accountName);
            //var sessionCaptcha = UtilsVTC.DeCaptcha("https://vtcgame.vn/CaptchaImage.ashx", sid, smasCookie);
            var sessionCaptcha = UtilsVTC.NoCaptcha(sid, smasCookie);
            sessionCaptcha.Type = 17;
            sessionCaptcha.Add();
            smasCookie.SessionId = sid;
            UtilsVTC.SetCookieCache(sid, smasCookie);

        }

        private static void ReportCaptcha(int TaskId)
        {
            var result = new Lib.Captcha.Anticaptcha.AnticaptchaService().ReportIncorrectImageCaptcha(TaskId);
            NLogLogger.Info(new string[] { "VTCService", "ReportCaptcha", TaskId.ToString(), result.ToString() });
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}