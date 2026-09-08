using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace APIMyViettel.Entity
{
    [Serializable]
    public class GppCookie
    {
        public CookieContainer CookieContainer { get; set; }
        public string X_XSRF_TOKEN { get; set; }
        public string SessionId { get; set; }
        //public bool IsTopup { get; set; }
        public bool IsLogin { get; set; }
        public string ResponseMsg { get; set; }


    }
}