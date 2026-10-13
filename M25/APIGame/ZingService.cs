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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIGame
{
    public class ZingService
    {
        static readonly string baseUrl = "https://pay.gosu.vn";
        static JavaScriptSerializer serializer = new JavaScriptSerializer();

        public static APIResponse TopupCard(string cardSerial, string cardCode, string cardType, string accountName, string passWord, string tranId, string tranId3rd, int gameType, string extData)
        {

            if (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(passWord))
            {
                return new APIResponse((int)ResponseCode.ServiceIsLocked)
                {
                    Description = "Mobile or AccountName or Password is Empty"
                };
            }

            var tranHis = new TransHistory();
            var getTopup = new GameCookie();

            if (gameType == 7 || gameType == 8) // Mobile ko cần qua các bước Cookie
            {
                getTopup = new GameCookie() { IsTopup = true };
            }
            else
            {
                var sid = UtilsZing.GenSid(accountName, gameType);
                var decaptcha = new Captcha().GetCaptcha(sid.SessionType, sid.Sid);
                if (decaptcha == null)
                {
                    getTopup = GetTopup(accountName, passWord, ref tranHis, gameType);
                    if (getTopup.IsTopup)
                    {
                        var sessionCaptcha = new Captcha();
                        sessionCaptcha.SessionId = sid.Sid;
                        sessionCaptcha.Type = sid.SessionType;
                        sessionCaptcha.Value = string.Empty;
                        sessionCaptcha.TaskId = 0;
                        sessionCaptcha.ImgBase64 = string.Empty;
                        sessionCaptcha.Add();

                        getTopup.HtmlContent = string.Empty;
                        getTopup.SessionId = sid.Sid;
                        UtilsZing.SetCookieCache(sid.Sid, getTopup);
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
                    getTopup = UtilsZing.GetCookieCache(decaptcha.SessionId);
                    if (getTopup == null) // Login lại quá trình
                    {
                        getTopup = GetTopup(accountName, passWord, ref tranHis, gameType);
                        if (getTopup.IsTopup)
                        {
                            var sessionCaptcha = new Captcha();
                            sessionCaptcha.SessionId = sid.Sid;
                            sessionCaptcha.Type = sid.SessionType;
                            sessionCaptcha.Value = string.Empty;
                            sessionCaptcha.TaskId = 0;
                            sessionCaptcha.ImgBase64 = string.Empty;
                            sessionCaptcha.Add();

                            getTopup.HtmlContent = string.Empty;
                            getTopup.SessionId = sid.Sid;
                            UtilsZing.SetCookieCache(sid.Sid, getTopup);
                        }
                        else
                        {
                            return new APIResponse((int)ResponseCode.ServiceIsLocked)
                            {
                                Description = getTopup.HtmlContent
                            };
                        }
                    }
                }
            }



            var res = "Processing";

            if (getTopup != null)
            {
                if (gameType == 7 || gameType == 8) // Mobile gọi hàm khác
                {
                    res = TopupM(cardSerial, cardCode, accountName, passWord, tranId, tranId3rd, gameType, extData);
                }
                else
                {
                    res = Topup(cardSerial, cardCode, accountName, cardType, getTopup, tranHis, tranId, tranId3rd, gameType);
                }


                NLogLogger.Info(new string[] { "ZingService", tranId, tranId3rd, "TopupCard", "Response", accountName, cardSerial, cardCode, res });

                if (res.Contains("Success"))
                {
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        Description = res,
                        ResponseContent = Regex.Match(res, @"(\d+.)+").Value.Trim().Replace(",", "")
                    };
                }

                if (res.Contains("Sai mật mã thẻ"))
                {
                    return new APIResponse((int)ResponseCode.CardCodeInvalid)
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

                if (res.Contains("Thẻ bị quá hạn"))
                {
                    return new APIResponse((int)ResponseCode.CardHasExpired)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Hệ thống gặp vấn đề"))
                {
                    return new APIResponse((int)ResponseCode.SystemBusy)
                    {
                        Description = res
                    };
                }

                if (res.Contains("POST_TIMEOUT"))
                {
                    return new APIResponse((int)ResponseCode.TransactionTimeout)
                    {
                        Description = res
                    };
                }
                if (res.Contains("POST_NULL")
                    || res.Contains("POST_EXEPTION"))
                {
                    return new APIResponse((int)ResponseCode.ParameterInvalid);
                }

                if (res.Contains("Giao dịch đang xử lý"))
                {
                    return new APIResponse((int)ResponseCode.TransactionSuspicious)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Xin vui lòng đăng nhập để thực hiện chức năng này")
                    || res.Contains("Tài khoản không tồn tại"))
                {
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Dữ liệu thanh toán không hợp lệ")
                    || res.Contains("Thông tin thẻ không đúng"))
                {
                    return new APIResponse((int)ResponseCode.CardFormatInvalid)
                    {
                        Description = res
                    };
                }


            }

            return new APIResponse((int)ResponseCode.TransactionFailed)
            {
                Description = res
            };
        }

        public static GameCookie GetTopup(string accountName, string passWord, ref TransHistory transHis, int gameType)
        {
            var u1 = string.Empty;
            var fp = string.Empty;
            var gameName = string.Empty;

            switch (gameType)
            {
                case 1:
                    u1 = "https://pay.zing.vn/product/volamfree";
                    fp = "https://pay.zing.vn/wplogin/pc/volamfree";
                    gameName = "Võ Lâm Truyền Kỳ Miễn Phí";
                    break;
                case 2:
                    u1 = "https://pay.zing.vn/product/volamctc";
                    fp = "https://pay.zing.vn/wplogin/pc/volamctc";
                    gameName = "VLTK - Công Thành Chiến";
                    break;
                case 3:
                    u1 = "https://pay.zing.vn/product/volamtruyenky";
                    fp = "https://pay.zing.vn/wplogin/pc/volamtruyenky";
                    gameName = "Võ Lâm Truyền Kỳ 1";
                    break;
                case 4:
                    u1 = "https://pay.zing.vn/product/kiemthe";
                    fp = "https://pay.zing.vn/wplogin/pc/kiemthe";
                    gameName = "Kiếm Thế";
                    break;
                case 5:
                    u1 = "https://pay.zing.vn/product/tanthienlong3d";
                    fp = "https://pay.zing.vn/wplogin/pc/tanthienlong3d";
                    gameName = "Tân Thiên Long 3D";
                    break;
                case 6:
                    u1 = "https://pay.zing.vn/product/volam2";
                    fp = "https://pay.zing.vn/wplogin/pc/volam2";
                    gameName = "Võ Lâm Truyền Kỳ 2";
                    break;

            }

            NLogLogger.Info(new string[] { "ZingService", "GetTopup", "Request", accountName, passWord, gameName });

            var parameters = new Dictionary<string, string>();
            parameters.Add("apikey", "6e731b7ae5ca4146a880d7b956eb3649");
            parameters.Add("pid", "12");
            parameters.Add("longtime", "0");
            parameters.Add("u", accountName.ToLower());
            parameters.Add("p", passWord);
            //parameters.Add("u1", "https://new.pay.zing.vn/product/volamfree?null");
            //parameters.Add("fp", "https://new.pay.zing.vn/login/?redirect=/product/volamfree?null");
            parameters.Add("u1", u1);
            parameters.Add("fp", fp);

            var loginResponse = Task.Run(() => UtilsZing.PostTask("https://sso3.zing.vn/alogin", parameters, new CookieContainer())).Result;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(loginResponse.HtmlContent);

            //var detectLogin = loginResponse.HtmlContent.Contains("Tên đăng nhập hoặc mật khẩu không đúng!");
            //if (detectLogin)
            //{
            //    NLogLogger.Info(new string[] { "ZingService", "Topup", "Error Login", serializer.Serialize(parameters), gameName });
            //    return new GameCookie()
            //    {
            //        IsTopup = false,
            //        HtmlContent = HttpUtility.HtmlDecode("Tên đăng nhập hoặc mật khẩu không đúng!")
            //    };
            //}

            //var detectUpdate = loginResponse.HtmlContent.Contains("Hệ thống đang được nâng cấp");
            //if (detectUpdate)
            //{
            //    NLogLogger.Info(new string[] { "ZingService", "Topup", "Login Busy", serializer.Serialize(parameters), gameName });
            //    return new GameCookie()
            //    {
            //        IsTopup = false,
            //        HtmlContent = HttpUtility.HtmlDecode("Hệ thống đang được nâng cấp")
            //    };
            //}


            var detectLogin = loginResponse.HtmlContent.Contains("Xin chào " + accountName);
            if (detectLogin)
            {
                loginResponse.IsTopup = true;
            }
            else
            {
                NLogLogger.Info(new string[] { "ZingService", "Topup", "Error Login", serializer.Serialize(parameters), gameName });
                return new GameCookie()
                {
                    IsTopup = false,
                    HtmlContent = HttpUtility.HtmlDecode("Tên đăng nhập hoặc mật khẩu không đúng! OR Hệ thống đang được nâng cấp")
                };
            }


            //var getTopupCard = Task.Run(() => UtilsZing.GetTask("https://pay.zing.vn/payment/volamctc", loginResponse.CookieContainer)).Result;
            //var script = doc.DocumentNode.Descendants().Where(n => n.Name == "script" && n.XPath == "/html[1]/body[1]/div[3]/script[1]").First().InnerText;
            //var engine = new Jurassic.ScriptEngine();
            //var result = engine.Evaluate("(function() { " + script + " return transHistory; })()");
            //var json = JSONObject.Stringify(engine, result);

            //if (!string.IsNullOrEmpty(json))
            //{
            //    transHis = serializer.Deserialize<TransHistory>(json);
            //    parameters.Clear();
            //    //parameters.Add("sID", transHis.sID.ToString());
            //    //parameters.Add("serverID", transHis.serverID.ToString());
            //    //parameters.Add("pmOptID", transHis.pmOptID.ToString());
            //    //parameters.Add("pmcID", transHis.pmcID.ToString());
            //    //parameters.Add("roleID", "");
            //    //parameters.Add("roleName", transHis.roleName);
            //    //parameters.Add("accountName", accountName.ToLower());

            //    parameters.Add("sID", "3659");
            //    parameters.Add("serverID", "all");
            //    parameters.Add("pmOptID", "41");
            //    parameters.Add("pmcID", "1");
            //    parameters.Add("roleID", "");
            //    parameters.Add("roleName", "");
            //    parameters.Add("accountName", accountName.ToLower());

            //    //loginResponse = Task.Run(() => UtilsZing.GetTask("https://new.pay.zing.vn/ajax/check-account?productCode=volamfree&accountName=tuyen0965197890t&accountID=575827906&accountGuid=-1&serverID=all&roleID=", loginResponse.CookieContainer)).Result;
            //    loginResponse = Task.Run(() => UtilsZing.PostTask("https://new.pay.zing.vn/payment/volamfree", parameters, loginResponse.CookieContainer)).Result;
            //}

            return loginResponse;

        }


        public static GameCookie GetLoginM(string accountName, string passWord, int gameType)
        {

            var loginUrl = string.Empty;
            var gameName = string.Empty;
            var appName = string.Empty;
            var mapId = string.Empty;
            var appId = string.Empty;
            var clientKey = string.Empty;

            switch (gameType)
            {
                case 7:
                    loginUrl = "https://login.pp.m.zing.vn/login/zing";
                    gameName = "Võ Lâm Truyền Kỳ Mobile";
                    appName = "vltkm";
                    mapId = "jxm";
                    appId = "jxm";
                    clientKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjIjoyMDIyMSwiYSI6MTAyMjEsInMiOjF9.9XincSYUb_NmrF_vJEqgQzLvV_yItGoFGHjVZjB_KlQ";
                    break;
                case 8:
                    loginUrl = "https://login.pp.m.zing.vn/login/zing";
                    gameName = "Danh Tướng 3Q";
                    appName = "3q";
                    mapId = "dt3q-billing";
                    appId = "dt3q";
                    clientKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjIjoyMDM4MSwiYSI6MTAzODEsInMiOjF9.pM-o1MVYNaTTmO1OiPzWZx0mKMFIMLWCaHiqaD0NBDA";
                    break;

            }
            NLogLogger.Info(new string[] { "ZingService", "GetTopup", "Request", accountName, passWord, gameName });

            var parameters = new Dictionary<string, string>();
            parameters.Add("u", accountName);
            parameters.Add("p", passWord);
            parameters.Add("mapID", mapId);
            parameters.Add("appID", appId);

            var loginResponse = Task.Run(() => UtilsZing.PostTask(loginUrl, parameters, new CookieContainer())).Result;

            NLogLogger.Info(new string[] { "ZingService", "Topup", "LoginM Result", loginResponse.HtmlContent });

            if (!string.IsNullOrEmpty(loginResponse.HtmlContent))
            {
                var loginObj = serializer.Deserialize<LoginM>(loginResponse.HtmlContent);

                if (loginObj.returnCode == 0)
                {

                    loginUrl = "https://billing.mto.zing.vn/fe/api/auth/login";

                    //Uri bUri = new Uri(loginObj.data.r.TrimEnd('&'));
                    //var query = bUri.Query.Replace("?", "");
                    var query = loginObj.data.r.TrimEnd('&').Split('?')[1];
                    var queryValues = query.Split('&').Select(q => q.Split('=')).ToDictionary(k => k[0], v => v[1]);

                    parameters.Clear();
                    parameters.Add("clientKey", clientKey);
                    parameters.Add("lang", "VI");
                    parameters.Add("success", queryValues["success"]);
                    parameters.Add("gameID", queryValues["gameID"]);
                    parameters.Add("session", queryValues["session"]);
                    parameters.Add("userID", queryValues["userID"]);
                    parameters.Add("userName", queryValues["userName"]);
                    parameters.Add("loginType", queryValues["loginType"]);
                    parameters.Add("ts", queryValues["ts"]);
                    parameters.Add("sig", queryValues["sig"]);
                    parameters.Add("appID", queryValues["appID"]);
                    //parameters.Add("muid", queryValues["muid"]);
                    //parameters.Add("maccesstoken", queryValues["maccesstoken"]);

                    var loginBillingResponse = Task.Run(() => UtilsZing.PostTask(loginUrl, parameters, new CookieContainer())).Result;
                    NLogLogger.Info(new string[] { "ZingService", "Topup", "LoginM Billing Result", loginBillingResponse.HtmlContent });

                    if (!string.IsNullOrEmpty(loginBillingResponse.HtmlContent))
                    {
                        var loginBillingObj = serializer.Deserialize<LoginMBilling>(loginBillingResponse.HtmlContent);
                        if (loginBillingObj.returnCode == 1)
                        {
                            loginResponse.IsTopup = true;
                            loginResponse.RequestVerificationToken = loginBillingObj.data.jtoken;
                            loginResponse.loginType = loginBillingObj.data.loginType;
                            loginResponse.userID = loginBillingObj.data.userID;

                            //Set to Redis login Ok
                            UtilsZing.SetTokenCache(appName, accountName, serializer.Serialize(loginResponse));

                        }
                        else
                        {
                            return new GameCookie()
                            {
                                IsTopup = false,
                                HtmlContent = loginBillingObj.returnMessage
                            };
                        }
                    }
                }
                else
                {
                    NLogLogger.Info(new string[] { "ZingService", "Topup", "Error Login", serializer.Serialize(parameters), gameName });
                    return new GameCookie()
                    {
                        IsTopup = false,
                        HtmlContent = loginObj.message
                    };
                }

            }
            return loginResponse;

        }

        public static List<Server> GetServerM(string accountName, string passWord, int gameType)
        {
            var serverUrl = string.Empty;
            var gameName = string.Empty;
            var appName = string.Empty;

            switch (gameType)
            {
                case 7:
                    gameName = "Võ Lâm Truyền Kỳ Mobile";
                    appName = "vltkm";
                    break;
                case 8:
                    gameName = "Danh Tướng 3Q";
                    appName = "3q";
                    break;

            }


            var serverListJson = UtilsZing.GetTokenCache(appName, "serverlist");
            if (!string.IsNullOrEmpty(serverListJson))
            {
                return serializer.Deserialize<List<Server>>(serverListJson);
            }


            var serverList = new List<Server>();

            var gameCookieJson = UtilsZing.GetTokenCache(appName, accountName);
            var gameCookie = new GameCookie();
            if (!string.IsNullOrEmpty(gameCookieJson))
            {
                gameCookie = serializer.Deserialize<GameCookie>(gameCookieJson);
            }
            else
            {
                gameCookie = GetLoginM(accountName, passWord, gameType);
            }

            var login = new LoginMBilling();
            var tryAgain = 0;
            serverUrl = "https://billing.mto.zing.vn/fe/api/store/getServers";


            while ((login.returnCode == 0 || login.returnCode == -4) && tryAgain < 2)
            {
                if (login.returnCode == -4) gameCookie = GetLoginM(accountName, passWord, gameType);

                var parameters = new Dictionary<string, string>();
                parameters.Add("userID", gameCookie.userID);
                parameters.Add("loginType", gameCookie.loginType);
                parameters.Add("jtoken", gameCookie.RequestVerificationToken);
                parameters.Add("lang", "VI");

                var serverListResult = Task.Run(() => UtilsZing.PostTask(serverUrl, parameters, new CookieContainer())).Result;

                NLogLogger.Info(new string[] { "ZingService", "Topup", "Get Server Result", serverListResult.HtmlContent });


                if (!string.IsNullOrEmpty(serverListResult.HtmlContent))
                {
                    login = serializer.Deserialize<LoginMBilling>(serverListResult.HtmlContent);
                    if (login.returnCode == 1)
                    {
                        tryAgain = 2;
                        dynamic jsonObj = JsonConvert.DeserializeObject(serverListResult.HtmlContent);
                        try
                        {
                            foreach (var s in jsonObj.data)
                            {
                                var server = serializer.Deserialize<Server>(Convert.ToString(s.First));
                                serverList.Add(server);
                            }
                        }
                        catch { }
                    }
                }
                tryAgain++;
            }


            if (serverList.Count > 0)
            {
                serverListJson = serializer.Serialize(serverList);
                UtilsZing.SetTokenCache(appName, "serverlist", serverListJson);
            }

            return serverList;

        }

        public static List<Role> GetRoleM(string accountName, string passWord, int gameType, string serverID)
        {

            var roleUrl = string.Empty;
            var gameName = string.Empty;
            var appName = string.Empty;

            switch (gameType)
            {
                case 7:
                    gameName = "Võ Lâm Truyền Kỳ Mobile";
                    appName = "vltkm";
                    break;
                case 8:
                    gameName = "Danh Tướng 3Q";
                    appName = "3q";
                    break;

            }


            var roleList = new List<Role>();

            var gameCookieJson = UtilsZing.GetTokenCache(appName, accountName);
            var gameCookie = new GameCookie();
            if (!string.IsNullOrEmpty(gameCookieJson))
            {
                gameCookie = serializer.Deserialize<GameCookie>(gameCookieJson);
            }
            else
            {
                gameCookie = GetLoginM(accountName, passWord, gameType);
            }

            var login = new LoginMBilling();
            var tryAgain = 0;
            roleUrl = "https://billing.mto.zing.vn/fe/api/store/getRoles";


            while ((login.returnCode == 0 || login.returnCode == -4) && tryAgain < 2)
            {
                if (login.returnCode == -4)
                    gameCookie = GetLoginM(accountName, passWord, gameType);

                var parameters = new Dictionary<string, string>();
                parameters.Add("userID", gameCookie.userID);
                parameters.Add("roleID", string.Empty);
                parameters.Add("serverID", serverID);
                parameters.Add("roleName", string.Empty);
                parameters.Add("loginType", gameCookie.loginType);
                parameters.Add("jtoken", gameCookie.RequestVerificationToken);
                parameters.Add("lang", "VI");
                var serverListResult = Task.Run(() => UtilsZing.PostTask(roleUrl, parameters, new CookieContainer())).Result;
                NLogLogger.Info(new string[] { "ZingService", "Topup", "Get Role Result", serverListResult.HtmlContent });

                if (!string.IsNullOrEmpty(serverListResult.HtmlContent))
                {
                    login = serializer.Deserialize<LoginMBilling>(serverListResult.HtmlContent);
                    if (login.returnCode == 1)
                    {
                        tryAgain = 2;
                        dynamic jsonObj = JsonConvert.DeserializeObject(serverListResult.HtmlContent);
                        try
                        {
                            foreach (var s in jsonObj.data)
                            {
                                var role = serializer.Deserialize<Role>(Convert.ToString(s.First));
                                roleList.Add(role);
                            }
                        }
                        catch { }
                    }


                }
                tryAgain++;

            }

            return roleList;


        }

        public static string Topup(string cardSerial, string cardCode, string accountName, string cardType, GameCookie smasCookie, TransHistory transHistory, string tranId, string tranId3rd, int gameType)
        {

            var pmcID = string.Empty;
            var pmOptID = string.Empty;
            var sID = string.Empty;
            var gameName = string.Empty;
            var productCode = string.Empty;

            switch (gameType)
            {
                case 1: // Thanh toán sản phẩm Võ Lâm Truyền Kỳ Miễn Phí
                    pmcID = "1";
                    pmOptID = "41";
                    sID = "3659";
                    productCode = "volamfree";
                    gameName = "Võ Lâm Truyền Kỳ Miễn Phí";
                    break;
                case 2: // Thanh toán sản phẩm VLTK - Công Thành Chiến
                    pmcID = "1";
                    pmOptID = "51";
                    sID = "4604";
                    productCode = "volamctc";
                    gameName = "VLTK - Công Thành Chiến";
                    break;
                case 3: // Thanh toán sản phẩm VLTK - Công Thành Chiến
                    pmcID = "1";
                    pmOptID = "50";
                    sID = "4316";
                    productCode = "volamtruyenky";
                    gameName = "Võ Lâm Truyền Kỳ 1";
                    break;
                case 4: // Thanh toán sản phẩm  Kiếm Thế
                    pmcID = "1";
                    pmOptID = "40";
                    sID = "3438";
                    productCode = "kiemthe";
                    gameName = "Kiếm Thế";
                    break;
                case 5: // Thanh toán sản phẩm  Tân Thiên Long
                    pmcID = "1";
                    pmOptID = "35";
                    sID = "3114";
                    productCode = "tanthienlong3d";
                    gameName = "Tân Thiên Long 3D";
                    break;
                case 6: // Sản phẩm Võ Lâm Truyền Kỳ 2
                    pmcID = "1";
                    pmOptID = "42";
                    sID = "3660";
                    productCode = "volam2";
                    gameName = "Võ Lâm Truyền Kỳ 2";
                    break;
            }

            var result = string.Empty;
            var parameters = new Dictionary<string, string>();
            GameCookie postTopup = null;
            try
            {
                //parameters.Add("pmcID", transHistory.pmcID.ToString());
                //parameters.Add("pmOptID", transHistory.pmOptID.ToString());
                //parameters.Add("sID", transHistory.sID.ToString());
                //parameters.Add("roleID", "");
                //parameters.Add("roleName", transHistory.roleName);
                //parameters.Add("productCode", transHistory.productCode);
                //parameters.Add("cardSerial", cardSerial);
                //parameters.Add("cardPassword", cardCode);
                //parameters.Add("accountName", accountName.ToLower());

                //parameters.Add("pmcID", "1"); // Zing card = 1
                //parameters.Add("pmOptID", "41");
                //parameters.Add("sID", "3659");
                parameters.Add("pmcID", pmcID); // Zing card = 1
                parameters.Add("pmOptID", pmOptID);
                parameters.Add("sID", sID);
                parameters.Add("roleID", "");
                parameters.Add("roleName", "");
                //parameters.Add("productCode", "volamfree");
                parameters.Add("productCode", productCode);
                parameters.Add("cardSerial", cardSerial);
                parameters.Add("cardPassword", cardCode);
                parameters.Add("accountName", accountName.ToLower());



                var tryAgain = 0;
                while (postTopup == null && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "ZingService", tranId, tranId3rd, "Topup", "Request", tryAgain.ToString(), gameName, serializer.Serialize(parameters) });
                    postTopup = Task.Run(() => UtilsZing.PostTask("https://pay.zing.vn/ajax/payment-zingcard", parameters, smasCookie.CookieContainer)).Result;
                    tryAgain++;
                    Thread.Sleep(1000);
                }

                if (postTopup == null)
                {
                    return "POST_NULL";
                }

                NLogLogger.Info(new string[] { "ZingService", tranId, tranId3rd, "Topup", "Response", gameName, postTopup.HtmlContent.Replace("\r\n", string.Empty) });

                if (postTopup.IsTimeout)
                {
                    NLogLogger.Info(new string[] { "ZingService", tranId, tranId3rd, "Topup", "Error TimeOut", gameName, serializer.Serialize(parameters) });
                    return "POST_TIMEOUT";
                }

                var payment = serializer.Deserialize<PaymentZingCard>(postTopup.HtmlContent);

                if (payment.returnCode == 1)
                {
                    parameters.Clear();
                    parameters.Add("transID", payment.transID.ToString());

                    NLogLogger.Info(new string[] { "ZingService", tranId, tranId3rd, "Check Tran", "Request", payment.transID.ToString() });
                    Thread.Sleep(1000); // Pending đễ ông ZING xử lý
                    var postTopupResult = Task.Run(() => UtilsZing.PostTask("https://pay.zing.vn/ajax/get-result", parameters, smasCookie.CookieContainer)).Result;
                    NLogLogger.Info(new string[] { "ZingService", tranId, tranId3rd, "Check Tran", "Response", postTopupResult.HtmlContent.Replace("\r\n", string.Empty) });

                    if (postTopupResult != null)
                    {
                        var paymentResult = serializer.Deserialize<PaymentZingCardResult>(postTopupResult.HtmlContent);
                        tryAgain = 0;
                        while (paymentResult.returnCode == 2 && tryAgain < 3)
                        {
                            Thread.Sleep(2000);
                            postTopupResult = Task.Run(() => UtilsZing.PostTask("https://pay.zing.vn/ajax/get-result", parameters, smasCookie.CookieContainer)).Result;
                            NLogLogger.Info(new string[] { "ZingService", tranId, tranId3rd, "Check Tran", tryAgain.ToString(), "Response", postTopupResult.HtmlContent });
                            paymentResult = serializer.Deserialize<PaymentZingCardResult>(postTopupResult.HtmlContent);
                            tryAgain++;
                        }

                        if (paymentResult.returnCode == 2)
                            result = paymentResult.returnMessage + " TranId: " + payment.transID;
                        else if (paymentResult.returnCode == 1)
                            result = "Success: " + paymentResult.grossValue + " | TranId: " + payment.transID;
                        else
                            result = paymentResult.returnMessage + " | TranId: " + payment.transID;
                    }
                }
                else
                {
                    result = payment.returnMessage;
                }

                NLogLogger.Info(new string[] { "ZingService", tranId, tranId3rd, "Result", cardSerial, cardCode, accountName, result });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "ZingService", tranId, tranId3rd, "Topup", "Error", serializer.Serialize(parameters), e.Message, smasCookie.SessionId });
                return "POST_EXCEPTION";
            }

            return result;
        }

        public static string TopupM(string cardSerial, string cardCode, string accountName, string passWord, string tranId, string tranId3rd, int gameType, string extData)
        {

            var result = string.Empty;
            var gameName = string.Empty;
            var appName = string.Empty;
            var paymentUrl = string.Empty;
            var clientKey = string.Empty;


            switch (gameType)
            {
                case 7: // Thanh toán sản phẩm Võ Lâm Truyền Mobile
                    paymentUrl = "https://billing.mto.zing.vn/fe/api/pmt/payCard";
                    gameName = "Võ Lâm Truyền Kỳ Mobile";
                    appName = "vltkm";
                    clientKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjIjoyMDIyMSwiYSI6MTAyMjEsInMiOjF9.9XincSYUb_NmrF_vJEqgQzLvV_yItGoFGHjVZjB_KlQ";
                    break;
                case 8: // Thanh toán sản phẩm Võ Lâm Truyền Mobile
                    paymentUrl = "https://billing.mto.zing.vn/fe/api/pmt/payCard";
                    gameName = "Danh Tướng 3Q";
                    appName = "3q";
                    clientKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjIjoyMDM4MSwiYSI6MTAzODEsInMiOjF9.pM-o1MVYNaTTmO1OiPzWZx0mKMFIMLWCaHiqaD0NBDA";
                    break;

            }

            var gameCookieJson = UtilsZing.GetTokenCache(appName, accountName);
            var gameCookie = new GameCookie();
            if (!string.IsNullOrEmpty(gameCookieJson))
            {
                gameCookie = serializer.Deserialize<GameCookie>(gameCookieJson);
            }
            else
            {
                gameCookie = GetLoginM(accountName, passWord, gameType);
            }

            var extDataObj = serializer.Deserialize<ZingGame>(extData);
            var tryAgainMaster = 0;
            var payment = new PaymentMZingCardResult();

            while ((payment.returnCode == 0 || payment.returnCode == -4) && tryAgainMaster < 2)
            {
                if (payment.returnCode == -4)
                {
                    //Set empty Cache
                    UtilsZing.SetTokenCache(appName, accountName, string.Empty);
                    gameCookie = GetLoginM(accountName, passWord, gameType);
                }

                var parameters = new Dictionary<string, string>();
                try
                {
                    parameters.Add("jtoken", gameCookie.RequestVerificationToken); // Zing card = 1
                    parameters.Add("roleID", extDataObj.roleID);
                    parameters.Add("serverID", extDataObj.serverID);
                    parameters.Add("productID", extDataObj.productID);
                    parameters.Add("pmcID", "1");
                    parameters.Add("paymentGatewayID", "1");
                    parameters.Add("paymentGroupID", "card");
                    parameters.Add("paymentPartnerID", "1");
                    parameters.Add("providerID", "1");
                    parameters.Add("country", "VN");
                    parameters.Add("currency", "VND");
                    parameters.Add("amount", extDataObj.amount);
                    parameters.Add("lang", "VI");
                    parameters.Add("cardSerial", cardSerial);
                    parameters.Add("cardPassword", cardCode);
                    parameters.Add("userID", gameCookie.userID);
                    parameters.Add("roleName", extDataObj.roleName);

                    var tryAgain = 0;
                    GameCookie postTopup = null;
                    while (postTopup == null && tryAgain < 3)
                    {
                        NLogLogger.Info(new string[] { "ZingService", tranId, tranId3rd, "Topup", "Request", tryAgain.ToString(), gameName, serializer.Serialize(parameters) });
                        postTopup = Task.Run(() => UtilsZing.PostTask(paymentUrl, parameters, new CookieContainer())).Result;
                        tryAgain++;
                        Thread.Sleep(1000);
                    }

                    if (postTopup == null)
                    {
                        return "POST_NULL";
                    }

                    NLogLogger.Info(new string[] { "ZingService", tranId, tranId3rd, "Topup", "Response", gameName, postTopup.HtmlContent.Replace("\r\n", string.Empty) });

                    if (postTopup.IsTimeout)
                    {
                        NLogLogger.Info(new string[] { "ZingService", tranId, tranId3rd, "Topup", "Error TimeOut", gameName, serializer.Serialize(parameters) });
                        return "POST_TIMEOUT";
                    }

                    if (!string.IsNullOrEmpty(postTopup.HtmlContent))
                    {
                        payment = serializer.Deserialize<PaymentMZingCardResult>(postTopup.HtmlContent.Replace("\r\n", string.Empty));
                        if (payment.returnCode == 1)
                        {
                            tryAgainMaster = 2;
                            parameters.Clear();
                            parameters.Add("country", "VN");
                            parameters.Add("orderNumber", payment.data.orderNumber.ToString());
                            parameters.Add("clientKey", clientKey);
                            parameters.Add("lang", "VI");

                            NLogLogger.Info(new string[] { "ZingService", tranId, tranId3rd, "Check Tran", "Request", payment.data.orderNumber.ToString() });
                            Thread.Sleep(1000); // Pending đễ ông ZING xử lý
                            var postTopupResult = Task.Run(() => UtilsZing.PostTask("https://billing.mto.zing.vn/fe/api/order/getResult", parameters, new CookieContainer())).Result;
                            NLogLogger.Info(new string[] { "ZingService", tranId, tranId3rd, "Check Tran", "Response", postTopupResult.HtmlContent.Replace("\r\n", string.Empty) });

                            if (postTopupResult != null)
                            {
                                var paymentResult = serializer.Deserialize<CheckZingCardOrderResult>(postTopupResult.HtmlContent);
                                tryAgain = 0;
                                while (paymentResult.data.orderStatus == 3 && tryAgain < 3)
                                {
                                    Thread.Sleep(2000);
                                    postTopupResult = Task.Run(() => UtilsZing.PostTask("https://billing.mto.zing.vn/fe/api/order/getResult", parameters, new CookieContainer())).Result;
                                    NLogLogger.Info(new string[] { "ZingService", tranId, tranId3rd, "Check Tran", tryAgain.ToString(), "Response", postTopupResult.HtmlContent });
                                    paymentResult = serializer.Deserialize<CheckZingCardOrderResult>(postTopupResult.HtmlContent);
                                    tryAgain++;
                                }

                                //if (paymentResult.data.orderStatus == -1)
                                //    result = paymentResult.returnMessage + " TranId: " + payment.data.orderNumber.ToString();
                                //else 
                                if (paymentResult.data.orderStatus == -1 || paymentResult.data.orderStatus == 1 || paymentResult.data.orderStatus == 5)
                                    result = "Success: " + paymentResult.data.paymentGrossAmount + " | TranId: " + payment.data.orderNumber;
                                else
                                    result = paymentResult.returnMessage + " | TranId: " + payment.data.orderNumber;
                            }
                        }
                        else
                        {
                            result = payment.returnMessage;
                            if (payment.data != null)
                            {
                                result = payment.returnMessage + " | " + payment.data.orderStatusMessage;
                            }
                        }
                    }

                    NLogLogger.Info(new string[] { "ZingService", tranId, tranId3rd, "Result", cardSerial, cardCode, accountName, result });
                }
                catch (Exception e)
                {
                    NLogLogger.Info(new string[] { "ZingService", tranId, tranId3rd, "Topup", "Error", serializer.Serialize(parameters), e.Message });
                    return "POST_EXEPTION";
                }
                tryAgainMaster++;
            }

            return result;
        }

    }
}