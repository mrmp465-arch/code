using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.BankDirect.Bicbic;
using Libs.Utils;

namespace BankGateV2.Callback
{
    /// <summary>
    /// Summary description for BicbicCallback
    /// </summary>
    public class BicbicCallback : IHttpHandler
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Request.ContentType = "application/json";
            context.Response.ContentType = "application/json";
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.StatusCode = 200;
            var jsonString = string.Empty;
            context.Request.InputStream.Position = 0;
            BicbicBankLib.CallbackResponse callbacResponse;
            try
            {
                // string signature = context.Request.QueryString["signature"];
                using (var inputStream = new StreamReader(context.Request.InputStream))
                {
                    jsonString = inputStream.ReadToEnd();
                }

                NLogLogger.Info(new string[] { "BicBic", "Callback", jsonString });

                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                var callback = javaScriptSerializer.Deserialize<BicbicBankLib.Callback>(jsonString);

                if (callback.type == "IN" && callback.status == 1)
                {
                    var callbackresult = new Libs.BankDirect.Bicbic.BicbicBank().Callback(callback);

                    if (callbackresult.ResponseCode == (int)ResponseCode.TransactionSuccessful)
                    {
                        callbacResponse = new BicbicBankLib.CallbackResponse()
                        {
                            errorCode = 0,
                            errorDescription = callbackresult.Description
                        };
                    }
                    else
                    {
                        callbacResponse = new BicbicBankLib.CallbackResponse()
                        {
                            errorCode = callbackresult.ResponseCode,
                            errorDescription = callbackresult.Description
                        };
                    }
                    NLogLogger.Info(new string[] { "VNPayCalback", "Approve Request", serializer.Serialize(callback) });
                    context.Response.Write(serializer.Serialize(callbacResponse));
                }
                if (callback.type == "OUT")

                {
                    var cashcallback = new Libs.BankCash.Tox.ToxBankLib.Callback
                    {
                        requestId = callback.requestId,
                        amount = callback.amount,
                        status = callback.status,
                        message = callback.message,
                        transId = callback.transId,
                       signature=callback.signature,

                    };
                    var callbackresult = new Libs.BankCash.Tox.ToxBank().Callback(cashcallback);

                    if (callbackresult.ResponseCode == (int)ResponseCode.TransactionSuccessful)
                    {
                        callbacResponse = new BicbicBankLib.CallbackResponse()
                        {
                            errorCode = 0,
                            errorDescription = callbackresult.Description
                        };
                    }
                    else
                    {
                        callbacResponse = new BicbicBankLib.CallbackResponse()
                        {
                            errorCode = callbackresult.ResponseCode,
                            errorDescription = callbackresult.Description
                        };
                    }
                    NLogLogger.Info(new string[] { "VNPayCalback", "Approve Request", serializer.Serialize(callback) });
                    context.Response.Write(serializer.Serialize(callbacResponse));
                }

                //callback.body = jsonString;
                //callback.sign = context.Request.Headers.Get("X-Signature");
                //var result = new JavBank().Callback(callback);
                // context.Response.Write("{ \"rc\" : 0, \"rd\" : \"message\" }");
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "BicBic Callback", "ProcessRequest", exp.Message });
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