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
    public class OTPGet : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            var accNo = HttpContext.Current.Request.QueryString["accNo"];

            //NLogLogger.Info(new string[] { "OTPGet", "ProcessRequest", accNo });
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var isOTP = false;

            var key = string.Format("OTP:{0}", accNo);
            var result = OTPDataCaching.GetCache<OTPRequest>(key);
            if (result == null)
            {
                isOTP = true;
            }
            else
            {
                if (result.time < 15)
                {
                    isOTP = true;
                }
                else
                {
                    var otptime = DateTime.ParseExact(result.timeCreate, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                    if (result.time - (DateTime.Now - otptime).TotalSeconds < 14)
                    {
                        isOTP = true;
                    }
                }

            }

            if (isOTP )
            {
                var bank = new BankAccounts().GetByAppDeviceIdCache(accNo);
                var data = new OTPInfo
                {
                    pin = bank.PinOtp,
                    extra = "",
                    command = "genOTPWithPin"

                };
                //NLogLogger.Info(new string[] { "OTPGet", "ProcessRequest", serializer.Serialize(data), accNo });
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