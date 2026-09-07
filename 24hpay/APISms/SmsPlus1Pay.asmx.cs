using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Libs.SMS;
using Libs.SMS.SMSHandler;
using Libs.SMS._1Pay;
using Libs.Utils;

namespace APISms
{
    /// <summary>
    /// Summary description for SmsPlus1Pay
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SmsPlus1Pay : System.Web.Services.WebService
    {

        //private static string secretKey = "yetkxokgb9esdasrwp2oca456jkfy4q7";
        //private static string accessKey = "hdb9upumlceo6s6r20qr";

        private static string secretKey = "shhq099buw2vydedqx6ewguibyosogh2";
        private static string accessKey = "yr2vm0zyobrkjtlrgome";

        public static bool billing_SMS_Enable = bool.Parse(ConfigurationManager.AppSettings["BILLING_SMS_ENABLE"] ?? "true");
        private static readonly SMSService service = new SMSService();

        [WebMethod]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string MoCheck(string access_key, string amount, string command_code, string mo_message, string msisdn, string telco, string signature)
        {

            string rt = string.Empty;
            SmsMo1Pay mo1Pay = new SmsMo1Pay();
            if (billing_SMS_Enable)
            {
                try
                {
                    mo1Pay.access_key = access_key;
                    mo1Pay.amount = amount;
                    mo1Pay.command = command_code;
                    mo1Pay.mo_message = mo_message;
                    mo1Pay.msisdn = msisdn;
                    mo1Pay.telco = telco;
                    mo1Pay.signature = signature;

                    var messageIn = new MessageIn()
                    {
                        Content = mo1Pay.mo_message,
                        SenderNumber = mo1Pay.msisdn,
                        Subject = mo1Pay.command,
                        Amount = Convert.ToInt64(mo1Pay.amount),
                        telco = mo1Pay.telco
                    };

                    var signData =
                        string.Format("access_key={0}&amount={1}&command_code={2}&mo_message={3}&msisdn={4}&telco={5}",
                            mo1Pay.access_key, mo1Pay.amount, mo1Pay.command, mo1Pay.mo_message, mo1Pay.msisdn,
                            mo1Pay.telco);

                    if (!isChecksumValid(mo1Pay.signature, signData))
                    {
                        rt = "{\"status\":0,\"sms\":\"Chu ki khong hop le\" ,\"type\" :\"text\"}";
                    }
                    else
                    {

                        if (service.SmsPlusMoCheck(messageIn, true))
                        {
                            rt = "{\"status\":1,\"sms\":\"Kiem tra thanh cong\" ,\"type\" :\"text\"}";
                        }
                        else
                        {
                            rt = "{\"status\":0,\"sms\":\"Kiem tra that bai (1)\" ,\"type\" :\"text\"}";
                        }

                    }
                }
                catch (Exception ex)
                {
                    NLogLogger.Info(new string[] { "1PaySMSPlus", "Error", ex.Message.Replace("\n", " ") });
                    rt = "{\"status\":0,\"sms\":\"Loi he thong\" ,\"type\" :\"text\"}";
                }
            }
            else
            {
                rt = "{\"status\":0,\"sms\":\"He thong dang tam dung.\" ,\"type\" :\"text\"}";
            }
            NLogLogger.Info(new string[] { "1PaySMSPlus", "MoCheck", mo1Pay.msisdn, mo1Pay.mo_message, "SentMT", rt });
            Context.Response.Output.Write(rt);
            Context.Response.End();
            return string.Empty;

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string MoForward(string access_key, string amount, string command_code, string error_code, string error_message, string mo_message, string msisdn, string request_id, string request_time, string signature)
        {
            string rt = string.Empty;
            SmsMo1Pay mo1Pay = new SmsMo1Pay();
            //SmsMo mo = new SmsMo();
            if (billing_SMS_Enable)
            {
                try
                {
                    mo1Pay.access_key = access_key;
                    mo1Pay.amount = amount;
                    mo1Pay.command = command_code;
                    mo1Pay.error_code = error_code;
                    mo1Pay.error_message = error_message;
                    mo1Pay.mo_message = mo_message;
                    mo1Pay.msisdn = msisdn;
                    mo1Pay.request_id = request_id;
                    mo1Pay.request_time = request_time; //DateTime.ParseExact(request_time, "yyyyMMddHHmmss", null);
                    mo1Pay.signature = signature;

                    if (mo1Pay.error_code.Equals("WCG-0000"))
                    {
                        var signData =
                            string.Format(
                                "access_key={0}&amount={1}&command_code={2}&error_code={3}&error_message={4}&mo_message={5}&msisdn={6}&request_id={7}&request_time={8}",
                                mo1Pay.access_key, mo1Pay.amount, mo1Pay.command, mo1Pay.error_code,
                                mo1Pay.error_message, mo1Pay.mo_message, mo1Pay.msisdn, mo1Pay.request_id,
                                mo1Pay.request_time);
                        if (isChecksumValid(mo1Pay.signature, signData))
                        {
                            var messageIn = new MessageIn()
                            {
                                Content = mo1Pay.mo_message,
                                SenderNumber = mo1Pay.msisdn,
                                CreatedTime = DateTime.Now,
                                Status = (int)MessageInStatus.WaitProcess,
                                ReceivedTime = DateTime.Parse(mo1Pay.request_time, null, System.Globalization.DateTimeStyles.RoundtripKind),
                                RefTranId = mo1Pay.request_id,
                                ReceiverNumber = "9029",
                                Subject = mo1Pay.command,
                                Provider = "1pay",
                                Amount = Convert.ToInt64(mo1Pay.amount),
                                error_code = mo1Pay.error_code,
                                error_message = mo1Pay.error_message,
                                PartnerCode = string.Empty,
                                PartnerId = 0,
                                PartnerCommand = string.Empty
                            };
                            messageIn.Add();
                            if (messageIn.Id > 0)
                            {
                                if (service.SmsPlusMoCheck(messageIn, false))
                                {
                                    rt = service.SmsPlusMoRouter(messageIn);
                                }
                                else
                                {
                                    messageIn.Status = (int)MessageInStatus.FailForward;
                                    var message = string.Format(SmsResponseConstant.PARTNER_NOT_FOUND);
                                    rt = string.Format("{{\"status\":{0},\"sms\":\"{1}\",\"type\":\"text\"}}", 0, message);
                                    messageIn.Description = rt;
                                }
                            }
                            else
                            {
                                rt = "{\"status\":0,\"sms\":\"Khong luu duoc log.\" ,\"type\" :\"text\"}";
                                messageIn.Description = rt;
                                messageIn.Status = (int)MessageInStatus.FailAddLog;
                            }

                            messageIn.Update();

                        }
                        else
                        {
                            rt = "{\"status\":0,\"sms\":\"Chu ki khong hop le\" ,\"type\" :\"text\"}";
                        }
                    }
                    else
                    {
                        rt = "{\"status\":0,\"sms\":\"Thue bao khong du dieu kien\" ,\"type\" :\"text\"}";
                    }
                }
                catch (Exception ex)
                {
                    NLogLogger.Info(new string[] { "1PaySMSPlus", "Error", ex.Message.Replace("\n", " ") });
                    rt = "{\"status\":0,\"sms\":\"Loi he thong\" ,\"type\" :\"text\"}";
                }

            }
            else
            {
                rt = "{\"status\":0,\"sms\":\"He thong dang tam dung.\" ,\"type\" :\"text\"}";
            }
            NLogLogger.Info(new string[] { "1PaySMSPlus", "MoForward", mo1Pay.msisdn, mo1Pay.mo_message, "SentMT", rt });
            Context.Response.Output.Write(rt);
            Context.Response.End();
            return string.Empty;

        }
        private bool isChecksumValid(string checksum, string data)
        {
            //return true;
            var encryptData = Encrypts.SHA256(data, secretKey);
            if (encryptData.Equals(checksum))
                return true;
            return false;
        }

    }
}
