using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for VMSToken
/// </summary>
public class VMSTokenEntity
{
    public VMSTokenEntity()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public class Request
    {
        public string Command { set; get; }
        public string Mobile { set; get; }
        public string Otp { set; get; }
        public string DeviceId { set; get; }

    }

    public class Response
    {
        public string Code { set; get; }
        public string Message { set; get; }
        public string Data { set; get; }

    }
}