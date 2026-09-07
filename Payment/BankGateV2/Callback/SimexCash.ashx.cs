using Libs.API;
using Libs.BankCash.Simex;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using static Libs.BankCash.Drum.DrumBankLib;

namespace BankGateV2.Callback
{
    /// <summary>
    /// Summary description for SimexCash
    /// </summary>
    public class SimexCash : IHttpHandler
    {

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            CallbackResponse callbacResponse;
            var cbObj = new PostGetHelper().GetFromQueryString<SimexBankLib.Callback>();
            NLogLogger.Info(new string[] { "Simex", "ProcessRequest", serializer.Serialize(cbObj) });

            try
            {

                var callback = new SimexBank().Callback(cbObj);

                if (callback.ResponseCode == (int)ResponseCode.TransactionSuccessful)
                {
                    callbacResponse = new CallbackResponse()
                    {
                        errorCode = 0,
                        errorDescription = callback.Description
                    };
                }
                else
                {
                    callbacResponse = new CallbackResponse()
                    {
                        errorCode = callback.ResponseCode,
                        errorDescription = callback.Description
                    };
                }
                NLogLogger.Info(new string[] { "Simex", "Approve Request", serializer.Serialize(callback) });
                context.Response.Write(serializer.Serialize(callbacResponse));

                //context.Response.Write("callback");
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "Simex", "ProcessRequest", exp.Message });
                callbacResponse = new CallbackResponse()
                {
                    errorCode = (int)ResponseCode.ParameterInvalid,
                    errorDescription = "Parameter Invalid"
                };
                context.Response.Write(serializer.Serialize(callbacResponse));
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