using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;

using Libs.BankDirect.Jav;

using Libs.Utils;

namespace BankGateV2.Callback
{
    /// <summary>
    /// Summary description for JavCharge
    /// </summary>
    public class JavChargeV2 : IHttpHandler
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

                NLogLogger.Info(new string[] { "JavCallback", "Callback", jsonString, context.Request.Headers.Get("X-Signature") });

                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                var callback = javaScriptSerializer.Deserialize<JavBankLib.Callback>(jsonString);

                callback.body = jsonString;
                callback.sign = context.Request.Headers.Get("X-Signature");
                var result = new JavBank().CallbackV2(callback, "fun", "ffb6a4cc70751410a0cbb1969f235bcf", "JAVBanksV2");
                NLogLogger.Info(new string[] { "JavCallback", "Callback Code", result.ResponseCode.ToString() });
                if (result.ResponseCode != -1)
                {
                    context.Response.Write("{ \"rc\" : 0, \"rd\" : \"message\" }");

                }
                else
                {
                    context.Response.Write("{ \"rc\" : -2, \"rd\" : \"sai noi dung\" }");

                }



            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "JavCallback", "ProcessRequest", exp.Message });
                context.Response.Write("{ \"rc\" : -1, \"rd\" : \"message\" }");
            }
            //context.Response.Write(string.Empty);
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