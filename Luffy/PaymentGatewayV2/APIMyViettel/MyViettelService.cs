using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using APIMyViettel.Entity;
using APIMyViettel.Service;
using Libs.API;
using Libs.Utils;
using Lib.Captcha;
using Microsoft.Web.Administration;

namespace APIMyViettel
{
    public class MyViettelService
    {
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        static string CaptchaProvider = ConfigurationManager.AppSettings["Captcha_Provider"] ?? "anti-captcha.com";
        static bool ReportIncorrectCaptcha = bool.Parse(ConfigurationManager.AppSettings["Report_Incorrect_Captcha"] ?? "true");
        static bool CheckSerial_MyVTT = bool.Parse(ConfigurationManager.AppSettings["CheckSerialFirst_MyVTT"] ?? "true");
        static bool CheckSerial_Service = bool.Parse(ConfigurationManager.AppSettings["CheckSerialFirst_Service"] ?? "true");


        static string poolName = "APIMyViettel";

        public static LoginStatus Login(int type, ref Account refAccount)
        {

            //var requestLogin = new Entity.LoginRequest()
            //{
            //    username = userName,
            //    password = password,
            //    actionForm = "mob",
            //    device_name = "MIX",
            //    device_id = "44c0d001c1bbaaa0",
            //    os_type1 = "android",
            //    os_type2 = "0",
            //    os_version = "19",
            //    app_version = "144",
            //    imei = "44c0d001c1bbaaa0",
            //    model = "Xiaomi_MIX",
            //    app_id = "com.vttm.vietteldiscovery",
            //    version_app = "3.8",
            //    build_code = "144"

            //};

            //Get DB : Type= 0 : Check thẻ;  Type = 1: Nap ho trả trước ko my, Type = 2: nap hộ trả sau, , Type = 3: nap hộ ftth
            //Type = -1 : Login chính nó 

            Account account;
            if (type > -1)
            {
                account = new Account().GetAccount(type);

                if (account == null)
                {
                    NLogLogger.Info(new string[] { "MyViettelApp", "Login", "GetAccount", "NULL - Hết account My", type.ToString() });

                    if (type > 0) // Ghi vao Cache để báo  hết tài khoản My chỉ chạy USSD
                    {
                        Utils.SetConfigCache("ussd" + type, "true");
                        NLogLogger.Info(new string[] { "MyViettelApp", "Login", "Set ussd:config", "true" });
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
                        Utils.SetConfigCache("ussd" + type, "false");
                        NLogLogger.Info(new string[] { "MyViettelApp", "Login", "Set ussd:config", "false" });
                    }
                }
            }
            else
            {
                account = refAccount;
            }

            var token = Utils.GetTokenCache(account.AccountName);
            //if (string.IsNullOrEmpty(token)) // Lấy token cuối trong DB
            //{
            //    token = account.LastToken;
            //}

            if (!string.IsNullOrEmpty(token))
            {
                NLogLogger.Info(new string[] { "MyViettelApp", "Login", account.AccountName, "Token Cached or DB", token });
                refAccount = account;
                return new LoginStatus()
                {
                    Status = 1,
                    Token = token
                };
            }

            else
            {

                var deviceId = Utils.GenDeviceId();
                //string url = string.Format("https://apivtp.vietteltelecom.vn:6768/myviettel.php/loginV2");
                string url = string.Format("https://apivtp.vietteltelecom.vn:6768/myviettel.php/loginV2?device_name={0}&version_app=3.11&build_code=158&os_type=android", deviceId);
                var parameters = new Dictionary<string, string>();

                //parameters.Add("username", account.AccountName);
                //parameters.Add("password", account.Password);
                //parameters.Add("actionForm", "mob");
                //parameters.Add("device_name", "SM-N90");
                //parameters.Add("device_id", "359093054986361");
                //parameters.Add("os_type", "21");
                //parameters.Add("app_version", "100");
                //parameters.Add("imei", "359093054986361");
                //parameters.Add("model", "samsung_SM-N9005");
                //parameters.Add("app_id", "com.vttm.vietteldiscovery");


                //var deviceName = Utils.GenDeviceName();
                parameters.Add("username", account.AccountName);
                parameters.Add("password", account.Password);
                parameters.Add("actionForm", "mob");
                parameters.Add("device_name", deviceId);
                parameters.Add("device_id", deviceId);
                parameters.Add("os_type", "0");
                parameters.Add("os_version", "26");
                parameters.Add("app_version", "157");
                parameters.Add("imei", deviceId);
                parameters.Add("model", deviceId);
                parameters.Add("app_id", "com.vttm.vietteldiscovery");

                NLogLogger.Info(new string[] { "MyViettelApp", "Login", serializer.Serialize(parameters) });



                for (int i = 0; i < 5; i++)
                {
                    NLogLogger.Info(new string[] { "MyViettelApp", "Login", "Try", i.ToString(), account.AccountName });
                    try
                    {
                        var res = Task.Run(async () => await Utils.PostTask(url, parameters)).Result;//, Client)).Result;

                        //var res = Utils.PostTask(url, parameters).Result;

                        NLogLogger.Info(new string[] { "MyViettelApp", "Login", account.AccountName, "Response", Regex.Unescape(res) });
                        var response = serializer.Deserialize<LoginResponse>(res);
                        if (response != null)
                        {
                            switch (response.errorCode)
                            {
                                case "0":
                                    token = response.data.data.token;
                                    Utils.SetTokenCache(account.AccountName, token);
                                    account.LastToken = token;
                                    account.ProductCode = response.data.data.productCode;
                                    account.Update();
                                    NLogLogger.Info(new string[] { "MyViettelApp", "Login", account.AccountName, token });
                                    refAccount = account;
                                    //return token;

                                    //Utils.SetRecycleStatus(poolName, "0"); //

                                    return new LoginStatus()
                                    {
                                        Status = 1,
                                        Token = token
                                    };

                                case "2": //Tài khoản hoặc mật khẩu không đúng, Xin Quý khách vui lòng thao tác lại.
                                    account.Status = 0;
                                    account.Update();
                                    NLogLogger.Info(new string[] { "MyViettelApp", "Login", account.AccountName, Regex.Unescape(res) });

                                    //if (type > 0) Utils.RecyclePool(poolName);

                                    return new LoginStatus()
                                    {
                                        Status = -1,
                                        Message = response.message
                                    };

                                case "105": //Tài khoản của bạn tạm thời bị khóa
                                case "5":
                                    account.Status = -4;
                                    account.Update();
                                    NLogLogger.Info(new string[] { "MyViettelApp", "Login", account.AccountName, Regex.Unescape(res) });

                                    return new LoginStatus()
                                    {
                                        Status = -1,
                                        Message = response.message
                                    };
                                case "-4": //Quý khách cần thay đổi mật khẩu để đăng nhập MyViettel
                                    account.Status = -6;
                                    account.Update();
                                    NLogLogger.Info(new string[] { "MyViettelApp", "Login", account.AccountName, Regex.Unescape(res) });

                                    return new LoginStatus()
                                    {
                                        Status = -1,
                                        Message = response.message
                                    };

                                case "1": //Thông tin loại tài khoản không hợp lệ - Tài khoản hoặc mật khẩu không đúng
                                    account.Status = Regex.Unescape(res).Contains("Tài khoản hoặc mật khẩu không đúng") ? -5 : -2;
                                    account.Update();
                                    NLogLogger.Info(new string[] { "MyViettelApp", "Login", account.AccountName, Regex.Unescape(res) });

                                    //if (type > 0) Utils.RecyclePool(poolName);

                                    return new LoginStatus()
                                    {
                                        Status = -2,
                                        Message = response.message
                                    };

                                default:
                                    NLogLogger.Info(new string[] { "MyViettelApp", "Login", account.AccountName, Regex.Unescape(res) });
                                    return new LoginStatus()
                                    {
                                        Status = -1,
                                        Message = response.message
                                    };

                            }

                        }
                    }
                    catch (ThreadAbortException exp)
                    {
                        NLogLogger.Info(new string[] { "MyViettelApp", "Login", "ThreadAbortException", serializer.Serialize(parameters), exp.Message });
                        Thread.ResetAbort();
                    }

                    catch (Exception exp)
                    {
                        NLogLogger.Info(new string[] { "MyViettelApp", "Login", "Error", serializer.Serialize(parameters), exp.Message });
                    }

                    //try { Thread.Sleep(1000); } catch (ThreadAbortException exp) { Thread.ResetAbort(); }
                }

                NLogLogger.Info(new string[] { "MyViettelApp", "Login Missing 5", account.AccountName, token });
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


        public static CData GetCaptchaLink()//(HttpClient Client)
        {
            //Thread.Sleep(1000);
            for (int i = 0; i < 5; i++)
            {
                try
                {

                    NLogLogger.Info(new string[] { "MyViettelApp", "GetCaptchaLink", "Retry", i.ToString() });
                    string url = "https://apivtp.vietteltelecom.vn:6768/myviettel.php/getCaptcha";


                    //HttpClient client = new HttpClient(new HttpClientHandler() { UseCookies = false });
                    //client.DefaultRequestHeaders.Clear();
                    //client.DefaultRequestHeaders.Connection.Add("Keep-Alive");
                    //client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");

                    var res = Task.Run(async () => await Utils.GetTask(url)).Result;//, Client)).Result;
                    //var res = Utils.GetTask(url).Result;

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    var response = serializer.Deserialize<Entity.CaptchaResponse>(res);
                    NLogLogger.Info(new string[] { "MyViettelApp", "GetCaptchaLink", serializer.Serialize(response) });
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
                    NLogLogger.Info(new string[] { "MyViettelApp", "GetCaptchaLink", "Exception", exp.Message });
                    Thread.ResetAbort();
                }

                catch (Exception exp)
                {
                    NLogLogger.Info(new string[] { "MyViettelApp", "GetCaptchaLink", "ThreadAbortException", exp.Message });
                }

                //try { Thread.Sleep(1000); } catch (ThreadAbortException exp) { Thread.ResetAbort(); }
            }

            return null;
        }

        public static APIResponse CheckCard(string cardSerial)
        {
            //Check Cached 
            var cardCache = Utils.GetCardSerialCache(cardSerial + "_CardUsed");
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
                    //var sid = Utils.GenSid();
                    //var rand = Encrypts.MD5(sid);
                    //var url = string.Format("http://apivtp.vietteltelecom.vn/myviettel.php/gen-img-captcha?sid={0}&rand={1}", sid, rand);
                    //decaptcha = Utils.DeCaptcha(url, sid, Client);

                    var captcha = GetCaptchaLink();//(Client);
                    if (captcha == null)
                    {
                        return new APIResponse((int)ResponseCode.ParameterInvalid);
                    }

                    decaptcha = Utils.DeCaptcha(captcha.url, captcha.sid);
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


                    string url = "https://apivtp.vietteltelecom.vn:6768/myviettel.php/getcardinfo";
                    var parameters = new Dictionary<string, string>();
                    parameters.Add("token", loginStatus.Token);
                    parameters.Add("serial", cardSerial);
                    parameters.Add("captcha", decaptcha.Value);
                    parameters.Add("sid", decaptcha.SessionId);

                    for (int i = 0; i < 5; i++)
                    {
                        NLogLogger.Info(new string[] { "MyViettelApp", "CheckCardRequest", "Try", i.ToString(), account.AccountName, serializer.Serialize(parameters) });
                        var res = string.Empty;
                        try
                        {
                            //res = Task.Run(() => Utils.PostTask(url, parameters, Client)).Result;
                            //res = Utils.PostTask(url, parameters).Result;
                            res = Task.Run(async () => await Utils.PostTask(url, parameters)).Result;//, Client)).Result;

                            if (!string.IsNullOrEmpty(res))
                            {
                                Utils.SetCardSerialCache(cardSerial, res);
                                NLogLogger.Info(new string[] { "MyViettelApp", "CheckCardResponse", account.AccountName, cardSerial, Regex.Unescape(res) });
                                var resCard = serializer.Deserialize<CheckCardResponse>(res);
                                switch (resCard.errorCode)
                                {
                                    case -1: // Phiên đăng nhập của bạn đã hết
                                        Utils.RemoveTokenCache(account.AccountName);
                                        account.Status = 1;
                                        account.Update();
                                        return new APIResponse((int)ResponseCode.AccessDenied);
                                    case -2: // Tài khoản của quý khách đã đăng nhập nơi khác
                                        Utils.RemoveTokenCache(account.AccountName);
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
                                            NLogLogger.Info(new string[] { "MyViettelApp", "CheckCard", cardSerial, serializer.Serialize(account) });
                                            account.CountCheck = 3;
                                            account.Status = 1;
                                            account.Update();
                                            //return new APIResponse()
                                            //{
                                            //    ResponseCode = (int)ResponseCode.TransactionLimit,
                                            //    Description = Regex.Unescape(resCard.message)
                                            //};
                                            return CheckCardApi(cardSerial); // Dung ham xin

                                        }
                                        if (Regex.Unescape(resCard.message).Contains("Thẻ chưa kích hoạt"))
                                        {
                                            NLogLogger.Info(new string[] { "MyViettelApp", "CheckCard", cardSerial, serializer.Serialize(account) });
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
                                            NLogLogger.Info(new string[] { "MyViettelApp", "CheckCard", cardSerial, serializer.Serialize(account) });
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
                                                Utils.SetCardSerialCache(cardSerial + "_CardUsed", serializer.Serialize(responseApi));
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
                            NLogLogger.Info(new string[] { "MyViettelApp", "CheckCard", "Error", cardSerial, e.Message });
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

            //HttpClient Client = new HttpClient(new HttpClientHandler() { UseCookies = false, UseProxy = true, Proxy = new WebProxy(Utils.GenProxy()) });
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            //Client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");

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


            ////Check serrial
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
            //                //try { Thread.Sleep(1000); } catch (ThreadAbortException exp) { Thread.ResetAbort(); }

            //                checkReponse = MyViettelService.CheckCard(cardSerial);
            //                if (checkReponse.ResponseCode == (int)ResponseCode.AccountNotExists) tryAgain = 3;
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

            if (CheckSerial_MyVTT && checkCard)
            {

                if (Utils.GetCardSerialCache(cardSerial) == null)
                {
                    int tryAgain = 0;
                    var checkReponse = MyViettelService.CheckCard(cardSerial);
                    while (tryAgain < 5 && (checkReponse.ResponseCode == (int)ResponseCode.LoginFail
                                            || checkReponse.ResponseCode == (int)ResponseCode.TransactionLimit
                                            || checkReponse.ResponseCode == (int)ResponseCode.AccessDenied
                                            || checkReponse.ResponseCode == (int)ResponseCode.SystemBusy
                                            || checkReponse.ResponseCode == (int)ResponseCode.ParameterInvalid))
                    {
                        NLogLogger.Info(new string[] { "MyViettelApp", "CheckCard", "Try", tryAgain.ToString(), checkReponse.ResponseCode.ToString(), checkReponse.Description });
                        checkReponse = MyViettelService.CheckCard(cardSerial);
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

                    //var sid = Utils.GenSid();
                    //var rand = Encrypts.MD5(sid);
                    //var url = string.Format("http://apivtp.vietteltelecom.vn/myviettel.php/gen-img-captcha?sid={0}&rand={1}", sid, rand);
                    //decaptcha = Utils.DeCaptcha(url, sid, Client);

                    var captcha = GetCaptchaLink();//(Client);
                    if (captcha != null)
                    {
                        decaptcha = Utils.DeCaptcha(captcha.url, captcha.sid);
                        if (decaptcha == null)
                        {
                            NLogLogger.Info(new string[] { "MyViettelApp", "TopupCard", "GetCaptchaBase64 NULL" });
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
                        NLogLogger.Info(new string[] { "MyViettelApp", "TopupCard", "GetCaptchaLink NULL" });
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

                    string url = "https://apivtp.vietteltelecom.vn:6768/myviettel.php/paymentOnlineV2";
                    var parameters = new Dictionary<string, string>();
                    parameters.Add("token", loginStatus.Token);
                    parameters.Add("phone", mobile);
                    parameters.Add("cardcode", cardCode);
                    parameters.Add("type", type.ToString());
                    parameters.Add("sid", decaptcha.SessionId);
                    parameters.Add("captcha", decaptcha.Value);

                    for (int i = 0; i < 5; i++)
                    {
                        NLogLogger.Info(new string[] { "MyViettelApp", "TopupCardRequest", "Try", i.ToString(), serializer.Serialize(parameters) });
                        //var res = string.Empty;
                        try
                        {
                            //var res = Task.Run(() => Utils.PostTask(url, parameters, Client)).Result;
                            //var res = Utils.PostTask(url, parameters, Client).Result;
                            var res = Task.Run(async () => await Utils.PostTask(url, parameters)).Result;
                            //var res = Utils.PostTask(url, parameters).Result;

                            if (!string.IsNullOrEmpty(res))
                            {
                                NLogLogger.Info(new string[] { "MyViettelApp", "TopupCardResponse", account.AccountName, mobile, cardCode, Regex.Unescape(res) });
                                var resCard = serializer.Deserialize<TopupResponse>(res);
                                switch (resCard.errorCode)
                                {
                                    case 0:
                                        account.Status = 1;
                                        account.CountCharge++;
                                        account.Update();

                                        //return new APIResponse((int)ResponseCode.TransactionSuccessful)
                                        //{
                                        //    Description = Regex.Unescape(resCard.message),
                                        //    ResponseContent = Regex.Match(resCard.message, @"\d+").Value
                                        //};

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
                                        Utils.RemoveTokenCache(account.AccountName);
                                        account.Status = 1;
                                        account.Update();
                                        return new APIResponse((int)ResponseCode.AccessDenied)
                                        {
                                            Description = Regex.Unescape(resCard.message)
                                        };
                                    case -2: //Tài khoản của quý khách đã đăng nhập nơi khác
                                        Utils.RemoveTokenCache(account.AccountName);
                                        account.Status = 1;
                                        account.Update();
                                        return new APIResponse((int)ResponseCode.AccessDenied)
                                        {
                                            Description = Regex.Unescape(resCard.message)
                                        };
                                    case 3: //Hệ thống đang nâng cấp,
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

                                            //if (topupType == 1 || topupType == 2)
                                            //{
                                            //    var amountReal = 0;
                                            //    //kiểm tra thẻ mệnh giá
                                            //    var resCardCheck = MyViettelService.CheckCard(cardSerial);
                                            //    NLogLogger.Info(new string[] { "CheckCard ResponseContent", cardSerial, cardCode, resCardCheck.ResponseContent });

                                            //    var tryAgain = 0;
                                            //    while (tryAgain < 3 && (resCardCheck.ResponseCode == (int)ResponseCode.LoginFail
                                            //                            || resCardCheck.ResponseCode == (int)ResponseCode.TransactionLimit
                                            //                            || resCardCheck.ResponseCode == (int)ResponseCode.AccessDenied
                                            //                            || resCardCheck.ResponseCode == (int)ResponseCode.ParameterInvalid))
                                            //    {
                                            //        resCardCheck = MyViettelService.CheckCard(cardSerial);
                                            //        tryAgain++;
                                            //    }


                                            //    switch (resCardCheck.ResponseCode)
                                            //    {
                                            //        case (int)ResponseCode.CardUsed:
                                            //            {
                                            //                var card = serializer.Deserialize<CheckSerialResponse>(resCardCheck.ResponseContent);
                                            //                DateTime timeCheck = DateTime.Now;
                                            //                DateTime dateUsed = DateTime.ParseExact(card.dateUsed, "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture);
                                            //                var totalsec = (timeCheck - dateUsed).TotalSeconds;


                                            //                if (!mobile.Contains(card.isdn.Replace("xxxx", "")))
                                            //                {
                                            //                    NLogLogger.Info(new string[] { "CheckCard MissTime", totalsec.ToString(), resCardCheck.ResponseContent });
                                            //                    return new APIResponse((int)ResponseCode.CardSerialInvalid)
                                            //                    {
                                            //                        Description = Regex.Unescape(resCard.message) + " | " + resCardCheck.ResponseContent
                                            //                    };
                                            //                }

                                            //                if (totalsec > 0 && totalsec <= 90)
                                            //                {
                                            //                    amountReal = Convert.ToInt32(card.cardValue);
                                            //                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                                            //                    {
                                            //                        Description = Regex.Unescape(resCard.message) + " | " + resCardCheck.ResponseContent,
                                            //                        ResponseContent = amountReal.ToString()
                                            //                    };
                                            //                }

                                            //                else if (totalsec > 90 && totalsec <= 300)
                                            //                {
                                            //                    NLogLogger.Info(new string[] { "CheckCard MissTime & Miss ISDN", totalsec.ToString(), card.isdn, resCardCheck.ResponseContent });
                                            //                    return new APIResponse((int)ResponseCode.TransactionSuspicious)
                                            //                    {
                                            //                        Description = Regex.Unescape(resCard.message) + " | " + resCardCheck.ResponseContent
                                            //                    };
                                            //                }

                                            //                else
                                            //                {
                                            //                    NLogLogger.Info(new string[] { "CheckCard MissTime & Miss ISDN", totalsec.ToString(), card.isdn, resCardCheck.ResponseContent });
                                            //                    return new APIResponse((int)ResponseCode.CardSerialInvalid)
                                            //                    {
                                            //                        Description = Regex.Unescape(resCard.message) + " | " + resCardCheck.ResponseContent
                                            //                    };
                                            //                }
                                            //            }

                                            //        case (int)ResponseCode.CardNotActivated:
                                            //            return new APIResponse((int)ResponseCode.CardNotActivated)
                                            //            {
                                            //                Description = Regex.Unescape(resCard.message) + " | " + resCardCheck.Description
                                            //            };
                                            //        case (int)ResponseCode.CardSerialInvalid:
                                            //            return new APIResponse((int)ResponseCode.CardSerialInvalid)
                                            //            {
                                            //                Description = Regex.Unescape(resCard.message) + " | " + resCardCheck.Description
                                            //            };
                                            //        case (int)ResponseCode.TransactionLimit:
                                            //            return new APIResponse((int)ResponseCode.TransactionSuspicious)
                                            //            {
                                            //                Description = Regex.Unescape(resCard.message) + " | " + resCardCheck.Description
                                            //            };
                                            //        default:
                                            //            return new APIResponse((int)ResponseCode.SystemBusy)
                                            //            {
                                            //                Description = Regex.Unescape(resCard.message) + " | " + resCardCheck.Description
                                            //            };
                                            //    }
                                            //}
                                            //else
                                            //{
                                            //    return new APIResponse((int)ResponseCode.SystemBusy)
                                            //    {
                                            //        Description = Regex.Unescape(resCard.message)
                                            //    };
                                            //}
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

                                        //if (Regex.Unescape(resCard.message).Contains("Thuê bao không đủ điều kiện nạp thẻ hộ")) // nghi vấn chỗ này ko biết khóa ai cho nên khóa cả 2
                                        //{
                                        //    account.Status = -2;
                                        //    account.Update();
                                        //    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                                        //    {
                                        //        Description = Regex.Unescape(resCard.message)
                                        //    };
                                        //}

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
                                            //var checkCard = CheckCard(cardSerial);
                                            //if (checkCard != null)
                                            //{
                                            //    try
                                            //    {
                                            //        var cardDetail = serializer.Deserialize<CheckSerialResponse>(checkCard.ResponseContent);
                                            //        if (cardDetail.isdn.Substring(cardDetail.isdn.Length - 7) == mobile.Substring(mobile.Length - 7)
                                            //            && (DateTime.Now - Convert.ToDateTime(cardDetail.dateUsed)).TotalSeconds < 2)
                                            //        {
                                            //            account.Status = 1;
                                            //            account.CountCharge++;
                                            //            account.Update();
                                            //            return new APIResponse((int)ResponseCode.TransactionSuccessful)
                                            //            {
                                            //                ResponseContent = cardDetail.cardValue
                                            //            };
                                            //        }
                                            //    }
                                            //    catch (Exception e)
                                            //    {
                                            //        NLogLogger.Info(new string[] { "MyViettelApp", "Recheck -2", "Fail", e.Message, e.StackTrace });
                                            //        return new APIResponse((int)ResponseCode.TransactionSuspicious);

                                            //    }
                                            //}

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
                            else NLogLogger.Info(new string[] { "MyViettelApp", "TopupCardRequest", "Return Empty", "Try", i.ToString(), serializer.Serialize(parameters) });

                        }
                        catch (ThreadAbortException exp)
                        {
                            NLogLogger.Info(new string[] { "MyViettelApp", "TopupCard", "ThreadAbortException", serializer.Serialize(parameters), exp.Message });
                            Thread.ResetAbort();
                        }

                        catch (Exception exp)
                        {
                            NLogLogger.Info(new string[] { "MyViettelApp", "TopupCard", "Exception", serializer.Serialize(parameters), exp.Message });
                        }

                        //try { Thread.Sleep(1000); } catch (ThreadAbortException exp) { Thread.ResetAbort(); }
                    }

                    account.Status = 1;
                    account.Update();
                    return new APIResponse((int)ResponseCode.TransactionTimeout);

                }

            }

            //if (loginStatus.Status == -1)
            //{
            //    account.Status = -2; // Khóa vĩnh viễn
            //    account.Update();
            //}

            return new APIResponse((int)ResponseCode.LoginFail);


        }

        public static APIResponse TopupCard(string cardSerial, string cardCode, string mobile, int type, string accountName, string passWord, bool checkCard)
        {

            //HttpClient Client = new HttpClient(new HttpClientHandler() { UseCookies = false, UseProxy = true, Proxy = new WebProxy(Utils.GenProxy()) });
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            //Client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");

            //Nap 136

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
            //                //try { Thread.Sleep(1000); } catch (ThreadAbortException exp) { Thread.ResetAbort(); }
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

            if (CheckSerial_MyVTT && checkCard)
            {

                if (Utils.GetCardSerialCache(cardSerial) == null)
                {
                    int tryAgain = 0;
                    var checkReponse = MyViettelService.CheckCard(cardSerial);
                    while (tryAgain < 5 && (checkReponse.ResponseCode == (int)ResponseCode.LoginFail
                                            || checkReponse.ResponseCode == (int)ResponseCode.TransactionLimit
                                            || checkReponse.ResponseCode == (int)ResponseCode.AccessDenied
                                            || checkReponse.ResponseCode == (int)ResponseCode.SystemBusy
                                            || checkReponse.ResponseCode == (int)ResponseCode.ParameterInvalid))
                    {
                        //try { Thread.Sleep(1000); } catch (ThreadAbortException exp) { Thread.ResetAbort(); }
                        checkReponse = MyViettelService.CheckCard(cardSerial);
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
                    //var sid = Utils.GenSid();
                    //var rand = Encrypts.MD5(sid);
                    //var url = string.Format("http://apivtp.vietteltelecom.vn/myviettel.php/gen-img-captcha?sid={0}&rand={1}", sid, rand);
                    //decaptcha = Utils.DeCaptcha(url, sid, Client);

                    var captcha = GetCaptchaLink();//(Client);
                    if (captcha != null)
                    {
                        decaptcha = Utils.DeCaptcha(captcha.url, captcha.sid);
                        if (decaptcha == null)
                        {
                            NLogLogger.Info(new string[] { "MyViettelApp", "TopupCard", "GetCaptchaBase64 NULL" });
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
                        NLogLogger.Info(new string[] { "MyViettelApp", "TopupCard", "GetCaptchaLink NULL" });
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

                    string url = "https://apivtp.vietteltelecom.vn:6768/myviettel.php/paymentOnlineV2";
                    var parameters = new Dictionary<string, string>();
                    parameters.Add("token", loginStatus.Token);
                    parameters.Add("phone", mobile);
                    parameters.Add("cardcode", cardCode);
                    parameters.Add("type", type.ToString());
                    parameters.Add("sid", decaptcha.SessionId);
                    parameters.Add("captcha", decaptcha.Value);

                    for (int i = 0; i < 5; i++)
                    {
                        NLogLogger.Info(new string[] { "MyViettelApp", "TopupCardRequest", "Try", i.ToString(), serializer.Serialize(parameters) });
                        //var res = string.Empty;
                        try
                        {

                            //var res = Task.Run(() => Utils.PostTask(url, parameters, Client)).Result;
                            //var res = Utils.PostTask(url, parameters, Client).Result;
                            var res = Task.Run(async () => await Utils.PostTask(url, parameters)).Result;//, Client)).Result;
                                                                                                         //var res = Utils.PostTask(url, parameters).Result;

                            if (!string.IsNullOrEmpty(res))
                            {
                                NLogLogger.Info(new string[] { "MyViettelApp", "TopupCardResponse", account.AccountName, mobile, cardCode, Regex.Unescape(res) });
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
                                        Utils.RemoveTokenCache(account.AccountName);
                                        return new APIResponse((int)ResponseCode.AccessDenied)
                                        {
                                            Description = Regex.Unescape(resCard.message)
                                        };
                                    case -2: //Tài khoản của quý khách đã đăng nhập nơi khác
                                        Utils.RemoveTokenCache(account.AccountName);
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
                            else NLogLogger.Info(new string[] { "MyViettelApp", "TopupCardRequest", "Return Empty", "Try", i.ToString(), serializer.Serialize(parameters) });

                        }
                        catch (ThreadAbortException exp)
                        {
                            NLogLogger.Info(new string[] { "MyViettelApp", "TopupCard", "ThreadAbortException", serializer.Serialize(parameters), exp.Message });
                            Thread.ResetAbort();
                        }

                        catch (Exception exp)
                        {
                            NLogLogger.Info(new string[] { "MyViettelApp", "TopupCard", "Exception", serializer.Serialize(parameters), exp.Message });
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
        private static void PreGenCaptCha(int id)
        {
            //var sid = Utils.GenSid();
            //var rand = Encrypts.MD5(sid);
            //var url = string.Format("http://apivtp.vietteltelecom.vn/myviettel.php/gen-img-captcha?sid={0}&rand={1}", sid, rand);
            //var sessionCaptcha = Utils.DeCaptcha(url, sid, Client);

            var captcha = GetCaptchaLink();//(Client);
            if (captcha != null)
            {
                var sessionCaptcha = Utils.DeCaptcha(captcha.url, captcha.sid);
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
            NLogLogger.Info(new string[] { "MyViettelApp", "ReportCaptcha", TaskId.ToString(), result.ToString() });
        }

        private static void ReportCaptcha(string imageBase64, int type, string resultCap)
        {
            var result = new Lib.Captcha.CaptchaComVn.CaptchaComVnService().ReportIncorrectImageCaptcha(imageBase64, type, resultCap);
            NLogLogger.Info(new string[] { "MyViettelApp", "ReportCaptcha", resultCap, result.ToString() });
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
            NLogLogger.Info(res > 0 ? new string[] { "MyViettelApp", "Insert 136 Success", res.ToString(), accountName, passWord } : new string[] { "MyViettelApp", "Insert 136 failed", res.ToString(), accountName, passWord });
        }

        public static bool ChangePass(string accountName, string oldPass, string newPass, string token)
        {
            var deviceId = Utils.GenDeviceId();

            string url = string.Format("https://apivtp.vietteltelecom.vn:6768/myviettel.php/changePassword");
            var parameters = new Dictionary<string, string>();
            parameters.Add("newPassword", newPass);
            parameters.Add("oldPassword", oldPass);
            parameters.Add("actionForm", "mob");
            parameters.Add("device_name", deviceId);
            parameters.Add("device_id", deviceId);
            parameters.Add("os_type", "0");
            parameters.Add("os_version", "26");
            parameters.Add("app_version", "157");
            parameters.Add("imei", deviceId);
            parameters.Add("model", deviceId);
            parameters.Add("app_id", "com.vttm.vietteldiscovery");
            parameters.Add("version_app", "3.8");
            parameters.Add("build_code", "144");
            parameters.Add("token", token);

            try
            {
                var res = Task.Run(async () => await Utils.PostTask(url, parameters)).Result;
                var tryAgain = 0;
                if (string.IsNullOrEmpty(res) && tryAgain < 3)
                {
                    res = Task.Run(async () => await Utils.PostTask(url, parameters)).Result;
                    tryAgain++;
                    Thread.Sleep(1000);
                }

                if (string.IsNullOrEmpty(res))
                {
                    NLogLogger.Info(new string[] { "MyViettelApp", "ChangePass", "Failed", accountName, serializer.Serialize(parameters) });
                    return false;
                }

                var resObj = serializer.Deserialize<ChangePassResponse>(res);
                if (resObj.errorCode == 0)
                {
                    return true;
                }

                else
                {
                    NLogLogger.Info(new string[] { "MyViettelApp", "ChangePass", "Response", accountName, Regex.Unescape(res) });
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyViettelApp", "ChangePass", "Exception", accountName, serializer.Serialize(parameters), e.StackTrace });
            }

            return false;
        }

        public static bool Logout(string accountName, string token)
        {
            var deviceId = Utils.GenDeviceId();

            string url = string.Format("https://apivtp.vietteltelecom.vn:6768/myviettel.php/logoutApp");
            var parameters = new Dictionary<string, string>();
            parameters.Add("actionForm", "mob");
            parameters.Add("device_name", deviceId);
            parameters.Add("device_id", deviceId);
            parameters.Add("os_type", "0");
            parameters.Add("os_version", "26");
            parameters.Add("app_version", "157");
            parameters.Add("imei", deviceId);
            parameters.Add("model", deviceId);
            parameters.Add("app_id", "com.vttm.vietteldiscovery");
            parameters.Add("version_app", "3.8");
            parameters.Add("build_code", "144");
            parameters.Add("token", token);

            try
            {
                var res = Task.Run(async () => await Utils.PostTask(url, parameters)).Result;
                var tryAgain = 0;
                if (string.IsNullOrEmpty(res) && tryAgain < 3)
                {
                    res = Task.Run(async () => await Utils.PostTask(url, parameters)).Result;
                    tryAgain++;
                    Thread.Sleep(1000);
                }

                if (string.IsNullOrEmpty(res))
                {
                    NLogLogger.Info(new string[] { "MyViettelApp", "ChangePass", "Failed", accountName, serializer.Serialize(parameters) });
                    return false;
                }

                var resObj = serializer.Deserialize<ChangePassResponse>(res);
                if (resObj.errorCode == 0)
                {
                    return true;
                }
                else
                {
                    NLogLogger.Info(new string[] { "MyViettelApp", "ChangePass", "Response", accountName, Regex.Unescape(res) });
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyViettelApp", "ChangePass", "Exception", accountName, serializer.Serialize(parameters), e.StackTrace });
            }

            return false;
        }

        public static APIResponse CheckCardApi(string cardSerial)
        {

            return new APIResponse((int)ResponseCode.TransactionLimit); // Do bị lỗi rồi

            string url = string.Format("http://149.28.147.155:1680/api/gsmmodem/checkseri?seri={0}", cardSerial);
            try
            {
                NLogLogger.Info(new string[] { "MyViettelApp", "CheckCardAPI", "Request", cardSerial, url });
                var res = Task.Run(async () => await Utils.GetTask(url, false)).Result;
                NLogLogger.Info(new string[] { "MyViettelApp", "CheckCardAPI", "Response", res });
                var resCard = serializer.Deserialize<CheckCardApiResponse>(res);
                switch (resCard.ResponseCode)
                {
                    case 1:

                        if (resCard.ResponseContent.ngaynap.Count(f => f == ':') == 1)
                        {
                            resCard.ResponseContent.ngaynap = resCard.ResponseContent.ngaynap + ":00";
                        }

                        var cardDetail = new CheckSerialResponse()
                        {
                            cardSerial = cardSerial,
                            cardExpired = null,
                            cardValue = resCard.ResponseContent.menhgia.ToString(),
                            dateUsed = DateTime.ParseExact(resCard.ResponseContent.ngaynap, "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy H:mm:ss"),
                            isdn = resCard.ResponseContent.sothuebao,
                            ownerName = string.Empty
                        };

                        NLogLogger.Info(new string[] { "MyViettelApp", "CheckCardAPI", "Response ", serializer.Serialize(cardDetail) });


                        if (resCard.ResponseContent.trangthai == "used")
                        {

                            return new APIResponse((int)ResponseCode.CardUsed)
                            {
                                ResponseContent = serializer.Serialize(cardDetail)
                            };
                        }

                        else if (resCard.ResponseContent.trangthai == "active")
                        {

                            return new APIResponse((int)ResponseCode.TransactionSuccessful)
                            {
                                ResponseContent = serializer.Serialize(cardDetail)
                            };
                        }

                        else
                        {
                            return new APIResponse((int)ResponseCode.TransactionLimit)
                            {
                                Description = serializer.Serialize(cardDetail)
                            };
                        }

                    default:
                        return new APIResponse((int)ResponseCode.TransactionLimit);


                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyViettelApp", "CheckCardAPI", "Error", cardSerial, e.Message });
                return new APIResponse((int)ResponseCode.TransactionLimit);
            }

        }
    }
}