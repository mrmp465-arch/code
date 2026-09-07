using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMomo.Entity
{
   public class momoMsg
    {
        public string _class { get; set; }
        public bool isSetup { get; set; }
    }

    public class extra
    {
        public string pHash { get; set; }
        public string AAID { get; set; }
        public string IDFA { get; set; }
        public string TOKEN { get; set; }
        public string ONESIGNAL_TOKEN { get; set; }
        public string SIMULATOR { get; set; }
        public string MODELID { get; set; }
        public string DEVICE_TOKEN { get; set; }
        public string checkSum { get; set; }
    }

    public class LoginRequest
    {
        public string user { get; set; }
        public string msgType { get; set; }
        public string pass { get; set; }
        public string cmdId { get; set; }
        public string lang { get; set; }
        public long time { get; set; }
        public string channel { get; set; }
        public int appVer { get; set; }
        public string appCode { get; set; }
        public string deviceOS { get; set; }
        public int buildNumber { get; set; }
        public string appId { get; set; }
        public bool result { get; set; }
        public int errorCode { get; set; }
        public string errorDesc { get; set; }
        public momoMsg momoMsg { get; set; }
        public extra extra { get; set; }
    }


}