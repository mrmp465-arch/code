<%@ WebHandler Language="C#" Class="MyVMSToken" %>

using System;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using APIMobiNext;
using APIMyMobi;
using Libs.API;
using Libs.Utils;

public class MyVMSToken : IHttpHandler
{
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    public void ProcessRequest(HttpContext context)
    {
        context.Request.ContentType = "application/json";
        context.Response.ContentType = "application/json";
        var jsonString = String.Empty;
        var result = string.Empty;
        context.Request.InputStream.Position = 0;
        try
        {
            using (var inputStream = new StreamReader(context.Request.InputStream))
            {
                jsonString = inputStream.ReadToEnd();
            }

            var request = serializer.Deserialize<VMSTokenEntity.Request>(jsonString);
            if (request.Command == "GETOTP")
            {

                if (request.Mobile.StartsWith("0") && request.Mobile.Length == 10)
                {
                    request.Mobile = request.Mobile.TrimStart('0');
                }
                var res = MyMobiService.GetOTP(request.Mobile);
                if (res.ResponseCode == (int)ResponseCode.TransactionSuccessful)
                    context.Response.Write(serializer.Serialize(new VMSTokenEntity.Response()
                    {
                        Code = "1",
                        Message = "Sussess",
                        Data = ""
                    }));
                else context.Response.Write(serializer.Serialize(new VMSTokenEntity.Response()
                {
                    Code = "-1",
                    Message = "Failed",
                    Data = res.Description
                }));
            }

            if (request.Command == "GETTOKEN")
            {

                if (request.Mobile.StartsWith("0") && request.Mobile.Length == 10)
                {
                    request.Mobile = request.Mobile.TrimStart('0');
                }
                var res = MyMobiService.GetToken(request.Mobile, request.Otp);
                if (res.ResponseCode == (int)ResponseCode.TransactionSuccessful)
                    context.Response.Write(serializer.Serialize(new VMSTokenEntity.Response()
                    {
                        Code = "1",
                        Message = "Sussess",
                        Data = res.ResponseContent
                    }));
                else context.Response.Write(serializer.Serialize(new VMSTokenEntity.Response()
                {
                    Code = "-1",
                    Message = "Failed",
                    Data = res.Description
                }));
            }

        }
        catch (Exception exp)
        {
            NLogLogger.Info(new string[] { "VPGJsonService", "ProcessRequest", exp.Message });
            context.Response.Write(ResponseUtils.Response((int)ResponseCode.ParameterInvalid));
        }
        context.Response.Write(result);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}