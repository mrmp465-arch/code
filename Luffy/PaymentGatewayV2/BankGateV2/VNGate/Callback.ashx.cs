using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.BankDirect.Bicbic;
using Libs.Utils;
using Libs.BankDirect.VNPayBank;
namespace BankGateV2.VNGate
{
    /// <summary>
    /// Summary description for Callback
    /// </summary>
    public class Callback : IHttpHandler
    {

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            BicbicBankLib.CallbackResponse callbacResponse;
            var cbObj = new PostGetHelper().GetFromQueryString<VNPayBankLib.Callback>();
            NLogLogger.Info(new string[] { "VNPayCalback", "ProcessRequest", serializer.Serialize(cbObj) });

            try
            {

                if (cbObj.status == "success")
                {
                    var callback = new VNPayBank().CallbackV2(cbObj);

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
                    NLogLogger.Info(new string[] { "VNPayCalback", "Approve Request", serializer.Serialize(callback) });
                    context.Response.Write(serializer.Serialize(callbacResponse));
                }
                //context.Response.Write("callback");
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "VNPayCalback", "ProcessRequest", exp.Message });
                callbacResponse = new BicbicBankLib.CallbackResponse()
                {
                    errorCode = (int)ResponseCode.TransactionFailed,
                    errorDescription = "TransactionFailed"
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