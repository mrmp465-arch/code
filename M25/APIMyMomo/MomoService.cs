using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using APIMomo.Entity;
using Lib.Captcha;
using Libs.API;
using Libs.Report;
using Libs.Utils;



namespace APIMomo
{

    public class MomoService
    {
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        //private static string urlService = ConfigurationManager.AppSettings["Web_AddPrepaid_Mobi"] ?? "https://next.mobifone.vn/SmartTopupApi2/webresources/topup/manualTopup?phoneNumber={0}&pin={1}&serial=&promoCode=&valueCaptcha={2}";
        //private static string urlService = ConfigurationManager.AppSettings["Web_AddPrepaid_Mobi"] ?? "https://next.mobifone.vn/SmartTopupApi2/webresources/topup/manualTopup2019?data={0}";
        public static APIResponse TopupCard(string cardSerial, string cardCode, string mobile, int type, string accountName, string passWord, string captcha)
        {
            //if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(passWord))
            //{
            //    return new APIResponse((int)ResponseCode.ServiceIsLocked)
            //    {
            //        Description = "Mobile or AccountName or Password is Empty"
            //    };
            //}

            //if (mobile.StartsWith("0") && mobile.Length == 10)
            //{
            //    mobile = mobile.TrimStart('0');
            //}

            //try
            //{

            //    //var balanceBefor = 0;
            //    //var balanceAfter = 0;
            //    var postData = new Dictionary<string, string>();
            //    var tryAgain = 0;

            //    var login = DoLogin(mobile, passWord, type);

            //    if (login.ResponseCode == (int)ResponseCode.ServiceIsLocked)
            //    {
            //        return new APIResponse((int)ResponseCode.ServiceIsLocked)
            //        {
            //            Description = login.Description
            //        };
            //    }

            //    while (login.ResponseCode == (int)ResponseCode.SystemBusy && tryAgain < 1)
            //    {
            //        login = DoLogin(mobile, passWord, type);
            //        tryAgain++;
            //    }

            //    if (login.ResponseCode == (int)ResponseCode.SystemBusy)
            //    {
            //        return new APIResponse((int)ResponseCode.SystemBusy)
            //        {
            //            Description = login.Description
            //        };
            //    }

            //    var apiSecretArr = login.ResponseContent.Split('|');

            //    //NLogLogger.Info(new string[] { "MyMobiService", "Request BeforBalance", accountName, mobile, cardCode });
            //    //tryAgain = 0;
            //    //var resBalance = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
            //    //while ((string.IsNullOrEmpty(resBalance) || resBalance == "{\"data\":false,\"errors\":null}") && tryAgain < 3)
            //    //{
            //    //    NLogLogger.Info(new string[] { "MyMobiService", "Request BeforBalance Try " + tryAgain, accountName, mobile, cardCode });
            //    //    resBalance = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
            //    //    tryAgain++;
            //    //    Thread.Sleep(1000);
            //    //}

            //    //NLogLogger.Info(new string[] { "MyMobiService", "Response BeforBalance", accountName, mobile, cardCode, resBalance });

            //    //if (!string.IsNullOrEmpty(resBalance) || resBalance != "{\"data\":false,\"errors\":null}")
            //    //{
            //    //    if (resBalance.Contains("statusCode"))
            //    //    {
            //    //        var balanceObjError = serializer.Deserialize<ProfileError>(resBalance);
            //    //        if (balanceObjError.error.statusCode == 401)
            //    //        {
            //    //            var reLogin = DoLogin(accountName, mobile, passWord, type, true);
            //    //            if (reLogin.ResponseCode != (int)ResponseCode.TransactionSuccessful) // Nếu sai mật khẩu Return luôn
            //    //            {
            //    //                reLogin.Description = reLogin.Description + " (balanceBefor)";
            //    //                return reLogin;
            //    //            }
            //    //            apiSecretArr = reLogin.ResponseContent.Split('|');

            //    //            tryAgain = 0;
            //    //            resBalance = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
            //    //            while ((string.IsNullOrEmpty(resBalance) || resBalance == "{\"data\":false,\"errors\":null}") && tryAgain < 3)
            //    //            {
            //    //                NLogLogger.Info(new string[] { "MyMobiService", "Request BeforBalance Try " + tryAgain, accountName, mobile, cardCode });
            //    //                resBalance = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
            //    //                tryAgain++;
            //    //                Thread.Sleep(1000);
            //    //            }

            //    //            NLogLogger.Info(new string[] { "MyMobiService", "Response BeforBalance", accountName, mobile, cardCode, resBalance });

            //    //            if (string.IsNullOrEmpty(resBalance) || resBalance == "{\"data\":false,\"errors\":null}")
            //    //            {

            //    //                return new APIResponse((int)ResponseCode.SystemBusy)
            //    //                {
            //    //                    Description = "Không lấy được số dư trước khi nạp"
            //    //                };

            //    //            }
            //    //            else
            //    //            {
            //    //                var balanceObj = serializer.Deserialize<Profile>(resBalance);

            //    //                if (balanceObj.data[0].title.Contains("TS"))
            //    //                {
            //    //                    balanceBefor = Convert.ToInt32(balanceObj.data[0].payment);
            //    //                }
            //    //                else
            //    //                {
            //    //                    balanceBefor = Convert.ToInt32(balanceObj.data[0].balance);
            //    //                }
            //    //            }


            //    //        }


            //    //    }
            //    //    else
            //    //    {
            //    //        var balanceObj = serializer.Deserialize<Profile>(resBalance);
            //    //        if (balanceObj.data[0].title.Contains("TS"))
            //    //        {
            //    //            balanceBefor = Convert.ToInt32(balanceObj.data[0].payment);
            //    //        }
            //    //        else
            //    //        {
            //    //            balanceBefor = Convert.ToInt32(balanceObj.data[0].balance);
            //    //        }
            //    //    }

            //    //}
            //    //else
            //    //{
            //    //    return new APIResponse((int)ResponseCode.SystemBusy)
            //    //    {
            //    //        Description = "Không lấy được số dư trước khi nạp"
            //    //    };
            //    //}



            //    // Recharge
            //    postData.Clear();
            //    postData.Add("phone", mobile);
            //    postData.Add("card_id", cardCode);
            //    //postData.Add("langcode", "vi"); 

            //    NLogLogger.Info(new string[] { "MyMobiService", "Recharge Request", serializer.Serialize(postData) });
            //    tryAgain = 0;
            //    var resRecharge = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/rechargecard", postData, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
            //    while (string.IsNullOrEmpty(resRecharge) && tryAgain < 3)
            //    {
            //        NLogLogger.Info(new string[] { "MyMobiService", "Recharge Request Try " + tryAgain, serializer.Serialize(postData) });
            //        resRecharge = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/rechargecard", postData, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
            //        tryAgain++;
            //        Thread.Sleep(1000);
            //    }
            //    NLogLogger.Info(new string[] { "MyMobiService", "Recharge Response", serializer.Serialize(postData), resRecharge });

            //    if (!string.IsNullOrEmpty(resRecharge))
            //    {
            //        if (resRecharge.Contains("POST_CANCELED"))
            //        {
            //            return new APIResponse((int)ResponseCode.TransactionSuspicious)
            //            {
            //                Description = "POST_CANCELED API Telco xử lý quá 60s không có kết quả"
            //            };
            //        }

            //        if (resRecharge.Contains("statusCode"))
            //        {
            //            var profileError = serializer.Deserialize<ProfileError>(resRecharge);
            //            if (profileError.error.statusCode == 401)
            //            {
            //                var reLogin = DoLogin(mobile, passWord, type, true);
            //                if (reLogin.ResponseCode != (int)ResponseCode.TransactionSuccessful) // Nếu sai mật khẩu Return luôn
            //                {
            //                    reLogin.Description = reLogin.Description;
            //                    return reLogin;
            //                }
            //                apiSecretArr = reLogin.ResponseContent.Split('|');
            //            }

            //            resRecharge = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/rechargecard", postData, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
            //            while (string.IsNullOrEmpty(resRecharge) && tryAgain < 3)
            //            {
            //                NLogLogger.Info(new string[] { "MyMobiService", "Recharge Request Try " + tryAgain, serializer.Serialize(postData) });
            //                resRecharge = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/rechargecard", postData, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
            //                tryAgain++;
            //                Thread.Sleep(1000);
            //            }
            //            NLogLogger.Info(new string[] { "MyMobiService", "Recharge Response", serializer.Serialize(postData), resRecharge });

            //        }

            //        var rechargeObj = serializer.Deserialize<TopupResponse>(resRecharge);

            //        if (rechargeObj.status == 2)
            //        {
            //            var profileInfor = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
            //            NLogLogger.Info(new string[] { "MyMobiService", "Recharge Failed", accountName, mobile, cardCode, profileInfor });
            //            return new APIResponse((int)ResponseCode.ServiceIsLocked)
            //            {
            //                Description = resRecharge + " | Lỗi thuê bao không nạp được chưa rõ nguyên nhân."
            //            };
            //        }

            //        if (rechargeObj.data == true
            //        //|| rechargeObj.errors[0].message.Contains("Quá số lần nạp trong ngày")
            //        //|| rechargeObj.errors[0].message.Contains("Bạn đã nạp sai quá số lần cho phép")
            //        )
            //        {

            //            //    NLogLogger.Info(new string[] { "MyMobiService", "Request AfterBalance", accountName, mobile, cardCode });
            //            //    tryAgain = 0;
            //            //    resBalance = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
            //            //    while ((string.IsNullOrEmpty(resBalance) || resBalance == "{\"data\":false,\"errors\":null}") && tryAgain < 3)
            //            //    {
            //            //        resBalance = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
            //            //        tryAgain++;
            //            //        Thread.Sleep(1000);
            //            //    }
            //            //    NLogLogger.Info(new string[] { "MyMobiService", "Response AfterBalance", accountName, mobile, cardCode, resBalance });

            //            //    if (!string.IsNullOrEmpty(resBalance) || resBalance != "{\"data\":false,\"errors\":null}")
            //            //    {

            //            //        if (resBalance.Contains("statusCode"))
            //            //        {
            //            //            var balanceObjError = serializer.Deserialize<ProfileError>(resBalance);
            //            //            if (balanceObjError.error.statusCode == 401)
            //            //            {
            //            //                var reLogin = DoLogin(accountName, mobile, passWord, type, true);
            //            //                if (reLogin.ResponseCode != (int)ResponseCode.TransactionSuccessful) // Nếu sai mật khẩu Return luôn
            //            //                {
            //            //                    reLogin.ResponseCode = (int)ResponseCode.TransactionSuspicious;
            //            //                    reLogin.Description = reLogin.Description + " (balanceAfter)";
            //            //                    return reLogin;
            //            //                }
            //            //                apiSecretArr = reLogin.ResponseContent.Split('|');

            //            //                tryAgain = 0;
            //            //                resBalance = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
            //            //                while ((string.IsNullOrEmpty(resBalance) || resBalance == "{\"data\":false,\"errors\":null}") && tryAgain < 3)
            //            //                {
            //            //                    resBalance = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
            //            //                    tryAgain++;
            //            //                    Thread.Sleep(1000);
            //            //                }

            //            //                NLogLogger.Info(new string[] { "MyMobiService", "Response AfterBalance", accountName, mobile, cardCode, resBalance });

            //            //                if (string.IsNullOrEmpty(resBalance) || resBalance == "{\"data\":false,\"errors\":null}")
            //            //                {
            //            //                    return new APIResponse((int)ResponseCode.TransactionSuspicious)
            //            //                    {
            //            //                        ResponseContent = "Không lấy được số dư sau khi nạp"
            //            //                    };
            //            //                }
            //            //                else
            //            //                {
            //            //                    var balanceObj = serializer.Deserialize<Profile>(resBalance);

            //            //                    if (balanceObj.data[0].title.Contains("TS"))
            //            //                    {
            //            //                        balanceAfter = Convert.ToInt32(balanceObj.data[0].payment);
            //            //                    }
            //            //                    else
            //            //                    {
            //            //                        balanceAfter = Convert.ToInt32(balanceObj.data[0].balance);
            //            //                    }

            //            //                }
            //            //            }
            //            //        }
            //            //        else
            //            //        {
            //            //            var balanceObj = serializer.Deserialize<Profile>(resBalance);
            //            //            if (balanceObj.data[0].title.Contains("TS"))
            //            //            {
            //            //                balanceAfter = Convert.ToInt32(balanceObj.data[0].payment);
            //            //            }
            //            //            else
            //            //            {
            //            //                balanceAfter = Convert.ToInt32(balanceObj.data[0].balance);
            //            //            }
            //            //        }

            //            //    }

            //            //    var amountReal = balanceAfter - balanceBefor;

            //            var amountReal = Convert.ToInt32(rechargeObj.card_value);


            //            //DateTime lastTime = DateTime.Now;
            //            //tryAgain = 0;
            //            //var amountReal = 0;
            //            //var logMessage = string.Empty;
            //            //var historyResult = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/transaction/paymenthistory", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
            //            //while (string.IsNullOrEmpty(historyResult) && tryAgain < 3)
            //            //{
            //            //    historyResult = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/transaction/paymenthistory", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
            //            //    tryAgain++;
            //            //    Thread.Sleep(1000);
            //            //}

            //            //if (!string.IsNullOrEmpty(historyResult))
            //            //{
            //            //    var historyObj = serializer.Deserialize<PaymentHistory>(historyResult);
            //            //    if (historyObj.errors == null)
            //            //    {
            //            //        if (historyObj.data.Count > 0)
            //            //        {

            //            //            var dateUsed = DateTime.ParseExact(historyObj.data[0].time, "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture);
            //            //            var totalsec = (dateUsed - lastTime).TotalSeconds;
            //            //            if (totalsec >= 0 && totalsec <= 5)
            //            //            {
            //            //                amountReal = historyObj.data[0].amount;
            //            //            }

            //            //        }
            //            //        else
            //            //        {
            //            //            logMessage = historyResult;
            //            //        }


            //            //    }
            //            //}
            //            //else
            //            //{
            //            //    logMessage = historyResult;
            //            //}

            //            int[] listValue = { 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000 };

            //            if (listValue.Contains(amountReal))
            //            {
            //                return new APIResponse((int)ResponseCode.TransactionSuccessful)
            //                {
            //                    ResponseContent = amountReal.ToString()
            //                };
            //            }
            //            else
            //            {
            //                return new APIResponse((int)ResponseCode.TransactionSuspicious)
            //                {
            //                    ResponseContent = amountReal.ToString()
            //                };
            //            }

            //        }
            //        else
            //        {
            //            if (rechargeObj.errors.Count > 0)
            //            {

            //                if (rechargeObj.errors.FirstOrDefault().message.Contains("Thẻ không tồn tại"))
            //                {
            //                    return new APIResponse((int)ResponseCode.CardCodeInvalid)
            //                    {
            //                        Description = resRecharge
            //                    };
            //                }

            //                if (rechargeObj.errors.FirstOrDefault().message.Contains("Thuê bao bị khóa"))
            //                {
            //                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
            //                    {
            //                        Description = resRecharge
            //                    };
            //                }

            //                if (rechargeObj.errors.FirstOrDefault().message.Contains("Thẻ đã được sử dụng"))
            //                {
            //                    return new APIResponse((int)ResponseCode.CardUsed)
            //                    {
            //                        Description = resRecharge
            //                    };
            //                }

            //                if (rechargeObj.errors.FirstOrDefault().message.Contains("Mã thẻ không đúng định dạng"))
            //                {
            //                    return new APIResponse((int)ResponseCode.CardFormatInvalid)
            //                    {
            //                        Description = resRecharge
            //                    };
            //                }

            //                if (rechargeObj.errors.FirstOrDefault().message.Contains("Quá số lần nạp trong ngày")
            //                    || rechargeObj.errors.FirstOrDefault().message.Contains("Bạn đã nạp quá số lần nạp thẻ được cho phép trong 1 ngày"))
            //                {
            //                    return new APIResponse((int)ResponseCode.TransactionLimit)
            //                    {
            //                        Description = resRecharge
            //                    };
            //                }

            //                if (rechargeObj.errors.FirstOrDefault().message.Contains("Bạn đã nạp sai quá số lần cho phép"))
            //                {
            //                    return new APIResponse((int)ResponseCode.TransactionSuspicious)
            //                    {
            //                        Description = resRecharge
            //                    };
            //                }

            //                if (rechargeObj.errors.FirstOrDefault().message.Contains("Địa chỉ IP của Quý khách đã nạp quá số lần cho phép trong ngày"))
            //                {
            //                    return new APIResponse((int)ResponseCode.IpInvalid)
            //                    {
            //                        Description = resRecharge
            //                    };
            //                }

            //                if (rechargeObj.errors.FirstOrDefault().message.Contains("Lỗi khác"))
            //                {
            //                    return new APIResponse((int)ResponseCode.TransactionSuspicious)
            //                    {
            //                        Description = resRecharge
            //                    };
            //                }
            //                if (rechargeObj.errors.FirstOrDefault().message.Contains("Lỗi hệ thống"))
            //                {
            //                    return new APIResponse((int)ResponseCode.SystemBusy)
            //                    {
            //                        Description = resRecharge
            //                    };
            //                }
            //            }

            //            //if (rechargeObj.errors.FirstOrDefault().message.Contains("Số điện thoại chưa đăng nhập vào hệ thống"))
            //            //{
            //            //    return new APIResponse((int)ResponseCode.TransactionLimit)
            //            //    {
            //            //        Description = resRecharge
            //            //    };
            //            //}

            //            return new APIResponse((int)ResponseCode.TransactionFailed)
            //            {
            //                Description = resRecharge
            //            };
            //        }



            //    }
            //    else
            //    {
            //        NLogLogger.Info(new string[] { "MyMobi", "Recharge Response", "Error Empty" });
            //        return new APIResponse((int)ResponseCode.SystemBusy)
            //        {
            //            Description = "Lỗi VMS Recharge Response Empty"
            //        };
            //    }

            //}
            //catch (Exception e)
            //{
            //    NLogLogger.Info(new string[] { "MyMobiService", "PostTopup", "Error", accountName, mobile, cardCode, e.Message, e.StackTrace });
            //    return new APIResponse((int)ResponseCode.TransactionFailed)
            //    {
            //        Description = e.Message
            //    };
            //}

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public static APIResponse DoLogin(string mobile, string passWord, int type, bool reLogin = false)
        {
            string apiSecret = null;
            if (!reLogin) apiSecret = Utils.GetTokenCache(mobile);
            if (apiSecret == null)
            {
                //Login
                //var postData = new Dictionary<string, string>();
                //postData.Add("phone", mobile);
                //postData.Add("password", Sercurity.Encrypts.HashSHA256(passWord));

                var momoMsg = new momoMsg();
                momoMsg._class = "mservice.backend.entity.msg.LoginMsg";
                momoMsg.isSetup = false;

                var extra = new extra();
                extra.pHash = "InXTbv1NWINCqG2XhfeLZwjLeeP6kHBQHVc77jb41u8mYwzTWlgqvGJw+VJdsAxE";
                extra.AAID = "";
                extra.IDFA = "26C4BA56-E8F5-476D-AA1B-9B43F84C0E05";
                extra.TOKEN = "e8jyoyw800CEiZoTn59GGy:APA91bFr-BspZjmxth7sTjZRQXgcnlEN-Q4FYWMc6MvVVEHIXFJfKtmSgRdgdIbzRVkMC3nWaklsfZ_jh9ZPruP_8LuJPPOZSDuIvVygDEX0vasMOcmKuUm6K793uVYvfGFI5mfRlxds";
                extra.ONESIGNAL_TOKEN = "e8jyoyw800CEiZoTn59GGy:APA91bFr-BspZjmxth7sTjZRQXgcnlEN-Q4FYWMc6MvVVEHIXFJfKtmSgRdgdIbzRVkMC3nWaklsfZ_jh9ZPruP_8LuJPPOZSDuIvVygDEX0vasMOcmKuUm6K793uVYvfGFI5mfRlxds";
                extra.SIMULATOR = "false";
                extra.MODELID = "A999FBDF-BF08-4DFC-9B00-49D2A51E34D4";
                extra.DEVICE_TOKEN = "4456AF9669E4AEFF37A922A2D376C2E4A1C974D743A921FC1F05C967CAA0B831";
                extra.checkSum = "Ro6wZ3c0GrYFLjao4bxYEMd+jFk9xcwx4i+wfJhFZqXae6EF0/XGK+IRE/rT8FTpiS7J42W3zhf7nbFBKv43Yg==";

                var loginrequest = new LoginRequest();
                loginrequest.user = mobile;
                loginrequest.msgType = "USER_LOGIN_MSG";
                loginrequest.pass = passWord;
                loginrequest.cmdId = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds.ToString();
                loginrequest.lang = "vi";
                loginrequest.time = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
                loginrequest.channel = "APP";
                loginrequest.appVer = 40164;
                //loginrequest.appCode = "3.0.18";
                loginrequest.appCode = "4.0.16";
                loginrequest.deviceOS = "IOS";
                loginrequest.buildNumber = 0;
                loginrequest.appId = "vn.momo.platform";
                loginrequest.result = true;
                loginrequest.errorCode = 0;
                loginrequest.errorDesc = "";
                loginrequest.momoMsg = momoMsg;
                loginrequest.extra = extra;

                NLogLogger.Info(new string[] { "MyMobi", "Request Login", serializer.Serialize(loginrequest) });
                try
                {

                    var tryAgain = 0;
                    var loginRes = Task.Run(async () => await Utils.PostTask("https://owa.momo.vn/public/login", serializer.Serialize(loginrequest), "USER_LOGIN_MSG", mobile)).Result;
                    while (string.IsNullOrEmpty(loginRes) && tryAgain < 3)
                    {
                        loginRes = Task.Run(async () => await Utils.PostTask("https://owa.momo.vn/public/login", serializer.Serialize(loginrequest), "USER_LOGIN_MSG", mobile)).Result;
                        tryAgain++;
                        Thread.Sleep(1000);
                    }
                    NLogLogger.Info(new string[] { "MyMobi", "Response Login", serializer.Serialize(loginrequest), loginRes });
                    if (!string.IsNullOrEmpty(loginRes))
                    {
                        var loginObj = serializer.Deserialize<LoginResponse>(loginRes);
                        if (loginObj.errorCode == 0)
                        {
                            return new APIResponse((int)ResponseCode.TransactionSuccessful)
                            {
                                ResponseContent = loginObj.errorDesc
                            };
                        }
                        else
                        {

                            return new APIResponse((int)ResponseCode.ServiceIsLocked)
                            {
                                Description = loginObj.errorDesc
                            };
                        }


                    }
                    else
                    {
                        return new APIResponse((int)ResponseCode.ServiceIsLocked)
                        {
                            Description = loginRes
                        };
                    }
                }
                catch (Exception e)
                {
                    NLogLogger.Info(new string[] { "MyMobi", "Exception Login", e.Message });
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = "Exception Login"
                    };
                }

            }
            return new APIResponse((int)ResponseCode.TransactionSuccessful)
            {
                ResponseContent = apiSecret
            };
        }

