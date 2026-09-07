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
using APIMyMobi.Entity;
using APIMyMobi.GSM;
using Lib.Captcha;
using Libs.API;
using Libs.Report;
using Libs.Utils;



namespace APIMyMobi
{

    public class MyMobiService
    {
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        //private static string urlService = ConfigurationManager.AppSettings["Web_AddPrepaid_Mobi"] ?? "https://next.mobifone.vn/SmartTopupApi2/webresources/topup/manualTopup?phoneNumber={0}&pin={1}&serial=&promoCode=&valueCaptcha={2}";
        //private static string urlService = ConfigurationManager.AppSettings["Web_AddPrepaid_Mobi"] ?? "https://next.mobifone.vn/SmartTopupApi2/webresources/topup/manualTopup2019?data={0}";
        public static APIResponse TopupCard(string cardSerial, string cardCode, string mobile, int type, string accountName, string passWord, string captcha)
        {
            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(passWord))
            {
                return new APIResponse((int)ResponseCode.ServiceIsLocked)
                {
                    Description = "Mobile or AccountName or Password is Empty"
                };
            }

            if (mobile.StartsWith("0") && mobile.Length == 10)
            {
                mobile = mobile.TrimStart('0');
            }

            try
            {

                //var balanceBefor = 0;
                //var balanceAfter = 0;
                var postData = new Dictionary<string, string>();
                var tryAgain = 0;

                var login = DoLogin(accountName, mobile, passWord, type);

                if (login.ResponseCode == (int)ResponseCode.ServiceIsLocked)
                {
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = login.Description
                    };
                }

                while (login.ResponseCode == (int)ResponseCode.SystemBusy && tryAgain < 1)
                {
                    login = DoLogin(accountName, mobile, passWord, type);
                    tryAgain++;
                }

                if (login.ResponseCode == (int)ResponseCode.SystemBusy)
                {
                    return new APIResponse((int)ResponseCode.SystemBusy)
                    {
                        Description = login.Description
                    };
                }

                var apiSecretArr = login.ResponseContent.Split('|');

                //NLogLogger.Info(new string[] { "MyMobiService", "Request BeforBalance", accountName, mobile, cardCode });
                //tryAgain = 0;
                //var resBalance = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
                //while ((string.IsNullOrEmpty(resBalance) || resBalance == "{\"data\":false,\"errors\":null}") && tryAgain < 3)
                //{
                //    NLogLogger.Info(new string[] { "MyMobiService", "Request BeforBalance Try " + tryAgain, accountName, mobile, cardCode });
                //    resBalance = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
                //    tryAgain++;
                //    Thread.Sleep(1000);
                //}

                //NLogLogger.Info(new string[] { "MyMobiService", "Response BeforBalance", accountName, mobile, cardCode, resBalance });

                //if (!string.IsNullOrEmpty(resBalance) || resBalance != "{\"data\":false,\"errors\":null}")
                //{
                //    if (resBalance.Contains("statusCode"))
                //    {
                //        var balanceObjError = serializer.Deserialize<ProfileError>(resBalance);
                //        if (balanceObjError.error.statusCode == 401)
                //        {
                //            var reLogin = DoLogin(accountName, mobile, passWord, type, true);
                //            if (reLogin.ResponseCode != (int)ResponseCode.TransactionSuccessful) // Nếu sai mật khẩu Return luôn
                //            {
                //                reLogin.Description = reLogin.Description + " (balanceBefor)";
                //                return reLogin;
                //            }
                //            apiSecretArr = reLogin.ResponseContent.Split('|');

                //            tryAgain = 0;
                //            resBalance = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
                //            while ((string.IsNullOrEmpty(resBalance) || resBalance == "{\"data\":false,\"errors\":null}") && tryAgain < 3)
                //            {
                //                NLogLogger.Info(new string[] { "MyMobiService", "Request BeforBalance Try " + tryAgain, accountName, mobile, cardCode });
                //                resBalance = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
                //                tryAgain++;
                //                Thread.Sleep(1000);
                //            }

                //            NLogLogger.Info(new string[] { "MyMobiService", "Response BeforBalance", accountName, mobile, cardCode, resBalance });

