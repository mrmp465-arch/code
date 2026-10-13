using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyMobi.Entity
{
    public class PhoneLogin
    {
        public string phone { get; set; }
        public int type { get; set; }
        public string name { get; set; }
        public int isPrimary { get; set; }
        public int receive_noti { get; set; }
    }

    public class DataLogin
    {
        public string apiSecret { get; set; }
        public string refreshKey { get; set; }
        public int userId { get; set; }
        public object avatar { get; set; }
        public int hasPass { get; set; }
        public List<PhoneLogin> phone { get; set; }
        public string name { get; set; }
    }

    public class Login
    {
        public DataLogin data { get; set; }
        public List<ErrorLogin> errors { get; set; }
    }

    public class ErrorLogin
    {
        public string code { get; set; }
        public string impact { get; set; }
        public string message { get; set; }
    }
}