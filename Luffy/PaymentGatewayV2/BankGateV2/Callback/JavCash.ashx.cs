using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;

using Libs.BankCash.Jav;

using Libs.Utils;


namespace BankGateV2.Callback
{
    /// <summary>
    /// Summary description for JavCash
    /// </summary>
    public class JavCash : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Request.ContentType = "application/json";
            context.Response.ContentType = "application/json";
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.StatusCode = 200;
            var jsonString = string.Empty;
            context.Request.InputStream.Position = 0;
            try
            {
                // string signature = context.Request.QueryString["signature"];
                using (var inputStream = new StreamReader(context.Request.InputStream))
                {
                    jsonString = inputStream.ReadToEnd();
                }

                NLogLogger.Info(new string[] { "JavCallback", "Callback", jsonString });

                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                var callback = javaScriptSerializer.Deserialize<JavBankLib.Callback>(jsonString);

                //callback.body = jsonString;
                //callback.sign = context.Request.Headers.Get("X-Signature");
                //var result = new JavBank().Callback(callback);
                context.Response.Write("{ \"rc\" : 0, \"rd\" : \"message\" }");
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "JavCallback Cash", "ProcessRequest", exp.Message });
                context.Response.Write("{ \"rc\" :-1, \"rd\" : \"message\" }");
            }
           
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