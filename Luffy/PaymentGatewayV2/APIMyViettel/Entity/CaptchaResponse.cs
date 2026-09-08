using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyViettel.Entity
{
    public class CData
    {
        public string url { get; set; }
        public string sid { get; set; }
    }

    public class CaptchaResponse
    {
        public int errorCode { get; set; }
        public string message { get; set; }
        public CData data { get; set; }
    }
}