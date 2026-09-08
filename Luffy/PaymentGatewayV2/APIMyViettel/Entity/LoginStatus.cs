using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyViettel.Entity
{
    public class LoginStatus
    {
        public int Status { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}