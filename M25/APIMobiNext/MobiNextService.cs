using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using APIMobiNext.Entity;
using Lib.Captcha;
using Libs.API;
using Libs.Utils;
using Newtonsoft.Json.Linq;


namespace APIMobiNext
{

    public class MobiNextService
    {
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        //private static string urlService = ConfigurationManager.AppSettings["Web_AddPrepaid_Mobi"] ?? "https://next.mobifone.vn/SmartTopupApi2/webresources/topup/manualTopup?phoneNumber={0}&pin={1}&serial=&promoCode=&valueCaptcha={2}";
        private static string urlService = ConfigurationManager.AppSettings["Web_AddPrepaid_Mobi"] ?? "https://next.mobifone.vn/SmartTopupApi2/webresources/topup/manualTopup2019?data={0}";
        public static APIResponse TopupCard(string cardSerial, string cardCode, string mobile, int type, string accountName, string token, string captcha)
        {
            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(token))
            {
                return new APIResponse((int)ResponseCode.ServiceIsLocked)
                {
                    Description = "Mobile or AccountName or Token is Empty"
                };
            }

            if (!mobile.StartsWith("0") && mobile.Length < 10)
            {
                mobile = "0" + mobile;
            }

            try
            {
                //var captcha = string.Empty;
                //var decaptcha = new Captcha().GetCaptcha(4, accountName);
                //if (decaptcha != null) captcha = decaptcha.Value;

                //var url = string.Format(urlService, mobile, cardCode, captcha);

                var param = string.Format("phoneNumber={0}&pin={1}&serial={2}&promoCode={3}&valueCaptcha={4}&appVersion={5}", mobile, cardCode, "", "", captcha, "5.1");
                var dataRequest = Sercurity.Encrypts.Encrypt(param, Sercurity.Encrypts.BuildKey(mobile, token));
                var url = string.Format(urlService, dataRequest);
                NLogLogger.Info(new string[] { "MobiNextService", "TopupCardRequest", accountName, mobile, cardSerial, cardCode, type.ToString(), token, captcha, dataRequest });

                var res = Task.Run(() => Utils.GeTask(url, token)).Result;

                var tryAgain = 0;

                while (string.IsNullOrEmpty(res) && tryAgain < 3)
                {
                    NLogLogger.Info(new string[] { "MobiNextService", "TopupCardRequest", "Try", tryAgain.ToString(), accountName, mobile, cardSerial, cardCode, type.ToString(), token, captcha });
                    res = Task.Run(() => Utils.GeTask(url, token)).Result;
                    tryAgain++;
                }

                if (!string.IsNullOrEmpty(res))
                {
                    NLogLogger.Info(new string[] { "MobiNextService", "TopupCardResponse", accountName, mobile, cardCode, res });
                    if (res == "Token Invalid")
                    {
                        return new APIResponse((int)ResponseCode.ServiceIsLocked)
                        {
                            Description = res
                        };
                    }
                    if (res == "Gateway Time-out")
                    {
                        return new APIResponse((int)ResponseCode.TransactionTimeout)
                        {
                            Description = res
                        };
                    }

                    //Add Account:
                    Action<string, string, int> addAccount = AddAccountVMS;
                    addAccount.BeginInvoke(accountName, token, type, null, null);


                    if (res.Contains("result"))
                    {

                        var resResult = serializer.Deserialize<TopupSuccessResponse>(res);
                        var resDecrypt = APIMobiNext.Sercurity.Encrypts.Decrypt(Regex.Unescape(resResult.result), Sercurity.Encrypts.BuildKey(mobile, token));
                        var resCard = serializer.Deserialize<TopupResponse>(resDecrypt);

                        //var resCard = serializer.Deserialize<TopupResponse>(res);

                        if (resCard.isSuccess)
                        {
                            return new APIResponse((int)ResponseCode.TransactionSuccessful)
                            {
                                ResponseContent = resCard.valueTopupSuccess.ToString()
                            };
                        }
                        return new APIResponse((int)ResponseCode.TransactionFailed);
                    }

                    var resCardFail = serializer.Deserialize<TopupErrorResponse>(res);

                    switch (resCardFail.code)
                    {
                        case "701":
                            //if (resCardFail.fields.Contains(".jpg"))
                            //{
                            //    //PreGen Captcha
                            //    Action<string, string> send = PreGenCaptCha;
                            //    send.BeginInvoke(accountName, resCardFail.fields, null, null);
                            //}
                            return new APIResponse((int)ResponseCode.CardUsed)
                            {
                                Description = resCardFail.message
                            };
                        case "606":
                            //if (resCardFail.fields.Contains(".jpg"))
                            //{
                            //    //PreGen Captcha
                            //    Action<string, string> send = PreGenCaptCha;
                            //    send.BeginInvoke(accountName, resCardFail.fields, null, null);
                            //}
                            return new APIResponse()
                            {
                                ResponseCode = (int)ResponseCode.ParameterInvalid,
                                ResponseContent = resCardFail.fields,
                                Description = resCardFail.message
                            };
                        case "204":
                            if (resCardFail.message.Contains("System busy") || resCardFail.message.Contains("Hệ thống đang bận"))
                                return new APIResponse((int)ResponseCode.SystemBusy)
                                {
                                    Description = resCardFail.message
                                };

                            return new APIResponse((int)ResponseCode.CardCodeInvalid)
                            {
                                Description = resCardFail.message
                            };

                        case "607":
                        case "605": //TopUp function will be locked for 5 minutes
                            return new APIResponse((int)ResponseCode.ServiceIsLocked)
                            {
                                Description = resCardFail.message
                            };
                        case "608": //Bạn đã thực hiện nạp tiền qua thẻ cào vượt quá số lần cho phép trong ngày
                            return new APIResponse((int)ResponseCode.TransactionLimit)
                            {
                                Description = resCardFail.message
                            };

                        case "609":
                        case "610": 
                            return new APIResponse((int)ResponseCode.SystemBusy)
                            {
                                Description = resCardFail.message
                            };

                        default:
                            return new APIResponse((int)ResponseCode.TransactionFailed)
                            {
                                Description = resCardFail.message
                            };
                    }

                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MobiNextService", "PostTopup", "Error", accountName, mobile, cardCode, e.Message, e.StackTrace });
                return new APIResponse((int)ResponseCode.TransactionFailed);
            }
            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public static APIResponse GetOTP(string mobile, string deviceId)
        {
            string url = "https://next.mobifone.vn/SmartTopupApi2/webresources/manualLogin/phoneNumber";

            var postData = new Dictionary<string, string>();
            postData.Add("MSISDN", mobile);
            postData.Add("osType", "Android");
            postData.Add("osVersion", "8.0");
            postData.Add("appLanguage", "en");
            var res = Task.Run(() => Utils.PostTask(url, postData, deviceId)).Result;
            NLogLogger.Info(new string[] { "MobiNextService", "GetOTP", "Response", res });

            if (res.Contains("isSuccess"))
                return new APIResponse((int)ResponseCode.TransactionSuccessful)
                {
                    ResponseContent = res
                };
            else
            {
                return new APIResponse((int)ResponseCode.TransactionFailed)
                {
                    Description = (string)JObject.Parse(res)["message"]
                };
            }

        }
        public static APIResponse GetToken(string mobile, string otp, string deviceId)
        {
            string url = "https://next.mobifone.vn/SmartTopupApi2/webresources/manualLogin/otp";

            var postData = new Dictionary<string, string>();
            postData.Add("otp", otp);
            postData.Add("phoneNumber", mobile);

            var res = Task.Run(() => Utils.PostTask(url, postData, deviceId)).Result;
            NLogLogger.Info(new string[] { "MobiNextService", "GetToken", "Response", res });
            if (!string.IsNullOrEmpty(res))
            {
                var token = (string)JObject.Parse(res)["token"];
                if (!string.IsNullOrEmpty(token))
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        ResponseContent = token,

                    };
                else
                {
                    return new APIResponse((int)ResponseCode.TransactionFailed)
                    {
                        Description = (string)JObject.Parse(res)["message"]
                    };
                }


            }
            else
            {
                return new APIResponse((int)ResponseCode.TransactionFailed);
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
                    NLogLogger.Info(new string[] { "MobiNextService", "Insert Account Success", res.ToString(), accountName, passWord });
                    break;
                case 2:
                    NLogLogger.Info(new string[] { "MobiNextService", "Update Account Success", res.ToString(), accountName, passWord });
                    break;
                case -99:
                    NLogLogger.Info(new string[] { "MobiNextService", "Insert or Update Account Failed", res.ToString(), accountName, passWord });
                    break;
                default:
                    NLogLogger.Info(new string[] { "MobiNextService", "Insert or Update Account Failed", res.ToString(), accountName, passWord });
                    break;
            }

        }

        
    }
}