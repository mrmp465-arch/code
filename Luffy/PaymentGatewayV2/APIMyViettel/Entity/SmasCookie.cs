using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace APIMyViettel.Entity
{
    [Serializable]
    public class SmasCookie
    {
        public CookieContainer CookieContainer { get; set; }
        public string RequestVerificationToken { get; set; }
        public string HtmlContent { get; set; }
        public string CaptChaLink { get; set; }
        public string CaptChaBase64 { get; set; }
        public string SessionId { get; set; }
        public bool IsTopup { get; set; }
        public bool IsTimeout { get; set; }
        //public string RdbSelectLevel { get; set; }
    }
}