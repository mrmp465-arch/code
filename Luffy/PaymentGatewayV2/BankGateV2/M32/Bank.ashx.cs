using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.BankDirect.Bicbic;
using Libs.BankDirect.M32;
using Libs.Utils;

namespace BankGateV2.M32
{
    /// <summary>
    /// Summary description for MoMo
    /// </summary>
    public class Bank : IHttpHandler
    {

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            BicbicBankLib.CallbackResponse callbacResponse;
            try
            {
               
                var cbObj = new PostGetHelper().GetFromQueryString<BicbicBankLib.Callback>();
                var callback = new BicbicBank().Callback(cbObj);

                NLogLogger.Info(new string[] { "BicbicCalback", "ProcessRequest", context.Request.RawUrl });
                if (callback.ResponseCode == (int)ResponseCode.TransactionSuccessful)
                {
                    callbacResponse = new BicbicBankLib.CallbackResponse()
                    {
                        errorCode = 0,
                        errorDescription = callback.Description
                    };
                }
                else
                {
                    callbacResponse = new BicbicBankLib.CallbackResponse()
                    {
                        errorCode = callback.ResponseCode,
                        errorDescription = callback.Description
                    };
                }
                NLogLogger.Info(new string[] { "BicbicCalback", "Approve Request", serializer.Serialize(callback) });
                context.Response.Write(serializer.Serialize(callbacResponse));
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "BicbicCalback", "ProcessRequest", exp.Message });
                callbacResponse = new BicbicBankLib.CallbackResponse()
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