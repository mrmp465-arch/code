using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyViettel.Entity
{
    public class TopupRequest
    {
        public string token { get; set; }
        public string cardcode { get; set; }
        public string phone { get; set; }
        public string captcha { get; set; }
        public string sid { get; set; }
        public int type { get; set; }
    }

    public class CheckCardRequest
    {
        public string token { get; set; }
        public string serial { get; set; }
        public string captcha { get; set; }
        public string sid { get; set; }

    }

}