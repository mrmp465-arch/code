using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.BankDirect.Khoai;
using Libs.Utils;

namespace BankGateV2.Callback
{
    /// <summary>
    /// Summary description for KhoaiCallback
    /// </summary>
    public class KhoaiCallback : IHttpHandler
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            KhoaiBankLib.CallbackResponse callbacResponse;
            try
            {
                var cbObj = new PostGetHelper().GetFromQueryString<KhoaiBankLib.Callback>();
                var newclObj = new KhoaiBankLib.CallbackV2
                {
                    chargeAmount = cbObj.chargeAmount,
                    requestId = cbObj.requestId,
                    chargeCode = cbObj.chargeCode,
                    status = cbObj.status,
                    momoTransId = cbObj.momoTransId,
                    chargeId = cbObj.chargeId,
                    chargeType = cbObj.chargeType,
                    pid = "pp",
                    signature=cbObj.signature
                };
                NLogLogger.Info(new string[] { "KhoaiCalback", "ProcessRequest", serializer.Serialize(newclObj) });
                if (newclObj.status!= "success" && newclObj.status != "unknown")
                {
                    context.Response.Write("fail");
                    return;
                }    
                var callback = new KhoaiBank().CallbackV2(newclObj);
               

                if (callback.ResponseCode == (int)ResponseCode.TransactionSuccessful)
                {
                    callbacResponse = new KhoaiBankLib.CallbackResponse()
                    {
                        errorCode = 0,
                        errorDescription = callback.Description
                    };
                }
                else
                {
                    callbacResponse = new KhoaiBankLib.CallbackResponse()
                    {
                        errorCode = callback.ResponseCode,
                        errorDescription = callback.Description
                    };
                }
                NLogLogger.Info(new string[] { "KhoaiCalback", "Approve Request", serializer.Serialize(callback) });
                context.Response.Write(serializer.Serialize(callbacResponse));
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "KhoaiCalback", "ProcessRequest", exp.Message });
                callbacResponse = new KhoaiBankLib.CallbackResponse()
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