using System;
using System.Collections.Generic;
using System.Configuration;
using System.EnterpriseServices;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.Services.Configuration;
using APIMyVNTP.Entity;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APIMyVNTP
{
    public class TopupAppVNP
    {

        //private static string urlService = ConfigurationManager.AppSettings["Web_AddPrepaid_Vina"] ?? "https://api-myvnpt.vnpt.vn/mapi/services/mobile_payment_recharge";
        private static string urlService = ConfigurationManager.AppSettings["Web_AddPrepaid_Vina"] ?? "https://api-myvnpt.vnpt.vn/mapi/services/mobile_payment_recharge_v2";
        //private static string urlServiceOtp = ConfigurationManager.AppSettings["ServiceOtp"] ?? "http://api.speedzxc.com/apiv2/getOTPVinaphone";
        //private static string fcm_registration_token = ConfigurationManager.AppSettings["Fcm_Registration_Token"] ?? "f40DtnuWWTg:APA91bGyl95ANRrAVmwlNykGDvDHw4FiZNkGzNt03TEWhKl8YgJ3s71g4STYqOJpwvVDkctGycefK1POc8CewqxCcdeMGcdT2RD4eyxzL2NGNES4IH6FAKA5FnEie_BzhS5z0TrXzvL6";
        private static string fcm_registration_token = ConfigurationManager.AppSettings["Fcm_Registration_Token"] ?? "dUGtz-1rH1E:APA91bFxAcOps6MHiwc6chGn-RTydfW2NsHpcV6pJ1kLKliVW9XD3SF6OHRq4lyojpzsKeH6z8aHjWOExZr_wiogh4lrB1lutS7nyQB5GsExHNo4Pe5BF2GQPZTqw5DfYZQGPNZ1BzN6";

        private static string device_info = ConfigurationManager.AppSettings["Device_Info"] ?? "SM-G532G";
        //private static string device_info = ConfigurationManager.AppSettings["Device_Info"] ?? "iPhone";

        private static string api_secret = ConfigurationManager.AppSettings["api_secret"] ?? "fd29ecee9163a30ce05c3648df9b78f2";

        private static JavaScriptSerializer serializer = new JavaScriptSerializer();
        //public static APIResponse TopupCard(string pin, string mobile)
        //{

        //    if (string.IsNullOrEmpty(mobile))
        //    {
        //        return new APIResponse((int)ResponseCode.ServiceIsLocked);
        //    }


        //    if (!mobile.StartsWith("0") && mobile.Length < 10)
        //    {
        //        mobile = "0" + mobile;
        //    }

        //    //if (mobile.Length == 11)
        //    //{
        //    //    mobile = Regex.Replace(mobile, "^84", "0");
        //    //}

        //    try
        //    {
        //        var parameters = new DataRequest
        //        {
        //            card_id = pin,
        //            for_msisdn = mobile
        //        };
        //        NLogLogger.Info(new string[] { "TopupAppVNTP", "TopupCard", "Request", serializer.Serialize(parameters) });
        //        var res = Task.Run(() => UtilsMyVNTPApp.PostTask(urlService, serializer.Serialize(parameters))).Result;
        //        NLogLogger.Info(new string[] { "TopupAppVNTP", "TopupCard", "Response", mobile, pin, res });
        //        if (!string.IsNullOrEmpty(res))
        //        {
        //            var response = serializer.Deserialize<CardResponse>(res);

        //            if (response.error_code.Equals("0"))
        //            {
        //                int amountresponse = !string.IsNullOrEmpty(response.message) ? Convert.ToInt32(Regex.Match(response.message, @"\d+").Value) : 0;
        //                if (amountresponse > 0)
        //                {
        //                    return new APIResponse()
        //                    {
        //                        ResponseCode = (int)ResponseCode.TransactionSuccessful,
        //                        ResponseContent = amountresponse.ToString(),
        //                        Description = response.message
        //                    };
        //                }
        //                else
        //                {
        //                    return new APIResponse((int)ResponseCode.ServiceIsLocked);
        //                }

        //            }
        //            else
        //            {
        //                if (response.message.Contains("Mã thẻ") && response.message.Contains("không tồn tại"))
        //                {
        //                    NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", response.message });
        //                    return new APIResponse((int)ResponseCode.CardCodeInvalid)
        //                    {
        //                        Description = response.message
        //                    };
        //                }
        //                else if (response.message.Contains("đã được sử dụng"))
        //                {
        //                    NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", response.message });
        //                    return new APIResponse((int)ResponseCode.CardUsed)
        //                    {
        //                        Description = response.message
        //                    };
        //                }

        //                else if (response.message.Contains("Dịch vụ đang được nâng cấp"))
        //                {
        //                    NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", response.message });
        //                    return new APIResponse((int)ResponseCode.SystemMaintain)
        //                    {
        //                        Description = response.message
        //                    };
        //                }

        //                else if (response.message.Contains("Thuê bao không tồn tại")
        //                         || response.message.Contains("Không khởi tạo được tài khoản Ezpay")
        //                         //|| message.Contains("Nạp thẻ không thành công")
        //                         || response.message.Contains("QK chỉ được phép nạp thẻ sai"))
        //                {
        //                    NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", response.message });
        //                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
        //                    {
        //                        Description = response.message
        //                    };
        //                }

        //                else if (response.message.Contains("Nạp thẻ không thành công"))
        //                {
        //                    NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", response.message });
        //                    if (!string.IsNullOrEmpty(response.result))
        //                    {
        //                        if (response.result.Contains("Could not determine Server id of msisdn"))
        //                            return new APIResponse((int)ResponseCode.ServiceIsLocked)
        //                            {
        //                                Description = response.message + " | " + response.result
        //                            };
        //                    }

        //                    return new APIResponse((int)ResponseCode.TransactionRejected)
        //                    {
        //                        Description = response.message
        //                    };
        //                }

        //                else
        //                {
        //                    NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", "Failded" });
        //                    return new APIResponse((int)ResponseCode.TransactionFailed)
        //                    {
        //                        Description = response.message
        //                    };
        //                }
        //            }

        //        }

        //        return new APIResponse((int)ResponseCode.TransactionFailed);


        //    }
        //    catch (Exception ex)
        //    {
        //        NLogLogger.Info(new string[] { "TopupAppVNTP", "TopupCard", "Error", ex.Message, ex.StackTrace });
        //        return new APIResponse((int)ResponseCode.TransactionFailed);
        //    }

        //}

        public static APIResponse TopupCardV2(long id, string serial, string pin, string mobile)
        {

            if (string.IsNullOrEmpty(mobile))
            {
                return new APIResponse((int)ResponseCode.ServiceIsLocked);
            }


            if (!mobile.StartsWith("0") && mobile.Length < 10)
            {
                mobile = "0" + mobile;
            }

            if (mobile.Length == 11)
            {
                mobile = Regex.Replace(mobile, "^84", "0");
            }

            //Thược hiện gọi OTP Service
            //var resOtp = Task.Run(() => UtilsMyVNTPApp.GetTask(urlServiceOtp)).Result;
            //if (string.IsNullOrEmpty(resOtp))
            //{
            //    NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", "Không có kết qua từ Service OTP", urlServiceOtp });
            //    return new APIResponse((int)ResponseCode.SystemBusy)
            //    {
            //        Description = "Không có kết qua từ Service OTP"
            //    };
            //}
            //var otpObj = serializer.Deserialize<FcmResponse>(resOtp);
            //if (!otpObj.isSuccess)
            //{
            //    NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", "Kết qua từ Service OTP không thành công", urlServiceOtp });
            //    return new APIResponse((int)ResponseCode.SystemBusy)
            //    {
            //        Description = "Kết qua từ Service OTP không thành công"
            //    };
            //}

            //Login MyVNTP


            //var account = new Account()
            //{
            //    AccountName = "0838332187",
            //    Password = "1234567a"
            //};

            var account = new Account().GetAccount(0);
            if (account == null)
            {
                NLogLogger.Info(new string[] { "TopupAppVNTP", id.ToString(), "LogMessage", "Hết tài khoản MyVNP" });
                return new APIResponse((int)ResponseCode.SystemBusy)
                {
                    Description = "Hết tài khoản MyVNP"
                };
            }

            if (!account.AccountName.StartsWith("0") && account.AccountName.Length < 10)
            {
                account.AccountName = "0" + account.AccountName;
            }

            if (account.AccountName.Length == 10)
            {
                account.AccountName = Regex.Replace(account.AccountName, "^0", "84");
            }

            var session = UtilsMyVNTPApp.GetTokenCache(account.AccountName);
            if (!string.IsNullOrEmpty(session))
            {
                NLogLogger.Info(new string[] { "TopupAppVNTP", id.ToString(), account.AccountName, "Session Cached", session });
            }
            else
            {
                // Login
                var loginAppObjResponse = new MyVNTPAppLoginResponse { error_code = "-99" };
                while (loginAppObjResponse.error_code != "0")
                {
                    var loginRequest = new MyVNTPAppLoginResquest()
                    {
                        device_info = device_info,
                        fcm_registration_token = fcm_registration_token,
                        mode = "password",
                        msisdn = account.AccountName,
                        password = Encrypts.MD5(account.Password).ToUpper()
                    };
                    var loginAppResponse = Task.Run(() => UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/authen_msisdn", serializer.Serialize(loginRequest))).Result;
                    NLogLogger.Info(new string[] { "TopupAppVNTP", id.ToString(), "Login response", account.AccountName, account.Password, loginAppResponse });
                    if (!string.IsNullOrEmpty(loginAppResponse))
                    {
                        loginAppObjResponse = serializer.Deserialize<MyVNTPAppLoginResponse>(loginAppResponse);
                        //Kiểm tra xem có login được không
                        switch (loginAppObjResponse.error_code)
                        {
                            case "0":
                                session = loginAppObjResponse.session;
                                UtilsMyVNTPApp.SetTokenCache(account.AccountName, session);
                                break;
                            case "1":
                                account.Status = -1; // Khóa không login được có thể là sai pass
                                account.Update();
                                return new APIResponse((int)ResponseCode.LoginFail)
                                {
                                    Description = loginAppObjResponse.message
                                };
                            case "2":
                                account.CountCharge = 3;
                                account.Status = 1;
                                account.Update();
                                return new APIResponse((int)ResponseCode.LoginFail)
                                {
                                    Description = loginAppObjResponse.message
                                };
                            default:
                                account.Status = -1; // Khóa không login được
                                account.Update();
                                break;

                        }
                    }
                }

            }

            try
            {
                var parameters = new DataRequestV2
                {
                    for_msisdn = mobile,
                    card_id = pin,
                    session = session,
                    api_secret = api_secret,
                    otp = string.Empty,
                    msisdn = account.AccountName
                };

                NLogLogger.Info(new string[] { "TopupAppVNTP", id.ToString(), account.AccountName, "TopupCard", "Request", serializer.Serialize(parameters) });
                var res = Task.Run(() => UtilsMyVNTPApp.PostTask(urlService, serializer.Serialize(parameters))).Result;

                //Update lượt request nạp:
                account.CountCharge = account.CountCharge + 1; // tăng lên 1 lần
                account.Status = 1; // mở lại cho chạy
                account.Update();

                NLogLogger.Info(new string[] { "TopupAppVNTP", id.ToString(), account.AccountName, "TopupCard", "Response", mobile, pin, res });
                if (!string.IsNullOrEmpty(res))
                {
                    var response = serializer.Deserialize<CardResponse>(res);

                    if (response.error_code.Equals("0"))
                    {
                        int amountresponse = !string.IsNullOrEmpty(response.message) ? Convert.ToInt32(Regex.Match(response.message, @"\d+").Value) : 0;
                        if (amountresponse > 0)
                        {
                            return new APIResponse()
                            {
                                ResponseCode = (int)ResponseCode.TransactionSuccessful,
                                ResponseContent = amountresponse.ToString(),
                                Description = response.message
                            };
                        }
                        else
                        {
                            return new APIResponse((int)ResponseCode.ServiceIsLocked);
                        }

                    }
                    else
                    {
                        if (response.message.Contains("Mã thẻ") && response.message.Contains("không tồn tại"))
                        {

                            return new APIResponse((int)ResponseCode.CardCodeInvalid)
                            {
                                Description = response.message
                            };
                        }
                        else if (response.message.Contains("đã được sử dụng"))
                        {

                            return new APIResponse((int)ResponseCode.CardUsed)
                            {
                                Description = response.message
                            };
                        }

                        else if (response.message.Contains("Dịch vụ đang được nâng cấp"))
                        {

                            return new APIResponse((int)ResponseCode.SystemMaintain)
                            {
                                Description = response.message
                            };
                        }

                        else if (response.message.Contains("Thuê bao không tồn tại")
                                 || response.message.Contains("Không khởi tạo được tài khoản Ezpay")
                                 //|| message.Contains("Nạp thẻ không thành công")
                                 || response.message.Contains("QK chỉ được phép nạp thẻ sai")
                                 || response.message.Contains("Bạn chưa đăng ký sử dụng tài khoản Ezpay"))
                        {
                            return new APIResponse((int)ResponseCode.ServiceIsLocked)
                            {
                                Description = response.message
                            };
                        }

                        else if (response.message.Contains("Nạp thẻ không thành công"))
                        {
                            //NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", response.message });
                            //if (!string.IsNullOrEmpty(response.result))
                            //{
                            //    if (response.result.Contains("Could not determine Server id of msisdn"))
                            //        return new APIResponse((int)ResponseCode.ServiceIsLocked)
                            //        {
                            //            Description = response.message + " | " + response.result
                            //        };
                            //}


                            return new APIResponse((int)ResponseCode.CardUsed)
                            {
                                Description = response.message
                            };
                        }

                        else if (response.message.Contains("nhập sai mã quá 5 lần") || response.message.Contains("Chưa yêu cầu otp"))
                        {
                            account.CountCharge = 3;
                            account.Status = 1; // Nghi vẫn dùng chung
                            account.Update();

                            return new APIResponse((int)ResponseCode.TransactionLimit)
                            {
                                Description = response.message
                            };
                        }

                        else if (response.message.Contains("chỉ được phép nạp thẻ cho thuê bao khác 3 lần"))
                        {

                            account.Status = -2; // Nghi vẫn dùng chung
                            account.Update();

                            return new APIResponse((int)ResponseCode.TransactionLimit)
                            {
                                Description = response.message
                            };
                        }

                        else if (response.message.Contains("đã được nạp thẻ quá số lần quy định trong một ngày"))
                        {
                            return new APIResponse((int)ResponseCode.TransactionLimit)
                            {
                                Description = response.message
                            };
                        }


                        else
                        {
                            return new APIResponse((int)ResponseCode.TransactionFailed)
                            {
                                Description = response.message
                            };
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "TopupAppVNTP", "TopupCard", "Error", ex.Message, ex.StackTrace });
                return new APIResponse((int)ResponseCode.TransactionFailed);
            }
            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public static APIResponse TopupCardV2(long id, string serial, string pin, string mobile, string accountName, string passWord)
        {

            if (string.IsNullOrEmpty(mobile))
            {
                return new APIResponse((int)ResponseCode.ServiceIsLocked);
            }


            if (!mobile.StartsWith("0") && mobile.Length < 10)
            {
                mobile = "0" + mobile;
            }

            if (mobile.Length == 10)
            {
                mobile = Regex.Replace(mobile, "^0", "84");
            }

            //Thược hiện gọi OTP Service
            //var resOtp = Task.Run(() => UtilsMyVNTPApp.GetTask(urlServiceOtp)).Result;
            //if (string.IsNullOrEmpty(resOtp))
            //{
            //    NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", "Không có kết qua từ Service OTP", urlServiceOtp });
            //    return new APIResponse((int)ResponseCode.SystemBusy)
            //    {
            //        Description = "Không có kết qua từ Service OTP"
            //    };
            //}
            //var otpObj = serializer.Deserialize<FcmResponse>(resOtp);
            //if (!otpObj.isSuccess)
            //{
            //    NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", "Kết qua từ Service OTP không thành công", urlServiceOtp });
            //    return new APIResponse((int)ResponseCode.SystemBusy)
            //    {
            //        Description = "Kết qua từ Service OTP không thành công"
            //    };
            //}

            //Login MyVNTP
            //var account = new Account().GetAccount(0);
            //if (account == null)
            //{
            //    NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", "Hết tài khoản MyVNP" });
            //    return new APIResponse((int)ResponseCode.SystemBusy)
            //    {
            //        Description = "Hết tài khoản MyVNP"
            //    };
            //}

            var session = UtilsMyVNTPApp.GetTokenCache(mobile);
            if (!string.IsNullOrEmpty(session))
            {
                NLogLogger.Info(new string[] { "TopupAppVNTP", mobile, "Session Cached", session });
            }
            else
            {
                // Login
                var loginAppObjResponse = new MyVNTPAppLoginResponse { error_code = "-99" };

                var loginRequest = new MyVNTPAppLoginResquest()
                {
                    device_info = device_info,
                    fcm_registration_token = fcm_registration_token,
                    mode = "password",
                    msisdn = mobile,
                    password = Encrypts.MD5(passWord).ToUpper()
                };
                var loginAppResponse = Task.Run(() => UtilsMyVNTPApp.PostTask("https://api-myvnpt.vnpt.vn/mapi/services/authen_msisdn", serializer.Serialize(loginRequest))).Result;
                if (!string.IsNullOrEmpty(loginAppResponse))
                {
                    loginAppObjResponse = serializer.Deserialize<MyVNTPAppLoginResponse>(loginAppResponse);
                    //Kiểm tra xem có login được không
                    switch (loginAppObjResponse.error_code)
                    {
                        case "0":
                            session = loginAppObjResponse.session;
                            UtilsMyVNTPApp.SetTokenCache(mobile, session);
                            break;
                        case "1":
                            return new APIResponse((int)ResponseCode.ServiceIsLocked)
                            {
                                Description = loginAppObjResponse.message
                            };
                        case "2":
                            return new APIResponse((int)ResponseCode.ServiceIsLocked)
                            {
                                Description = loginAppObjResponse.message
                            };
                        default:
                            NLogLogger.Info(new string[] { "TopupAppVNTP", "Login Failed", mobile, passWord, loginAppResponse });
                            return new APIResponse((int)ResponseCode.ServiceIsLocked)
                            {
                                Description = loginAppObjResponse.message
                            };
                    }
                }
            }

            try
            {
                var parameters = new DataRequestV2
                {
                    for_msisdn = mobile,
                    card_id = pin,
                    session = session,
                    api_secret = api_secret,
                    otp = string.Empty,
                    msisdn = mobile
                };

                NLogLogger.Info(new string[] { "TopupAppVNTP", "TopupCard", "Request", serializer.Serialize(parameters) });
                var res = Task.Run(() => UtilsMyVNTPApp.PostTask(urlService, serializer.Serialize(parameters))).Result;
                NLogLogger.Info(new string[] { "TopupAppVNTP", "TopupCard", "Response", mobile, pin, res });
                if (!string.IsNullOrEmpty(res))
                {
                    var response = serializer.Deserialize<CardResponse>(res);

                    if (response.error_code.Equals("401"))
                    {
                        UtilsMyVNTPApp.SetTokenCache(mobile, string.Empty);
                        return new APIResponse((int)ResponseCode.ParameterInvalid)
                        {
                            Description = response.message
                        };
                    }


                    if (response.error_code.Equals("0"))
                    {
                        int amountresponse = !string.IsNullOrEmpty(response.message) ? Convert.ToInt32(Regex.Match(response.message, @"\d+").Value) : 0;
                        if (amountresponse > 0)
                        {
                            return new APIResponse()
                            {
                                ResponseCode = (int)ResponseCode.TransactionSuccessful,
                                ResponseContent = amountresponse.ToString(),
                                Description = response.message
                            };
                        }
                        else
                        {
                            return new APIResponse((int)ResponseCode.ServiceIsLocked);
                        }

                    }
                    else
                    {
                        if (response.message.Contains("Mã thẻ") && response.message.Contains("không tồn tại"))
                        {
                            NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", response.message });
                            return new APIResponse((int)ResponseCode.CardCodeInvalid)
                            {
                                Description = response.message
                            };
                        }
                        else if (response.message.Contains("đã được sử dụng"))
                        {
                            NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", response.message });
                            return new APIResponse((int)ResponseCode.CardUsed)
                            {
                                Description = response.message
                            };
                        }

                        else if (response.message.Contains("Dịch vụ đang được nâng cấp"))
                        {
                            NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", response.message });
                            return new APIResponse((int)ResponseCode.SystemMaintain)
                            {
                                Description = response.message
                            };
                        }

                        else if (response.message.Contains("Thuê bao không tồn tại")
                                 || response.message.Contains("Không khởi tạo được tài khoản Ezpay")
                                 //|| message.Contains("Nạp thẻ không thành công")
                                 || response.message.Contains("QK chỉ được phép nạp thẻ sai")
                                 || response.message.Contains("Bạn chưa đăng ký sử dụng tài khoản Ezpay"))
                        {
                            NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", response.message });
                            return new APIResponse((int)ResponseCode.ServiceIsLocked)
                            {
                                Description = response.message
                            };
                        }

                        else if (response.message.Contains("Nạp thẻ không thành công"))
                        {
                            //NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", response.message });
                            //if (!string.IsNullOrEmpty(response.result))
                            //{
                            //    if (response.result.Contains("Could not determine Server id of msisdn"))
                            //        return new APIResponse((int)ResponseCode.ServiceIsLocked)
                            //        {
                            //            Description = response.message + " | " + response.result
                            //        };
                            //}


                            return new APIResponse((int)ResponseCode.CardUsed)
                            {
                                Description = response.message
                            };
                        }

                        else if (response.message.Contains("nhập sai mã quá 5 lần"))
                        {

                            return new APIResponse((int)ResponseCode.ServiceIsLocked)
                            {
                                Description = response.message
                            };
                        }

                        else if (response.message.Contains("Chưa yêu cầu otp") 
                                 || response.message.Contains("quá số lần quy định trong một ngày"))
                        {
                            //Delete Sesssion
                            UtilsMyVNTPApp.SetTokenCache(mobile, string.Empty);
                            return new APIResponse((int)ResponseCode.ServiceIsLocked)
                            {
                                Description = response.message
                            };
                        }


                        else
                        {
                            NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", "Failded" });
                            return new APIResponse((int)ResponseCode.TransactionFailed)
                            {
                                Description = response.message
                            };
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "TopupAppVNTP", "TopupCard", "Error", ex.Message, ex.StackTrace });
                return new APIResponse((int)ResponseCode.TransactionFailed);
            }
            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
    }






}