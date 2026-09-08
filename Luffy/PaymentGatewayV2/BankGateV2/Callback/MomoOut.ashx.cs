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
    /// Summary description for MomoOut
    /// </summary>
    public class MomoOut : IHttpHandler
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

                NLogLogger.Info(new string[] { "HynBankCalback", "Callback", jsonString });

                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                var respone = javaScriptSerializer.Deserialize<JavBankLib.CallbackResponse>(jsonString);
                //var callback = javaScriptSerializer.Deserialize<JavBankLib.Callback>(respone.ResponseContent);

                var result = new JavBank().Callback(respone);
                if (result.ResponseCode == (int)ResponseCode.IpInvalid)
                    context.Response.StatusCode = 403; //Forbidden
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "HynBankCalback", "ProcessRequest", exp.Message });
            }
            context.Response.Write(string.Empty);
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