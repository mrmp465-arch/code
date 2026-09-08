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
    public class GppService
    {

        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        public static APIResponse TopupCard(string cardSerial, string cardCode, string mobile, int type, string accountName, string passWord)
        {

            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(passWord))
            {
                return new APIResponse((int)ResponseCode.ServiceIsLocked)
                {
                    Description = "Mobile or AccountName or Password is Empty"
                };
            }

            var decaptcha = new Captcha().GetCaptcha(4, "gpp:" + accountName);
            GppCookie getTopup = null;
            if (decaptcha != null)
                getTopup = UtilsGpp.GetCookieCache(decaptcha.SessionId);

            if (getTopup != null)
            {
                NLogLogger.Info(new string[] { "GppService", "GetTopup", "Cache", accountName, passWord, serializer.Serialize(getTopup) });
                var res = Topup(cardSerial, cardCode, getTopup);

                if (res.Contains("Service Unavailable"))
                {
                    decaptcha.DeleteCaptcha(4, "gpp:" + accountName);
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Token Invalid"))
                {
                    decaptcha.DeleteCaptcha(4, "gpp:" + accountName);
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = res
                    };
                    //return new APIResponse((int)ResponseCode.ParameterInvalid)
                    //{
                    //    Description = res
                    //};
                }

                var resObj = serializer.Deserialize<GppTopupResponse>(res);
                if (!string.IsNullOrEmpty(res))
                    switch (resObj.result.code)
                    {
                        case 1:
                            return new APIResponse((int)ResponseCode.TransactionSuccessful)
                            {
                                Description = resObj.result.message,
                                ResponseContent = resObj.result.returnValue.menh_gia.ToString()
                            };

                        case 0:
                            if (resObj.result.message.Contains("The khong ton tai"))
                                return new APIResponse((int)ResponseCode.CardSerialInvalid)
                                {
                                    Description = resObj.result.message
                                };
                            //if (resObj.result.message.Contains("Nạp thẻ lỗi"))
                            //    return new APIResponse((int)ResponseCode.CardCodeInvalid)
                            //    {
                            //        Description = resObj.result.message
                            //    };
                            if (resObj.result.message.Contains("The da duoc su dung"))
                                return new APIResponse((int)ResponseCode.CardUsed)
                                {
                                    Description = resObj.result.message
                                };

                            return new APIResponse((int)ResponseCode.TransactionFailed)
                            {
                                Description = resObj.result.message
                            };

                        case -1:
                            return new APIResponse((int)ResponseCode.ParameterInvalid)
                            {
                                Description = resObj.result.message
                            };

                    }

                return new APIResponse((int)ResponseCode.TransactionFailed);
            }
            else
            {

                getTopup = GetTopup(accountName, passWord);
                NLogLogger.Info(new string[] { "GppService", "GetTopup", "Non Cache", accountName, passWord, serializer.Serialize(getTopup) });
                if (getTopup.IsLogin)
                {
                    getTopup.SessionId = UtilsGpp.GenSid(accountName);
                    UtilsGpp.SetCookieCache(getTopup.SessionId, getTopup);
                    var sessionCaptcha = new Captcha()
                    {
                        SessionId = getTopup.SessionId,
                        Value = string.Empty,
                        TaskId = 0
                    };
                    sessionCaptcha.Type = 4;
                    sessionCaptcha.Add();
                }
                else
                {
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = getTopup.ResponseMsg
                    };
                }

                var res = Topup(cardSerial, cardCode, getTopup);

                if (res.Contains("Service Unavailable"))
                {
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = res
                    };
                }

                if (res.Contains("Token Invalid"))
                {
                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                    {
                        Description = res
                    };
                    //return new APIResponse((int)ResponseCode.ParameterInvalid)
                    //{
                    //    Description = res
                    //};
                }
                var resObj = serializer.Deserialize<GppTopupResponse>(res);
                if (!string.IsNullOrEmpty(res))
                    switch (resObj.result.code)
                    {
                        case 1:
                            return new APIResponse((int)ResponseCode.TransactionSuccessful)
                            {
                                Description = res,
                                ResponseContent = resObj.result.returnValue.menh_gia.ToString()
                            };

                        case 0:
                            if (resObj.result.message.Contains("The khong ton tai"))
                                return new APIResponse((int)ResponseCode.CardSerialInvalid)
                                {
                                    Description = resObj.result.message
                                };
                            //if (resObj.result.message.Contains("Nạp thẻ lỗi"))
                            //    return new APIResponse((int)ResponseCode.CardCodeInvalid)
                            //    {
                            //        Description = resObj.result.message
                            //    };
                            if (resObj.result.message.Contains("The da duoc su dung"))
                                return new APIResponse((int)ResponseCode.CardUsed)
                                {
                                    Description = resObj.result.message
                                };

                            return new APIResponse((int)ResponseCode.TransactionFailed)
                            {
                                Description = resObj.result.message
                            };

                        case -1:
                            return new APIResponse((int)ResponseCode.ParameterInvalid)
                            {
                                Description = res
                            };

                    }
            }

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public static GppCookie GetTopup(string accountName, string passWord)
        {
            NLogLogger.Info(new string[] { "GppService", "GetTopup", "Request", accountName, passWord });
            var getLogin = Task.Run(() => UtilsGpp.GetTask("https://gpp.com.vn", new GppCookie() { CookieContainer = new CookieContainer(), X_XSRF_TOKEN = string.Empty, IsLogin = false, ResponseMsg = string.Empty })).Result;
            string tenancyName;
            try
            {
                var index = accountName.LastIndexOf("_", StringComparison.Ordinal);
                tenancyName = accountName.Substring(0, index);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[]
                    {"GppService", "GetToup", "Account invalid Format", accountName, passWord, e.Message, e.StackTrace});
                return new GppCookie()
                {
                    IsLogin = false
                };
            }

            var param = new Dictionary<string, string>();
            param.Add("returnUrlHash", string.Empty);
            param.Add("tenancyName", tenancyName);
            param.Add("usernameOrEmailAddress", accountName);
            param.Add("password", passWord);
            param.Add("g-recaptcha-response", string.Empty);
            param.Add("loginAttemptCount", "0");
            var postTaskLogin = Task.Run(() => UtilsGpp.PostTaskLogin("https://gpp.com.vn/?returnUrl=/Application/Index", param, getLogin)).Result;

            if (postTaskLogin.IsLogin)
                //return Task.Run(() => UtilsGpp.GetTask("https://gpp.com.vn/Application/Index", postTaskLogin)).Result;
                return Task.Run(() => UtilsGpp.GetTask("https://gpp.com.vn/Application/Index#!/lichsugiaodich", postTaskLogin)).Result;

            return postTaskLogin;

        }

        public static string Topup(string cardSerial, string cardCode, GppCookie gppCookie)
        {
            var result = string.Empty;
            var requestCard = new GppTopupRequest()
            {
                maKhachHang = string.Empty,
                maTheCao = cardCode,
                serial = cardSerial
            };
            try
            {
                result = Task.Run(() => UtilsGpp.PostTaskTopup("https://gpp.com.vn/api/services/app/lichSuGiaoDich/NapTien", serializer.Serialize(requestCard), gppCookie)).Result;
                NLogLogger.Info(new string[] { "GppService", "TopupResponse", serializer.Serialize(requestCard), result });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GppService", "Topup", "Error", serializer.Serialize(requestCard), e.Message, gppCookie.SessionId });
                return "{\"result\":{\"code\":-1,\"errorCode\":\"\",\"message\":\"Nạp thẻ lỗi Error Exeption!\",\"returnValue\":null},\"targetUrl\":null,\"success\":true,\"error\":null,\"unAuthorizedRequest\":false,\"__abp\":true}";
            }

            return result;
        }

    }
}