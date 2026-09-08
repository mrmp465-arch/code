using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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
    public class WebService
    {
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        static string CaptchaProvider = ConfigurationManager.AppSettings["Captcha_Provider"] ?? "captcha.com.vn";
        static bool ReportIncorrectCaptcha = bool.Parse(ConfigurationManager.AppSettings["Report_Incorrect_Captcha"] ?? "true");
        static bool CheckSerial_MyVTT = bool.Parse(ConfigurationManager.AppSettings["CheckSerialFirst_MyVTT"] ?? "true");
        static bool CheckSerial_Service = bool.Parse(ConfigurationManager.AppSettings["CheckSerialFirst_Service"] ?? "true");


        public static LoginStatus Login(int type, ref Account refAccount)
        {

            //Get DB : Type= 0 : Check thẻ;  Type = 1: Nap ho trả trước ko my, Type = 2: nap hộ trả sau, , Type = 3: nap hộ ftth
            //Type = -1 : Login chính nó 

            Account account;
            if (type > -1)
            {
                account = new Account().GetAccount(type);

                if (account == null)
                {
                    NLogLogger.Info(new string[] { "MyViettelWeb", "Login", "GetAccount", "NULL - Hết account My", type.ToString() });

                    if (type > 0) // Ghi vao Cache để báo  hết tài khoản My chỉ chạy USSD
                    {
                        UtilsWeb.SetConfigCache("ussd" + type, "true");
                        NLogLogger.Info(new string[] { "MyViettelWeb", "Login", "Set ussd:config", "true" });
                    }

                    return new LoginStatus()
                    {
                        Status = 0
                    };
                    //return string.Empty;
                }
                else
                {
                    if (type > 0) // Ghi vao Cache để báo vẫn con My
                    {
                        UtilsWeb.SetConfigCache("ussd" + type, "false");
                        NLogLogger.Info(new string[] { "MyViettelWeb", "Login", "Set ussd:config", "false" });
                    }
                }
            }
            else
            {
                account = refAccount;
            }

            var token = UtilsWeb.GetTokenCache(account.AccountName);
            //if (string.IsNullOrEmpty(token)) // Lấy token cuối trong DB
            //{
            //    token = account.LastToken;
            //}

            if (!string.IsNullOrEmpty(token))
            {
                NLogLogger.Info(new string[] { "MyViettelWeb", "Login", account.AccountName, "Token Cached or DB", token });
                refAccount = account;
                return new LoginStatus()
                {
                    Status = 1,
                    Token = token
                };
            }

            else
            {

                var loginParam = new LoginRequestWeb() { account = account.AccountName, account_target = string.Empty, device_id = "webportal", password = account.Password };

                for (int i = 0; i < 5; i++)
                {
                    NLogLogger.Info(new string[] { "MyViettelWeb", "Login", "Try", i.ToString(), account.AccountName });
                    try
                    {
                        var res = Task.Run(async () => await UtilsWeb.PostTask("https://vietteltelecom.vn/api/login-user-by-phone", serializer.Serialize(loginParam))).Result;

                        NLogLogger.Info(new string[] { "MyViettelWeb", "Login", account.AccountName, "Response", Regex.Unescape(res) });

                        token = (string)JObject.Parse(res)["\u0000*\u0000info"]["token"];

                        if (!string.IsNullOrEmpty(token))
                        {

                            UtilsWeb.SetTokenCache(account.AccountName, token);
                            account.LastToken = token;
                            account.ProductCode = (string)JObject.Parse(res)["\u0000*\u0000info"]["productCode"];
                            account.Update();
                            NLogLogger.Info(new string[] { "MyViettelWeb", "Login", account.AccountName, token });
                            refAccount = account;
                            return new LoginStatus()
                            {
                                Status = 1,
                                Token = token
                            };
                        }
                        else
                        {
                            account.Status = 0;
                            account.Update();
                            return new LoginStatus()
                            {
                                Status = -2,
                                Message = (string)JObject.Parse(res)["message"]
                            };

                        }


                    }
                    catch (ThreadAbortException exp)
                    {
                        NLogLogger.Info(new string[] { "MyViettelWeb", "Login", "ThreadAbortException", serializer.Serialize(loginParam), exp.Message });
                        Thread.ResetAbort();
                    }

                    catch (Exception exp)
                    {
                        NLogLogger.Info(new string[] { "MyViettelWeb", "Login", "Error", serializer.Serialize(loginParam), exp.Message });
                    }
                }

                NLogLogger.Info(new string[] { "MyViettelWeb", "Login Missing 5", account.AccountName, token });
                account.Status = 1;
                account.Update();
            }
            refAccount = account;

            return new LoginStatus()
            {
                Status = 0,
                Token = string.Empty
            };
        }

        public static CData GetCaptchaLink()
        {
            //Thread.Sleep(1000);
            for (int i = 0; i < 5; i++)
            {
                try
                {

                    NLogLogger.Info(new string[] { "MyViettelWeb", "GetCaptchaLink", "Retry", i.ToString() });
                    string url = "https://vietteltelecom.vn/api/get-captcha";

                    var res = Task.Run(async () => await UtilsWeb.PostTask(url, string.Empty)).Result;

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    var response = serializer.Deserialize<Entity.CaptchaResponse>(res);
                    NLogLogger.Info(new string[] { "MyViettelWeb", "GetCaptchaLink", serializer.Serialize(response) });
                    if (response != null)
                    {
                        if (response.errorCode == 0)
                        {
                            return response.data;
                        }
                    }
                }

                catch (ThreadAbortException exp)
                {
                    NLogLogger.Info(new string[] { "MyViettelWeb", "GetCaptchaLink", "Exception", exp.Message });
                    Thread.ResetAbort();
                }

                catch (Exception exp)
                {
                    NLogLogger.Info(new string[] { "MyViettelWeb", "GetCaptchaLink", "ThreadAbortException", exp.Message });
                }

                //try { Thread.Sleep(1000); } catch (ThreadAbortException exp) { Thread.ResetAbort(); }
            }

            return null;
        }

        private static void PreGenCaptCha(int id)
        {
            var captcha = GetCaptchaLink();
            if (captcha != null)
            {
                var sessionCaptcha = UtilsWeb.DeCaptcha(captcha.url, captcha.sid);
                if (sessionCaptcha != null)
                {
                    sessionCaptcha.Type = 2;
                    sessionCaptcha.Add();
                }
            }
        }

        private static void ReportCaptcha(int TaskId)
        {
            var result = new Lib.Captcha.Anticaptcha.AnticaptchaService().ReportIncorrectImageCaptcha(TaskId);
            NLogLogger.Info(new string[] { "MyViettelWeb", "ReportCaptcha", TaskId.ToString(), result.ToString() });
        }

        private static void ReportCaptcha(string imageBase64, int type, string resultCap)
        {
            var result = new Lib.Captcha.CaptchaComVn.CaptchaComVnService().ReportIncorrectImageCaptcha(imageBase64, type, resultCap);
            NLogLogger.Info(new string[] { "MyViettelWeb", "ReportCaptcha", resultCap, result.ToString() });
        }

        public static APIResponse CheckCard(string cardSerial)
        {
            //Check Cached 
            var cardCache = UtilsWeb.GetCardSerialCache(cardSerial + "_CardUsed");
            if (!string.IsNullOrEmpty(cardCache))
                return serializer.Deserialize<APIResponse>(cardCache);

            //Thread.Sleep(500);

            var account = new Account();

            var tryAgain = true;
            var loginStatus = Login(0, ref account);
            while (tryAgain && loginStatus.Status == -1)
            {
                loginStatus = Login(0, ref account);
                tryAgain = false;
            }

            if (loginStatus.Status == 0) // Het tai khoan check
            {
                return new APIResponse()
                {
                    ResponseCode = (int)ResponseCode.AccountNotExists
                };
            }

            if (loginStatus.Status == 1)
            {
                var decaptcha = new Captcha().GetCaptcha(2);
                if (decaptcha == null)
                {
                    //var sid = UtilsWeb.GenSid();
                    //var rand = Encrypts.MD5(sid);
                    //var url = string.Format("http://apivtp.vietteltelecom.vn/myviettel.php/gen-img-captcha?sid={0}&rand={1}", sid, rand);
                    //decaptcha = UtilsWeb.DeCaptcha(url, sid, Client);

                    var captcha = GetCaptchaLink();//(Client);
                    if (captcha == null)
                    {
                        return new APIResponse((int)ResponseCode.ParameterInvalid);
                    }

                    decaptcha = UtilsWeb.DeCaptcha(captcha.url, captcha.sid);
                }


                //Gen truoc x Captcha
                if (CaptchaProvider == "anti-captcha.com")
                {
                    for (int i = 0; i < 1; i++)
                    {

                        //Action<int, HttpClient> send = PreGenCaptCha;
                        Action<int> send = PreGenCaptCha;
                        send.BeginInvoke(0, null, null);
                    }
                }

                if (decaptcha != null)
                {


                    string url = "https://vietteltelecom.vn/api/check-card-info";

                    var parameters = serializer.Serialize(new CheckCardRequest()
                    {
                        token = loginStatus.Token,
                        serial = cardSerial,
                        captcha = decaptcha.Value,
                        sid = decaptcha.SessionId
                    });

                    for (int i = 0; i < 5; i++)
                    {
                        NLogLogger.Info(new string[] { "MyViettelWeb", "CheckCardRequest", "Try", i.ToString(), account.AccountName, serializer.Serialize(parameters) });
                        var res = string.Empty;
                        try
                        {
                            res = Task.Run(async () => await UtilsWeb.PostTask(url, parameters)).Result;
                            if (!string.IsNullOrEmpty(res))
                            {
                                UtilsWeb.SetCardSerialCache(cardSerial, res);
                                NLogLogger.Info(new string[] { "MyViettelWeb", "CheckCardResponse", account.AccountName, cardSerial, Regex.Unescape(res) });
                                var resCard = serializer.Deserialize<CheckCardResponse>(res.Replace("[]", "null"));
                                switch (resCard.errorCode)
                                {
                                    case -1: // Phiên đăng nhập của bạn đã hết
                                        UtilsWeb.RemoveTokenCache(account.AccountName);
                                        account.Status = 1;
                                        account.Update();
                                        return new APIResponse((int)ResponseCode.AccessDenied);
                                    case -2: // Tài khoản của quý khách đã đăng nhập nơi khác
                                        UtilsWeb.RemoveTokenCache(account.AccountName);
                                        account.Status = 1;
                                        account.Update();
                                        return new APIResponse((int)ResponseCode.AccessDenied);
                                    case 2: //Hệ thống đang bận,
                                        account.Status = 1;
                                        account.Update();
                                        return new APIResponse((int)ResponseCode.SystemBusy);
                                    case 3: //Hệ thống đang nâng cấp,
                                        account.Status = 1;
                                        account.Update();
                                        return new APIResponse((int)ResponseCode.SystemMaintain);
                                    case 0:
                                        if (Regex.Unescape(resCard.message).Contains("Số lần tra cứu quá giới hạn"))
                                        {
                                            NLogLogger.Info(new string[] { "MyViettelWeb", "CheckCard", cardSerial, serializer.Serialize(account) });
                                            account.CountCheck = 3;
                                            account.Status = 1;
                                            account.Update();
                                            return new APIResponse()
                                            {
                                                ResponseCode = (int)ResponseCode.TransactionLimit,
                                                Description = Regex.Unescape(resCard.message)
                                            };

                                            //return CheckCardApi(cardSerial); // Dung ham xin

                                        }
                                        if (Regex.Unescape(resCard.message).Contains("Thẻ chưa kích hoạt"))
                                        {
                                            NLogLogger.Info(new string[] { "MyViettelWeb", "CheckCard", cardSerial, serializer.Serialize(account) });
                                            account.Status = 1;
                                            account.Update();
                                            return new APIResponse()
                                            {
                                                ResponseCode = (int)ResponseCode.CardNotActivated,
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        if (Regex.Unescape(resCard.message).Contains("Không tìm thấy thông tin thẻ cào"))
                                        {
                                            NLogLogger.Info(new string[] { "MyViettelWeb", "CheckCard", cardSerial, serializer.Serialize(account) });
                                            account.Status = 1;
                                            account.Update();
                                            return new APIResponse()
                                            {
                                                ResponseCode = (int)ResponseCode.CardSerialInvalid,
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        if (resCard.data != null)
                                        {
                                            account.CountCheck++;
                                            account.Status = 1;
                                            account.Update();
                                            var responseApi = new APIResponse();
                                            var cardDetail = new CheckSerialResponse()
                                            {
                                                cardSerial = cardSerial,
                                                cardExpired = resCard.data.datExp,
                                                cardValue = resCard.data.amount,
                                                dateUsed = resCard.data.dateUse,
                                                isdn = resCard.data.isdn,
                                                ownerName = string.Empty
                                            };

                                            responseApi.Description = resCard.message;
                                            responseApi.ResponseContent = serializer.Serialize(cardDetail);

                                            if (Regex.Unescape(resCard.message).Contains("Thẻ đã sử dụng"))
                                            {
                                                responseApi.ResponseCode = (int)ResponseCode.CardUsed;
                                                // Set Cached Check Card for review checking
                                                UtilsWeb.SetCardSerialCache(cardSerial + "_CardUsed", serializer.Serialize(responseApi));
                                            }

                                            if (Regex.Unescape(resCard.message).Contains("Thẻ chưa sử dụng"))
                                            {
                                                responseApi.ResponseCode = (int)ResponseCode.TransactionSuccessful;
                                            }

                                            return responseApi;

                                        }
                                        else
                                        {
                                            return new APIResponse((int)ResponseCode.TransactionFailed);
                                        }
                                    case -4:
                                        account.Status = 1;
                                        account.Update();
                                        //if (ReportIncorrectCaptcha)
                                        //{
                                        //    Action<int> send = ReportCaptcha;
                                        //    send.BeginInvoke(decaptcha.TaskId, null, null);
                                        //}

                                        //Cap Comvn
                                        if (ReportIncorrectCaptcha)
                                        {
                                            Action<string, int, string> send = ReportCaptcha;
                                            send.BeginInvoke(decaptcha.ImgBase64, 1, decaptcha.Value, null, null);
                                        }

                                        return new APIResponse((int)ResponseCode.ParameterInvalid);
                                    case -3:
                                        account.Status = 1;
                                        account.Update();
                                        return new APIResponse((int)ResponseCode.CardSerialInvalid);
                                    default:
                                        return new APIResponse((int)ResponseCode.TransactionFailed);


                                }

                            }
                        }
                        catch (Exception e)
                        {
                            NLogLogger.Info(new string[] { "MyViettelWeb", "CheckCard", "Error", cardSerial, e.Message });
                        }

                        //try { Thread.Sleep(1000); } catch (ThreadAbortException exp) { Thread.ResetAbort(); }
                    }
                    account.Status = 1;
                    account.Update();
                    return new APIResponse((int)ResponseCode.TransactionFailed);

                }
                else
                {
                    return new APIResponse((int)ResponseCode.ParameterInvalid);
                }
            }

            //if (loginStatus.Status == -1)
            //{
            //    account.Status = -2; //Khóa vĩnh viễn
            //    account.Update();
            //}

            return new APIResponse((int)ResponseCode.LoginFail);

        }

        public static APIResponse TopupCard(string cardSerial, string cardCode, string mobile, int type, int topupType, int userAmount, bool checkCard)
        {


            if (string.IsNullOrEmpty(mobile))
            {
                return new APIResponse((int)ResponseCode.ServiceIsLocked)
                {
                    Description = "Mobile là bắt buộc"
                };
            }

            if (type == 1 || type == 3)
            {
                Regex regex = new Regex("^[0-9]+$");
                if (!regex.IsMatch(mobile))
                {
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = "Mobile không phải số điện thoại"
                    };
                }
            }

            if (CheckSerial_MyVTT && checkCard)
            {

                if (UtilsWeb.GetCardSerialCache(cardSerial) == null)
                {
                    int tryAgain = 0;
                    var checkReponse = WebService.CheckCard(cardSerial);
                    while (tryAgain < 5 && (checkReponse.ResponseCode == (int)ResponseCode.LoginFail
                                            //|| checkReponse.ResponseCode == (int)ResponseCode.TransactionLimit
                                            || checkReponse.ResponseCode == (int)ResponseCode.AccessDenied
                                            || checkReponse.ResponseCode == (int)ResponseCode.SystemBusy
                                            || checkReponse.ResponseCode == (int)ResponseCode.ParameterInvalid))
                    {
                        NLogLogger.Info(new string[] { "MyViettelWeb", "CheckCard", "Try", tryAgain.ToString(), checkReponse.ResponseCode.ToString(), checkReponse.Description });
                        checkReponse = WebService.CheckCard(cardSerial);
                        if (checkReponse.ResponseCode == (int)ResponseCode.AccountNotExists) tryAgain = 3;
                        tryAgain++;
                    }
                    if (checkReponse.ResponseCode == (int)ResponseCode.CardUsed
                        || checkReponse.ResponseCode == (int)ResponseCode.CardSerialInvalid
                        || checkReponse.ResponseCode == (int)ResponseCode.CardNotActivated)
                    {
                        return checkReponse;
                    }
                }
            }
            //End Check serial

            var account = new Account();

            var tryAgainl = true;
            LoginStatus loginStatus = new LoginStatus();

            if (topupType == 1)
            {
                loginStatus = Login(1, ref account);
                while (tryAgainl && loginStatus.Status == -1)
                {
                    loginStatus = Login(1, ref account);
                    tryAgainl = false;
                }
            }
            else if (topupType == 2 || topupType == 3 || topupType == 5) // Trả sau, FTTH, HomePhone
            {

                //if (topupType == 3) // Nap FTTH
                //    loginStatus = Login(3, ref account);
                //else
                //    loginStatus = Login(2, ref account);
                loginStatus = Login(topupType, ref account);

                while (tryAgainl && loginStatus.Status == -1)
                {
                    loginStatus = Login(topupType, ref account);
                    tryAgainl = false;
                }
            }

            if (loginStatus.Status == 1)
            {
                var decaptcha = new Captcha().GetCaptcha(2);
                if (decaptcha == null)
                {
                    //Gen truoc x Captcha

                    //for (int i = 0; i < 1; i++)
                    //{

                    //    Action<int> send = PreGenCaptCha;
                    //    send.BeginInvoke(0, null, null);
                    //}

                    //account.Status = 1;
                    //account.Update();
                    //return new APIResponse((int)ResponseCode.ParameterInvalid);

                    //var sid = UtilsWeb.GenSid();
                    //var rand = Encrypts.MD5(sid);
                    //var url = string.Format("http://apivtp.vietteltelecom.vn/myviettel.php/gen-img-captcha?sid={0}&rand={1}", sid, rand);
                    //decaptcha = UtilsWeb.DeCaptcha(url, sid, Client);

                    var captcha = GetCaptchaLink();
                    if (captcha != null)
                    {
                        decaptcha = UtilsWeb.DeCaptcha(captcha.url, captcha.sid);
                        if (decaptcha == null)
                        {
                            NLogLogger.Info(new string[] { "MyViettelWeb", "TopupCard", "GetCaptchaBase64 NULL" });
                            account.Status = 1;
                            account.Update();
                            return new APIResponse((int)ResponseCode.ParameterInvalid)
                            {
                                Description = "Parameter Invalid"
                            };
                        }
                    }
                    else
                    {
                        NLogLogger.Info(new string[] { "MyViettelWeb", "TopupCard", "GetCaptchaLink NULL" });
                        account.Status = 1;
                        account.Update();
                        return new APIResponse((int)ResponseCode.ParameterInvalid)
                        {
                            Description = "Parameter Invalid"
                        };
                    }

                }

                //Gen truoc x Captcha
                if (CaptchaProvider == "anti-captcha.com")
                {
                    for (int i = 0; i < 1; i++)
                    {
                        //Action<int, HttpClient> send = PreGenCaptCha;
                        //send.BeginInvoke(0, Client, null, null);
                        Action<int> send = PreGenCaptCha;
                        send.BeginInvoke(0, null, null);
                    }
                }

                if (decaptcha != null)
                {

                    string url = "https://vietteltelecom.vn/api/thanh-toan-online-v2";

                    var parameters = serializer.Serialize(new TopupRequest()
                    {
                        token = loginStatus.Token,
                        cardcode = cardCode,
                        phone = mobile,
                        captcha = decaptcha.Value,
                        sid = decaptcha.SessionId,
                        type = 0
                    });

                    for (int i = 0; i < 5; i++)
                    {
                        NLogLogger.Info(new string[] { "MyViettelWeb", "TopupCardRequest", "Try", i.ToString(), parameters });
                        try
                        {
                            var res = Task.Run(async () => await UtilsWeb.PostTask(url, parameters)).Result;

                            
                            if (!string.IsNullOrEmpty(res))
                            {
                                NLogLogger.Info(new string[] { "MyViettelWeb", "TopupCardResponse", account.AccountName, mobile, cardCode, Regex.Unescape(res) });

                                if (res.Contains("</script>"))
                                {
                                    res = "{" + res.Split('{')[1];
                                }

                                var resCard = serializer.Deserialize<TopupResponse>(res);
                                switch (resCard.errorCode)
                                {
                                    case 0:
                                        account.Status = 1;
                                        account.CountCharge++;
                                        account.Update();
                                        int amountresponse;
                                        int.TryParse(Regex.Match(resCard.message, @"\d+").Value, out amountresponse);
                                        int[] listValue = { 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000 };

                                        if (listValue.Contains(amountresponse))
                                        {
                                            return new APIResponse((int)ResponseCode.TransactionSuccessful)
                                            {
                                                Description = Regex.Unescape(resCard.message),
                                                ResponseContent = amountresponse.ToString()
                                            };
                                        }

                                        else
                                        {

                                            if (userAmount == 10000) //Nghiễm nhiên thành công
                                            {
                                                return new APIResponse((int)ResponseCode.TransactionSuccessful)
                                                {
                                                    Description = Regex.Unescape(resCard.message) + " | Nghiem nhien thanh cong",
                                                    ResponseContent = 10000.ToString()
                                                };
                                            }

                                            var amountReal = 0;
                                            //kiểm tra thẻ mệnh giá
                                            var resCardCheck = MyViettelService.CheckCard(cardSerial);
                                            NLogLogger.Info(new string[] { "CheckCard ResponseContent", resCardCheck.ResponseContent });

                                            var tryAgain = 0;
                                            while (tryAgain < 3 && (resCardCheck.ResponseCode == (int)ResponseCode.LoginFail
                                                                    || resCardCheck.ResponseCode == (int)ResponseCode.TransactionLimit
                                                                    || resCardCheck.ResponseCode == (int)ResponseCode.AccessDenied
                                                                    || resCardCheck.ResponseCode == (int)ResponseCode.SystemBusy
                                                                    || resCardCheck.ResponseCode == (int)ResponseCode.ParameterInvalid))
                                            {
                                                resCardCheck = MyViettelService.CheckCard(cardSerial);
                                                tryAgain++;
                                            }


                                            switch (resCardCheck.ResponseCode)
                                            {
                                                case (int)ResponseCode.CardUsed:
                                                    {
                                                        var card = serializer.Deserialize<CheckSerialResponse>(resCardCheck.ResponseContent);
                                                        DateTime timeCheck = DateTime.Now;
                                                        DateTime dateUsed = DateTime.ParseExact(card.dateUsed, "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture);
                                                        var totalsec = (timeCheck - dateUsed).TotalSeconds;

                                                        if (topupType == 1 || topupType == 2)
                                                        {
                                                            if (!mobile.Contains(card.isdn.Replace("xxxx", "")))
                                                            {
                                                                NLogLogger.Info(new string[] { "CheckCard MissTime", totalsec.ToString(), resCardCheck.ResponseContent });
                                                                return new APIResponse((int)ResponseCode.CardUsed)
                                                                {
                                                                    Description = Regex.Unescape(resCard.message) + " | " + resCardCheck.ResponseContent
                                                                };
                                                            }
                                                        }

                                                        if (totalsec > 0 && totalsec <= 90) // && mobile.Contains(card.isdn.Replace("xxxx", "")))
                                                        {
                                                            amountReal = Convert.ToInt32(card.cardValue);
                                                            return new APIResponse((int)ResponseCode.TransactionSuccessful)
                                                            {
                                                                Description = Regex.Unescape(resCard.message) + " | " + resCardCheck.ResponseContent,
                                                                ResponseContent = amountReal.ToString()
                                                            };
                                                        }

                                                        else if (totalsec > 90 && totalsec <= 300)
                                                        {
                                                            NLogLogger.Info(new string[] { "CheckCard MissTime", totalsec.ToString(), resCardCheck.ResponseContent });
                                                            return new APIResponse((int)ResponseCode.TransactionSuspicious)
                                                            {
                                                                Description = Regex.Unescape(resCard.message) + " | " + resCardCheck.ResponseContent
                                                            };
                                                        }

                                                        else
                                                        {
                                                            NLogLogger.Info(new string[] { "CheckCard MissTime", totalsec.ToString(), resCardCheck.ResponseContent });
                                                            return new APIResponse((int)ResponseCode.CardSerialInvalid)
                                                            {
                                                                Description = Regex.Unescape(resCard.message) + " | " + resCardCheck.ResponseContent
                                                            };
                                                        }
                                                    }

                                                case (int)ResponseCode.CardNotActivated:
                                                    return new APIResponse((int)ResponseCode.CardNotActivated)
                                                    {
                                                        Description = Regex.Unescape(resCard.message) + " | " + resCardCheck.Description
                                                    };
                                                case (int)ResponseCode.CardSerialInvalid:
                                                    return new APIResponse((int)ResponseCode.CardSerialInvalid)
                                                    {
                                                        Description = Regex.Unescape(resCard.message) + " | " + resCardCheck.Description
                                                    };
                                                default:
                                                    return new APIResponse((int)ResponseCode.TransactionSuspicious)
                                                    {
                                                        Description = Regex.Unescape(resCard.message) + " | " + resCardCheck.Description
                                                    };
                                            }
                                        }

                                    case -1: // Phiên đăng nhập của bạn đã hết
                                        UtilsWeb.RemoveTokenCache(account.AccountName);
                                        account.Status = 1;
                                        account.Update();
                                        return new APIResponse((int)ResponseCode.AccessDenied)
                                        {
                                            Description = Regex.Unescape(resCard.message)
                                        };
                                    case -2: //Tài khoản của quý khách đã đăng nhập nơi khác
                                        UtilsWeb.RemoveTokenCache(account.AccountName);
                                        account.Status = 1;
                                        account.Update();
                                        return new APIResponse((int)ResponseCode.AccessDenied)
                                        {
                                            Description = Regex.Unescape(resCard.message)
                                        };
                                    case 3: //Hệ thống đang nâng cấp, Truyền thiếu tham số token.
                                        account.Status = 1;
                                        account.Update();
                                        return new APIResponse((int)ResponseCode.SystemMaintain)
                                        {
                                            Description = Regex.Unescape(resCard.message)
                                        };
                                    case -4: //Mã bảo mật không chính xác
                                        account.Status = 1;
                                        account.Update();
                                        if (ReportIncorrectCaptcha)
                                        {
                                            Action<string, int, string> send = ReportCaptcha;
                                            send.BeginInvoke(decaptcha.ImgBase64, 1, decaptcha.Value, null, null);
                                        }

                                        return new APIResponse((int)ResponseCode.ParameterInvalid)
                                        {
                                            Description = Regex.Unescape(resCard.message)
                                        };

                                    case 1:
                                        if (Regex.Unescape(resCard.message).Contains("Thuê bao cố định không được nạp thẻ hộ cho các thuê bao khác"))
                                        {
                                            account.Status = -1;
                                            account.Update();
                                            return new APIResponse((int)ResponseCode.AccountLocked)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }
                                        break;

                                    case 2:
                                        if (Regex.Unescape(resCard.message).Contains("Quý khách đã thực hiện nạp thẻ hộ quá số lần quy định trong ngày") ||
                                            Regex.Unescape(resCard.message).Contains("Thuê bao đã nạp thẻ hộ đủ số lần trong ngày"))
                                        {
                                            account.CountCharge = 5;
                                            account.Status = 1;
                                            account.Update();
                                            return new APIResponse((int)ResponseCode.TransactionLimit)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        //Trả trước 136 nạp sai qua lần lock đơn
                                        if (Regex.Unescape(resCard.message).Contains("Nạp tiền không thành công do thuê bao đã nạp sai quá số lần quy định"))
                                        {
                                            //account.Status = 1;
                                            //account.Update();
                                            //return new APIResponse((int)ResponseCode.ServiceIsLocked)
                                            //{
                                            //    Description = Regex.Unescape(resCard.message)
                                            //};

                                            account.Status = 1;
                                            account.CountCharge = 5;
                                            account.Update();
                                            return new APIResponse((int)ResponseCode.TransactionLimit)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        if (Regex.Unescape(resCard.message).Contains("Nạp tiền không thành công"))
                                        {
                                            account.Status = 1;
                                            account.Update();

                                            return new APIResponse((int)ResponseCode.SystemBusy)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };

                                        }


                                        if (Regex.Unescape(resCard.message).Contains("Mã thẻ cào phải là 13 hoặc 15 ký tự")
                                            || Regex.Unescape(resCard.message).Contains("Thẻ cào không hợp lệ hoặc đã được sử dụng")
                                            //|| Regex.Unescape(resCard.message).Contains("Thẻ cào không hợp lệ hoặc thuê bao nạp không đủ điều kiện nạp thẻ hộ")
                                            )

                                        {
                                            account.Status = 1;
                                            account.Update();
                                            return new APIResponse((int)ResponseCode.CardCodeInvalid)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        if (Regex.Unescape(resCard.message).Contains("Thẻ cào không hợp lệ hoặc thuê bao nạp không đủ điều kiện nạp thẻ hộ")) // Nghi vấn nuốt thẻ phải check thêm lần nữa

                                        {

                                            account.Status = 1;
                                            account.Update();
                                            return new APIResponse((int)ResponseCode.CardCodeInvalid)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };

                                        }

                                        if (Regex.Unescape(resCard.message).Contains("Thuê bao không đủ điều kiện nạp thẻ hộ")
                                            || Regex.Unescape(resCard.message).Contains("Nạp thẻ không thành công do thông tin thuê bao không hợp lệ")
                                            || Regex.Unescape(resCard.message).Contains("Thuê bao cố định không được nạp thẻ hộ cho các thuê bao khác")
                                            //|| Regex.Unescape(resCard.message).Contains("Thẻ cào không hợp lệ hoặc thuê bao nạp không đủ điều kiện nạp thẻ hộ")
                                            )
                                        {
                                            account.Status = -2;
                                            account.Update();
                                            return new APIResponse((int)ResponseCode.AccountLocked)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        if (Regex.Unescape(resCard.message).Contains("Nạp sai quá số lần qui định trong ngày")
                                            || Regex.Unescape(resCard.message).Contains("Quý khách đã nhập sai quá 5 lần mã thẻ cào")
                                            || Regex.Unescape(resCard.message).Contains("Quý khách đã nạp sai mã thẻ cào 5 lần liên tiếp")
                                            || Regex.Unescape(resCard.message).Contains("Tài khoản của quý khách không đủ để thực hiện giao dịch nạp thẻ hộ")

                                            //|| Regex.Unescape(resCard.message).Contains("Thuê bao không đủ điều kiện nạp thẻ hộ") //Khóa TK nạp hộ (ko nghịch ngu)
                                            )
                                        {
                                            account.Status = -1;
                                            account.Update();
                                            return new APIResponse((int)ResponseCode.AccountLocked)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }


                                        if (Regex.Unescape(resCard.message).Contains("Hệ thống đang bận"))
                                        {
                                            account.Status = 1;
                                            account.Update();
                                            return new APIResponse((int)ResponseCode.SystemBusy)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        if (Regex.Unescape(resCard.message).Contains("Thuê bao không được phép thanh toán bằng thẻ cào")
                                            //|| Regex.Unescape(resCard.message).Contains("Thuê bao không đủ điều kiện nạp thẻ hộ") //Mở lại đã test
                                            //|| Regex.Unescape(resCard.message).Contains("Hệ thống đang bận, Quý khách vui lòng thử lại sau")
                                            || Regex.Unescape(resCard.message).Contains("Thuê bao nhận không đủ điều kiện nạp thẻ") //136 ko My
                                            )
                                        {
                                            account.Status = 1;
                                            account.Update();
                                            return new APIResponse((int)ResponseCode.ServiceIsLocked)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        if (Regex.Unescape(resCard.message).Contains("Quá trình thực hiện có lỗi")
                                            /*|| Regex.Unescape(resCard.message).Contains("Thao tác không thành công")*/)
                                        {
                                            account.Status = 1;
                                            account.Update();
                                            return new APIResponse((int)ResponseCode.TransactionRejected)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        if (Regex.Unescape(resCard.message).Contains("Hệ thống đang nâng cấp"))
                                        {
                                            account.Status = 1;
                                            account.Update();
                                            return new APIResponse((int)ResponseCode.SystemMaintain)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        if (Regex.Unescape(resCard.message).Contains("Thẻ cào đã nạp thành công cho thuê bao nhưng chưa được gạch nợ")
                                            || Regex.Unescape(resCard.message).Contains("Thao tác không thành công"))

                                        {

                                            account.Status = 1;
                                            account.CountCharge++;
                                            account.Update();
                                            return new APIResponse((int)ResponseCode.TransactionSuspicious)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        account.Status = 1;
                                        account.Update();
                                        return new APIResponse((int)ResponseCode.TransactionFailed)
                                        {
                                            Description = Regex.Unescape(resCard.message)
                                        };

                                    default:
                                        account.Status = 1;
                                        account.Update();
                                        return new APIResponse((int)ResponseCode.TransactionFailed)
                                        {
                                            Description = Regex.Unescape(resCard.message)
                                        };
                                }
                            }
                            else NLogLogger.Info(new string[] { "MyViettelWeb", "TopupCardRequest", "Return Empty", "Try", i.ToString(), serializer.Serialize(parameters) });

                        }
                        catch (ThreadAbortException exp)
                        {
                            NLogLogger.Info(new string[] { "MyViettelWeb", "TopupCard", "ThreadAbortException", serializer.Serialize(parameters), exp.Message });
                            Thread.ResetAbort();
                        }

                        catch (Exception exp)
                        {
                            NLogLogger.Info(new string[] { "MyViettelWeb", "TopupCard", "Exception", serializer.Serialize(parameters), exp.Message });
                        }


                    }

                    account.Status = 1;
                    account.Update();
                    return new APIResponse((int)ResponseCode.TransactionTimeout);

                }

            }

            return new APIResponse((int)ResponseCode.LoginFail);

        }

        public static APIResponse TopupCard(string cardSerial, string cardCode, string mobile, int type, string accountName, string passWord, bool checkCard)
        {
            //Nap 136

            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(passWord))
            {
                return new APIResponse((int)ResponseCode.ServiceIsLocked)
                {
                    Description = "Mobile or AccountName or Password is Empty"
                };
            }


            if (CheckSerial_MyVTT && checkCard)
            {

                if (UtilsWeb.GetCardSerialCache(cardSerial) == null)
                {
                    int tryAgain = 0;
                    var checkReponse = WebService.CheckCard(cardSerial);
                    while (tryAgain < 5 && (checkReponse.ResponseCode == (int)ResponseCode.LoginFail
                                            || checkReponse.ResponseCode == (int)ResponseCode.TransactionLimit
                                            || checkReponse.ResponseCode == (int)ResponseCode.AccessDenied
                                            || checkReponse.ResponseCode == (int)ResponseCode.SystemBusy
                                            || checkReponse.ResponseCode == (int)ResponseCode.ParameterInvalid))
                    {
                        //try { Thread.Sleep(1000); } catch (ThreadAbortException exp) { Thread.ResetAbort(); }
                        checkReponse = WebService.CheckCard(cardSerial);
                        if (checkReponse.ResponseCode == (int)ResponseCode.AccountNotExists) tryAgain = 3;
                        tryAgain++;
                    }
                    if (checkReponse.ResponseCode == (int)ResponseCode.CardUsed
                        || checkReponse.ResponseCode == (int)ResponseCode.CardSerialInvalid
                        || checkReponse.ResponseCode == (int)ResponseCode.CardNotActivated)
                    {
                        return checkReponse;
                    }
                }
            }


            var account = new Account()
            {
                AccountName = accountName,
                Password = passWord,
                LastChangePass = DateTime.Now
            };


            //Login chính đơn
            var loginStatus = Login(-1, ref account);

            if (loginStatus.Status == 1)
            {

                //Add Account for Check
                Action<string, string> addAccount = AddAccount136;
                addAccount.BeginInvoke(account.AccountName, account.Password, null, null);

                var decaptcha = new Captcha().GetCaptcha(2);
                if (decaptcha == null)
                {
                    //Gen truoc x Captcha
                    //var sid = UtilsWeb.GenSid();
                    //var rand = Encrypts.MD5(sid);
                    //var url = string.Format("http://apivtp.vietteltelecom.vn/myviettel.php/gen-img-captcha?sid={0}&rand={1}", sid, rand);
                    //decaptcha = UtilsWeb.DeCaptcha(url, sid, Client);

                    var captcha = GetCaptchaLink();
                    if (captcha != null)
                    {
                        decaptcha = UtilsWeb.DeCaptcha(captcha.url, captcha.sid);
                        if (decaptcha == null)
                        {
                            NLogLogger.Info(new string[] { "MyViettelWeb", "TopupCard", "GetCaptchaBase64 NULL" });
                            account.Status = 1;
                            account.Update();
                            return new APIResponse((int)ResponseCode.ParameterInvalid)
                            {
                                Description = "Parameter Invalid"
                            };
                        }
                    }
                    else
                    {
                        NLogLogger.Info(new string[] { "MyViettelWeb", "TopupCard", "GetCaptchaLink NULL" });
                        account.Status = 1;
                        account.Update();
                        return new APIResponse((int)ResponseCode.ParameterInvalid)
                        {
                            Description = "Parameter Invalid"
                        };
                    }
                }

                //Gen truoc x Captcha
                if (CaptchaProvider == "anti-captcha.com")
                {
                    for (int i = 0; i < 1; i++)
                    {
                        //Action<int, HttpClient> send = PreGenCaptCha;
                        //send.BeginInvoke(0, Client, null, null);
                        Action<int> send = PreGenCaptCha;
                        send.BeginInvoke(0, null, null);
                    }
                }

                if (decaptcha != null)
                {

                    string url = "https://vietteltelecom.vn/api/thanh-toan-online-v2";
                    var parameters = serializer.Serialize(new TopupRequest()
                    {
                        token = loginStatus.Token,
                        cardcode = cardCode,
                        phone = mobile,
                        captcha = decaptcha.Value,
                        sid = decaptcha.SessionId,
                        type = 0
                    });

                    //var parameters = new Dictionary<string, string>();
                    //parameters.Add("token", loginStatus.Token);
                    //parameters.Add("phone", mobile);
                    //parameters.Add("cardcode", cardCode);
                    //parameters.Add("type", type.ToString());
                    //parameters.Add("sid", decaptcha.SessionId);
                    //parameters.Add("captcha", decaptcha.Value);

                    for (int i = 0; i < 5; i++)
                    {
                        NLogLogger.Info(new string[] { "MyViettelWeb", "TopupCardRequest", "Try", i.ToString(), serializer.Serialize(parameters) });
                        //var res = string.Empty;
                        try
                        {

                            var res = Task.Run(async () => await UtilsWeb.PostTask(url, parameters)).Result;

                            if (!string.IsNullOrEmpty(res))
                            {
                                NLogLogger.Info(new string[] { "MyViettelWeb", "TopupCardResponse", account.AccountName, mobile, cardCode, Regex.Unescape(res) });

                                if (res.Contains("</script>"))
                                {
                                    res = "{" + res.Split('{')[1];
                                }

                                var resCard = serializer.Deserialize<TopupResponse>(res);
                                switch (resCard.errorCode)
                                {
                                    case 0:
                                        return new APIResponse((int)ResponseCode.TransactionSuccessful)
                                        {
                                            Description = Regex.Unescape(resCard.message),
                                            ResponseContent = Regex.Match(resCard.message, @"\d+").Value
                                        };
                                    case -1: // Phiên đăng nhập của bạn đã hết
                                        UtilsWeb.RemoveTokenCache(account.AccountName);
                                        return new APIResponse((int)ResponseCode.AccessDenied)
                                        {
                                            Description = Regex.Unescape(resCard.message)
                                        };
                                    case -2: //Tài khoản của quý khách đã đăng nhập nơi khác
                                        UtilsWeb.RemoveTokenCache(account.AccountName);
                                        return new APIResponse((int)ResponseCode.AccessDenied)
                                        {
                                            Description = Regex.Unescape(resCard.message)
                                        };
                                    case 3: //Hệ thống đang nâng cấp,
                                        return new APIResponse((int)ResponseCode.SystemMaintain)
                                        {
                                            Description = Regex.Unescape(resCard.message)
                                        };
                                    case -4: //Mã bảo mật không chính xác
                                             //if (ReportIncorrectCaptcha)
                                             //{
                                             //    Action<int> send = ReportCaptcha;
                                             //    send.BeginInvoke(decaptcha.TaskId, null, null);
                                             //}

                                        //Cap Comvn
                                        if (ReportIncorrectCaptcha)
                                        {
                                            Action<string, int, string> send = ReportCaptcha;
                                            send.BeginInvoke(decaptcha.ImgBase64, 1, decaptcha.Value, null, null);
                                        }

                                        return new APIResponse((int)ResponseCode.ParameterInvalid)
                                        {
                                            Description = Regex.Unescape(resCard.message)
                                        };
                                    case 2:
                                        if (Regex.Unescape(resCard.message).Contains("Quý khách đã thực hiện nạp thẻ hộ quá số lần quy định trong ngày")
                                            || Regex.Unescape(resCard.message).Contains("Nạp sai quá số lần qui định trong ngày")
                                            || Regex.Unescape(resCard.message).Contains("Quý khách đã nhập sai quá 5 lần mã thẻ cào")
                                            || Regex.Unescape(resCard.message).Contains("Quý khách đã nạp sai mã thẻ cào 5 lần liên tiếp")
                                            )
                                        {
                                            return new APIResponse((int)ResponseCode.TransactionIgnore)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        if (Regex.Unescape(resCard.message).Contains("Nạp thẻ không thành công do thông tin thuê bao không hợp lệ")
                                            || Regex.Unescape(resCard.message).Contains("Tài khoản của quý khách không đủ để thực hiện giao dịch nạp thẻ hộ")
                                            || Regex.Unescape(resCard.message).Contains("Thuê bao không được phép thanh toán bằng thẻ cào")
                                            || Regex.Unescape(resCard.message).Contains("Thuê bao không đủ điều kiện nạp thẻ hộ"))
                                        {
                                            return new APIResponse((int)ResponseCode.ServiceIsLocked)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        if (Regex.Unescape(resCard.message).Contains("Mã thẻ cào phải là 13 hoặc 15 ký tự")
                                            || Regex.Unescape(resCard.message).Contains("Thẻ cào không hợp lệ hoặc đã được sử dụng"))
                                        {

                                            return new APIResponse((int)ResponseCode.CardCodeInvalid)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        //if (Regex.Unescape(resCard.message).Contains("Nạp sai quá số lần qui định trong ngày")
                                        //    || Regex.Unescape(resCard.message).Contains("Quý khách đã nhập sai quá 5 lần mã thẻ cào")
                                        //    || Regex.Unescape(resCard.message).Contains("Quý khách đã nạp sai mã thẻ cào 5 lần liên tiếp"))
                                        //{
                                        //    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                                        //    {
                                        //        Description = Regex.Unescape(resCard.message)
                                        //    };
                                        //}

                                        if (Regex.Unescape(resCard.message).Contains("Hệ thống đang bận"))
                                        {
                                            return new APIResponse((int)ResponseCode.SystemBusy)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        //if (Regex.Unescape(resCard.message).Contains("Thuê bao không được phép thanh toán bằng thẻ cào")
                                        //    || Regex.Unescape(resCard.message).Contains("Thuê bao không đủ điều kiện nạp thẻ hộ")
                                        //    //|| Regex.Unescape(resCard.message).Contains("Hệ thống đang bận, Quý khách vui lòng thử lại sau")
                                        //    )
                                        //{
                                        //    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                                        //    {
                                        //        Description = Regex.Unescape(resCard.message)
                                        //    };
                                        //}

                                        if (Regex.Unescape(resCard.message).Contains("Quá trình thực hiện có lỗi")
                                            /*|| Regex.Unescape(resCard.message).Contains("Thao tác không thành công")*/)
                                        {
                                            return new APIResponse((int)ResponseCode.TransactionRejected)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        if (Regex.Unescape(resCard.message).Contains("Hệ thống đang nâng cấp"))
                                        {
                                            return new APIResponse((int)ResponseCode.SystemMaintain)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        if (Regex.Unescape(resCard.message).Contains("Thẻ cào đã nạp thành công cho thuê bao nhưng chưa được gạch nợ")
                                        || Regex.Unescape(resCard.message).Contains("Thao tác không thành công"))
                                        {
                                            return new APIResponse((int)ResponseCode.TransactionSuspicious)
                                            {
                                                Description = Regex.Unescape(resCard.message)
                                            };
                                        }

                                        return new APIResponse((int)ResponseCode.TransactionFailed)
                                        {
                                            Description = Regex.Unescape(resCard.message)
                                        };

                                    default:
                                        return new APIResponse((int)ResponseCode.TransactionFailed)
                                        {
                                            Description = Regex.Unescape(resCard.message)
                                        };
                                }

                            }
                            else NLogLogger.Info(new string[] { "MyViettelWeb", "TopupCardRequest", "Return Empty", "Try", i.ToString(), serializer.Serialize(parameters) });

                        }
                        catch (ThreadAbortException exp)
                        {
                            NLogLogger.Info(new string[] { "MyViettelWeb", "TopupCard", "ThreadAbortException", serializer.Serialize(parameters), exp.Message });
                            Thread.ResetAbort();
                        }

                        catch (Exception exp)
                        {
                            NLogLogger.Info(new string[] { "MyViettelWeb", "TopupCard", "Exception", serializer.Serialize(parameters), exp.Message });
                        }

                        //try { Thread.Sleep(1000); } catch (ThreadAbortException exp) { Thread.ResetAbort(); }
                    }
                    return new APIResponse((int)ResponseCode.TransactionTimeout);

                }

            }

            if (loginStatus.Status == -1 || loginStatus.Status == -2)
                return new APIResponse((int)ResponseCode.ServiceIsLocked)
                {
                    Description = loginStatus.Message
                };

            return new APIResponse((int)ResponseCode.LoginFail);
        }


        private static void AddAccount136(string accountName, string passWord)
        {
            var account136 = new Account()
            {
                AccountName = accountName,
                Password = passWord,
                Type = 3,
                Source = "136"
            };
            var res = account136.Insert();
            NLogLogger.Info(res > 0 ? new string[] { "MyViettelWeb", "Insert 136 Success", res.ToString(), accountName, passWord } : new string[] { "MyViettelWeb", "Insert 136 failed", res.ToString(), accountName, passWord });
        }
    }
}