        public static APIResponse SendMessage(string sender, string receiver, int type, bool reLogin = false)
        {

            
            var dataPost =
                "[{ \"ip_address\":\"27.79.135.149\", \"location\":\"{\\\"lat\\\":\\\"\\\",\\\"long\\\":\\\"\\\"}\", " +
                "\"momo_session_id\":\"0ddd3f72-1bc1-40c8-8681-4f77ce69805c\"" +
                ", \"phone_number\":\"0838332187\", \"app_info\":{ \"id\":\"com.mservice.com.vn.MoMoTransfer\", \"version\":\"3.0.18\", \"firebase_app_id\":\"1:938081013731:ios:0c602a37c9cdbd3f\" }, " +
                "\"device\":{ \"category\":\"mobile\", \"mobile_brand_name\":\"Apple\", \"mobile_model_name\":\"iPhone 6\", \"operating_system\":\"IOS\", \"operating_system_version\":\"12.4.6\" }, " +
                "\"event_name\":\"feature_chat_connection\", \"event_params\":{ \"action\":\"receive_message_grpc\", \"stage\":\"chat_1to1\", \"appVersion\":\"3.0.18\", \"phoneOs\":\"IOS\", " +
                "\"message_type\":\"MESSAGE_SENT\", \"message\":{ \"id\":\"00000001627659532924\", " +
                "\"customId\":\"0ddd3f72-1bc1-40c8-8681-4f77ce69805c\", \"senderId\":\"0838332187\", " +
                "\"roomId\":\"92233704092015460892be60f82-82d7-44f6-b29d-64488d34c981\", " +
                "\"requestId\":\"0ddd3f72-1bc1-40c8-8681-4f77ce69805c\", " +
                "\"parts\":{ \"partType\":\"INLINE\", \"payload\":{ \"type\":\"\", \"content\":\"DCMMay\", \"url\":\"\", \"customData\":{ \"avatar\":\"https://s3-ap-southeast-1.amazonaws.com/avatars.mservice.io/01238332187.png\", " +
                "\"_id\":\"01238332187\", \"name\":\"Mr Bin\", \"userName\":\"Mr Bin\" } } }, \"hideFrom\":[ ], \"createdAt\":1627659532924, " +
                "\"messageStatus\":{ \"01238332187\":\"DELIVERED\", \"0912440644\":\"DELIVERED\" } }, " +
                "\"ip_address\":\"27.79.135.149\", \"location\":\"{\\\"lat\\\":\\\"\\\",\\\"long\\\":\\\"\\\"}\", " +
                "\"momo_session_id\":\"d3bf9037-580a-4e70-9690-f9ba49a59810\", " +
                "\"phone_number\":\"0838332187\", \"mac_address\":\"90:fd:73:d6:ad:2c\" }, \"timestamp\":1627659532924 }]";

            NLogLogger.Info(new string[] { "MyMobi", "Request Login", dataPost });
            try
            {

                var tryAgain = 0;
                var sendMessageRes = Task.Run(async () => await Utils.PostTask("https://m.mservice.io/apenpup-api/v1/send", serializer.Serialize(dataPost), "USER_SEND_MSG", sender)).Result;
                while (string.IsNullOrEmpty(sendMessageRes) && tryAgain < 3)
                {
                    sendMessageRes = Task.Run(async () => await Utils.PostTask("https://m.mservice.io/apenpup-api/v1/send", serializer.Serialize(dataPost), "USER_SEND_MSG", sender)).Result;
                    tryAgain++;
                    Thread.Sleep(1000);
                }
                NLogLogger.Info(new string[] { "MyMobi", "Response Send", serializer.Serialize(dataPost), sendMessageRes });
                if (!string.IsNullOrEmpty(sendMessageRes))
                {
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        Description = sendMessageRes
                    };

                }
                else
                {
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = sendMessageRes
                    };
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyMobi", "Exception Login", e.Message });
                return new APIResponse((int)ResponseCode.ServiceIsLocked)
                {
                    Description = "Exception Login"
                };
            }

        }
    }
}