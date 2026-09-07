using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;


using Libs.BankDirect.VNPayBank;
using Libs.BankCash.Jav;
using Libs.Utils;
using static Libs.BankDirect.MDrum.MDrumBankLib;
namespace BankGateV2.Callback
{
    /// <summary>
    /// Summary description for VNPay
    /// </summary>
    public class VNPayCallback : IHttpHandler
    {

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            CallbackResponse callbacResponse;
            var cbObj = new PostGetHelper().GetFromQueryString<Libs.BankDirect.VNPayBank.SimexBankLib.Callback>();
            NLogLogger.Info(new string[] { "VNPayCalback", "ProcessRequest", serializer.Serialize(cbObj) });

            try
            {

                if (cbObj.chargeType == "bank")
                {
                    if (cbObj.status == "success")
                    {
                        var callback = new VNPayBank().Callback(cbObj);

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
                        NLogLogger.Info(new string[] { "VNPayCalback", "Approve Request", serializer.Serialize(callback) });
                        context.Response.Write(serializer.Serialize(callbacResponse));
                    }
                }
                if (cbObj.chargeType == "bankout")
                {
                    var cashcallback = new Libs.BankCash.Jav.SimexBankLib.Callback
                    {
                        bank = cbObj.bank,
                        chargeAmount = cbObj.chargeAmount,
                        chargeCode = cbObj.chargeCode,
                        chargeId = cbObj.chargeId,
                        chargeType = cbObj.chargeType,
                        momoTransId = cbObj.momoTransId,
                        requestId = cbObj.requestId,
                        signature = cbObj.signature,
                        status = cbObj.status,
                        result = cbObj.result,

                    };
                    var callback = new JavBank().Callback(cashcallback);

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
                    NLogLogger.Info(new string[] { "VNPayCalback", "Approve Request", serializer.Serialize(callback) });
                    context.Response.Write(serializer.Serialize(callbacResponse));

                }

                //context.Response.Write("callback");
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "VNPayCalback", "ProcessRequest", exp.Message });
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