                //            if (string.IsNullOrEmpty(resBalance) || resBalance == "{\"data\":false,\"errors\":null}")
                //            {

                //                return new APIResponse((int)ResponseCode.SystemBusy)
                //                {
                //                    Description = "Không lấy được số dư trước khi nạp"
                //                };

                //            }
                //            else
                //            {
                //                var balanceObj = serializer.Deserialize<Profile>(resBalance);

                //                if (balanceObj.data[0].title.Contains("TS"))
                //                {
                //                    balanceBefor = Convert.ToInt32(balanceObj.data[0].payment);
                //                }
                //                else
                //                {
                //                    balanceBefor = Convert.ToInt32(balanceObj.data[0].balance);
                //                }
                //            }


                //        }


                //    }
                //    else
                //    {
                //        var balanceObj = serializer.Deserialize<Profile>(resBalance);
                //        if (balanceObj.data[0].title.Contains("TS"))
                //        {
                //            balanceBefor = Convert.ToInt32(balanceObj.data[0].payment);
                //        }
                //        else
                //        {
                //            balanceBefor = Convert.ToInt32(balanceObj.data[0].balance);
                //        }
                //    }

                //}
                //else
                //{
                //    return new APIResponse((int)ResponseCode.SystemBusy)
                //    {
                //        Description = "Không lấy được số dư trước khi nạp"
                //    };
                //}



                // Recharge
                postData.Clear();
                postData.Add("phone", mobile);
                postData.Add("card_id", cardCode);
                //postData.Add("langcode", "vi"); 

