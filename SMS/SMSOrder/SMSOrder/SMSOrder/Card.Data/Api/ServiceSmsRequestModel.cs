using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Data.Api
{
    public class ServiceSmsRequestModel
    {
        public int port { get; set; }
        public long smsId { get; set; }
        public string text { get; set; }
        public string number { get; set; }
        public string sn { get; set; }
    }
}