using Libs.API;
using Libs.BankDirect.M32VTP;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace BankGateV2.M32
{
    /// <summary>
    /// Summary description for VTP
    /// </summary>
    public class VTP : IHttpHandler
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            M32VTPBankLib.CallbackResponse callbacResponse;
            try
            {
                var cbObj = new PostGetHelper().GetFromQueryString<M32VTPBankLib.Callback>();
                var callback = new M32VTPBank().Callback(cbObj);
                NLogLogger.Info(new string[] { "M32VTPCalback", "ProcessRequest", serializer.Serialize(cbObj) });

                if (callback.ResponseCode == (int)ResponseCode.TransactionSuccessful)
                {
                    callbacResponse = new M32VTPBankLib.CallbackResponse()
                    {
                        errorCode = 0,
                        errorDescription = callback.Description
                    };
                }
                else
                {
                    callbacResponse = new M32VTPBankLib.CallbackResponse()
                    {
                        errorCode = callback.ResponseCode,
                        errorDescription = callback.Description
                    };
                }
                NLogLogger.Info(new string[] { "M32VTPCalback", "Approve Request", serializer.Serialize(callback) });
                context.Response.Write(serializer.Serialize(callbacResponse));
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "M32VTPCalback", "ProcessRequest", exp.Message });
                callbacResponse = new M32VTPBankLib.CallbackResponse()
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