                NLogLogger.Info(new string[] { "MyMobiService", "Recharge Request", serializer.Serialize(postData) });
                tryAgain = 0;
                var resRecharge = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/rechargecard", postData, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
                while (string.IsNullOrEmpty(resRecharge) && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "MyMobiService", "Recharge Request Try " + tryAgain, serializer.Serialize(postData) });
                    resRecharge = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/rechargecard", postData, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
                    tryAgain++;
                    Thread.Sleep(1000);
                }
                NLogLogger.Info(new string[] { "MyMobiService", "Recharge Response", serializer.Serialize(postData), resRecharge });

                if (!string.IsNullOrEmpty(resRecharge))
                {
                    if (resRecharge.Contains("POST_CANCELED"))
                    {
                        return new APIResponse((int)ResponseCode.TransactionSuspicious)
                        {
                            Description = "POST_CANCELED API Telco xử lý quá 60s không có kết quả"
                        };
                    }

                    if (resRecharge.Contains("statusCode"))
                    {
                        var profileError = serializer.Deserialize<ProfileError>(resRecharge);
                        if (profileError.error.statusCode == 401)
                        {
                            var reLogin = DoLogin(accountName, mobile, passWord, type, true);
                            if (reLogin.ResponseCode != (int)ResponseCode.TransactionSuccessful) // Nếu sai mật khẩu Return luôn
                            {
                                reLogin.Description = reLogin.Description;
                                return reLogin;
                            }
                            apiSecretArr = reLogin.ResponseContent.Split('|');
                        }

                        resRecharge = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/rechargecard", postData, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
                        while (string.IsNullOrEmpty(resRecharge) && tryAgain < 3)
                        {
                            NLogLogger.Info(new string[] { "MyMobiService", "Recharge Request Try " + tryAgain, serializer.Serialize(postData) });
                            resRecharge = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/rechargecard", postData, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
                            tryAgain++;
                            Thread.Sleep(1000);
                        }
                        NLogLogger.Info(new string[] { "MyMobiService", "Recharge Response", serializer.Serialize(postData), resRecharge });

                    }

                    var rechargeObj = serializer.Deserialize<TopupResponse>(resRecharge);

                    if (rechargeObj.status == 2)
                    {
                        var profileInfor = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
                        NLogLogger.Info(new string[] { "MyMobiService", "Recharge Failed", accountName, mobile, cardCode, profileInfor });
                        return new APIResponse((int)ResponseCode.ServiceIsLocked)
                        {
                            Description = resRecharge + " | Lỗi thuê bao không nạp được chưa rõ nguyên nhân."
                        };
                    }

                    if (rechargeObj.data == true
                    //|| rechargeObj.errors[0].message.Contains("Quá số lần nạp trong ngày")
                    //|| rechargeObj.errors[0].message.Contains("Bạn đã nạp sai quá số lần cho phép")
                    )
                    {

                        //    NLogLogger.Info(new string[] { "MyMobiService", "Request AfterBalance", accountName, mobile, cardCode });
                        //    tryAgain = 0;
                        //    resBalance = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
                        //    while ((string.IsNullOrEmpty(resBalance) || resBalance == "{\"data\":false,\"errors\":null}") && tryAgain < 3)
                        //    {
                        //        resBalance = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
                        //        tryAgain++;
                        //        Thread.Sleep(1000);
                        //    }
                        //    NLogLogger.Info(new string[] { "MyMobiService", "Response AfterBalance", accountName, mobile, cardCode, resBalance });

                        //    if (!string.IsNullOrEmpty(resBalance) || resBalance != "{\"data\":false,\"errors\":null}")
                        //    {

                        //        if (resBalance.Contains("statusCode"))
                        //        {
                        //            var balanceObjError = serializer.Deserialize<ProfileError>(resBalance);
                        //            if (balanceObjError.error.statusCode == 401)
                        //            {
                        //                var reLogin = DoLogin(accountName, mobile, passWord, type, true);
                        //                if (reLogin.ResponseCode != (int)ResponseCode.TransactionSuccessful) // Nếu sai mật khẩu Return luôn
                        //                {
                        //                    reLogin.ResponseCode = (int)ResponseCode.TransactionSuspicious;
                        //                    reLogin.Description = reLogin.Description + " (balanceAfter)";
                        //                    return reLogin;
                        //                }
                        //                apiSecretArr = reLogin.ResponseContent.Split('|');

                        //                tryAgain = 0;
                        //                resBalance = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
                        //                while ((string.IsNullOrEmpty(resBalance) || resBalance == "{\"data\":false,\"errors\":null}") && tryAgain < 3)
                        //                {
                        //                    resBalance = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/user/getprofile", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
                        //                    tryAgain++;
                        //                    Thread.Sleep(1000);
                        //                }

                        //                NLogLogger.Info(new string[] { "MyMobiService", "Response AfterBalance", accountName, mobile, cardCode, resBalance });

                        //                if (string.IsNullOrEmpty(resBalance) || resBalance == "{\"data\":false,\"errors\":null}")
                        //                {
                        //                    return new APIResponse((int)ResponseCode.TransactionSuspicious)
                        //                    {
                        //                        ResponseContent = "Không lấy được số dư sau khi nạp"
                        //                    };
                        //                }
                        //                else
                        //                {
                        //                    var balanceObj = serializer.Deserialize<Profile>(resBalance);

                        //                    if (balanceObj.data[0].title.Contains("TS"))
                        //                    {
                        //                        balanceAfter = Convert.ToInt32(balanceObj.data[0].payment);
                        //                    }
                        //                    else
                        //                    {
                        //                        balanceAfter = Convert.ToInt32(balanceObj.data[0].balance);
                        //                    }

                        //                }
                        //            }
                        //        }
                        //        else
                        //        {
                        //            var balanceObj = serializer.Deserialize<Profile>(resBalance);
                        //            if (balanceObj.data[0].title.Contains("TS"))
                        //            {
                        //                balanceAfter = Convert.ToInt32(balanceObj.data[0].payment);
                        //            }
                        //            else
                        //            {
                        //                balanceAfter = Convert.ToInt32(balanceObj.data[0].balance);
                        //            }
                        //        }

                        //    }

                        //    var amountReal = balanceAfter - balanceBefor;

                        var amountReal = Convert.ToInt32(rechargeObj.card_value);


                        //DateTime lastTime = DateTime.Now;
                        //tryAgain = 0;
                        //var amountReal = 0;
                        //var logMessage = string.Empty;
                        //var historyResult = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/transaction/paymenthistory", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
                        //while (string.IsNullOrEmpty(historyResult) && tryAgain < 3)
                        //{
                        //    historyResult = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/transaction/paymenthistory", null, mobile, apiSecretArr[0], apiSecretArr[1])).Result;
                        //    tryAgain++;
                        //    Thread.Sleep(1000);
                        //}

                        //if (!string.IsNullOrEmpty(historyResult))
                        //{
                        //    var historyObj = serializer.Deserialize<PaymentHistory>(historyResult);
                        //    if (historyObj.errors == null)
                        //    {
                        //        if (historyObj.data.Count > 0)
                        //        {

                        //            var dateUsed = DateTime.ParseExact(historyObj.data[0].time, "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture);
                        //            var totalsec = (dateUsed - lastTime).TotalSeconds;
                        //            if (totalsec >= 0 && totalsec <= 5)
                        //            {
                        //                amountReal = historyObj.data[0].amount;
                        //            }

                        //        }
                        //        else
                        //        {
                        //            logMessage = historyResult;
                        //        }


                        //    }
                        //}
                        //else
                        //{
                        //    logMessage = historyResult;
                        //}

                        int[] listValue = { 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000 };

                        if (listValue.Contains(amountReal))
                        {
                            return new APIResponse((int)ResponseCode.TransactionSuccessful)
                            {
                                ResponseContent = amountReal.ToString()
                            };
                        }
                        else
                        {
                            return new APIResponse((int)ResponseCode.TransactionSuspicious)
                            {
                                ResponseContent = amountReal.ToString()
                            };
                        }

                    }
                    else
                    {
                        if (rechargeObj.errors.Count > 0)
                        {

                            if (rechargeObj.errors.FirstOrDefault().message.Contains("Thẻ không tồn tại"))
                            {
                                return new APIResponse((int)ResponseCode.CardCodeInvalid)
                                {
                                    Description = resRecharge
                                };
                            }

                            if (rechargeObj.errors.FirstOrDefault().message.Contains("Thuê bao bị khóa"))
                            {
                                return new APIResponse((int)ResponseCode.ServiceIsLocked)
                                {
                                    Description = resRecharge
                                };
                            }

                            if (rechargeObj.errors.FirstOrDefault().message.Contains("Thẻ đã được sử dụng"))
                            {
                                return new APIResponse((int)ResponseCode.CardUsed)
                                {
                                    Description = resRecharge
                                };
                            }

                            if (rechargeObj.errors.FirstOrDefault().message.Contains("Mã thẻ không đúng định dạng"))
                            {
                                return new APIResponse((int)ResponseCode.CardFormatInvalid)
                                {
                                    Description = resRecharge
                                };
                            }

                            if (rechargeObj.errors.FirstOrDefault().message.Contains("Quá số lần nạp trong ngày")
                                || rechargeObj.errors.FirstOrDefault().message.Contains("Bạn đã nạp quá số lần nạp thẻ được cho phép trong 1 ngày"))
                            {
                                return new APIResponse((int)ResponseCode.TransactionLimit)
                                {
                                    Description = resRecharge
                                };
                            }

                            if (rechargeObj.errors.FirstOrDefault().message.Contains("Bạn đã nạp sai quá số lần cho phép"))
                            {
                                return new APIResponse((int)ResponseCode.TransactionSuspicious)
                                {
                                    Description = resRecharge
                                };
                            }

                            if (rechargeObj.errors.FirstOrDefault().message.Contains("Địa chỉ IP của Quý khách đã nạp quá số lần cho phép trong ngày"))
                            {
                                return new APIResponse((int)ResponseCode.IpInvalid)
                                {
                                    Description = resRecharge
                                };
                            }

                            if (rechargeObj.errors.FirstOrDefault().message.Contains("Lỗi khác"))
                            {
                                return new APIResponse((int)ResponseCode.TransactionSuspicious)
                                {
                                    Description = resRecharge
                                };
                            }
                            if (rechargeObj.errors.FirstOrDefault().message.Contains("Lỗi hệ thống"))
                            {
                                return new APIResponse((int)ResponseCode.SystemBusy)
                                {
                                    Description = resRecharge
                                };
                            }
                        }

                        //if (rechargeObj.errors.FirstOrDefault().message.Contains("Số điện thoại chưa đăng nhập vào hệ thống"))
                        //{
                        //    return new APIResponse((int)ResponseCode.TransactionLimit)
                        //    {
                        //        Description = resRecharge
                        //    };
                        //}

                        return new APIResponse((int)ResponseCode.TransactionFailed)
                        {
                            Description = resRecharge
                        };
                    }



                }
                else
                {
                    NLogLogger.Info(new string[] { "MyMobi", "Recharge Response", "Error Empty" });
                    return new APIResponse((int)ResponseCode.SystemBusy)
                    {
                        Description = "Lỗi VMS Recharge Response Empty"
                    };
                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyMobiService", "PostTopup", "Error", accountName, mobile, cardCode, e.Message, e.StackTrace });
                return new APIResponse((int)ResponseCode.TransactionFailed)
                {
                    Description = e.Message
                };
            }
            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        private static APIResponse DoLogin(string accountName, string mobile, string passWord, int type, bool reLogin = false)
        {
            string apiSecret = null;
            if (!reLogin) apiSecret = Utils.GetTokenCache(mobile);
            if (apiSecret == null)
            {
                //Login
                var postData = new Dictionary<string, string>();
                postData.Add("phone", mobile);
                postData.Add("password", Sercurity.Encrypts.HashSHA256(passWord));
                NLogLogger.Info(new string[] { "MyMobi", "Request Login", serializer.Serialize(postData) });
                try
                {

                    var tryAgain = 0;
                    var loginRes = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/auth/passwordlogin", postData, mobile, "UEJ34gtH345DFG45G3ht1", "0")).Result;
                    while (string.IsNullOrEmpty(loginRes) && tryAgain < 3)
                    {
                        loginRes = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/auth/passwordlogin", postData, mobile, "UEJ34gtH345DFG45G3ht1", "0")).Result;
                        tryAgain++;
                        Thread.Sleep(1000);
                    }
                    NLogLogger.Info(new string[] { "MyMobi", "Response Login", serializer.Serialize(postData), loginRes });
                    if (!string.IsNullOrEmpty(loginRes))
                    {
                        var loginObj = serializer.Deserialize<Login>(loginRes);
                        if (loginObj.errors == null)
                        {
                            apiSecret = loginObj.data.apiSecret + "|" + loginObj.data.userId;
                            Utils.SetTokenCache(mobile, apiSecret);
                            //Add Account:
                            Action<string, string, int> addAccount = AddAccountVMS;
                            addAccount.BeginInvoke(accountName, passWord, type, null, null);

                            return new APIResponse((int)ResponseCode.TransactionSuccessful)
                            {
                                ResponseContent = apiSecret
                            };
                        }
                        else
                        {
                            Utils.RemoveTokenCache(mobile, apiSecret);
                            if (loginRes.Contains("Xảy ra lỗi hệ thống"))
                            {
                                NLogLogger.Info(new string[] { "MyMobi", "Response Login", "Xảy ra lỗi hệ thống", "SystemBusy" });
                                return new APIResponse((int)ResponseCode.SystemBusy)
                                {
                                    Description = loginRes
                                };
                            }
                            return new APIResponse((int)ResponseCode.ServiceIsLocked)
                            {
                                Description = loginRes
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

        public static APIResponse GetOTP(string mobile)
        {

            var postData = new Dictionary<string, string>();
            postData.Add("phone", mobile);
            var tryAgain = 0;
            var loginRes = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/auth/getloginotp", postData, mobile, "UEJ34gtH345DFG45G3ht1", "0")).Result;
            while (string.IsNullOrEmpty(loginRes) && tryAgain < 3)
            {
                loginRes = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/auth/getloginotp", postData, mobile, "UEJ34gtH345DFG45G3ht1", "0")).Result;
                tryAgain++;
                Thread.Sleep(1000);
            }

            NLogLogger.Info(new string[] { "MyMobi", "GetOTP", "Response", loginRes });

            if (!string.IsNullOrEmpty(loginRes))
            {
                var loginObj = serializer.Deserialize<GetToken>(loginRes);
                if (loginObj.errors == null)
                {
                    return new APIResponse((int)ResponseCode.TransactionSuccessful);
                }
                else
                {
                    return new APIResponse((int)ResponseCode.TransactionFailed)
                    {
                        Description = loginObj.errors[0].message
                    };
                }


            }
            else
            {
                return new APIResponse((int)ResponseCode.TransactionFailed)
                {
                    Description = "Get Otp thất bại!"
                };
            }

        }
        public static APIResponse GetToken(string mobile, string otp)
        {

            var postData = new Dictionary<string, string>();
            postData.Add("phone", mobile);
            postData.Add("otp", otp);
            var tryAgain = 0;
            var loginRes = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/auth/otplogin", postData, mobile, "UEJ34gtH345DFG45G3ht1", "0")).Result;
            while (string.IsNullOrEmpty(loginRes) && tryAgain < 3)
            {
                loginRes = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/auth/otplogin", postData, mobile, "UEJ34gtH345DFG45G3ht1", "0")).Result;
                tryAgain++;
                Thread.Sleep(1000);
            }
            NLogLogger.Info(new string[] { "MyMobi", "GetToken", "Response", loginRes });

            if (!string.IsNullOrEmpty(loginRes))
            {
                var loginObj = serializer.Deserialize<Login>(loginRes);
                if (loginObj.errors == null)
                {
                    //return new APIResponse((int)ResponseCode.TransactionSuccessful);
                    var passWord = GenPassword();
                    postData.Clear();
                    postData.Add("password", Sercurity.Encrypts.HashSHA256(passWord));
                    tryAgain = 0;
                    var changePassRes = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/auth/changepassword", postData, mobile, loginObj.data.apiSecret, loginObj.data.userId.ToString())).Result;
                    while (string.IsNullOrEmpty(loginRes) && tryAgain < 3)
                    {
                        changePassRes = Task.Run(async () => await Utils.PostTask("https://api.mobifone.vn/api/auth/changepassword", postData, mobile, loginObj.data.apiSecret, loginObj.data.userId.ToString())).Result;
                        tryAgain++;
                        Thread.Sleep(1000);
                    }
                    NLogLogger.Info(new string[] { "MyMobi", "Change Password", "Response", changePassRes });

                    if (!string.IsNullOrEmpty(changePassRes))
                    {
                        var changePassObj = serializer.Deserialize<Login>(loginRes);
                        if (changePassObj.errors == null)
                        {
                            return new APIResponse((int)ResponseCode.TransactionSuccessful)
                            {
                                ResponseContent = passWord
                            };
                        }
                        else
                        {
                            return new APIResponse((int)ResponseCode.TransactionFailed)
                            {
                                Description = changePassObj.errors[0].message
                            };
                        }
                    }
                    else
                    {
                        return new APIResponse((int)ResponseCode.TransactionFailed)
                        {
                            Description = changePassRes
                        };
                    }
                }
                else
                {
                    return new APIResponse((int)ResponseCode.TransactionFailed)
                    {
                        Description = loginRes
                    };
                }


            }
            else
            {
                return new APIResponse((int)ResponseCode.TransactionFailed)
                {
                    Description = loginRes
                };
            }

        }

        private static void AddAccountVMS(string accountName, string passWord, int type)
        {
            var account = new Account()
            {
                AccountName = accountName,
                Password = passWord,
                Type = type, //1 : Trả trước; 2: Trả sau
                Source = "Auto"
            };
            var res = account.Insert();
            switch (res)
            {
                case 1:
                    NLogLogger.Info(new string[] { "MyMobiService", "Insert Account Success", res.ToString(), accountName, passWord });
                    break;
                case 2:
                    NLogLogger.Info(new string[] { "MyMobiService", "Update Account Success", res.ToString(), accountName, passWord });
                    break;
                case -99:
                    NLogLogger.Info(new string[] { "MyMobiService", "Insert or Update Account Failed", res.ToString(), accountName, passWord });
                    break;
                default:
                    NLogLogger.Info(new string[] { "MyMobiService", "Insert or Update Account Failed", res.ToString(), accountName, passWord });
                    break;
            }

        }

        public static string GenPassword()
        {
            string[] pp = ("q,w,e,r,t,y,u,i,o,p,a,s,d,f,g,h,j,k,l,z,x,c,v,b,n,m,0,1,2,3,4,5,6,7,8,9").Split(',');
            string tmp = "";
            Random rd = new Random();
            for (int i = 1; i <= 20; i++)
            {
                tmp += pp[rd.Next(0, pp.Length - 1)];
            }
            //var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return tmp.ToLower(); //+ timeSpan.ToString();
        }
    }
}