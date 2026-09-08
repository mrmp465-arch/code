using System;
using System.Collections.Generic;
using System.Configuration;
using System.EnterpriseServices;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.Services.Configuration;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APIMyVNTP
{
    public class TopupWebVNP
    {

        private static string urlService = ConfigurationManager.AppSettings["Web_AddPrepaid_Vina"] ?? "https://api-myvnpt.vnpt.vn/mapi/services/mobile_payment_recharge";

        private static JavaScriptSerializer serializer = new JavaScriptSerializer();
        public static APIResponse TopupCard(string pin, string mobile)
        {

            if (string.IsNullOrEmpty(mobile))
            {
                return new APIResponse((int)ResponseCode.ServiceIsLocked);
            }

            
            if (!mobile.StartsWith("0") && mobile.Length < 10)
            {
                mobile = "0" + mobile;
            }

            //if (mobile.Length == 11)
            //{
            //    mobile = Regex.Replace(mobile, "^84", "0");
            //}

            try
            {
                var parameters = new DataRequest
                {
                    card_id = pin,
                    for_msisdn = mobile
                };
                NLogLogger.Info(new string[] { "TopupAppVNTP", "TopupCard", "Request", serializer.Serialize(parameters) });
                var res = Task.Run(() => UtilsMyVNTPApp.PostTask(urlService, serializer.Serialize(parameters))).Result;
                NLogLogger.Info(new string[] { "TopupAppVNTP", "TopupCard", "Response", mobile, pin, res });
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

                        else if (response.message.Contains("Thuê bao không tồn tại")
                                 || response.message.Contains("Không khởi tạo được tài khoản Ezpay")
                                 //|| message.Contains("Nạp thẻ không thành công")
                                 || response.message.Contains("QK chỉ được phép nạp thẻ sai"))
                        {
                            NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", response.message });
                            return new APIResponse((int)ResponseCode.ServiceIsLocked)
                            {
                                Description = response.message
                            };
                        }

                        else if (response.message.Contains("Nạp thẻ không thành công"))
                        {
                            NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", response.message });
                            if (!string.IsNullOrEmpty(response.result))
                            {
                                if (response.result.Contains("Could not determine Server id of msisdn"))
                                    return new APIResponse((int)ResponseCode.ServiceIsLocked)
                                    {
                                        Description = response.message + " | " + response.result
                                    };
                            }

                            return new APIResponse((int)ResponseCode.TransactionRejected)
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

                return new APIResponse((int)ResponseCode.TransactionFailed);


            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "TopupAppVNTP", "TopupCard", "Error", ex.Message, ex.StackTrace });
                return new APIResponse((int)ResponseCode.TransactionFailed);
            }

        }

    }






}