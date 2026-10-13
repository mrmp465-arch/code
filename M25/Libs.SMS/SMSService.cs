using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Libs.SMS.SMSHandler;
using Libs.SMS._1Pay;
using Libs.Utils;

namespace Libs.SMS
{
    public class SMSService
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public string SmsPlusMoRouter(MessageIn mo)
        {

            //var content = Regex.Replace(mo.message.Trim(), @"\s+", " ");
            NLogLogger.Info(new string[] { "1PaySMSPlus", "SmsPlusMoRouter", serializer.Serialize(mo) });
            var processResult = new SmsPlusForwardHandler().ProcessSms(mo);
            return processResult.ResultMT;
        }

        public bool SmsPlusMoCheck(MessageIn mo, bool realCheck = true)
        {
            var pattern = string.Format(@"^CH +NAP([0-9])+ +{0}_([0-9])+$", "(PC|PL)");
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var regResult = regex.IsMatch(mo.Content);

            if (realCheck)
            {
                if (regResult)
                {
                    return new SmsPlusForwardHandler().ProcessSmsCheck(mo).IsSuccess;
                }
                return false;
            }

            return regResult;
        }
    }


}
