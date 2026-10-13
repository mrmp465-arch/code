
using Lib.Captcha;
using Libs.API;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    public class GateService
    {
        static readonly string baseUrl = "https://pay.gate.vn/";
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        static string CaptchaProvider = ConfigurationManager.AppSettings["Captcha_Provider"] ?? "anti-captcha.com";
        static string CaptchaProviderIn = ConfigurationManager.AppSettings["Captcha_Provider_In"] ?? "inhouse-captcha";

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
            var sessionName = "gate:" + accountName + ":in";
            NLogLogger.Info(new string[] { "GateService", id.ToString(), "TopupCard", "Begin Get SESSION In DB", sessionName });
            var decaptcha = new Captcha().GetCaptcha(20, sessionName);
            NLogLogger.Info(decaptcha != null ? new string[] { "GateService", id.ToString(), "TopupCard", "Get SESSION OK", sessionName } : new string[] { "GateService", id.ToString(), "TopupCard", "Get SESSION NULL", sessionName });
            if (decaptcha == null)
            {
                var sid = UtilsGate.GenSid(sessionName);
                var tryAgain = 0;
                getTopup = GetTopup(id, accountName, passWord, "https://pay.gate.vn/NoAuth/paymentexpress/Account/");
                while (getTopup.IsTopup == false && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "GetTopup", "Try", tryAgain.ToString(), accountName, passWord });
                    getTopup = GetTopup(id, accountName, passWord, "https://pay.gate.vn/NoAuth/paymentexpress/Account/");
                    tryAgain++;
                    Thread.Sleep(1000);
                }

                if (getTopup.IsTopup)
                {
                    decaptcha = UtilsGate.DeCaptcha(baseUrl + getTopup.CaptChaLink, sid, getTopup, CaptchaProviderIn);
                    tryAgain = 0;
                    while (decaptcha == null && tryAgain < 3)
                    {
                        NLogLogger.Info(new string[] { "GateService", id.ToString(), "GetTopup", "Try Decaptcha null", tryAgain.ToString(), sid, getTopup.CaptChaLink });
                        decaptcha = UtilsGate.DeCaptcha(baseUrl + getTopup.CaptChaLink, sid, getTopup);
                        tryAgain++;
                        Thread.Sleep(2000);
                    }

                    if (decaptcha == null)
                    {
                        return new APIResponse((int)ResponseCode.ParameterInvalid) { Description = "DeCaptcha Null" };
                    }

                    getTopup.HtmlContent = string.Empty;
                    getTopup.SessionId = sid;
                    UtilsGate.SetCookieCache(sid, getTopup);
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
                NLogLogger.Info(new string[] { "GateService", id.ToString(), "TopupCard", "Begin Get CACHED", decaptcha.SessionId });
                getTopup = UtilsGate.GetCookieCache(decaptcha.SessionId);
                NLogLogger.Info(getTopup != null ? new string[] { "GateService", id.ToString(), "TopupCard", "Get CACHED OK", decaptcha.SessionId } : new string[] { "GateService", id.ToString(), "TopupCard", "Get CACHED NULL", decaptcha.SessionId });
            }

            if (getTopup != null)
            {

                var tryAgain = 0;
                var res = Topup(id, cardSerial, cardCode, decaptcha.Value, getTopup, accountName);
                while (string.IsNullOrEmpty(res) && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "TopupCard", "Try", tryAgain.ToString(), cardSerial, cardCode, decaptcha.Value, accountName });
                    res = Topup(id, cardSerial, cardCode, decaptcha.Value, getTopup, accountName);
                    tryAgain++;
                    Thread.Sleep(1000);
                }


                NLogLogger.Info(new string[] { "GateService", id.ToString(), "TopupCard", "Response", accountName, cardSerial, cardCode, res });


                if (res.Contains("Mã xác nhận không chính xác") || res.Contains("Chúng tôi xin lỗi, quá trình xử lý bị lỗi"))
                {

                    //Repost Captcha
                    //Action<int> send = ReportCaptcha;
                    //send.BeginInvoke(decaptcha.TaskId, null, null);

                    return new APIResponse((int)ResponseCode.ParameterInvalid)
                    {
                        Description = res
                    };
                }

                //Gen truoc x Captcha
                if (CaptchaProviderIn == "anti-captcha.com" || CaptchaProviderIn == "inhouse-captcha")
                {
                    for (int i = 0; i < 1; i++)
                    {
                        Action<string, string, GameCookie, string> send = PreGenCaptCha;
                        send.BeginInvoke(id.ToString(), sessionName, getTopup, CaptchaProviderIn, null, null);
                        Thread.Sleep(500);
                    }
                }

                if (res.Contains("Đã nạp thành công"))
                {

                    //var strAmount = Regex.Match(res, @"\d+").Value;
                    var strAmount = Regex.Match(res, @"(\d+.)+").Value.Trim().Replace(",", "");
                    var amount = !string.IsNullOrEmpty(strAmount) ? Convert.ToInt32(strAmount) : 0;
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
                

                if (res.Contains("Chúng tôi xin lỗi, quá trình xử lý bị lỗi"))
                {
                    return new APIResponse((int)ResponseCode.ParameterInvalid)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Thông tin mã thẻ không hợp lệ"))
                {
                    return new APIResponse((int)ResponseCode.CardFormatInvalid)
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

                if (res.Contains("Thẻ không tồn tại hoặc chưa được kích hoạt"))
                {
                    return new APIResponse((int)ResponseCode.CardNotActivated)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Bạn đã hết hạn mức nạp thẻ trong ngày"))
                {
                    return new APIResponse((int)ResponseCode.TransactionLimit)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Tài khoản bị tạm khóa trong 5 phút"))
                {
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = res
                    };
                }

                if (res.Contains("UN_HANDLER"))
                {
                    return new APIResponse((int)ResponseCode.ParameterInvalid)
                    {
                        Description = "Không bắt được lỗi"
                    };
                }

                //if (res.Contains("TRANSACTION_SUSPICIOUS"))
                //{
                //    return new APIResponse((int)ResponseCode.TransactionSuspicious)
                //    {
                //        Description = "Transaction Timeout"
                //    };
                //}

            }

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public static GameCookie GetTopup(long id, string accountName, string passWord, string urlGet)
        {
            try
            {
                NLogLogger.Info(new string[] { "GateService", id.ToString(), "GetTopup", "Request", accountName, passWord });

                GameCookie getTopup = new GameCookie();
                //Login
                var param = new Dictionary<string, string>();
                param.Add("acc", accountName);
                param.Add("pwd", Sercurity.Encrypts.MD5(passWord));
                param.Add("captcha", "");
                param.Add("remember", "0");
                getTopup = Task.Run(() => UtilsGate.PostTask("https://psp.gate.vn/ajax-signin", param, new CookieContainer(), id)).Result;

                NLogLogger.Info(new string[] { "GateService", id.ToString(), "GetTopup", "Result", getTopup.HtmlContent });

                if (Regex.Unescape(getTopup.HtmlContent).Contains("Đăng nhập thành công"))
                {

                    getTopup = Task.Run(() => UtilsGate.GetTask("https://psp.gate.vn/complete.html", getTopup.CookieContainer)).Result;

                    var doc = new HtmlDocument();
                    doc.LoadHtml(getTopup.HtmlContent);
                    var access_token = doc.GetElementbyId("access_token").Attributes["value"].Value;

                    getTopup = Task.Run(() => UtilsGate.GetTask($"https://pay.gate.vn/loginpsp.aspx?access_token={access_token}", getTopup.CookieContainer)).Result;

                    getTopup = Task.Run(() => UtilsGate.GetTask(urlGet, getTopup.CookieContainer)).Result;

                    getTopup.IsTopup = getTopup.HtmlContent.Contains(accountName);
                    if (getTopup.IsTopup)
                    {
                        doc.LoadHtml(getTopup.HtmlContent);
                        var __VIEWSTATE = doc.GetElementbyId("__VIEWSTATE").Attributes["value"].Value;
                        var __VIEWSTATEGENERATOR = doc.GetElementbyId("__VIEWSTATEGENERATOR").Attributes["value"].Value;
                        var __EVENTVALIDATION = doc.GetElementbyId("__EVENTVALIDATION").Attributes["value"].Value;
                        getTopup.__VIEWSTATE = __VIEWSTATE;
                        getTopup.__VIEWSTATEGENERATOR = __VIEWSTATEGENERATOR;
                        getTopup.__EVENTVALIDATION = __EVENTVALIDATION;
                        getTopup.RequestVerificationToken = access_token;
                    }


                }
                return getTopup;

            }
            catch (Exception e)
            {
                return new GameCookie()
                {
                    IsTopup = false,
                    HtmlContent = "Không Get được trang Nạp Tiền"
                };
            }
        }

        public static string Topup(long id, string cardSerial, string cardCode, string answer, GameCookie gateCookie, string accountName)
        {
            var result = string.Empty;
            var parameters = new Dictionary<string, string>();
            GameCookie postTopup = new GameCookie();
            var tryAgain = 0;

            try
            {
                parameters.Add("__EVENTTARGET", "");
                parameters.Add("__EVENTARGUMENT", "");
                parameters.Add("__VIEWSTATE", gateCookie.__VIEWSTATE);
                parameters.Add("__VIEWSTATEGENERATOR", gateCookie.__VIEWSTATEGENERATOR);
                parameters.Add("__EVENTVALIDATION", gateCookie.__EVENTVALIDATION);

                parameters.Add("ctl00$PSPContent$ddlService", "GATE");
                parameters.Add("ctl00$PSPContent$txtGateName", accountName);
                parameters.Add("ctl00$PSPContent$txtConfirmGateName", accountName);
                parameters.Add("ctl00$PSPContent$txtSerial", cardSerial);
                parameters.Add("ctl00$PSPContent$txtPin", cardCode);
                parameters.Add("ctl00$PSPContent$txtImageCode", answer);
                parameters.Add("ctl00$PSPContent$btnOK", "Nạp thẻ");

                NLogLogger.Info(new string[] { "GateService", id.ToString(), "Topup", "Request", serializer.Serialize(parameters) });
                var timeRequest = DateTime.Now;
                postTopup = Task.Run(async () => await UtilsGate.PostTask("https://pay.gate.vn/NoAuth/paymentexpress/Account/", parameters, gateCookie.CookieContainer, id)).Result;
                tryAgain = 0;
                while (postTopup == null && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "PostTask", "Try", tryAgain.ToString(), serializer.Serialize(parameters) });
                    postTopup = Task.Run(async () => await UtilsGate.PostTask("https://pay.gate.vn/NoAuth/paymentexpress/Account/", parameters, gateCookie.CookieContainer)).Result;
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "Result non Success", cardSerial, cardCode, accountName, postTopup.HtmlContent });
                    tryAgain++;
                    Thread.Sleep(1000);
                }

                if (postTopup != null)
                {
                    // Check Result qua History
                    if (postTopup.IsTimeout)
                    {
                        NLogLogger.Info(new string[] { "GateService", "Topup", id.ToString(), "Transaction Timeout", "Parse Table", gateCookie.SessionId });

                        var getHistory = Task.Run(async () => await UtilsGate.GetTask("https://pay.gate.vn/Business/History/CashTransfer/CashInput/", gateCookie.CookieContainer)).Result;
                        var doc = new HtmlDocument();
                        doc.LoadHtml(getHistory.HtmlContent);

                        List<List<string>> table = doc.DocumentNode.SelectSingleNode("//table[@class='tinygridview']")
                            .Descendants("tr")
                            .Skip(1)
                            .Where(tr => tr.Elements("td").Count() > 1)
                            .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                            .ToList();

                        foreach (var t in table)
                        {
                            if (t.Contains(cardSerial))
                            {
                                var timeRecharge = DateTime.ParseExact(t[1], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                var amount = t[4];
                                var diffInSeconds = (timeRecharge - timeRequest).TotalSeconds;
                                if (diffInSeconds > 0 && diffInSeconds <= 120)
                                {
                                    result = "Đã nạp thành công với mệnh giá " + amount;
                                }
                                else
                                {
                                    result = "Thẻ đã được sử dụng";
                                }

                                NLogLogger.Info(new string[] { "GateService", "Topup", id.ToString(), "Transaction Timeout", "Parse Table Result", result, gateCookie.SessionId });
                                return result;
                            }
                        }
                    }


                    if (!string.IsNullOrEmpty(postTopup.HtmlContent))
                    {

                        //if (postTopup.HtmlContent.Contains("Chúng tôi xin lỗi, quá trình xử lý bị lỗi"))
                        //    result = "Chúng tôi xin lỗi, quá trình xử lý bị lỗi.";
                        //else
                        //{
                        //    var amount = string.Empty;
                        //    var doc = new HtmlDocument();
                        //    doc.LoadHtml(postTopup.HtmlContent);
                        //    result = doc.GetElementbyId("PSPContent_lbMsg").InnerHtml;
                        //}


                        //if (!string.IsNullOrEmpty(result))
                        //{
                        //    amount = Regex.Match(res, @"(\d+.)+").Value.Trim().Replace(",", "");
                        //}

                        //int amountParse = 0;
                        //int.TryParse(amount, out amountParse);
                        //if (amountParse > 0)
                        //    result = "success: " + amount;
                        //else
                        //    result = "failed: " + res;

                        var doc = new HtmlDocument();
                        doc.LoadHtml(postTopup.HtmlContent);

                        if (!string.IsNullOrEmpty(postTopup.HtmlContent))
                        {
                            if (postTopup.HtmlContent.Contains("Chúng tôi xin lỗi, quá trình xử lý bị lỗi"))
                            {
                                result = "Chúng tôi xin lỗi, quá trình xử lý bị lỗi.";
                                NLogLogger.Info(new string[] { "GateService", "Topup", id.ToString(), "Result HtmlContent Null", gateCookie.SessionId });
                            }
                            else
                            {
                                var mesError = string.Empty;
                                if (doc.GetElementbyId("PSPContent_lbMsg") != null) mesError = doc.GetElementbyId("PSPContent_lbMsg").InnerHtml;

                                if (string.IsNullOrEmpty(mesError))
                                {
                                    result = mesError = "UN_HANDLER";
                                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "Topup", "Result HTML", "\r\n", postTopup.HtmlContent });
                                }
                                else
                                {
                                    result = mesError;
                                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "Topup", "Result Message", mesError });
                                }

                            }

                        }

                        return result;

                    }
                    else
                    {
                        NLogLogger.Info(new string[] { "GateService", id.ToString(), "Topup", "Result HtmlContent Null", cardSerial, cardCode, answer, result, gateCookie.SessionId });
                    }
                }
                else
                {
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "Topup", "Result Object postTopup Null", cardSerial, cardCode, answer, result, gateCookie.SessionId });
                    return result;
                }


            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GateService", id.ToString(), "Topup", "Error", serializer.Serialize(parameters), e.Message, gateCookie.SessionId, postTopup.HtmlContent });
                return "Post Topup Exception";
            }

            return result;
        }

        private static void PreGenCaptCha(string id, string accountName, GameCookie gateCookie, string captchaProvider = "")
        {

            var urlCaptcha = "https://pay.gate.vn/Contents/Imgs/ImageCaptcha.aspx";
            //var urlCaptcha = "https://pay.gate.vn/Contents/Imgs/ImageVerifier.aspx";
            NLogLogger.Info(new string[] { "GateService", "PreGenCaptCha Begin", id, accountName, });
            var sid = UtilsGate.GenSid(accountName);
            var sessionCaptcha = UtilsGate.DeCaptcha(urlCaptcha, sid, gateCookie, captchaProvider);

            //if (!isNumber && tryAgain < 3)
            //{
            //    NLogLogger.Info(new string[] { "GateService", "PreGenCaptCha", "Not is Numeric Value", sessionCaptcha.Value });
            //    ReportCaptcha(sessionCaptcha.TaskId);
            //    sessionCaptcha = UtilsGate.DeCaptcha("https://pay.gate.vn/Contents/Imgs/ImageVerifier.aspx", sid, gateCookie);
            //    isNumber = int.TryParse(sessionCaptcha.Value, out numericValue);
            //    tryAgain++;
            //}

            var tryAgain = 0;
            while (sessionCaptcha == null && tryAgain < 3)
            {
                sessionCaptcha = UtilsGate.DeCaptcha(urlCaptcha, sid, gateCookie, captchaProvider);
                tryAgain++;
            }

            if (sessionCaptcha != null)
            {
                //int numericValue;
                //var isNumber = int.TryParse(sessionCaptcha.Value, out numericValue);
                //if (!isNumber)
                //{
                //    ReportCaptcha(sessionCaptcha.TaskId);
                //}
                sessionCaptcha.Type = 20;
                sessionCaptcha.Add();
                gateCookie.SessionId = sid;
                UtilsGate.SetCookieCache(sid, gateCookie);
                NLogLogger.Info(new string[] { "GateService", "PreGenCaptCha", id, accountName, sid, sessionCaptcha.TaskId.ToString(), sessionCaptcha.Value });
            }
            else
            {
                NLogLogger.Info(new string[] { "GateService", "PreGenCaptCha", id, "NULL", accountName, sid, sessionCaptcha.Value });
            }
        }

        private static void ReportCaptcha(int TaskId)
        {
            var result = new Lib.Captcha.Anticaptcha.AnticaptchaService().ReportIncorrectImageCaptcha(TaskId);
            NLogLogger.Info(new string[] { "GateService", "ReportCaptcha", TaskId.ToString(), result.ToString() });
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static GameCookie BuyCard(long id, string telco, int amount, int quanity, string answer, GameCookie gateCookie, string accountName, string session_id)
        {
            var parameters = new Dictionary<string, string>();
            GameCookie postTopup = new GameCookie();
            var tryAgain = 0;

            var dictTeco = new Dictionary<string, string>()
            {
                {"VMS","mobi"},
                {"VTT","viettel"},
                {"VNP","vina"},
            };

            var ddlPartner = dictTeco[telco.ToUpper()];

            var dictAmount = new Dictionary<string, string>()
            {
                {"VMS10000","63"},
                {"VMS20000","64"},
                {"VMS30000","96"},
                {"VMS50000","41"},
                {"VMS100000","42"},
                {"VMS200000","65"},
                {"VMS300000","66"},
                {"VMS500000","67"},

                {"VTT10000","73"},
                {"VTT20000","61"},
                {"VTT30000","98"},
                {"VTT50000","45"},
                {"VTT100000","46"},
                {"VTT200000","62"},
                {"VTT300000","74"},
                {"VTT500000","75"},
                {"VTT1000000","156"},

                {"VNP10000","68"},
                {"VNP20000","69"},
                {"VNP30000","97"},
                {"VNP50000","43"},
                {"VNP100000","44"},
                {"VNP200000","70"},
                {"VNP300000","71"},
                {"VNP500000","72"}
            };

            var ddlCard = dictAmount[telco.ToUpper() + amount.ToString()];

            try
            {
                parameters.Add("__EVENTTARGET", "");
                parameters.Add("__EVENTARGUMENT", "");
                parameters.Add("__VIEWSTATE", gateCookie.__VIEWSTATE);
                parameters.Add("__VIEWSTATEGENERATOR", gateCookie.__VIEWSTATEGENERATOR);
                parameters.Add("__EVENTVALIDATION", gateCookie.__EVENTVALIDATION);

                parameters.Add("ctl00$PSPContent$hfCheckSum", "");
                parameters.Add("ctl00$PSPContent$ddlPartner", ddlPartner);
                parameters.Add("ddlCard", ddlCard);
                parameters.Add("ctl00$PSPContent$txtQuantity", quanity.ToString());
                parameters.Add("ctl00$PSPContent$txtImageCode", answer);
                parameters.Add("ctl00$PSPContent$btnThanhtoan", "Xác nhận");

                NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCard", "Request", serializer.Serialize(parameters) });

                postTopup = Task.Run(async () => await UtilsGate.PostTaskV2("https://pay.gate.vn/Business/ShoppingCard/?type=mobile", parameters, gateCookie.CookieContainer, session_id)).Result;
                tryAgain = 0;
                while (postTopup == null && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCard", "Try", tryAgain.ToString(), serializer.Serialize(parameters) });
                    postTopup = Task.Run(async () => await UtilsGate.PostTaskV2("https://pay.gate.vn/Business/ShoppingCard/?type=mobile", parameters, gateCookie.CookieContainer, session_id)).Result;
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCard", "Result non Success" });
                    tryAgain++;
                    Thread.Sleep(1000);
                }


                if (postTopup != null)
                {
                    if (!string.IsNullOrEmpty(postTopup.HtmlContent))
                    {
                        if (postTopup.HtmlContent.Contains("Chúng tôi xin lỗi, quá trình xử lý bị lỗi"))
                        {
                            NLogLogger.Info(new string[] { "GateService", "BuyCard", "Error", id.ToString(), "Chúng tôi xin lỗi, quá trình xử lý bị lỗi.", serializer.Serialize(parameters), gateCookie.SessionId });
                            postTopup.HtmlContent = "Chúng tôi xin lỗi, quá trình xử lý bị lỗi.";
                            postTopup.IsTopup = false;
                            return postTopup;
                        }

                        var doc = new HtmlDocument();
                        doc.LoadHtml(postTopup.HtmlContent);

                        if (doc.GetElementbyId("PSPContent_txtPassword") != null)
                        {
                            var __VIEWSTATE = doc.GetElementbyId("__VIEWSTATE").Attributes["value"].Value;
                            var __VIEWSTATEGENERATOR = doc.GetElementbyId("__VIEWSTATEGENERATOR").Attributes["value"].Value;
                            var __EVENTVALIDATION = doc.GetElementbyId("__EVENTVALIDATION").Attributes["value"].Value;
                            postTopup.__VIEWSTATE = __VIEWSTATE;
                            postTopup.__VIEWSTATEGENERATOR = __VIEWSTATEGENERATOR;
                            postTopup.__EVENTVALIDATION = __EVENTVALIDATION;
                            postTopup.HtmlContent = "Bạn hãy nhập mật khẩu giao dịch vào ô bên dưới";
                            postTopup.IsTopup = true;
                            return postTopup;
                        }

                        var mesError = string.Empty;
                        if (doc.GetElementbyId("PSPContent_lbError") != null) mesError = doc.GetElementbyId("PSPContent_lbError").InnerHtml;
                        NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCard", "Result Message", accountName, mesError });

                        if (!string.IsNullOrEmpty(mesError))
                        {

                            postTopup.HtmlContent = mesError;
                            postTopup.IsTopup = false;

                        }
                        else
                        {
                            NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCard", "Un Handler", "\r\n", postTopup.HtmlContent });
                            postTopup.HtmlContent = "UN_HANDLER";
                            postTopup.IsTopup = false;
                        }
                        return postTopup;
                    }
                    else
                    {
                        postTopup.HtmlContent = "UN_HANDLER_EMPTY";
                        NLogLogger.Info(new string[] { "GateService", "BuyCard", id.ToString(), "Result HtmlContent Null", ddlPartner, ddlCard, quanity.ToString(), answer, gateCookie.SessionId });
                    }
                }
                else
                {
                    NLogLogger.Info(new string[] { "GateService", "BuyCard", id.ToString(), "Result Object postTopup Null", ddlPartner, ddlCard, quanity.ToString(), answer, gateCookie.SessionId });
                    return postTopup;
                }


            }
            catch (ThreadAbortException e)
            {
                NLogLogger.Info(new string[] { "GateService", "BuyCard", id.ToString(), "ThreadAbortException Thread.ResetAbort", e.Message });
                Thread.ResetAbort();
                return null;
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCard", "Error", serializer.Serialize(parameters), e.Message, gateCookie.SessionId, postTopup.HtmlContent });
                return null;
            }

            return postTopup;
        }

        public static GameCookie BuyCardConfirm(long id, GameCookie gateCookie, string passWord, string session_id)
        {
            var parameters = new Dictionary<string, string>();
            GameCookie postBuyCardConfirm = new GameCookie();
            var tryAgain = 0;

            try
            {
                parameters.Add("__EVENTTARGET", "");
                parameters.Add("__EVENTARGUMENT", "");
                parameters.Add("__VIEWSTATE", gateCookie.__VIEWSTATE);
                parameters.Add("__VIEWSTATEGENERATOR", gateCookie.__VIEWSTATEGENERATOR);
                parameters.Add("__EVENTVALIDATION", gateCookie.__EVENTVALIDATION);

                parameters.Add("ctl00$PSPContent$txtPassword", passWord);
                parameters.Add("ctl00$PSPContent$btnBuyCard", "Thanh toán");

                NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCardConfirm", "Request", serializer.Serialize(parameters) });

                postBuyCardConfirm = Task.Run(async () => await UtilsGate.PostTaskV2("https://pay.gate.vn/Business/ShoppingCard/ConfirmOrder.aspx?type=mobile", parameters, gateCookie.CookieContainer, session_id)).Result;
                tryAgain = 0;
                while (postBuyCardConfirm == null && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCardConfirm", "PostTask", "Try", tryAgain.ToString(), serializer.Serialize(parameters) });
                    postBuyCardConfirm = Task.Run(async () => await UtilsGate.PostTaskV2("https://pay.gate.vn/Business/ShoppingCard/ConfirmOrder.aspx?type=mobile", parameters, gateCookie.CookieContainer, session_id)).Result;
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCardConfirm", "Result non Success" });
                    tryAgain++;
                    Thread.Sleep(1000);
                }

                if (postBuyCardConfirm != null)
                {

                    if (!string.IsNullOrEmpty(postBuyCardConfirm.HtmlContent))
                    {

                        if (postBuyCardConfirm.HtmlContent.Contains("Chúng tôi xin lỗi, quá trình xử lý bị lỗi"))
                        {
                            NLogLogger.Info(new string[] { "GateService", "BuyCardConfirm", "Error", id.ToString(), "Chúng tôi xin lỗi, quá trình xử lý bị lỗi.", serializer.Serialize(parameters), gateCookie.SessionId });
                            postBuyCardConfirm.HtmlContent = "Chúng tôi xin lỗi, quá trình xử lý bị lỗi.";
                            return postBuyCardConfirm;
                        }

                        var doc = new HtmlDocument();
                        doc.LoadHtml(postBuyCardConfirm.HtmlContent);
                        var mesError_pass = string.Empty;
                        if (doc.GetElementbyId("PSPContent_lblError_Pass") != null) mesError_pass = doc.GetElementbyId("PSPContent_lblError_Pass").InnerHtml;
                        var mesError = string.Empty;
                        if (doc.GetElementbyId("PSPContent_lbError") != null) mesError = doc.GetElementbyId("PSPContent_lbError").InnerHtml;
                        var mesInfo = string.Empty;
                        if (doc.GetElementbyId("PSPContent_lbInfo") != null) mesInfo = doc.GetElementbyId("PSPContent_lbInfo").InnerHtml;
                        NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCardConfirm", "Result Message", mesError_pass, mesError, mesInfo });

                        if (!string.IsNullOrEmpty(mesInfo))
                        {
                            if (mesInfo.Contains("Bạn đã mua hàng thành công"))
                            {
                                var __VIEWSTATE = doc.GetElementbyId("__VIEWSTATE").Attributes["value"].Value;
                                var __VIEWSTATEGENERATOR = doc.GetElementbyId("__VIEWSTATEGENERATOR").Attributes["value"].Value;
                                var __EVENTVALIDATION = doc.GetElementbyId("__EVENTVALIDATION").Attributes["value"].Value;
                                postBuyCardConfirm.__VIEWSTATE = __VIEWSTATE;
                                postBuyCardConfirm.__VIEWSTATEGENERATOR = __VIEWSTATEGENERATOR;
                                postBuyCardConfirm.__EVENTVALIDATION = __EVENTVALIDATION;
                                postBuyCardConfirm.IsTopup = true;
                                postBuyCardConfirm.HtmlContent = "Bạn đã mua hàng thành công";
                            }
                            else
                            {
                                postBuyCardConfirm.IsTopup = false;
                                postBuyCardConfirm.HtmlContent = mesInfo;
                            }

                        }
                        else
                        {
                            postBuyCardConfirm.IsTopup = false;
                            postBuyCardConfirm.HtmlContent = mesError_pass + " | " + mesError + " | " + mesInfo;
                        }

                        if (string.IsNullOrEmpty(mesInfo) && string.IsNullOrEmpty(mesError) && string.IsNullOrEmpty(mesError_pass))
                        {
                            postBuyCardConfirm.IsTopup = false;
                            NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCardConfirm", "Un Handler", "\r\n", postBuyCardConfirm.HtmlContent });
                        }

                        return postBuyCardConfirm;
                    }
                    else
                    {
                        NLogLogger.Info(new string[] { "GateService", "BuyCardConfirm", id.ToString(), "Result HtmlContent Null", gateCookie.SessionId });
                    }
                }
                else
                {
                    NLogLogger.Info(new string[] { "GateService", "BuyCardConfirm", id.ToString(), "Result Object postTopup Null", gateCookie.SessionId });
                    return postBuyCardConfirm;
                }


            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GateService", "BuyCardConfirm", "Error", id.ToString(), serializer.Serialize(parameters), e.Message, gateCookie.SessionId });
                return null;
            }

            return postBuyCardConfirm;
        }

        public static string BuyCardExport(long id, GameCookie gateCookie, string session_id)
        {
            var result = string.Empty;
            var parameters = new Dictionary<string, string>();
            GameCookie postBuyCardExport = new GameCookie();
            var tryAgain = 0;

            try
            {
                parameters.Add("__VIEWSTATE", gateCookie.__VIEWSTATE);
                parameters.Add("__VIEWSTATEGENERATOR", gateCookie.__VIEWSTATEGENERATOR);
                parameters.Add("__EVENTVALIDATION", gateCookie.__EVENTVALIDATION);

                parameters.Add("ctl00$PSPContent$btnExportTXT", "Xuất Text");

                NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCardExport", "Request", serializer.Serialize(parameters) });

                postBuyCardExport = Task.Run(async () => await UtilsGate.PostTaskV2("https://pay.gate.vn/Business/ShoppingCard/ConfirmOrder.aspx?type=mobile", parameters, gateCookie.CookieContainer, session_id)).Result;
                tryAgain = 0;
                while (postBuyCardExport == null && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCardExport", "PostTask", "Try", tryAgain.ToString(), serializer.Serialize(parameters) });
                    postBuyCardExport = Task.Run(async () => await UtilsGate.PostTaskV2("https://pay.gate.vn/Business/ShoppingCard/ConfirmOrder.aspx?type=mobile", parameters, gateCookie.CookieContainer, session_id)).Result;
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCardExport", "Result non Success" });
                    tryAgain++;
                    Thread.Sleep(1000);
                }

                if (postBuyCardExport != null)
                {

                    if (!postBuyCardExport.HtmlContent.Contains("Chúng tôi xin lỗi, quá trình xử lý bị lỗi"))
                    {
                        //Bóc thẻ trả về
                        char[] lineDelimiter = new char[] { '\n' };
                        string[] line = postBuyCardExport.HtmlContent.Trim().Split(lineDelimiter);
                        char[] delimiter = new char[] { '\t' };
                        var cardList = new List<CardDVO>();
                        for (int i = 1; i < line.Length; i++)
                        {
                            string[] colum = line[i].Split(delimiter);

                            cardList.Add(new CardDVO()
                            {
                                Name = colum[0],
                                Serial = colum[1],
                                Pin = colum[2]
                            });
                        }

                        result = serializer.Serialize(cardList);
                        NLogLogger.Info(new string[] { "GateService", "BuyCardExport", id.ToString(), "Result", result, gateCookie.SessionId });

                        return result;
                    }
                    else
                    {
                        NLogLogger.Info(new string[] { "GateService", "BuyCardExport", id.ToString(), "Result HtmlContent Null", "Chúng tôi xin lỗi, quá trình xử lý bị lỗi", gateCookie.SessionId });
                    }
                }
                else
                {
                    NLogLogger.Info(new string[] { "GateService", "BuyCardExport", id.ToString(), "Result Object postTopup Null", gateCookie.SessionId });
                    return result;
                }


            }
            catch (ThreadAbortException e)
            {
                NLogLogger.Info(new string[] { "GateService", "BuyCardExport", id.ToString(), "ThreadAbortException", e.Message });
                Thread.ResetAbort();
                return string.Empty;
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GateService", "BuyCardExport", "Error", id.ToString(), serializer.Serialize(parameters), e.Message, gateCookie.SessionId });
                return string.Empty;
            }

            return result;
        }

        public static APIResponse BuyCardMaster(long id, string telco, int amount, int quanity, string accountName, string passWord)
        {

            var getTopup = new GameCookie();
            var sessionName = "gate:" + accountName + ":buy";
            NLogLogger.Info(new string[] { "GateService", "BuyCardMaster", id.ToString(), "Begin Get SESSION In DB", sessionName });
            var decaptcha = new Captcha().GetCaptcha(20, sessionName);
            NLogLogger.Info(decaptcha != null ? new string[] { "GateService", "BuyCardMaster", id.ToString(), "Get SESSION OK", sessionName } : new string[] { "GateService", "BuyCardMaster", id.ToString(), "Get SESSION NULL", sessionName });
            if (decaptcha == null)
            {
                var sid = UtilsGate.GenSid(sessionName);

                var tryAgain = 0;
                getTopup = GetTopup(id, accountName, passWord, "https://pay.gate.vn/Business/ShoppingCard/?type=mobile");
                while (getTopup.IsTopup == false && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GateService", "BuyCardMaster", "GetTopup", id.ToString(), "Try", tryAgain.ToString(), accountName, passWord });
                    getTopup = GetTopup(id, accountName, passWord, "https://pay.gate.vn/Business/ShoppingCard/?type=mobile");
                    tryAgain++;
                    Thread.Sleep(1000);
                }

                if (getTopup.IsTopup)
                {
                    decaptcha = UtilsGate.DeCaptcha(baseUrl + getTopup.CaptChaLink, sid, getTopup);
                    tryAgain = 0;
                    while (decaptcha == null && tryAgain < 3)
                    {
                        NLogLogger.Info(new string[] { "GateService", "DeCaptcha", id.ToString(), "Try Decaptcha null", tryAgain.ToString(), sid, getTopup.CaptChaLink });
                        decaptcha = UtilsGate.DeCaptcha(baseUrl + getTopup.CaptChaLink, sid, getTopup);
                        tryAgain++;
                        Thread.Sleep(1000);
                    }

                    if (decaptcha == null)
                    {
                        return new APIResponse((int)ResponseCode.ParameterInvalid) { Description = "DeCaptcha Null" };
                    }

                    getTopup.HtmlContent = string.Empty;
                    getTopup.SessionId = sid;
                    UtilsGate.SetCookieCache(sid, getTopup);
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
                NLogLogger.Info(new string[] { "GateService", "BuyCardMaster", id.ToString(), "Begin Get CACHED", decaptcha.SessionId });
                getTopup = UtilsGate.GetCookieCache(decaptcha.SessionId);
                NLogLogger.Info(getTopup != null ? new string[] { "GateService", "BuyCardMaster", id.ToString(), "Get CACHED OK", decaptcha.SessionId, decaptcha.TaskId.ToString(), decaptcha.Value } : new string[] { "GateService", "BuyCardMaster", id.ToString(), "Get CACHED NULL", decaptcha.SessionId });
            }

            if (getTopup != null)
            {
                var session_id = new Random().Next().ToString();
                var tryAgain = 0;
                var res = BuyCard(id, telco, amount, quanity, decaptcha.Value, getTopup, accountName, session_id);
                while (res == null && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GateService", "BuyCardMaster", "BuyCard", id.ToString(), "Try", tryAgain.ToString(), telco, amount.ToString(), quanity.ToString(), decaptcha.Value, accountName });
                    res = BuyCard(id, telco, amount, quanity, decaptcha.Value, getTopup, accountName, session_id);
                    tryAgain++;
                }
                NLogLogger.Info(new string[] { "GateService", "BuyCard", id.ToString(), "Response", accountName, amount.ToString(), quanity.ToString(), res.HtmlContent });

                //Gen truoc x Captcha
                //if (CaptchaProvider == "anti-captcha.com")
                //{
                //    for (int i = 0; i < 1; i++)
                //    {
                //        Action<string, string, GameCookie> send = PreGenCaptCha;
                //        send.BeginInvoke(id.ToString(), sessionName, getTopup, null, null);
                //        Thread.Sleep(500);
                //    }
                //}


                if (res.IsTopup)
                {
                    tryAgain = 0;
                    var resConfirm = BuyCardConfirm(id, res, passWord, session_id);
                    while (resConfirm == null && tryAgain < 3)
                    {
                        NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCard Confirm", "Try", tryAgain.ToString(), telco, amount.ToString(), quanity.ToString(), decaptcha.Value, accountName });
                        resConfirm = BuyCardConfirm(id, res, passWord, session_id);
                        tryAgain++;
                    }
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCard Confirm", "Response", accountName, amount.ToString(), quanity.ToString() });

                    if (resConfirm.IsTopup)
                    {
                        tryAgain = 0;
                        var resExport = BuyCardExport(id, resConfirm, session_id);
                        while (string.IsNullOrEmpty(resExport) && tryAgain < 3)
                        {
                            NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCardExport", "Try", tryAgain.ToString(), telco, amount.ToString(), quanity.ToString(), decaptcha.Value, accountName });
                            resExport = BuyCardExport(id, resConfirm, session_id);
                            tryAgain++;
                        }
                        NLogLogger.Info(new string[] { "GateService", id.ToString(), "BuyCardExport", "Response", accountName, amount.ToString(), quanity.ToString() });

                        return new APIResponse((int)ResponseCode.TransactionSuccessful)
                        {
                            ResponseContent = resExport
                        };
                    }
                    else
                    {
                        return new APIResponse((int)ResponseCode.TransactionFailed);
                    }


                }
                else
                {
                    //Error BuyCard
                    if (res.HtmlContent.Contains("số lượng đã hết"))
                    {
                        return new APIResponse((int)ResponseCode.CardOutOfStock)
                        {
                            Description = res.HtmlContent
                        };
                    }


                    if (res.HtmlContent.Contains("Mã xác nhận không chính xác"))
                    {
                        //Repost Captcha
                        //Action<int> send = ReportCaptcha;
                        //send.BeginInvoke(decaptcha.TaskId, null, null);

                        return new APIResponse((int)ResponseCode.ParameterInvalid)
                        {
                            Description = "Mã xác nhận không chính xác"
                        };
                    }

                    if (res.HtmlContent.Contains("Chúng tôi xin lỗi, quá trình xử lý bị lỗi"))
                    {
                        return new APIResponse((int)ResponseCode.ParameterInvalid)
                        {
                            Description = "Chúng tôi xin lỗi, quá trình xử lý bị lỗi"
                        };
                    }

                    if (res.HtmlContent.Contains("Bạn chưa cập nhật hình thức xác minh giao dịch"))
                    {
                        return new APIResponse((int)ResponseCode.ServiceIsLocked)
                        {
                            Description = "Bạn chưa cập nhật hình thức xác minh giao dịch"
                        };
                    }

                    if (res.HtmlContent.Contains("Số tiền của bạn không đủ để giao dịch"))
                    {
                        return new APIResponse((int)ResponseCode.ServiceIsLocked)
                        {
                            Description = "Số tiền của bạn không đủ để giao dịch"
                        };
                    }

                    if (res.HtmlContent.Contains("Bạn đã vượt hạn mức giao dịch trong ngày"))
                    {
                        return new APIResponse((int)ResponseCode.TransactionLimit)
                        {
                            Description = "Bạn đã vượt hạn mức giao dịch trong ngày"
                        };
                    }

                    if (res.HtmlContent.Contains("UN_HANDLER"))
                    {
                        return new APIResponse((int)ResponseCode.SystemBusy)
                        {
                            Description = "Không bắt được lỗi"
                        };
                    }
                    if (res.HtmlContent.Contains("UN_HANDLER_EMPTY"))
                    {
                        return new APIResponse((int)ResponseCode.SystemBusy)
                        {
                            Description = "Không bắt được html content"
                        };
                    }
                    else
                    {
                        return new APIResponse((int)ResponseCode.TransactionFailed);
                    }

                }

            }

            return new APIResponse((int)ResponseCode.TransactionFailed);

        }

        public static GameCookie Tranfer(long id, string telco, int amount, string answer, string simTarget, GameCookie gateCookie, string accountName, string session_id)
        {
            var parameters = new Dictionary<string, string>();
            GameCookie postTranfer = new GameCookie();
            var tryAgain = 0;

            var dictTeco = new Dictionary<string, string>()
            {
                {"VMS","89"},
                {"VTT","123"},
                {"VNP","97"},
            };

            var ddlPartner = dictTeco[telco.ToUpper()];

            try
            {
                parameters.Add("__EVENTTARGET", "");
                parameters.Add("__EVENTARGUMENT", "");
                parameters.Add("__VIEWSTATE", gateCookie.__VIEWSTATE);
                parameters.Add("__VIEWSTATEGENERATOR", gateCookie.__VIEWSTATEGENERATOR);
                parameters.Add("__EVENTVALIDATION", gateCookie.__EVENTVALIDATION);

                parameters.Add("ctl00$PSPContent$hfCheckSum", "");
                parameters.Add("ctl00$PSPContent$ddlService", ddlPartner);
                parameters.Add("ctl00$PSPContent$txtID", simTarget);
                parameters.Add("ctl00$PSPContent$txtIDConfirm", simTarget);
                parameters.Add("ctl00$PSPContent$ddlMoney", amount.ToString());
                parameters.Add("ctl00$PSPContent$txtImageCode", answer);
                parameters.Add("ctl00$PSPContent$btnDefault", "Thực hiện");

                NLogLogger.Info(new string[] { "GateService", id.ToString(), "Tranfer", "Request", serializer.Serialize(parameters) });

                postTranfer = Task.Run(async () => await UtilsGate.PostTaskV2("https://pay.gate.vn/Business/TopupMobile/?type=mobile", parameters, gateCookie.CookieContainer, session_id)).Result;
                tryAgain = 0;
                while (postTranfer == null && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "Tranfer", "Try", tryAgain.ToString(), serializer.Serialize(parameters) });
                    postTranfer = Task.Run(async () => await UtilsGate.PostTaskV2("https://pay.gate.vn/Business/TopupMobile/?type=mobile", parameters, gateCookie.CookieContainer, session_id)).Result;
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "Tranfer", "Result non Success" });
                    tryAgain++;
                    Thread.Sleep(1000);
                }


                if (postTranfer != null)
                {
                    if (!string.IsNullOrEmpty(postTranfer.HtmlContent))
                    {

                        var doc = new HtmlDocument();
                        doc.LoadHtml(postTranfer.HtmlContent);
                        var txtPasss = string.Empty;

                        if (doc.GetElementbyId("PSPContent_txtPassword") != null)
                        //if (postTranfer.HtmlContent.Contains("Bạn hãy nhập mật khẩu giao dịch vào ô bên"))
                        {
                            var __VIEWSTATE = doc.GetElementbyId("__VIEWSTATE").Attributes["value"].Value;
                            var __VIEWSTATEGENERATOR = doc.GetElementbyId("__VIEWSTATEGENERATOR").Attributes["value"].Value;
                            var __EVENTVALIDATION = doc.GetElementbyId("__EVENTVALIDATION").Attributes["value"].Value;
                            var PSPContent_hfCheckSum = doc.GetElementbyId("PSPContent_hfCheckSum").Attributes["value"].Value;
                            postTranfer.__VIEWSTATE = __VIEWSTATE;
                            postTranfer.__VIEWSTATEGENERATOR = __VIEWSTATEGENERATOR;
                            postTranfer.__EVENTVALIDATION = __EVENTVALIDATION;
                            postTranfer.RequestVerificationToken = PSPContent_hfCheckSum;
                            postTranfer.IsTopup = true;
                        }

                        else if (postTranfer.HtmlContent.Contains("Chúng tôi xin lỗi, quá trình xử lý bị lỗi"))
                        {
                            NLogLogger.Info(new string[] { "GateService", "Tranfer", "Error", id.ToString(), "Chúng tôi xin lỗi, quá trình xử lý bị lỗi.", serializer.Serialize(parameters), gateCookie.SessionId });
                            postTranfer.HtmlContent = "Chúng tôi xin lỗi, quá trình xử lý bị lỗi.";
                            postTranfer.IsTopup = false;
                        }

                        else
                        {
                            var mesError = string.Empty;
                            if (doc.GetElementbyId("PSPContent_lbError") != null) mesError = doc.GetElementbyId("PSPContent_lbError").InnerHtml;

                            if (string.IsNullOrEmpty(mesError))
                            {
                                NLogLogger.Info(new string[] { "GateService", id.ToString(), "Tranfer", "Result HTML", "\r\n", postTranfer.HtmlContent });
                            }
                            else
                            {
                                postTranfer.HtmlContent = mesError;
                                NLogLogger.Info(new string[] { "GateService", id.ToString(), "Tranfer", "Result Message", mesError });
                            }
                            postTranfer.IsTopup = false;
                        }


                    }
                    else
                    {
                        postTranfer.IsTopup = false;
                        NLogLogger.Info(new string[] { "GateService", "Tranfer", id.ToString(), "Result HtmlContent Null", ddlPartner, simTarget, amount.ToString(), answer, gateCookie.SessionId });
                    }
                }
                else
                {
                    NLogLogger.Info(new string[] { "GateService", "Tranfer", id.ToString(), "Result Object postTopup Null", ddlPartner, simTarget, amount.ToString(), answer, gateCookie.SessionId });
                    postTranfer.IsTopup = false;
                }


            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GateService", "Tranfer", id.ToString(), "Error", serializer.Serialize(parameters), e.Message, gateCookie.SessionId, postTranfer.HtmlContent });
                return null;
            }

            return postTranfer;
        }

        public static GameCookie TranferConfirm(long id, GameCookie gateCookie, string passWord, string session_id)
        {
            var parameters = new Dictionary<string, string>();
            GameCookie postTranferConfirm = new GameCookie();

            try
            {
                parameters.Add("__LASTFOCUS", "");
                parameters.Add("__EVENTTARGET", "");
                parameters.Add("__EVENTARGUMENT", "");
                parameters.Add("__VIEWSTATE", gateCookie.__VIEWSTATE);
                parameters.Add("__VIEWSTATEGENERATOR", gateCookie.__VIEWSTATEGENERATOR);
                parameters.Add("__EVENTVALIDATION", gateCookie.__EVENTVALIDATION);

                parameters.Add("ctl00$PSPContent$hfCheckSum", gateCookie.RequestVerificationToken);
                parameters.Add("ctl00$PSPContent$txtPassword", passWord);
                parameters.Add("ctl00$PSPContent$btnConfirmPass", "Thực hiện");

                NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferConfirm", "Request", serializer.Serialize(parameters) });

                postTranferConfirm = Task.Run(async () => await UtilsGate.PostTaskV2("https://pay.gate.vn/Business/TopupMobile/?type=mobile", parameters, gateCookie.CookieContainer, session_id)).Result;
                var tryAgain = 0;
                while (postTranferConfirm == null && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferConfirm", "PostTask", "Try", tryAgain.ToString(), serializer.Serialize(parameters) });
                    postTranferConfirm = Task.Run(async () => await UtilsGate.PostTaskV2("https://pay.gate.vn/Business/TopupMobile/?type=mobile", parameters, gateCookie.CookieContainer, session_id)).Result;
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferConfirm", "Result non Success" });
                    tryAgain++;
                    Thread.Sleep(1000);
                }

                if (postTranferConfirm != null)
                {

                    var doc = new HtmlDocument();
                    doc.LoadHtml(postTranferConfirm.HtmlContent);

                    if (!string.IsNullOrEmpty(postTranferConfirm.HtmlContent))
                    {
                        if (postTranferConfirm.HtmlContent.Contains("Bạn đã nạp tiền cho điện thoại di dộng thành công"))
                        {
                            postTranferConfirm.IsTopup = true;
                            postTranferConfirm.HtmlContent = "Bạn đã nạp tiền cho điện thoại di dộng thành công.";

                        }
                        else if (postTranferConfirm.HtmlContent.Contains("Chúng tôi xin lỗi, quá trình xử lý bị lỗi."))
                        {
                            postTranferConfirm.IsTopup = false;
                            postTranferConfirm.HtmlContent = "Chúng tôi xin lỗi, quá trình xử lý bị lỗi.";
                            NLogLogger.Info(new string[] { "GateService", "TranferConfirm", id.ToString(), "Chúng tôi xin lỗi, quá trình xử lý bị lỗi.", gateCookie.SessionId });
                        }
                        else
                        {
                            var mesError = string.Empty;
                            if (doc.GetElementbyId("PSPContent_lblError_Pass") != null) mesError = doc.GetElementbyId("PSPContent_lblError_Pass").InnerHtml;


                            if (string.IsNullOrEmpty(mesError))
                            {
                                postTranferConfirm.HtmlContent = "Không nhận được kết quả trả về";
                                NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferConfirm", "Result HTML", "\r\n", postTranferConfirm.HtmlContent });
                            }
                            else
                            {
                                postTranferConfirm.HtmlContent = mesError;
                                NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferConfirm", "Result Message", mesError });
                            }

                            postTranferConfirm.IsTopup = false;
                        }

                    }

                }
                else
                {
                    NLogLogger.Info(new string[] { "GateService", "TranferConfirm", id.ToString(), "Result Object postTopup Null", gateCookie.SessionId });
                    postTranferConfirm.IsTopup = false;
                }

            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GateService", "TranferConfirm", "Error", id.ToString(), serializer.Serialize(parameters), e.Message, gateCookie.SessionId });
                return null;
            }

            return postTranferConfirm;
        }

        public static APIResponse TranferMaster(long id, string telco, int amount, string simTarget, string accountName, string passWord)
        {

            var getTopup = new GameCookie();
            var sessionName = "gate:" + accountName + ":topup";
            NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferBalance", "Begin Get SESSION In DB", sessionName });
            var decaptcha = new Captcha().GetCaptcha(20, sessionName);
            NLogLogger.Info(decaptcha != null
                ? new string[] { "GateService", id.ToString(), "TranferBalance", "Get SESSION OK", "gate:" + accountName }
                : new string[] { "GateService", id.ToString(), "TranferBalance", "Get SESSION NULL", "gate:" + accountName });
            if (decaptcha == null)
            {
                var sid = UtilsGate.GenSid(sessionName);

                var tryAgain = 0;
                getTopup = GetTopup(id, accountName, passWord, "https://pay.gate.vn/Business/TopupMobile/?type=mobile");
                while (getTopup.IsTopup == false && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GateService", "TranferBalance", "GetTopup", id.ToString(), "Try", tryAgain.ToString(), accountName, passWord });
                    getTopup = GetTopup(id, accountName, passWord, "https://pay.gate.vn/Business/TopupMobile/?type=mobile");
                    tryAgain++;
                    Thread.Sleep(10000);
                }

                if (getTopup.IsTopup)
                {
                    decaptcha = UtilsGate.DeCaptcha(baseUrl + getTopup.CaptChaLink, sid, getTopup);
                    tryAgain = 0;
                    while (decaptcha == null && tryAgain < 3)
                    {
                        NLogLogger.Info(new string[] { "GateService", "DeCaptcha", id.ToString(), "Try Decaptcha null", tryAgain.ToString(), sid, getTopup.CaptChaLink });
                        decaptcha = UtilsGate.DeCaptcha(baseUrl + getTopup.CaptChaLink, sid, getTopup);
                        tryAgain++;
                        Thread.Sleep(1000);
                    }

                    if (decaptcha == null)
                    {
                        return new APIResponse((int)ResponseCode.ParameterInvalid) { Description = "DeCaptcha Null" };
                    }

                    getTopup.HtmlContent = string.Empty;
                    getTopup.SessionId = sid;
                    UtilsGate.SetCookieCache(sid, getTopup);
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
                NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferBalance", "Begin Get CACHED", decaptcha.SessionId });
                getTopup = UtilsGate.GetCookieCache(decaptcha.SessionId);
                NLogLogger.Info(getTopup != null ? new string[] { "GateService", id.ToString(), "TranferBalance", "Get CACHED OK", decaptcha.SessionId } : new string[] { "GateService", id.ToString(), "TranferBalance", "Get CACHED NULL", decaptcha.SessionId });
            }

            if (getTopup != null)
            {

                var tryAgain = 0;
                var session_id = new Random().Next().ToString();

                var res = Tranfer(id, telco, amount, decaptcha.Value, simTarget, getTopup, accountName, session_id);
                while (res == null && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GateService", "TranferBalance", "Tranfer", id.ToString(), "Try", tryAgain.ToString(), accountName, telco, amount.ToString(), simTarget.ToString(), decaptcha.Value });
                    res = Tranfer(id, telco, amount, decaptcha.Value, simTarget, getTopup, accountName, session_id);
                    tryAgain++;
                }
                if (!res.IsTopup)
                    NLogLogger.Info(new string[] { "GateService", "TranferBalance", "Tranfer", id.ToString(), "Response", accountName, amount.ToString(), simTarget.ToString(), res.HtmlContent });

                //Gen truoc x Captcha
                //if (CaptchaProvider == "anti-captcha.com")
                //{
                //    for (int i = 0; i < 1; i++)
                //    {
                //        Action<string, string, GameCookie> send = PreGenCaptCha;
                //        send.BeginInvoke(id.ToString(), sessionName, getTopup, null, null);
                //        Thread.Sleep(1000);
                //    }
                //}

                GameCookie resConfrim = new GameCookie();
                if (res.IsTopup)
                {
                    tryAgain = 0;
                    resConfrim = TranferConfirm(id, res, passWord, session_id);
                    while (resConfrim == null && tryAgain < 3)
                    {
                        NLogLogger.Info(new string[] { "GateService", "TranferBalance", "Tranfer", id.ToString(), "Try", tryAgain.ToString(), accountName, telco, amount.ToString(), simTarget.ToString(), decaptcha.Value });
                        resConfrim = TranferConfirm(id, res, passWord, session_id);
                        tryAgain++;
                    }
                }
                else
                {
                    if (res.HtmlContent.Contains("Mã xác nhận không chính xác"))
                    {

                        return new APIResponse((int)ResponseCode.ParameterInvalid)
                        {
                            Description = "Mã xác nhận không chính xác"
                        };
                    }

                    else if (res.HtmlContent.Contains("Chúng tôi xin lỗi, quá trình xử lý bị lỗi"))
                    {
                        return new APIResponse((int)ResponseCode.ParameterInvalid)
                        {
                            Description = "Chúng tôi xin lỗi, quá trình xử lý bị lỗi"
                        };
                    }

                    else if (res.HtmlContent.Contains("Bạn chưa cập nhật hình thức xác minh giao dịch"))
                    {
                        return new APIResponse((int)ResponseCode.ServiceIsLocked)
                        {
                            Description = "Bạn chưa cập nhật hình thức xác minh giao dịch"
                        };
                    }

                    else if (res.HtmlContent.Contains("Bạn đã vượt hạn mức giao dịch trong ngày"))
                    {
                        return new APIResponse((int)ResponseCode.TransactionLimit)
                        {
                            Description = "Bạn đã vượt hạn mức giao dịch trong ngày"
                        };
                    }
                    else
                        return new APIResponse((int)ResponseCode.TransactionFailed);

                }

                if (resConfrim.HtmlContent.Contains("Bạn đã nạp tiền cho điện thoại di dộng thành công"))
                {
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        Description = "Bạn đã nạp tiền cho điện thoại di dộng thành công."
                    };
                }
                else if (resConfrim.HtmlContent.Contains("Không nhận được kết quả trả về"))
                {
                    return new APIResponse((int)ResponseCode.SystemBusy)
                    {
                        Description = "Không nhận được kết quả trả về."
                    };
                }

            }

            return new APIResponse((int)ResponseCode.TransactionFailed);

        }

        public static GameCookie TranferAccount(string id, string sim, string simTarget, int amount, string answer, GameCookie gateCookie, string session_id)
        {
            var parameters = new Dictionary<string, string>();
            GameCookie postTranfer = new GameCookie();
            var tryAgain = 0;

            try
            {
                parameters.Add("__EVENTTARGET", "");
                parameters.Add("__EVENTARGUMENT", "");
                parameters.Add("__VIEWSTATE", gateCookie.__VIEWSTATE);
                parameters.Add("__VIEWSTATEGENERATOR", gateCookie.__VIEWSTATEGENERATOR);
                parameters.Add("__EVENTVALIDATION", gateCookie.__EVENTVALIDATION);

                parameters.Add("ctl00$PSPContent$wzdTransfer$txtUserName", simTarget);
                parameters.Add("ctl00$PSPContent$wzdTransfer$txtAmount", amount.ToString());
                parameters.Add("ctl00$PSPContent$wzdTransfer$ddlType", "0");
                parameters.Add("ctl00$PSPContent$wzdTransfer$txtNote", "");
                parameters.Add("ctl00$PSPContent$wzdTransfer$txtImageCode", answer);
                parameters.Add("ctl00$PSPContent$wzdTransfer$btContinue", "Tiếp tục");

                NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferAccount", "Request", serializer.Serialize(parameters) });

                postTranfer = Task.Run(async () => await UtilsGate.PostTaskV2("https://pay.gate.vn/Business/Transfer/", parameters, gateCookie.CookieContainer, session_id)).Result;
                tryAgain = 0;
                while (postTranfer == null && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferAccount", "Try", tryAgain.ToString(), serializer.Serialize(parameters) });
                    postTranfer = Task.Run(async () => await UtilsGate.PostTaskV2("https://pay.gate.vn/Business/Transfer/", parameters, gateCookie.CookieContainer, session_id)).Result;
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferAccount", "Result non Success" });
                    tryAgain++;
                    Thread.Sleep(1000);
                }


                if (postTranfer != null)
                {
                    if (!string.IsNullOrEmpty(postTranfer.HtmlContent))
                    {

                        var doc = new HtmlDocument();
                        doc.LoadHtml(postTranfer.HtmlContent);
                        var txtPasss = string.Empty;

                        if (doc.GetElementbyId("PSPContent_wzdTransfer_txtPassword") != null)
                        //if (postTranfer.HtmlContent.Contains("Bạn hãy nhập mật khẩu giao dịch vào ô bên"))
                        {
                            var __VIEWSTATE = doc.GetElementbyId("__VIEWSTATE").Attributes["value"].Value;
                            var __VIEWSTATEGENERATOR = doc.GetElementbyId("__VIEWSTATEGENERATOR").Attributes["value"].Value;
                            var __EVENTVALIDATION = doc.GetElementbyId("__EVENTVALIDATION").Attributes["value"].Value;

                            postTranfer.__VIEWSTATE = __VIEWSTATE;
                            postTranfer.__VIEWSTATEGENERATOR = __VIEWSTATEGENERATOR;
                            postTranfer.__EVENTVALIDATION = __EVENTVALIDATION;

                            postTranfer.IsTopup = true;
                        }

                        else if (postTranfer.HtmlContent.Contains("Chúng tôi xin lỗi, quá trình xử lý bị lỗi"))
                        {
                            NLogLogger.Info(new string[] { "GateService", "TranferAccount", "Error", id.ToString(), "Chúng tôi xin lỗi, quá trình xử lý bị lỗi.", serializer.Serialize(parameters), gateCookie.SessionId });
                            postTranfer.HtmlContent = "Chúng tôi xin lỗi, quá trình xử lý bị lỗi.";
                            postTranfer.IsTopup = false;
                        }

                        else
                        {
                            var mesError = string.Empty;
                            if (doc.GetElementbyId("PSPContent_wzdTransfer_lbError") != null) mesError = doc.GetElementbyId("PSPContent_wzdTransfer_lbError").InnerHtml;

                            if (string.IsNullOrEmpty(mesError))
                            {
                                NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferAccount", "Un Handler Message", "\r\n", postTranfer.HtmlContent });
                            }
                            else
                            {
                                if (mesError.Contains("Số tiền của bạn không đủ để giao dịch"))
                                {
                                    var balanceStr = doc.DocumentNode.SelectSingleNode("//div[@class='info-acc block-right']//span");
                                    var balance = balanceStr.InnerHtml.Trim().Replace(",", "");
                                    postTranfer.HtmlContent = "Số tiền của bạn không đủ để giao dịch: " + balance;
                                }
                                else
                                {
                                    postTranfer.HtmlContent = mesError;
                                }
                                NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferAccount", "Result Message", mesError });
                            }
                            postTranfer.IsTopup = false;
                        }


                    }
                    else
                    {
                        postTranfer.IsTopup = false;
                        NLogLogger.Info(new string[] { "GateService", "TranferAccount", id.ToString(), "Result HtmlContent Null", sim, simTarget, amount.ToString(), answer, gateCookie.SessionId });
                    }
                }
                else
                {
                    NLogLogger.Info(new string[] { "GateService", "TranferAccount", id.ToString(), "Result Object postTopup Null", sim, simTarget, amount.ToString(), answer, gateCookie.SessionId });
                    postTranfer.IsTopup = false;
                }


            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GateService", "TranferAccount", id.ToString(), "Error", serializer.Serialize(parameters), e.Message, gateCookie.SessionId, postTranfer.HtmlContent });
                return null;
            }

            return postTranfer;
        }
        public static GameCookie TranferAccountConfirm(string id, GameCookie gateCookie, string passWord, string session_id)
        {
            var parameters = new Dictionary<string, string>();
            GameCookie postTranferConfirm = new GameCookie();
            var tryAgain = 0;

            try
            {
                parameters.Add("__EVENTTARGET", "");
                parameters.Add("__EVENTARGUMENT", "");
                parameters.Add("__VIEWSTATE", gateCookie.__VIEWSTATE);
                parameters.Add("__VIEWSTATEGENERATOR", gateCookie.__VIEWSTATEGENERATOR);
                parameters.Add("__EVENTVALIDATION", gateCookie.__EVENTVALIDATION);

                parameters.Add("ctl00$PSPContent$wzdTransfer$txtPassword", passWord);
                parameters.Add("ctl00$PSPContent$wzdTransfer$btTransfer", "Thực hiện");

                NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferAccountConfirm", "Request", serializer.Serialize(parameters) });

                postTranferConfirm = Task.Run(async () => await UtilsGate.PostTaskV2("https://pay.gate.vn/Business/Transfer/", parameters, gateCookie.CookieContainer, session_id)).Result;
                tryAgain = 0;
                while (postTranferConfirm == null && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferAccountConfirm", "PostTask", "Try", tryAgain.ToString(), serializer.Serialize(parameters) });
                    postTranferConfirm = Task.Run(async () => await UtilsGate.PostTaskV2("https://pay.gate.vn/Business/Transfer/", parameters, gateCookie.CookieContainer, session_id)).Result;
                    NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferAccountConfirm", "Result non Success" });
                    tryAgain++;
                    Thread.Sleep(1000);
                }

                if (postTranferConfirm != null)
                {

                    if (!string.IsNullOrEmpty(postTranferConfirm.HtmlContent))
                    {

                        if (postTranferConfirm.HtmlContent.Contains("Chúng tôi xin lỗi, quá trình xử lý bị lỗi"))
                        {
                            NLogLogger.Info(new string[] { "GateService", "TranferAccountConfirm", "Error", id.ToString(), "Chúng tôi xin lỗi, quá trình xử lý bị lỗi.", serializer.Serialize(parameters), gateCookie.SessionId });
                            postTranferConfirm.HtmlContent = "Chúng tôi xin lỗi, quá trình xử lý bị lỗi.";
                            return postTranferConfirm;
                        }
                        else if (postTranferConfirm.HtmlContent.Contains("Bạn đã chuyển tiền thành công"))
                        {
                            postTranferConfirm.IsTopup = true;
                            return postTranferConfirm;

                        }

                        var doc = new HtmlDocument();
                        doc.LoadHtml(postTranferConfirm.HtmlContent);
                        var mesError = string.Empty;
                        if (doc.GetElementbyId("PSPContent_wzdTransfer_lbError2") != null) mesError = doc.GetElementbyId("PSPContent_wzdTransfer_lbError2").InnerHtml;

                        NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferAccountConfirm", "Result Message", mesError });
                        if (!string.IsNullOrEmpty(mesError))
                        {
                            postTranferConfirm.IsTopup = false;
                            postTranferConfirm.HtmlContent = mesError;
                        }
                        else
                        {
                            postTranferConfirm.IsTopup = false;
                            postTranferConfirm.HtmlContent = "Message Un Handler";
                            NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferAccountConfirm", "Un Handler", "\r\n", postTranferConfirm.HtmlContent });
                        }

                        return postTranferConfirm;
                    }
                    else
                    {
                        postTranferConfirm.IsTopup = false;
                        NLogLogger.Info(new string[] { "GateService", "TranferAccountConfirm", id.ToString(), "Result HtmlContent Null", gateCookie.SessionId });
                    }
                }
                else
                {
                    NLogLogger.Info(new string[] { "GateService", "TranferAccountConfirm", id.ToString(), "Result Object postTopup Null", gateCookie.SessionId });
                    return postTranferConfirm;
                }


            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GateService", "TranferAccountConfirm", "Error", id.ToString(), serializer.Serialize(parameters), e.Message, gateCookie.SessionId });
                return null;
            }

            return postTranferConfirm;
        }
        public static APIResponse TranferAccountMaster(string id, string sim, string simTarget, int amount, string accountName, string passWord)
        {

            var getTopup = new GameCookie();
            var sessionName = "gate:" + accountName + ":tran";
            NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferAccountMaster", "Begin Get SESSION In DB", sessionName });
            var decaptcha = new Captcha().GetCaptcha(20, "gate:" + accountName);
            NLogLogger.Info(decaptcha != null ? new string[] { "GateService", id.ToString(), "TranferAccountMaster", "Get SESSION OK", sessionName } : new string[] { "GateService", id.ToString(), "TranferAccountMaster", "Get SESSION NULL", sessionName });
            if (decaptcha == null)
            {
                var sid = UtilsGate.GenSid(sessionName);

                var tryAgain = 0;
                getTopup = GetTopup(Convert.ToInt64(id), accountName, passWord, "https://pay.gate.vn/Business/Transfer/");
                while (getTopup.IsTopup == false && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GateService", "TranferAccountMaster", "GetTopup", id.ToString(), "Try", tryAgain.ToString(), accountName, passWord });
                    getTopup = GetTopup(Convert.ToInt64(id), accountName, passWord, "https://pay.gate.vn/Business/Transfer/");
                    tryAgain++;
                    Thread.Sleep(10000);
                }

                if (getTopup.IsTopup)
                {
                    decaptcha = UtilsGate.DeCaptcha(baseUrl + getTopup.CaptChaLink, sid, getTopup);
                    tryAgain = 0;
                    while (decaptcha == null && tryAgain < 3)
                    {
                        NLogLogger.Info(new string[] { "GateService", "DeCaptcha", id.ToString(), "Try Decaptcha null", tryAgain.ToString(), sid, getTopup.CaptChaLink });
                        decaptcha = UtilsGate.DeCaptcha(baseUrl + getTopup.CaptChaLink, sid, getTopup);
                        tryAgain++;
                        Thread.Sleep(1000);
                    }

                    if (decaptcha == null)
                    {
                        return new APIResponse((int)ResponseCode.ParameterInvalid) { Description = "DeCaptcha Null" };
                    }

                    getTopup.HtmlContent = string.Empty;
                    getTopup.SessionId = sid;
                    UtilsGate.SetCookieCache(sid, getTopup);
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
                NLogLogger.Info(new string[] { "GateService", id.ToString(), "TranferAccountMaster", "Begin Get CACHED", decaptcha.SessionId });
                getTopup = UtilsGate.GetCookieCache(decaptcha.SessionId);
                NLogLogger.Info(getTopup != null ? new string[] { "GateService", id.ToString(), "TranferAccountMaster", "Get CACHED OK", decaptcha.SessionId } : new string[] { "GateService", id.ToString(), "TranferAccountMaster", "Get CACHED NULL", decaptcha.SessionId });
            }

            if (getTopup != null)
            {

                var tryAgain = 0;
                var session_id = new Random().Next().ToString();

                var res = TranferAccount(id, sim, simTarget, amount, decaptcha.Value, getTopup, session_id);
                while (res == null && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "GateService", "TranferAccountMaster", "Tranfer", id.ToString(), "Try", tryAgain.ToString(), accountName, amount.ToString(), simTarget.ToString(), decaptcha.Value });
                    res = TranferAccount(id, sim, simTarget, amount, decaptcha.Value, getTopup, session_id);
                    tryAgain++;
                }
                if (!res.IsTopup)
                    NLogLogger.Info(new string[] { "GateService", "TranferAccountMaster", "Tranfer", id.ToString(), "Response", accountName, amount.ToString(), simTarget.ToString(), res.HtmlContent });

                //Gen truoc x Captcha
                //if (CaptchaProvider == "anti-captcha.com")
                //{
                //    for (int i = 0; i < 1; i++)
                //    {
                //        Action<string, string, GameCookie> send = PreGenCaptCha;
                //        send.BeginInvoke(id.ToString(), sessionName, getTopup, null, null);
                //        Thread.Sleep(1000);
                //    }
                //}

                GameCookie resConfrim = new GameCookie();
                if (res.IsTopup)
                {
                    tryAgain = 0;
                    resConfrim = TranferAccountConfirm(id, res, passWord, session_id);
                    while (resConfrim == null && tryAgain < 3)
                    {
                        NLogLogger.Info(new string[] { "GateService", "TranferAccountMaster", "Confirm", id.ToString(), "Try", tryAgain.ToString(), accountName, amount.ToString(), simTarget.ToString(), decaptcha.Value });
                        resConfrim = TranferAccountConfirm(id, res, passWord, session_id);
                        tryAgain++;
                    }
                }
                else
                {
                    if (res.HtmlContent.Contains("Mã xác nhận không chính xác"))
                    {
                        //Repost Captcha
                        //Action<int> send = ReportCaptcha;
                        //send.BeginInvoke(decaptcha.TaskId, null, null);

                        return new APIResponse((int)ResponseCode.ParameterInvalid)
                        {
                            Description = "Mã xác nhận không chính xác"
                        };
                    }

                    else if (res.HtmlContent.Contains("Chúng tôi xin lỗi, quá trình xử lý bị lỗi"))
                    {
                        return new APIResponse((int)ResponseCode.ParameterInvalid)
                        {
                            Description = "Chúng tôi xin lỗi, quá trình xử lý bị lỗi"
                        };
                    }

                    else if (res.HtmlContent.Contains("Bạn chưa cập nhật hình thức xác minh giao dịch"))
                    {
                        return new APIResponse((int)ResponseCode.ServiceIsLocked)
                        {
                            Description = "Bạn chưa cập nhật hình thức xác minh giao dịch"
                        };
                    }

                    else if (res.HtmlContent.Contains("Số tiền của bạn không đủ để giao dịch"))
                    {
                        return new APIResponse((int)ResponseCode.BalanceNotEnough)
                        {
                            Description = "Số tiền của bạn không đủ để giao dịch",
                            ResponseContent = Regex.Match(res.HtmlContent, @"\d+").Value

                        };
                    }
                    else if (res.HtmlContent.Contains("Giao dịch vượt quá số tiền trong tài khoản"))
                    {
                        return new APIResponse((int)ResponseCode.BalanceNotEnough)
                        {
                            Description = "Giao dịch vượt quá số tiền trong tài khoản",
                            ResponseContent = Regex.Match(res.HtmlContent, @"\d+").Value

                        };
                    }
                    else
                        return new APIResponse((int)ResponseCode.TransactionFailed);

                }

                if (resConfrim.HtmlContent.Contains("thành công"))
                {
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        Description = "Bạn đã nạp chuyển thành công."
                    };
                }

            }

            return new APIResponse((int)ResponseCode.TransactionFailed);

        }
    }

    public class CardDVO
    {
        public string Name { get; set; }
        public string Serial { get; set; }
        public string Pin { get; set; }
    }
}