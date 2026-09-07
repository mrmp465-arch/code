using Libs.BankDirect.MDrum;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using static System.Net.WebRequestMethods;

namespace BankGateV2.ServiceHandler
{
    /// <summary>
    /// Summary description for OTPPost
    /// </summary>
    public class OTPPost : IHttpHandler
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            var accNo = HttpContext.Current.Request.QueryString["accNo"];

            var jsonString = String.Empty;
            using (var inputStream = new StreamReader(context.Request.InputStream))
            {
                jsonString = inputStream.ReadToEnd();
            }
            //NLogLogger.Info(new string[] { "OTPPost", "ProcessRequest", jsonString, " accNo ", accNo });
            if (jsonString.Contains("__"))
            {
                jsonString = jsonString.Replace("___", "_");
                jsonString = jsonString.Replace("__", "_");
                var arr = jsonString.Split('_');
                var objRquest = new OTPRequest
                {
                    state = "success",
                    pin = "",
                    otp = arr[0],
                    time = int.Parse(arr[1]),
                    timeCreate=DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    extra = ""
                };
                var key = string.Format("OTP:{0}", accNo);
                //var result = DataCaching.GetCache<OTPRequest>(key);
                OTPDataCaching.SetCache(key, objRquest, 60);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonString))
                {
                    var objRquest = new OTPRequest
                    {
                        state = "success",
                        pin = "",
                        otp = jsonString,
                        time = 0,
                        extra = ""
                    };
                    var key = string.Format("OTP:{0}", accNo);
                    //var result = DataCaching.GetCache<OTPRequest>(key);
                    OTPDataCaching.SetCache(key, objRquest, 60);
                }
            }


            //NLogLogger.Info(new string[] { "OTPPost", "ProcessRequest", jsonString, " accNo " , accNo });

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
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