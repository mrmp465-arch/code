using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.BankDirect.M32;
using Libs.Utils;

namespace BankGateV2.M32
{
    /// <summary>
    /// Summary description for MoMo
    /// </summary>
    public class MoMoV5 : IHttpHandler
    {

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            M32BankLib.CallbackResponse callbacResponse;
            try
            {
                var cbObj = new PostGetHelper().GetFromQueryString<M32BankLib.Callback>();
                NLogLogger.Info(new string[] { "M32Calback", "ProcessRequest", serializer.Serialize(cbObj) });

                var newclObj = new M32BankLib.CallbackV2
                {
                    authKey = cbObj.authKey,
                    message = cbObj.message,
                    account_receive = cbObj.account_receive,
                    money = cbObj.money,
                    phone = cbObj.phone,
                    momo_transId = cbObj.momo_transId,
                    requestTime = cbObj.requestTime,
                    type = cbObj.type,
                    pid = "144"
                };
                //if (cbObj.message.ToLower().StartsWith("biz "))
                //    newclObj.pid = "64";
                var callback = new M32Bank().CallbackV2(newclObj);

                if (callback.ResponseCode == (int)ResponseCode.TransactionSuccessful)
                {
                    callbacResponse = new M32BankLib.CallbackResponse()
                    {
                        errorCode = 0,
                        errorDescription = callback.Description
                    };
                }
                else
                {
                    callbacResponse = new M32BankLib.CallbackResponse()
                    {
                        errorCode = callback.ResponseCode,
                        errorDescription = callback.Description
                    };
                }
                NLogLogger.Info(new string[] { "M32Calback", "Approve Request", serializer.Serialize(callback) });
                context.Response.Write(serializer.Serialize(callbacResponse));
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "M32Calback", "ProcessRequest", exp.Message });
                callbacResponse = new M32BankLib.CallbackResponse()
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