using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.SMS._1Pay;
using Libs.Utils;

namespace Libs.SMS.SMSHandler
{
    public class SmsPlusForwardHandler
    {
        public ProcessResult ProcessSmsCheck(MessageIn mo)
        {

            var resultMT = string.Empty;
            //Content =CH NAP1 MRQ_XXX

            var x = mo.Content.Trim().Split(' ');
            var partnerCommand = string.Empty;
            string message = string.Empty;

            if (x.Length >= 3)
            {
                partnerCommand = x[2];
                partnerCommand = partnerCommand.Split('_')[0];
            }
            try
            {
                //Get Partner
                var partner = new Partners().GetSms(partnerCommand);
                if (partner == null)
                {
                    message = string.Format(SmsResponseConstant.PARTNER_NOT_FOUND);
                    return new ProcessResult()
                    {
                        IsSuccess = false,
                        Message = "Loi he thong (e)",
                        ResultMT = string.Format("{{\"status\":{0},\"sms\":\"{1}\",\"type\":\"text\"}}", 0, message),
                        IsCharge = false
                    };
                }

                //Kiểm tra Partner co được add Service ko
                var _payments = new Payments().GetCheckCache("smsplus");
                PartnerService _partnerService = new PartnerService();
                var partnerStatus = _partnerService.GetCache(partner.PartnerID, _payments.ServiceID);
                if (partnerStatus == null || partnerStatus.Status == 0)
                {
                    message = string.Format(SmsResponseConstant.PARTNER_NOT_FOUND);
                    return new ProcessResult()
                    {
                        IsSuccess = false,
                        Message = "Loi he thong (e)",
                        ResultMT = string.Format("{{\"status\":{0},\"sms\":\"{1}\",\"type\":\"text\"}}", 0, message),
                        IsCharge = false
                    };
                }

                var sign = Encrypts.MD5(String.Format("{0}{1}{2}{3}{4}{5}{6}", partner.PartnerCode, mo.Amount, partnerCommand, mo.Content, mo.SenderNumber, mo.telco, partner.PrivateKey));

                var uri = string.Format("{0}?partnerCode={1}&amount={2}&command_code={3}&mo_message={4}&msisdn={5}&telco={6}&signature={7}",
                partner.SMSPlusUrl, partner.PartnerCode, mo.Amount, partnerCommand, mo.Content, mo.SenderNumber, mo.telco, sign);
                NLogLogger.Info(new string[] { "1PaySMSPlus Check", "Request Partner", uri });
                var resJson = SMSServiceLib.HttpGet(uri);
                NLogLogger.Info(new string[] { "1PaySMSPlus Check", "Response Partner", resJson });
                // Dich nguoc respose de tra MT

                try
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    var response = serializer.Deserialize<SmsMt>(resJson);

                    if (response.status == 1)
                    {

                        return new ProcessResult()
                        {
                            IsSuccess = true,
                            Message = response.sms,
                            ResultMT = resJson,
                            IsCharge = true
                        };
                    }
                    else
                    {

                        return new ProcessResult()
                        {
                            IsSuccess = false,
                            Message = "Loi he thong (e)",
                            ResultMT = resJson,
                            IsCharge = false
                        };
                    }

                }
                catch (Exception exp)
                {
                    message = string.Format(SmsResponseConstant.SMS_SYSTEM_ERROR);
                    return new ProcessResult()
                    {
                        IsSuccess = false,
                        Message = "Loi he thong (e)",
                        //ResultMT = resultMT,
                        ResultMT = string.Format("{{\"status\":{0},\"sms\":\"{1}\",\"type\":\"text\"}}", 0, message),
                        IsCharge = false
                    };
                }

            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "1PaySMSPlus Check", "Error", ex.Message.Replace("\n", " "), ex.StackTrace.Replace("\n", " ") });
                message = string.Format(SmsResponseConstant.SMS_SYSTEM_ERROR);
                return new ProcessResult()
                {
                    IsSuccess = false,
                    Message = "Loi he thong (e)",
                    ResultMT = string.Format("{{\"status\":{0},\"sms\":\"{1}\",\"type\":\"text\"}}", 0, message),
                    IsCharge = false
                };
            }
        }
        public ProcessResult ProcessSms(MessageIn mo)
        {

            var resultMT = string.Empty;
            //Content =CH NAP1 MRQ_XXX

            var x = mo.Content.Trim().Split(' ');
            var partnerCommand = string.Empty;
            string message = string.Empty;

            if (x.Length >= 3)
            {
                partnerCommand = x[2];
                partnerCommand = partnerCommand.Split('_')[0];
            }
            try
            {
                //Get Partner
                var partner = new Partners().GetSms(partnerCommand);
                if (partner == null)
                {
                    message = string.Format(SmsResponseConstant.PARTNER_NOT_FOUND);
                    return new ProcessResult()
                    {
                        IsSuccess = false,
                        Message = "Loi he thong (e)",
                        ResultMT = string.Format("{{\"status\":{0},\"sms\":\"{1}\",\"type\":\"text\"}}", 0, message),
                        IsCharge = false
                    };
                }

                //Kiểm tra Partner co được add Service ko
                var _payments = new Payments().GetCheckCache("smsplus");
                PartnerService _partnerService = new PartnerService();
                var partnerStatus = _partnerService.GetCache(partner.PartnerID, _payments.ServiceID);
                if (partnerStatus == null || partnerStatus.Status == 0)
                {
                    message = string.Format(SmsResponseConstant.PARTNER_NOT_FOUND);
                    return new ProcessResult()
                    {
                        IsSuccess = false,
                        Message = "Loi he thong (e)",
                        ResultMT = string.Format("{{\"status\":{0},\"sms\":\"{1}\",\"type\":\"text\"}}", 0, message),
                        IsCharge = false
                    };
                }

                //Add mo property
                mo.PartnerId = partner.PartnerID;
                mo.PartnerCode = partner.PartnerCode;
                mo.PartnerCommand = partner.SMSPlusCommand;

                //var msin = new MessageIn();
                mo.Update();

                var sign = Encrypts.MD5(String.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", partner.PartnerCode, mo.Amount, partnerCommand, mo.error_code, mo.error_message, mo.Content, mo.SenderNumber, mo.Id, mo.ReceivedTime.ToString("yyyyMMddHHmmss"), partner.PrivateKey));

                var uri = string.Format(
                    "{0}?partnerCode={1}&amount={2}&command_code={3}&error_code={4}&error_message={5}&mo_message={6}&msisdn={7}&request_id={8}&request_time={9}&signature={10}",
                    partner.SMSPlusUrl, partner.PartnerCode, mo.Amount, partnerCommand, mo.error_code, mo.error_message, mo.Content, mo.SenderNumber, mo.Id, mo.ReceivedTime.ToString("yyyyMMddHHmmss"), sign);
                NLogLogger.Info(new string[] { "1PaySMSPlus", "Request Partner", uri });
                var resJson = SMSServiceLib.HttpGet(uri);
                NLogLogger.Info(new string[] { "1PaySMSPlus", "Response Partner", resJson });
                // Dich nguoc respose de tra MT
                try
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    var response = serializer.Deserialize<SmsMt>(resJson);

                    if (response.status == 1)
                    {
                        mo.Status = (int)MessageInStatus.Done;
                        mo.Description = resJson;
                        mo.Update();
                        return new ProcessResult()
                        {
                            IsSuccess = true,
                            Message = response.sms,
                            ResultMT = resJson,
                            IsCharge = true
                        };
                    }
                    else
                    {
                        mo.Status = (int)MessageInStatus.Fail;
                        mo.Description = resJson;
                        mo.Update();
                        return new ProcessResult()
                        {
                            IsSuccess = false,
                            Message = "Loi he thong (e)",
                            ResultMT = resJson,
                            IsCharge = false
                        };
                    }

                }
                catch (Exception exp)
                {
                    mo.Status = (int)MessageInStatus.Fail;
                    mo.Description = resJson;
                    mo.Update();
                    message = string.Format(SmsResponseConstant.SMS_SYSTEM_ERROR);
                    return new ProcessResult()
                    {
                        IsSuccess = false,
                        Message = "Loi he thong (e)",
                        //ResultMT = resultMT,
                        ResultMT = string.Format("{{\"status\":{0},\"sms\":\"{1}\",\"type\":\"text\"}}", 0, message),
                        IsCharge = false
                    };
                }
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "1PaySMSPlus", "Error", ex.Message.Replace("\n", " "), ex.StackTrace.Replace("\n", " ") });
                message = string.Format(SmsResponseConstant.SMS_SYSTEM_ERROR);
                return new ProcessResult()
                {
                    IsSuccess = false,
                    Message = "Loi he thong (e)",
                    ResultMT = string.Format("{{\"status\":{0},\"sms\":\"{1}\",\"type\":\"text\"}}", 0, message),
                    IsCharge = false
                };
            }
        }

    }
}
