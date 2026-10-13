<%@ WebHandler Language="C#" Class="ZingService" %>

using System;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using APIGame;
using Libs.API;
using Libs.Utils;
using System.Linq;

public class ZingService : IHttpHandler
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

            var request = serializer.Deserialize<ZingServerEntity.Request>(jsonString);
            var gameType = -1;
            switch (request.GameType.ToString())
            {
                case "16":
                    gameType = 7;
                    break;
                case "17":
                    gameType = 8; // Fix gói giữ Point
                    break;
            }
            if (request.Command == "GETSERVER")
            {
                var res = APIGame.ZingService.GetServerM(request.AcoutName, request.Password, gameType).OrderBy(x => x.serverID);
                if (res != null)
                    context.Response.Write(serializer.Serialize(new ZingServerEntity.ResponseServer()
                    {
                        Code = "1",
                        Message = "Sussess",
                        Data = res.ToList()
                    }));
                else context.Response.Write(serializer.Serialize(new ZingServerEntity.ResponseServer()
                {
                    Code = "-1",
                    Message = "Failed",
                    Data = null
                }));
            }

            if (request.Command == "GETROLE")
            {

                var res = APIGame.ZingService.GetRoleM(request.AcoutName, request.Password, gameType, request.serverID);
                if (res != null)
                    context.Response.Write(serializer.Serialize(new ZingServerEntity.Response()
                    {
                        Code = "1",
                        Message = "Sussess",
                        Data = res.ToList()
                    }));
                else context.Response.Write(serializer.Serialize(new ZingServerEntity.Response()
                {
                    Code = "-1",
                    Message = "Failed",
                    Data = null
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