using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyViettel.Entity
{
    public class TopupResponse
    {
        public int errorCode { get; set; }
        public string message { get; set; }
        public List<object> data { get; set; }
    }
}