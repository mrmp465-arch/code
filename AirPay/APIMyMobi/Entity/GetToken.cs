using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyMobi.Entity
{
    public class TokeError
    {
        public string code { get; set; }
        public string impact { get; set; }
        public string message { get; set; }
    }

    public class GetToken
    {
        public List<TokeError> errors { get; set; }
        public bool data { get; set; }
    }
}