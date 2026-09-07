using APIGame.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for VMSToken
/// </summary>
public class ZingServerEntity
{
    public ZingServerEntity()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public class Request
    {
        public string Command { set; get; }
        public string AcoutName { set; get; }
        public string Password { set; get; }
        public int GameType { set; get; }

        public string serverID { set; get; }

        public string DeviceId { set; get; }

    }

    public class Response
    {
        public string Code { set; get; }
        public string Message { set; get; }
        public List<Role> Data { set; get; }

    }
    public class ResponseServer
    {
        public string Code { set; get; }
        public string Message { set; get; }
        public List<Server> Data { set; get; }

    }

}