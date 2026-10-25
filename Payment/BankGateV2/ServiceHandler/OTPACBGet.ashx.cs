using Libs.API;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace BankGateV2.ServiceHandler
{
    /// <summary>
    /// Summary description for OTPGet
    /// </summary>
    public class OTPACBGet : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            var accNo = HttpContext.Current.Request.QueryString["accNo"];
            if (string.IsNullOrEmpty(accNo))
                return;
            NLogLogger.Info(new string[] { "ACBOTPGet", "ProcessRequest", accNo });
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var isOTP = false;

            isOTP = true;
            var systemDataConfig = new SystemDataConfig();
            var lstConfig = systemDataConfig.GetListCache();
            var BankCodeVS8 = systemDataConfig.GetKey(lstConfig, "BankCodeVS8").ToUpper();

            if (!string.IsNullOrEmpty(BankCodeVS8))
            {
                //var bank = new BankAccounts().GetByAppDeviceIdCache(accNo);
                var data = new OTPInfo
                {
                    pin = "0000",
                    extra = BankCodeVS8,
                    command = "genOTPWithPin"

                };
                context.Response.Write(serializer.Serialize(data));
            }



        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        public class OTPInfo
        {
            public string pin { get; set; }
            public string command { get; set; }
            public string extra { get; set; }

        }
        public class OTPRequest
        {
            public string pin { get; set; }
            public string state { get; set; }
            public string otp { get; set; }
            public int time { get; set; }
            public string extra { get; set; }
            public string timeCreate { get; set; }

        }
    }
}