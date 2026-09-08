using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyViettel.Entity
{
    public class LoginRequest
    {
        public string username { set; get; }
        public string password { set; get; }
        public string actionForm { set; get; }
        public string device_name { set; get; }
        public string device_id { set; get; }
        public string os_type { set; get; }
        public string os_version { set; get; }
        public string app_version { set; get; }
        public string imei { set; get; }
        public string model { set; get; }
        public string app_id { set; get; }
        public string build_code { set; get; }
        public string version_app { set; get; }
    }

    public class LoginRequestWeb
    {
        public string account { set; get; }
        public string password { set; get; }
        public string account_target { set; get; }
        public string device_id { set; get; }
    }
}