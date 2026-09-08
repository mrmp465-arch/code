using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.BankDirect.Bicbic;
using Libs.Utils;
using Libs.BankCash.Ken;


namespace BankGateV2.Ken
{
    /// <summary>
    /// Summary description for Bank
    /// </summary>
    public class BankOut : IHttpHandler
    {

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            BicbicBankLib.CallbackResponse callbacResponse;
            try
            {
                var cbObj = new PostGetHelper().GetFromQueryString<KenBankLib.Callback>();
                NLogLogger.Info(new string[] { "KZCalback", "ProcessRequest", serializer.Serialize(cbObj) });

                var callback = new KenBank().Callback(cbObj);

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
                NLogLogger.Info(new string[] { "KZCalback", "Approve Request", serializer.Serialize(callback) });
                context.Response.Write(serializer.Serialize(callbacResponse));


                //context.Response.Write("callback");
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "KZCalback", "ProcessRequest", exp.Message });
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