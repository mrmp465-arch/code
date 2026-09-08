using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace APIMyVNTP.Entity
{
    [Serializable]
    public class MyVNTPWebCookie
    {
        public CookieContainer CookieContainer { get; set; }
        public string HtmlContent { get; set; }
        public string CaptChaLink { get; set; }
        public string CaptChaBase64 { get; set; }
        public string SessionId { get; set; }
        public string SessionApp { get; set; }
        public bool IsTopup { get; set; }
        public bool IsTimeout { get; set; }
    }
}