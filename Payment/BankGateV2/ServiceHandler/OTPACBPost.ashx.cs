using Libs.BankDirect.MDrum;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using static Libs.BankDirect.KZ.KZBankLib;
using static System.Net.WebRequestMethods;

namespace BankGateV2.ServiceHandler
{
    /// <summary>
    /// Summary description for OTPPost
    /// </summary>
    public class OTPACBPost : IHttpHandler
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
            NLogLogger.Info(new string[] { "OTPACBPost", "ProcessRequest", jsonString, " accNo ", accNo });
            


            //NLogLogger.Info(new string[] { "OTPPost", "ProcessRequest", jsonString, " accNo " , accNo });

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        
    }
}