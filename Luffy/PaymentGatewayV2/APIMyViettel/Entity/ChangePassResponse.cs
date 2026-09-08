using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyViettel.Entity
{
    public class ChangePassResponse
    {
        public int errorCode { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
}