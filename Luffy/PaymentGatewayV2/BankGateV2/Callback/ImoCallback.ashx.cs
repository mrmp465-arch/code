using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.BankDirect.Bicbic;
using Libs.Utils;
using Libs.BankDirect.VNPayBank;
using Libs.BankCash.Jav;
using Libs.BankDirect.ToxBank;


namespace BankGateV2.Callback
{
    /// <summary>
    /// Summary description for ImoCallback
    /// </summary>
    public class ImoCallback : IHttpHandler
    {

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            BicbicBankLib.CallbackResponse callbacResponse;
            try
            {

                var cbObj = new PostGetHelper().GetFromQueryString<Libs.BankDirect.ImoBank.ImoBankLib.Callback>();
                if (cbObj.status == "timeout")
                {
                    return;
                }
                NLogLogger.Info(new string[] { "IMOCalback", "ProcessRequest", serializer.Serialize(cbObj) });

                if (cbObj.chargeType == "bank" || cbObj.chargeType == "momo" || cbObj.chargeType == "vtpay" | cbObj.chargeType == "usdt")
                {
                    if (cbObj.status == "success")
                    {
                        var callback = new Libs.BankDirect.ImoBank.ImoBank().Callback(cbObj);

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
                }
                if (cbObj.chargeType == "momoout" || cbObj.chargeType == "bankout")
                {

                    if (cbObj.status == "success" || cbObj.status == "deleted"|| cbObj.status == "timeout" || cbObj.status == "cancel")
                    {
                        var cashcallback = new Libs.BankCash.Imo.ImoBankLib.Callback
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
                        var callback = new Libs.BankCash.Imo.ImoBank().Callback(cashcallback);
                        // NLogLogger.Info(new string[] { "VNPayCalback", "ProcessRequest", serializer.Serialize(cbObj) });

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

                }
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "IMOCalback", "ProcessRequest", exp.Message
    });
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